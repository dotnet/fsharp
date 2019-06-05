// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2369
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error" span="(8,2-8,3)">Unexpected symbol '<' in expression\. Expected '}' or other token\.
//<Expects id="FS0604" status="error" span="(8,1-8,2)"></Expects>: error : Unmatched '{'$</Expects>
#light

{<
