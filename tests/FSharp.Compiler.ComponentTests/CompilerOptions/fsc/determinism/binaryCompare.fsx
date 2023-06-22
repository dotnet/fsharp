let areSame (first,second) =
    let load = System.IO.File.ReadAllBytes
    (load first) = (load second)

let compareFiles shouldBeSame pair =
    let correct = areSame pair = shouldBeSame
    if not correct then 
        printfn "Expected %s and %s to be %s" (fst pair) (snd pair) (if shouldBeSame then "same" else "different")
    correct

// Check all pairs of files are exactly the same
let shouldBeSame, filePairsToCheck =
    match fsi.CommandLineArgs with
    | [| _; shouldBeSame |] ->
        // given 'dummy' and 'dummy2', and 'dummy.exe' and 'dummy.pdb' exist in the current directory
        // this will check 'dummy.exe' and 'dummy2.exe are exactly the same, and the same for the 'pdb' files
        // expects 1 arg: whether files should be exactly the same

        bool.Parse shouldBeSame, System.IO.Directory.EnumerateFiles(__SOURCE_DIRECTORY__, "dummy.*")
        |> Seq.filter (fun s -> s.EndsWith(".fs") |> not )                              // Don't check the source code
        |> Seq.map (fun f -> f, f.Replace("dummy","dummy2"))
    | args when args.Length >= 3 ->
        true, args
        |> Seq.pairwise
        |> Seq.indexed
        |> Seq.choose (fun (i, pair) -> if i % 2 = 1 then Some pair else None)
    | args ->
        failwithf "Expected '<true|false>' or '<left> <right>'"

exit (if filePairsToCheck |> Seq.forall (compareFiles shouldBeSame) then 0 else 1)
