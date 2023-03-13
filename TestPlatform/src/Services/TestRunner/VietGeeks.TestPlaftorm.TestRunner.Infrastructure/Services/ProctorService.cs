using System;
using ListShuffle;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ProctorService : IProctorService
{
    private readonly DBContext _dbContext;

    public ProctorService(DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VerifyTestResultViewModel> VerifyTest(VerifyTestViewModel viewModel)
    {
        if (!string.IsNullOrEmpty(viewModel.TestId))
        {
            var test = await _dbContext.Find<TestDefinition>().Match(c => c.ID == viewModel.TestId && c.Status == TestDefinitionStatus.Activated).ExecuteFirstAsync();
            return test == null
                ? new VerifyTestResultViewModel
                {
                    IsValid = false
                }
                : new VerifyTestResultViewModel
                {
                    TestId = test.ID,
                    IsValid = true
                };
        }

         if (!string.IsNullOrEmpty(viewModel.AccessCode))
        {
            // Use raw query.
            var pipeline = new Template<TestDefinition>(@"
                [
                    {
                        '$match': {
                            'Status': <status>,
                            'TestAccessSettings.Settings._t': 'PrivateAccessCodeType', 
                            'TestAccessSettings.Settings.Configs.Code': '<access_code>'
                        }
                    }
                ]
            ")
                .Tag("status", $"{((int)TestDefinitionStatus.Activated)}")
                .Tag("access_code", viewModel.AccessCode);
            var test = await DB.PipelineSingleAsync(pipeline);
            //todo: move condition to db also.
            return test == null
                ? new VerifyTestResultViewModel
                {
                    IsValid = false
                }
                : new VerifyTestResultViewModel
            {
                TestId = test.ID,
                AccessCode = viewModel.AccessCode,
                IsValid = true
            };
        }

        throw new Exception("Input not valid");
    }

    public async Task<string> ProvideExamineeInfo(ProvideExamineeInfoInputViewModel viewModel)
    {
        var existingExam = await DB.Find<Exam>().Match(c => c.TestId == viewModel.TestId && c.AccessCode == viewModel.AccessCode).ExecuteFirstAsync();

        if (existingExam != null)
        {
            return existingExam.ID;
        }

        var exam = new Exam
        {
            TestId = viewModel.TestId,
            AccessCode = viewModel.AccessCode,
            ExamineeInfo = viewModel.ExamineeInfo
        };

        await DB.InsertAsync(exam);

        return exam.ID;
    }

    public async Task<List<dynamic>> GenerateExamContent(GenerateExamContentInput input)
    {
        // Get test defintion and validate it
        var testDefinition = await DB.Find<TestDefinition>().MatchID(input.TestDefinitionId).ExecuteSingleAsync();
        if(testDefinition == null)
        {
            throw new EntityNotFoundException(input.TestDefinitionId, nameof(TestDefinition));
        }

        // Get questions of test definition.
        var questions = await DB.Find<QuestionDefinition>().ManyAsync(c => c.TestId == input.TestDefinitionId);

        var rs = new List<dynamic>();
        if(!string.IsNullOrEmpty(input.AccessCode) && questions != null)
        {
           if (testDefinition.TestSetSettings?.GeneratorType == TestSetGeneratorType.Default)
            {
                questions.Shuffle();

                rs.AddRange(questions);
            }
           else if(testDefinition.TestSetSettings?.GeneratorType == TestSetGeneratorType.RandomByCategories)
            {
                var configs = testDefinition.TestSetSettings.Generator as RandomFromCategoriesGenerator;
                if(configs == null)
                {
                    throw new Exception("");
                }
                foreach (var g in questions.GroupBy(c => c.CategoryId))
                {
                    var qs = g.ToList();
                    qs.Shuffle();
                    var foundConfig = configs.Configs.FirstOrDefault(c => c.QuestionCategoryId == g.Key);
                    if(foundConfig != null)
                    {
                        //todo: check out of range.
                        rs.AddRange(qs.GetRange(0, foundConfig.DrawNumber));
                    }
                }
            }
        }

        return rs;
    }
}

