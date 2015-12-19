// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests.LanguageService

open System
open System.IO
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService

type ErrorRecoveryTests()  = 
    inherit LanguageServiceBaseTests()

    //Verify the error list containd the expected string
    member private this.VerifyErrorListContainedExpectedString(fileContents : string, expectedStr : string) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)       
        let errorList = GetErrors(project)
        let GetErrorMessages(errorList : Error list) =
            [ for i = 0 to errorList.Length - 1 do
                yield errorList.[i].Message]
            
        Assert.IsTrue(errorList
                          |> GetErrorMessages
                          |> Seq.exists (fun errorMessage ->
                                errorMessage.Contains(expectedStr)))

   // Not a recovery case, but make sure we get a squiggle at the unfinished Main()
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4538_3``() = 
        let fileContent = """
            type MyType() = 
                override x.ToString() = ""
            let Main() =
                let x = MyType()"""
        let expectedStr = "Block following this 'let' is unfinished"
        this.VerifyErrorListContainedExpectedString(fileContent,expectedStr)

    // Not a recovery case, but make sure we get a squiggle at the unfinished Main()
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4538_4``() =  
        let fileContent = """
            type MyType() = 
                override x.ToString() = ""
            let Main() =
                use x = MyType()"""
        let expectedStr = "Block following this 'use' is unfinished"
        this.VerifyErrorListContainedExpectedString(fileContent,expectedStr)

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    [<Category("PerfCheck")>]
    member public this.``ErrorRecovery.Bug4881_1``() =  
        let code = 
                                    ["let s = \"\""
                                     "if true then"
                                     "    ()"
                                     "elif s."   
                                     "else ()"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"elif s.")
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContains(completions,"Split")       
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4881_2``() =  
        let code =
                                    ["let s = \"\""
                                     "if true then"
                                     "    ()"
                                     "elif true"   
                                     "elif s."   
                                     "else ()"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"elif s.")
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContains(completions,"Split")      

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4881_3``() =  
        let code = 
                                    ["let s = \"\""
                                     "if true then"
                                     "    ()"
                                     "elif s."   
                                     "elif true"   
                                     "else ()"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"elif s.")
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContains(completions,"Split")  
        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4881_4``() =  
        let code = 
                                    ["let s = \"\""
                                     "if true then"
                                     "    ()"
                                     "elif s."   
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"elif s.")
        TakeCoffeeBreak(this.VS)
        let completions = AutoCompleteAtCursor file
        AssertCompListContains(completions,"Split")    
        
        
    // This case was fixed while investigating 4538.            
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.NotFixing4538_1``() =  
        let code = 
                                    ["type MyType() = "
                                     "    override x.ToString() = \"\""
                                     "let Main() ="
                                     "    let _ = new MyT"
                                     "    ()"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"new MyT")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"MyType")
        
    // This case was fixed while investigating 4538.            
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.NotFixing4538_2``() =  
        let code =
                                    ["type MyType() = "
                                     "    override x.ToString() = \"\""
                                     "let Main() ="
                                     "    let _ = MyT"
                                     "    ()"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"= MyT")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"MyType")
        
    // This case was fixed while investigating 4538.            
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.NotFixing4538_3``() =  
        let code = 
                                    ["type MyType() = "
                                     "    override x.ToString() = \"\""
                                     "let Main() ="
                                     "    let _ = MyT"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"= MyT")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"MyType")
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4538_1``() =  
        let code = 
                                    ["type MyType() = "
                                     "    override x.ToString() = \"\""
                                     "let Main() ="
                                     "    let _ = MyT"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        
        MoveCursorToEndOfMarker(file,"= MyT")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"MyType")    
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4538_2``() =  
        let code = 
                                    ["type MyType() = "
                                     "    override x.ToString() = \"\""
                                     "let Main() ="
                                     "    let x = MyType()"
                                     "    let _ = MyT"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"_ = MyT")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"MyType")  
        

        
      
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4538_5``() =  
        let code = 
                                    ["type MyType() = "
                                     "    override x.ToString() = \"\""
                                     "let Main() ="
                                     "    use x = null"
                                     "    use _ = MyT"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"_ = MyT")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"MyType")          
        
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("error_recovery")>]
    member public this.``ErrorRecovery.Bug4594_1``() =  
        let code = 
                                    ["let Bar(xyz) ="
                                     "    let hello ="
                                     "        if x"
                                     ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        MoveCursorToEndOfMarker(file,"if x")
        TakeCoffeeBreak(this.VS)
        let completions = CtrlSpaceCompleteAtCursor file
        AssertCompListContains(completions,"xyz")     

    /// In this bug, the Module. at the very end of the file was treated as if it were in the scope
    /// of Module rather than right after it. This check just makes sure we can see a data tip because
    /// Module is available.
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ErrorRecovery.5878_1``() =
        Helper.AssertMemberDataTipContainsInOrder
            (this.TestRunner,
            (*code *)
              [
               "module Module ="
               "    /// Union comment"
               "    type Union ="
               "        /// Case comment"
               "        | Case of int"
               "Module."
               ] ,
             (* marker *)
             "Module.",
             (* completed item *)             
             "Case", 
             (* expect to see in order... *)
             [
              "union case Module.Union.Case: int -> Module.Union";
              "Case comment";
             ]
            )

//Allow the TimeStampTests run under different context
namespace UnitTests.Tests.LanguageService.ErrorRecovery
open UnitTests.Tests.LanguageService
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem
open NUnit.Framework
open Salsa.Salsa

// context msbuild
[<Parallelizable(ParallelScope.Self)>][<TestFixture>] 
[<Category("LanguageService.MSBuild")>]
type ``MSBuild`` = 
   inherit ErrorRecoveryTests
   new() = { inherit ErrorRecoveryTests(VsOpts = fst (Models.MSBuild())); }

// Context project system
[<Parallelizable(ParallelScope.Self)>][<TestFixture>] 
[<Category("LanguageService.ProjectSystem")>]
type ``ProjectSystem`` = 
    inherit ErrorRecoveryTests
    new() = { inherit ErrorRecoveryTests(VsOpts = LanguageServiceExtension.ProjectSystem); } 