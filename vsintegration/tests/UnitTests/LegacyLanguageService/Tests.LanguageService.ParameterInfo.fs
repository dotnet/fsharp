// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.ParameterInfo

open System
open Xunit
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

type UsingMSBuild()  = 
    inherit LanguageServiceBaseTests()

    let GetParamDisplays(methods:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip_DEPRECATED) =
            [ for i = 0 to methods.GetCount() - 1 do
                yield [ for j = 0 to methods.GetParameterCount(i) - 1 do
                            let (name,display,description) = methods.GetParameterInfo(i,j) 
                            yield display ] ]
      
    let AssertEmptyMethodGroup(resultMethodGroup:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip_DEPRECATED option) =
        Assert.True(resultMethodGroup.IsNone, "Expected an empty method group")              
        
    let AssertMethodGroupDescriptionsDoNotContain(methods:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip_DEPRECATED, expectNotToBeThere) = 
        for i = 0 to methods.GetCount() - 1 do
            let description = methods.GetDescription(i)
            if (description.Contains(expectNotToBeThere)) then
                Console.WriteLine("Expected description {0} to not contain {1}", description, expectNotToBeThere)
                AssertNotContains(description,expectNotToBeThere)
 
    let AssertMethodGroup(resultMethodGroup:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip_DEPRECATED option, expectedParamNamesSet:string list list) =
        Assert.True(resultMethodGroup.IsSome, "Expected a method group")
        let resultMethodGroup = resultMethodGroup.Value
        Assert.Equal(expectedParamNamesSet.Length, resultMethodGroup.GetCount())           
        Assert.True(resultMethodGroup 
                         |> GetParamDisplays
                         |> Seq.forall (fun paramDisplays -> 
                                expectedParamNamesSet |> List.exists (fun expectedParamNames -> 
                                       expectedParamNames.Length = paramDisplays.Length && 
                                       (expectedParamNames,paramDisplays) ||> List.forall2 (fun expectedParamName paramDisplay -> 
                                           paramDisplay.Contains(expectedParamName)))))
    
    let AssertMethodGroupContain(resultMethodGroup:Microsoft.VisualStudio.FSharp.LanguageService.MethodListForAMethodTip_DEPRECATED option, expectedParamNames:string list) = 
        Assert.True(resultMethodGroup.IsSome, "Expected a method group")
        let resultMethodGroup = resultMethodGroup.Value
        Assert.True(resultMethodGroup
                          |> GetParamDisplays
                          |> Seq.exists (fun paramDisplays ->
                                expectedParamNames.Length = paramDisplays.Length &&
                                (expectedParamNames,paramDisplays) ||> List.forall2 (fun expectedParamName paramDisplay -> 
                                           paramDisplay.Contains(expectedParamName))))

    member private this.GetMethodListForAMethodTip(fileContents : string, marker : string, ?addtlRefAssy : string list) = 
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker(file, marker)

        GetParameterInfoAtCursor(file)

     //Verify all the overload method parameterInfo 
    member private this.VerifyParameterInfoAtStartOfMarker(fileContents : string, marker : string, expectedParamNamesSet:string list list, ?addtlRefAssy :string list) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        AssertMethodGroup(methodstr,expectedParamNamesSet)

   //Verify No parameterInfo at the marker     
    member private this.VerifyNoParameterInfoAtStartOfMarker(fileContents : string, marker : string, ?addtlRefAssy : string list) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        AssertEmptyMethodGroup(methodstr)

    //Verify one method parameterInfo if contained in parameterInfo list
    member private this.VerifyParameterInfoContainedAtStartOfMarker(fileContents : string, marker : string, expectedParamNames:string list, ?addtlRefAssy : string list) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        AssertMethodGroupContain(methodstr,expectedParamNames)

    //Verify the parameterInfo of one of the list order
    member private this.VerifyParameterInfoOverloadMethodIndex(fileContents : string, marker : string, index : int, expectedParams:string list, ?addtlRefAssy : string list) = 
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        Assert.True(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        let paramDisplays = 
            [ for i = 0 to methodstr.GetParameterCount(index) - 1 do
                let (name,display,description) = methodstr.GetParameterInfo(index,i)
                yield display]
        Assert.True((expectedParams, paramDisplays) ||> List.forall2 (fun expectedParam paramDisplay -> paramDisplay.Contains(expectedParam)))

    //Verify there is at least one parameterInfo
    member private this.VerifyHasParameterInfo(fileContents : string, marker : string) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker)
        Assert.True(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        Assert.True (methodstr.GetCount() > 0)

    //Verify return content after the colon
    member private this.VerifyFirstParameterInfoColonContent(fileContents : string, marker : string, expectedStr : string, ?addtlRefAssy : string list) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker,?addtlRefAssy=addtlRefAssy)
        Assert.True(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value

        Assert.Equal(expectedStr, methodstr.GetReturnTypeText(0)) // Expecting a method info like X(a:int,b:int) : int [used to be  X(a:int,b:int) -> int]

    member private this.VerifyParameterCount(fileContents : string, marker : string, expectedCount: int) =
        let methodstr = this.GetMethodListForAMethodTip(fileContents,marker)
        Assert.True(methodstr.IsSome, "Expected a method group")
        let methodstr = methodstr.Value
        Assert.Equal(0, methodstr.GetParameterCount(expectedCount))

    // Test on .NET functions with no parameter
    member private this.TestGenericParameterInfo (testLine, methReq) =
        let code = [ "open System"; "open System.Threading"; ""; testLine ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        MoveCursorToEndOfMarker(file, testLine)
        let methodGroup = GetParameterInfoAtCursor file
        if (methReq = []) then
            Assert.True(methodGroup.IsNone, "expected no method group")
        else
            AssertMethodGroup(methodGroup, methReq)
    
    member private this.ExtractLineInfo (line:string) =
        let idx, lines, foundDollar = line.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray |> List.foldBack (fun l (idx, lines, foundDollar) ->
            let i = l.IndexOf("$")
            if i = -1 then (idx, l::lines, foundDollar) 
            else (l.Substring(0, i), l.Replace("$", "")::lines, true) ) <| ("", [], false)
        if not foundDollar then
            failwith "bad unit test: did not find '$' in input to mark cursor location!"
        idx, lines
        
    member public this.TestParameterInfoNegative (testLine, ?addtlRefAssy : string list) =
        let cursorPrefix, testLines = this.ExtractLineInfo testLine

        let code = 
                      [
                        "open System"
                        "open System.Threading"
                        "open System.Collections.Generic"; ""] @ testLines
        let (_, _, file) = this.CreateSingleFileProject(code, ?references = addtlRefAssy)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.True(info.IsNone, "expected no parameter info")
        gpatcc.AssertExactly(0,0)
        
    member public this.TestParameterInfoLocation (testLine, expectedPos, ?addtlRefAssy : string list) =
        let cursorPrefix, testLines = this.ExtractLineInfo testLine
        let code =
                      [
                        "open System"
                        "open System.Threading"
                        "open System.Collections.Generic"; ""] @ testLines
        let (_, _, file) = this.CreateSingleFileProject(code, ?references = addtlRefAssy)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.True(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual(expectedPos, info.GetColumnOfStartOfLongId())

    // Tests the current behavior, we may want to specify it differently in the future
    // There are more comments below that explain particular tricky cases
    

    [<Fact>]
    member public this.``Single.Locations.Simple``() =
        this.TestParameterInfoLocation("let a = System.Math.Sin($", 8)
        
    //This test verifies that ParamInfo location on a provided type with namespace that exposes static parameter that takes >1 argument works normally.
    //This test verifies that ParamInfo location on a provided type without the namespace that exposes static parameter that takes >1 argument works normally.
    //This test verifies that no ParamInfo in a string for a provided type  that exposes static parameter that takes >1 argument works normally.
     //The intent here to make sure the ParamInfo is not shown when inside a string
    //This test verifies that no ParamInfo in a Comment for a provided type that exposes static parameter that takes >1 argument works normally.
    //The intent here to make sure the ParamInfo is not shown when inside a comment
    [<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
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
        Assert.True(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual("f1", info.GetName(0))
        // note about (5,0): service.fs adds three lines of empty text to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        AssertEqual([|(2,10);(2,12);(2,13);(3,0)|], info.GetParameterLocations())

    [<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
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
        Assert.True(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual("Foo", info.GetName(0))
        // note about (4,0): service.fs adds three lines of empty text to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        AssertEqual([|(1,14);(1,17);(1,18);(2,0)|], info.GetParameterLocations())


(*
This does not currently work, because the 'fallback to name environment' does weird QuickParse-ing and mangled the long id "Bar.Foo".
We really need to rewrite some code paths here to use the real parse tree rather than QuickParse-ing.
    [<Fact>]
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
        AssertEqual([|(1,14);(1,21);(1,21);(4,0)|], info.GetParameterLocations())
*)

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
        // note: service.fs adds a new line character to the end of every file, so it reports the location of 'end of file' as first the char, 3 lines past the last line of the file
        let expectedLocs = if defaultArg markAtEOF false then 
                                Array.append expectedLocs [| (testLines.Length-1)+1, 0 |] 
                           else 
                                expectedLocs
        let cursorPrefix = cursorPrefix.Replace("^","")

        let references = "System.Core"::(defaultArg additionalReferenceAssemblies [])
        let (_, _, file) = this.CreateSingleFileProject(testLines, references = references)
        MoveCursorToEndOfMarker(file, cursorPrefix)
        let info = GetParameterInfoAtCursor file
        Assert.True(info.IsSome, "expected parameter info")
        let info = info.Value
        AssertEqual(expectedLocs, info.GetParameterLocations()) 

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
        Assert.True(info.IsNone, "expected no parameter info for this particular test, though it would be nice if this has started to work")

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
