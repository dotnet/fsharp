// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)
//<Expects id="FS0941" span="(14,28-14,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(20,28-20,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(26,28-26,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
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

