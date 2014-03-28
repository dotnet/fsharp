// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)
//<Expects id="FS0941" span="(15,28-15,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(21,28-21,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(27,28-27,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>

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
