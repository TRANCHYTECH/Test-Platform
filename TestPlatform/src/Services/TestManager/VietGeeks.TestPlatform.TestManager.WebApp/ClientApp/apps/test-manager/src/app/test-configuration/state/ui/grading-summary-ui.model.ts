
export const GradeType =
{
  Grade: 1,
  Descriptive: 2,
  GradeAndDescriptive: 3
}

export const GradeTypeUI =
{
  Grade: GradeType.Grade.toString(),
  Descriptive: GradeType.Descriptive.toString(),
  GradeAndDescriptive: GradeType.GradeAndDescriptive.toString()
}

export const GradingCriteriaConfigType =
{
  PassMask: 1,
  GradeRanges: 2
}

export const GradingCriteriaConfigTypeUI =
{
  PassMask: GradingCriteriaConfigType.PassMask.toString(),
  GradeRanges: GradingCriteriaConfigType.GradeRanges.toString()
}

export const RangeUnit =
{
  Percent: 1,
  Point: 2
}

export const RangeUnitUI =
{
  Percent: RangeUnit.Percent.toString(),
  Point: RangeUnit.Point.toString()
}

export const InformFactor =
{
  // General: control id, enabled state.
  PercentageScore: 1,
  PointsScore: 2,
  CorrectAnwsers: 3,
  // Grade ranges.
  Grade: 4,
  DescriptiveGrade: 5,
  // Grade Pass mask
  PassOrFailMessage: 6
}

export const InformFactorUI =
{
  // General
  PercentageScore: InformFactor.PercentageScore.toString(),
  PointsScore: InformFactor.PointsScore.toString(),
  CorrectAnwsers: InformFactor.CorrectAnwsers.toString(),
  // Grade ranges.
  Grade: InformFactor.Grade.toString(),
  DescriptiveGrade: InformFactor.DescriptiveGrade.toString(),
  // Grade Pass mask
  PassOrFailMessage: InformFactor.PassOrFailMessage.toString()
}

export const RangeDetailsUI: {
  [key: number]: {
    enabled?: string[],
    disabled?: string[]
  }
} =
{
  [GradeType.Grade]: {
    enabled: [GradeTypeUI.Grade],
    disabled: [GradeTypeUI.Descriptive]
  },
  [GradeType.Descriptive]: {
    enabled: [GradeTypeUI.Descriptive],
    disabled: [GradeTypeUI.Grade]
  },
  [GradeType.GradeAndDescriptive]: {
    enabled: [GradeTypeUI.Grade, GradeTypeUI.Descriptive]
  }
}

export const InformFactorCriteriaUI: {
  [key: string]: {
    enabled?: string[],
    disabled?: string[]
  }
} =
{
  [GradingCriteriaConfigTypeUI.PassMask]: {
    enabled: [InformFactorUI.PassOrFailMessage]
  },
  [GradingCriteriaConfigTypeUI.GradeRanges + "_" + GradeTypeUI.Grade]: {
    enabled: [InformFactorUI.Grade],
    disabled: [InformFactorUI.DescriptiveGrade]
  },
  [GradingCriteriaConfigTypeUI.GradeRanges + "_" + GradeTypeUI.Descriptive]: {
    enabled: [InformFactorUI.DescriptiveGrade],
    disabled: [InformFactorUI.Grade]
  },
  [GradingCriteriaConfigTypeUI.GradeRanges + "_" + GradeTypeUI.GradeAndDescriptive]: {
    enabled: [InformFactorUI.Grade, InformFactorUI.DescriptiveGrade]
  }
}