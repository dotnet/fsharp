// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Runtime-async attribute for library extensibility.
// RuntimeAsyncAttribute is available on all TFMs but is only meaningful on .NET 10.0+.
namespace Microsoft.FSharp.Control

open System

/// Attribute applied to computation expression builder types to indicate they use
/// runtime-async semantics. Methods using such builders will have the async IL flag (0x2000) emitted.
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false)>]
type RuntimeAsyncAttribute() =
    inherit Attribute()
