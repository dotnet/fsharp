#nowarn "52"
open System.Runtime.CompilerServices
open System.Data
[<Extension>]
type DataSetExts =
    [<Extension>]
    static member Disposed(self: DataSet) = self

let d = System.Data.DataSet()
d.Disposed().Disposed().add_Disposed(fun a b -> ())

module Exts =
    type DataSet with
        member x.Disposed() = x
        
open Exts

d.Disposed().Disposed().add_Disposed(fun a b -> ())