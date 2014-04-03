// #NoMT #CompilerOptions #RequiresENU #NETFX20Only #NETFX40Only 
#light

let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]

// Read file into an array
let File2List (filename:string) = 
    use s = new System.IO.StreamReader(filename)
    let mutable l = []
    while not s.EndOfStream do
        l <- List.append l ( s.ReadLine() :: [])
    l

let f1 = File2List fn1
let f2 = File2List fn2

let mutable i = 0
let compare (f1:string list) (f2:string list) = List.forall2 (fun (a:string) (b:string) ->
                                                                         let aa = System.Text.RegularExpressions.Regex.Replace(a, @"F# Interactive build .+", "F# Interactive build")
                                                                         let bb = System.Text.RegularExpressions.Regex.Replace(b, @"F# Interactive build .+", "F# Interactive build")

                                                                         // unify fsi.exe and FsiAnyCPU.exe
                                                                         let aa = aa.Replace("FsiAnyCPU.exe", "fsi.exe")
                                                                         let bb = aa.Replace("FsiAnyCPU.exe", "fsi.exe")

                                                                         i <- i+1
                                                                         if (aa = bb) then
                                                                            true 
                                                                         else
                                                                            printfn "Files differ at line %d:" i
                                                                            printfn "\t>> %s" a
                                                                            printfn "\t<< %s" b
                                                                            false
                                                                       ) f1 f2

exit (if compare f1 f2 then 0 else 1)
