// #Conformance #PatternMatching #Constants 
#light

// Match against an enum

open System

let isWeekend day =
    match day with
    | DayOfWeek.Sunday      | DayOfWeek.Saturday 
        -> true
    | DayOfWeek.Monday      | DayOfWeek.Tuesday
    | DayOfWeek.Wednesday   | DayOfWeek.Thursday
    | DayOfWeek.Friday 
        -> false
    | _ -> false
   
    
if isWeekend DayOfWeek.Saturday <> true then exit 1

// This is why a catch all pattern must be there...
if isWeekend (enum -42) <> false then exit 1

exit 0
