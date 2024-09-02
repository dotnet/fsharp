namespace Xunit

open Xunit
open Xunit.Sdk
open System.Threading
open System.Globalization

module Assert =

    [<assembly: CollectionBehavior(DisableTestParallelization = true)>]
    do()

    let inline fail message = Assert.Fail message

    let inline failf fmt = Printf.kprintf fail fmt

    let inline areEqual (expected: ^T) (actual: ^T) =
        Assert.Equal<^T>(expected, actual)

    // let inline contains (expected: string) (actual: string) = Assert.Contains(expected, actual)

    //let inline doesNotThrow (action: unit -> unit) =
    //    Record.Exception action
    //    |> Assert.Null


