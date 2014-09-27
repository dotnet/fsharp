// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests

(*
    Is NUNIT just disappearing when you run one of these tests?
    There are paths in the compiler that call Process.Exit, if we manage to hit one of these paths then the process (nunit-gui.exe)
    will be exited:
    
    Put a break point on:
    
        System.Environment.Exit(n)  
*)    

#nowarn "52" // The value has been copied to ensure the original is not mutated

open NUnit.Framework
open System
open System.Text.RegularExpressions 
open System.Diagnostics
open System.Collections.Generic
open System.IO
open System.Reflection

type Check = 
    static member public FscLevelException<'TException when 'TException :> exn>(simulationCode)  =
        try 
            try
#if DEBUG
                Microsoft.FSharp.Compiler.Build.FullCompiler.showAssertForUnexpectedException := false
#endif
                if (File.Exists("watson-test.fs")) then
                    File.Delete("watson-test.fs")
                File.WriteAllText("watson-test.fs", "// Hello watson" )
                let _code = Microsoft.FSharp.Compiler.CommandLineMain.main([| "--simulateException:"+simulationCode; "watson-test.fs"|]) 
                ()
            with 
            | :? 'TException as e -> 
                let msg = e.ToString();
                if msg.Contains("ReportTime") || msg.Contains("TypecheckOneInput") then ()
                else
                    printfn "%s" msg
                    Assert.Fail("The correct callstack was not reported to watson.")
        finally               
#if DEBUG
            Microsoft.FSharp.Compiler.Build.FullCompiler.showAssertForUnexpectedException := true
#endif
        File.Delete("watson-test.fs")


[<TestFixture>] 
type Watson() = 

    [<Test>]
    member public w.FscOutOfMemory() = Check.FscLevelException<System.OutOfMemoryException>("fsc-oom")

    [<Test>]
    member public w.FscArgumentNull() = Check.FscLevelException<System.ArgumentNullException>("fsc-an")        
    
    [<Test>]
    member public w.FscInvalidOperation() = Check.FscLevelException<System.InvalidOperationException>("fsc-invop")        

// As of .NET 4.0 some exception types cannot be caught. As a result, we cannot test this case. I did visually confirm a Watson report is sent, though.
//    [<Test>]
//    member public w.FscAccessViolation() = Check.FscLevelException<System.AccessViolationException>("fsc-ac")        

    [<Test>]
    member public w.FscArgumentOutOfRange() = Check.FscLevelException<System.ArgumentOutOfRangeException>("fsc-aor")        

    [<Test>]
    member public w.FscDivideByZero() = Check.FscLevelException<System.DivideByZeroException>("fsc-dv0")        

    [<Test>]
    member public w.FscNotFiniteNumber() = Check.FscLevelException<System.NotFiniteNumberException>("fsc-nfn")        

    [<Test>]
    member public w.FscOverflow() = Check.FscLevelException<System.OverflowException>("fsc-oe")        

    [<Test>]
    member public w.FscArrayTypeMismatch() = Check.FscLevelException<System.ArrayTypeMismatchException>("fsc-atmm")        

    [<Test>]
    member public w.FscBadImageFormat() = Check.FscLevelException<System.BadImageFormatException>("fsc-bif")        

    [<Test>]
    member public w.FscKeyNotFound() = Check.FscLevelException<System.Collections.Generic.KeyNotFoundException>("fsc-knf")        

    [<Test>]
    member public w.FscIndexOutOfRange() = Check.FscLevelException<System.IndexOutOfRangeException>("fsc-ior")        

    [<Test>]
    member public w.FscInvalidCast() = Check.FscLevelException<System.InvalidCastException>("fsc-ic")        

    [<Test>]
    member public w.FscInvalidProgram() = Check.FscLevelException<System.InvalidProgramException>("fsc-ip")        

    [<Test>]
    member public w.FscMemberAccess() = Check.FscLevelException<System.MemberAccessException>("fsc-ma")        

    [<Test>]
    member public w.FscNotImplemented() = Check.FscLevelException<System.NotImplementedException>("fsc-ni")        

    [<Test>]
    member public w.FscNullReference() = Check.FscLevelException<System.NullReferenceException>("fsc-nr")        

    [<Test>]
    member public w.FscOperationCancelled() = Check.FscLevelException<System.OperationCanceledException>("fsc-oc")        
    
    //[<Test>]
    //member public w.FscFailure() = Check.FscLevelException<Microsoft.FSharp.Core.FailureException>("fsc-fail")            
      
    [<Test>]
    member public w.TypeCheckOutOfMemory() = Check.FscLevelException<System.OutOfMemoryException>("tc-oom")

    [<Test>]
    member public w.TypeCheckArgumentNull() = Check.FscLevelException<System.ArgumentNullException>("tc-an")        
    
    [<Test>]
    member public w.TypeCheckInvalidOperation() = Check.FscLevelException<System.InvalidOperationException>("tc-invop")        

// As of .NET 4.0 some exception types cannot be caught. As a result, we cannot test this case. I did visually confirm a Watson report is sent, though.
//    [<Test>]
//    member public w.TypeCheckAccessViolation() = Check.FscLevelException<System.AccessViolationException>("tc-ac")        

    [<Test>]
    member public w.TypeCheckArgumentOutOfRange() = Check.FscLevelException<System.ArgumentOutOfRangeException>("tc-aor")        

    [<Test>]
    member public w.TypeCheckDivideByZero() = Check.FscLevelException<System.DivideByZeroException>("tc-dv0")        

    [<Test>]
    member public w.TypeCheckNotFiniteNumber() = Check.FscLevelException<System.NotFiniteNumberException>("tc-nfn")        

    [<Test>]
    member public w.TypeCheckOverflow() = Check.FscLevelException<System.OverflowException>("tc-oe")        

    [<Test>]
    member public w.TypeCheckArrayTypeMismatch() = Check.FscLevelException<System.ArrayTypeMismatchException>("tc-atmm")        

    [<Test>]
    member public w.TypeCheckBadImageFormat() = Check.FscLevelException<System.BadImageFormatException>("tc-bif")        

    [<Test>]
    member public w.TypeCheckKeyNotFound() = Check.FscLevelException<System.Collections.Generic.KeyNotFoundException>("tc-knf")        

    [<Test>]
    member public w.TypeCheckIndexOutOfRange() = Check.FscLevelException<System.IndexOutOfRangeException>("tc-ior")        

    [<Test>]
    member public w.TypeCheckInvalidCast() = Check.FscLevelException<System.InvalidCastException>("tc-ic")        

    [<Test>]
    member public w.TypeCheckInvalidProgram() = Check.FscLevelException<System.InvalidProgramException>("tc-ip")        

    [<Test>]
    member public w.TypeCheckMemberAccess() = Check.FscLevelException<System.MemberAccessException>("tc-ma")        

    [<Test>]
    member public w.TypeCheckNotImplemented() = Check.FscLevelException<System.NotImplementedException>("tc-ni")        

    [<Test>]
    member public w.TypeCheckNullReference() = Check.FscLevelException<System.NullReferenceException>("tc-nr")        

    [<Test>]
    member public w.TypeCheckOperationCancelled() = Check.FscLevelException<System.OperationCanceledException>("tc-oc")        

    [<Test>]
    member public w.TypeCheckFailure() = Check.FscLevelException<System.Exception>("tc-fail")            

