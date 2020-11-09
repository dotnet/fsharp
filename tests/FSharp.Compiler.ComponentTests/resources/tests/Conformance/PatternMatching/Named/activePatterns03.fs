// #Conformance #PatternMatching #ActivePatterns 
#light

// Single case partial active pattern

let dayOfWeek = new System.Collections.Generic.List<string>(["Sun"; "Mon"; "Tue"; "Wed"; "Thu"; "Fri"; "Sat"])

let (|DayOfWeek|_|) string = if dayOfWeek.IndexOf(string) <> -1 then Some() else None

let isDayOfWeek x =
    match x with
    | DayOfWeek -> true
    | _ -> false
    
if isDayOfWeek("red") <> false then exit 1
if isDayOfWeek("Fri") <> true then exit 1

exit 0
