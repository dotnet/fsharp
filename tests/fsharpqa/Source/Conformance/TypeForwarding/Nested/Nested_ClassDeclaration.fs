#light
open System

let a = 1
let i = System.TimeZoneInfo.Local
let delta = TimeSpan(1,0,0)
let transitionRuleEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(DateTime(1, 1, 1, 2, 0, 0), 10, 5, DayOfWeek.Sunday)
let transitionRuleStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(DateTime(1, 1, 1, 2, 0, 0), 04, 05, DayOfWeek.Sunday)
let adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(DateTime(1976, 1, 1), DateTime(1986, 12, 31), delta, transitionRuleStart, transitionRuleEnd)

