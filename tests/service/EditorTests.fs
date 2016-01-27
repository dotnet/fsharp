
#if INTERACTIVE
#r "../../Debug/net40/bin/FSharp.LanguageService.Compiler.dll"
#r "../../Debug/net40/bin/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.Editor
#endif

open NUnit.Framework
open FsUnit
open System
open System.IO
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common


let input = 
  """
  open System

  let foo() = 
    let msg = String.Concat("Hello"," ","world")
    if true then 
      printfn "%s" msg.
  """

[<Test>]
let ``Intro test`` () = 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 
    let identToken = FSharpTokenTag.IDENT

    // We only expect one reported error. However,
    // on Unix, using filenames like /home/user/Test.fsx gives a second copy of all parse errors due to the
    // way the load closure for scripts is generated. So this returns two identical errors
    (match typeCheckResults.Errors.Length with 1 | 2 -> true | _ -> false)  |> shouldEqual true

    // So we check that the messages are the same
    for msg in typeCheckResults.Errors do 
        printfn "Good! got an error, hopefully with the right text: %A" msg
        msg.Message.Contains("Missing qualification after '.'") |> shouldEqual true

    // Get tool tip at the specified location
    let tip = typeCheckResults.GetToolTipTextAlternate(4, 7, inputLines.[1], ["foo"], identToken) |> Async.RunSynchronously
    // Get declarations (autocomplete) for a location
    let decls =  typeCheckResults.GetDeclarationListInfo(Some untyped, 7, 23, inputLines.[6], [], "msg", fun _ -> false)|> Async.RunSynchronously
    [ for item in decls.Items -> item.Name ] |> shouldEqual
          ["Chars"; "Clone"; "CompareTo"; "Contains"; "CopyTo"; "EndsWith"; "Equals";
           "GetEnumerator"; "GetHashCode"; "GetType"; "GetTypeCode"; "IndexOf";
           "IndexOfAny"; "Insert"; "IsNormalized"; "LastIndexOf"; "LastIndexOfAny";
           "Length"; "Normalize"; "PadLeft"; "PadRight"; "Remove"; "Replace"; "Split";
           "StartsWith"; "Substring"; "ToCharArray"; "ToLower"; "ToLowerInvariant";
           "ToString"; "ToUpper"; "ToUpperInvariant"; "Trim"; "TrimEnd"; "TrimStart"]
    // Get overloads of the String.Concat method
    let methods = typeCheckResults.GetMethodsAlternate(5, 27, inputLines.[4], Some ["String"; "Concat"]) |> Async.RunSynchronously

    methods.MethodName  |> shouldEqual "Concat"

    // Print concatenated parameter lists
    [ for mi in methods.Methods do
        yield methods.MethodName , [ for p in mi.Parameters do yield p.Display ] ]
        |> shouldEqual
              [("Concat", ["[<ParamArray>] args: obj []"]);
               ("Concat", ["[<ParamArray>] values: string []"]);
               ("Concat", ["values: Collections.Generic.IEnumerable<'T>"]);
               ("Concat", ["values: Collections.Generic.IEnumerable<string>"]);
               ("Concat", ["arg0: obj"]); ("Concat", ["arg0: obj"; "arg1: obj"]);
               ("Concat", ["str0: string"; "str1: string"]);
               ("Concat", ["arg0: obj"; "arg1: obj"; "arg2: obj"]);
               ("Concat", ["str0: string"; "str1: string"; "str2: string"]);
               ("Concat", ["arg0: obj"; "arg1: obj"; "arg2: obj"; "arg3: obj"]);
               ("Concat", ["str0: string"; "str1: string"; "str2: string"; "str3: string"])]

[<Test>]
let ``GetMethodsAsSymbols should return all overloads of a method as FSharpSymbolUse`` () =

    let extractCurriedParams (symbol:FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mvf ->
            [for pg in mvf.CurriedParameterGroups do 
                for (p:FSharpParameter) in pg do 
                    yield p.DisplayName, p.Type.Format (symbol.DisplayContext)]
        | _ -> []

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input)
    let methodsSymbols = typeCheckResults.GetMethodsAsSymbols(5, 27, inputLines.[4], ["String"; "Concat"]) |> Async.RunSynchronously
    match methodsSymbols with
    | Some methods ->
        [ for ms in methods do
            yield ms.Symbol.DisplayName, extractCurriedParams ms ]
        |> List.sortBy (fun (_name, parameters) -> parameters.Length, (parameters |> List.map snd ))
        |> shouldEqual
            [("Concat", [("values", "Collections.Generic.IEnumerable<'T>")]);
             ("Concat", [("values", "Collections.Generic.IEnumerable<string>")]);
             ("Concat", [("arg0", "obj")]);
             ("Concat", [("args", "obj []")]);
             ("Concat", [("values", "string []")]);
             ("Concat", [("arg0", "obj"); ("arg1", "obj")]);
             ("Concat", [("str0", "string"); ("str1", "string")]);
             ("Concat", [("arg0", "obj"); ("arg1", "obj"); ("arg2", "obj")]);
             ("Concat", [("str0", "string"); ("str1", "string"); ("str2", "string")]);
             ("Concat", [("arg0", "obj"); ("arg1", "obj"); ("arg2", "obj"); ("arg3", "obj")]);
             ("Concat", [("str0", "string"); ("str1", "string"); ("str2", "string"); ("str3", "string")])]
    | None -> failwith "No symbols returned"


let input2 = 
        """
[<System.CLSCompliant(true)>]
let foo(x, y) = 
    let msg = String.Concat("Hello"," ","world")
    if true then 
        printfn "x = %d, y = %d" x y 
        printfn "%s" msg

type C() = 
    member x.P = 1
        """

[<Test>]
let ``Symbols basic test`` () = 

    let file = "/home/user/Test.fsx"
    let untyped2, typeCheckResults2 = parseAndTypeCheckFileInProject(file, input2)

    let partialAssemblySignature = typeCheckResults2.PartialAssemblySignature
    
    partialAssemblySignature.Entities.Count |> shouldEqual 1  // one entity

[<Test>]
let ``Symbols many tests`` () = 

    let file = "/home/user/Test.fsx"
    let untyped2, typeCheckResults2 = parseAndTypeCheckFileInProject(file, input2)

    let partialAssemblySignature = typeCheckResults2.PartialAssemblySignature
    
    partialAssemblySignature.Entities.Count |> shouldEqual 1  // one entity
    let moduleEntity = partialAssemblySignature.Entities.[0]

    moduleEntity.DisplayName |> shouldEqual "Test"

    let classEntity = moduleEntity.NestedEntities.[0]

    let fnVal = moduleEntity.MembersFunctionsAndValues.[0]

    fnVal.Accessibility.IsPublic |> shouldEqual true
    fnVal.Attributes.Count |> shouldEqual 1
    fnVal.CurriedParameterGroups.Count |> shouldEqual 1
    fnVal.CurriedParameterGroups.[0].Count |> shouldEqual 2
    fnVal.CurriedParameterGroups.[0].[0].Name.IsSome |> shouldEqual true
    fnVal.CurriedParameterGroups.[0].[1].Name.IsSome |> shouldEqual true
    fnVal.CurriedParameterGroups.[0].[0].Name.Value |> shouldEqual "x"
    fnVal.CurriedParameterGroups.[0].[1].Name.Value |> shouldEqual "y"
    fnVal.DeclarationLocation.StartLine |> shouldEqual 3
    fnVal.DisplayName |> shouldEqual "foo"
    fnVal.EnclosingEntity.DisplayName |> shouldEqual "Test"
    fnVal.EnclosingEntity.DeclarationLocation.StartLine |> shouldEqual 1
    fnVal.GenericParameters.Count |> shouldEqual 0
    fnVal.InlineAnnotation |> shouldEqual FSharpInlineAnnotation.OptionalInline
    fnVal.IsActivePattern |> shouldEqual false
    fnVal.IsCompilerGenerated |> shouldEqual false
    fnVal.IsDispatchSlot |> shouldEqual false
    fnVal.IsExtensionMember |> shouldEqual false
    fnVal.IsPropertyGetterMethod |> shouldEqual false
    fnVal.IsImplicitConstructor |> shouldEqual false
    fnVal.IsInstanceMember |> shouldEqual false
    fnVal.IsMember |> shouldEqual false
    fnVal.IsModuleValueOrMember |> shouldEqual true
    fnVal.IsMutable |> shouldEqual false
    fnVal.IsPropertySetterMethod |> shouldEqual false
    fnVal.IsTypeFunction |> shouldEqual false

    fnVal.FullType.IsFunctionType |> shouldEqual true // int * int -> unit
    fnVal.FullType.GenericArguments.[0].IsTupleType |> shouldEqual true // int * int 
    let argTy1 = fnVal.FullType.GenericArguments.[0].GenericArguments.[0]

    argTy1.TypeDefinition.DisplayName |> shouldEqual "int" // int

    argTy1.HasTypeDefinition |> shouldEqual true
    argTy1.TypeDefinition.IsFSharpAbbreviation |> shouldEqual true // "int"

    let argTy1b = argTy1.TypeDefinition.AbbreviatedType
    argTy1b.TypeDefinition.Namespace |> shouldEqual (Some "Microsoft.FSharp.Core")
    argTy1b.TypeDefinition.CompiledName |> shouldEqual "int32" 

    let argTy1c = argTy1b.TypeDefinition.AbbreviatedType
    argTy1c.TypeDefinition.Namespace |> shouldEqual (Some "System")
    argTy1c.TypeDefinition.CompiledName |> shouldEqual "Int32" 

    let typeCheckContext = typeCheckResults2.ProjectContext
    
    typeCheckContext.GetReferencedAssemblies() |> List.exists (fun s -> s.FileName.Value.Contains("mscorlib")) |> shouldEqual true
    

let input3 = 
  """
let date = System.DateTime.Now.ToString().PadRight(25)
  """

[<Test>]
let ``Expression typing test`` () = 

    // Split the input & define file name
    let inputLines = input3.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input3) 
    let identToken = FSharpTokenTag.IDENT

    // We only expect one reported error. However,
    // on Unix, using filenames like /home/user/Test.fsx gives a second copy of all parse errors due to the
    // way the load closure for scripts is generated. So this returns two identical errors
    typeCheckResults.Errors.Length |> shouldEqual 0

    // Get declarations (autocomplete) for a location
    //
    // Getting the declarations at columns 42 to 43 with [], "" for the names and residue 
    // gives the results for the string type. 
    // 
    for col in 42..43 do 
        let decls =  typeCheckResults.GetDeclarationListInfo(Some untyped, 2, col, inputLines.[1], [], "", fun _ -> false)|> Async.RunSynchronously
        set [ for item in decls.Items -> item.Name ] |> shouldEqual
           (set
              ["Chars"; "Clone"; "CompareTo"; "Contains"; "CopyTo"; "EndsWith"; "Equals";
               "GetEnumerator"; "GetHashCode"; "GetType"; "GetTypeCode"; "IndexOf";
               "IndexOfAny"; "Insert"; "IsNormalized"; "LastIndexOf"; "LastIndexOfAny";
               "Length"; "Normalize"; "PadLeft"; "PadRight"; "Remove"; "Replace"; "Split";
               "StartsWith"; "Substring"; "ToCharArray"; "ToLower"; "ToLowerInvariant";
               "ToString"; "ToUpper"; "ToUpperInvariant"; "Trim"; "TrimEnd"; "TrimStart"])

// The underlying problem is that the parser error recovery doesn't include _any_ information for
// the incomplete member:
//    member x.Test = 

[<Test; Ignore("Currently failing, see #139")>]
let ``Find function from member 1`` () = 
    let input = 
      """
type Test() = 
    let abc a b c = a + b + c
    member x.Test = """ 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 

    let decls = typeCheckResults.GetDeclarationListInfo(Some untyped, 4, 21, inputLines.[3], [], "", fun _ -> false)|> Async.RunSynchronously
    let item = decls.Items |> Array.tryFind (fun d -> d.Name = "abc")
    match item with
    | Some item -> 
       printf "%s" item.Name
    | _ -> ()
    decls.Items |> Seq.exists (fun d -> d.Name = "abc") |> shouldEqual true

[<Test>]
let ``Find function from member 2`` () = 
    let input = 
      """
type Test() = 
    let abc a b c = a + b + c
    member x.Test = a""" 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 

    let decls = typeCheckResults.GetDeclarationListInfo(Some untyped, 4, 22, inputLines.[3], [], "", fun _ -> false)|> Async.RunSynchronously
    let item = decls.Items |> Array.tryFind (fun d -> d.Name = "abc")
    match item with
    | Some item -> 
       printf "%s" item.Name
    | _ -> ()
    decls.Items |> Seq.exists (fun d -> d.Name = "abc") |> shouldEqual true
 
[<Test>]
let ``Find function from var`` () = 
    let input = 
      """
type Test() = 
    let abc a b c = a + b + c
    let test = """ 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 

    let decls = typeCheckResults.GetDeclarationListInfo(Some untyped, 4, 15, inputLines.[3], [], "", fun _ -> false)|> Async.RunSynchronously
    decls.Items |> Seq.exists (fun d -> d.Name = "abc") |> shouldEqual true

[<Test; Ignore("Currently failing, see #139")>]
let ``Symbol based find function from member 1`` () = 
    let input = 
      """
type Test() = 
    let abc a b c = a + b + c
    member x.Test = """ 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 

    let decls = typeCheckResults.GetDeclarationListSymbols(Some untyped, 4, 21, inputLines.[3], [], "", fun _ -> false)|> Async.RunSynchronously
    let item = decls |> List.tryFind (fun d -> d.Head.Symbol.DisplayName = "abc")
    match item with
    | Some items -> 
       for symbolUse in items do
           printf "%s" symbolUse.Symbol.DisplayName
    | _ -> ()
    decls |> Seq.exists (fun d -> d.Head.Symbol.DisplayName = "abc") |> shouldEqual true

[<Test>]
let ``Symbol based find function from member 2`` () = 
    let input = 
      """
type Test() = 
    let abc a b c = a + b + c
    member x.Test = a""" 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 

    let decls = typeCheckResults.GetDeclarationListSymbols(Some untyped, 4, 22, inputLines.[3], [], "", fun _ -> false)|> Async.RunSynchronously
    let item = decls |> List.tryFind (fun d -> d.Head.Symbol.DisplayName = "abc")
    match item with
    | Some items -> 
       for symbolUse in items do
           printf "%s" symbolUse.Symbol.DisplayName
    | _ -> ()
    decls |> Seq.exists (fun d -> d.Head.Symbol.DisplayName = "abc") |> shouldEqual true
    true |> should equal true

[<Test>]
let ``Symbol based find function from var`` () = 
    let input = 
      """
type Test() = 
    let abc a b c = a + b + c
    let test = """ 

    // Split the input & define file name
    let inputLines = input.Split('\n')
    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults =  parseAndTypeCheckFileInProject(file, input) 

    let decls = typeCheckResults.GetDeclarationListSymbols(Some untyped, 4, 15, inputLines.[3], [], "", fun _ -> false)|> Async.RunSynchronously
    decls|> Seq .exists (fun d -> d.Head.Symbol.DisplayName = "abc") |> shouldEqual true

[<Test>]
let ``Printf specifiers for regular and verbatim strings`` () = 
    let input = 
      """
let _ = Microsoft.FSharp.Core.Printf.printf "%A" 0
let _ = Printf.printf "%A" 0
let _ = Printf.kprintf (fun _ -> ()) "%A" 1
let _ = Printf.bprintf null "%A" 1
let _ = sprintf "%*d" 1
let _ = sprintf "%7.1f" 1.0
let _ = sprintf "%-8.1e+567" 1.0
let _ = sprintf @"%-5s" "value"
let _ = printfn @"%-A" -10
let _ = printf @"
            %-O" -10
let _ = sprintf "

            %-O" -10
let _ = List.map (sprintf @"%A
                           ")
let _ = (10, 12) ||> sprintf "%A
                              %O"
let _ = sprintf "\n%-8.1e+567" 1.0
let _ = sprintf @"%O\n%-5s" "1" "2" """

    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults = parseAndTypeCheckFileInProject(file, input) 

    typeCheckResults.Errors |> shouldEqual [||]
    typeCheckResults.GetFormatSpecifierLocations() 
    |> Array.map (fun range -> range.StartLine, range.StartColumn, range.EndLine, range.EndColumn)
    |> shouldEqual [|(2, 45, 2, 46); 
                     (3, 23, 3, 24); 
                     (4, 38, 4, 39); 
                     (5, 29, 5, 30); 
                     (6, 17, 6, 19);
                     (7, 17, 7, 21); 
                     (8, 17, 8, 22);
                     (9, 18, 9, 21); 
                     (10, 18, 10, 20);
                     (12, 12, 12, 14); 
                     (15, 12, 15, 14);
                     (16, 28, 16, 29); 
                     (18, 30, 18, 31);
                     (19, 30, 19, 31);
                     (20, 19, 20, 24); 
                     (21, 18, 21, 19); (21, 22, 21, 25)|]

[<Test>]
let ``Printf specifiers for triple-quote strings`` () = 
    let input = 
      "
let _ = sprintf \"\"\"%-A\"\"\" -10
let _ = printfn \"\"\"
            %-A
                \"\"\" -10
let _ = List.iter(printfn \"\"\"%-A
                             %i\\n%O
                             \"\"\" 1 2)"

    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults = parseAndTypeCheckFileInProject(file, input) 

    typeCheckResults.Errors |> shouldEqual [||]
    typeCheckResults.GetFormatSpecifierLocations() 
    |> Array.map (fun range -> range.StartLine, range.StartColumn, range.EndLine, range.EndColumn)
    |> shouldEqual [|(2, 19, 2, 21);
                     (4, 12, 4, 14);
                     (6, 29, 6, 31);
                     (7, 29, 7, 30); (7, 33, 7, 34)|]
 
[<Test>]
let ``Printf specifiers for user-defined functions`` () = 
    let input = 
      """
let debug msg = Printf.kprintf System.Diagnostics.Debug.WriteLine msg
let _ = debug "Message: %i - %O" 1 "Ok"
let _ = debug "[LanguageService] Type checking fails for '%s' with content=%A and %A.\nResulting exception: %A" "1" "2" "3" "4"
"""

    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults = parseAndTypeCheckFileInProject(file, input) 

    typeCheckResults.Errors |> shouldEqual [||]
    typeCheckResults.GetFormatSpecifierLocations() 
    |> Array.map (fun range -> range.StartLine, range.StartColumn, range.EndLine, range.EndColumn)
    |> shouldEqual [|(3, 24, 3, 25); 
                     (3, 29, 3, 30);
                     (4, 58, 4, 59); (4, 75, 4, 76); (4, 82, 4, 83); (4, 108, 4, 109)|]

[<Test>]
let ``should not report format specifiers for illformed format strings`` () = 
    let input = 
      """
let _ = sprintf "%.7f %7.1A %7.f %--8.1f"
let _ = sprintf "%%A"
let _ = sprintf "ABCDE"
"""

    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults = parseAndTypeCheckFileInProject(file, input) 
    typeCheckResults.GetFormatSpecifierLocations() 
    |> Array.map (fun range -> range.StartLine, range.StartColumn, range.EndLine, range.EndColumn)
    |> shouldEqual [||]

[<Test>]
let ``Single case discreminated union type definition`` () = 
    let input = 
      """
type DU = Case1
"""

    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults = parseAndTypeCheckFileInProject(file, input) 
    typeCheckResults.GetAllUsesOfAllSymbolsInFile()
    |> Async.RunSynchronously
    |> Array.map (fun su -> 
        let r = su.RangeAlternate 
        r.StartLine, r.StartColumn, r.EndLine, r.EndColumn)
    |> shouldEqual [|(2, 10, 2, 15); (2, 5, 2, 7); (1, 0, 1, 0)|]

[<Test>]
let ``Synthetic symbols should not be reported`` () = 
    let input = 
      """
let arr = [|1|]
let number1, number2 = 1, 2
let _ = arr.[0..number1]
let _ = arr.[..number2]
"""

    let file = "/home/user/Test.fsx"
    let untyped, typeCheckResults = parseAndTypeCheckFileInProject(file, input) 
    typeCheckResults.GetAllUsesOfAllSymbolsInFile()
    |> Async.RunSynchronously
    |> Array.map (fun su -> 
        let r = su.RangeAlternate 
        su.Symbol.ToString(), (r.StartLine, r.StartColumn, r.EndLine, r.EndColumn))
    |> shouldEqual 
        [|("val arr", (2, 4, 2, 7)); 
          ("val number2", (3, 13, 3, 20));
          ("val number1", (3, 4, 3, 11)); 
          ("val arr", (4, 8, 4, 11));
          ("OperatorIntrinsics", (4, 11, 4, 12)); 
          ("Operators", (4, 11, 4, 12));
          ("Core", (4, 11, 4, 12)); 
          ("FSharp", (4, 11, 4, 12));
          ("Microsoft", (4, 11, 4, 12)); 
          ("val number1", (4, 16, 4, 23));
          ("val arr", (5, 8, 5, 11)); 
          ("OperatorIntrinsics", (5, 11, 5, 12));
          ("Operators", (5, 11, 5, 12)); 
          ("Core", (5, 11, 5, 12));
          ("FSharp", (5, 11, 5, 12)); 
          ("Microsoft", (5, 11, 5, 12));
          ("val number2", (5, 15, 5, 22)); 
          ("Test", (1, 0, 1, 0))|]

 