// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2324
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error">Unexpected symbol '\[<' in definition</Expects>

#light "off"

module M1 = 
   [<ClassAttribute>]                   // error FS0010: unexpected symbol '[<' in definition
   type T =
       static let mutable f = ()


