// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)



[<AbstractClass>]
type D() = class
                abstract P : int with get, set
           end

type D1() = class
                inherit D()
                override x.P  with public get () = 1
                              and set (a : int) = ()
            end

type D2() = class
                inherit D()
                override x.P  with private get () = 1
                              and set (a : int) = ()
            end
            
type D3() = class
                inherit D()
                override x.P  with internal get () = 1
                              and set (a : int) = ()
            end

