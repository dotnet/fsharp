namespace Microsoft.FSharp.Compatibility

    open System

    [<RequireQualifiedAccess>]
    module Seq = 


        [<Obsolete("This function will be removed in a future release. Use a sqeuence expression instead")>]
        val generate   : opener:(unit -> 'b) -> generator:('b -> 'T option) -> closer:('b -> unit) -> seq<'T>

        [<Obsolete("This function will be removed in a future release. If necessary, take a copy of its implementation from the F# PowerPack and copy it into your application")>]
        val generate_using   : opener:(unit -> ('T :> IDisposable)) -> generator:('T -> 'b option) -> seq<'b>
