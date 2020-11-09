// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1243
// Deprecate OCaml-compat polymorphic record fields
//<Expects id="FS0010" span="(6,21-6,22)" status="error">Unexpected quote symbol in field declaration$</Expects>

type t = { id : 'a. 'a -> 'a }

