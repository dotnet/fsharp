// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Regression test for FSHARP1.0:5475
//<Expects status="error" span="(6,31-6,32)" id="FS0441">Duplicate property\. The property 'M' has the same name and signature as another property in type 'C'\.$</Expects>
type C = class
                member __.M = ()
                static member M = ()
         end
