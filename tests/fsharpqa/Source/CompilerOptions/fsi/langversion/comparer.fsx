// #NoMT #CompilerOptions #RequiresENU   
#light

let arg0 = System.Environment.GetCommandLineArgs().[0]
let path = System.Environment.GetEnvironmentVariable("PATH")
let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]

// Read file into an array
let File2List (filename:string) = System.IO.File.ReadAllLines(filename)

let f1 = File2List fn1
let f2 = File2List fn2

let mutable i = 0
let compare (f1:string[]) (f2:string[]) = 
    if f1.Length <> f2.Length then failwithf "Help text did not match. f1.Length = %d, f2.Length = %d. Check you have fsc on path, arg0 = %s, PATH=%s" f1.Length f2.Length arg0 path
    (f1, f2) ||> Array.forall2 (fun (a:string) (b:string) ->
         let aa = System.Text.RegularExpressions.Regex.Replace(a, @"F# Compiler version .+", "F# Compiler")
         let bb = System.Text.RegularExpressions.Regex.Replace(b, @"F# Compiler version .+", "F# Compiler")
         i <- i+1
         if (aa = bb) then
            true 
         else
            printfn "Files differ at line %d:" i
            printfn "\t>> %s" a
            printfn "\t<< %s" b
            false
       ) 

exit (if compare f1 f2 then 0 else 1)

