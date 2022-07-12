// Regression test for DevDiv:20718
// The following code was triggering a Watson error
// Watson Clr20r3 across buckets with: Application fsc.exe from Dev10 RTM; Exception system.exception; UNKNOWN!FSharp.Compiler.TypedTreeOps.dest_fun_typ

let main() =
    let (@@) = id

    [1;2;3;4;5]
    |> Seq.map @@ fun x -> x * x
    |> ignore

    ["a";"b";"c";"d"]
    |> Seq.map @@ fun x -> x + x
    |> Seq.iter @@ printf "%s"

main()
