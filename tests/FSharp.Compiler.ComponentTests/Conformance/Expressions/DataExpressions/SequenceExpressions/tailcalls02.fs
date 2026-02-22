// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4135
// We want to make sure that this call succeeds (typically if tail calls are not enabled,
// this would yield an out of memory)
// Same as 01, but with MUTUALLY RECURSIVE PAIR OF SEQUENCES
let rec rwalk1 x = seq { yield x; yield! rwalk2 (x+1) }
    and rwalk2 x = seq { yield x; yield! rwalk1 (x+1) }

rwalk1 3 |> Seq.truncate 10000000 |> Seq.iter (fun _ -> ())

exit 0
