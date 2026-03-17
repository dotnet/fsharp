// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4135
// We want to make sure that this call succeeds (typically if tail calls are not enabled,
// this would yield an out of memory)
let rec rwalk x = seq { yield x; yield! rwalk (x+1) }
rwalk 3 |> Seq.truncate 10000000 |> Seq.iter (fun _ -> ())
exit 0
