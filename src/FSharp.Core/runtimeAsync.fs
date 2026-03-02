// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Runtime-async attribute for library extensibility.
// RuntimeAsyncAttribute is available on all TFMs but is only meaningful on .NET 10.0+.
namespace Microsoft.FSharp.Control

open System

/// Marker attribute that the F# compiler reads to enable runtime-async semantics on a type or module.
/// The compiler uses this attribute for three purposes:
/// (1) Gating <c>cil managed async</c> IL flag emission — only methods whose enclosing type or module
///     carries <c>[&lt;RuntimeAsync&gt;]</c> receive the async IL flag (0x2000), whether via explicit
///     <c>[&lt;MethodImpl(0x2000)&gt;]</c> or via body analysis.
/// (2) Implicit <c>NoDynamicInvocation</c> — public members of a <c>[&lt;RuntimeAsync&gt;]</c>-marked class
///     automatically have their bodies replaced with a <c>raise (NotSupportedException ...)</c> in
///     compiled (non-inline) form, preventing unsafe dynamic invocation.
/// (3) Optimizer anti-inlining — the F# optimizer does not cross-module inline functions from
///     <c>[&lt;RuntimeAsync&gt;]</c>-marked types, preserving the <c>cil managed async</c> contract.
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false)>]
type RuntimeAsyncAttribute() =
    inherit Attribute()
