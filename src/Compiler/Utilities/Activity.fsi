// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System

/// For activities following the dotnet distributed tracing concept
/// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts?source=recommendations
[<RequireQualifiedAccess>]
module internal Activity =

    module Tags =
        val fileName: string
        val qualifiedNameOfFile: string
        val project: string
        val userOpName: string
        val length: string
        val cache: string
        val cpuDelta: string
        val realDelta: string
        val gc0: string
        val gc1: string
        val gc2: string
        val outputDllFile: string

        val AllKnownTags: string[]

    val startNoTags: name: string -> IDisposable

    val start: name: string -> tags: (string * string) seq -> IDisposable

    val addCsvFileListener: pathToFile: string -> IDisposable
