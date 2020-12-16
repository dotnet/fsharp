// #Conformance #PatternMatching #ActivePatterns 
#light

// Match against an enum const defined in another assembly
open System

let isWeekend day =
    match day with
    | DayOfWeek.Sunday | DayOfWeek.Saturday -> true
    | DayOfWeek.Monday | DayOfWeek.Tuesday
    | DayOfWeek.Wednesday | DayOfWeek.Thursday
        -> false
    | DayOfWeek.Friday -> false
    | _ -> failwith "Invalid value"
    
    
if isWeekend DayOfWeek.Saturday <> true then exit 1

exit 0
    
    
