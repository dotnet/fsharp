// #Regression #Conformance #PatternMatching 
#light

//<Status expect=warning>FS0025: Incomplete pattern matches on this expression. The value '7' will not be matched</Status>

open System

let isWeekend day =
    match day with
    | DayOfWeek.Monday      | DayOfWeek.Tuesday | DayOfWeek.Wednesday
    | DayOfWeek.Thursday    | DayOfWeek.Friday
        -> false
    | DayOfWeek.Saturday    | DayOfWeek.Sunday
        -> true
        
exit 0
