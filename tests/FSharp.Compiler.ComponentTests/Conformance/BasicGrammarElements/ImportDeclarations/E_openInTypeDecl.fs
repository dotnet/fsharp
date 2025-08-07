// #Regression #Conformance #DeclarationElements #Import 
module openInTypeDecl

type Foo() =
    open type System.DateTime
    
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
    open global.System

type A = A of int
    with
        member _.RandomNumber with get() = Random().Next()
        open System

type ARecord = { A : int }
    with
        member _.RandomNumber with get() = Random().Next()
        open System
        
exception AException of int
    with
        member _.RandomNumber with get() = Random().Next()
        open System
        
type B = A of Int32
    with
        open System

exception BException of Int32
    with
        open System
        
type BRecord = { A : Int32 }
    with
        open System
        
[<Struct>]
type ABC =
    val a: Int32
    val b: Int32
    new (a) = 
        open type System.Int32
        { a = a; b = MinValue }
    open System

type System.Int32 with
    member this.Abs111 = Abs(this)
    open type System.Math

type A123 =
    | A
    open System
    member _.F _ = 3
and B123 =
    | B
    member _.F _ = Console.WriteLine()