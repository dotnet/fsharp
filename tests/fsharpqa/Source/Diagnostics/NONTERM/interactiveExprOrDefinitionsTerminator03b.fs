// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error">Unexpected keyword 'member' in type definition</Expects>

#light "off"

type foo() =
  member x.Bar = 0
end
