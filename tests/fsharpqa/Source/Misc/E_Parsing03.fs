// #Regression #Misc 
// Regression test for FSharp1.0:4712
// Title: suggestion: offside syntax of match...with
// Descr: 'with' can not be at the same indentation level as 'match'
// See also DevDiv:268041 
//<Expects status="error" span="(21,3-21,4)" id="FS0010">Unexpected start of structured construct in expression$</Expects>
//<Expects status="error" span="(24,1-24,5)" id="FS0010">Incomplete structured construct at or before this point in implementation file$</Expects>
let goo x = Some(x)

let foo x =
  match
    goo x
   with
  | Some(z) -> z
  | None -> x

let foo' x =
  match
    goo x
  with
  | Some(z) -> z
  | None -> x

exit 1
