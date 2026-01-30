// #Regression #Diagnostics 
// Regression test for FSHARP1.0:4245
//<Expects id="FS0010" span="(5,1-5,7)" status="error">Unexpected keyword 'member' in implementation file</Expects>

member private this.Size with  set newSize = m_size <- newSize;;
