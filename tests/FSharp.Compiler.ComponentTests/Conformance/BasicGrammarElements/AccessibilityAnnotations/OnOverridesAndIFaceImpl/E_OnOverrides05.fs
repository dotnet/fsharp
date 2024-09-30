// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility desc on overrides (inheritance chain)



[<AbstractClass>]
type E() = class
                abstract P : int with get
           end

[<AbstractClass>]
type F() = class
                inherit E()
           end

type G() = class
                inherit F()
                override x.P with internal get () = 1    // err
           end
