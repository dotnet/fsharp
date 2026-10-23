module Consumer

open System
open System.Runtime.CompilerServices

[<Struct; IsByRefLike>]
type Small(value: int) =
    member _.Value = value
    member _.Add x = value + x

type Operations() =
    member _.Add(s: Span<int>, x) = s.Length + x

let add (s: Span<int>) x = s.Length + x
let read (s: ReadOnlySpan<int>) x = s.Length + x
let small (s: Small) x = s.Value + x
let tupled (s: Span<int>, x) y = s.Length + x + y
let later x (s: Span<int>) y = x + s.Length + y
let returning (s: Span<int>, x: int) = s.Slice(x)
let synchronous (span: Span<int>) = span[0] + span[1] + span[2]

let run () =
    let s = Provider.make ()
    let good = add (Provider.make ()) 4
    if good <> 7 || add s 4 <> 7 || Provider.add s 4 <> 7 || Provider.f s () <> 3 then
        failwith "curried application"
    if read (ReadOnlySpan<int>([| 1; 2; 3 |])) 4 <> 7 || small (Small(3)) 4 <> 7 then
        failwith "byref-like types"
    if tupled (Span<int>([| 1; 2; 3 |]), 1) 2 <> 6 || later 1 s 2 <> 6 then
        failwith "argument groups"
    let mutable instance = Small(3)
    if Operations().Add(s, 4) <> 7 || instance.Add 4 <> 7 then
        failwith "member application"
    if synchronous (Provider.make ()) <> 6 || add (returning (Provider.make (), 1)) 4 <> 6 then
        failwith "synchronous and span-returning application"
    let tail = returning (s, 1)
    if tail.Length <> 2 || add Span<int>.Empty 4 <> 4 then
        failwith "computed local and property"
    let ordinary x y = x + y
    let text x y = x + y
    let measured (x: float<1>) y = x + y
    if (ordinary 3) 4 <> 7 || (text "a") "b" <> "ab" || (measured 3.0) 4.0 <> 7.0 then
        failwith "ordinary partial application"
    Provider.make ()
    let _ = Provider.make ()
    printfn "Issue 20803 controls passed"

run ()
