// #Conformance #PatternMatching 
#light

open System

let abbreviate (ex:#obj) =
    match box ex with
    | :? NotImplementedException    -> "nyi"
    | :? ArgumentException          -> "ae"
    | :? Exception                  -> "e"
    | _                             -> ""
    
if abbreviate (new NotImplementedException "") <> "nyi" then exit 1
if abbreviate (new ArgumentException "") <> "ae" then exit 1
if abbreviate (new Exception "") <> "e" then exit 1
if abbreviate ("foo") <> "" then exit 1

exit 0
