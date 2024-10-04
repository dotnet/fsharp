open System.Reflection
open System.IO

let _a () =
    DependentAssembly.Say.hello "hello"

let _b () =
    AscendentAssembly.Ascendent.hello()

type localType = class end
[<EntryPoint>]
let main args =

    let references = typeof<localType>.Assembly.GetReferencedAssemblies()

    let versions =
        [| for reference in references do
            if reference.Name = "DependentAssembly" then yield "DependentAssembly-" + reference.Version.ToString()
            if reference.Name = "AscendentAssembly" then yield "AscendentAssembly-" + reference.Version.ToString()
        |]

    if (Seq.compareWith (fun a v -> if a <> v then 1 else 0) args versions) <> 0 then
        printfn "References don't match"
        printfn "Expected: %A " args
        printfn "Actual: %A "   versions
        1
    else 
        printf "TEST PASSED OK" ;
        0
