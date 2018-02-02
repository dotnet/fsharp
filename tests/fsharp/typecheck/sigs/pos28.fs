module Pos28

open FSharp.NativeInterop

module Test1 = 
    [<Struct>]
    type Point =
        val mutable X: int
        val mutable Y: int
        new(x,y) = { X=x; Y=y; }    

    let fixPoint1() =
        let mutable point = Point(1,2)
        let p1 = &&point.X
        NativePtr.read<int> p1

module Test2 = 
    [<Struct>]
    type Point = { mutable x : int; mutable y : int }

    let fixPoint1() =
        let mutable point = Unchecked.defaultof<Point>
        let p1 = &&point.x
        NativePtr.read<int> p1


    