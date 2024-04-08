// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)
//<Expects id="FS0941" span="(13,33-13,34)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(18,34-18,35)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(23,35-23,36)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
type C() = class
                abstract member M : int
           end
           
type C1() = class
              inherit C()
              override public x.M = 11     // err
            end

type C2() = class
              inherit C()
              override private x.M = 11     // err
            end

type C3() = class
              inherit C()
              override internal x.M = 11     // err
            end


