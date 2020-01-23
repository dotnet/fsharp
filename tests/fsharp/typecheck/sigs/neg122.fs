module Neg122

// See https://github.com/dotnet/fsharp/pull/3582#issuecomment-399755533, which listed
// this as a test case of interest.
//
// This is to pin down that behaviour doesn't change in the future unless we intend it to.
open System
type System.String  with static member inline ParseApply (path:string) (fn: string -> ^b) : ^b = fn ""
type System.Int32   with static member inline ParseApply (path:string) (fn: int   -> ^b) : ^b = fn 0
type System.Double  with static member inline ParseApply (path:string) (fn: float -> ^b) : ^b = fn 0.
type System.Boolean with static member inline ParseApply (path:string) (fn: bool -> ^b) : ^b = fn true

let inline parser (fmt:PrintfFormat< ^a -> ^b,_,_,^b>) (fn:^a -> ^b) (v:string) : ^b 
    when ^a : (static member ParseApply: string -> (^a -> ^b) -> ^b) =
    (^a : (static member ParseApply: string -> (^a -> ^b) -> ^b)(v,fn))

let inline patternTest (fmt:PrintfFormat< ^a -> Action< ^T>,_,_,Action< ^T>>) (fn:^a -> Action< ^T>) v : Action< ^T> = parser fmt fn v

let parseFn2 = patternTest "adf%s245" (fun v -> printfn "%s" v; Unchecked.defaultof<Action<unit>> ) 
