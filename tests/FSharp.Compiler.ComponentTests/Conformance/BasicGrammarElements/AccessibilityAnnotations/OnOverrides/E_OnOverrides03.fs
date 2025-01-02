// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)




[<AbstractClass>]
type D() = class
                abstract P : int with get, set
           end

type D1a() = class
                inherit D()
                override x.P  with get () = 1
                              and public set (a : int) = ()
            end

type D2a() = class
                inherit D()
                override x.P  with get () = 1
                              and private set (a : int) = ()
            end
            
type D3a() = class
                inherit D()
                override x.P  with get () = 1
                              and internal set (a : int) = ()
            end
