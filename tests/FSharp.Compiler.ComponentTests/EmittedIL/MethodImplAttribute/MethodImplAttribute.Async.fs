// Regression test for DevDiv:212424
// "NoInlining attribute not emitted into IL"
module M
open System.Threading.Tasks
open System.Runtime.CompilerServices
[<System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.Async)>]
let getUnit (f : unit -> Task<unit>) = AsyncHelpers.Await(f())