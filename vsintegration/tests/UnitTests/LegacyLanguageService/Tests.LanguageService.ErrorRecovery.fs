// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.ErrorRecovery

open System
open System.IO
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

type UsingMSBuild()  = 
    inherit LanguageServiceBaseTests()

    //Verify the error list contained the expected string
    member private this.VerifyErrorListContainedExpectedString(fileContents : string, expectedStr : string) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)       
        let errorList = GetErrors(project)
        let GetErrorMessages(errorList : Error list) =
            [ for i = 0 to errorList.Length - 1 do
                yield errorList.[i].Message]
            
        Assert.True(errorList
                          |> GetErrorMessages
                          |> Seq.exists (fun errorMessage ->
                                errorMessage.Contains(expectedStr)))

    // Not a recovery case, but make sure we get a squiggle at the unfinished Main()
    [<Fact>]
    member public this.``ErrorRecovery.Bug4538_4``() =  
        let fileContent = """
            type MyType() = 
                override x.ToString() = ""
            let Main() =
                use x = MyType()"""
        let expectedStr = "The block following this 'use' is unfinished. Every code block is an expression and must have a result. 'use' cannot be the final code element in a block. Consider giving this block an explicit result."
        this.VerifyErrorListContainedExpectedString(fileContent,expectedStr)


// Context project system
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
