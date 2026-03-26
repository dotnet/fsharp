// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module CodeGenRegressions_Closures =

    let private getActualIL (result: CompilationResult) =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some p ->
                let (_, _, actualIL) = ILChecker.verifyILAndReturnActual [] p [ "// dummy" ]
                actualIL
            | None -> failwith "No output path"
        | _ -> failwith "Compilation failed"

    // https://github.com/dotnet/fsharp/issues/19068
    [<Fact>]
    let ``Issue_19068_StructObjectExprByrefField`` () =
        let source = """
module Test

type Class(test : obj) = class end

[<Struct>]
type Struct(test : obj) =
    member _.Test() = {
        new Class(test) with
        member _.ToString() = ""
    }

let run() =
    let s = Struct("hello")
    s.Test() |> ignore
    printfn "Success"

run()
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19068 — sequence expression path
    [<Fact>]
    let ``Issue_19068_StructSeqExprByrefCapture`` () =
        let source = """
module Test

[<Struct>]
type StructSeq(items: obj[]) =
    member _.GetItems() =
        let arr = items
        seq {
            for item in arr do
                yield item
        }

let s = StructSeq([| box 1; box "hello"; box 3.14 |])
let result = s.GetItems() |> Seq.toArray
if result.Length <> 3 then failwithf "Expected 3 items, got %d" result.Length
if result.[0] :?> int <> 1 then failwith "First item wrong"
if result.[1] :?> string <> "hello" then failwith "Second item wrong"
printfn "Sequence expression test passed"
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19068 — delegate expression path
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Issue_19068_StructDelegateByrefCapture`` () =
        let source = """
module Test

[<Struct>]
type StructDelegate(value: obj) =
    member _.CreateAction() =
        let v = value
        System.Action(fun () -> printfn "Value: %A" v)

let s = StructDelegate(box 42)
let action = s.CreateAction()
action.Invoke()
printfn "Delegate expression test passed"
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19068 — closure/lambda path
    [<Fact>]
    let ``Issue_19068_StructClosureByrefCapture`` () =
        let source = """
module Test

[<Struct>]
type StructClosure(value: obj) =
    member _.GetValue() =
        let v = value
        let f = fun () -> v
        f()

let s = StructClosure(box "test")
let result = s.GetValue()
if result :?> string <> "test" then failwithf "Expected 'test', got '%A'" result
printfn "Closure capture test passed"
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/17692
    [<Fact>]
    let ``Issue_17692_MutualRecursionDuplicateParamName`` () =
        let source = """
module Test

let rec caller x = callee (x - 1)
and callee y = if y > 0 then caller y else 0

let rec f1 a = f2 (a - 1) + f3 (a - 2)
and f2 b = if b > 0 then f1 b else 1
and f3 c = if c > 0 then f2 c else 2

let result1 = caller 5
let result2 = f1 5
printfn "Results: %d %d" result1 result2
"""
        // Verify compilation and runtime
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/17692
    // Verify IL closure constructors have unique parameter names
    [<Fact>]
    let ``Issue_17692_MutualRecursionNoDuplicateCtorParams`` () =
        let source = """
module Test

let rec caller x = callee (x - 1)
and callee y = if y > 0 then caller y else 0
"""
        let result =
            FSharp source
            |> asLibrary
            |> withOptimize
            |> compile
            |> shouldSucceed

        let actualIL = getActualIL result

        // Find all .ctor methods and check for duplicate param names
        let ctorPattern = System.Text.RegularExpressions.Regex(@"\.method.*\.ctor\(([^)]*)\)")
        let paramPattern = System.Text.RegularExpressions.Regex(@"(\w+[\w@]*)\s+\w+")
        for m in ctorPattern.Matches(actualIL) do
            let paramStr = m.Groups.[1].Value
            let paramNames = 
                [ for p in paramPattern.Matches(paramStr) -> p.Groups.[1].Value ]
                |> List.filter (fun n -> n <> "class" && n <> "int32" && n <> "object" && n <> "string" && n <> "valuetype" && n <> "void" && n <> "bool")
            let distinct = paramNames |> List.distinct
            if paramNames.Length <> distinct.Length then
                failwithf "Duplicate param names in .ctor: %A (from: %s)" paramNames paramStr

    // let-rec lambda reordering: ensures closures are allocated before non-lambda bindings
    [<Fact>]
    let ``LetRec_MutRecInitOrder`` () =
        let source = """
module MutRecInitTest

type Node = { Next: Node; Prev: Node; Value: int }

let rec zero = { Next = zero; Prev = zero; Value = 0 }

let rec one = { Next = two; Prev = two; Value = 1 }
and two = { Next = one; Prev = one; Value = 2 }

[<EntryPoint>]
let main _ =
    let zeroOk = obj.ReferenceEquals(zero.Next, zero) && obj.ReferenceEquals(zero.Prev, zero)
    let oneNextOk = obj.ReferenceEquals(one.Next, two)
    let onePrevOk = obj.ReferenceEquals(one.Prev, two)
    let twoNextOk = obj.ReferenceEquals(two.Next, one)
    let twoPrevOk = obj.ReferenceEquals(two.Prev, one)
    
    if zeroOk && oneNextOk && onePrevOk && twoNextOk && twoPrevOk then
        0
    else
        failwith (sprintf "Mutual recursion initialization failed: zero=%b one.Next=%b one.Prev=%b two.Next=%b two.Prev=%b" 
                         zeroOk oneNextOk onePrevOk twoNextOk twoPrevOk)
"""
        FSharp source
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // let-rec lambda reordering: ensures closures are allocated before non-lambda bindings
    [<Fact>]
    let ``LetRec_LetRecNonLambdaOrderPreserved`` () =
        let source = """
module Test

#nowarn "40"
#nowarn "22"

[<NoComparison; NoEquality>]
type Node = { Value: int; GetLabel: unit -> string }

let mutable log = []

let test () =
    let rec a = (log <- "a" :: log; { Value = 1; GetLabel = labelA })
    and b = (log <- "b" :: log; { Value = 2; GetLabel = labelB })
    and labelA () = sprintf "A(%d)" a.Value
    and labelB () = sprintf "B(%d)" b.Value
    
    // Non-lambda bindings 'a' and 'b' should keep their relative order
    // (both come after lambdas labelA, labelB in the reordered list)
    let reversedLog = log |> List.rev
    if reversedLog <> ["a"; "b"] then 
        failwithf "Expected non-lambda order [a; b], got %A" reversedLog
    if a.GetLabel() <> "A(1)" then failwithf "a.GetLabel() = %s" (a.GetLabel())
    if b.GetLabel() <> "B(2)" then failwithf "b.GetLabel() = %s" (b.GetLabel())
    printfn "Order preserved correctly"

[<EntryPoint>]
let main _ = test(); 0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // let-rec lambda reordering: ensures closures are allocated before non-lambda bindings
    [<Fact>]
    let ``LetRec_LetRecLambdaDependsOnLambda`` () =
        let source = """
module Test

let test () =
    let rec f x = g (x - 1)
    and g x = if x <= 0 then 0 else f x
    
    let result = f 5
    if result <> 0 then failwithf "Expected 0, got %d" result
    printfn "Lambda-to-lambda: %d" result

[<EntryPoint>]
let main _ = test(); 0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // let-rec lambda reordering: ensures closures are allocated before non-lambda bindings
    [<Fact>]
    let ``LetRec_DeepMixedMutualRecursion`` () =
        let source = """
module Test

[<NoComparison; NoEquality>]
type Tree = { Label: string; Children: unit -> Tree list }

let test () =
    let rec root = { Label = "root"; Children = rootChildren }
    and child1 = { Label = "child1"; Children = child1Children }
    and child2 = { Label = "child2"; Children = fun () -> [root] }
    and rootChildren () = [child1; child2]
    and child1Children () = [child2]
    
    if root.Label <> "root" then failwith "root label"
    let kids = root.Children()
    if kids.Length <> 2 then failwithf "Expected 2 children, got %d" kids.Length
    if kids.[0].Label <> "child1" then failwith "child1 label"
    if kids.[1].Label <> "child2" then failwith "child2 label"
    
    let grandkids = kids.[0].Children()
    if grandkids.Length <> 1 then failwithf "Expected 1 grandchild, got %d" grandkids.Length
    if grandkids.[0].Label <> "child2" then failwith "grandchild label"
    
    let backRef = kids.[1].Children()
    if not (obj.ReferenceEquals(backRef.[0], root)) then failwith "back reference to root"
    
    printfn "Deep mixed mutual recursion passed"

[<EntryPoint>]
let main _ = test(); 0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // let-rec lambda reordering: ensures closures are allocated before non-lambda bindings
    // Tests letrec lambda reordering: lambdas must be allocated before non-lambda bindings
    // that reference them, to avoid null closure references in mutual recursion.
    [<Fact>]
    let ``LetRec_LetRecLambdaReordering`` () =
        let source = """
module Test

[<NoComparison; NoEquality>]
type Node = { Value: int; GetLabel: unit -> string }

let test () =
    let rec a = { Value = 1; GetLabel = labelA }
    and b = { Value = 2; GetLabel = labelB }
    and labelA () = sprintf "A(%d)->B(%d)" a.Value b.Value
    and labelB () = sprintf "B(%d)->A(%d)" b.Value a.Value
    
    if a.GetLabel() <> "A(1)->B(2)" then failwithf "Expected A(1)->B(2), got %s" (a.GetLabel())
    if b.GetLabel() <> "B(2)->A(1)" then failwithf "Expected B(2)->A(1), got %s" (b.GetLabel())
    printfn "SUCCESS"

[<EntryPoint>]
let main _ =
    test()
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/3660
    [<Fact>]
    let ``Issue_3660_ArrayOfFunctionsInvocationCorrectness`` () =
        let source =
            """
module Test

let runAll (fArr: (int -> int) array) x =
    let mutable n = 0

    for i = 0 to fArr.Length - 1 do
        n <- n + fArr[i] x

    n

[<EntryPoint>]
let main _ =
    let fns = [| (fun x -> x + 1); (fun x -> x * 2); (fun x -> x - 3) |]
    let result = runAll fns 10
    if result <> (11 + 20 + 7) then failwithf "Expected 38, got %d" result
    printfn "SUCCESS"
    0
"""

        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/3660
    // Verify that indexed array function invocation does not emit closure classes
    [<Fact>]
    let ``Issue_3660_NoClosureClassForIndexedArrayInvocation`` () =
        let source =
            """
module Test

let runAll (fArr: (int -> int) array) x =
    let mutable n = 0

    for i = 0 to fArr.Length - 1 do
        n <- n + fArr[i] x

    n
"""

        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [
            "extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc"
        ]
        |> ignore

