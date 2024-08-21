// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1600
// New tiebreaker for method overloading
// Rule #1: non-generic methods are preferred over generic methods
  

module M2 = 

    type C() = 
        static member F<'a>(x : 'a ,y : 'a) = 'a'
        static member F(x : obj, y : obj) = 'b'

    let res = C.F(1,1) = 'b'

    if res then () else failwith "Failed: 1"
