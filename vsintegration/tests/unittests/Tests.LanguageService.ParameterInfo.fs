// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService.ParameterInfo

open System
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

[<AutoOpen>]
module ParamInfoStandardSettings = 
    let standard40AssemblyRefs  = [| "System"; "System.Core"; "System.Numerics" |]
    let queryAssemblyRefs = [ "System.Xml.Linq"; "System.Core" ]

[<TestFixture>]
[<Category "LanguageService">] 
type UsingMSBuild()  = 
    inherit LanguageServiceBaseTests()

    let GetParamDisplays(methods:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip) =
            [ for i = 0 to methods.GetCount() - 1 do
                yield [ for j = 0 to methods.GetParameterCount(i) - 1 do
                            let (name,display,description) = methods.GetParameterInfo(i,j) 
                            yield display ] ]
      
    let AssertEmptyMethodGroup(resultMethodGroup:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip option) =
        Assert.IsTrue(resultMethodGroup.IsNone, "Expected an empty method group")              
        
    let AssertMethodGroupDesciptionsDoNotContain(methods:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip, expectNotToBeThere) = 
        for i = 0 to methods.GetCount() - 1 do
            let description = methods.GetDescription(i)
            if (description.Contains(expectNotToBeThere)) then
                Console.WriteLine("Expected description {0} to not contain {1}", description, expectNotToBeThere)
                AssertNotContains(description,expectNotToBeThere)
 
    let AssertMethodGroup(resultMethodGroup:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip option, expectedParamNamesSet:string list list) =
        Assert.IsTrue(resultMethodGroup.IsSome, "Expected a method group")
        let resultMethodGroup = resultMethodGroup.Value
        Assert.AreEqual(expectedParamNamesSet.Length, resultMethodGroup.GetCount())           
        Assert.IsTrue(resultMethodGroup 
                         |> GetParamDisplays
                         |> Seq.forall (fun paramDisplays -> 
                                expectedParamNamesSet |> List.exists (fun expectedParamNames -> 
                                       expectedParamNames.Length = paramDisplays.Length && 
                                       (expectedParamNames,paramDisplays) ||> List.forall2 (fun expectedParamName paramDisplay -> 
                                           paramDisplay.Contains(expectedParamName)))))
    
    let AssertMethodGroupContain(resultMethodGroup:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip option, expectedParamNames:string list) = 
        Assert.IsTrue(resultMethodGroup.IsSome, "Expected a method group")
        let resultMethodGroup = resultMethodGroup.Value
        Assert.IsTrue(resultMethodGroup
                          |> GetParamDisplays
                          |> Seq.exists (fun paramDisplays ->
                                expectedParamNames.Length = paramDisplays.Length &&
                                (expectedParamNames,paramDisplays) ||> List.forall2 (fun expectedParamName paramDisplay -> 
                                           paramDisplay.Contains(expectedParamName))))

    member private this.GetMethodListForAMethodTip(fileContents : string, marker : string, ?addtlRefAssy : list<string>) = 
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker(file, marker)

        GetParameterInfoAtCursor(file)

     //Verify all the overload method parameterInfo 
    member private this.VerifyParameterInfoAtStartOfMarker(fileContents : string, marker : string, expectedParamNamesSet:string list list, ?addtlRefAssy :list<string>) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        AssertMethodGroup(methodstr,expectedParamNamesSet)

   //Verify No parameterInfo at the marker     
    member private this.VerifyNoParameterInfoAtStartOfMarker(fileContents : string, marker : string, ?addtlRefAssy : list<string>) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        AssertEmptyMethodGroup(methodstr)

    //Verify one method parameterInfo if contained in parameterInfo list
    member private this.VerifyParameterInfoContainedAtStartOfMarker(fileContents : string, marker : string, expectedParamNames:string list, ?addtlRefAssy : list<string>) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        AssertMethodGroupContain(methodstr,expectedParamNames)

    //Verify the parameterInfo of one of the list order
    member private this.VerifyParameterInfoOverloadMethodIndex(fileContents : string, marker : string, index : int, expectedParams:string list, ?addtlRefAssy : list<string>) = 
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        Assert.IsTrue(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        let paramDisplays = 
            [ for i = 0 to methodstr.GetParameterCount(index) - 1 do
                let (name,display,description) = methodstr.GetParameterInfo(index,i)
                yield display]
        Assert.IsTrue((expectedParams, paramDisplays) ||> List.forall2 (fun expectedParam paramDisplay -> paramDisplay.Contains(expectedParam)))

    //Verify there is at least one parameterInfo
    member private this.VerifyHasParameterInfo(fileContents : string, marker : string) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker)
        Assert.IsTrue(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        Assert.IsTrue (methodstr.GetCount() > 0)

    //Verify return content after the colon
    member private this.VerifyFirstParameterInfoColonContent(fileContents : string, marker : string, expectedStr : string, ?addtlRefAssy : list<string>) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        Assert.IsTrue(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        Assert.AreEqual(expectedStr, methodstr.GetReturnTypeText(0)) // Expecting a method info like X(a:int,b:int) : int [used to be  X(a:int,b:int) -> int]

    member private this.VerifyParameterCount(fileContents : string, marker : string, expectedCount: int) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker)
        Assert.IsTrue(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value
        Assert.AreEqual(0, methodstr.GetParameterCount(expectedCount))

    [<Test>]
    member public this.``Regression.OnConstructor.881644``() =
        let fileContent = """new System.IO.StreamReader((*Mark*)"""
        let methodstr = this.GetMethodListForAMethodTip(fileContent,"(*Mark*)")
        Assert.IsTrue(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        if not (methodstr.GetDescription(0).Contains("#ctor")) then
            failwith "Expected parameter info to contain #ctor"

    [<Test>]
    member public this.``Regression.InsideWorkflow.6437``() =
        let fileContent = """
            open System.IO 
            let computation2 =
                async { use file = File.Open("",FileMode.Open)
                        let! buffer = file.AsyncRead((*Mark*)0)
                        return 0 }"""
        let methodstr = this.GetMethodListForAMethodTip(fileContent,"(*Mark*)")
        Assert.IsTrue(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        if not (methodstr.GetDescription(0).Contains("AsyncRead")) then
            failwith "Expected parameter info to contain AsyncRead"
    
    [<Test>]
    member public this.``Regression.MethodInfo.WithColon.Bug4518_1``() =
        let fileContent = """
            type T() =
                member this.X
                    with set ((a:int), (b:int)) (c:int) = ()
            ((new T()).X((*Mark*)"""
        this.VerifyFirstParameterInfoColonContent(fileContent,"(*Mark*)",": int")
    
    [<Test>]
    member public this.``Regression.MethodInfo.WithColon.Bug4518_2``() =
        let fileContent = """
           type IFoo = interface
                abstract f : int -> int
                    end
           let i : IFoo = null
           i.f((*Mark*)"""
        this.VerifyFirstParameterInfoColonContent(fileContent,"(*Mark*)",": int")
        
    [<Test>]
    member public this.``Regression.MethodInfo.WithColon.Bug4518_3``() =
        let fileContent = """
           type M() = 
                member this.f x = ()
           let m = new M()
           m.f((*Mark*)"""
        this.VerifyFirstParameterInfoColonContent(fileContent,"(*Mark*)",": unit")
        
    [<Test>]
    member public this.``Regression.MethodInfo.WithColon.Bug4518_4``() =
        let fileContent = """
           type T() =
                member this.Foo(a,b) = ""
           let t = new T()
           t.Foo((*Mark*)"""
        this.VerifyFirstParameterInfoColonContent(fileContent,"(*Mark*)",": string")    
        
    [<Test>]
    member public this.``Regression.MethodInfo.WithColon.Bug4518_5``() =
        let fileContent = """
           let f x y = x + y
           f((*Mark*)"""
        this.VerifyFirstParameterInfoColonContent(fileContent,"(*Mark*)",": (int -> int) ")  

    [<Test>]
    member public this.``Regression.StaticVsInstance.Bug3626.Case1``() =
        let fileContent = """
            type Foo() = 
                member this.Bar(instanceReturnsString:int) = "hllo"
                static member Bar(staticReturnsInt:int) = 13
            let z = Foo.Bar((*Mark*))"""
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark*)",[["staticReturnsInt"]])

    [<Test>]
    member public this.``Regression.StaticVsInstance.Bug3626.Case2``() =
        let fileContent = """
            type Foo() = 
                member this.Bar(instanceReturnsString:int) = "hllo"
                static member Bar(staticReturnsInt:int) = 13
            let Hoo = new Foo()
            let y = Hoo.Bar((*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark*)",[["instanceReturnsString"]])

    [<Test>]
    member public this.``Regression.MethodInfo.Bug808310``() =
        let fileContent = """System.Console.WriteLine((*Mark*)"""
        let methodGroup = this.GetMethodListForAMethodTip(fileContent,"(*Mark*)")   
        Assert.IsTrue(methodGroup.IsSome, "Expected a method group")
        let methodGroup = methodGroup.Value

        let description = methodGroup.GetDescription(0)
        // Make sure that System.Console.WriteLine is not mentioned anywhere exception in the XML comment signature
        let xmlCommentIndex = description.IndexOf("System.Console.WriteLine]")
        let noBracket =       description.IndexOf("System.Console.WriteLine")
        Assert.IsTrue(noBracket>=0)
        Assert.AreEqual(noBracket, xmlCommentIndex)

    [<Test>]
    member public this.``NoArguments``() =
        // we want to see e.g. 
        //     g() : int
        // and not
        //     g(unit) : int
        let fileContents = """
            type T =
                static member F() = 42
                static member G(x:unit) = 42

            let r1 = T.F((*1*))
            let r2 = T.G((*2*))

            let g() = 42
            let h((x:unit)) = 42
            let r3 = h((*3*))
            let r4 = g((*4*))"""
        this.VerifyParameterCount(fileContents,"(*1*)", 0)
        this.VerifyParameterCount(fileContents,"(*2*)", 0)
        this.VerifyParameterCount(fileContents,"(*3*)", 0)
        this.VerifyParameterCount(fileContents,"(*4*)", 0)

    [<Test>]
    member public this.``Single.Constructor1``() =
        let fileContent = """new System.DateTime((*Mark*)"""
        this.VerifyHasParameterInfo(fileContent, "(*Mark*)")
    
    [<Test>]
    member public this.``Single.Constructor2``() =
        let fileContent = """
            open System
            new DateTime((*Mark*)"""
        this.VerifyHasParameterInfo(fileContent, "(*Mark*)")

    [<Test>]
    member public this.``Single.DotNet.StaticMethod``() =
        let code = 
                                    ["#light"
                                     "System.Object.ReferenceEquals("
                                    ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file,"Object.ReferenceEquals(")
        let methodGroup = GetParameterInfoAtCursor file
        AssertMethodGroup(methodGroup, [["objA"; "objB"]])
        gpatcc.AssertExactly(0,0)

    [<Test>]
    member public this.``Regression.NoParameterInfo.100I.Bug5038``() =
        let fileContent = """100I((*Mark*)"""
        this.VerifyNoParameterInfoAtStartOfMarker(fileContent,"(*Mark*)")

    [<Test>]
    member public this.``Single.DotNet.InstanceMethod``() =
        let fileContent = """
            let s = "Hello"
            s.Substring((*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark*)",[["startIndex"]; ["startIndex"; "length"]])
    
    [<Test>]
    member public this.``Single.BasicFSharpFunction``() =
        let fileContent = """
            let foo(x) = 1
            foo((*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark*)",[["'a"]])
        
    [<Test>]
    member public this.``Single.DiscriminatedUnion.Construction``() =
        let fileContent = """
            type MyDU = 
              | Case1 of int * string
              | Case2 of V1 : int * string * V3 : bool
              | Case3 of ``Long Name`` : int * Item2 : string
              | Case4 of int
              
            let x1 = Case1((*Mark1*)
            let x2 = Case2((*Mark2*)
            let x3 = Case3((*Mark3*)
            let x4 = Case4((*Mark4*)
            """

        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark1*)",[["int"; "string"]])
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark2*)",[["V1: int"; "string"; "V3: bool"]])
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark3*)",[["Long Name: int"; "string"]])
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark4*)",[["int"]])
        
    [<Test>]
    member public this.``Single.Exception.Construction``() =
        let fileContent = """
            exception E1 of int * string
            exception E2 of V1 : int * string * V3 : bool
            exception E3 of ``Long Name`` : int * Data1 : string
              
            let x1 = E1((*Mark1*)
            let x2 = E2((*Mark2*)
            let x3 = E3((*Mark3*)
            """

        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark1*)",[["int"; "string"]])
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark2*)",[["V1: int"; "string"; "V3: bool" ]])
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark3*)",[["Long Name: int"; "string" ]])
    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo on a provided type that exposes one (static) method that takes one argument works normally.
    member public this.``TypeProvider.StaticMethodWithOneParam`` () =
        let fileContent = """
            let foo = N1.T1.M1((*Marker*)
            """
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Marker*)",[["arg1"]],
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
                  
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo on a provided type that exposes a (static) method that takes >1 arguments works normally.
    member public this.``TypeProvider.StaticMethodWithMoreParam`` () =
        let fileContent = """
            let foo = N1.T1.M2((*Marker*)
            """
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Marker*)",[["arg1";"arg2"]],
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case verify the TypeProvider static method return type or colon content of the method
    //This test verifies that ParamInfo on a provided type that exposes one (static) method that takes one argument
    //and returns something works correctly (more precisely, it checks that the return type is 'int')
    member public this.``TypeProvider.StaticMethodColonContent`` () =
        let fileContent = """
            let foo = N1.T1.M2((*Marker*)
            """
        this.VerifyFirstParameterInfoColonContent(fileContent,"(*Marker*)",": int",
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
    

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo on a provided type that exposes a Constructor that takes no argument works normally.
    member public this.``TypeProvider.ConstructorWithNoParam`` () =
        let fileContent = """
            let foo = new N1.T1((*Marker*)
            """
        this.VerifyParameterInfoOverloadMethodIndex(fileContent,"(*Marker*)",0,[],
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
              
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo on a provided type that exposes a Constructor that takes one argument works normally.
    member public this.``TypeProvider.ConstructorWithOneParam`` () =
        let fileContent = """
            let foo = new N1.T1((*Marker*)
            """
        this.VerifyParameterInfoOverloadMethodIndex(fileContent,"(*Marker*)",1,["arg1"],
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
         
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo on a provided type that exposes a Constructor that takes >1 argument works normally.
    member public this.``TypeProvider.ConstructorWithMoreParam`` () =
        let fileContent = """
            let foo = new N1.T1((*Marker*)
            """
        this.VerifyParameterInfoOverloadMethodIndex(fileContent,"(*Marker*)",2,["arg1";"arg2"],
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
         
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo on a provided type that exposes a static parameter that takes >1 argument works normally.
    member public this.``TypeProvider.Type.WhenOpeningBracket`` () =
        let fileContent = """
            type foo = N1.T<(*Marker*)
            """
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Marker*)",[["Param1";"ParamIgnored"]],
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
        
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that after closing bracket ">" the ParamInfo isn't showing on a provided type that exposes a static parameter that takes >1 argument works normally.
    //This is a regression test for Bug DevDiv:181000
    member public this.``TypeProvider.Type.AfterCloseBracket`` () =
        let fileContent = """
            type foo = N1.T< "Hello", 2>(*Marker*)
            """
        this.VerifyNoParameterInfoAtStartOfMarker(fileContent,"(*Marker*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo is showing after delimiter "," on a provided type that exposes a static parameter that takes >1 argument works normally.
    member public this.``TypeProvider.Type.AfterDelimiter`` () =
        let fileContent = """
            type foo = N1.T<"Hello",(*Marker*)
            """
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContent,"(*Marker*)",["Param1";"ParamIgnored"],
             [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
        
              
    [<Test>]
    member public this.``Single.InMatchClause``() =
        let fileContent = """
            let rec f l = 
                match l with
                | [] -> System.String.Format((*Mark*)
                | x :: xs -> f xs"""
        // Note, 3 of these 8 are only available on .NET 4.6.1.  On .NET 4.5 only 5 overloads are returned.
        this.VerifyParameterInfoAtStartOfMarker(fileContent,"(*Mark*)",[["format"; "arg0"];
                                                                        ["format"; "args"];
                                                                        ["provider"; "format"; "args"];
                                                                        ["provider"; "format"; "arg0"];
                                                                        ["format"; "arg0"; "arg1"];
                                                                        ["provider"; "format"; "arg0"; "arg1"];
                                                                        ["format"; "arg0"; "arg1"; "arg2"];
                                                                        ["provider"; "format"; "arg0"; "arg1"; "arg2"]])
  
    (* --- Parameter Info Systematic Tests ------------------------------------------------- *)

    member public this.TestSystematicParameterInfo (marker, methReq, ?startOfMarker) =
        let code = 
                                   ["let arr = "
                                    "    seq { for c = 'a' to 'z' do yield c }"
                                    "    |> Seq.map ( fun c ->"
                                    "        async { let x = c.ToString() in"
                                    "                return System.String.Format(\"[{0}] for [{1}]\"(*loc-1*), x.ToUpperInvariant()(*loc-2*), c) })"
                                    "    |> Async.Parallel"
                                    "    |> Async.RunSynchronously"

                                    "let (alist: System.Collections.ArrayList) = System.Collections.ArrayList(2)"
                                    "alist.[0] |> ignore"
                                    "<@@ let x = 1 in x(*loc-8*) @@>"

                                    "type FunkyType ="
                                    "    private (*loc-4*)new() = {}"
                                    "    static member ConvertToInt32 (s : string) ="
                                    "        let mutable n = 0 in"
                                    "        let parseRes = System.Int32.TryParse(s, &n) in"
                                    "        if not parseRes then"
                                    "            raise (new System.ArgumentException(\"incorrect number format\"))"
                                    "        n"

                                    "type Fruit = | Apple | Banana"
                                    "type KeyValuePair = { Key : int; Value : float }"
                                    "let print (x : Fruit, kvp : KeyValuePair) = System.Console.WriteLine(x); System.Console.WriteLine(kvp)"
                                    "print ((*loc-9*)Banana, {Key = 0; Value = 0.0})"

                                    "type Emp = "
                                    "    [<DefaultValue((*loc-6*)true)>]"
                                    "    static val mutable private m_ID : int"
                                    "    static member private NextID () = Emp.m_ID <- Emp.m_ID + 1; Emp.m_ID"
                                    "    val mutable private m_EmpID   : int"
                                    "    val mutable private m_Name    : string"
                                    "    val mutable private m_Salary  : float"
                                    "    val mutable private m_DoB     : System.DateTime"
                                    "    (*loc-5*)"

                                    "    // Overloaded Constructors"
                                    "    public new() ="
                                    "       { m_EmpID  = Emp.NextID();"
                                    "         m_Name   = System.String.Empty;"
                                    "         m_Salary = 0.0;"
                                    "         m_DoB    = System.DateTime.Today }"

                                    "    public new(name, salary, dob) as self = "
                                    "       new Emp() then"
                                    "         self.m_Name   <- name"
                                    "         self.m_Salary <- salary"
                                    "         self.m_DoB    <- dob"

                                    "    public new(name, dob) ="
                                    "        new (*loc-3*)Emp(name, 0.0, dob)"

                                    "    // Overloaded methods"
                                    "    member this.IncreaseBy(amount : float  ) = this.m_Salary <- this.m_Salary + amount"
                                    "    member this.IncreaseBy(amount : int    ) = this.IncreaseBy(float(amount))"
                                    "    member this.IncreaseBy(amount : float32) = this.IncreaseBy(float(amount))"

                                    "let ``Random Number Generator`` = System.Random()"
                                    "let ``?Max!Value?`` = 100"
                                    "let swap (a, b) = (b, a)"

                                    "[ \"Kevin\", System.DateTime.Today.AddYears(-25); \"John\", new System.DateTime(1980, 1, 1) ]"
                                    "|> List.map ( fun a -> let pair = swap a in Emp(dob = fst pair, name = snd pair) )"
                                    "|> List.iter ( fun a -> a.IncreaseBy(``Random Number Generator``.Next((*loc-7*)``?Max!Value?``)) )"
                                    
                                    "System.Console.ReadLine("
                                    ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        match startOfMarker with
        | Some(start) when start = true
            -> MoveCursorToStartOfMarker(file, marker)
        | _ -> MoveCursorToEndOfMarker(file, marker)
        
        let methodGroup = GetParameterInfoAtCursor file
        if (methReq = []) then
            Assert.IsTrue(methodGroup.IsNone, "Expected no method group")
        else
            AssertMethodGroup(methodGroup, methReq)
            
    // Test on .NET functions with no parameter
    [<Test>]
    member public this.``Single.DotNet.NoParameters`` () = 
        this.TestSystematicParameterInfo("x.ToUpperInvariant(", [ [] ])

    // Test on .NET function with one parameter
    [<Test>]
    member public this.``Single.DotNet.OneParameter`` () = 
        this.TestSystematicParameterInfo("System.DateTime.Today.AddYears(", [ ["value: int"] ] )

    // Test appearance of PI on second parameter of .NET function
    [<Test>]
    [<Ignore("FSharp1.0:2394")>]
    member public this.``Single.DotNet.OnSecondParameter`` () = 
        this.TestSystematicParameterInfo("loc-1*),", [ ["format"; "args"];
                                                       ["format"; "arg0"];
                                                       ["provider"; "format"; "args"];
                                                       ["format"; "arg0"; "arg1"];
                                                       ["format"; "arg0"; "arg1"; "arg2"] ] )
    // Test on .NET functions with parameter array
    [<Test>]
    [<Ignore("FSharp1.0:2394")>]
    member public this.``Single.DotNet.ParameterArray`` () = 
        this.TestSystematicParameterInfo("loc-2*),", [ ["format"; "args"];
                                                       ["format"; "arg0"];
                                                       ["provider"; "format"; "args"];
                                                       ["format"; "arg0"; "arg1"];
                                                       ["format"; "arg0"; "arg1"; "arg2"] ] )
    // Test on .NET indexers
    [<Test>]
    [<Ignore("FSharp1.0:5245")>]
    member public this.``Single.DotNet.IndexerParameter`` () = 
        this.TestSystematicParameterInfo("alist.[", [ ["index: int"] ] )
    
    // Test on .NET parameters passed with 'out' keyword (byref)
    [<Test>]
    [<Ignore("FSharp1.0:2394")>]
    member public this.``Single.DotNet.ParameterByReference`` () = 
        this.TestSystematicParameterInfo("Int32.TryParse(s,", [ ["s: string"; "result: int byref"]; ["s"; "style"; "provider"; "result"] ] )
        
    // Test on reference type and value type paramaters (e.g. string & DateTime)
    [<Test>]
    member public this.``Single.DotNet.RefTypeValueType`` () = 
        this.TestSystematicParameterInfo("loc-3*)Emp(", [ [];
                                                          ["name: string"; "dob: System.DateTime"];
                                                          ["name: string"; "salary: float"; "dob: System.DateTime"] ] )
                                                          
    // Test PI does not pop up at point of definition/declaration
    [<Test>]
    [<Ignore("FSharp1.0:5160")>]
    member public this.``Single.Locations.PointOfDefinition`` () = 
        this.TestSystematicParameterInfo("loc-4*)new(", [ ] )
        this.TestSystematicParameterInfo("member ConvertToInt32 (", [ ] )
        this.TestSystematicParameterInfo("member this.IncreaseBy(", [ ] )
        
    // Test PI does not pop up on whitespace after type annotation
    [<Test>]
    [<Ignore("FSharp1.0:5244")>]
    member public this.``Single.Locations.AfterTypeAnnotation`` () = 
        this.TestSystematicParameterInfo("(*loc-5*)", [], true)        


    // Test PI does not pop up after non-parameterized properties
    [<Test>]
    member public this.``Single.Locations.AfterProperties`` () = 
        this.TestSystematicParameterInfo("System.DateTime.Today", [])
        //this.TestSystematicParameterInfo("(*loc-8*)", [], true)

    // Test PI does not pop up after non-function values
    [<Test>]
    member public this.``Single.Locations.AfterValues`` () = 
        this.TestSystematicParameterInfo("(*loc-8*)", [], true)
    
    // Test PI does not pop up after non-parameterized properties and after values
    [<Test>]
    member public this.``Single.Locations.EndOfFile`` () = 
        this.TestSystematicParameterInfo("System.Console.ReadLine(", [ [] ])
        
    // Test PI pop up on parameter list for attributes
    [<Test>]
    [<Ignore("FSharp1.0:5242")>]
    member public this.``Single.OnAttributes`` () = 
        this.TestSystematicParameterInfo("(*loc-6*)", [ []; [ "check: bool" ] ], true)
    
    // Test PI when quoted identifiers are used as parameter
    [<Test>]
    member public this.``Single.QuotedIdentifier`` () = 
        this.TestSystematicParameterInfo("(*loc-7*)", [ []; [ "maxValue" ]; [ "minValue"; "maxValue" ] ], true)
        
    // Test PI with parameters of custom type 
    [<Test>]
    member public this.``Single.RecordAndUnionType`` () = 
        this.TestSystematicParameterInfo("(*loc-9*)", [ [ "Fruit"; "KeyValuePair" ] ], true)

    (* --- End Of Parameter Info Systematic Tests ------------------------------------------ *)

(* Tests for Generic parameterinfos -------------------------------------------------------- *)
  
    member private this.TestGenericParameterInfo (testLine, methReq) =
        let code = [ "#light"; "open System"; "open System.Threading"; ""; testLine ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        MoveCursorToEndOfMarker(file, testLine)
        let methodGroup = GetParameterInfoAtCursor file
        if (methReq = []) then
            Assert.IsTrue(methodGroup.IsNone, "expected no method group")
        else
            AssertMethodGroup(methodGroup, methReq)
    
    [<Test>]
    member public this.``Single.Generics.Typeof``() =
        let sevenTimes l = [ l; l; l; l; l; l; l ]
        this.TestGenericParameterInfo("typeof<int>(", [])
    [<Test>]
    member public this.``Single.Generics.MathAbs``() =
        let sevenTimes l = [ l; l; l; l; l; l; l ]
        this.TestGenericParameterInfo("Math.Abs(", sevenTimes ["value"])
    [<Test>]
    member public this.``Single.Generics.ExchangeInt``() =
        let sevenTimes l = [ l; l; l; l; l; l; l ]
        this.TestGenericParameterInfo("Interlocked.Exchange<int>(", sevenTimes ["location1"; "value"])
    [<Test>]
    member public this.``Single.Generics.Exchange``() =
        let sevenTimes l = [ l; l; l; l; l; l; l ]
        this.TestGenericParameterInfo("Interlocked.Exchange(", sevenTimes ["location1"; "value"])
    [<Test>]
    member public this.``Single.Generics.ExchangeUnder``() =
        let sevenTimes l = [ l; l; l; l; l; l; l ]
        this.TestGenericParameterInfo("Interlocked.Exchange<_> (", sevenTimes ["location1"; "value"])
    [<Test>]
    member public this.``Single.Generics.Dictionary``() =
        this.TestGenericParameterInfo("System.Collections.Generic.Dictionary<_, option<int>>(", [ []; ["capacity"]; ["comparer"]; ["capacity"; "comparer"]; ["dictionary"]; ["dictionary"; "comparer"] ])
    [<Test>]
    member public this.``Single.Generics.List``() =
        this.TestGenericParameterInfo("new System.Collections.Generic.List< _ > ( ", [ []; ["capacity"]; ["collection"] ])
    [<Test>]
    member public this.``Single.Generics.ListInt``() =
        this.TestGenericParameterInfo("System.Collections.Generic.List<int>(", [ []; ["capacity"]; ["collection"] ])
    [<Test>]
    member public this.``Single.Generics.EventHandler``() =
        this.TestGenericParameterInfo("new System.EventHandler( ", [ [""] ]) // function arg doesn't have a name
    [<Test>]
    member public this.``Single.Generics.EventHandlerEventArgs``() =
        this.TestGenericParameterInfo("System.EventHandler<EventArgs>(", [ [""] ]) // function arg doesn't have a name
    [<Test>]
    member public this.``Single.Generics.EventHandlerEventArgsNew``() =
        this.TestGenericParameterInfo("new System.EventHandler<EventArgs> ( ", [ [""] ]) // function arg doesn't have a name

    // Split into multiple lines using "\n" and find the index of "$" (and remove it from the text)
    member private this.ExtractLineInfo (line:string) =
        let idx, lines, foundDollar = line.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray |> List.foldBack (fun l (idx, lines, foundDollar) ->
            let i = l.IndexOf("$")
            if i = -1 then (idx, l::lines, foundDollar) 
            else (l.Substring(0, i), l.Replace("$", "")::lines, true) ) <| ("", [], false)
        if not foundDollar then
            failwith "bad unit test: did not find '$' in input to mark cursor location!"
        idx, lines
        
    member public this.TestParameterInfoNegative (testLine, ?addtlRefAssy : list<string>) =
        let cursorPrefix, testLines = this.ExtractLineInfo testLine

        let code = 
                      [ "#light"
                        "open System"
                        "open System.Threading"
                        "open System.Collections.Generic"; ""] @ testLines
        let (_, _, file) = this.CreateSingleFileProject(code, ?references = addtlRefAssy)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.IsTrue(info.IsNone, "expected no parameter info")
        gpatcc.AssertExactly(0,0)
        
    member public this.TestParameterInfoLocation (testLine, expectedPos, ?addtlRefAssy : list<string>) =
        let cursorPrefix, testLines = this.ExtractLineInfo testLine
        let code =
                      [ "#light"
                        "open System"
                        "open System.Threading"
                        "open System.Collections.Generic"; ""] @ testLines
        let (_, _, file) = this.CreateSingleFileProject(code, ?references = addtlRefAssy)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.IsTrue(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual(expectedPos, info.GetColumnOfStartOfLongId())

    // Tests the current behavior, we may want to specify it differently in the future
    // There are more comments below that explain particular tricky cases
    

    [<Test>]
    member public this.``Single.Locations.Simple``() =
        this.TestParameterInfoLocation("let a = System.Math.Sin($", 8)
        
    [<Test>]
    member public this.``Single.Locations.LineWithSpaces``() =
        this.TestParameterInfoLocation("let r =\n"+
                                       "   System.Math.Abs($0)", 3) // on the beginning of "System", not line!
        
    [<Test>]
    member public this.``Single.Locations.FullCall``() =
        this.TestParameterInfoLocation("System.Math.Abs($0)", 0)
        
    [<Test>]
    member public this.``Single.Locations.SpacesAfterParen``() =
        this.TestParameterInfoLocation("let a = Math.Sign( $-10  )", 8)
        
    [<Test>]
    member public this.``Single.Locations.WithNamespace``() =
        this.TestParameterInfoLocation("let a = System.Threading.Interlocked.Exchange($", 8)
        
    [<Test>]
    member public this.``ParameterInfo.Locations.WithoutNamespace``() =
        this.TestParameterInfoLocation("let a = Interlocked.Exchange($", 8)
        
    [<Test>]
    member public this.``Single.Locations.WithGenericArgs``() =
        this.TestParameterInfoLocation("Interlocked.Exchange<int>($", 0)
        
    [<Test>]
    member public this.``Single.Locations.FunctionWithSpace``() =
        this.TestParameterInfoLocation("let a = sin 0$.0", 8) 
        
    [<Test>]
    member public this.``Single.Locations.MethodCallWithoutParens``() =
        this.TestParameterInfoLocation("let n = Math.Sin 1$0.0", 8)
        
    [<Test>]
    member public this.``Single.Locations.GenericCtorWithNamespace``() =
        this.TestParameterInfoLocation("let _ = new System.Collections.Generic.Dictionary<_, _>($)", 12) // on the beginning of "System" (not on "new")
        
    [<Test>]
    member public this.``Single.Locations.GenericCtor``() =
        this.TestParameterInfoLocation("let _ = new Dictionary<_, _>($)", 12) // on the beginning of "System" (not on "new")
 
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo location on a provided type with namespace that exposes static parameter that takes >1 argument works normally.
    member public this.``TypeProvider.Type.ParameterInfoLocation.WithNamespace`` () =
        this.TestParameterInfoLocation("type boo = N1.T<$",11,
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
 
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that ParamInfo location on a provided type without the namespace that exposes static parameter that takes >1 argument works normally.
    member public this.``TypeProvider.Type.ParameterInfoLocation.WithOutNamespace`` () =
        this.TestParameterInfoLocation("open N1 \n"+ 
                                       "type boo = T<$",
            expectedPos = 11,
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
 
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that no ParamInfo in a string for a provided type  that exposes static parameter that takes >1 argument works normally.
     //The intent here to make sure the ParamInfo is not shown when inside a string
    member public this.``TypeProvider.Type.Negative.InString`` () =
        this.TestParameterInfoNegative("type boo = \"N1.T<$\"",
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test verifies that no ParamInfo in a Comment for a provided type that exposes static parameter that takes >1 argument works normally.
    //The intent here to make sure the ParamInfo is not shown when inside a comment
    member public this.``TypeProvider.Type.Negative.InComment`` () =
        this.TestParameterInfoNegative("// type boo = N1.T<$",
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])
 

 // Following are tricky:
    // if we can't find end of the identifier on the current line, 
    // we *must* look at the previous line to find the location where NameRes info ends
    // so in these cases we can find the identifier and location of tooltip is beginning of it
    // (but in general, we don't search for it)
    [<Test>]
    member public this.``Single.Locations.Multiline.IdentOnPrevLineWithGenerics``() =
        this.TestParameterInfoLocation("let d = Dictionary<_, option< int >>  \n" +
                                       "                ( $  )", 8) // on the "D" (line untestable)

    [<Test>]
    member public this.``Single.Locations.Multiline.IdentOnPrevLine``() =
        this.TestParameterInfoLocation("do Console.WriteLine\n" + 
                                       "        ($\"Multiline\")", 3)
    [<Test>]
    member public this.``Single.Locations.Multiline.IdentOnPrevPrevLine``() =
        this.TestParameterInfoLocation("do Console.WriteLine\n" + 
                                       "        (  \n" +
                                       "          $ \"Multiline\")", 3)
             
    [<Test>]
    member public this.``Single.Locations.GenericCtorWithoutNew``() =
        this.TestParameterInfoLocation("let d = System.Collections.Generic.Dictionary<_, option< int >>   ( $ )", 8) // on "S" - standard 

    [<Test>]
    member public this.``Single.Locations.Multiline.GenericTyargsOnTheSameLine``() =
        this.TestParameterInfoLocation("let dict3 = System.Collections.Generic.Dictionary<_, \n" +
                                       "                option< int>>( $ )", 12) // on "S" (beginning of "System")
    [<Test>]
    member public this.``Single.Locations.Multiline.LongIdentSplit``() =
        this.TestParameterInfoLocation("let ll = new System.Collections.\n" +
                                       "                    Generic.List< _ > ($)", 13) // on "S" (beginning of "System")

    [<Test>]
    member public this.``Single.Locations.OperatorTrick3``() =
        this.TestParameterInfoLocation
            ("let mutable n = null\n" + 
             "let aaa = Interlocked.Exchange<obj>(&n$, new obj())", 10) // "I" of Interlocked
          
    // A several cases that are tricky and we don't want to show anything
    // in the following cases, we may return a location of an operator (its ambigous), but we don't want to show info about it!
    
    [<Test>]
    member public this.``Single.Negative.OperatorTrick1``() =
        this.TestParameterInfoNegative
            ("let fooo = 0\n" + 
             "             >($ 1 )") // this may be end of a generic args specification
                                       
    [<Test>]
    member public this.``Single.Negative.OperatorTrick2``() =
        this.TestParameterInfoNegative
            ("let fooo = 0\n" + 
             "             <($ 1 )")

    /// No intellisense in comments/strings!
    [<Test>]
    member public this.``Single.InString``() =        
        this.TestParameterInfoNegative
            ("let s = \"System.Console.WriteLine($)\"")

    /// No intellisense in comments/strings!
    [<Test>]
    member public this.``Single.InComment``() =        
        this.TestParameterInfoNegative
            ("// System.Console.WriteLine($)")
  
    [<Test>]
    member this.``Regression.LocationOfParams.AfterQuicklyTyping.Bug91373``() =        
        let code = [ "let f x = x   "
                     "let f1 y = y  "
                     "let z = f(    " ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)

        // In this case, we quickly type "f1(" and then see what parameter info would pop up.
        // This simulates the case when the user quickly types after the file has been TCed before.
        ReplaceFileInMemoryWithoutCoffeeBreak file (
                   [ "let f x = x   "
                     "let f1 y = y  "
                     "let z = f(f1( " ] )
        MoveCursorToEndOfMarker(file, "f1(")
        let info = GetParameterInfoAtCursor file // this will fall back to using the name environment, which is stale, but sufficient to look up the call to 'f1'
        Assert.IsTrue(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual("f1", info.GetName(0))
        // note about (5,0): service.fs adds three lines of empty text to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        AssertEqual([|(2,10);(2,12);(2,13);(5,0)|], info.GetNoteworthyParamInfoLocations())

    [<Test>]
    member this.``LocationOfParams.AfterQuicklyTyping.CallConstructor``() =        
        let code = [ "type Foo() = class end" ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)

        // In this case, we quickly type "new Foo(" and then see what parameter info would pop up.
        // This simulates the case when the user quickly types after the file has been TCed before.
        ReplaceFileInMemoryWithoutCoffeeBreak file (
                   [ "type Foo() = class end"
                     "let foo = new Foo(    " ] )
        MoveCursorToEndOfMarker(file, "new Foo(")
        // Note: no TakeCoffeeBreak(this.VS)
        let info = GetParameterInfoAtCursor file // this will fall back to using the name environment, which is stale, but sufficient to look up the call to 'f1'
        Assert.IsTrue(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual("Foo", info.GetName(0))
        // note about (4,0): service.fs adds three lines of empty text to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        AssertEqual([|(1,14);(1,17);(1,18);(4,0)|], info.GetNoteworthyParamInfoLocations())


(*
This does not currently work, because the 'fallback to name environment' does weird QuickParse-ing and mangled the long id "Bar.Foo".
We really need to rewrite some code paths here to use the real parse tree rather than QuickParse-ing.
    [<Test>]
    member this.``ParameterInfo.LocationOfParams.AfterQuicklyTyping.CallConstructorViaLongId.Bug94333``() =        
        let solution = CreateSolution(this.VS)
        let project = CreateProject(solution,"testproject")
        let code = [ "module Bar = type Foo() = class end" ]
        let file = AddFileFromText(project,"File1.fs", code)
        let file = OpenFile(project,"File1.fs")
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)

        // In this case, we quickly type "new Foo(" and then see what parameter info would pop up.
        // This simulates the case when the user quickly types after the file has been TCed before.
        ReplaceFileInMemoryWithoutCoffeeBreak file (
                   [ "module Bar = type Foo() = class end"
                     "let foo = new Bar.Foo(    " ] )
        MoveCursorToEndOfMarker(file, "new Bar.Foo(")
        // Note: no TakeCoffeeBreak(this.VS)
        let info = GetParameterInfoAtCursor file // this will fall back to using the name environment, which is stale, but sufficient to look up the call to 'f1'
        AssertEqual("Foo", info.GetName(0))
        // note about (4,0): service.fs adds three lines of empty text to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        AssertEqual([|(1,14);(1,21);(1,21);(4,0)|], info.GetNoteworthyParamInfoLocations())
*)

    [<Test>]
    member public this.``ParameterInfo.NamesOfParams``() =
        let testLines = [
            "type Foo ="
            "    static member F(a:int, b:bool, c:int, d:int, ?e:int) = ()"
            "let a = 42"
            "Foo.F(0,(a=42),d=3,?e=Some 4,c=2)"
            "// names are _,_,d,e,c" ]
        let (_, _, file) = this.CreateSingleFileProject(testLines)
        MoveCursorToStartOfMarker(file, "0")
        let info = GetParameterInfoAtCursor file
        Assert.IsTrue(info.IsSome, "expected parameter info")
        let info = info.Value
        let names = info.GetParameterNames()
        AssertEqual([| null; null; "d"; "e"; "c" |], names)

    // $ is the location of the cursor/caret
    // ^ marks all of these expected points:
    //     - start of the long id that is the method call containing the caret
    //     - end of the long id that is the method call containing the caret
    //     - open paren of the method call (or first char of arg expression if no open paren)
    //     - for every param, end of expr that is the param (or closeparen if no params (unit))
    member public this.TestParameterInfoLocationOfParams (testLine, ?markAtEOF, ?additionalReferenceAssemblies) =
        let cursorPrefix, testLines = this.ExtractLineInfo testLine
        let testLinesAndLocs = testLines |> List.mapi (fun i s ->
            let r = new System.Text.StringBuilder(s)
            let locs = 
                [while r.ToString().IndexOf('^') <> -1 do
                    let c = r.ToString().IndexOf('^')
                    r.Remove(c,1) |> ignore
                    yield (i,c)]
            r.ToString(), locs)
        let testLines = testLinesAndLocs |> List.map fst
        let expectedLocs = testLinesAndLocs |> List.map snd |> List.collect id |> List.toArray 
        // note: service.fs adds three lines of empty text to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        let expectedLocs = if defaultArg markAtEOF false then 
                                Array.append expectedLocs [| (testLines.Length-1)+3, 0 |] 
                           else 
                                expectedLocs
        let cursorPrefix = cursorPrefix.Replace("^","")

        let references = "System.Core"::(defaultArg additionalReferenceAssemblies [])
        let (_, _, file) = this.CreateSingleFileProject(testLines, references = references)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.IsTrue(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual(expectedLocs, info.GetNoteworthyParamInfoLocations()) 

    // These pin down known failing cases
    member public this.TestNoParameterInfo (testLine, ?additionalReferenceAssemblies) =
        let cursorPrefix, testLines = this.ExtractLineInfo testLine
        let cursorPrefix = cursorPrefix.Replace("^","")
        let testLinesAndLocs = testLines |> List.mapi (fun i s ->
            let r = new System.Text.StringBuilder(s)
            let locs = 
                [while r.ToString().IndexOf('^') <> -1 do
                    let c = r.ToString().IndexOf('^')
                    r.Remove(c,1) |> ignore
                    yield (i,c)]
            r.ToString(), locs)
        let testLines = testLinesAndLocs |> List.map fst
        let references = "System.Core"::(defaultArg additionalReferenceAssemblies [])
        let (_, _, file) = this.CreateSingleFileProject(testLines, references = references)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.IsTrue(info.IsNone, "expected no parameter info for this particular test, though it would be nice if this has started to work")

    [<Test>]
    member public this.``LocationOfParams.Case1``() =        
        this.TestParameterInfoLocationOfParams("""^System.Console.WriteLine^(^"hel$lo"^)""")

    [<Test>]
    member public this.``LocationOfParams.Case2``() =        
        this.TestParameterInfoLocationOfParams("""^System.Console.WriteLine^   (^  "hel$lo {0}"  ,^ "Brian" ^)""")

    [<Test>]
    member public this.``LocationOfParams.Case3``() =        
        this.TestParameterInfoLocationOfParams(
            """^System.Console.WriteLine^  
                 (^  
                        "hel$lo {0}"  ,^ 
                        "Brian" ^)  """)

    [<Test>]
    member public this.``LocationOfParams.Case4``() =        
        this.TestParameterInfoLocationOfParams("""^System.Console.WriteLine^   (^  "hello {0}"  ,^ ("tuples","don't $ confuse it") ^)""")

    [<Test>]
    member public this.``ParameterInfo.LocationOfParams.Bug112688``() =
        let testLines = [
            "let f x y = ()"
            "module MailboxProcessorBasicTests ="
            "    do f 0"
            "         0"
            "    let zz = 42"
            "    for timeout in [0; 10] do" 
            "      ()" ]
        let (_,_, file) = this.CreateSingleFileProject(testLines)
        MoveCursorToStartOfMarker(file, "let zz")
        // in the bug, this caused an assert to fire
        let info = GetParameterInfoAtCursor file
        ()

    [<Test>]
    member public this.``ParameterInfo.LocationOfParams.Bug112340``() =
        let testLines = [
            """let a = typeof<N."""
            """printfn "%A" a""" ]
        let (_,_, file) = this.CreateSingleFileProject(testLines)
        MoveCursorToStartOfMarker(file, "printfn")
        // in the bug, this caused an assert to fire
        let info = GetParameterInfoAtCursor file
        ()

    [<Test>]
    member public this.``Regression.LocationOfParams.Bug91479``() =        
        this.TestParameterInfoLocationOfParams("""let z = fun x -> x + ^System.Int16.Parse^(^$ """, markAtEOF=true)

    [<Test>]
    member public this.``LocationOfParams.Attributes.Bug230393``() =        
        this.TestParameterInfoLocationOfParams("""
            let paramTest((strA : string),(strB : string)) =
                strA + strB
            ^paramTest^(^ $ 
 
            [<^Measure>]
            type RMB
        """)

    [<Test>]
    member public this.``LocationOfParams.InfixOperators.Case1``() =        
        // infix operators like '+' do not give their own param info
        this.TestParameterInfoLocationOfParams("""^System.Console.WriteLine^(^"" + "$"^)""")

    [<Test>]
    member public this.``LocationOfParams.InfixOperators.Case2``() =        
        // infix operators like '+' do give param info when used as prefix ops
        this.TestParameterInfoLocationOfParams("""System.Console.WriteLine((^+^)(^$3^)(4))""")

    [<Test>]
    member public this.``LocationOfParams.GenericMethodExplicitTypeArgs()``() =        
        this.TestParameterInfoLocationOfParams("""
            type T<'a> =
                static member M(x:int, y:string) = x + y.Length
            let x = ^T<int>.M^(^1,^ $"test"^)    """)

    [<Test>]
    member public this.``LocationOfParams.InsideAMemberOfAType``() =        
        this.TestParameterInfoLocationOfParams("""
            type Widget(z) = 
                member x.a = (1 <> ^System.Int32.Parse^(^"$"^)) """)

    [<Test>]
    member public this.``LocationOfParams.InsidePropertyGettersAndSetters.Case1``() =        
        this.TestParameterInfoLocationOfParams("""
            type Widget(z) = 
                member x.P1 
                    with get() = ^System.Int32.Parse^(^"$"^)
                    and set(z) = System.Int32.Parse("") |> ignore
                member x.P2 with get() = System.Int32.Parse("")
                member x.P2 with set(z) = System.Int32.Parse("") |> ignore """)

    [<Test>]
    member public this.``LocationOfParams.InsidePropertyGettersAndSetters.Case2``() =        
        this.TestParameterInfoLocationOfParams("""
            type Widget(z) = 
                member x.P1 
                    with get() = System.Int32.Parse("")
                    and set(z) = ^System.Int32.Parse^(^"$"^) |> ignore
                member x.P2 with get() = System.Int32.Parse("")
                member x.P2 with set(z) = System.Int32.Parse("") |> ignore """)

    [<Test>]
    member public this.``LocationOfParams.InsidePropertyGettersAndSetters.Case3``() =        
        this.TestParameterInfoLocationOfParams("""
            type Widget(z) = 
                member x.P1 
                    with get() = System.Int32.Parse("")
                    and set(z) = System.Int32.Parse("") |> ignore
                member x.P2 with get() = ^System.Int32.Parse^(^"$"^)
                member x.P2 with set(z) = System.Int32.Parse("") |> ignore """)

    [<Test>]
    member public this.``LocationOfParams.InsidePropertyGettersAndSetters.Case4``() =        
        this.TestParameterInfoLocationOfParams("""
            type Widget(z) = 
                member x.P1 
                    with get() = System.Int32.Parse("")
                    and set(z) = System.Int32.Parse("") |> ignore
                member x.P2 with get() = System.Int32.Parse("")
                member x.P2 with set(z) = ^System.Int32.Parse^(^"$"^) |> ignore """)

    [<Test>]
    member public this.``LocationOfParams.InsideObjectExpression``() =        
        this.TestParameterInfoLocationOfParams("""
                let _ = { new ^System.Object^(^$^) with member __.GetHashCode() = 2}""")

    [<Test>]
    member public this.``LocationOfParams.Nested1``() =        
        this.TestParameterInfoLocationOfParams("""System.Console.WriteLine("hello {0}"  , ^sin^  (^4$2.0 ^) )""")


    [<Test>]
    member public this.``LocationOfParams.MatchGuard``() =        
        this.TestParameterInfoLocationOfParams("""match [1] with | [x] when ^box^(^$x^) <> null -> ()""")

    [<Test>]
    member public this.``LocationOfParams.Nested2``() =        
        this.TestParameterInfoLocationOfParams("""System.Console.WriteLine("hello {0}"  , ^sin^  4^$2.0^ )""")

    [<Test>]
    member public this.``LocationOfParams.Generics1``() =        
        this.TestParameterInfoLocationOfParams("""
            let f<'T,'U>(x:'T, y:'U) = (y,x)
            let r = ^f^<int,string>(^4$2,^""^)""")

    [<Test>]
    member public this.``LocationOfParams.Generics2``() =        
        this.TestParameterInfoLocationOfParams("""let x = ^System.Collections.Generic.Dictionary^<int,int>(^42,^n$ull^)""")

    [<Test>]
    member public this.``LocationOfParams.Unions1``() =        
        this.TestParameterInfoLocationOfParams("""
            type MyDU =
                | FOO of int * string
            let r = ^FOO^(^42,^"$"^) """)

    [<Test>]
    member public this.``LocationOfParams.EvenWhenOverloadResolutionFails.Case1``() =        
        this.TestParameterInfoLocationOfParams("""let a = new ^System.IO.FileStream^(^$^)""")

    [<Test>]
    member public this.``LocationOfParams.EvenWhenOverloadResolutionFails.Case2``() =        
        this.TestParameterInfoLocationOfParams("""
            open System.Collections.Generic
            open System.Linq
            let l = List<int>([||])
            ^l.Aggregate^(^$^) // was once a bug""")

    [<Test>]
    member public this.``LocationOfParams.BY_DESIGN.WayThatMismatchedParensFailOver.Case1``() =        
        // when only one 'statement' after the mismatched parens after a comma, the comma swallows it and it becomes a badly-indented
        // continuation of the expression from the previous line
        this.TestParameterInfoLocationOfParams("""
            type CC() =
                member this.M(a,b,c,d) = a+b+c+d
            let c = new CC()
            ^c.M^(^1,^2,^3,^ $
            c.M(1,2,3,4)""", markAtEOF=true)

    [<Test>]
    member public this.``LocationOfParams.BY_DESIGN.WayThatMismatchedParensFailOver.Case2``() =        
        // when multiple 'statements' after the mismatched parens after a comma, the parser sees a single argument to the method that
        // is a statement sequence, e.g. a bunch of discarded expressions.  That is, 
        //     c.M(1,2,3,
        //     c.M(1,2,3,4)
        //     c.M(1,2,3,4)
        //     c.M(1,2,3,4)
        // is like
        //     c.M(let r = 1,2,3,
        //                     c.M(1,2,3,4)
        //                 c.M(1,2,3,4)
        //                 c.M(1,2,3,4)
        //         in r)
        this.TestParameterInfoLocationOfParams("""
            type CC() =
                member this.M(a,b,c,d) = a+b+c+d
            let c = new CC()
            ^c.M^(^1,2,3, $
            c.M(1,2,3,4)
            c.M(1,2,3,4)
            c.M(1,2,3,4)""", markAtEOF=true)

    [<Test>]
    member public this.``LocationOfParams.Tuples.Bug91360.Case1``() =        
        this.TestParameterInfoLocationOfParams("""
            ^System.Console.WriteLine^(^ (4$2,43) ^) // oops""")

    [<Test>]
    member public this.``LocationOfParams.Tuples.Bug91360.Case2``() =        
        this.TestParameterInfoLocationOfParams("""
            ^System.Console.WriteLine^(^ $(42,43) ^) // oops""")

    [<Test>]
    member public this.``LocationOfParams.Tuples.Bug123219``() =
        this.TestParameterInfoLocationOfParams("""
            type Expr = | Num of int
            type T<'a>() = 
                member this.M1(a:int*string, b:'a -> unit) = ()
            let x = new T<Expr>()
 
            ^x.M1^(^(1,$ """, markAtEOF=true)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug91609.OtherCases.Open``() =        
        this.TestParameterInfoLocationOfParams("""
            let arr = Array.create 4 1
            arr.[1] <- ^System.Int32.Parse^(^$
            open^ System""")

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug91609.OtherCases.Module``() =        
        this.TestParameterInfoLocationOfParams("""
            let arr = Array.create 4 1
            arr.[1] <- ^System.Int32.Parse^(^$
            ^module Foo =
                let x = 42""")

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug91609.OtherCases.Namespace``() =        
        this.TestParameterInfoLocationOfParams("""
            namespace Foo
            module Bar =
                let arr = Array.create 4 1
                arr.[1] <- ^System.Int32.Parse^(^$
            namespace^ Other""")

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug91609.Ok``() =        
        this.TestParameterInfoLocationOfParams("""
            let arr = Array.create 4 1
            arr.[1] <- ^System.Int32.Parse^(^$
            let squares3 = () 
            ^type Expr = class end
            let rec Evaluate (env:Map<string,int>) exp = ()""")

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug91609.AlsoOk``() =        
        this.TestParameterInfoLocationOfParams("""
            let arr = Array.create 4 1
            arr.[1] <- System.Int32.Parse(int(int(int(^int^(^$
            let squares3 = () 
            ^type Expr = class end
            let rec Evaluate (env:Map<string,int>) exp = ()""")

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug91609.NowGood``() =        
        // This case originally failed, by Design, as there is a finite limit to how many unmatched parens we can handle before the parser gives up and fails catastrophically.
        // However now that we recover from more kinds of tokens, e.g. OBLOCKEND, we can easily go much much deeper, and so this case (and most practical cases) now succeeds.
        this.TestParameterInfoLocationOfParams("""
            let arr = Array.create 4 1
            arr.[1] <- System.Int32.Parse(int(int(int(int(int(int(^int^(^$
            let squares3 = () 
            ^type Expr = class end
            let rec Evaluate (env:Map<string,int>) exp = ()""")

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug150492.Case1``() =        
        this.TestParameterInfoLocationOfParams("""
            module Inner =
                ^System.Console.Write^(^$
                let y = 4 
            ^type Foo() = inherit obj()
            [<assembly:System.Security.AllowPartiallyTrustedCallersAttribute>]
            do () """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug150492.Case2``() =        
        // like previous test, but with explicit begin-end at module
        this.TestParameterInfoLocationOfParams("""
            module Inner = begin
                ^System.Console.Write^(^$
                let y = 4 
            ^end
            type Foo() = inherit obj()
            [<assembly:System.Security.AllowPartiallyTrustedCallersAttribute>]
            do () """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug150492.Case1.WhenExtraModule``() =        
        this.TestParameterInfoLocationOfParams("""
            module Program
            let xxx = 42
            type FooBaz() = class end
            module Inner =
                ^System.Console.Write^(^$
                let y = 4 
            ^type Foo() = inherit obj()
            [<assembly:System.Security.AllowPartiallyTrustedCallersAttribute>]
            do () """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParens.Bug150492.Case2.OkWhenExtraModule``() =        
        // like previous test, but with explicit begin-end at module
        this.TestParameterInfoLocationOfParams("""
            module Program
            let xxx = 42
            type FooBaz() = class end
            module Inner = begin
                ^System.Console.Write^(^$
                let y = 4 
            ^end
            type Foo() = inherit obj()
            [<assembly:System.Security.AllowPartiallyTrustedCallersAttribute>]
            do () """)

    [<Test>]
    member this.``LocationOfParams.InheritsClause.Bug192134``() =        
        this.TestParameterInfoLocationOfParams("""
            type B(x : int) = 
               new(x1:int, x2: int) = new B(10)
            type A() =
               inherit ^B^(^1$,^2^)""")

    [<Test>]
    member public this.``LocationOfParams.ThisOnceAsserted``() =        
            this.TestNoParameterInfo("""
                module CSVTypeProvider

                f(fun x ->
                    match args with
                    | [| y |] -> 
                        for name, kind in (headerNames,
                        rowType.AddMember(new ^ProvidedProperty^(^$
                        null                       
                    | _ -> failwith "unexpected generic params" )

                let rec emitRegKeyNamedType (container:TypeContainer) (typeName:string) (key:RegistryKey) =         
                    let keyType = 0
                    keyType

                match types |> Array.tryFind (fun ty -> ty.Name = typeName^) with _ -> ()""")

    [<Test>]
    member public this.``LocationOfParams.ThisOnceAssertedToo``() =        
            this.TestNoParameterInfo("""
                let readString() =
                    let x = 42
                    while ('"' = '""' then
                            () 
                        else
                            let sb = new System.Text.StringBuilder()
                            while true do
                                ($)  """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case1a``() =        
        this.TestParameterInfoLocationOfParams("""
            module Repro =
                for a in ^System.Int16.TryParse^(^$  
            ^module AA = 
                let x = 10 """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case1b``() =        
        this.TestParameterInfoLocationOfParams("""
            module Repro =
                for a in ^System.Int16.TryParse^(^"4$2"  
            ^module AA = 
                let x = 10 """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case1c``() =        
        this.TestParameterInfoLocationOfParams("""
            module Repro =
                for a in ^System.Int16.TryParse^(^"4$2",^  
            ^module AA = 
                let x = 10 """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case2a``() =        
        this.TestParameterInfoLocationOfParams("""
            module Repro =
                query { for a in ^System.Int16.TryParse^(^$   
            ^module AA = 
                let x = 10 """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case2b``() =        
        this.TestParameterInfoLocationOfParams("""
            module Repro =
                query { for a in ^System.Int16.TryParse^(^"4$2"  
            ^module AA = 
                let x = 10 """)

    [<Test>]
    member public this.``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case2c``() =        
        this.TestParameterInfoLocationOfParams("""
            module Repro =
                query { for a in ^System.Int16.TryParse^(^"4$2",^  
            ^module AA = 
                let x = 10 """)

    [<Test>]
    member public this.``LocationOfParams.QueryCustomOperation.Bug222128``() =        
        this.TestParameterInfoLocationOfParams("""
            type T() =
                 member x.GetCollection() = [1;2;3;4]
            let q2 = query {
               for e in T().GetCollection() do
                 where (e > 250)
                 ^skip^(^$  
            ^} """)

    [<Test>]
    member public this.``LocationOfParams.QueryCurlies.Bug204150.Case1``() =        
        this.TestParameterInfoLocationOfParams("""
            type T() =
                 member x.GetCollection() = [1;2;3;4]
            open System.Linq
            let q6 =
                  query {
                    for E in ^T().GetCollection().Aggregate^(^$
                  ^} """)

    [<Test>]
    member public this.``LocationOfParams.QueryCurlies.Bug204150.Case2``() =        
        this.TestParameterInfoLocationOfParams("""
            type T() =
                 member x.GetCollection() = [1;2;3;4]
            open System.Linq
            let q6 =
                  query {
                    for E in ^T().GetCollection().Aggregate^(^42$
                  ^} """)

    [<Test>]
    member public this.``LocationOfParams.QueryCurlies.Bug204150.Case3``() =        
        this.TestParameterInfoLocationOfParams("""
            type T() =
                 member x.GetCollection() = [1;2;3;4]
            open System.Linq
            let q6 =
                  query {
                    for E in ^T().GetCollection().Aggregate^(^42,^$
                  ^} """)

    [<Test>]
    member public this.``LocationOfParams.QueryCurlies.Bug204150.Case4``() =        
        this.TestParameterInfoLocationOfParams("""
            type T() =
                 member x.GetCollection() = [1;2;3;4]
            open System.Linq
            let q6 =
                  query {
                    for E in ^T().GetCollection().Aggregate^(^42,^ 43$
                  ^} """)

    (* Tests for type provider static argument parameterinfos ------------------------------------------ *)

    member public this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts (testLine:string, ?markAtEnd, ?additionalReferenceAssemblies) =
        let numSpacesOfIndent =
            let lines = testLine.Split[|'\n'|]
            let firstLineWithText = lines |> Array.find (fun s -> s |> Seq.exists (fun c -> not(Char.IsWhiteSpace c)))
            firstLineWithText |> Seq.findIndex (fun c -> not(Char.IsWhiteSpace c))
        let indent = String.replicate numSpacesOfIndent " "
        let contexts = [
            false, ""
            true,  "namespace Foo"
            false, "module Program"
            ]
        let prefixes = [
            true,  ""
            false, "let x = 42"
            false, "let f x = 42"
            true,  "type MyClass() = class end"
            ]
        let suffixes = [
            true,  ""
            false, "let x = 42"
            false, "let f x = 42"
            true,  "type MyClass2() = class end"
            true,  "module M = begin end"
            //true,  "namespace Bar"  // TODO only legal to test this if already in a namespace
            ]
        for isNamespace, startText in contexts do
        for p in prefixes |> List.filter (fun (okInNS,_) -> if isNamespace then okInNS else true) |> List.map snd do
        for s in suffixes |> List.filter (fun (okInNS,_) -> if isNamespace then okInNS else true) |> List.map snd do
        (
            let needMarkAtEnd = defaultArg markAtEnd false
            let s, needMarkAtEnd =
                if needMarkAtEnd && s<>"" then
                    "^"+s, false
                else
                    s, needMarkAtEnd
            let allText = indent + startText + Environment.NewLine 
                        + indent + p + Environment.NewLine 
                        + testLine + Environment.NewLine 
                        + indent + s
            printfn "-----------------"
            printfn "%s" allText
            this.TestParameterInfoLocationOfParams (allText, markAtEOF=needMarkAtEnd, ?additionalReferenceAssemblies=additionalReferenceAssemblies)
        )

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Basic``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ 42 ^>""", 
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.BasicNamed``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ ParamIgnored=42 ^>""", 
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])


    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Prefix0``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ $ """, // missing all params, just have <
            markAtEnd = true,
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Prefix1``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ 42 """, // missing >
            markAtEnd = true,
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Prefix1Named``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ ParamIgnored=42 """, // missing >
            markAtEnd = true,
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Prefix2``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ """, // missing last param
            markAtEnd = true,
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Prefix2Named1``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ ParamIgnored= """, // missing last param after name with equals
            markAtEnd = true,
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Prefix2Named2``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ ParamIgnored """, // missing last param after name sans equals
            markAtEnd = true,
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Negative1``() =       
            this.TestNoParameterInfo("""
                type D = ^System.Collections.Generic.Dictionary^<^ in$t, int ^>""")

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Negative2``() =       
            this.TestNoParameterInfo("""
                type D = ^System.Collections.Generic.List^<^ in$t ^>""")

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Negative3``() =       
            this.TestNoParameterInfo("""
                let i = 42
                let b = ^i^<^ 4$2""")

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Negative4.Bug181000``() =       
            this.TestNoParameterInfo("""
                type U = ^N1.T^<^ "foo",^ 42 ^>$  """,   // when the caret is right of the '>', we should not report any param info
                additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.BasicWithinExpr``() =
            this.TestNoParameterInfo("""
                let f() =
                    let r = id( ^N1.T^<^ "fo$o",^ ParamIgnored=42 ^> )
                    r    """, 
                additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.BasicWithinExpr.DoesNotInterfereWithOuterFunction``() =        
        this.TestParameterInfoLocationOfParams("""
            let f() =
                let r = ^id^(^ N1.$T< "foo", ParamIgnored=42 > ^)
                r    """, 
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Bug199744.ExcessCommasShouldNotAssertAndShouldGiveInfo.Case1``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ 42,^ ,^ ^>""", 
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Bug199744.ExcessCommasShouldNotAssertAndShouldGiveInfo.Case2``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ "fo$o",^ ,^ ^>""", 
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``LocationOfParams.TypeProviders.Bug199744.ExcessCommasShouldNotAssertAndShouldGiveInfo.Case3``() =        
        this.TestParameterInfoLocationOfParamsWithVariousSurroundingContexts("""
            type U = ^N1.T^<^ ,^$ ^>""", 
            additionalReferenceAssemblies = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])

    [<Test>]
    member public this.``TypeProvider.FormatOfNamesOfSystemTypes``() =
        let code = ["""type TTT = N1.T< "foo", ParamIgnored=42 > """]
        let references = [PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")]
        let (_, _, file) = this.CreateSingleFileProject(code, references = references)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file,"foo")
        let methodGroup = GetParameterInfoAtCursor file
        Assert.IsTrue(methodGroup.IsSome, "expected parameter info")
        let methodGroup = methodGroup.Value
        let actualDisplays =
            [ for i = 0 to methodGroup.GetCount() - 1 do
                yield [ for j = 0 to methodGroup.GetParameterCount(i) - 1 do
                            let (name,display,description) = methodGroup.GetParameterInfo(i,j) 
                            yield display ] ]
        let expected = [["Param1: string"; "ParamIgnored: int"]]  // key here is we want e.g. "int" and not "System.Int32"
        AssertEqual(expected, actualDisplays)
        gpatcc.AssertExactly(0,0)

    [<Test>]
    member public this.``ParameterNamesInFunctionsDefinedByLetBindings``() = 
        let useCases = 
            [
                """
                let foo (n1 : int) (n2 : int) = n1 + n2
                foo(
                """, "foo(", ["n1: int"]

                """
                let foo (n1 : int, n2 : int) = n1 + n2
                foo(
                """, "foo(", ["n1: int"; "n2: int"]

                """
                let foo (n1 : int, n2 : int) = n1 + n2
                foo(2,
                """, "foo(2,", ["n1: int"; "n2: int"]

                (* Negative tests - display only types*)
                """
                let foo = List.map
                foo(
                """, "foo(", ["'a -> 'b"]

                """
                let foo x = 
                    let bar y = x + y
                    bar(
                """, "bar(", ["int"]

                """
                type T() = 
                    let foo x = x + 1
                    member this.Run() = 
                        foo(
                """, "foo(", ["int"]

                """
                let f (Some x) = x + 1
                f(
                """, "f(", ["int option"]
            ]

        for (code, marker, expectedParams) in useCases do
            let (_, _, file) = this.CreateSingleFileProject(code)
            MoveCursorToEndOfMarker(file, marker)
            let methodGroup = GetParameterInfoAtCursor file
            
            Assert.IsTrue(methodGroup.IsSome, "expected parameter info")
            let methodGroup = methodGroup.Value

            Assert.AreEqual(1, methodGroup.GetCount(), "Only one function expected")            

            let expectedParamsCount = List.length expectedParams
            Assert.AreEqual(expectedParamsCount, methodGroup.GetParameterCount(0), sprintf "%d parameters expected" expectedParamsCount)
            
            let actualParams = [ for i = 0 to (expectedParamsCount - 1) do yield methodGroup.GetParameterInfo(0, i) ]               
            let ok = 
                actualParams
                |> List.map (fun (_, d, _) -> d)
                |> List.forall2 (=) expectedParams
            if not ok then
                printfn "==Parameters dont't match=="
                printfn "Expected parameters %A" expectedParams
                printfn "Actual parameters %A" actualParams
                Assert.Fail()
                  
    (* Tests for multi-parameterinfos ------------------------------------------------------------------ *)

    [<Test>]
    member public this.``ParameterInfo.ArgumentsWithParamsArrayAttribute``() =
        let content = """let _ = System.String.Format("",(*MARK*))"""
        let methodTip = this.GetMethodListForAMethodTip(content, "(*MARK*)")
        Assert.IsTrue(methodTip.IsSome, "expected parameter info")
        let methodTip = methodTip.Value

        let overloadWithTwoParamsOpt = 
            Seq.init (methodTip.GetCount()) (fun i -> 
                let count = methodTip.GetParameterCount(i)
                let paramInfos = 
                    [
                        for c = 0 to (count - 1) do
                            let name = ref ""
                            let display = ref ""
                            let description = ref ""
                            methodTip.GetParameterInfo(i, c, name, display, description)
                            yield !name, !display,!description
                    ]
                count, paramInfos
                )
            |> Seq.tryFind(fun (i, _) -> i = 2)
        match overloadWithTwoParamsOpt with
        | Some(_, [_;(_name, display, _description)]) -> Assert.IsTrue(display.Contains("[<System.ParamArray>] args"))
        | x -> Assert.Fail(sprintf "Expected overload not found, current result %A" x)

    (* DotNet functions for multi-parameterinfo tests -------------------------------------------------- *)
    [<Test>]
    member public this.``Multi.DotNet.StaticMethod``() =
        let fileContents = """System.Console.WriteLine("Today is {0:dd MMM yyyy}",(*Mark*)System.DateTime.Today)"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["string";"obj"])

    [<Test>]
    member public this.``Multi.DotNet.StaticMethod.WithinClassMember``() =
        let fileContents = """
            type Widget(z) = 
                member x.a = (1 <> System.Int32.Parse("",(*Mark*)

            let widget = Widget(1)
            45"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["string";"System.Globalization.NumberStyles"])

    [<Test>]
    member public this.``Multi.DotNet.StaticMethod.WithinLambda``() =
        let fileContents = """let z = fun x -> x + System.Int16.Parse("",(*Mark*)"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["string";"System.Globalization.NumberStyles"])

    [<Test>]
    member public this.``Multi.DotNet.StaticMethod.WithinLambda2``() = 
        let fileContents = "let _ = fun file -> new System.IO.FileInfo((*Mark*)"
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["string"]])

    [<Test>]
    member public this.``Multi.DotNet.InstanceMethod``() = 
        let fileContents = """
            let s = "Hello"
            s.Substring(0,(*Mark*)"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["int";"int"])

    (* Common functions for multi-parameterinfo tests -------------------------------------------------- *)
    [<Test>]
    member public this.``Multi.DotNet.Constructor``() = 
        let fileContents = "let _ = new System.DateTime(2010,12,(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["int";"int";"int"])

    [<Test>]
    member public this.``Multi.Constructor.WithinObjectExpression``() = 
        let fileContents = "let _ = { new System.Object((*Mark*)) with member __.GetHashCode() = 2}"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",[])

    [<Test>]
    member public this.``Multi.Function.InTheClassMember``() = 
        let fileContents = """
            type Foo() = 
                let foo1(a : int, b:int) = ()

                member this.A() = 
                    foo1(1,(*Mark*)
                member this.A(a : string, b:int) = ()"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int";"int"]])

    [<Test>]
    member public this.``Multi.ParamAsTupleType``() = 
        let fileContents = """
            let tuple((a : int, b : int), c : int) = a * b + c
            let result = tuple((1, 2)(*Mark*), 3)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int * int";"int"]])

    [<Test>]
    member public this.``Multi.ParamAsCurryType``() = 
        let fileContents = """
            let multi (x : float) (y : float) = 0
            let sum(a, b) = a + b
            let rtnValue = sum(multi (1.0(*Mark*)) 3.0, 5)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["float"]])

    [<Test>]
    member public this.``Multi.MethodInMatchCause``() = 
        let fileContents = """
            let rec f l = 
                    match l with
                    | [] -> System.String.Format("{0:X2}",(*Mark*)
                    | x :: xs -> f xs"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["string";"obj"])

    [<Test>]
    [<Ignore("93945 - No param info shown on the Indexer Property")>]
    member public this.``Regression.Multi.IndexerProperty.Bug93945``() = 
        let fileContents = """
            type Year2(year : int) =
              member this.Item (month : int, day : int) =
                let monthIdx =
                    match month with
                    | _ when month > 12 -> failwithf "Invalid month [%d]" month
                    | _ when month < 1 -> failwithf "Invalid month [%d]" month
                    | _ -> month
                let dateStr = sprintf "1-1-%d" year
                DateTime.Parse(dateStr).AddMonths(monthIdx - 1).AddDays(float (day - 1))

            let O'seven = new Year2(2007)
            let randomDay = O'seven.[12,(*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int";"int"]])

    [<Test>]
    [<Ignore("93188 - No param info shown in the Attribute memthod")>]
    member public this.``Regression.Multi.ExplicitAnnotate.Bug93188``() = 
        let fileContents = """
            type LiveAnimalAttribute(a : int, b: string) =
                inherit System.Attribute()

            [<LiveAnimal(1,(*Mark*)"Bat")>]
            type Wombat() = class end"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int";"string"]])

    [<Test>]
    member public this.``Multi.Function.WithRecordType``() = 
        let fileContents = """
            type Vector =
                { X : float; Y : float; Z : float }
            let foo(x : int,v : Vector) = ()
            foo(12, { X = 10.0; Y = (*Mark*)20.0; Z = 30.0 })"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int";"Vector"]])

    [<Test>]
    member public this.``Multi.Function.AsParameter``() = 
        let fileContents = """
            let isLessThanZero x = (x < 0)
            let containsNegativeNumbers intList =
                let filteredList = List.filter isLessThanZero intList
                if List.length filteredList > 0
                then Some(filteredList)
                else None
            let _ = Option.get(containsNegativeNumbers [6; 20; (*Mark*)8; 45; 5])"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int list"]])

    [<Test>]
    member public this.``Multi.Function.WithOptionType``() = 
        let fileContents = """
            let foo( a : int option, b : string ref) = 0
            let _ = foo(Some(12),(*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int option";"string ref"]])

    [<Test>]
    member public this.``Multi.Function.WithOptionType2``() = 
        let fileContents = """
            let multi (x : float) (y : float) = x * y
            let sum(a : int, b) = a + b
            let options(a1 : int option, b1 : float option) = a1.ToString() + b1.ToString()
            let rtnOption = options(Some(sum(1, 3)), (*Mark*)Some(multi 3.1 5.0)) """
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int option";"float option"]])

    [<Test>]
    member public this.``Multi.Function.WithRefType``() = 
        let fileContents = """
            let foo( a : int ref, b : string ref) = 0
            let _ = foo(ref 12,(*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int ref";"string ref"]])

    (* Overload list/Adjust method's param for multi-parameterinfo tests ------------------------------ *)

    [<Test>]
    member public this.``Multi.OverloadMethod.OrderedParamters``() = 
        let fileContents = "new System.DateTime(2000,12,(*Mark*)"
        this.VerifyParameterInfoOverloadMethodIndex(fileContents,"(*Mark*)",3(*The fourth method*),["int";"int";"int"])

    [<Test>]
    member public this.``Multi.Overload.WithSameParameterCount``() = 
        let fileContents = """
            type Foo() = 
              member this.A1(x1 : int, x2 : int, ?y : string, ?Z: bool) = ()
              member this.A1(x1 : int, X2 : string, ?y : int, ?Z: bool) = ()
            let foo = new Foo()
            foo.A1(1,1,(*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int";"int";"string";"bool"];["int";"string";"int";"bool"]])
        
    [<Test>]
    member public this.``ExtensionMethod.Overloads``() = 
        let fileContents = """
            module MyCode =
                type A() = 
                    member this.Method(a:string) = ""
            module MyExtension = 
                type MyCode.A with
                    member this.Method(a:int) = ""
            
            open MyCode
            open MyExtension
            let foo = A()
            foo.Method((*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["string"];["int"]])
 
    [<Test>]
    [<Ignore("Parameterinfo not retrieved properly for indexed properties by test infra")>]
    member public this.``ExtensionProperty.Overloads``() = 
        let fileContents = """
            module MyCode =
                type A() = 
                    member this.Prop with get(a:string) = ""
            module MyExtension = 
                type MyCode.A with
                    member this.Prop with get(a:int) = ""
            
            open MyCode
            open MyExtension
            let foo = A()
            foo.Prop((*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["string"];["int"]])
        
    (* Generic functions for multi-parameterinfo tests ------------------------------------------------ *)

    [<Test>]
    member public this.``Multi.Generic.ExchangeInt``() = 
        let fileContents = "System.Threading.Interlocked.Exchange<int>(123,(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["byref<int>";"int"])

    [<Test>]
    member public this.``Multi.Generic.Exchange.``() = 
        let fileContents = "System.Threading.Interlocked.Exchange(12.0,(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["byref<float>";"float"])

    [<Test>]
    member public this.``Multi.Generic.ExchangeUnder``() = 
        let fileContents = "System.Threading.Interlocked.Exchange<_> (obj,(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["byref<obj>";"obj"])

    [<Test>]
    member public this.``Multi.Generic.Dictionary``() = 
        let fileContents = "System.Collections.Generic.Dictionary<_, option<int>>(12,(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["int";"System.Collections.Generic.IEqualityComparer<obj>"])

    [<Test>]
    [<Ignore("95862 - [Unittests] parseInfo(TypeCheckResult.TypeCheckInfo).GetMethods can not get MethodOverloads")>]
    member public this.``Multi.Generic.HashSet``() = 
        let fileContents = "System.Collections.Generic.HashSet<int>({ 1 ..12 },(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["Seq<'a>";"System.Collections.Generic.IEqualityComparer<'a>"])
    
    [<Test>]
    [<Ignore("95862 - [Unittests] parseInfo(TypeCheckResult.TypeCheckInfo).GetMethods can not get MethodOverloads")>]
    member public this.``Multi.Generic.SortedList``() = 
        let fileContents = "System.Collections.Generic.SortedList<_,option<int>> (12,(*Mark*)"
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["int";"System.Collections.Generic.IComparer<'TKey>"])
    
    (* No Param Info Shown for multi-parameterinfo tests ---------------------------------------------- *)

    [<Test>]
    member public this.``ParameterInfo.Multi.NoParamterInfo.InComments``() = 
        let fileContents = "//let _ = System.Object((*Mark*))"
        this.VerifyNoParameterInfoAtStartOfMarker(fileContents,"(*Mark*)")
    
    [<Test>]
    member public this.``Multi.NoParameterInfo.InComments2``() = 
        let fileContents = """(*System.Console.WriteLine((*Mark*)"Test on Fsharp style comments.")*)"""
        this.VerifyNoParameterInfoAtStartOfMarker(fileContents,"(*Mark*)")

    [<Test>]
    member public this.``Multi.NoParamterInfo.OnFunctionDeclaration``() = 
        let fileContents = "let Foo(x : int, (*Mark*)b : string) = ()"
        this.VerifyNoParameterInfoAtStartOfMarker(fileContents,"(*Mark*)")

    [<Test>]
    member public this.``Multi.NoParamterInfo.WithinString``() = 
        let fileContents = """let s = "new System.DateTime(2000,12(*Mark*)" """
        this.VerifyNoParameterInfoAtStartOfMarker(fileContents,"(*Mark*)")

    [<Test>]
    member public this.``Multi.NoParamterInfo.OnProperty``() = 
        let fileContents = """
            let s = "Hello"
            let _ = s.Length(*Mark*)"""
        this.VerifyNoParameterInfoAtStartOfMarker(fileContents,"(*Mark*)")

    [<Test>]
    member public this.``Multi.NoParamterInfo.OnValues``() = 
        let fileContents = """
            type Foo = class
                val private size : int
                val private path : string
                new (s : int, p : string) = {size = s; path(*Mark*) = p}
            end"""
        this.VerifyNoParameterInfoAtStartOfMarker(fileContents,"(*Mark*)")

    (* Project ref method for multi-parameterinfo tests ----------------------------------------------- *)

    [<Test; Category("Expensive")>]
    member public this.``Multi.ReferenceToProjectLibrary``() = 
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project1 = CreateProject(solution, "FSharpLib")
        let project2 = CreateProject(solution, "FSharpPro")
        AddProjectReference(project2,project1)
        let _ = AddFileFromText(project1, "file1.fs", ["namespace Test";"type public Foo() = class";"  static member Sum(x:int,y:int) = x+y";"end"])
        let result1 = Build(project1)
        AddFileFromText(project2, "file2.fs", ["open Test";"Foo.Sum(12,(*Mark*)"]) |> ignore
        let result2 = Build(project2)
        let file = OpenFile(project2, "file2.fs")
        MoveCursorToStartOfMarker(file, "(*Mark*)")

        let methodstr = GetParameterInfoAtCursor(file)
        AssertMethodGroupContain(methodstr,["int";"int"])

    (* Regression tests/negative tests for multi-parameterinfos --------------------------------------- *) 
    // To be added when the bugs are fixed...
    [<Test>]
    //[<Ignore("90832 - [ParameterInfo] No Parameter Info shown on string parameter with operator")>]
    member public this.``Regrssion.ParameterWithOperators.Bug90832``() = 
        let fileContents = """System.Console.WriteLine("This(*Mark*) is a" + " bug.")"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["string"])

    [<Test>]
    member public this.``Regression.OptionalArguuments.Bug4042``() = 
        let fileContents = """
            module ParameterInfo
            type TT(x : int, ?y : int) = 
                let z = y
                do printfn "%A" z
                member this.Foo(?z : int) = z
    
            type TT2(x : int, y : int option) = 
                let z  = y
                do printfn "%A" z
            let tt = TT((*Mark*)"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["int";"int"]])

    [<Test>]
    //[<Ignore("90798 - [ParameterInfo] No param info when typing ( for the first time")>]
    member public this.``Regression.ParameterFirstTypeOpenParen.Bug90798``() = 
        let fileContents = """
            let a = async {
                    Async.AsBeginEnd((*Mark*)
                }
            let p = 10"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["'Arg -> Async<'T>"]])

    [<Test>]   
    // regression test for bug 3878: no parameter info triggered by "("
    member public this.``Regression.NoParameterInfoTriggeredByOpenBrace.Bug3878``() = 
        let fileContents = """
            module ParameterInfo
            let x = 1 + 2

            let _ = System.Console.WriteLine ((*Mark*))

            let y = 1"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",[""])

    [<Test>]   
    // regression test for bug 4495 : Should alway sort method lists in order of argument count
    member public this.``Regression.MehtodSortedByArgumentCount.Bug4495.Case1``() = 
        let fileContents = """
            module ParameterInfo
            
            let a1 = System.Reflection.Assembly.Load("mscorlib")
            let m = a1.GetType("System.Decimal").GetConstructor((*Mark*)null)"""
        this.VerifyParameterInfoOverloadMethodIndex(fileContents,"(*Mark*)",0,["System.Type []"])

    [<Test>]   
    member public this.``Regression.MehtodSortedByArgumentCount.Bug4495.Case2``() = 
        let fileContents = """
            module ParameterInfo
            
            let a1 = System.Reflection.Assembly.Load("mscorlib")
            let m = a1.GetType("System.Decimal").GetConstructor((*Mark*)null)"""
        this.VerifyParameterInfoOverloadMethodIndex(fileContents,"(*Mark*)",1,["System.Reflection.BindingFlags";
                                                                                "System.Reflection.Binder";
                                                                                "System.Type []";
                                                                                "System.Reflection.ParameterModifier []"])

    [<Test>]   
    [<Ignore("Bug 95862")>]
    member public this.``BasicBehavior.WithReference``() = 
        let fileContents = """
            open System.ServiceModel
            let serviceHost = new ServiceHost((*Mark*))"""
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, references = ["System.ServiceModel"])
 
        MoveCursorToStartOfMarker(file, "(*Mark*)") 
        TakeCoffeeBreak(this.VS)      
        let methodstr = GetParameterInfoAtCursor(file)
        printfn "%A" methodstr
        let expected = ["System.Type";"System.Uri []"]
        AssertMethodGroupContain(methodstr,expected)

    [<Test>]   
    member public this.``BasicBehavior.CommonFunction``() = 
        let fileContents = """
            let f(x) = 1
            f((*Mark*))"""
        this.VerifyParameterInfoAtStartOfMarker(fileContents,"(*Mark*)",[["'a"]])

    [<Test>]   
    member public this.``BasicBehavior.DotNet.Static``() = 
        let fileContents = """System.String.Format((*Mark*)"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Mark*)",["string";"obj []"])

(*------------------------------------------IDE Query automation start -------------------------------------------------*)
    [<Test>]   
    [<Category("Query")>]
    // ParamInfo works normally for calls as query operator arguments
    // wroks fine In nested queries
    member public this.``Query.InNestedQuery``() = 
        let fileContents = """
        let tuples = [ (1, 8, 9); (56, 45, 3)] 
        let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
        let tp = (2,3,6)
        let foo = 
            query {
                for n in numbers do
                yield (n, query {for x in tuples do 
                                 let r = x.Equals((*Marker1*)tp)
                                 let _ = System.String.Format("",(*Marker2*)x)
                                 select r })
                }"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker1*)",["obj"],queryAssemblyRefs)
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker2*)",["string";"obj []"],queryAssemblyRefs)

    [<Test>]   
    [<Category("Query")>]
    // ParamInfo works normally for calls as query operator arguments
    // ParamInfo Still works when an error exists
    member public this.``Query.WithErrors``() = 
        let fileContents = """
        let tuples = [ (1, 8, 9); (56, 45, 3)] 
        let tp = (2,3,6)
        let foo = 
            query {
                for t in tuples do
                orderBy (t.Equals((*Marker*)tp))
                }"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker*)",["obj"],queryAssemblyRefs)

    [<Test>]   
    [<Category("Query")>]
    // ParamInfo works normally for calls as query operator arguments
    member public this.``Query.OperatorWithParentheses``() = 
        let fileContents = """
        type Product() =
            let mutable id = 0
            let mutable name = ""

            member x.ProductID with get() = id and set(v) = id <- v
            member x.ProductName with get() = name and set(v) = name <- v

        let getProductList() =
            [
            Product(ProductID = 1, ProductName = "Chai");
            Product(ProductID = 2, ProductName = "Chang"); ]
        let products = getProductList()
        let categories = ["Beverages"; "Condiments"; "Vegetables";]
        // Group Join
        let q2 =
            query {
                for c in categories do
                groupJoin((*Marker1*)for p in products(*Marker2*) -> c = p.ProductName) into ps
                select (c, ps)
            } |> Seq.toArray"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker1*)",[],queryAssemblyRefs)
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker2*)",[],queryAssemblyRefs)

    [<Test>]   
    [<Category("Query")>]
    // ParamInfo works normally for calls as query operator arguments
    // ParamInfo Still works when there is an optional argument
    member public this.``Query.OptionalArgumentsInQuery``() = 
        let fileContents = """
        type TT(x : int, ?y : int) = 
            let z = y
            do printfn "%A" z
            member this.Foo(?z : int) = z
    
        type TT2(x : int, y : int option) = 
            let z  = y
            do printfn "%A" z
        let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]

        let test3 =
            query {
                for n in numbers do
                let tt = TT((*Marker*)
                minBy n
            }"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker*)",["int";"int"],queryAssemblyRefs)

    [<Test>]   
    [<Category("Query")>]
    // ParamInfo works normally for calls as query operator arguments
    // ParamInfo Still works when there are overload methods with the same param count
    member public this.``Query.OverloadMethod.InQuery``() = 
        let fileContents = """
        let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]

        type Foo() = 
            member this.A1(x1 : int, x2 : int, ?y : string, ?Z: bool) = ()
            member this.A1(x1 : int, X2 : string, ?y : int, ?Z: bool) = ()

        let test3 =
            query {
                for n in numbers do
                let foo = new Foo()
                foo.A1(1,1,(*Marker*)
                minBy n
            }"""
        this.VerifyParameterInfoContainedAtStartOfMarker(fileContents,"(*Marker*)",["int";"int";"string";"bool"],queryAssemblyRefs)


// Context project system
[<TestFixture>] 
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
