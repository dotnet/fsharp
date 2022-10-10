// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics

/// For activities following the dotnet distributed tracing concept
/// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts?source=recommendations
[<RequireQualifiedAccess>]
module internal Activity =

    val StartNoTags: name: string -> IDisposable

    val Start: name: string -> tags: (string * #obj) seq -> IDisposable
