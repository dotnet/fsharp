// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Runtime-async attribute for library extensibility.
// RuntimeAsyncAttribute is available on all TFMs but is only meaningful on .NET 10.0+.
namespace Microsoft.FSharp.Control

open System

/// Marker attribute reserved for future library extensibility with runtime-async semantics.
/// Note: the compiler does not read this attribute to propagate the async IL flag (0x2000).
/// The async flag is propagated via detection of AsyncHelpers.Await call sites in the method body.
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false)>]
type RuntimeAsyncAttribute() =
    inherit Attribute()
