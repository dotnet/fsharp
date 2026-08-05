// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.ErrorList

open System
open System.IO
open Xunit
open Microsoft.VisualStudio.FSharp
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

type UsingMSBuild() as this = 
    inherit LanguageServiceBaseTests()

    let VerifyErrorListContainedExpectedStr(expectedStr:string,project : OpenProject) = 
        let convertNewlines (s:string) = s.Replace("\r\n", " ").Replace("\n", " ")
        let errorList = GetErrors(project)
        let GetErrorMessages(errorList : Error list) =
            [ for i = 0 to errorList.Length - 1 do
                yield errorList.[i].Message]
        let msgs = GetErrorMessages errorList
        if msgs |> Seq.exists (fun errorMessage -> convertNewlines(errorMessage).Contains(convertNewlines expectedStr)) then
            ()
        else
            Assert.Fail(sprintf "Expected errors to contain '%s' but it was not there; errors were %A" expectedStr msgs)

    let GetWarnings(project : OpenProject) =
        let errorList = GetErrors(project)
        [for error in errorList do
            if (error.Severity = Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning) then
                yield error]
    
    let CheckErrorList (content : string) f : unit = 
        let (_, project, file) = this.CreateSingleFileProject(content)
        Build(project) |> ignore

        TakeCoffeeBreak(this.VS)
        let errors = GetErrors project
        printfn "==="
        for err in errors do
            printfn "%s" err.Message
        f errors

    let assertContains (errors : list<Error>) text = 
        let ok = errors |> List.exists (fun err -> err.Message = text)
        Assert.True(ok, sprintf "Error list should contain '%s' message" text)

    let assertExpectedErrorMessages expected (actual: list<Error>) =
        let normalizeCR input = System.Text.RegularExpressions.Regex.Replace(input, @"\r\n|\n\r|\n|\r", "\r\n")
        let actual = 
            actual 
            |> Seq.map (fun e -> e.Message)
            |> String.concat Environment.NewLine
            |> normalizeCR
        let expected = expected |> String.concat Environment.NewLine |> normalizeCR
        
        Assert.Equal(expected, actual)

    //verify the error list Count
    member private this.VerifyErrorListCountAtOpenProject(fileContents : string, num : int) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        let errorList = GetErrors(project)
        let errorTexts = new System.Text.StringBuilder()
        for error in errorList do
            printfn "%A" error.Severity
            let s = error.ToString()
            errorTexts.AppendLine s |> ignore
            printf "%s\n" s 

        if num <> errorList.Length then 
            failwithf "The error list number is not the expected %d but %d%s%s" 
                num 
                errorList.Length
                System.Environment.NewLine
                (errorTexts.ToString())

    //Verify the warning list Count
    member private this.VerifyWarningListCountAtOpenProject(fileContents : string, expectedNum : int, ?addtlRefAssy : string list) = 
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)
        
        TakeCoffeeBreak(this.VS) // Wait for the background compiler to catch up.
        let warnList = GetWarnings(project)
        Assert.Equal(expectedNum,warnList.Length)

    //verify no the error list 
    member private this.VerifyNoErrorListAtOpenProject(fileContents : string, ?addtlRefAssy : string list) = 
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)
        
        TakeCoffeeBreak(this.VS) // Wait for the background compiler to catch up.
        let errorList = GetErrors(project)      
        for error in errorList do
            printfn "%A" error.Severity
            printf "%s\n" (error.ToString()) 
        Assert.True(errorList.IsEmpty)
    
    //Verify the error list contained the expected string
    member private this.VerifyErrorListContainedExpectedString(fileContents : string, expectedStr : string, ?addtlRefAssy : string list) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)
        
        TakeCoffeeBreak(this.VS) // Wait for the background compiler to catch up.
       
        VerifyErrorListContainedExpectedStr(expectedStr,project)

    //Verify error Count at specified file
    member private this.VerifyCountAtSpecifiedFile(project : OpenProject ,num : int) = 
        TakeCoffeeBreak(this.VS)
        let errorList = GetErrors(project)
        for error in errorList do
            printfn "%A" error.Severity
            printf "%s\n" (error.ToString()) 
        if (num = errorList.Length) then 
                ()
            else
                failwithf "The error list number is not the expected %d" num
    
    [<Fact>]
    //This is an verify action test & example
    member public this.``TestErrorMessage``() =
        let fileContent = """Console.WriteLine("test")"""
        let expectedStr = "The value, namespace, type or module 'Console' is not defined"
        this.VerifyErrorListContainedExpectedString(fileContent,expectedStr)
    

type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
