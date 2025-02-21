// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Diagnostics

open System
open Internal.Utilities.Library

/// For activities following the dotnet distributed tracing concept
/// https://learn.microsoft.com/dotnet/core/diagnostics/distributed-tracing-concepts?source=recommendations
[<RequireQualifiedAccess>]
module ActivityNames =
    [<Literal>]
    val FscSourceName: string = "fsc"

    [<Literal>]
    val ProfiledSourceName: string = "fsc_with_env_stats"

    val AllRelevantNames: string[]

/// For activities following the dotnet distributed tracing concept
/// https://learn.microsoft.com/dotnet/core/diagnostics/distributed-tracing-concepts?source=recommendations
[<RequireQualifiedAccess>]
module internal Activity =

    module Tags =
        val fileName: string
        val qualifiedNameOfFile: string
        val project: string
        val userOpName: string
        val length: string
        val cache: string
        val buildPhase: string
        val version: string
        val stackGuardName: string
        val stackGuardCurrentDepth: string
        val stackGuardMaxDepth: string
        val callerMemberName: string
        val callerFilePath: string
        val callerLineNumber: string

    module Events =
        val cacheHit: string

    val startNoTags: name: string -> IDisposable MaybeNull

    val start: name: string -> tags: (string * string) seq -> IDisposable MaybeNull

    val addEvent: name: string -> unit

    val addEventWithTags: name: string -> tags: (string * objnull) seq -> unit

    module Profiling =
        val startAndMeasureEnvironmentStats: name: string -> IDisposable MaybeNull
        val addConsoleListener: unit -> IDisposable

    module CsvExport =
        val addCsvFileListener: pathToFile: string -> IDisposable
