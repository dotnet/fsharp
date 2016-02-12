// #NoMT #CompilerOptions #RequiresENU #NETFX20Only #NETFX40Only 
let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]
let File2List(filename : string) = System.IO.File.ReadAllLines filename |> Array.toList
let f1 = File2List fn1
let f2 = File2List fn2
let mutable i = 0

let compare f1 f2 = 
    (f1,f2) ||> List.forall2 (fun a b -> 
        i <- i + 1
        if (a = b) then true
        else 
            printfn "Files differ at line %d:" i
            printfn "\t>> %s" a
            printfn "\t<< %s" b
            false) 

exit (if compare f1 f2 then 0
      else 1)

