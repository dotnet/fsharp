// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1600
// New tiebreaker for method overloading
// Rule #1: non-generic methods are preferred over generic methods
module M1 =
   type C() = class
                 static member F<'a> (x : 'a) = 1

                 static member F (x : int) = 1.1
              end
   
   let c = C.F('a') = 1  // ok; c is 1
   let d = C.F(1) = 1.1  // ok; d is 1.1

   if c && d then () else failwith "Failed: 1"
   
