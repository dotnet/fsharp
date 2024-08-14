// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1600
// New tiebreaker for method overloading
// Rule #2: ParamArray: normal form vs expanded form
#light

module M3 = 
    type C() = class
                 static member F ([<System.ParamArray>] x : obj) = 1

                 static member F (x : obj array) = 1.1
               end
               
    let r = C.F([|1|])
    if r = 1 then () else     if r = 1 then () else failwith "Failed: 1"

