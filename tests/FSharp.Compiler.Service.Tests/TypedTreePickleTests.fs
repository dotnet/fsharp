module FSharp.Compiler.Service.Tests.TypedTreePickleTests

open System.IO

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Driver
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreePickle
open FSharp.Compiler.Syntax

open Internal.Utilities.Library

open Xunit

type TypedTreePickleTests() =

    /// random auxiliary stuff

    let builder = TcConfigBuilder.CreateNew(
        SimulatedMSBuildReferenceResolver.getResolver(),
        Directory.GetCurrentDirectory(),
        ReduceMemoryFlag.No,
        "",
        false,
        false,
        CopyFSharpCoreFlag.No,
        (fun _ -> None),
        None,
        range0,
        compressMetadata = false)

    let tcConfig = TcConfig.Create(builder, false)

    let sysRes, otherRes, knownUnresolved =
        TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)

    let foundationalTcConfigP = TcConfigProvider.Constant tcConfig
    let tcGlobals, frameworkTcImports = 
        TcImports.BuildFrameworkTcImports(
            foundationalTcConfigP,
            sysRes,
            otherRes) 
        |> Async.RunImmediate

    let tcImports =
        TcImports.BuildNonFrameworkTcImports(
            foundationalTcConfigP,
            frameworkTcImports, 
            otherRes, 
            knownUnresolved, 
            new DependencyProvider())
        |> Async.RunImmediate

    let tcEnv0, openDecls0 = ParseAndCheckInputs.GetInitialTcEnv(
        "test",
        rangeStartup,
        tcConfig, 
        tcImports,
        tcGlobals)

    /// helper functions

    let prepareData sourceFiles =
        let inputs =
            ParseAndCheckInputs.ParseInputFiles(
                tcConfig, 
                Lexhelp.LexResourceManager(), 
                sourceFiles, 
                DiagnosticsLogger.AssertFalseDiagnosticsLogger, 
                false) |> List.map fst

        let tcState, topAttribs, implFiles, _ = 
            TypeCheck(
                CompilationThreadToken(),
                tcConfig,
                tcImports,
                tcGlobals,
                DiagnosticsLogger.AssertFalseDiagnosticsLogger,
                "test",
                tcEnv0,
                openDecls0,
                inputs,
                DiagnosticsLogger.QuitProcessExiter,
                "")

        let tcInfo : PickledTcInfo = {
            MainMethodAttrs = topAttribs.mainMethodAttrs
            NetModuleAttrs = topAttribs.netModuleAttrs
            AssemblyAttrs = topAttribs.assemblyAttrs
            DeclaredImpls = implFiles
        }

        tcState.Ccu, tcInfo


    [<Fact>]
    let PickleAndUnpickleTcData() =
        // prepare stuff

        let fileName1 = "test1.fs"
        let source1 = """
module BlahModule1 =

    let blahFunction1() = 1

System.Console.WriteLine(1)
"""
        File.WriteAllText(fileName1, source1)

        let fileName2 = "test1.fs"
        let source2 = """
module BlahModule2 =

    let blahFunction2() = 2

System.Console.WriteLine(2)
"""
        File.WriteAllText(fileName2, source2)

        let ccuThunk, tcInfo = prepareData [ fileName1; fileName2 ]

        // convert back and forth

        let encodedTcData = EncodeTypecheckingData(tcConfig, tcGlobals, ccuThunk, "", false, tcInfo)
        let memoryReader () = encodedTcData.Head.GetBytes()
        let decodedTcData = GetTypecheckingData("", ILScopeRef.Local, None, memoryReader, None)

        // compare data

        let originalTcInfo = tcInfo
        let restoredTcInfo = decodedTcData.RawData

        // won't work, part of the data has no equality constraint
        // Assert.Equal(originalTcInfo, restoredTcInfo)

        Assert.Equal<Attribs>(originalTcInfo.MainMethodAttrs, restoredTcInfo.MainMethodAttrs)
        Assert.Equal<Attribs>(originalTcInfo.NetModuleAttrs, restoredTcInfo.NetModuleAttrs)
        Assert.Equal<Attribs>(originalTcInfo.AssemblyAttrs, restoredTcInfo.AssemblyAttrs)

        // won't work, part of the data has no equality constraint
        //Assert.Equal<CheckedImplFile list>(originalTcInfo.DeclaredImpls, restoredTcInfo.DeclaredImpls)

        let originalFiles = originalTcInfo.DeclaredImpls
        let restoredFiles = restoredTcInfo.DeclaredImpls
        Assert.Equal(originalFiles.Length, restoredFiles.Length)

        for i = 0 to originalFiles.Length - 1 do
            let originalFile = originalFiles[i]
            let restoredFile = restoredFiles[i]

            // doesn't work, need to figure out namespace equivalenc
            // Assert.Equal(originalFile.Signature, restoredFile.Signature)

            // doesn't work, no equality for QualifiedNameOfFile
            // Assert.Equal(originalFile.QualifiedNameOfFile, restoredFile.QualifiedNameOfFile)

            Assert.Equal<ScopedPragma list>(originalFile.Pragmas, restoredFile.Pragmas)
            Assert.Equal(originalFile.IsScript, restoredFile.IsScript)
            Assert.Equal(originalFile.HasExplicitEntryPoint, restoredFile.HasExplicitEntryPoint)
