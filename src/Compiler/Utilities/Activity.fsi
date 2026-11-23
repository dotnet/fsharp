// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics.Metrics

/// For activities following the dotnet distributed tracing concept
/// https://learn.microsoft.com/dotnet/core/diagnostics/distributed-tracing-concepts?source=recommendations
[<RequireQualifiedAccess>]
module ActivityNames =
    [<Literal>]
    val FscSourceName: string = "fsc"

    [<Literal>]
    val ProfiledSourceName: string = "fsc_with_env_stats"

    val AllRelevantNames: string[]

module internal Metrics =
    val Meter: Meter

    val printTable: headers: string list -> rows: string list list -> string

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

    val startNoTags: name: string -> System.IDisposable | null

    val start: name: string -> tags: (string * string) seq -> System.IDisposable | null

    val addEvent: name: string -> unit

    val addEventWithTags: name: string -> tags: (string * objnull) seq -> unit

    module Profiling =
        val startAndMeasureEnvironmentStats: name: string -> System.IDisposable | null
        val addConsoleListener: unit -> IDisposable

        /// A sequence of profiled phases, where starting a phase disposes the one before it
        /// and disposing ends the last. Not thread-safe.
        [<Sealed>]
        type PhaseTimer =
            new: unit -> PhaseTimer

            /// <summary>Ends the current phase (if any) and starts one named <paramref name="name"/>.</summary>
            member Next: name: string -> unit

            interface IDisposable

    module CsvExport =
        val addCsvFileListener: pathToFile: string -> IDisposable
