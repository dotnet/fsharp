namespace FSharp.Compiler.ComponentTests.ErrorMessages

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
        |> withLangVersionPreview
        // |> typecheck
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 19
                        EndLine = 12
                        EndColumn = 43 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 21
                        EndLine = 12
                        EndColumn = 45 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview        
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 13
                        StartColumn = 25
                        EndLine = 13
                        EndColumn = 49 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 17
                        StartColumn = 17
                        EndLine = 17
                        EndColumn = 28 }
              Message =
               "The member or function 'bar' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcall in type method`` () =
        """
namespace N

    module M =

        type C () =
            [<TailCall>]
            member this.M1() = this.M1() + 1
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 32
                        EndLine = 8
                        EndColumn = 41 }
              Message =
               "The member or function 'M1' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 17
                        EndLine = 10
                        EndColumn = 26 }
              Message =
               "The member or function 'M2' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 15
                        StartColumn = 17
                        EndLine = 15
                        EndColumn = 26 }
              Message =
#if Debug               
               "The member or function 'M2' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
#else
               "The member or function 'M1' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 25
                        EndLine = 9
                        EndColumn = 34 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 9
                        StartColumn = 24
                        EndLine = 9
                        EndColumn = 33 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 22
                        EndLine = 8
                        EndColumn = 23 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 8
                        StartColumn = 32
                        EndLine = 8
                        EndColumn = 43 }
              Message =
               "The member or function 'm2func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 32
                        EndLine = 12
                        EndColumn = 43 }
              Message =
               "The member or function 'm2func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 7
                        StartColumn = 37
                        EndLine = 7
                        EndColumn = 44 }
              Message =
               "The member or function 'foo' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
                        yield! allValsAndExprsOfModDef def  // ToDo: okay to warn here?
            }
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 34
                        StartColumn = 32
                        EndLine = 34
                        EndColumn = 59 }
              Message =
                "The member or function 'allValsAndExprsOfModDef' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
        
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 24
                        StartColumn = 17
                        EndLine = 24
                        EndColumn = 48 }
              Message =
                "The member or function 'CheckDefnInModule' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 35
                        StartColumn = 17
                        EndLine = 35
                        EndColumn = 66 }
              Message =
                "The member or function 'CheckModuleSpec' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 21
                        StartColumn = 27
                        EndLine = 21
                        EndColumn = 35 }
              Message =
                "The member or function 'instType' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 17
                        StartColumn = 32
                        EndLine = 17
                        EndColumn = 77 }
              Message =
                "The member or function 'instType' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 11
                        StartColumn = 25
                        EndLine = 11
                        EndColumn = 36 }
              Message =
                "The member or function 'foldBackOpt' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3569
              Range = { StartLine = 14
                        StartColumn = 25
                        EndLine = 14
                        EndColumn = 36 }
              Message =
                "The member or function 'foldBackOpt' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 12
                        StartColumn = 28
                        EndLine = 12
                        EndColumn = 34 }
              Message =
                "The member or function 'addOne' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
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
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3569
              Range = { StartLine = 10
                        StartColumn = 37
                        EndLine = 10
                        EndColumn = 43 }
              Message =
                "The member or function 'addOne' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for ColonColon as arg of valid tail call `` () =
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
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
