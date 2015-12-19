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
                Microsoft.FSharp.Compiler.CompileOps.FullCompiler.showAssertForUnexpectedException := false
#endif
                if (File.Exists("watson-test.fs")) then
                    File.Delete("watson-test.fs")
                File.WriteAllText("watson-test.fs", "// Hello watson" )
                let _code = Microsoft.FSharp.Compiler.CommandLineMain.main([| "--simulateException:"+simulationCode; "watson-test.fs"|]) 
                ()
            with 
            | :? 'TException as e -> 
                let msg = e.ToString();
                if msg.Contains("ReportTime") || msg.Contains("TypeCheckOneInput") then ()
                else
                    printfn "%s" msg
                    Assert.Fail("The correct callstack was not reported to watson.")
        finally               
#if DEBUG
            Microsoft.FSharp.Compiler.CompileOps.FullCompiler.showAssertForUnexpectedException := true 
#endif
        File.Delete("watson-test.fs")


[<Parallelizable(ParallelScope.Self)>][<TestFixture>] 
type Watson() = 

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscOutOfMemory() = Check.FscLevelException<System.OutOfMemoryException>("fsc-oom")

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscArgumentNull() = Check.FscLevelException<System.ArgumentNullException>("fsc-an")        
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscInvalidOperation() = Check.FscLevelException<System.InvalidOperationException>("fsc-invop")        

// As of .NET 4.0 some exception types cannot be caught. As a result, we cannot test this case. I did visually confirm a Watson report is sent, though.
//    [<Parallelizable(ParallelScope.Self)>][<Test>]
//    member public w.FscAccessViolation() = Check.FscLevelException<System.AccessViolationException>("fsc-ac")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscArgumentOutOfRange() = Check.FscLevelException<System.ArgumentOutOfRangeException>("fsc-aor")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscDivideByZero() = Check.FscLevelException<System.DivideByZeroException>("fsc-dv0")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscNotFiniteNumber() = Check.FscLevelException<System.NotFiniteNumberException>("fsc-nfn")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscOverflow() = Check.FscLevelException<System.OverflowException>("fsc-oe")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscArrayTypeMismatch() = Check.FscLevelException<System.ArrayTypeMismatchException>("fsc-atmm")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscBadImageFormat() = Check.FscLevelException<System.BadImageFormatException>("fsc-bif")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscKeyNotFound() = Check.FscLevelException<System.Collections.Generic.KeyNotFoundException>("fsc-knf")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscIndexOutOfRange() = Check.FscLevelException<System.IndexOutOfRangeException>("fsc-ior")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscInvalidCast() = Check.FscLevelException<System.InvalidCastException>("fsc-ic")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscInvalidProgram() = Check.FscLevelException<System.InvalidProgramException>("fsc-ip")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscMemberAccess() = Check.FscLevelException<System.MemberAccessException>("fsc-ma")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscNotImplemented() = Check.FscLevelException<System.NotImplementedException>("fsc-ni")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscNullReference() = Check.FscLevelException<System.NullReferenceException>("fsc-nr")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.FscOperationCancelled() = Check.FscLevelException<System.OperationCanceledException>("fsc-oc")        
    
    //[<Parallelizable(ParallelScope.Self)>][<Test>]
    //member public w.FscFailure() = Check.FscLevelException<Microsoft.FSharp.Core.FailureException>("fsc-fail")            
      
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckOutOfMemory() = Check.FscLevelException<System.OutOfMemoryException>("tc-oom")

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckArgumentNull() = Check.FscLevelException<System.ArgumentNullException>("tc-an")        
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckInvalidOperation() = Check.FscLevelException<System.InvalidOperationException>("tc-invop")        

// As of .NET 4.0 some exception types cannot be caught. As a result, we cannot test this case. I did visually confirm a Watson report is sent, though.
//    [<Parallelizable(ParallelScope.Self)>][<Test>]
//    member public w.TypeCheckAccessViolation() = Check.FscLevelException<System.AccessViolationException>("tc-ac")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckArgumentOutOfRange() = Check.FscLevelException<System.ArgumentOutOfRangeException>("tc-aor")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckDivideByZero() = Check.FscLevelException<System.DivideByZeroException>("tc-dv0")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckNotFiniteNumber() = Check.FscLevelException<System.NotFiniteNumberException>("tc-nfn")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckOverflow() = Check.FscLevelException<System.OverflowException>("tc-oe")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckArrayTypeMismatch() = Check.FscLevelException<System.ArrayTypeMismatchException>("tc-atmm")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckBadImageFormat() = Check.FscLevelException<System.BadImageFormatException>("tc-bif")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckKeyNotFound() = Check.FscLevelException<System.Collections.Generic.KeyNotFoundException>("tc-knf")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckIndexOutOfRange() = Check.FscLevelException<System.IndexOutOfRangeException>("tc-ior")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckInvalidCast() = Check.FscLevelException<System.InvalidCastException>("tc-ic")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckInvalidProgram() = Check.FscLevelException<System.InvalidProgramException>("tc-ip")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckMemberAccess() = Check.FscLevelException<System.MemberAccessException>("tc-ma")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckNotImplemented() = Check.FscLevelException<System.NotImplementedException>("tc-ni")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckNullReference() = Check.FscLevelException<System.NullReferenceException>("tc-nr")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckOperationCancelled() = Check.FscLevelException<System.OperationCanceledException>("tc-oc")        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public w.TypeCheckFailure() = Check.FscLevelException<System.Exception>("tc-fail")            

