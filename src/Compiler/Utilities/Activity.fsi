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

    val startNoTags: name: string -> IDisposable

    val start: name: string -> tags: (string * string) seq -> IDisposable

    module Profiling =
        val startAndMeasureEnvironmentStats: name: string -> IDisposable
        val addConsoleListener: unit -> IDisposable

    module CsvExport =
        val addCsvFileListener: pathToFile: string -> IDisposable
