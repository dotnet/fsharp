// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on overrides (class inheritance)



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


