// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:2435 (ICE on duplicate property definitions.)
// See also bug FSHARP1.0:4925 (this test will have to be updated once that bug is resolved)
//<Expects id="FS0438" span="(10,18-10,29)" status="error">Duplicate method\. The method 'get_Property001' has the same name and signature as another method in type 'NM.ClassMembers'</Expects>
module NM = 
    type ClassMembers ()  = 
     let mutable adjustableInstanceValue = "3"
     member this.Property001
            with get()         = adjustableInstanceValue
     member this.Property001
            with get()         = adjustableInstanceValue
