// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.QuickInfo

open System
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem
open Xunit

[<AutoOpen>]
module QuickInfoStandardSettings = 
    let standard40AssemblyRefs  = [ "System"; "System.Core"; "System.Numerics" ]
    let queryAssemblyRefs = [ "System.Xml.Linq"; "System.Core" ]

type UsingMSBuild() = 
    inherit LanguageServiceBaseTests()

    // Work around an innocuous 'feature' with how QuickInfo is displayed, lines which 
    // should have a "\r\n" just have a "\r"
    let trimnewlines (str : string) = 
        str.Replace("\r", "").Replace("\n", "")
    
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

    let checkTooltip expected ((tooltip, span : TextSpan), (row, col)) = 
        // Instrumentation: Show actual tooltip for debugging
        printfn "=== ACTUAL TOOLTIP START ==="
        printfn "%s" tooltip
        printfn "=== ACTUAL TOOLTIP END ==="
        printfn "=== EXPECTED TOOLTIP START ==="
        printfn "%s" expected
        printfn "=== EXPECTED TOOLTIP END ==="
        AssertContains(tooltip, expected)
        // cursor should be inside the span
        Assert.True(row = (span.iStartLine + 1) && row = (span.iEndLine + 1), "Cursor should be one the same line with the tooltip span")
        Assert.True(col >= span.iStartIndex && col <= span.iEndIndex, "Cursor should be located inside the span")


//    (* Tests for QuickInfos ---------------------------------------------------------------- *)
    member public this.InfoInDeclarationTestQuickInfoImplWithTrim (code : string) marker expected =
        let (_, _, file) = this.CreateSingleFileProject(code)
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToStartOfMarker(file, marker)
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertContains(trimnewlines tooltip, trimnewlines expected) 
        gpatcc.AssertExactly(0,0)

    member public this.CheckTooltip(code : string,marker,atStart, f, ?addtlRefAssy : string list) =
        let (_, _, file) = this.CreateSingleFileProject(code, ?references = addtlRefAssy)

        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        if atStart then
            MoveCursorToStartOfMarker(file, marker)
        else
            MoveCursorToEndOfMarker(file, marker)
        let pos = GetCursorLocation file
        let tooltip = GetQuickInfoAndSpanAtCursor file
        f (tooltip, pos)
        gpatcc.AssertExactly(0,0)
                         
    member public this.InfoInDeclarationTestQuickInfoImpl(code,marker,expected,atStart, ?addtlRefAssy : string list) =
        let check ((tooltip, _), _) = AssertContains(tooltip, expected)
        this.CheckTooltip(code, marker, atStart, check, ?addtlRefAssy=addtlRefAssy )

    member public this.AssertQuickInfoContainsAtEndOfMarker(code,marker,expected, ?addtlRefAssy : string list) =
        this.InfoInDeclarationTestQuickInfoImpl(code,marker,expected,false,?addtlRefAssy=addtlRefAssy)

    member public this.AssertQuickInfoContainsAtStartOfMarker(code, marker, expected, ?addtlRefAssy : string list) =
        this.InfoInDeclarationTestQuickInfoImpl(code,marker,expected,true,?addtlRefAssy=addtlRefAssy)
        
    member public this.VerifyQuickInfoDoesNotContainAnyAtEndOfMarker (code : string) marker notexpected =
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, marker)
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertNotContains(tooltip, notexpected)         
        gpatcc.AssertExactly(0,0)

    member public this.VerifyQuickInfoDoesNotContainAnyAtStartOfMarker (code : string) marker notexpected =
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToStartOfMarker(file, marker)
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertNotContains(tooltip, notexpected)         
        gpatcc.AssertExactly(0,0)

    member public this.AssertIdentifierInToolTipExactlyOnce (code : string) marker ident =
    /// Asserts that an identifier occurs exactly once in a tooltip
        let AssertIdentifierInToolTipExactlyOnce(ident, (tooltip:string)) =
            let count = tooltip.Split([| '='; '.'; ' '; '\t'; '('; ':'; ')'; '\n' |]) |> Array.filter ((=) ident) |> Array.length
            if (count <> 1) then
                Assert.Fail(sprintf "Identifier '%s' doesn't occur once in the tooltip '%s'" ident tooltip)
        
        let (_, _, file) = this.CreateSingleFileProject(code)

        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file, marker)
        let tooltip = GetQuickInfoAtCursor file
        AssertIdentifierInToolTipExactlyOnce(ident, tooltip)
        gpatcc.AssertExactly(0,0)

    member this.VerifyOrderOfNestedTypesInQuickInfo (source : string, marker : string, expectedExactOrder : string list, ?extraRefs : string list) = 
        let (_, _, file) = this.CreateSingleFileProject(source, ?references = extraRefs)
        
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToStartOfMarker(file, "(*M*)")
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertContainsInOrder(tooltip, expectedExactOrder)

    
            
    


    



    
    // Regression for 2948
    

    // Regression for 2494
        

   

          
          
          
          


          
          
          

          




    [<Fact>]
    member public this.``JustAfterIdentifier``() =
        this.AssertQuickInfoContainsAtEndOfMarker
          ("""let f x = x + 1 ""","let f","int")
        
          

    // Disabled due to issue #11752   ---  https://github.com/dotnet/fsharp/issues/11752
    //[<Fact>]
    member public this.``Regression.ModulesFromExternalLibrariesBug5785``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let projectLib = CreateProject(solution,"testlib")
        let file = AddFileFromText(projectLib,"MyLibrary.fs", 
                      [ "module MyLibrary"
                        "let x = 1"
                        "module Nested ="
                        "    let y = 2"
                        "    module Deeper ="
                        "        let z = 3"
                      ])
        let project = CreateProject(solution,"testapp")
        let file = AddFileFromText(project,"App.fs", 
                      [ "let a = MyLibrary.Nested.Deeper.z" ])
        SetConfigurationAndPlatform(project, "Debug|AnyCPU")  // we must set config/platform when building with ProjectReferences
        SetConfigurationAndPlatform(projectLib, "Debug|AnyCPU")  // we must set config/platform when building with ProjectReferences
        AddProjectReference(project, projectLib)
        let br = BuildTarget(projectLib, "Build") // build the dependent library
        Assert.True(br.BuildSucceeded, "build should succeed")
        let file = OpenFile(project,"App.fs")
        TakeCoffeeBreak(this.VS) // Wait for the background compiler to catch up.

        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)

        ShiftKeyUp(this.VS)
        MoveCursorToEndOfMarker(file, "MyLi")
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertMatchesRegex '\n' "module MyLibrary[Filename:.*\\bin\\Debug\\testlib.exe]\n[Signature:T:MyLibrary]" tooltip  

        MoveCursorToEndOfMarker(file, "Nes")
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertMatchesRegex '\n' "module Nested\n\nfrom MyLibrary[Filename:.*\\bin\Debug\\testlib.exe]\n[Signature:T:MyLibrary.Nested]" tooltip

        MoveCursorToEndOfMarker(file, "Dee")
        let tooltip = time1 GetQuickInfoAtCursor file "Time of first tooltip"
        AssertMatchesRegex '\n' "module Deeper\n\nfrom MyLibrary.Nested[Filename:.*\\bin\\Debug\\testlib.exe]\n[Signature:T:MyLibrary.Nested]" tooltip

        gpatcc.AssertExactly(0,0)

    (* ------------------------------------------------------------------------------------- *)













    member private this.QuickInfoResolutionTest lines queries =
        let code = [ yield! lines ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        TakeCoffeeBreak(this.VS) 


        // Move along the entire length of the identifier checking that the tooltip text contains something familiar
        for (initial,ident:string,expectedText) in queries do
            for i in 0..ident.Length-1 do
                let marker = initial+ident.[0..i-1]
                MoveCursorToEndOfMarker(file,marker)
                let tooltip = time1 GetQuickInfoAtCursor file "Time for tooltip"
                printf "QuickInfo at marker '%s' is '%s', expect '%s'\n" marker tooltip expectedText
                Assert.True(tooltip.Contains(expectedText))

    member public this.GetLongPathsTestCases() =
            ["let test0 = System.Console.In"
             "let test0b = System.Collections.Generic.List<int>()"
             "let test0c = System.Collections.Generic.KeyNotFoundException()"
             "type Test0d = System.Collections.Generic.List<int>"
             "type Test0e = System.Collections.Generic.KeyNotFoundException"],
        
              // The quick info specification                                    // Some of the expected quick info text
             [("let test0 = ","System"                                           ,"namespace System"); 
              ("let test0 = System.","Console"                                   ,"Console =");  
              ("let test0 = System.Console.","In"                                ,"System.Console.In");  
              ("let test0 = System.Console.","In"                                ,"TextReader");  
              ("let test0b = ","System"                                          ,"namespace System"); 
              ("let test0b = System.","Collections"                              ,"namespace System.Collections");  
              ("let test0b = System.Collections.","Generic"                      ,"namespace System.Collections.Generic");  
              ("let test0b = System.Collections.Generic.","List"                 ,"List()"); // note resolves to constructor  
              ("let test0c = ","System"                                          ,"namespace System"); 
              ("let test0c = System.","Collections"                              ,"namespace System.Collections");  
              ("let test0c = System.Collections.","Generic"                      ,"namespace System.Collections.Generic");  
              ("let test0c = System.Collections.Generic.","KeyNotFoundException" ,"KeyNotFoundException()");  // note resolves to constructor  
              ("type Test0d = ","System"                                         ,"namespace System"); 
              ("type Test0d = System.","Collections"                             ,"namespace System.Collections");  
              ("type Test0d = System.Collections.","Generic"                     ,"namespace System.Collections.Generic");  
              ("type Test0d = System.Collections.Generic.","List"                ,"Generic.List"); // note resolves to type
              ("type Test0e = ","System"                                         ,"namespace System"); 
              ("type Test0e = System.","Collections"                             ,"namespace System.Collections");  
              ("type Test0e = System.Collections.","Generic"                     ,"namespace System.Collections.Generic");  
              ("type Test0e = System.Collections.Generic.","KeyNotFoundException","Generic.KeyNotFoundException");  // note resolves to type
             ]
        

        
        

        
        
        
        
        

        
        







            

    // Check to see that two distinct projects can be present
        
    // In this bug, relative paths with .. in them weren't working.


        

    /// Complete a member completion and confirm that its data tip contains the fragments
    /// in rhsContainsOrder
    member public this.AssertMemberDataTipContainsInOrder(code : string list,marker,completionName,rhsContainsOrder) =
        let code = code |> Seq.collect (fun s -> s.Split [|'\r'; '\n'|]) |> List.ofSeq
        let (_, project, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        TakeCoffeeBreak(this.VS) (* why needed? *)       
        MoveCursorToEndOfMarker(file,marker)
        let completions = CtrlSpaceCompleteAtCursor file
        match completions |> Array.tryFind (fun (CompletionItem(name, _, _, _, _)) -> name = completionName) with
        | Some(CompletionItem(_, _, _, descrFunc, _)) ->
            let descr = descrFunc()
            AssertContainsInOrder(descr,rhsContainsOrder)
        | None -> 
            ShowErrors(project)
            failwith $"Could not find completion name '{completionName}'"














            
    [<Fact>]
    member public this.``Regression.OnMscorlibMethodInScript.Bug6489``() =
        this.AssertMemberDataTipContainsInOrder
            ((*code *)
              [
               "let Func() = 
    let actual = [| |]
    actual."
               ] ,
             (* marker *)
             "actual.",
             (* completed item *)             
             "CopyTo", 
             (* expect to see in order... *)
             [
              "[Filename"; "Reference Assemblies\Microsoft\Framework\.NETFramework"; "mscorlib.dll]";
              "[Signature:M:System.Array.CopyTo("
             ]
            ) 
              
              






(*------------------------------------------IDE automation starts here -------------------------------------------------*)























    member private this.VerifyUsingFsTestLib fileContent queries crossProject =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let testLibCode ="""namespace FSTestLib

                            /// DocComment: This is MyStruct type, represents a struct.
                            type MyPoint =
                                struct
                                    val mutable private m_X : float
                                    val mutable private m_Y : float
        
                                    new (x, y) = { m_X = x; m_Y = y }
        
                                    /// Gets and sets X
                                    member this.X with get () = this.m_X and set x = this.m_X <- x
        
                                    /// Gets and sets Y
                                    member this.Y with get () = this.m_Y and set y = this.m_Y <- y
        
                                    // Length of given Point
                                    member this.Len = sqrt ( this.X * this.X + this.Y * this.Y )
        
                                    static member (+) (p1 : MyPoint, p2 : MyPoint) = MyPoint(p1.X + p2.X, p1.Y + p2.Y)
        
                                end

                            [<NoComparison;NoEquality>]
                            /// DocComment: This is my record type.
                            type MyEmployee = 
                                { mutable Name  : string;
                                  mutable Age   : int;
                                  /// DocComment: Indicates whether the employee is full time or not
                                  mutable IsFTE : bool }
    
                                interface System.IComparable with
                                    member this.CompareTo (emp : obj) = 
                                        let r = emp :?> MyEmployee
                                        match r.IsFTE && this.IsFTE with
                                        | true -> this.Age - r.Age
                                        | _ -> System.Convert.ToInt32(this.IsFTE) - System.Convert.ToInt32(r.IsFTE)
    
                                override this.ToString() = sprintf "%s is %d." this.Name this.Age
    
                                /// DocComment: Method
                                static member MakeDummy () =
                                    { Name = System.String.Empty; Age = -1; IsFTE = false }
    
                                // TODO: Normally there's no DotCompletion after "this" here
                                override this.Equals(ob : obj) = 
                                    let r = ob :?> MyEmployee
                                    this.Name = r.Name && this.Age = r.Age && this.IsFTE = r.IsFTE

                            /// DocComment: This is my interface type
                            type IMyInterface = 
                                interface
                                    /// DocComment: abstract method in Interface
                                    abstract Represent : unit -> string        
                                end

                            // TODO: add formatable ToString()
                            /// DocComment: This is my discriminated union type
                            type MyDistance =
                                | Kilometers of float
                                | Miles of float
                                | NauticalMiles of float
    
    
                                /// DocComment: Static Method
                                static member toMiles x =
                                    Miles(
                                        match x with
                                        | Miles x -> x
                                        | Kilometers x -> x / 1.6
                                        | NauticalMiles x -> x * 1.15
                                    )

                                /// DocComment: Property        
                                member this.toNautical =
                                    NauticalMiles(
                                        match this with
                                        | Kilometers x -> x / 1.852
                                        | Miles x -> x / 1.15
                                        | NauticalMiles x -> x
                                    )

                                /// DocComment: Method        
                                member this.IncreaseBy dist = 
                                    match this with
                                    | Kilometers x -> Kilometers (x + dist)
                                    | Miles x -> Miles (x + dist)
                                    | NauticalMiles x -> NauticalMiles (x + dist)
        
                                /// DocComment: Event
                                static member Event = 
                                    let evnt = new Event<string>()
                                    evnt
        
                            /// DocComment: This is my enum type
                            type MyColors =
                                | /// DocComment: Field
                                  Red = 0
                                | Green = 1
                                | Blue = 2
    
                            /// DocComment: This is my class type
                            type MyCar( number: int, color:MyColors) =
                                /// DocComment: This is static field
                                static member Owner     = "MySelf"
                                /// DocComment: This is instance field
                                member this.Number      = number
                                member this.Color       = color
                                /// DocComment: This is static method
                                static member Run (number:int)      = printf "%s" (number.ToString()+"Running")
                                /// DocComment: This is instance method
                                member this.Repair  (expense:int)  = printf "%s" ("Spent " + expense.ToString() + " for repairing. ")
    
                            /// DocComment: This is my delegate type
                            type ControlEventHandler = delegate of int -> unit"""
        let file2 =
            if (crossProject = true) then
                let file1 = AddFileFromTextBlob(project,"File1.fs",testLibCode)
                let project2 = CreateProject(solution,"codeProject")
                let file2 = AddFileFromTextBlob(project2,"File2.fs",fileContent)
                Build(project).BuildSucceeded |> ignore
                AddProjectReference(project2, project)
                let file1 = OpenFile(project,"File1.fs")
                let file2 = OpenFile(project2,"File2.fs")
                file2
            else
                let file1 = AddFileFromTextBlob(project,"File1.fs",testLibCode)
                let file2 = AddFileFromTextBlob(project,"File2.fs",fileContent)
                let file1 = OpenFile(project,"File1.fs")
                let file2 = OpenFile(project,"File2.fs")
                file2
        //Build(project).BuildSucceeded |> printf "%b"
        for (marker, expectedTip) in queries do
            MoveCursorToStartOfMarker(file2, marker)
            let tooltip = time1 GetQuickInfoAtCursor file2 "Time of first tooltip"
            printf "First-%s\n" tooltip
            AssertContains(tooltip, expectedTip)

                

    member private this.AssertQuickInfoInQuery(code: string, mark : string, expectedstring : string) =
        use _guard = this.UsingNewVS()
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
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        this.AddAssemblyReference(project, "System.Xml.Linq")
        let file1 = AddFileFromTextBlob(project,"File1.fs",datacode)
        //build
        let file2 = AddFileFromTextBlob(project,"File2.fs",code)
        let file1 = OpenFile(project,"File1.fs")
        let file2 = OpenFile(project,"File2.fs")
        
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToStartOfMarker(file2,mark)
        let tooltip = time1 GetQuickInfoAtCursor file2 "Time of first tooltip"
        printfn "%s" tooltip
        AssertContains(tooltip, expectedstring) 
        gpatcc.AssertExactly(0,0)   



// Context project system
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
