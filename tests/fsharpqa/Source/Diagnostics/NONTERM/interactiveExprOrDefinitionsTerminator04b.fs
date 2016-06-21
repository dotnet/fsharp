// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error">Unexpected keyword 'member' in type definition</Expects>

#light "off"

type foo() =
  member f.Foo with get() = 0
  and set (x: int) = ()
