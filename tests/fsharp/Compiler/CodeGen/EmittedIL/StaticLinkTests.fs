// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.IO
open System.Reflection
open FSharp.Test.Utilities
open NUnit.Framework

[<TestFixture>]
module StaticLinkTests =

    [<Test>]
    let ``Static link simple library``() =
        let module1 =
            let source =
                """
module Module1

type C() = class end
                """
            Compilation.Create(source, Fsx, Library)

        let module2 =
            let source =
                """
let y = Module1.C()
printfn "%A" y
                """
            Compilation.Create(source, Fsx, Exe, cmplRefs=[CompilationReference.CreateFSharp(module1, staticLink=true)])

        CompilerAssert.Execute(module2, 
            beforeExecute=(fun _ deps ->
                deps
                |> List.iter (fun dep -> try File.Delete dep with | _ -> ())))

    [<Test>]
    let ``Simple exe should fail to execute if dependency was not found and is not statically linked``() =
        let module1 =
            let source =
                """
module Module1

type C() = class end
                """
            Compilation.Create(source, Fsx, Library)

        let module2 =
            let source =
                """
let y = Module1.C()
printfn "%A" y
                """
            Compilation.Create(source, Fsx, Exe, cmplRefs=[CompilationReference.CreateFSharp module1])

        Assert.Throws<TargetInvocationException>(fun _ ->
            CompilerAssert.Execute(module2, 
                beforeExecute=(fun _ deps ->
                    deps
                    |> List.iter (fun dep -> try File.Delete dep with | _ -> ())))) |> ignore

    [<Test>]
    let ``Simple exe should execute if dependency was found and is not statically linked``() =
        let module1 =
            let source =
                """
module Module1

type C() = class end
                """
            Compilation.Create(source, Fsx, Library)

        let module2 =
            let source =
                """
let y = Module1.C()
printfn "%A" y
                """
            Compilation.Create(source, Fsx, Exe, cmplRefs=[CompilationReference.CreateFSharp module1])

        CompilerAssert.Execute module2

    [<Test>]
    let ``Static link quotes in multiple modules``() =
        let module1 =
            let source =
                """
module Module1

module Test =
    let inline run() = 
       <@ fun (output:'T[]) (input:'T[]) (length:int) ->
          let start = 0
          let mutable i = start
          while i < length do
             output.[i] <- input.[i]
             i <- i + 1 @>

    let bar() = 
        sprintf "%A" (run())

type C() = 

  [<ReflectedDefinition>]
  static member F x = (C(), System.DateTime.Now)
                """
            Compilation.Create(source, Fsx, Library)

        let module2 =
            let source =
                """

let a = Module1.Test.bar()
let b = sprintf "%A" (Module1.Test.run())

let test1 = (a=b)
type D() = 

  [<ReflectedDefinition>]
  static member F x = (Module1.C(), D(), System.DateTime.Now)


let z2 = Quotations.Expr.TryGetReflectedDefinition(typeof<Module1.C>.GetMethod("F"))
let s2 = (sprintf "%2000A" z2) 
let test2 = (s2 = "Some Lambda (x, NewTuple (NewObject (C), PropertyGet (None, Now, [])))")

let z3 = Quotations.Expr.TryGetReflectedDefinition(typeof<D>.GetMethod("F"))
let s3 = (sprintf "%2000A" z3) 
let test3 = (s3 = "Some Lambda (x, NewTuple (NewObject (C), NewObject (D), PropertyGet (None, Now, [])))")

#if EXTRAS
// Add some references to System.ValueTuple, and add a test case which statically links this DLL
let test4 = struct (3,4)
let test5 = struct (z2,z3)
#endif

if not test1 then 
    stdout.WriteLine "*** test1 FAILED"; 
    eprintf "FAILED, in-module result %s is different from out-module call %s" a b

if not test2 then 
    stdout.WriteLine "*** test2 FAILED"; 
    eprintf "FAILED, %s is different from expected" s2
if not test3 then 
    stdout.WriteLine "*** test3 FAILED"; 
    eprintf "FAILED, %s is different from expected" s3


if test1 && test2 && test3 then ()
else failwith "Test Failed"
                """
            Compilation.Create(source, Fsx, Exe, cmplRefs=[CompilationReference.CreateFSharp(module1, staticLink=true)])

        CompilerAssert.Execute(module2, ignoreWarnings=true)

    [<Test>]
    let ``Static link quotes in multiple modules - optimized``() =
        let module1 =
            let source =
                """
module Module1

module Test =
    let inline run() = 
       <@ fun (output:'T[]) (input:'T[]) (length:int) ->
          let start = 0
          let mutable i = start
          while i < length do
             output.[i] <- input.[i]
             i <- i + 1 @>

    let bar() = 
        sprintf "%A" (run())

type C() = 

  [<ReflectedDefinition>]
  static member F x = (C(), System.DateTime.Now)
                """
            Compilation.Create(source, Fsx, Library, [|"--optimize+"|])

        let module2 =
            let source =
                """

let a = Module1.Test.bar()
let b = sprintf "%A" (Module1.Test.run())

let test1 = (a=b)
type D() = 

  [<ReflectedDefinition>]
  static member F x = (Module1.C(), D(), System.DateTime.Now)


let z2 = Quotations.Expr.TryGetReflectedDefinition(typeof<Module1.C>.GetMethod("F"))
let s2 = (sprintf "%2000A" z2) 
let test2 = (s2 = "Some Lambda (x, NewTuple (NewObject (C), PropertyGet (None, Now, [])))")

let z3 = Quotations.Expr.TryGetReflectedDefinition(typeof<D>.GetMethod("F"))
let s3 = (sprintf "%2000A" z3) 
let test3 = (s3 = "Some Lambda (x, NewTuple (NewObject (C), NewObject (D), PropertyGet (None, Now, [])))")

#if EXTRAS
// Add some references to System.ValueTuple, and add a test case which statically links this DLL
let test4 = struct (3,4)
let test5 = struct (z2,z3)
#endif

if not test1 then 
    stdout.WriteLine "*** test1 FAILED"; 
    eprintf "FAILED, in-module result %s is different from out-module call %s" a b

if not test2 then 
    stdout.WriteLine "*** test2 FAILED"; 
    eprintf "FAILED, %s is different from expected" s2
if not test3 then 
    stdout.WriteLine "*** test3 FAILED"; 
    eprintf "FAILED, %s is different from expected" s3


if test1 && test2 && test3 then ()
else failwith "Test Failed"
                """
            Compilation.Create(source, Fsx, Exe, [|"--optimize+"|], [CompilationReference.CreateFSharp(module1, staticLink=true)])

        CompilerAssert.Execute(module2, ignoreWarnings=true)

    [<Test>]
    let ``Standalone linking``() =
        let source =
            """
module Module1

let _ = List.iter (fun s -> eprintf "%s" s) ["hello"; " "; "world"]
let _ = eprintfn "%s" "."
let _ = exit 0
            """

        let module1 = Compilation.Create(source, Fsx, Exe, [|"--standalone"|])

        CompilerAssert.Execute(module1, newProcess=true)

    [<Test>]
    let ``Standalone linking - optimized``() =
        let source =
            """
module Module1

let _ = List.iter (fun s -> eprintf "%s" s) ["hello"; " "; "world"]
let _ = eprintfn "%s" "."
let _ = exit 0
            """

        let module1 = Compilation.Create(source, Fsx, Exe, [|"--standalone"; "--optimize+"|])

        CompilerAssert.Execute(module1, newProcess=true)
