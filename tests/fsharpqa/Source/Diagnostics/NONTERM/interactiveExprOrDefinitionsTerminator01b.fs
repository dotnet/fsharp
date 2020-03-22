// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects status="notin">NONTERM</Expects>
//<Expects span="(6,3)" status="error" id="FS0010">Unexpected symbol ';;' in expression\. Expected '\]' or other token\.$</Expects>
//<Expects id="FS0598" span="(6,1)" status="error">Unmatched '\['$</Expects>
[1;;2]
