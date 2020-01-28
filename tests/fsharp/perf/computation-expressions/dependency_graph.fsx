
let mutable nodes = 0
let mutable recalcs = 0

[<AbstractClass>]
type Node(dirty) = 
    do nodes <- nodes + 1

    let dependees = ResizeArray<System.WeakReference<Node>>()
    let mutable dirty = dirty

    member _.Dirty with get() = dirty and set v = dirty <- v

    member _.Dependees =
        dependees.ToArray() 
        |> Array.choose (fun c -> match c.TryGetTarget() with true, tg -> Some tg | _ -> None)

    member _.AddDependee(c) =
        dependees.Add(System.WeakReference<_>(c))

    member _.InputChanged() =
        for c in dependees do 
            match c.TryGetTarget() with 
            | true, tg -> tg.SetDirty()
            | _ -> ()

    member n.SetDirty() =
        if not dirty then 
            dirty <- true
            n.InputChanged()


[<AbstractClass>]
type Node<'T>(dirty) =
    inherit Node(dirty)
    abstract Value : 'T

/// A node that recomputes if any if its inputs change
type RecalcNode<'T>(dirty, initial, f: unit -> 'T) =
    inherit Node<'T>(dirty)

    let mutable cachedValue = initial 

    new (f) = new RecalcNode<'T>(true, Unchecked.defaultof<_>, f)

    new (initial, f) = new RecalcNode<'T>(false, initial, f)

    override n.Value = 
       if n.Dirty then 
           recalcs <- recalcs + 1
           cachedValue <- f()
           n.Dirty <- false
       cachedValue

    override _.ToString() = sprintf "(latest %A)" cachedValue

/// A node that never recomputes 
type ConstantNode<'T>(x: 'T) =
    inherit Node<'T>(false)

    override _.Value = x

    override _.ToString() = sprintf "(latest %A)" x

type InputNode<'T>(v: 'T) =
    inherit Node<'T>(false)
    let mutable currentValue = v
    override _.Value = currentValue

    member node.SetValue v =
        currentValue <- v
        node.InputChanged()

type NodeBuilder() =

    member _.Bind(x: Node<'T1>, f: 'T1 -> Node<'T2>) : Node<'T2> =
        let rec n = 
            RecalcNode<'T2>(fun () -> 
                let n2 = f x.Value
                n2.AddDependee(n)
                n2.Value)
        x.AddDependee(n)
        n :> Node<_>

    member _.BindReturn(x: Node<'T1>, f: 'T1 -> 'T2) : Node<'T2> =
        let n = RecalcNode<'T2>(fun () -> f x.Value)
        x.AddDependee(n)
        n :> Node<_>

    member _.Bind2(x1: Node<'T1>, x2: Node<'T2>, f: 'T1 * 'T2 -> Node<'T3>) : Node<'T3> =
        let rec n = 
            RecalcNode<'T3>(fun () -> 
                let n2 = f (x1.Value, x2.Value)
                n2.AddDependee(n)
                n2.Value)
        x1.AddDependee(n)
        x2.AddDependee(n)
        n :> Node<_>

    member _.Bind2Return(x1: Node<'T1>, x2: Node<'T2>, f: 'T1 * 'T2 -> 'T3) : Node<'T3> =
        let n = RecalcNode<'T3>(fun () -> f (x1.Value, x2.Value))
        x1.AddDependee(n)
        x2.AddDependee(n)
        n :> Node<_>

    member _.Bind3(x1: Node<'T1>, x2: Node<'T2>, x3: Node<'T3>, f: 'T1 * 'T2 * 'T3 -> Node<'T4>) : Node<'T4> =
        let rec n = 
            RecalcNode<'T4>(fun () -> 
                let n2 = f (x1.Value, x2.Value, x3.Value)
                n2.AddDependee(n)
                n2.Value)
        x1.AddDependee(n)
        x2.AddDependee(n)
        x3.AddDependee(n)
        n :> Node<_>

    member _.Bind3Return(x1: Node<'T1>, x2: Node<'T2>, x3: Node<'T3>, f: 'T1 * 'T2 * 'T3 -> 'T4) : Node<'T4> =
        let n = RecalcNode<'T4>(fun () -> f (x1.Value, x2.Value, x3.Value))
        x1.AddDependee(n)
        x2.AddDependee(n)
        x3.AddDependee(n)
        n :> Node<_>

    member _.MergeSources(x1: Node<'T1>, x2: Node<'T2>) : Node<'T1 * 'T2> =
        let n = RecalcNode<_>(fun () -> (x1.Value, x2.Value))
        x1.AddDependee(n)
        x2.AddDependee(n)
        n :> Node<_>

    member _.Return(x: 'T) : Node<'T> =
        ConstantNode<'T>(x) :> Node<_>

let node = NodeBuilder()
let input v = InputNode(v)

let inp1 = input 3
let inp2 = input 7
let inp3 = input 0

let test1() = 
    node { 
        let! v1 = inp1
        and! v2 = inp2
        and! v3 = inp3
        return v1 + v2 + v3
    }
   //let n1 = node.Bind3Return(inp1.Node, inp2.Node, inp3.Node, (fun (v1, v2, v3) -> v1 + v2 + v3))

let test2() = 
    node { 
        let! v1 = inp1
        let! v2 = inp2
        let! v3 = inp3
        return v1 + v2 + v3
    }

let test msg f = 
    recalcs <- 0
    nodes <- 0

    let (n: Node<int>) = f()

    let v1 = n.Value  // now 10

    recalcs <- 0

    for i in 1 .. 1000 do
        inp1.SetValue  4 
        let v2 = n.Value  // now 11

        inp2.SetValue 10
        let v3 = n.Value // now 14
        ()

    printfn "inp1.Dependees.Length = %d" inp1.Dependees.Length
    printfn "inp2.Dependees.Length = %d" inp2.Dependees.Length
    printfn "total recalcs %s = %d" msg recalcs
    printfn "total nodes %s = %d" msg nodes
    printfn "----" 

test "using and!" test1
test "using let!" test2

//inp1.Dependees.Length = 1
//inp2.Dependees.Length = 1
//total recalcs using and! = 2000
//total nodes using and! = 1
//----
//inp1.Dependees.Length = 1
//inp2.Dependees.Length = 2000
//total recalcs using let! = 6000
//total nodes using let! = 4003