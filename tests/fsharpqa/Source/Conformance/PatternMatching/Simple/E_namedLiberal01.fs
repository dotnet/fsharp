// #Regression #Conformance #PatternMatching 
// Match warning when covering all defined values of an enum

//<Expects status="warning" id="FS0104">Enums may take values outside known cases.</Expects>


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
    
    
