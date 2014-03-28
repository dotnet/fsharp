// #Conformance #PatternMatching #TypeTests 
#light

let listOfWhat (x : 'a list) =
    match box x with
    | :? List<int>    -> "int list"
    | :? List<string> -> "string list"
    | :? List<obj>    -> "obj list"
    | _ -> "unknown"

if listOfWhat ["foo"]     <> "string list" then exit 1
if listOfWhat [1 .. 10]   <> "int list"    then exit 2
if listOfWhat [([]:>obj)] <> "obj list"    then exit 3
if listOfWhat [  ]        <> "obj list"    then exit 4

exit 0
