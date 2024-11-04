namespace ErrorMessages

open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module ``TailCall Attribute`` =

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in if-else`` () =
        """
namespace N

    module M =

        let mul x y = x * y

        [<TailCall>]
        let rec fact n acc =
            if n = 0
            then acc
            else (fact (n - 1) (mul n acc)) + 23
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 19
                        EndLine = 12
                        EndColumn = 43 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in match clause`` () =
        """
namespace N

    module M =

        let mul x y = x * y

        [<TailCall>]
        let rec fact n acc =
            match n with
            | 0 -> acc
            | _ -> (fact (n - 1) (mul n acc)) + 23
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 21
                        EndLine = 12
                        EndColumn = 45 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for rec call in binding`` () =
        """
namespace N

    module M =

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
        |> withLangVersion80        
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 25
                        EndLine = 13
                        EndColumn = 49 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from toplevel`` () =
        """
namespace N

    module M =

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
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for mutually recursive functions`` () =
        """
namespace N

    module M =

        let foo x =
            printfn "Foo: %x" x

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
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 17
                        StartColumn = 17
                        EndLine = 17
                        EndColumn = 28 }
              Message =
               "The member or function 'bar' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcall in type method`` () =
        """
namespace N

    module M =

        type C () =
            [<TailCall>]
            member this.M1() = this.M1() + 1
            
            type InnerC () =
                [<TailCall>]
                member this.InnerCMeth x = this.InnerCMeth x + 23
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 32
                        EndLine = 8
                        EndColumn = 41 }
              Message =
               "The member or function 'M1' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 44
                        EndLine = 12
                        EndColumn = 61 }
              Message =
               "The member or function 'InnerCMeth' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall in type method`` () =
        """
namespace N

    module M =

        type C () =
            [<TailCall>]
            member this.M1() =
                printfn "M1 called"
                this.M1()

        let c = C()
        c.M1()
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in type methods`` () =
        """
namespace N

    module M =

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
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcalls in type methods`` () =
        """
namespace N

    module M =

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
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 17
                        EndLine = 10
                        EndColumn = 26 }
              Message =
               "The member or function 'M2' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 15
                        StartColumn = 17
                        EndLine = 15
                        EndColumn = 26 }
              Message =
#if Debug               
               "The member or function 'M2' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
#else
               "The member or function 'M1' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
#endif
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from nested bind`` () =
        """
namespace N

    module M =

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
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in seq expression because of bind`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec f x : seq<int> =
            seq {
                let r = f (x - 1)
                let r2 = Seq.map (fun x -> x + 1) r
                yield! r2
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 25
                        EndLine = 9
                        EndColumn = 34 }
              Message =
               "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in seq expression because of pipe`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec f x : seq<int> =
            seq {
                yield! f (x - 1) |> Seq.map (fun x -> x + 1)
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 24
                        EndLine = 9
                        EndColumn = 33 }
              Message =
               "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in seq expression`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec f x = seq {
            let y = x - 1
            let z = y - 1
            yield! f (z - 1)
        }

        let a: seq<int> = f 10
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in async expression`` () =
        """
namespace N

    module M =

        [<TailCall>] 
        let rec f x = async {
            let y = x - 1
            let z = y - 1
            return! f (z - 1)
        }

        let a: Async<int> = f 10
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for return! rec call in task`` () =
        """
namespace N

    module M =

        [<TailCall>] 
        let rec f x = task {
            let y = x - 1
            let z = y - 1
            return! f (z - 1)
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 21
                        EndLine = 10
                        EndColumn = 22 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in use scope`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec f () =
            use x = System.IO.File.OpenRead(@"C:\tmp\testfile")
            f ()
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 13
                        EndLine = 9
                        EndColumn = 14 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 13
                        EndLine = 9
                        EndColumn = 17 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in Sequential in use scope`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec f () =
            let path = System.IO.Path.GetTempFileName()
            use file = System.IO.File.Open(path, System.IO.FileMode.Open)
            printfn "Hi!"
            f ()
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 11
                        StartColumn = 13
                        EndLine = 11
                        EndColumn = 14 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in async expression`` () =
        """
namespace N

    module M =

        [<TailCall>] 
        let rec f x = async { 
            let! r = f (x - 1)
            return r
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 22
                        EndLine = 8
                        EndColumn = 23 }
              Message =
               "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in rec module`` () =
        """
namespace N

    module rec M =

        module M1 =
            [<TailCall>]
            let m1func() = M2.m2func()

        module M2 =
            [<TailCall>]
            let m2func() = M1.m1func()
            
        let f () =
            M1.m1func() |> ignore
            
    module M2 =

        M.M1.m1func() |> ignore
        M.M2.m2func()
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in rec module`` () =
        """
namespace N

    module rec M =

        module M1 =
            [<TailCall>]
            let m1func() = 1 + M2.m2func()

        module M2 =
            [<TailCall>]
            let m2func() = 2 + M1.m1func()
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 32
                        EndLine = 8
                        EndColumn = 43 }
              Message =
               "The member or function 'm2func' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 32
                        EndLine = 12
                        EndColumn = 43 }
              Message =
               "The member or function 'm2func' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for byref parameters`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec foo(x: int byref) = foo(&x)
        let run() = let mutable x = 0 in foo(&x)
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 7
                        StartColumn = 37
                        EndLine = 7
                        EndColumn = 44 }
              Message =
               "The member or function 'foo' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for yield! in tail position`` () =
        """
namespace N

    module M =

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
                        yield! allValsAndExprsOfModDef def  // ToDo: okay to not warn here?
            }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for calls in for and iter`` () =
        """
namespace N

    module M =

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
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 24
                        StartColumn = 17
                        EndLine = 24
                        EndColumn = 48 }
              Message =
                "The member or function 'CheckDefnInModule' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 35
                        StartColumn = 17
                        EndLine = 35
                        EndColumn = 66 }
              Message =
                "The member or function 'CheckModuleSpec' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for partial application but for calls in map and total applications`` () =
        """
namespace N

    module M =
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
                let ety = instType true 23    // ToDo: also warn for partial app?
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
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 21
                        StartColumn = 27
                        EndLine = 21
                        EndColumn = 35 }
              Message =
                "The member or function 'instType' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 17
                        StartColumn = 32
                        EndLine = 17
                        EndColumn = 77 }
              Message =
                "The member or function 'instType' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid calls in inner bindings of conditional`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec foldBackOpt f (m: Map<'Key, 'Value>) x =
            if not (Map.isEmpty m) then
                x
            else if m.Count = 1 then
                let a = foldBackOpt f m x
                f  x
            else
                let a = foldBackOpt f m x
                let x = f x
                foldBackOpt f m a
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 11
                        StartColumn = 25
                        EndLine = 11
                        EndColumn = 36 }
              Message =
                "The member or function 'foldBackOpt' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 25
                        EndLine = 14
                        EndColumn = 36 }
              Message =
                "The member or function 'foldBackOpt' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for piped arg in tailrec call`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec loop xs =
            xs
            |> fun xs ->
                 loop xs
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for ColonColon with inner let-bound value to rec call`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec addOne (input: int list) : int list =
            match input with
            | [] -> []
            | x :: xs ->
                let head = (x + 1)
                let tail = addOne xs
                head :: tail
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 28
                        EndLine = 12
                        EndColumn = 34 }
              Message =
                "The member or function 'addOne' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for ColonColon with rec call`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec addOne (input: int list) : int list =
            match input with
            | [] -> []
            | x :: xs -> (x + 1) :: addOne xs
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 37
                        EndLine = 10
                        EndColumn = 43 }
              Message =
                "The member or function 'addOne' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for ColonColon as arg of valid tail call`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec addOne (input: int list) (acc: int list) : int list = 
            match input with
            | [] -> acc
            | x :: xs -> addOne xs ((x + 1) :: acc)
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for non tail-rec traversal`` () =
        """
namespace N

    module M =
    
        type 'a Tree =
            | Leaf of 'a
            | Node of 'a Tree * 'a Tree
    
        [<TailCall>]
        let rec findMax (tree: int Tree) : int =
            match tree with
            | Leaf i -> i
            | Node (l, r) -> System.Math.Max(findMax l, findMax r)
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 46
                        EndLine = 14
                        EndColumn = 53 }
              Message =
                "The member or function 'findMax' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 57
                        EndLine = 14
                        EndColumn = 64 }
              Message =
                "The member or function 'findMax' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 46
                        EndLine = 14
                        EndColumn = 55 }
              Message =
                "The member or function 'findMax' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 57
                        EndLine = 14
                        EndColumn = 66 }
              Message =
                "The member or function 'findMax' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for non tail-rec traversal with List.collect`` () =
        """
namespace N

    module M =
    
        type Tree =
        | Leaf of int
        | Node of Tree list

        [<TailCall>]
        let rec loop tree =
            match tree with
            | Leaf n -> [ n ]
            | Node branches -> branches |> List.collect loop
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 57
                        EndLine = 14
                        EndColumn = 61 }
              Message =
                "The member or function 'loop' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for simple rec call in try-with`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec gTryWith x =
            try
                gTryWith (x + 1)
            with e ->
                raise (System.InvalidOperationException("Operation has failed", e))
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 17
                        EndLine = 9
                        EndColumn = 25 }
              Message =
                "The member or function 'gTryWith' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 17
                        EndLine = 9
                        EndColumn = 33 }
              Message =
                "The member or function 'gTryWith' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for return! rec call in async try-with`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec gAsyncTryWith (x: int) : Async<int> = async {
            try
                return! gAsyncTryWith (x + 1)
            with e ->
                return raise (System.InvalidOperationException("Operation has failed", e))
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 25
                        EndLine = 9
                        EndColumn = 38 }
              Message =
                "The member or function 'gAsyncTryWith' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in match branch in try-finally`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec gTryFinallyMatch x =
            try
                match x with
                | 0 -> x
                | _ -> gTryFinallyMatch x
            finally
                ()
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 11
                        StartColumn = 24
                        EndLine = 11
                        EndColumn = 40 }
              Message =
                "The member or function 'gTryFinallyMatch' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in if-else branch in try-with`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec gTryWithIfElse x =
            try
                if (x = 0) then
                    x
                else gTryWithIfElse x
            with e ->
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw(e)
                Unchecked.defaultof<_>
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 11
                        StartColumn = 22
                        EndLine = 11
                        EndColumn = 36 }
              Message =
                "The member or function 'gTryWithIfElse' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in match branch in try-with`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec factorialWithAccTryWith n accumulator =
            try
                match n with
                | 0u | 1u -> accumulator
                | _ -> factorialWithAccTryWith (n - 1u) (n * accumulator)
            with e ->
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw(e)
                Unchecked.defaultof<_>
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 11
                        StartColumn = 24
                        EndLine = 11
                        EndColumn = 47 }
              Message =
                "The member or function 'factorialWithAccTryWith' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in with`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec gWithRecCallInWith x =
            try
                failwith "foo"
            with _ ->
                match x with
                | 0 -> x
                | _ -> gWithRecCallInWith x
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 24
                        EndLine = 13
                        EndColumn = 42 }
              Message =
                "The member or function 'gWithRecCallInWith' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call in finally`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec gWithRecCallInFinally x =
            try
                failwith "foo"
            finally
                match x with
                | 0 -> x
                | _ -> gWithRecCallInFinally x
                |> ignore
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 24
                        EndLine = 13
                        EndColumn = 45 }
              Message =
                "The member or function 'gWithRecCallInFinally' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call inside of match lambda with closure over local function`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec f x y z =
            let g x = x
            function
            | [] -> None
            | h :: tail ->
                h ()
                f x (g y) z tail
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 17
                        EndLine = 13
                        EndColumn = 33 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for rec call inside of match lambda with closure over local function using pipe`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec f x y z =
            let g x = x
            function
            | [] -> None
            | h :: tail ->
                h ()
                tail |> f x (g y) z // using the pipe in this match lambda and closing over g caused FS0251 in 8.0 release, issue #16330
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 17
                        EndLine = 13
                        EndColumn = 36 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for Continuation Passing Style func using [<TailCall>] func in continuation lambda`` () =
        """
namespace N

    module M =
    
        type 'a Tree =
            | Leaf of 'a
            | Node of 'a Tree * 'a Tree
    
        [<TailCall>]
        let rec findMaxInner (tree: int Tree) (continuation: int -> int) : int =
            match tree with
            | Leaf i -> i |> continuation
            | Node (left, right) ->
                findMaxInner left (fun lMax ->
                    findMaxInner right (fun rMax ->
                        System.Math.Max(lMax, rMax) |> continuation
                    )
                )
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for Continuation Passing Style func not using [<TailCall>] func in continuation lambda`` () =
        """
namespace N

    module M =
    
        [<TailCall>]
        let rec loop
            (files: string list)
            (finalContinuation: string list * string list -> string list * string list)
            =
            match files with
            | [] -> finalContinuation ([], [])
            | h :: rest ->
                loop rest (fun (files, folders) ->
                    if h.EndsWith("/") then
                        files, (h :: folders)
                    else
                        (h :: files), folders
                    |> finalContinuation)
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for Continuation Passing Style func using [<TailCall>] func in continuation lambda 2`` () =
        """
namespace N

    module M =
        type 'a RoseTree =
            | Leaf of 'a
            | Node of 'a * 'a RoseTree list
    
        [<TailCall>]
        let rec findMaxInner (roseTree : int RoseTree) (continuation : int -> 'ret) : 'ret =
            match roseTree with
            | Leaf i
            | Node (i, [])  -> i |> continuation
            | Node (i, [ x ]) ->
                findMaxInner x (fun xMax ->
                    System.Math.Max(i, xMax) |> continuation
                )
            | Node (i, [ x; y ]) ->
                findMaxInner x (fun xMax ->
                    findMaxInner y (fun yMax ->
                        System.Math.Max(i, System.Math.Max(xMax, yMax)) |> continuation
                    )
                )
            | _ -> failwith "Nodes with lists longer than 2 are not supported"
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for Continuation Passing Style func using [<TailCall>] func in list of continuations`` () =
        """
namespace N

    [<RequireQualifiedAccess>]
    module Continuation =
        let rec sequence<'a, 'ret> (recursions : (('a -> 'ret) -> 'ret) list) (finalContinuation : 'a list -> 'ret) : 'ret =
            match recursions with
            | [] -> [] |> finalContinuation
            | recurse :: recurses ->
                recurse (fun ret ->
                    sequence recurses (fun rets ->
                        ret :: rets |> finalContinuation
                    )
                )

    module M =
        type 'a RoseTree =
            | Leaf of 'a
            | Node of 'a * 'a RoseTree list
    
        [<TailCall>]
        let rec findMaxInner (roseTree : int RoseTree) (finalContinuation : int -> 'ret) : 'ret =
            match roseTree with
            | Leaf i ->
                i |> finalContinuation
            | Node (i : int, xs : int RoseTree list) ->
                let continuations : ((int -> 'ret) -> 'ret) list = xs |> List.map findMaxInner
                let finalContinuation (maxValues : int list) : 'ret = List.max (i :: maxValues) |> finalContinuation
                Continuation.sequence continuations finalContinuation
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for Continuation Passing Style func using [<TailCall>] func in object interface expression`` () =
        """
namespace N

[<NoComparison>]
type Foo<'a> =
    | Pure of 'a
    | Apply of ApplyCrate<'a>

and ApplyEval<'a, 'ret> = abstract Eval<'b,'c,'d> : 'b Foo -> 'c Foo -> 'd Foo -> ('b -> 'c -> 'd -> 'a) Foo -> 'ret

and ApplyCrate<'a> = abstract Apply : ApplyEval<'a, 'ret> -> 'ret

module M =

    [<TailCall>]
    let rec evaluateCps<'a, 'b> (f : 'a Foo) (cont : 'a -> 'b) : 'b =
        match f with
        | Pure a -> cont a
        | Apply crate ->
            crate.Apply
                { new ApplyEval<_,_> with
                    member _.Eval b c d f =
                        evaluateCps f (fun f ->
                            evaluateCps b (fun b ->
                                evaluateCps c (fun c ->
                                    evaluateCps d (fun d -> cont (f b c d))
                                )
                            )
                        )
                }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn about attribute on non-rec function`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let someNonRecFun x = x + x
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3861
              Range = { StartLine = 7
                        StartColumn = 13
                        EndLine = 7
                        EndColumn = 26 }
              Message =
               "The TailCall attribute should only be applied to recursive functions." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Error about attribute on non-recursive let-bound value`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let someX = 23
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 842, Line 6, Col 11, Line 6, Col 19, "This attribute is not valid for use on this language element")

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Error about attribute on recursive let-bound value`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec someRecLetBoundValue = nameof(someRecLetBoundValue)
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 842, Line 6, Col 11, Line 6, Col 19, "This attribute is not valid for use on this language element")

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn about self-defined attribute`` () = // is the analysis available for users of older FSharp.Core versions
        """
module Microsoft.FSharp.Core

    open System
    
    [<AttributeUsage(AttributeTargets.Method)>]
    type TailCallAttribute() = inherit Attribute()

    [<TailCall>]
    let rec f x = 1 + f x
        """
        |> FSharp
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 23
                        EndLine = 10
                        EndColumn = 26 }
              Message =
                "The member or function 'f' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for recursive call in list comprehension`` () =
        """
namespace N

    module M =

        [<TailCall>]
        let rec reverse (input: list<'t>) =
            match input with
            | head :: tail -> [ yield! reverse tail; head ]
            | [] -> []
        """
        |> FSharp
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 40
                        EndLine = 9
                        EndColumn = 52 }
              Message =
                "The member or function 'reverse' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for yield! call of rec func in seq`` () =
        """
namespace N

module M =

    type SynExpr =
        | Sequential of expr1 : SynExpr * expr2 : SynExpr
        | NotSequential
        member _.Range = 99

    type SyntaxNode = SynExpr of SynExpr

    type SyntaxVisitor () = member _.VisitExpr _ = None

    let visitor = SyntaxVisitor ()
    let dive expr range f = range, fun () -> Some expr
    let traverseSynExpr _ expr = Some expr

    [<TailCall>]
    let rec traverseSequentials path expr =
        seq {
            match expr with
            | SynExpr.Sequential(expr1 = expr1; expr2 = SynExpr.Sequential _ as expr2) ->
                yield dive expr expr.Range (fun expr -> visitor.VisitExpr(path, traverseSynExpr path, (fun _ -> None), expr))
                let path = SyntaxNode.SynExpr expr :: path
                yield dive expr1 expr1.Range (traverseSynExpr path)
                yield! traverseSequentials path expr2   // should not warn

            | _ ->
                yield dive expr expr.Range (traverseSynExpr path)
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for yield! call of rec func in list comprehension`` () =
        """
namespace N

module M =

    type SynExpr =
        | Sequential of expr1 : SynExpr * expr2 : SynExpr
        | NotSequential
        member _.Range = 99

    type SyntaxNode = SynExpr of SynExpr

    type SyntaxVisitor () = member _.VisitExpr _ = None

    let visitor = SyntaxVisitor ()
    let dive expr range f = range, fun () -> Some expr
    let traverseSynExpr _ expr = Some expr

    [<TailCall>]
    let rec traverseSequentials path expr =
        [
            match expr with
            | SynExpr.Sequential(expr1 = expr1; expr2 = SynExpr.Sequential _ as expr2) ->
                // It's a nested sequential expression.
                // Visit it, but make defaultTraverse do nothing,
                // since we're going to traverse its descendants ourselves.
                yield dive expr expr.Range (fun expr -> visitor.VisitExpr(path, traverseSynExpr path, (fun _ -> None), expr))

                // Now traverse its descendants.
                let path = SyntaxNode.SynExpr expr :: path
                yield dive expr1 expr1.Range (traverseSynExpr path)
                yield! traverseSequentials path expr2   // should warn

            | _ ->
                // It's not a nested sequential expression.
                // Traverse it normally.
                yield dive expr expr.Range (traverseSynExpr path)
        ]
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 32
                        StartColumn = 24
                        EndLine = 32
                        EndColumn = 54 }
              Message =
                "The member or function 'traverseSequentials' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for yield! call of rec func in custom CE`` () =
        """
namespace N

module M =

    type SynExpr =
        | Sequential of expr1 : SynExpr * expr2 : SynExpr
        | NotSequential
        member _.Range = 99

    type SyntaxNode = SynExpr of SynExpr

    type SyntaxVisitor () = member _.VisitExpr _ = None

    let visitor = SyntaxVisitor ()
    let dive expr range f = range, fun () -> Some expr
    let traverseSynExpr _ expr = Some expr

    type ThingsBuilder() =

        member _.Yield(x) = [ x ]

        member _.Combine(currentThings, newThings) = currentThings @ newThings

        member _.Delay(f) = f ()

        member _.YieldFrom(x) = x

    let things = ThingsBuilder()

    [<TailCall>]
    let rec traverseSequentials path expr =
        things {
            match expr with
            | SynExpr.Sequential(expr1 = expr1; expr2 = SynExpr.Sequential _ as expr2) ->
                // It's a nested sequential expression.
                // Visit it, but make defaultTraverse do nothing,
                // since we're going to traverse its descendants ourselves.
                yield dive expr expr.Range (fun expr -> visitor.VisitExpr(path, traverseSynExpr path, (fun _ -> None), expr))

                // Now traverse its descendants.
                let path = SyntaxNode.SynExpr expr :: path
                yield dive expr1 expr1.Range (traverseSynExpr path)
                yield! traverseSequentials path expr2   // should warn

            | _ ->
                // It's not a nested sequential expression.
                // Traverse it normally.
                yield dive expr expr.Range (traverseSynExpr path)
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 43
                        StartColumn = 17
                        EndLine = 43
                        EndColumn = 68 }
              Message =
                "The member or function 'traverseSequentials' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for rec call of async func that evaluates an async parameter in a match!`` () =
        """
namespace N

module M =

    [<TailCall>]
    let rec f (g: bool Async) = async {
        match! g with
        | false -> ()
        | true -> return! f g
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for rec call of async func that evaluates an async parameter in a let!`` () =
        """
namespace N

module M =

    [<TailCall>]
    let rec f (g: bool Async) = async {
        let! x = g
        match x with
        | false -> ()
        | true -> return! f g
        }
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for tail rec call returning unit`` () =
        """
namespace N

module M =

    [<TailCall>]
    let rec go (args: string list) =
        match args with
        | [] -> ()
        | "--" :: _ -> ()
        | arg :: args -> go args
        """
        |> FSharp
        |> withLangVersion80
        |> compile
        |> shouldSucceed
