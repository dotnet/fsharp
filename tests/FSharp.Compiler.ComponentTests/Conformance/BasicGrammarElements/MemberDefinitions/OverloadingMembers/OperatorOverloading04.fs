// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression for Dev10 bug #860557
// Overloading (+) does not work properly when first argument is a string
module M

type Step = Step of string with
    static member (+)(s, Step t) = s + t
    static member (+)(Step s, t) = s + t
let str1 = "a" + (Step "b") // should be no error
let str2 = (Step "a") + "b" 


 
type Step2 = Step2 of int with
    static member (+)(s, Step2 t) = s + t
let int3 = 1 + Step2 2 

if str1 <> "ab" then failwith "Failed: 1"
if str2 <> "ab" then failwith "Failed: 2"
if int3 <> 3 then failwith "Failed: 3"

