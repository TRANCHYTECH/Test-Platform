export enum TestStatus {
  Draft = 1,
  Activated = 2,
  Scheduled = 3,
  Ended = 4
}

export const GradingCriteriaConfigType =
{
  PassMask: 1,
  GradeRanges: 2
}

export enum RangeUnit
{
    Percent = 1,
    Point = 2
}

export enum InformFactor
{
    PercentageScore = 1,
    PointsScore = 2,
    Grade = 3,
    DescriptiveGrade = 4,
    CorrectAnwsers = 5,
    PassOrFailMessage = 6
}
