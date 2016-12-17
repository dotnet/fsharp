[<AutoOpen>]
module Microsoft.VisualStudio.FSharp.Pervasive

open System


[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Option =

    /// Gets the value associated with the option or the supplied default value.
    let inline getOrElse v = function
        | Some x -> x | None -> v

type System.IServiceProvider with
    member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
    member x.GetService<'S, 'T>() = x.GetService(typeof<'S>) :?> 'T