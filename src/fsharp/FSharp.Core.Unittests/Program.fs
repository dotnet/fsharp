module Program

open System
open System.Reflection
open NUnitLite

type HelperType() = inherit System.Object()

[<EntryPoint>]
let main argv =
    AutoRun().Execute(typeof<HelperType>.GetTypeInfo().Assembly, Console.Out, Console.In, argv)
