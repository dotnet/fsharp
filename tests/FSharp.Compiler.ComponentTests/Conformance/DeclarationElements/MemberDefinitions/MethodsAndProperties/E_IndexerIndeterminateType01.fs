// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for TFS#903376
//<Expects status="error" span="(5,11-5,16)" id="FS0752">The operator 'expr\.\[idx\]' has been used on an object of indeterminate type based on information prior to this program point\. Consider adding further type constraints$</Expects>

let f x = x.[1]
