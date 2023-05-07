import { Injectable, inject } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { AppSettingsService, UserProfileService } from "@viet-geeks/core";
import { TestCategory, TestCategoryUncategorizedId } from '../../../_state/test-category.model';
import { keys, sumBy } from "lodash-es";
import { AppSettings } from "../../../app-setting.model";
import { QuestionSummary } from "../../_state/questions/question.model";
import {
    BasicSettings,
    CompleteQuestionDuration,
    CompleteTestDuration,
    GeneratorTypes,
    GradeRangeCriteria,
    GradingSettings,
    ManualTestActivation,
    PassMaskCriteria,
    Test,
    TestAccess,
    TestAccessType,
    TestActivationMethod,
    TestSets,
    TimePeriodActivation,
    TimeSettings
} from "../../_state/tests/test.model";
import { GradingCriteriaConfigType, RangeUnit } from "../../_state/ui/grading-summary-ui.model";
import { TestConfigRoutes } from "../test-configuration-routing.module";
import { TestActivationMethodType, TestDurationMethod } from "../test-time-settings/test-time-settings.component";

export type Summary = { desc: string, status: 'ok' | 'action', url?: string };

@Injectable({ providedIn: 'root' })
export class SummaryBuilder {
    private _translate = inject(TranslateService);
    private _appSettingsService = inject(AppSettingsService);
    private _userProfileService = inject(UserProfileService);

    buildSummaryForDaftTest(test: Test, testCategories: TestCategory[], questionSummary: QuestionSummary[]) {
        return [
            this.getBasicSettingSummaryForDraftTest(test.basicSettings, testCategories),
            this.getQuestionSummaryForDraftTest(questionSummary),
            this.geTestSetSummary(test.testSetSettings, questionSummary),
            this.getGradingSummaryForDaftTest(test.gradingSettings),
            this.getTestAccessSummaryForDraftTest(test.testAccessSettings),
            this.getTimeSummaryForDraftTest(test.timeSettings)
        ];
    }

    buildSummaryForActiveTest(test: Test, questionSummary: QuestionSummary[]) {
        return [
            this.getTimeSummaryForActiveTest(test.timeSettings),
            this.getTestAccessSummaryForActiveTest(test.testAccessSettings, test.id),
            this.geTestSetSummary(test.testSetSettings, questionSummary)
        ];
    }

    buildSummaryForScheduledTest(test: Test) {
        // Test access: Private access code
        // Test will activate at 2023-04-21 17:14 BDT. Respondents will be able to start taking it until 2025-04-27 17:14 BDT. Current server time is 2023-04-11 21:06 BDT.
        // Number of questions in the test: 7

        return [
            this.getTestAccessSummaryForDraftTest(test.testAccessSettings),
            this.getTimeSummaryForDraftTest(test.timeSettings)
        ];
    }

    private getTimeSummaryForDraftTest(part: TimeSettings): Summary {
        let testDuration = '';
        if (part.testDurationMethod.$type === TestDurationMethod.CompleteTestTime) {
            const completeTestDuration = part.testDurationMethod as CompleteTestDuration;
            testDuration = this._translate.instant('summary.testTimeLimit', { duration: completeTestDuration.duration });
        } else if (part.testDurationMethod.$type === TestDurationMethod.CompleteQuestionTime) {
            const completeQuestionDuration = part.testDurationMethod as CompleteQuestionDuration;
            testDuration = this._translate.instant('summary.questionTimeLimit', { duration: completeQuestionDuration.duration });
        }

        // Test will activate immediately after all settings are confirmed. Time limit for each question is set to 4 min
        switch (part.testActivationMethod.$type) {
            case TestActivationMethodType.ManualTest:
                {
                    return {
                        url: this.getConfigRouteLink(TestConfigRoutes.TimeSettings),
                        status: 'ok',
                        desc: `${this._translate.instant('summary.manualActivation')}. ${testDuration}`
                    }
                }
            case TestActivationMethodType.TimePeriod: {
                const timePeriod = part.testActivationMethod as TimePeriodActivation;
                return {
                    url: this.getConfigRouteLink(TestConfigRoutes.TimeSettings),
                    status: 'ok',
                    desc: `${this._translate.instant('summary.timePeriodActivation', { activeFromDate: this._userProfileService.convertUtcToLocalDateString(timePeriod.activeFromDate), timeZone: this._userProfileService.currentTimeZone })}. ${testDuration}.`
                };
            }

            default:
                throw Error('not implemented yet');
        }
    }

    private getTestAccessSummaryForDraftTest(part: TestAccess): Summary {
        switch (part.settings.$type) {
            case TestAccessType.PublicLink:
                return {
                    url: this.getConfigRouteLink(TestConfigRoutes.TestAccess),
                    status: 'ok',
                    desc: this._translate.instant('summary.publicLinkTest')
                }
            case TestAccessType.PrivateAccessCode:
                //todo: in case there is no access code. this msg: Add respondents or change test access type.
                return {
                    url: this.getConfigRouteLink(TestConfigRoutes.TestAccess),
                    status: 'ok',
                    desc: this._translate.instant('summary.privateAccessCodeTest')
                }
            default:
                throw Error('not implemented yet');
        }
    }

    private getGradingSummaryForDaftTest(part: GradingSettings): Summary {
        if (keys(part.gradingCriterias).length === 0) {
            return {
                url: this.getConfigRouteLink(TestConfigRoutes.GradingAndSummary),
                status: 'ok',
                desc: "Grading criteria haven't been set. You can do this later when you know test results"
            };
        } else {
            const passMarkGrade = part.gradingCriterias[GradingCriteriaConfigType.PassMask] as PassMaskCriteria;
            const rangesGrade = part.gradingCriterias[GradingCriteriaConfigType.GradeRanges] as GradeRangeCriteria;
            const passMarkMsg = passMarkGrade !== undefined ? `Test pass mark: ${passMarkGrade.value} ${RangeUnit.Percent === passMarkGrade.unit ? 'Percent' : 'Point'}. ` : '';
            const gradeRangesMsg = rangesGrade != undefined ? `Grading is based on ${rangesGrade.details.length} ${RangeUnit.Percent === rangesGrade.unit ? 'Percent' : 'Point'} range` : '';
            return {
                url: this.getConfigRouteLink(TestConfigRoutes.GradingAndSummary),
                status: 'ok',
                desc: `${passMarkMsg}${gradeRangesMsg}`
            };
        }
    }

    private getQuestionSummaryForDraftTest(questionSummary: QuestionSummary[]): Summary {
        const questionCount = sumBy(questionSummary, c => c.numberOfQuestions);
        if (questionCount === 0) {
            return {
                url: this.getConfigRouteLink(TestConfigRoutes.ManageQuestions),
                status: 'action',
                desc: 'Add questions to the test.'
            };
        } else {
            return {
                url: this.getConfigRouteLink(TestConfigRoutes.ManageQuestions),
                status: 'ok',
                desc: `${questionCount} questions have been created.`
            };
        }
    }

    private getBasicSettingSummaryForDraftTest(part: BasicSettings, testCategories: TestCategory[]): Summary {
        if (part.category === TestCategoryUncategorizedId) {
            return {
                url: this.getConfigRouteLink(TestConfigRoutes.BasicSettings),
                status: 'ok',
                desc: 'You can assign this test to a caterory.'
            };
        } else {
            return {
                url: this.getConfigRouteLink(TestConfigRoutes.BasicSettings),
                status: 'ok',
                desc: `Your test belongs to the category ${testCategories.find(c => c.id === part.category)?.name}.`
            };
        }
    }

    private getConfigRouteLink(path: string) {
        return `../${path}`;
    }

    private getTestTimeUtc(config: TestActivationMethod): { startAt?: Date, endAt: Date } {
        switch (config.$type) {
            case TestActivationMethodType.ManualTest:
                {
                    const method = config as ManualTestActivation;
                    return { startAt: undefined, endAt: method.activeUntilDate }
                }
            case TestActivationMethodType.TimePeriod:
                {
                    const method = config as TimePeriodActivation;
                    return { startAt: method.activeFromDate, endAt: method.activeUntilDate }
                }
            default:
                throw Error('not supported config');
        }
    }

    private getTimeSummaryForActiveTest(part: TimeSettings): Summary {
        const testTime = this.getTestTimeUtc(part.testActivationMethod);

        return {
            status: 'ok',
            desc: `Test access will close on: ${this._userProfileService.convertUtcToLocalDateString(testTime.endAt)} (${this._userProfileService.currentTimeZone}). Current server time is: ${this._userProfileService.currentUserTime} (${this._userProfileService.currentTimeZone}).`
        };
    }

    private getTestAccessSummaryForActiveTest(part: TestAccess, testId: string): Summary {
        switch (part.settings.$type) {
            case TestAccessType.PublicLink:
                return { desc: 'Test access: PublicLink', status: 'ok', url: `${this._appSettingsService.get<AppSettings>().testRunnerBaseUrl}/test/${testId}` };
            case TestAccessType.PrivateAccessCode:
                return { desc: 'Test access: PrivateAccessCode', status: 'ok' };
            default:
                throw Error('not implemented yet');
        }
    }

    private geTestSetSummary(part: TestSets, questionSummary: QuestionSummary[]): Summary {
        switch (part.generatorType) {
            case GeneratorTypes.Default:
                return {
                    url: this.getConfigRouteLink(TestConfigRoutes.TestSets),
                    status: 'ok',
                    desc: ' Fixed order of questions and answers enabled.'
                };
            case GeneratorTypes.RandomFromCategories:
                {
                    //todo: this summary lead to discussion, confirm solution how to get draw number, in case there is no select by user.
                    const totalDraws = sumBy(part.generator?.configs ?? [], c => c.draw);
                    const questionCount = sumBy(questionSummary, c => c.numberOfQuestions);
                    return {
                        url: this.getConfigRouteLink(TestConfigRoutes.TestSets),
                        status: 'ok',
                        desc: `Random order of questions and answers enabled. Each respondent will get ${totalDraws} out of ${questionCount} available questions.`
                    };
                }
            default:
                throw Error('not implemented yet');
        }
    }
}