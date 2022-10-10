// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System

/// For activities following the dotnet distributed tracing concept
/// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts?source=recommendations
[<RequireQualifiedAccess>]
module internal Activity =

    val startNoTags: name: string -> IDisposable

    val start: name: string -> tags: (string * string) seq -> IDisposable
