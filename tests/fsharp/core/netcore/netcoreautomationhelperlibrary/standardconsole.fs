// fake desktop module for printf, printfn, and exit function

[<AutoOpen>]
module DesktopModule

[<AutoOpen>]
module stderr = 
    open System.Collections.Generic

    let Write(str) = Hooks.write.Invoke(str)
    let WriteLine(str) = Write (sprintf "%s\r\n" str)
    
    let Flush _ = ()

// console printing functions don't exist in portable, so we fake them here
[<AutoOpen>]
module stdout = 

    let Write(str) = Hooks.write.Invoke(str)
    let WriteLine(str) = Write (sprintf "%s\r\n" str)
     
    open System
    type FSV = Microsoft.FSharp.Reflection.FSharpValue
    type FST = Microsoft.FSharp.Reflection.FSharpType

    let notImpl<'T> : 'T = raise (NotImplementedException())

    let printfn (fmt : Printf.TextWriterFormat<'T>) : 'T = 
        let rec chain (ty : System.Type) : obj = 
            if FST.IsFunction ty then
                let argTy, retTy = FST.GetFunctionElements ty
                FSV.MakeFunction(ty, (fun x -> 
                                        WriteLine (sprintf "    [%A]" x)
                                        chain retTy))
            else
                if ty.IsValueType then Activator.CreateInstance(ty) else null

        WriteLine (fmt.Value)
        chain typeof<'T> :?> 'T

    let eprintf (fmt : Printf.TextWriterFormat<'T>) : 'T = 
        let rec chain (ty : System.Type) : obj = 
            if FST.IsFunction ty then
                let argTy, retTy = FST.GetFunctionElements ty
                FSV.MakeFunction(ty, (fun _ -> chain retTy))
            else
                if ty.IsValueType then Activator.CreateInstance(ty) else null

        chain typeof<'T> :?> 'T

    let printf fmt = printfn fmt

    let Flush _ = ()

// many tests complete with an "exit" statement, which doesn't exist in portable
// workaround is to redefine "exit" as a global function that either does nothing (exit 0) or throws an exception (exit n, n <> 0)
[<AutoOpen>]
module control = 
    exception Exit of int
    let exit n = 
        if n = 0 then ()
        else raise (Exit(n))

module Printf = 
    let printf a b = stdout.Write(sprintf a b)