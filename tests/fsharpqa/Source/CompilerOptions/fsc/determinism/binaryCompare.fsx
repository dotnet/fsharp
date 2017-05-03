// sgiven 'dummy' and 'dummy2', and 'dummy.exe' and 'dummy.pdb' exist in the current directory
// this will check 'dummy.exe' and 'dummy2.exe are exactly the same, and the same for the 'pdb' files
// expects 1 arg: whether files should be exactly the same
let shouldBeSame = bool.Parse fsi.CommandLineArgs.[1]

let areSame (first,second) = 
    let load = System.IO.File.ReadAllBytes
    (load first) = (load second)

let filePairsToCheck = 
    System.IO.Directory.EnumerateFiles(__SOURCE_DIRECTORY__, "dummy.*") 
    |> Seq.filter (fun s -> s.EndsWith(".fs") |> not )                              // Don't check the source code
    |> Seq.map (fun f -> f, f.Replace("dummy","dummy2"))

let compareFiles pair =
    let correct = areSame pair = shouldBeSame
    if not correct then 
        printfn "Expected %s and %s to be %s" (fst pair) (snd pair) (if shouldBeSame then "same" else "different")
    correct

// Check all pairs of files are exactly the same
exit (if filePairsToCheck |> Seq.forall compareFiles then 0 else 1)
