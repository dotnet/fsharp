[<AutoOpen>]
module Microsoft.VisualStudio.FSharp.Editor.Pervasive

open System

[<RequireQualifiedAccess>]
module Option =
    /// Gets the value associated with the option or the supplied default value.
    let inline getOrElse v = function
        | Some x -> x | None -> v

    /// Gets the option if Some x, otherwise try to get another value
    let inline orTry f =
        function
        | Some x -> Some x
        | None -> f()

type System.IServiceProvider with
    member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
    member x.GetService<'S, 'T>() = x.GetService(typeof<'S>) :?> 'T