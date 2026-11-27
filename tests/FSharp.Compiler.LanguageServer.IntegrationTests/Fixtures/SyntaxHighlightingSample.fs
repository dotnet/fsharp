// Sample F# source exercising every semantic and syntactic classification the LSP server emits.
// Used for manual spot-checks of syntax highlighting in VS / VS Code and as a fixture for
// integration tests. Keep it warning-free.

module FSharp.Samples.Highlighting

open System
open System.IO
open System.Runtime.CompilerServices

/// Top-level literal -> "constant name".
[<Literal>]
let MaxItems = 100

/// Top-level non-disposable, non-literal binding -> "variable" (FCS Value).
let pi = 3.14159

[<Struct>]
type Vector = { X: float; Y: float }

[<Struct>]
type Point(x: int, y: int) =
    member _.X = x
    member _.Y = y

type Color =
    | Red = 0
    | Green = 1
    | Blue = 2

type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

type Person =
    { Name: string
      mutable Age: int
      Greet: string -> string }

type IShape =
    abstract member Area: float
    abstract member Describe: prefix: string -> string

type Greeter = delegate of string -> unit

exception InvalidShape of string

type Counter() =
    let mutable count = 0
    member _.Count = count
    member _.Increment() = count <- count + 1

[<Extension>]
type StringExtensions =
    [<Extension>]
    static member Shout(s: string) = s.ToUpper()

module Sample =

    let identity<'T> (x: 'T) = x

    let inline (++) (a: int) (b: int) = a + b

    let greet (name: string) =
        printfn "Hello, %s (%d chars)" name name.Length

    let openFile () =
        let stream = new FileStream("local.txt", FileMode.Create)
        stream.Close()

    let topStream = new FileStream("top.txt", FileMode.Create)

    let counter = Counter()
    let p = Point(1, 2)

    let pipeline () =
        async {
            let! n = async.Return 42
            return n + 1
        }

    let nameOfShape (s: Shape) = nameof s

    let area (s: Shape) =
        match s with
        | Circle r -> r * r * Math.PI
        | Rectangle (w, h) -> w * h
