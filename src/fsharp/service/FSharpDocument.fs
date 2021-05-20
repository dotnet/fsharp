// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.IO.MemoryMappedFiles
open System.Xml
open System.Runtime.InteropServices
open System.Threading
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.CreateILModule
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

[<RequireQualifiedAccess>]
type FSharpDocumentText =
    | Stream of Stream
    | SourceText of ISourceText

    interface IDisposable with

        member this.Dispose() =
            match this with
            | Stream stream -> stream.Dispose()
            | _ -> ()

[<AbstractClass>]
type FSharpDocument internal () =

    abstract FilePath : string

    abstract TimeStamp : DateTime

    abstract GetText : unit -> FSharpDocumentText

type private FSharpDocumentMemoryMappedFile(filePath: string, timeStamp: DateTime, mmf: MemoryMappedFile) =
    inherit FSharpDocument()

    override _.FilePath = filePath

    override _.TimeStamp = timeStamp

    override _.GetText() =
        FSharpDocumentText.Stream(mmf.CreateViewStream() :> Stream)

type private FSharpDocumentByteArray(filePath: string, timeStamp: DateTime, bytes: byte[]) =
    inherit FSharpDocument()

    override _.FilePath = filePath

    override _.TimeStamp = timeStamp

    override _.GetText() =
        FSharpDocumentText.Stream(new MemoryStream(bytes, 0, bytes.Length, false) :> Stream)

type private FSharpDocumentFromFile(filePath: string) =
    inherit FSharpDocument()

    override _.FilePath = filePath

    override _.TimeStamp = FileSystem.GetLastWriteTimeShim(filePath)

    override _.GetText() = FSharpDocumentText.Stream(File.OpenRead(filePath) :> Stream)

type private FSharpDocumentCustom(filePath: string, getTimeStamp, getSourceText) =
    inherit FSharpDocument()

    override _.FilePath = filePath

    override _.TimeStamp = getTimeStamp()

    override _.GetText() =
        FSharpDocumentText.SourceText(getSourceText())

type FSharpDocument with

    static member Create(filePath, getTimeStamp, getSourceText) =
        FSharpDocumentCustom(filePath, getTimeStamp, getSourceText) :> FSharpDocument

    static member CreateFromFile(filePath: string) =
        FSharpDocumentFromFile(filePath) :> FSharpDocument

    static member CreateCopyFromFile(filePath: string) =
        let fileMode = FileMode.Open
        let fileAccess = FileAccess.Read
        let fileShare = FileShare.Delete ||| FileShare.ReadWrite

        let timeStamp = FileSystem.GetLastWriteTimeShim(filePath)

        // We want to use mmaped files only when:
        //   -  Opening large binary files (no need to use for source or resource files really)
        //   -  Running on mono, since its MemoryMappedFile implementation throws when "mapName" is not provided (is null).
        //      (See: https://github.com/mono/mono/issues/10245)
        if runningOnMono then
            let bytes = File.ReadAllBytes filePath
            FSharpDocumentByteArray(filePath, timeStamp, bytes) :> FSharpDocument
        else
            let fileStream = new FileStream(filePath, fileMode, fileAccess, fileShare)
            let length = fileStream.Length
            let mmf =
                MemoryMappedFile.CreateNew(
                    null,
                    length,
                    MemoryMappedFileAccess.Read,
                    MemoryMappedFileOptions.None,
                    HandleInheritability.None)
            use stream = mmf.CreateViewStream(0L, length, MemoryMappedFileAccess.Read)
            fileStream.CopyTo(stream)
            fileStream.Dispose()
            FSharpDocumentMemoryMappedFile(filePath, timeStamp, mmf) :> FSharpDocument
            
