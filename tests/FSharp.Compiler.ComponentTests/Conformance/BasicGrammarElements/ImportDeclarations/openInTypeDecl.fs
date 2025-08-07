// #Regression #Conformance #DeclarationElements #Import 
module openInTypeDecl

type Foo() =
    open global.System
    open type DateTime
    
    inherit Object()

    [<DefaultValue>] val mutable x: Int64
    let x = 42
    let timeConstructed = Now.Ticks
    do printfn "%d" Int32.MaxValue    
    static member Now () = DateTime.Now
    member val TimeConstructed = timeConstructed with get, set

    member _.M() = 
        open type System.ArgumentException
        Int32.MaxValue

    interface IDisposable with
        member this.Dispose (): unit = 
            raise (NotImplementedException())
 
type A = A of int
    with
        open System
        member _.RandomNumber with get() = Random().Next()

type ARecord = { A : int }
    with
        open System
        member _.RandomNumber with get() = Random().Next()

exception AException of int
    with
        open System
        member _.RandomNumber with get() = Random().Next()

[<Struct>]
type ABC =
    open System
    val a: Int32
    val b: Int32
    new (a) = 
        open type System.Int32
        { a = a; b = MinValue }

type System.Int32 with
    open type System.Math
    member this.Abs111 = Abs(this)

type IA =
    open System
    open type Int32
    open System.Collections.Generic
    inherit IDisposable
    inherit IEquatable<IA>
