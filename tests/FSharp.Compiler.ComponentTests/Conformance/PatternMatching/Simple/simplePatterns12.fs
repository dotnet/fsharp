// #Conformance #PatternMatching 
#light

type Person = { Name : string; Age : int }

type Classification =
    | Child
    | Adult
    | Senior
    
let getClass person =
    match person with
    | { Name = _; Age = age} when age < 12 -> Child
    | { Name = _; Age = age} when age > 55 -> Senior
    | { Name=_; Age=_} -> Adult

if getClass {Name="Uncle Simpson"; Age=70} <> Senior then exit 1
if getClass {Name="Homer"; Age=40} <> Adult then exit 1
if getClass {Name="Lisa";  Age=11} <> Child then exit 1

exit 0
