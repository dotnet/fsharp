
#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.FsiTests
#endif

open FSharp.Compiler
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.SourceCodeServices

open NUnit.Framework
open FsUnit
open System
open System.IO
open System.Text

// Intialize output and input streams
let inStream = new StringReader("")
let outStream = new CompilerOutputStream()
let errStream = new CompilerOutputStream()

// Build command line arguments & start FSI session
let argv = [| "C:\\fsi.exe" |]
let allArgs = Array.append argv [|"--noninteractive"|]

#if NETCOREAPP2_0
let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
#else
let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration(fsi)
#endif
let fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, new StreamWriter(outStream), new StreamWriter(errStream))  

/// Evaluate expression & return the result
let evalExpression text =
    match fsiSession.EvalExpression(text) with
    | Some value -> sprintf "%A" value.ReflectionValue
    | None -> sprintf "null or no result"

let formatErrors (errs: FSharpErrorInfo[]) = 
   [ for err in errs do yield sprintf "%s %d,%d - %d,%d; %s" (match err.Severity with FSharpErrorSeverity.Error -> "error" | FSharpErrorSeverity.Warning -> "warning") err.StartLineAlternate err.StartColumn err.EndLineAlternate err.EndColumn err.Message ]

let showErrorsAndResult (x, errs) = 
   [ match x with 
       | Choice1Of2 res -> yield sprintf "result %A" res
       | Choice2Of2 (exn:exn) -> yield sprintf "exception %s" exn.Message
     yield! formatErrors errs ]

let showErrors (x, errs: FSharpErrorInfo[]) = 
   [ match x with 
       | Choice1Of2 () -> ()
       | Choice2Of2 (exn:exn) -> yield sprintf "exception %s" exn.Message
     yield! formatErrors errs ]

/// Evaluate expression & return the result
let evalExpressionNonThrowing text =
   let res, errs = fsiSession.EvalExpressionNonThrowing(text)
   [ match res with 
       | Choice1Of2 valueOpt -> 
            match valueOpt with 
            | Some value -> yield sprintf "%A" value.ReflectionValue
            | None -> yield sprintf "null or no result"
       | Choice2Of2 (exn:exn) -> yield sprintf "exception %s" exn.Message
     yield! formatErrors errs ]

// For some reason NUnit doesn't like running these FsiEvaluationSession tests. We need to work out why.
//#if INTERACTIVE
[<Test>]
let ``EvalExpression test 1``() = 
    evalExpression "42+1" |> shouldEqual "43"

[<Test>]
let ``EvalExpression test 1 nothrow``() = 
    evalExpressionNonThrowing "42+1" |> shouldEqual ["43"]

[<Test>]
// 'fsi' can be evaluated because we passed it in explicitly up above
let ``EvalExpression fsi test``() = 
    evalExpression "fsi" |> shouldEqual "FSharp.Compiler.Interactive.InteractiveSession"

[<Test>]
// 'fsi' can be evaluated because we passed it in explicitly up above
let ``EvalExpression fsi test 2``() = 
    fsiSession.EvalInteraction "fsi.AddPrinter |> ignore" 

[<Test>]
// 'fsi' can be evaluated because we passed it in explicitly up above
let ``EvalExpression fsi test 2 non throwing``() = 
    fsiSession.EvalInteractionNonThrowing "fsi.AddPrinter |> ignore" 
       |> showErrors
       |> shouldEqual []


[<Test>]
let ``EvalExpression typecheck failure``() = 
    (try evalExpression "42+1.0"  |> ignore
         false
     with e -> true)
    |> shouldEqual true

[<Test>]
let ``EvalExpression typecheck failure nothrow``() = 
    evalExpressionNonThrowing("42+1.0")
    |> shouldEqual 
          ["exception Operation could not be completed due to earlier error";
           "error 1,3 - 1,6; The type 'float' does not match the type 'int'";
           "error 1,2 - 1,3; The type 'float' does not match the type 'int'"]


[<Test>]
let ``EvalExpression function value 1``() = 
    fsiSession.EvalExpression "(fun x -> x + 1)"  |> fun s -> s.IsSome
    |> shouldEqual true

[<Test>]
let ``EvalExpression function value 2``() = 
    fsiSession.EvalExpression "fun x -> x + 1"  |> fun s -> s.IsSome
    |> shouldEqual true

[<Test>]
let ``EvalExpression function value 3``() = 
    fsiSession.EvalExpression "incr"  |> fun s -> s.IsSome
    |> shouldEqual true

[<Test>]
let ``EvalExpression display value 1``() = 
    let v = fsiSession.EvalExpression "[1..200]"  |> Option.get
    let s = fsiSession.FormatValue(v.ReflectionValue, v.ReflectionType)
    let equalToString (s1: string) (s2: string) = 
        s1.Replace("\r\n","\n") |> shouldEqual (s2.Replace("\r\n","\n"))

    s |> equalToString """[1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15; 16; 17; 18; 19; 20; 21; 22;
 23; 24; 25; 26; 27; 28; 29; 30; 31; 32; 33; 34; 35; 36; 37; 38; 39; 40; 41;
 42; 43; 44; 45; 46; 47; 48; 49; 50; 51; 52; 53; 54; 55; 56; 57; 58; 59; 60;
 61; 62; 63; 64; 65; 66; 67; 68; 69; 70; 71; 72; 73; 74; 75; 76; 77; 78; 79;
 80; 81; 82; 83; 84; 85; 86; 87; 88; 89; 90; 91; 92; 93; 94; 95; 96; 97; 98;
 99; 100; ...]"""
    begin 
      use _holder = 
        let origPrintLength = fsi.PrintLength
        fsi.PrintLength <- 200
        { new System.IDisposable with member __.Dispose() = fsi.PrintLength <- origPrintLength }
      let sB = fsiSession.FormatValue(v.ReflectionValue, v.ReflectionType)

      sB |> equalToString """[1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15; 16; 17; 18; 19; 20; 21; 22;
 23; 24; 25; 26; 27; 28; 29; 30; 31; 32; 33; 34; 35; 36; 37; 38; 39; 40; 41;
 42; 43; 44; 45; 46; 47; 48; 49; 50; 51; 52; 53; 54; 55; 56; 57; 58; 59; 60;
 61; 62; 63; 64; 65; 66; 67; 68; 69; 70; 71; 72; 73; 74; 75; 76; 77; 78; 79;
 80; 81; 82; 83; 84; 85; 86; 87; 88; 89; 90; 91; 92; 93; 94; 95; 96; 97; 98;
 99; 100; 101; 102; 103; 104; 105; 106; 107; 108; 109; 110; 111; 112; 113; 114;
 115; 116; 117; 118; 119; 120; 121; 122; 123; 124; 125; 126; 127; 128; 129;
 130; 131; 132; 133; 134; 135; 136; 137; 138; 139; 140; 141; 142; 143; 144;
 145; 146; 147; 148; 149; 150; 151; 152; 153; 154; 155; 156; 157; 158; 159;
 160; 161; 162; 163; 164; 165; 166; 167; 168; 169; 170; 171; 172; 173; 174;
 175; 176; 177; 178; 179; 180; 181; 182; 183; 184; 185; 186; 187; 188; 189;
 190; 191; 192; 193; 194; 195; 196; 197; 198; 199; 200]"""

    end
    let v2 = fsiSession.EvalExpression "(System.Math.PI, System.Math.PI*10.0)"  |> Option.get
    let s2 = fsiSession.FormatValue(v2.ReflectionValue, v2.ReflectionType)

    s2 |> equalToString "(3.141592654, 31.41592654)"

    begin 
        use _holder2 = 
            let orig = fsi.FloatingPointFormat
            fsi.FloatingPointFormat <- "g3"
            { new System.IDisposable with member __.Dispose() = fsi.FloatingPointFormat <- orig }

        let s2B = fsiSession.FormatValue(v2.ReflectionValue, v2.ReflectionType)

        s2B |> equalToString "(3.14, 31.4)"
    end



[<Test; Ignore("Failing test for #135")>]
let ``EvalExpression function value 4``() = 
    fsiSession.EvalInteraction  "let hello(s : System.IO.TextReader) = printfn \"Hello World\""
    fsiSession.EvalExpression "hello"  |> fun s -> s.IsSome
    |> shouldEqual true

[<Test>]
let ``EvalExpression runtime failure``() = 
    (try evalExpression """ (failwith "fail" : int) """  |> ignore
         false
     with e -> true)
    |> shouldEqual true

[<Test>]
let ``EvalExpression parse failure``() = 
    (try evalExpression """ let let let let x = 1 """  |> ignore
         false
     with e -> true)
    |> shouldEqual true

[<Test>]
let ``EvalExpression parse failure nothrow``() = 
    evalExpressionNonThrowing """ let let let let x = 1 """  
    |> shouldEqual 
          ["exception Operation could not be completed due to earlier error";
           "error 1,5 - 1,8; Unexpected keyword 'let' or 'use' in binding";
           "error 1,1 - 1,4; The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result."]

[<Test>]
let ``EvalInteraction typecheck failure``() = 
    (try fsiSession.EvalInteraction "let x = 42+1.0"  |> ignore
         false
     with e -> true)
    |> shouldEqual true

[<Test>]
let ``EvalInteraction typecheck failure nothrow``() = 
    fsiSession.EvalInteractionNonThrowing "let x = 42+1.0"  
    |> showErrors
    |> shouldEqual
      ["exception Operation could not be completed due to earlier error";
       "error 1,11 - 1,14; The type 'float' does not match the type 'int'";
       "error 1,10 - 1,11; The type 'float' does not match the type 'int'"]

[<Test>]
let ``EvalInteraction runtime failure``() = 
    (try fsiSession.EvalInteraction """let x = (failwith "fail" : int) """  |> ignore
         false
     with e -> true)
    |> shouldEqual true

[<Test>]
let ``EvalInteraction runtime failure nothrow``() = 
    fsiSession.EvalInteractionNonThrowing """let x = (failwith "fail" : int) """  
    |> showErrors
    |> shouldEqual ["exception fail"]

[<Test>]
let ``EvalInteraction parse failure``() = 
    (try fsiSession.EvalInteraction """ let let let let x =  """  |> ignore
         false
     with e -> true)
    |> shouldEqual false  // EvalInteraction doesn't fail for parse failures, it just reports errors.

[<Test>]
let ``EvalInteraction parse failure nothrow``() = 
    fsiSession.EvalInteractionNonThrowing """ let let let let x =  """  
    |> showErrors
    |> shouldEqual 
          ["exception Operation could not be completed due to earlier error";
           "error 1,5 - 1,8; Unexpected keyword 'let' or 'use' in binding";
           "warning 1,0 - 1,22; Possible incorrect indentation: this token is offside of context started at position (1:14). Try indenting this token further or using standard formatting conventions.";
           "warning 1,22 - 1,22; Possible incorrect indentation: this token is offside of context started at position (1:14). Try indenting this token further or using standard formatting conventions."]

[<Test>]
let ``PartialAssemblySignatureUpdated test``() = 
    let count = ref 0 
    fsiSession.PartialAssemblySignatureUpdated.Add(fun x -> count := count.Value + 1)
    count.Value |> shouldEqual 0
    fsiSession.EvalInteraction """ let x = 1 """  
    count.Value |> shouldEqual 1
    fsiSession.EvalInteraction """ let x = 1 """  
    count.Value |> shouldEqual 2


[<Test>]
let ``ParseAndCheckInteraction test 1``() = 
    fsiSession.EvalInteraction """ let xxxxxx = 1 """  
    fsiSession.EvalInteraction """ type CCCC() = member x.MMMMM()  = 1 + 1 """  
    let untypedResults, typedResults, _ = fsiSession.ParseAndCheckInteraction("xxxxxx") |> Async.RunSynchronously
    Path.GetFileName(untypedResults.FileName) |> shouldEqual "stdin.fsx"
    untypedResults.Errors.Length |> shouldEqual 0
    untypedResults.ParseHadErrors |> shouldEqual false

    // Check we can't get a declaration location for text in the F# interactive state (because the file doesn't exist)
    // TODO: check that if we use # line directives, then the file will exist correctly
    let identToken = FSharpTokenTag.IDENT
    typedResults.GetDeclarationLocationAlternate(1,6,"xxxxxx",["xxxxxx"]) |> Async.RunSynchronously |> shouldEqual (FSharpFindDeclResult.DeclNotFound  FSharpFindDeclFailureReason.NoSourceCode) 

    // Check we can get a tooltip for text in the F# interactive state
    let tooltip = 
        match typedResults.GetToolTipTextAlternate(1,6,"xxxxxx",["xxxxxx"],identToken)  |> Async.RunSynchronously with 
        | FSharpToolTipText [FSharpToolTipElement.Single(text, FSharpXmlDoc.None)] -> text
        | _ -> failwith "incorrect tool tip"

    Assert.True(tooltip.Contains("val xxxxxx : int"))

[<Test>]
let ``ParseAndCheckInteraction test 2``() = 
    let fileName1 = Path.Combine(Path.Combine(__SOURCE_DIRECTORY__, "data"), "testscript.fsx")
    File.WriteAllText(fileName1, "let x = 1")
    let interaction1 = 
        sprintf """
#load @"%s"  
let y = Testscript.x + 1 
"""        fileName1
    let untypedResults, typedResults, _ = fsiSession.ParseAndCheckInteraction interaction1 |> Async.RunSynchronously
    Path.GetFileName(untypedResults.FileName) |> shouldEqual "stdin.fsx"
    untypedResults.Errors.Length |> shouldEqual 0
    untypedResults.ParseHadErrors |> shouldEqual false


[<Test>]
let ``Bad arguments to session creation 1``() = 
    let inStream = new StringReader("")
    let outStream = new CompilerOutputStream()
    let errStream = new CompilerOutputStream()
    let errWriter = new StreamWriter(errStream)
    let fsiSession = 
        try 
           FsiEvaluationSession.Create(fsiConfig, [| "fsi.exe"; "-r:nonexistent.dll" |], inStream, new StreamWriter(outStream), errWriter) |> ignore
           false
        with _ -> true
    Assert.True fsiSession
    Assert.False (String.IsNullOrEmpty (errStream.Read())) // error stream contains some output
    Assert.True (String.IsNullOrEmpty (outStream.Read())) // output stream contains no output

[<Test>]
let ``Bad arguments to session creation 2``() = 
    let inStream = new StringReader("")
    let outStream = new CompilerOutputStream()
    let errStream = new CompilerOutputStream()
    let errWriter = new StreamWriter(errStream)
    let fsiSession = 
        try 
           FsiEvaluationSession.Create(fsiConfig, [| "fsi.exe"; "-badarg" |], inStream, new StreamWriter(outStream), errWriter) |> ignore
           false
        with _ -> true
    Assert.True fsiSession
    Assert.False (String.IsNullOrEmpty (errStream.Read())) // error stream contains some output
    Assert.True (String.IsNullOrEmpty (outStream.Read())) // output stream contains no output

[<Test>]
// Regression test for #184
let ``EvalScript accepts paths verbatim``() =
    // Path contains escape sequences (\b and \n)
    // Let's ensure the exception thrown (if any) is FileNameNotResolved
    (try
        let scriptPath = @"C:\bad\path\no\donut.fsx"
        fsiSession.EvalScript scriptPath |> ignore
        false
     with
        | e ->
            true)
    |> shouldEqual true

[<Test>]
// Regression test for #184
let ``EvalScript accepts paths verbatim nothrow``() =
    // Path contains escape sequences (\b and \n)
    // Let's ensure the exception thrown (if any) is FileNameNotResolved
    let scriptPath = @"C:\bad\path\no\donut.fsx"
    fsiSession.EvalScriptNonThrowing scriptPath 
    |> showErrors 
    |> List.map (fun s -> s.[0..20])  // avoid seeing the hardwired paths
    |> Seq.toList
    |> shouldEqual 
          ["exception Operation c";
           "error 1,0 - 1,33; Una"]


[<Test>]
let ``Disposing interactive session (collectible)``() =

    let createSession i =
        let defaultArgs = [|"fsi.exe";"--noninteractive";"--nologo";"--gui-"|]
        let sbOut = StringBuilder()
        use inStream = new StringReader("")
        use outStream = new StringWriter(sbOut)
        let sbErr = StringBuilder("")
        use errStream = new StringWriter(sbErr)

        let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
        use session = FsiEvaluationSession.Create(fsiConfig, defaultArgs, inStream, outStream, errStream, collectible=true)
        
        session.EvalInteraction <| sprintf "let x%i = 42" i

    // Dynamic assemblies should be collected and handle count should not be increased
    for i in 1 .. 50 do
        printfn "iteration %d" i
        createSession i

[<Test>]
let ``interactive session events``() =

        let defaultArgs = [|"fsi.exe";"--noninteractive";"--nologo";"--gui-"|]
        let sbOut = StringBuilder()
        use inStream = new StringReader("")
        use outStream = new StringWriter(sbOut)
        let sbErr = StringBuilder("")
        use errStream = new StringWriter(sbErr)

        let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
        let evals = ResizeArray()
        use evaluator = fsiConfig.OnEvaluation.Subscribe (fun eval -> evals.Add (eval.FsiValue, eval.Name, eval.SymbolUse))

        use session = FsiEvaluationSession.Create(fsiConfig, defaultArgs, inStream, outStream, errStream, collectible=true)
        session.EvalInteraction  "let x = 42"
        
        let value, name, symbol = evals.[0]
        name |> should equal "x"
        value.IsSome |> should equal true
        value.Value.ReflectionValue |> should equal 42
        symbol.Symbol.GetType() |> should equal typeof<FSharpMemberOrFunctionOrValue>
        symbol.Symbol.DisplayName |> should equal "x"

        session.EvalInteraction  "type C() = member x.P = 1"
        
        let value, name, symbol = evals.[1]
        name |> should equal "C"
        value.IsNone |> should equal true
        symbol.Symbol.GetType() |> should equal typeof<FSharpEntity>
        symbol.Symbol.DisplayName |> should equal "C"

        session.EvalInteraction  "module M = let x = ref 1"
        let value, name, symbol = evals.[2]
        name |> should equal "M"
        value.IsNone |> should equal true
        symbol.Symbol.GetType() |> should equal typeof<FSharpEntity>
        symbol.Symbol.DisplayName |> should equal "M"

let RunManually() = 
  ``EvalExpression test 1``() 
  ``EvalExpression test 1 nothrow``() 
  ``EvalExpression fsi test``() 
  ``EvalExpression fsi test 2``() 
  ``EvalExpression typecheck failure``() 
  ``EvalExpression typecheck failure nothrow``() 
  ``EvalExpression function value 1``() 
  ``EvalExpression function value 2``() 
  ``EvalExpression runtime failure``() 
  ``EvalExpression parse failure``() 
  ``EvalExpression parse failure nothrow``() 
  ``EvalInteraction typecheck failure``() 
  ``EvalInteraction typecheck failure nothrow``() 
  ``EvalInteraction runtime failure``() 
  ``EvalInteraction runtime failure nothrow``() 
  ``EvalInteraction parse failure``() 
  ``EvalInteraction parse failure nothrow``() 
  ``PartialAssemblySignatureUpdated test``() 
  ``ParseAndCheckInteraction test 1``() 
  ``Bad arguments to session creation 1``()
  ``Bad arguments to session creation 2``()
  ``EvalScript accepts paths verbatim``()
  ``EvalScript accepts paths verbatim nothrow``()
  ``interactive session events``()
  ``Disposing interactive session (collectible)``() 

//#endif
