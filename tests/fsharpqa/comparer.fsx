// #NoMT #CompilerOptions #RequiresENU   
#light
open System
open System.IO
open System.Text.RegularExpressions

let arg0 = Environment.GetCommandLineArgs().[0]
let path = Environment.GetEnvironmentVariable("PATH")
let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]

// Read file into an array
let File2List (filename:string) = File.ReadAllLines(filename)

let f1 = File2List fn1
let f2 = File2List fn2

let mutable i = 0
let compare (f1:string[]) (f2:string[]) = 
    if f1.Length <> f2.Length then failwithf "Help text did not match. f1.Length = %d, f2.Length = %d, Check you have right fsi on path. fsi = %s, PATH=%s" f1.Length f2.Length arg0 path
    (f1, f2) ||> Array.forall2 (fun (a:string) (b:string) ->

        let replace (sourcepattern:string) (replacement:string) (str:string) : string =
            Regex.Replace(str, sourcepattern, replacement)

        let normalizeText (str:string) =
            str |> replace @"F# Interactive version .+" "F# Interactive"
                |> replace @"F# Compiler version .+" "F# Compiler"
                |> replace "fsiAnyCpu.exe" "fsi.exe"

        let aa = normalizeText a
        let bb = normalizeText b

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
