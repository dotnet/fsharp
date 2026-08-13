// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.AutoCompletion

open System
open Microsoft.VisualStudio.FSharp.LanguageService
open Salsa.Salsa
open Salsa.VsMocks
open Salsa.VsOpsUtils
open Xunit
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

[<AutoOpen>]
module StandardSettings = 
    let standard40AssemblyRefs  = [ "System"; "System.Core"; "System.Numerics" ]
    let queryAssemblyRefs = [ "System.Xml.Linq"; "System.Core" ]
    type Expectation = 
        | QuickInfoExpected of string * string
        | AutoCompleteExpected of string * string
        | DotCompleteExpected of string * string
    let QI x y = QuickInfoExpected(x,y)
    let AC x y = AutoCompleteExpected(x,y)
    let DC x y = DotCompleteExpected(x,y)

type UsingMSBuild() as this  = 
    inherit LanguageServiceBaseTests()

    let createFile (code : string list) fileKind refs otherFlags = 
        let (_, _, file) = 
            match code with
            | [code] when code.IndexOfAny([|'\r'; '\n'|]) <> -1 ->
                this.CreateSingleFileProject(code, fileKind = fileKind, references = refs, ?otherFlags=otherFlags)
            | code ->
                this.CreateSingleFileProject(code, fileKind = fileKind, references = refs, ?otherFlags=otherFlags)
        file

    let DoWithAutoCompleteUsingExtraRefs refs otherFlags coffeeBreak fileKind reason (code : string list) marker f  =        
        // Up to 2 untyped parse operations are OK: we do an initial parse to provide breakpoint validation etc. 
        // This might be before the before the background builder is ready to process the foreground typecheck.
        // In this case the background builder calls us back when its ready, and we then request a foreground typecheck 
        let file = createFile code fileKind refs otherFlags
            
        if coffeeBreak then
            TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, marker)
        let completions = CompleteAtCursorForReason(file,reason)
        f completions
        gpatcc.AssertExactly(0,0)


    let DoWithAutoComplete coffeeBreak fileKind reason otherFlags (code : string list) marker f  =
        DoWithAutoCompleteUsingExtraRefs [] otherFlags coffeeBreak fileKind reason code marker f

    let AssertAutoCompleteContainsAux coffeeBreak fileName reason otherFlags code marker  should shouldnot  =        
        DoWithAutoComplete coffeeBreak fileName reason otherFlags code marker (fun completions ->
            AssertCompListContainsAll(completions, should)
            AssertCompListDoesNotContainAny(completions, shouldnot))

    let AssertAutoCompleteContains =
        AssertAutoCompleteContainsAux true SourceFileKind.FS BackgroundRequestReason.MemberSelect None

    let AssertAutoCompleteContainsNoCoffeeBreak = 
        AssertAutoCompleteContainsAux false SourceFileKind.FS BackgroundRequestReason.MemberSelect None

    let AutoCompleteInInterfaceFileContains = 
        AssertAutoCompleteContainsAux true SourceFileKind.FSI BackgroundRequestReason.MemberSelect None

    let AssertCtrlSpaceCompleteContains = 
        AssertAutoCompleteContainsAux true SourceFileKind.FS BackgroundRequestReason.CompleteWord None

    let AssertCtrlSpaceCompleteContainsWithOtherFlags otherFlags = 
        AssertAutoCompleteContainsAux true SourceFileKind.FS BackgroundRequestReason.CompleteWord (Some otherFlags)

    let AssertCtrlSpaceCompleteContainsNoCoffeeBreak = 
        AssertAutoCompleteContainsAux false SourceFileKind.FS BackgroundRequestReason.CompleteWord None
    
    let AssertCtrlSpaceCompletionListIsEmpty code marker = 
        DoWithAutoComplete true SourceFileKind.FS BackgroundRequestReason.CompleteWord None code marker AssertCompListIsEmpty

    let AssertCtrlSpaceCompletionListIsEmptyNoCoffeeBreak code marker = 
        DoWithAutoComplete false SourceFileKind.FS BackgroundRequestReason.CompleteWord None code marker AssertCompListIsEmpty

    let AssertAutoCompleteCompletionListIsEmpty code marker = 
        DoWithAutoComplete true SourceFileKind.FS BackgroundRequestReason.MemberSelect None code marker AssertCompListIsEmpty

    let AssertAutoCompleteCompletionListIsEmptyNoCoffeeBreak code marker = 
        DoWithAutoComplete false SourceFileKind.FS BackgroundRequestReason.MemberSelect None code marker AssertCompListIsEmpty

    let testAutoCompleteAdjacentToDot op =
        let text = sprintf "System.Console%s" op

        // First, test that pressing dot works.
        AssertAutoCompleteContainsNoCoffeeBreak 
          [ text ]
          "System.Console."
          [ "BackgroundColor" ] // should contain
          [ ] // should not contain
 
    let testAutoCompleteAdjacentToDotNegative op =
        let text = sprintf "System.Console%s" op

        // Next test that there is no completion after then end.
        AssertCtrlSpaceCompleteContainsNoCoffeeBreak 
          [ text ]
          text
          [ "abs" ] // should contain (top-level autocomplete on empty identifier)
          [ "BackgroundColor" ] // should not contain (from prior System.Console)

    let customOperations = 
      [ "contains"; "count";"last"; "lastOrDefault"; "exactlyOne"; "exactlyOneOrDefault"; "headOrDefault"; "select"; "where"
        "minBy"; "maxBy"; "groupBy"; "sortBy"; "sortByDescending"; "thenBy"; "thenByDescending"; "groupValBy"; "join"
        "groupJoin"; "sumByNullable"; "minByNullable"; "maxByNullable"; "averageByNullable"; "averageBy"
        "distinct"; "exists"; "find"; "all"; "head"; "nth"; "skip"; "skipWhile"; "sumBy"; "take"
        "takeWhile"; "sortByNullable"; "sortByNullableDescending"; "thenByNullable"; "thenByNullableDescending"]

    let AA l = Some(System.IO.Path.Combine(System.IO.Path.GetTempPath(), ".NETFramework,Version=v4.0.AssemblyAttributes.fs")), l
    let notAA l = None,l
    let stopWatch = new System.Diagnostics.Stopwatch()
    let ResetStopWatch() = stopWatch.Reset(); stopWatch.Start()
    let time1 op a message = 
        ResetStopWatch()
        let result = op a
        //printf "%s %d ms\n" message stopWatch.ElapsedMilliseconds
        result

    let ShowErrors(project:OpenProject) =     
        for error in (GetErrors(project)) do
            printf "%s\n" (error.ToString())    


    // There are some dot completion tests in this type as well, in the systematic tests for queries
    member private this.VerifyDotCompListContainAllAtStartOfMarker(fileContents : string, marker : string, list :string list, ?addtlRefAssy:string list, ?coffeeBreak:bool) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        //to add references
        if defaultArg coffeeBreak false then TakeCoffeeBreak(this.VS)
        let completions = DotCompletionAtStartOfMarker file marker
        AssertCompListContainsAll(completions, list)

    // There are some quickinfo tests in this file as well, in the systematic tests for queries
    member public this.InfoInDeclarationTestQuickInfoImpl(code : string,marker,expected,atStart, ?addtlRefAssy : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(code, ?references = addtlRefAssy)

        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        if atStart then
            MoveCursorToStartOfMarker(file, marker)
        else
            MoveCursorToEndOfMarker(file, marker)

        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertContains(tooltip, expected) 
        gpatcc.AssertExactly(0,0)

    member public this.AssertQuickInfoContainsAtEndOfMarker(code,marker,expected, ?addtlRefAssy : string list) =
        this.InfoInDeclarationTestQuickInfoImpl(code,marker,expected,false,?addtlRefAssy=addtlRefAssy)
    static member charExpectedCompletions = [
        "CompareTo"; // Members defined on System.Char
        "GetHashCode"] // Members defined on System.Object

    static member intExpectedCompletions = [
        "CompareTo"; // Members defined on System.Int32
        "GetHashCode"] // Members defined on System.Object
    
    static member stringExpectedCompletions = [
        "Substring"; // Methods of System.String
        "GetHashCode"] // Methods of System.Object

    static member arrayExpectedCompletions = [
        "Length"; ] // Methods of System.Object

    member private this.AutoCompleteBug70080HelperHelper(programText:string, shouldContain, shouldNotContain) =
        let i = programText.IndexOf("[<Attr ")
        if i = -1 then failwith "Could not find expected '[<Attr ' in program"
        AssertCtrlSpaceCompleteContains
          [ programText ]
          "[<Attr"       // marker
          shouldContain
          shouldNotContain
        let s = programText.Insert(i+2, "type:")
        AssertCtrlSpaceCompleteContains 
          [ s ]
          "[<type:Attr"       // marker
          shouldContain // should contain
          shouldNotContain
        let s = programText.Insert(i+2, "module:")
        AssertCtrlSpaceCompleteContains 
          [ s ]
          "[<module:Attr"       // marker
          shouldContain // should contain
          shouldNotContain

    member public this.AutoCompleteBug70080Helper(programText: string) =
        this.AutoCompleteBug70080HelperHelper(programText, ["AttributeUsage"], [])

    member private this.testAutoCompleteAdjacentToDot op =
        let text = sprintf "System.Console%s" op
        // First, test that pressing dot works.
        AssertAutoCompleteContainsNoCoffeeBreak 
          [ text ]
          "System.Console."
          [ "BackgroundColor" ] // should contain
          [ ] // should not contain   

    //**Help Function for checking Ctrl-Space Completion Contains the expected value *************
    member private this.AssertCtrlSpaceCompletionContains(fileContents : string list, marker, expected, ?addtlRefAssy: string list)  = 
        this.AssertCtrlSpaceCompletion(
            fileContents,
            marker,
            (fun completions -> 
                Assert.NotEqual(0,completions.Length)
                let found = completions |> Array.exists(fun (CompletionItem(s,_,_,_,_)) -> s = expected)
                if not(found) then 
                    failwithf "Expected: %A to contain %s" completions expected  
            ),
            ?addtlRefAssy = addtlRefAssy
        )

   //**Help Function for checking Ctrl-Space Completion Contains the expected value *************
    member private this.AssertCtrlSpaceCompletion(fileContents : string list, marker, checkCompletion: (CompletionItem array -> unit), ?addtlRefAssy: string list)  = 
        let (_, _, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)
        MoveCursorToEndOfMarker(file,marker)
        let completions = CtrlSpaceCompleteAtCursor file
        checkCompletion completions

    member private this.AutoCompletionListNotEmpty (fileContents : string list) marker  = 
        let (_, _, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToEndOfMarker(file,marker)
        let completions = AutoCompleteAtCursor file
        Assert.NotEqual(0,completions.Length)

    member public this.TestCompletionNotShowingWhenFastUpdate (firstSrc : string list) secondSrc marker =     
        let (_, _, file) = this.CreateSingleFileProject(firstSrc)
        MoveCursorToEndOfMarker(file,marker)

        // Now delete the property and leave only dot at the end 
        //  - user is typing fast so replace the content without background compilation
        ReplaceFileInMemoryWithoutCoffeeBreak file secondSrc      
        let completions = time1 AutoCompleteAtCursor file "Time of first autocomplete."
        AssertCompListIsEmpty(completions)

        // Recheck after some time - after the background compilation runs
        TakeCoffeeBreak(this.VS)                                      
        let completions = time1 AutoCompleteAtCursor file "Time of first autocomplete."
        AssertCompListIsEmpty(completions)      

   /////Helper Functions 
        //DotCompList ContainAll At End Of Marker Helper Function
    member private this.VerifyDotCompListContainAllAtEndOfMarker(fileContents : string, marker : string, list : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        let completions = DotCompletionAtEndOfMarker file marker
        AssertCompListContainsAll(completions, list)

        //DoesNotContainAny At Start Of Marker Helper Function 
    member private this.VerifyDotCompListDoesNotContainAnyAtStartOfMarker(fileContents : string, marker : string, list : string list, ?addtlRefAssy : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        let completions = DotCompletionAtStartOfMarker file marker
        AssertCompListDoesNotContainAny(completions, list)

        //DotCompList Is Empty At Start Of Marker Helper Function
    member private this.VerifyDotCompListIsEmptyAtStartOfMarker(fileContents : string, marker : string, ?addtlRefAssy : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        let completions = DotCompletionAtStartOfMarker file marker
        AssertCompListIsEmpty(completions)


                    
                
               



    

    


    


        


    


    
    
    
    

    

    


    


            









    








    [<Fact>]
    member public this.``CtrlSpaceCompletion.Bug294974.Case1``() =
        
        AssertCtrlSpaceCompleteContains
          [ """
              let xxx = [1]
         (*M*)xxx.IsEmpty // Ctrl-J just before the '.'""" ]
          "(*M*)xxx"
          [ "xxx" ] // should contain (completions before dot)
          [ "IsEmpty" ] // should not contain (completions after dot)















         
    member this.QueryExpressionFileExamples() = 
           [ """
                module BasicTest
                let x = query { for x in [1;2;3] do (*TYPING*)"""
             """
                module BasicTest
                let x = query { for x in [1;2;3] do (*TYPING*) }""" 
             """
                module BasicTest
                let x = query { for x in [1;2;3] do 
                                (*TYPING*)""" 
             """
                module BasicTest
                let x = query { for x in [1;2;3] do 
                                if x > 3 then 
                                (*TYPING*)""" 
             """
                module BasicTest
                let x = query { for x in [1;2;3] do 
                                where (x > 3)
                                (*TYPING*)""" 
             """
                module BasicTest
                let x = query { for x in [1;2;3] do 
                                sortBy x 
                                (*TYPING*)""" 
             """
                module BasicTest
                let x = query { for x in [1;2;3] do 
                                (*TYPING*)
                                sortBy x """ 
             """
                module BasicTest
                let x = query { for x in [1;2;3] do 
                                let y = x + 1
                                (*TYPING*)""" ]



    member this.WordByWordSystematicTestWithSpecificExpectations(prefix, suffixes, lines, variations, knownFailures:list<_>) = 

        let knownFailuresDict = set knownFailures
        printfn "Building systematic tests, excluding %d known failures" knownFailures.Length  
        let tests = 
            [ for (suffixName,suffixText) in suffixes  do
                for builderName in variations do
                  for (lineName, line, checks) in lines builderName do 
                    for check in checks do
                      let expectedToFail = knownFailuresDict.Contains (lineName, suffixName, builderName, check)
                      if not expectedToFail then yield (lineName, suffixName, suffixText, builderName, line, check, expectedToFail) ]

        let unexpectedSuccesses = ResizeArray<_>()
        let successes = ResizeArray<_>()
        let failures = ResizeArray<_>()
        printfn "Running %d systematic tests.... Failure will be printed if it occurs..."  tests.Length
        for (lineName, suffixName, suffixText, builderName, fileContents, check, expectedToFail) in tests do
            if successes.Count % 50 = 0 then
                printfn "Making progress, run %d so far..." successes.Count
            let fileContents = prefix + fileContents + suffixText
            try
                match check with
                | QuickInfoExpected(where,expected) ->
                      let where = where.[0..where.Length-2] // chop a character off to get in the middle of the text
                      this.AssertQuickInfoContainsAtEndOfMarker(fileContents,where,expected,addtlRefAssy=standard40AssemblyRefs )
                | AutoCompleteExpected(where,expected) ->
                      this.VerifyCtrlSpaceListContainAllAtStartOfMarker(fileContents,where,[expected],addtlRefAssy=standard40AssemblyRefs )
                | DotCompleteExpected(where,expected) ->
                      this.VerifyDotCompListContainAllAtStartOfMarker(fileContents,where,[expected], addtlRefAssy=standard40AssemblyRefs)
                if not expectedToFail then
                    successes.Add(lineName,suffixName,builderName,check)
                else
                    unexpectedSuccesses.Add(lineName,suffixName,builderName,check)
            with e ->
                printfn "Exception thrown: (\"%s\", \"%s\", \"%s\", %A) " lineName suffixName builderName check
                if not expectedToFail then
                    printfn " FAILURE on systematic test: (\"%s\", \"%s\", \"%s\", %A) " lineName suffixName builderName check
                    printfn "\n\nfileContents = <<<%s>>>" fileContents
                    failures.Add(lineName,suffixName,builderName,check)

        let nFail = failures.Count
        let nSuccess = successes.Count
        printfn "%d TESTS, %d SUCCESS, %d FAILURE, %%%2.2f success rate" (nSuccess+nFail) successes.Count nFail  (float nSuccess / float (nSuccess+nFail) * 100.0)

        if failures.Count <> 0 then 
            printfn "EXTRA OBSERVED FAILURES:  "
            printfn "["
            for (lineName,suffixName,builderName,check) in failures do
                printfn "   (\"%s\", \"%s\", \"%s\", %A) " lineName suffixName builderName check
            printfn "]"
             
        if unexpectedSuccesses.Count <> 0 then 
            printfn "EXTRA OBSERVED SUCCESSES:  "
            printfn "["
            for (lineName,suffixName,builderName,check) in unexpectedSuccesses do
                printfn "   (\"%s\", \"%s\", \"%s\", %A) " lineName suffixName builderName check
            printfn "]"

        if failures.Count <> 0 || unexpectedSuccesses.Count <> 0 then 
            raise <| new Exception("there were unexpected results, see console output for details")


             






    (* Various parser recovery test cases -------------------------------------------------- *)

//*****************Helper Function*****************
    member public this.AutoCompleteRecoveryTest(source : string list, marker, expected) =
        let (_, _, file) = this.CreateSingleFileProject(source)
        MoveCursorToEndOfMarker(file, marker)
        let completions = time1 CtrlSpaceCompleteAtCursor file "Time of first autocomplete."
        AssertCompListContainsAll(completions, expected)
            

                      


    
                                            
    // Another test case for the same thing - this goes through a different code path


   (* Tests for autocomplete -------------------------------------------------------------- *)
                     
    member public this.TestGenericAutoComplete(line, expected) =
        let code = 
         [  "type GT<'a> ="
            "  static member P = 12"
            "  static member Q = 13"
            "type GT2 ="
            "  static member R = 12"
            "  static member S = 13"
            "type D = | DD"
            "let td = typeof<D>"
            "let f i = typeof<D>"
            ""; line ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, line)
        let completions = AutoCompleteAtCursor file
        AssertCompListContainsAll(completions, expected)
        gpatcc.AssertExactly(0,0)









            









































            (**)



















    [<Fact>]
    member public this.``OpenNamespaceOrModule.CompletionOnlyContainsNamespaceOrModule.Case1``() =        
        AssertAutoCompleteContains 
            [ "open System." ]
            "." // marker
            [ "Collections" ] // should contain (namespace)
            [ ] // should not contain









          
                                             
    [<Fact>]
    member public this.``Expressions.Sequence``() =        
        AssertAutoCompleteContains 
          [  
            "(seq { yield 1 })." ]
          "})."       // marker
          [ "GetEnumerator" ] // should contain
          [ ] // should not contain
                      
                      
                                           
                                          
                                         
                      
                      

                                        


    (* Tests for various uses of ObsoleteAttribute ----------------------------------------- *)
    (* Members marked with obsolete shouldn't be visible, but we should support              *)
    (* dot completions on them                                                               *)
 
    // Obsolete and CompilerMessage(IsError=true) should not appear.
    member public this.AutoCompleteObsoleteTest testLine appendDot should shouldnot =        
        let code = 
          [ "[<System.ObsoleteAttribute(\"!\", false)>]"
            "module ObsoleteTop ="
            "  let T = \"T\""
            "module Module = "
            "  [<System.ObsoleteAttribute(\"!\", false)>]"
            "  module ObsoleteM ="
            "    let A = \"A\""
            "    [<System.ObsoleteAttribute(\"!\", false)>]"
            "    module ObsoleteNested ="
            "      let C = \"C\""
            "  [<System.ObsoleteAttribute(\"!\", false)>]"
            "  type ObsoleteT = "
            "    static member B = \"B\""
            "  let Other = 0"
            "let mutable level = \"\""
            "" ]
        let (_, _, file) = this.CreateSingleFileProject(code @ [ testLine ])
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        if not appendDot then        
            // In this case, we want to check Ctrl+J completions
            // For "level <- O" this shows completions starting with O (e.g. Other)
            MoveCursorToEndOfMarker(file, testLine)
            let completions = CtrlSpaceCompleteAtCursor file
            AssertCompListContainsAll(completions, should)
            AssertCompListDoesNotContainAny(completions, shouldnot) 
        else
            // In this case, we quickly type "." and then get dot-completions
            // For "level <- Module" this shows completions from the "Module" (e.g. "Module.Other")
            // This simulates the case when the user quickly types "dot" after the file has been TCed before.
            ReplaceFileInMemoryWithoutCoffeeBreak file (code @ [ testLine + "." ])      
            MoveCursorToEndOfMarker(file, testLine + ".")
            let completions = AutoCompleteAtCursor file
            AssertCompListContainsAll(completions, should)
            AssertCompListDoesNotContainAny(completions, shouldnot) 
        gpatcc.AssertExactly(0,0)

    // When the module isn't empty, we should show completion for the module
    // (and not type-inference based completion on strings - therefore test for 'Chars')
    






    member internal this.AutoCompleteDuplicatesTest (marker, shortName, fullName:string) =
        let code =
            [  
                "namespace A "
                "module Test = "
                "  let foo n = n + 1"
                "  let (|Pat|) x = x + 1"
                "  exception Failed"
                "  type Del = delegate of int -> int"
                "  type A = | Foo"
                "  type B = | Bar = 0"
                "type TestType ="
                "  static member Prop = 0"
                "  static member Event = (new Event<_>()).Publish"
                "namespace B"
                "open A"
                "open A"
                marker ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        MoveCursorToEndOfMarker(file, marker)
        let completions = AutoCompleteAtCursor file
        let (CompletionItem(_, _, _, descrFunc, _)) = completions |> Array.find (fun (CompletionItem(name, _, _, _, _)) -> name = shortName)
        let descr = descrFunc()
        // Check whether the description contains the name only once        
        let occurrences = ("  " + descr + "  ").Split([| fullName |], System.StringSplitOptions.None).Length - 1
        AssertEqualWithMessage(1, occurrences, "The entry for '" + fullName + "' is duplicated.")

    // Return the number of occurrences of the specified method in a tooltip string
    member this.CountMethodOccurrences(descr, methodName:string) =
        let occurrences = ("  " + descr + "  ").Split([| methodName |], System.StringSplitOptions.None).Length - 1
        // This is some tag in the tooltip that also contains the overload name text
        if descr.Contains("[Signature:") then occurrences - 1 else occurrences
                                      

        
    // FEATURE: Saving file N does not cause files 1 to N-1 to re-typecheck (but does cause files N to <end> to 
    [<Fact>]
    member public this.``Performance.Bug5774``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        
        let file1 = AddFileFromText(project,"File1.fs", [""])
        let file1 = OpenFile(project,"File1.fs")
        //file1.

        let file2 = AddFileFromText(project,"File2.fs", ["let x = 4"; "x."])
        let file2 = OpenFile(project,"File2.fs")

        let file3 = AddFileFromText(project,"File3.fs", [""])
        let file3 = OpenFile(project,"File3.fs")

        // ensure that the incremental builder is running        
        MoveCursorToEndOfMarker(file2,"x.")
        AutoCompleteAtCursor file2 |> ignore

        TakeCoffeeBreak(this.VS)

        // Start the key instrumentation
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        
        // Save file2
        ReplaceFileInMemory file2 [""]
        SaveFileToDisk file2    
        let file3 = OpenFile(project,"File3.fs")
        TakeCoffeeBreak(this.VS)

        gpatcc.AssertExactly(notAA[file2], notAA[file2;file3])

    /// FEATURE: References added to the project bring corresponding new .NET and F# items into scope.
    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.``AfterAssemblyReferenceAdded``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let file = AddFileFromText(project,"File1.fs",
                                    [ 
                                     "let y = System.Deployment.Application."
                                     "()"])
        let file = OpenFile(project, "File1.fs")
        MoveCursorToEndOfMarker(file,"System.Deployment.Application.")
        let completions = AutoCompleteAtCursor(file)
        // printf "Completions=%A\n" completions
        Assert.Equal(0, completions.Length) // Expect none here because reference hasn't been added.
        
        // Now, add a reference to the given assembly.
        this.AddAssemblyReference(project,"System.Deployment")

        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor(file)
        Assert.NotEqual(0, completions.Length) 

    /// FEATURE: Updating the active project configuration influences the language service
    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.``AfterUpdateProjectConfiguration``() = 
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        PlaceIntoProjectFileBeforeImport
            (project, @"
                <ItemGroup>
                    <Reference Include=""System.Deployment"" Condition=""'$(Configuration)'=='Foo'"" />
                </ItemGroup>")
        let file = AddFileFromText(project,"File1.fs",
                                    [ 
                                     "let y = System.Deployment.Application."
                                     "()"])
        let file = OpenFile(project, "File1.fs")
        MoveCursorToEndOfMarker(file,"System.Deployment.Application.")
        let completions = AutoCompleteAtCursor(file)
        // printf "Completions=%A\n" completions
        Assert.Equal(0, completions.Length) // Expect none here because reference hasn't been added.
        
        // Now, update active configuration
        SetConfigurationAndPlatform(project, "Foo|x86")
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor(file)
        Assert.NotEqual(0, completions.Length) 

    /// FEATURE: Updating the active project platform influences the language service
    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.``AfterUpdateProjectPlatform``() = 
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        PlaceIntoProjectFileBeforeImport
            (project, @"
            <ItemGroup>
                <Reference Include=""System.Deployment"" Condition=""'$(Platform)'=='x86'"" />
            </ItemGroup>")
        SetConfigurationAndPlatform(project, "Debug|AnyCPU")
        let file = AddFileFromText(project,"File1.fs",
                                    [ 
                                     "let y = System.Deployment.Application."
                                     "()"])
        let file = OpenFile(project, "File1.fs")
        MoveCursorToEndOfMarker(file,"System.Deployment.Application.")
        let completions = AutoCompleteAtCursor(file)
        // printf "Completions=%A\n" completions
        Assert.Equal(0, completions.Length) // Expect none here because reference hasn't been added.
        
        // Now, update active platform
        SetConfigurationAndPlatform(project, "Debug|x86")
        let completions = AutoCompleteAtCursor(file)
        Assert.NotEqual(0, completions.Length) 

(*
/// FEATURE: The fileName on disk and the fileName in the project can differ in case.
    [<Fact>]
    member this.``Filenames.MayBeDifferentlyCased``() =
        use _guard = this.UsingNewVS() 
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let file = AddFileFromTextEx(project,"file1.fs","FILE1.FS",BuildAction.Compile,
                                    [ 
                                     "let y = System.Deployment.Application."
                                     "()"])
        let file = OpenFile(project, "file1.fs")
        MoveCursorToEndOfMarker(file,"System.Deployment.Application.")
        let completions = AutoCompleteAtCursor(file)
        // printf "Completions=%A\n" completions
        Assert.Equal(0, completions.Length) // Expect none here because reference hasn't been added.
        
        // Now, add a reference to the given assembly.
        this.AddAssemblyReference(project,"System.Deployment")
        let completions = AutoCompleteAtCursor(file)
        Assert.NotEqual(0, completions.Length, "Expected some items in the list after adding a reference.") 
*)

        
        
        
        
        
      
    // If there is a compile error that prevents a data tip from resolving then show that data tip.

    // Bunch of crud in empty list. This test asserts that unwanted things don't exist at the top level.
              
              
        
        
               
                            
           
        
    // This was a bug in which the third level of dotting was ignored.

    // Test completions in an incomplete computation expression (case 1: for "let")
 
    
      
    
    [<Fact>]
    member public this.``VisualStudio.CloseAndReopenSolution``() = 
        use _guard = this.UsingNewVS()
        // This test exposes what was once a bug, where closing a solution and then re-opening
        // it caused the old stale IProjectSiteOption (that the LanguageService had cached)
        // to eventually throw a NullReferenceException and assert.
        let solution = this.CreateSolution()
        let projName = "testproject"
        let project = CreateProject(solution,projName)
        let dir = ProjectDirectory(project)
        let file = AddFileFromText(project,"File1.fs", 
                                    [ 
                                     "let x = 0"
                                     "let y = x."
                                    ]) 
        let file = OpenFile(project, "File1.fs")
        MoveCursorToEndOfMarker(file,"x.")
        let completions = time1 AutoCompleteAtCursor file "Time of first autocomplete."
        // printf "Completions=%A\n" completions
        Assert.True(completions.Length>0)
        this.CloseSolution(solution)
        let project,solution = OpenExistingProject(this.VS, dir, projName)
        let file = List.item 0 (GetOpenFiles(project))
        MoveCursorToEndOfMarker(file,"x.")
        let completions = time1 AutoCompleteAtCursor file "Time of first autocomplete."
        // printf "Completions=%A\n" completions
        Assert.True(completions.Length>0)

    [<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
    member this.``BadCompletionAfterQuicklyTyping.Bug72561``() =        
        let code = [ "        " ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        // In this case, we quickly type "." and then get dot-completions
        // This simulates the case when the user quickly types "dot" after the file has been TCed before.
        ReplaceFileInMemoryWithoutCoffeeBreak file ([ "[1]." ])      
        MoveCursorToEndOfMarker(file, ".")
        // Note: no TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContainsExactly(completions, [])  // there are no stale results for an expression at this location, so nothing is returned immediately
        // second-chance intellisense will kick in:
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContainsAll(completions, ["Length"])
        AssertCompListDoesNotContainAny(completions, ["AbstractClassAttribute"]) 
        gpatcc.AssertExactly(0,0)

    [<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
    member this.``BadCompletionAfterQuicklyTyping.Bug72561.Noteworthy.NowWorks``() =        
        let code = [ "123      " ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        // In this case, we quickly type "." and then get dot-completions
        // This simulates the case when the user quickly types "dot" after the file has been TCed before.
        ReplaceFileInMemoryWithoutCoffeeBreak file ([ "[1]." ])      
        MoveCursorToEndOfMarker(file, ".")
        // Note: no TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListIsEmpty(completions)  // empty completion list means second-chance intellisense will kick in
        // if we wait...
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        // ... we get the expected answer
        AssertCompListContainsAll(completions, ["Length"])
        AssertCompListDoesNotContainAny(completions, ["AbstractClassAttribute"]) 
        gpatcc.AssertExactly(0,0)

    [<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
    member this.``BadCompletionAfterQuicklyTyping.Bug130733.NowWorks``() =        
        let code = [ "let someCall(x) = null"
                     "let xe = someCall(System.IO.StringReader()  "]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        TakeCoffeeBreak(this.VS)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        // In this case, we quickly type "." and then get dot-completions
        // This simulates the case when the user quickly types "dot" after the file has been TCed before.
        ReplaceFileInMemoryWithoutCoffeeBreak file [ "let someCall(x) = null"
                                                     "let xe = someCall(System.IO.StringReader(). "]
        MoveCursorToEndOfMarker(file, "().")
        // Note: no TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContainsAll(completions, ["ReadBlock"]) // text to the left of the dot did not change, so we use stale (correct) result immediately
        // if we wait...
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        // ... we get the expected answer
        AssertCompListContainsAll(completions, ["ReadBlock"])
        gpatcc.AssertExactly(0,0)


//*********************************************Previous Completion test and helper*****
    member private this.VerifyCompListDoesNotContainAnyAtStartOfMarker(fileContents : string, marker : string, list : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToStartOfMarker(file, marker)    
        let completions = AutoCompleteAtCursor(file)
        AssertCompListDoesNotContainAny(completions,list)

    member private this.VerifyCtrlSpaceListDoesNotContainAnyAtStartOfMarker(fileContents : string, marker : string, list : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToStartOfMarker(file, marker)    
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListDoesNotContainAny(completions,list)

    member private this.VerifyCompListContainAllAtStartOfMarker(fileContents : string, marker : string, list : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToStartOfMarker(file, marker)
        let completions = AutoCompleteAtCursor(file)
        AssertCompListContainsAll(completions, list)

    member private this.VerifyCtrlSpaceListContainAllAtStartOfMarker(fileContents : string, marker : string, list : string list, ?coffeeBreak:bool, ?addtlRefAssy:string list) =
        let coffeeBreak = defaultArg coffeeBreak false
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)
        MoveCursorToStartOfMarker(file, marker)
        if coffeeBreak then TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContainsAll(completions, list)

        
    member private this.VerifyAutoCompListIsEmptyAtEndOfMarker(fileContents : string, marker : string) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToEndOfMarker(file, marker)
        let completions = AutoCompleteAtCursor(file)   
        AssertEqual(0,completions.Length)              

    member private this.VerifyCtrlSpaceCompListIsEmptyAtEndOfMarker(fileContents : string, marker : string) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToEndOfMarker(file, marker)
        let completions = CtrlSpaceCompleteAtCursor(file)   
        AssertEqual(0,completions.Length)              
                
                    
         
    // Regression for bug 2116 -- Consider making selected item in completion list case-insensitive       
      




(*------------------------------------------IDE Query automation start -------------------------------------------------*)
    member private this.AssertAutoCompletionInQuery(fileContent : string list, marker:string,contained:string list) =
        let file = createFile fileContent SourceFileKind.FS ["System.Xml.Linq"] None
            
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, marker)
        let completions = CompleteAtCursorForReason(file,BackgroundRequestReason.CompleteWord)
        AssertCompListContainsAll(completions, contained)
        gpatcc.AssertExactly(0,0)



                    










    member private this.AssertDotCompletionListInQuery(fileContents: string, marker : string, list : string list) =
        let datacode = """
        namespace DataSource
        open System
        open System.Xml.Linq
        type Product() =
            let mutable id = 0
            let mutable name = ""
            let mutable category = ""
            let mutable price = 0M
            let mutable unitsInStock = 0
            member x.ProductID with get() = id and set(v) = id <- v
            member x.ProductName with get() = name and set(v) = name <- v
            member x.Category with get() = category and set(v) = category <- v
            member x.UnitPrice with get() = price and set(v) = price <- v
            member x.UnitsInStock with get() = unitsInStock and set(v) = unitsInStock <- v

        module Products =
            let getProductList() =
                [
                Product(ProductID = 1, ProductName = "Chai", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 39 );
                Product(ProductID = 2, ProductName = "Chang", Category = "Beverages", UnitPrice = 19.0000M, UnitsInStock = 17 ); 
                Product(ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", UnitPrice = 10.0000M, UnitsInStock = 13 );
                ] 
        """
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        this.AddAssemblyReference(project, "System.Xml.Linq")
        let file1 = AddFileFromTextBlob(project,"File1.fs",datacode)
        //build
        let file2 = AddFileFromTextBlob(project,"File2.fs",fileContents)
        let file1 = OpenFile(project,"File1.fs")
        let file2 = OpenFile(project,"File2.fs")

        TakeCoffeeBreak(this.VS)
        let completions = DotCompletionAtStartOfMarker file2 marker
        AssertCompListContainsAll(completions, list) 


    // Intellisense still appears on arguments when the operator is used in error 






type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)


               
