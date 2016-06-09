// #Regression #Diagnostics 
// Regression test for FSHARP1.0:4245
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error" span="(6,1-6,7)">Unexpected keyword 'member' in implementation file</Expects>

member private this.Size with  set newSize = m_size <- newSize;;