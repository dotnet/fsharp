// Will check all filetypes are exactly the same for to staring names, except for source code
// so given 'dummy' and 'dummy2', and 'dummy.exe' and 'dummy.pdb' exist in the current directory
// this will check 'dummy.exe' and 'dummy2.exe are exactly the same, and the same for the 'pdb' files
// expects 3 args, first filename, second filename, whether files should be exactly the same
let firstFilename = fsi.CommandLineArgs.[1]
let secondFilename = fsi.CommandLineArgs.[2]
let shouldBeSame = bool.Parse fsi.CommandLineArgs.[3]

let areSame (first,second) = 
    let load = System.IO.File.ReadAllBytes
    (load first) = (load second)

let filePairsToCheck = 
    System.IO.Directory.EnumerateFiles(__SOURCE_DIRECTORY__, firstFilename + ".*") 
    |> Seq.filter (fun (s:string) -> s.EndsWith(".fs") |> not )                              // Don't check the source code
    |> Seq.map (fun (f:string) -> f, f.Replace(firstFilename,secondFilename))

let compareFiles pair =
    let correct = areSame pair = shouldBeSame
    if not correct then 
        printfn "Expected %s and %s to be %s" (fst pair) (snd pair) (if shouldBeSame then "same" else "different")
    correct

// Check all pairs of files are exactly the same
exit (if filePairsToCheck |> Seq.forall compareFiles then 0 else 1)
