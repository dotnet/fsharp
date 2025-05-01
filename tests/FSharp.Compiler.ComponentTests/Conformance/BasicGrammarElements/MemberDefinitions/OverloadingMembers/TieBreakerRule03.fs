// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1600
// New tiebreaker for method overloading
// Rule #3: methods have same # of args and each arg type subsumes the other

module M4 = 
    type A() = class
               end

    type B() = class
                inherit A()
               end

    type C() = class

                 static member F (x : B) = 1

                 static member F (x : A) = 1.1
               end

    let r1 = C.F(new B()) = 1
    let r2 = C.F(new A()) = 1.1

    if r1 && r2 then () else failwith "Failed: 1"
