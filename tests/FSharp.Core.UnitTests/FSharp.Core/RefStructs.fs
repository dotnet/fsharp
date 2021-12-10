// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace System.Runtime.CompilerServices

#if NETCOREAPP
open System

[<AttributeUsage(AttributeTargets.Struct)>]
type IsByRefLikeAttribute() = inherit Attribute()
#endif

namespace FSharp.Core.UnitTests

#if NETCOREAPP
open System
open System.Runtime.CompilerServices
open Xunit

[<Struct; IsByRefLike>]
type SpanWrapper(span: Span<int>) =
    member _.Span = span

type CustomIsByRefLikeAttributeTests() =
    [<Fact>]
    member _.TestSpanWrapper() =
        let array = Array.init 5 id
        let span = array.AsSpan()
        let spanWrapper = SpanWrapper(span)
        Assert.True(span.SequenceEqual(Span<_>.op_Implicit spanWrapper.Span))
        Assert.Equal<int>(array, spanWrapper.Span.ToArray())
        ()
#endif
