// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:2439
// fsi.CommandLineArgs
// scenario: no arguments
(if ((Seq.length fsi.CommandLineArgs) <> 1) then 1 else 0) |> exit;;

