{{MODULE_ATTRIBUTE}}
module Test

open System.Threading.Tasks
open Microsoft.FSharp.Quotations

type Context =
    | C0 of int | C1 of int | C2 of int | C3 of int
    | C4 of int | C5 of int | C6 of int | C7 of int
    | C8 of int | C9 of int | C10 of int | C11 of int
    | C12 of int | C13 of int | C14 of int | C15 of int

let trace = ResizeArray<int>()
let guard n value col = trace.Add n; col <= value + n

{{CASE}}
