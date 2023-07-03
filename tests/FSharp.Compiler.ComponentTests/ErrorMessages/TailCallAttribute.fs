namespace FSharp.Compiler.ComponentTests.ErrorMessages

open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module ``TailCall Attribute`` =

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in if-else`` () =
        """
let mul x y = x * y

module M =
    [<TailCall>]
    let rec fact n acc =
        if n = 0
        then acc
        else (fact (n - 1) (mul n acc)) + 23
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 15
                        EndLine = 9
                        EndColumn = 39 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 15
                        EndLine = 9
                        EndColumn = 19 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in match clause`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    match n with
    | 0 -> acc
    | _ -> (fact (n - 1) (mul n acc)) + 23
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 13
                        EndLine = 8
                        EndColumn = 37 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 13
                        EndLine = 8
                        EndColumn = 17 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for rec call in binding`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    match n with
    | 0 -> acc
    | _ ->
        let r = fact (n - 1) (mul n acc)
        r + 23
        """
        |> FSharp
        |> withLangVersionPreview        
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 17
                        EndLine = 9
                        EndColumn = 21 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from toplevel`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else
        printfn "%A" n
        fact (n - 1) (mul n acc)
    
let r = fact 100000 1
r |> ignore
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for mutually recursive functions`` () =
        """
let foo x =
    printfn "Foo: %x" x

module M =
    [<TailCall>]
    let rec bar x =
        match x with
        | 0 ->
            foo x           // OK: non-tail-recursive call to a function which doesn't share the current stack frame (i.e., 'bar' or 'baz').
            printfn "Zero"
            
        | 1 ->
            bar (x - 1)     // Warning: this call is not tail-recursive
            printfn "Uno"
            baz x           // OK: tail-recursive call.

        | x ->
            printfn "0x%08x" x
            bar (x - 1)     // OK: tail-recursive call.
            
    and [<TailCall>] baz x =
        printfn "Baz!"
        bar (x - 1)         // OK: tail-recursive call.
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 13
                        EndLine = 14
                        EndColumn = 24 }
              Message =
               "The member or function 'bar' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 13
                        EndLine = 14
                        EndColumn = 16 }
              Message =
               "The member or function 'bar' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcall in type method`` () =
        """
type C () =
    [<TailCall>]
    member this.M1() = this.M1() + 1
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 4
                        StartColumn = 24
                        EndLine = 4
                        EndColumn = 33 }
              Message =
               "The member or function 'M1' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall in type method`` () =
        """
type C () =
    [<TailCall>]
    member this.M1() =
        printfn "M1 called"
        this.M1()

let c = C()
c.M1()
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in type methods`` () =
        """
type C () =
    [<TailCall>]
    member this.M1() =
        printfn "M1 called"
        this.M2()    // ok

    [<TailCall>]
    member this.M2() =
        printfn "M2 called"
        this.M1()     // ok
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcalls in type methods`` () =
        """
module M1 =
    module M2 =
        type F () =
            [<TailCall>]
            member this.M1() =
                printfn "M1 called"
                this.M2() + 1   // should warn

            [<TailCall>]
            member this.M2() =
                printfn "M2 called"
                this.M1() + 2    // should warn
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 17
                        EndLine = 8
                        EndColumn = 26 }
              Message =
               "The member or function 'M2' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 17
                        EndLine = 13
                        EndColumn = 26 }
              Message =
               "The member or function 'M1' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from nested bind`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else
        printfn "%A" n
        fact (n - 1) (mul n acc)
    
let f () =
    let r = fact 100000 1
    r |> ignore
    
fact 100000 1 |> ignore
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in seq expression because of bind`` () =
        """
[<TailCall>]
let rec f x : seq<int> =
    seq {
        let r = f (x - 1)
        let r2 = Seq.map (fun x -> x + 1) r
        yield! r2
}
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 5
                        StartColumn = 17
                        EndLine = 5
                        EndColumn = 18 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in seq expression because of pipe`` () =
        """
[<TailCall>]
let rec f x : seq<int> =
    seq {
        yield! f (x - 1) |> Seq.map (fun x -> x + 1)
}
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 5
                        StartColumn = 16
                        EndLine = 5
                        EndColumn = 25 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 5
                        StartColumn = 16
                        EndLine = 5
                        EndColumn = 17 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in seq expression`` () =
        """
[<TailCall>]
let rec f x = seq {
    let y = x - 1
    let z = y - 1
    yield! f (z - 1)
}

let a: seq<int> = f 10
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in async expression`` () =
        """
[<TailCall>] 
let rec f x = async {
    let y = x - 1
    let z = y - 1
    return! f (z - 1)
}

let a: Async<int> = f 10
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in async expression`` () =
        """
[<TailCall>] 
let rec f x = async { 
    let! r = f (x - 1)
    return r
}
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 4
                        StartColumn = 14
                        EndLine = 4
                        EndColumn = 23 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 4
                        StartColumn = 14
                        EndLine = 4
                        EndColumn = 15 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in rec module`` () =
        """
module rec M =

    module M1 =
        [<TailCall>]
        let m1func() = M2.m2func()

    module M2 =
        [<TailCall>]
        let m2func() = M1.m1func()
        
    let f () =
        M1.m1func() |> ignore

M.M1.m1func() |> ignore
M.M2.m2func()
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in rec module`` () =
        """
module rec M =

    module M1 =
        [<TailCall>]
        let m1func() = 1 + M2.m2func()

    module M2 =
        [<TailCall>]
        let m2func() = 2 + M1.m1func()
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 6
                        StartColumn = 28
                        EndLine = 6
                        EndColumn = 39 }
              Message =
               "The member or function 'm2func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 6
                        StartColumn = 28
                        EndLine = 6
                        EndColumn = 37 }
              Message =
               "The member or function 'm2func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 28
                        EndLine = 10
                        EndColumn = 39 }
              Message =
               "The member or function 'm1func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 28
                        EndLine = 10
                        EndColumn = 37 }
              Message =
               "The member or function 'm1func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for byref parameters`` () =
        """
[<TailCall>]
let rec foo(x: int byref) = foo(&x)
let run() = let mutable x = 0 in foo(&x)
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 3
                        StartColumn = 29
                        EndLine = 3
                        EndColumn = 36 }
              Message =
               "The member or function 'foo' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for yield! in tail position`` () =
        """
type Bind = { Var: string; Expr: string }

type ModuleOrNamespaceBinding =
    | Binding of bind: Bind
    | Module of moduleOrNamespaceContents: MDef

and MDef =
    | TMDefRec of tycons: string list * bindings: ModuleOrNamespaceBinding list
    | TMDefLet of binding: Bind
    | TMDefDo of expr: string
    | TMDefOpens of expr: string
    | TMDefs of defs: MDef list

[<TailCall>]
let rec allValsAndExprsOfModDef mdef =
    seq {
        match mdef with
        | TMDefRec(tycons = _tycons; bindings = mbinds) ->
            for mbind in mbinds do
                match mbind with
                | ModuleOrNamespaceBinding.Binding bind -> yield bind.Var, bind.Expr
                | ModuleOrNamespaceBinding.Module(moduleOrNamespaceContents = def) ->
                    yield! allValsAndExprsOfModDef def
        | TMDefLet(binding = bind) -> yield bind.Var, bind.Expr
        | TMDefDo _ -> ()
        | TMDefOpens _ -> ()
        | TMDefs defs ->
            for def in defs do
                yield! allValsAndExprsOfModDef def
    }
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for calls in for and iter`` () =
        """
type Bind = { Var: string; Expr: string }

type ModuleOrNamespaceBinding =
    | Binding of bind: Bind
    | Module of moduleOrNamespaceContents: MDef

and MDef =
    | TMDefRec of isRec: bool * tycons: string list * bindings: ModuleOrNamespaceBinding list
    | TMDefLet of binding: Bind
    | TMDefDo of expr: string
    | TMDefOpens of expr: string
    | TMDefs of defs: MDef list

let someCheckFunc x = ()

[<TailCall>]
let rec CheckDefnsInModule cenv env mdefs =
    for mdef in mdefs do
        CheckDefnInModule cenv env mdef

and CheckNothingAfterEntryPoint cenv =
    if true then
        printfn "foo"

and [<TailCall>] CheckDefnInModule cenv env mdef =
    match mdef with
    | TMDefRec(isRec, tycons, mspecs) ->
        CheckNothingAfterEntryPoint cenv
        someCheckFunc tycons
        List.iter (CheckModuleSpec cenv env isRec) mspecs
    | TMDefLet bind ->
        CheckNothingAfterEntryPoint cenv
        someCheckFunc bind
    | TMDefOpens _ -> ()
    | TMDefDo e ->
        CheckNothingAfterEntryPoint cenv
        let isTailCall = true
        someCheckFunc isTailCall
    | TMDefs defs -> CheckDefnsInModule cenv env defs

and [<TailCall>] CheckModuleSpec cenv env isRec mbind =
    match mbind with
    | ModuleOrNamespaceBinding.Binding bind -> someCheckFunc bind
    | ModuleOrNamespaceBinding.Module mspec ->
        someCheckFunc mspec
        CheckDefnInModule cenv env mspec
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 20
                        StartColumn = 9
                        EndLine = 20
                        EndColumn = 40 }
              Message =
                "The member or function 'CheckDefnInModule' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 20
                        StartColumn = 9
                        EndLine = 20
                        EndColumn = 26 }
              Message =
                "The member or function 'CheckDefnInModule' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 31
                        StartColumn = 20
                        EndLine = 31
                        EndColumn = 50 }
              Message =
                "The member or function 'CheckModuleSpec' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 31
                        StartColumn = 20
                        EndLine = 31
                        EndColumn = 35 }
              Message =
                "The member or function 'CheckModuleSpec' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for partial application but for calls in map and total applications`` () =
        """
type Type() =
    member val HasElementType = true with get, set
    member val IsArray = true with get, set
    member val IsPointer = false with get, set
    member val IsByRef = false with get, set
    member val IsGenericParameter = false with get, set
    member _.GetArray () = Array.empty
    member _.GetArrayRank () = 2

[<TailCall>]
let rec instType a b (ty: Type) =
    if a then
        let typeArgs = Array.map (instType true 100) (ty.GetArray())
        22
    elif ty.HasElementType then
        let ety = instType true 23    // should not warn
        let ety = instType true 23 ty // should warn
        if ty.IsArray then
            let rank = ty.GetArrayRank()
            23
        elif ty.IsPointer then 24
        elif ty.IsByRef then 25
        else 26
    elif ty.IsGenericParameter then
        27
    else 28
        """
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 18
                        StartColumn = 19
                        EndLine = 18
                        EndColumn = 27 }
              Message =
                "The member or function 'instType' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 35
                        EndLine = 14
                        EndColumn = 43 }
              Message =
                "The member or function 'instType' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
