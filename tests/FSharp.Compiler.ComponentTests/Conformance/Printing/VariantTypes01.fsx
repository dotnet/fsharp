// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:1401
// incorrect pretty printing of variant types
// The issue here was the missing parens around the nested Some ...
//<Expects status="success">val it: int option option = Some \(Some 1\)</Expects>
Some(Some(1)) |> ignore
()