module Program

open System
open System.Reflection
open NUnitLite
open NUnit.Common

type HelperType() = inherit System.Object()

[<EntryPoint>]
let main argv =
    AutoRun(typeof<HelperType>.GetTypeInfo().Assembly).Execute(argv, new ExtendedTextWrapper(Console.Out), Console.In)
