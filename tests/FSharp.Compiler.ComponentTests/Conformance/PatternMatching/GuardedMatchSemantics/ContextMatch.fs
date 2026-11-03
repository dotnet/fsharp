{{MODULE_ATTRIBUTE}}
module Test

open System.Threading.Tasks
open Microsoft.FSharp.Quotations

type Context = {{CONTEXTS}}

let trace = ResizeArray<int>()
let guard n value col = trace.Add n; col <= value + n

{{CASE}}
