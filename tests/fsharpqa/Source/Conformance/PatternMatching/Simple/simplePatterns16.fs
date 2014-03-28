// #Conformance #PatternMatching 
#light

open System

let abbreviate (ex:#obj) =
    match box ex with
    | :? NotImplementedException as e
        ->  if e.GetType() <> typeof<NotImplementedException> then
                exit 1
            "nyi"
    | :? ArgumentException as e
        ->  if e.GetType() <> typeof<ArgumentException> then
                exit 1
            "ae"
    | :? Exception as e
        ->  if e.GetType() <> typeof<Exception> then
                exit 1
            "e"
    | _                             -> ""
    
if abbreviate (new NotImplementedException "") <> "nyi" then exit 1
if abbreviate (new ArgumentException "") <> "ae" then exit 1
if abbreviate (new Exception "") <> "e" then exit 1
if abbreviate ("foo") <> "" then exit 1

exit 0
