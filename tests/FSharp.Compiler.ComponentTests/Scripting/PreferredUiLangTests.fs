// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Scripting

open Xunit
open System
open System.IO
open FSharp.Test
open FSharp.Test.CompilerAssertHelpers
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Diagnostics

module ``PreferredUiLang tests`` =

    [<Fact>]
    let ``preferreduilang switch before script is consumed from CommandLineArgs``() =
        let scriptContent = """
printfn "Args: %A" fsi.CommandLineArgs
if fsi.CommandLineArgs.Length <> 2 then exit 1
if fsi.CommandLineArgs.[0] <> __SOURCE_FILE__ then exit 1
if fsi.CommandLineArgs.[1] <> "arg1" then exit 1
exit 0
"""
        let tmpFile = Path.GetTempFileName() + ".fsx"
        try
            File.WriteAllText(tmpFile, scriptContent)
            let errors, _, _ = 
                CompilerAssert.RunScriptWithOptionsAndReturnResult 
                    [| "--preferreduilang:es-ES"; tmpFile; "arg1" |] 
                    ""
            
            // Should succeed (exit 0)
            Assert.True((errors: ResizeArray<string>).Count = 0, sprintf "Expected no errors, got: %A" errors)
        finally
            if File.Exists(tmpFile) then File.Delete(tmpFile)

    [<Fact>]
    let ``preferreduilang switch after script is consumed from CommandLineArgs``() =
        let scriptContent = """
printfn "Args: %A" fsi.CommandLineArgs
if fsi.CommandLineArgs.Length <> 2 then exit 1
if fsi.CommandLineArgs.[0] <> __SOURCE_FILE__ then exit 1
if fsi.CommandLineArgs.[1] <> "arg1" then exit 1
exit 0
"""
        let tmpFile = Path.GetTempFileName() + ".fsx"
        try
            File.WriteAllText(tmpFile, scriptContent)
            let errors, _, _ = 
                CompilerAssert.RunScriptWithOptionsAndReturnResult 
                    [| tmpFile; "--preferreduilang:es-ES"; "arg1" |] 
                    ""
            
            // Should succeed (exit 0)
            Assert.True((errors: ResizeArray<string>).Count = 0, sprintf "Expected no errors, got: %A" errors)
        finally
            if File.Exists(tmpFile) then File.Delete(tmpFile)

    [<Fact>]
    let ``preferreduilang with slash form is consumed from CommandLineArgs``() =
        let scriptContent = """
printfn "Args: %A" fsi.CommandLineArgs
if fsi.CommandLineArgs.Length <> 2 then exit 1
if fsi.CommandLineArgs.[0] <> __SOURCE_FILE__ then exit 1
if fsi.CommandLineArgs.[1] <> "arg1" then exit 1
exit 0
"""
        let tmpFile = Path.GetTempFileName() + ".fsx"
        try
            File.WriteAllText(tmpFile, scriptContent)
            let errors, _, _ = 
                CompilerAssert.RunScriptWithOptionsAndReturnResult 
                    [| tmpFile; "/preferreduilang:de-DE"; "arg1" |] 
                    ""
            
            // Should succeed (exit 0)
            Assert.True((errors: ResizeArray<string>).Count = 0, sprintf "Expected no errors, got: %A" errors)
        finally
            if File.Exists(tmpFile) then File.Delete(tmpFile)

    [<Fact>]
    let ``preferreduilang sets CurrentUICulture correctly``() =
        let scriptContent = """
let culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
printfn "Culture: %s" culture
if not (culture.StartsWith("fr-FR")) then 
    printfn "Expected culture starting with fr-FR, got: %s" culture
    exit 1
exit 0
"""
        let tmpFile = Path.GetTempFileName() + ".fsx"
        try
            File.WriteAllText(tmpFile, scriptContent)
            let errors, _, _ = 
                CompilerAssert.RunScriptWithOptionsAndReturnResult 
                    [| "--preferreduilang:fr-FR"; tmpFile |] 
                    ""
            
            // Should succeed (exit 0)
            Assert.True((errors: ResizeArray<string>).Count = 0, sprintf "Expected no errors, got: %A" errors)
        finally
            if File.Exists(tmpFile) then File.Delete(tmpFile)

    [<Fact>]
    let ``preferreduilang after script also sets culture``() =
        let scriptContent = """
let culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
printfn "Culture: %s" culture
if not (culture.StartsWith("ja-JP")) then 
    printfn "Expected culture starting with ja-JP, got: %s" culture
    exit 1
exit 0
"""
        let tmpFile = Path.GetTempFileName() + ".fsx"
        try
            File.WriteAllText(tmpFile, scriptContent)
            let errors, _, _ = 
                CompilerAssert.RunScriptWithOptionsAndReturnResult 
                    [| tmpFile; "--preferreduilang:ja-JP" |] 
                    ""
            
            // Should succeed (exit 0)
            Assert.True((errors: ResizeArray<string>).Count = 0, sprintf "Expected no errors, got: %A" errors)
        finally
            if File.Exists(tmpFile) then File.Delete(tmpFile)

    [<Fact>]
    let ``invalid culture in preferreduilang is ignored gracefully``() =
        let scriptContent = """
printfn "Args: %A" fsi.CommandLineArgs
if fsi.CommandLineArgs.Length <> 2 then exit 1
if fsi.CommandLineArgs.[0] <> __SOURCE_FILE__ then exit 1
if fsi.CommandLineArgs.[1] <> "arg1" then exit 1
exit 0
"""
        let tmpFile = Path.GetTempFileName() + ".fsx"
        try
            File.WriteAllText(tmpFile, scriptContent)
            let errors, _, _ = 
                CompilerAssert.RunScriptWithOptionsAndReturnResult 
                    [| tmpFile; "--preferreduilang:invalid-culture-xyz"; "arg1" |] 
                    ""
            
            // Should succeed - invalid culture is ignored, but switch is still consumed
            Assert.True((errors: ResizeArray<string>).Count = 0, sprintf "Expected no errors, got: %A" errors)
        finally
            if File.Exists(tmpFile) then File.Delete(tmpFile)
