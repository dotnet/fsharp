module Pos27

module TUple = 
    let x1: System.Tuple<int> = System.Tuple.Create(1)
    let x2: System.Tuple<int,int> = System.Tuple.Create(1,2)
    let x3: System.Tuple<int,int,int> = System.Tuple.Create(1,2,3)
    let x4: System.Tuple<int,int,int,int> = System.Tuple.Create(1,2,3,4)
    let x5: System.Tuple<int,int,int,int,int> = System.Tuple.Create(1,2,3,4,5)
    let x6: System.Tuple<int,int,int,int,int,int> = System.Tuple.Create(1,2,3,4,5,6)
    let x7: System.Tuple<int,int,int,int,int,int,int> = System.Tuple.Create(1,2,3,4,5,6,7)
    let x9: System.Tuple<int,int,int,int,int,int,int,System.Tuple<int>> = System.Tuple.Create(1,2,3,4,5,6,7,8)

module ValueTuple = 
    let x1: System.ValueTuple<int> = System.ValueTuple.Create(1)
    let x2: System.ValueTuple<int,int> = System.ValueTuple.Create(1,2)
    let x3: System.ValueTuple<int,int,int> = System.ValueTuple.Create(1,2,3)
    let x4: System.ValueTuple<int,int,int,int> = System.ValueTuple.Create(1,2,3,4)
    let x5: System.ValueTuple<int,int,int,int,int> = System.ValueTuple.Create(1,2,3,4,5)
    let x6: System.ValueTuple<int,int,int,int,int,int> = System.ValueTuple.Create(1,2,3,4,5,6)
    let x7: System.ValueTuple<int,int,int,int,int,int,int> = System.ValueTuple.Create(1,2,3,4,5,6,7)
    let x9: System.ValueTuple<int,int,int,int,int,int,int,System.ValueTuple<int>> = System.ValueTuple.Create(1,2,3,4,5,6,7,8)

module FSharpFunc = 
    let x1: FSharpFunc<int,int> = (fun x -> x + 1)
    let x2: FSharpFunc<int,FSharpFunc<int,int>> = (fun x y -> x + 1 + y)
