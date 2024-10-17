// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)



[<AbstractClass>]
type D() = class
                abstract P : int with get, set
           end

type D1b() = class
                inherit D()
                override x.P  with public get () = 1
                              and public set (a : int) = ()
            end

type D2b() = class
                inherit D()
                override x.P  with private get () = 1
                              and public set (a : int) = ()
            end
            
type D3b() = class
                inherit D()
                override x.P  with internal get () = 1
                              and public set (a : int) = ()
            end
