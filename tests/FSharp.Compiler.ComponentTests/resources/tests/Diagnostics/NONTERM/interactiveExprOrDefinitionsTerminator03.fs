// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects id="FS0010" span="(6,3-6,9)" status="error">'member'</Expects>
#light "off"
type foo() =
  member x.Bar = 0
end
