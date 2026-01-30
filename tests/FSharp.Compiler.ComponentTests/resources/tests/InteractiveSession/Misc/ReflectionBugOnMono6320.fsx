// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:6320 or
// This is really a bug in Mono - see https://bugzilla.novell.com/show_bug.cgi?id=581984
//<Expects status="success">path1</Expects>
//<Expects status="success">result = 1</Expects>

let reduce gen = 
  match gen with 
  | [_; _] -> 
     printfn "path1"
     1
  | [_] -> 
     printfn "path2"
     2
  | _ -> 
     3

printfn "result = %A" (reduce  [1;2])

#q;;