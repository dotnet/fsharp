// #Regression #Conformance #PatternMatching 
// Match warning when using enum for incomplete match.
// (Even if you use every possible value.

//<Expects status="warning" id="FS0025">Incomplete pattern matches on this expression</Expects>


open System

let isWeekend day =
    match day with
    | DayOfWeek.Sunday      | DayOfWeek.Saturday 
        -> true
    | DayOfWeek.Monday      | DayOfWeek.Tuesday
    | DayOfWeek.Wednesday   | DayOfWeek.Thursday
    | DayOfWeek.Friday 
        -> false
   
    
if isWeekend DayOfWeek.Saturday <> true then exit 1

exit 0
    
    
