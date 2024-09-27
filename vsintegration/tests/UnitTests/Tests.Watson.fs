// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.Compiler.Watson

#nowarn "52" // The value has been copied to ensure the original is not mutated

open FSharp.Compiler
open FSharp.Compiler.IO
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CodeAnalysis
open Internal.Utilities.Library 
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Driver
open Xunit
open System.IO

type Check = 
    static member public FscLevelException<'TException when 'TException :> exn>(simulationCode)  =
        try 
            try
#if DEBUG
                FSharp.Compiler.CompilerDiagnostics.showAssertForUnexpectedException := false
#endif
                if (FileSystem.FileExistsShim("watson-test.fs")) then
                    FileSystem.FileDeleteShim("watson-test.fs")
                FileSystem.OpenFileForWriteShim("watson-test.fs").Write("// Hello watson" )
                let argv =
                    [|  "--simulateException:"+simulationCode
                        "--nowarn:988" // don't show `watson-test.fs(1,16): warning FS0988: Main module of program is empty: nothing will happen when it is run`
                        "watson-test.fs"
                    |]

                let ctok = AssumeCompilationThreadWithoutEvidence ()
                let _code = CompileFromCommandLineArguments (ctok, argv, LegacyMSBuildReferenceResolver.getResolver(), false, ReduceMemoryFlag.No, CopyFSharpCoreFlag.No, FSharp.Compiler.DiagnosticsLogger.QuitProcessExiter, ConsoleLoggerProvider(), None, None)
                ()
            with 
            | :? 'TException as e -> 
                let msg = e.ToString();
                if msg.Contains("ReportTime") || msg.Contains("CheckOneInput") then ()
                else
                    printfn "%s" msg
                    Assert.Fail("The correct callstack was not reported to watson.")
            | (FSharp.Compiler.DiagnosticsLogger.ReportedError (Some (FSharp.Compiler.DiagnosticsLogger.InternalError (msg, range) as e)))
            | (FSharp.Compiler.DiagnosticsLogger.InternalError (msg, range) as e) -> 
                printfn "InternalError Exception: %s, range = %A, stack = %s" msg range (e.ToString())
                Assert.Fail("An InternalError exception occurred.")
        finally               
#if DEBUG
            FSharp.Compiler.CompilerDiagnostics.showAssertForUnexpectedException := true 
#endif
        FileSystem.FileDeleteShim("watson-test.fs")


module WatsonTests = 

    [<Fact>]
    let FscOutOfMemory() = Check.FscLevelException<System.OutOfMemoryException>("fsc-oom")

    [<Fact>]
    let FscArgumentNull() = Check.FscLevelException<System.ArgumentNullException>("fsc-an")        
    
    [<Fact>]
    let FscInvalidOperation() = Check.FscLevelException<System.InvalidOperationException>("fsc-invop")        

// As of .NET 4.0 some exception types cannot be caught. As a result, we cannot test this case. I did visually confirm a Watson report is sent, though.
//    [<Fact>]
//    let FscAccessViolation() = Check.FscLevelException<System.AccessViolationException>("fsc-ac")        

    [<Fact>]
    let FscArgumentOutOfRange() = Check.FscLevelException<System.ArgumentOutOfRangeException>("fsc-aor")        

    [<Fact>]
    let FscDivideByZero() = Check.FscLevelException<System.DivideByZeroException>("fsc-dv0")        

    [<Fact>]
    let FscNotFiniteNumber() = Check.FscLevelException<System.NotFiniteNumberException>("fsc-nfn")        

    [<Fact>]
    let FscOverflow() = Check.FscLevelException<System.OverflowException>("fsc-oe")        

    [<Fact>]
    let FscArrayTypeMismatch() = Check.FscLevelException<System.ArrayTypeMismatchException>("fsc-atmm")        

    [<Fact>]
    let FscBadImageFormat() = Check.FscLevelException<System.BadImageFormatException>("fsc-bif")        

    [<Fact>]
    let FscKeyNotFound() = Check.FscLevelException<System.Collections.Generic.KeyNotFoundException>("fsc-knf")        

    [<Fact>]
    let FscIndexOutOfRange() = Check.FscLevelException<System.IndexOutOfRangeException>("fsc-ior")        

    [<Fact>]
    let FscInvalidCast() = Check.FscLevelException<System.InvalidCastException>("fsc-ic")        

    [<Fact>]
    let FscInvalidProgram() = Check.FscLevelException<System.InvalidProgramException>("fsc-ip")        

    [<Fact>]
    let FscMemberAccess() = Check.FscLevelException<System.MemberAccessException>("fsc-ma")        

    [<Fact>]
    let FscNotImplemented() = Check.FscLevelException<System.NotImplementedException>("fsc-ni")        

    [<Fact>]
    let FscNullReference() = Check.FscLevelException<System.NullReferenceException>("fsc-nr")        

    [<Fact>]
    let FscOperationCancelled() = Check.FscLevelException<System.OperationCanceledException>("fsc-oc")        
    
    //[<Fact>]
    //let FscFailure() = Check.FscLevelException<Microsoft.FSharp.Core.FailureException>("fsc-fail")            
      
    [<Fact>]
    let TypeCheckOutOfMemory() = Check.FscLevelException<System.OutOfMemoryException>("tc-oom")

    [<Fact>]
    let TypeCheckArgumentNull() = Check.FscLevelException<System.ArgumentNullException>("tc-an")        
    
    [<Fact>]
    let TypeCheckInvalidOperation() = Check.FscLevelException<System.InvalidOperationException>("tc-invop")        

// As of .NET 4.0 some exception types cannot be caught. As a result, we cannot test this case. I did visually confirm a Watson report is sent, though.
//    [<Fact>]
//    let TypeCheckAccessViolation() = Check.FscLevelException<System.AccessViolationException>("tc-ac")        

    [<Fact>]
    let TypeCheckArgumentOutOfRange() = Check.FscLevelException<System.ArgumentOutOfRangeException>("tc-aor")        

    [<Fact>]
    let TypeCheckDivideByZero() = Check.FscLevelException<System.DivideByZeroException>("tc-dv0")        

    [<Fact>]
    let TypeCheckNotFiniteNumber() = Check.FscLevelException<System.NotFiniteNumberException>("tc-nfn")        

    [<Fact>]
    let TypeCheckOverflow() = Check.FscLevelException<System.OverflowException>("tc-oe")        

    [<Fact>]
    let TypeCheckArrayTypeMismatch() = Check.FscLevelException<System.ArrayTypeMismatchException>("tc-atmm")        

    [<Fact>]
    let TypeCheckBadImageFormat() = Check.FscLevelException<System.BadImageFormatException>("tc-bif")        

    [<Fact>]
    let TypeCheckKeyNotFound() = Check.FscLevelException<System.Collections.Generic.KeyNotFoundException>("tc-knf")        

    [<Fact>]
    let TypeCheckIndexOutOfRange() = Check.FscLevelException<System.IndexOutOfRangeException>("tc-ior")        

    [<Fact>]
    let TypeCheckInvalidCast() = Check.FscLevelException<System.InvalidCastException>("tc-ic")        

    [<Fact>]
    let TypeCheckInvalidProgram() = Check.FscLevelException<System.InvalidProgramException>("tc-ip")        

    [<Fact>]
    let TypeCheckMemberAccess() = Check.FscLevelException<System.MemberAccessException>("tc-ma")        

    [<Fact>]
    let TypeCheckNotImplemented() = Check.FscLevelException<System.NotImplementedException>("tc-ni")        

    [<Fact>]
    let TypeCheckNullReference() = Check.FscLevelException<System.NullReferenceException>("tc-nr")        

    [<Fact>]
    let TypeCheckOperationCancelled() = Check.FscLevelException<System.OperationCanceledException>("tc-oc")        

    [<Fact>]
    let TypeCheckFailure() = Check.FscLevelException<System.Exception>("tc-fail")            

