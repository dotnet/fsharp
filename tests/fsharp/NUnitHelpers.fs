namespace Xunit

open Xunit
open Xunit.Sdk
open System.Threading
open System.Globalization

type UseCulture(culture: string, uiCulture: string) =

    inherit BeforeAfterTestAttribute()

    let mutable restore = ignore

    new(culture) = UseCulture(culture, culture)
    override _.Before _ = 
        let originalCulture = Thread.CurrentThread.CurrentCulture
        let originalUiCulture = Thread.CurrentThread.CurrentUICulture

        Thread.CurrentThread.CurrentCulture <- CultureInfo(culture)
        Thread.CurrentThread.CurrentUICulture <- CultureInfo(uiCulture)

        restore <- fun () ->
            Thread.CurrentThread.CurrentCulture <- originalCulture
            Thread.CurrentThread.CurrentUICulture <- originalUiCulture

    override _.After _ = restore ()

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


