module Neg117

#nowarn "64" // This construct causes code to be less generic than indicated by the type annotations.

module TargetA =

    [<RequireQualifiedAccess>]
    type TransformerKind =
        | A
        | B

    type M1 = int

    type M2 = float

    type Target() =

        member __.TransformM1 (kind: TransformerKind) : M1[] option = [| 0 |] |> Some
        member __.TransformM2 (kind: TransformerKind) : M2[] option = [| 1. |] |> Some

    type TargetA =

        static member instance : Target option = None

        static member inline Transform(_: ^r, _: TargetA) = fun (kind:TransformerKind) -> TargetA.instance.Value.TransformM1 kind : ^r
        static member inline Transform(_: ^r, _: TargetA) = fun (kind:TransformerKind) ->  TargetA.instance.Value.TransformM2 kind : ^r

        static member inline Transform(kind: TransformerKind) =
            let inline call2(a:^a, b:^b) = ((^a or ^b) : (static member Transform: _ * _ -> _) b, a)
            let inline call (a: 'a) = fun (x: 'x) -> call2(a, Unchecked.defaultof<'r>) x : 'r
            call Unchecked.defaultof<TargetA> kind

    let inline Transform kind = TargetA.Transform kind

module TargetB =
    [<RequireQualifiedAccess>]
    type TransformerKind =
        | C
        | D

    type M1 = | M1

    type M2 = | M2

    type Target() =

        member __.TransformM1 (kind: TransformerKind) = [| M1 |] |> Some
        member __.TransformM2 (kind: TransformerKind) = [| M2 |] |> Some

    type TargetB =

        static member instance : Target option = None
    
        static member inline Transform(_: ^r, _: TargetB) = fun (kind:TransformerKind) -> TargetB.instance.Value.TransformM1 kind : ^r
        static member inline Transform(_: ^r, _: TargetB) = fun (kind:TransformerKind) -> TargetB.instance.Value.TransformM2 kind : ^r

        static member inline Transform(kind: TransformerKind) =
            let inline call2(a:^a, b:^b) = ((^a or ^b) : (static member Transform: _ * _ -> _) b, a)
            let inline call (a: 'a) = fun (x: 'x) -> call2(a, Unchecked.defaultof<'r>) x : 'r
            call Unchecked.defaultof<TargetB> kind
    let inline Transform kind = TargetB.Transform kind

module Superpower =

    type Transformer =
        
        static member inline Transform(_: ^f, _: TargetB.TargetB, _: Transformer) =
            fun x -> TargetB.Transform x : ^f
        
        static member inline Transform(_: ^r, _: TargetA.TargetA, _: Transformer) =
           fun x -> TargetA.Transform x : ^r

        static member inline YeahTransform kind =
            let inline call2(a:^a, b:^b, c: ^c) = ((^a or ^b or ^c) : (static member Transform: _ * _ * _ -> _) c, b, a)
            let inline call (a: 'a) = fun (x: 'x) -> call2(a, Unchecked.defaultof<_>, Unchecked.defaultof<'r>) x : 'r
            call Unchecked.defaultof<Transformer> kind 

module Examples =
    let a kind = Superpower.Transformer.YeahTransform kind : TargetA.M1[]
    let b = Superpower.Transformer.YeahTransform TargetA.TransformerKind.A : TargetA.M2[] option
    let c = Superpower.Transformer.YeahTransform TargetB.TransformerKind.C : TargetB.M1[] option
    let d = Superpower.Transformer.YeahTransform TargetA.TransformerKind.A : TargetA.M1[] option
