module FSharp.Core.UnitTests.Interop.CSharp.CollectionExpressions

// These tests require types available only in netstandard2.1 and up.
#if NET8_0_OR_GREATER

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

/// Usings for the C# tests.
/// These must be prepended to each test's C# source text.
let csUsings =
    """
    using System;
    using System.Linq;
    using Microsoft.FSharp.Collections;

    #nullable enable
    """

/// Utility types and functions for the C# tests.
/// These must be appended to each test's C# source text.
let csUtils =
    """
    public sealed record RecordClass(int X) : IComparable
    {
        public int CompareTo(object? obj) => obj switch
        {
            null => 1,
            RecordClass(var otherX) => X.CompareTo(otherX),
            _ => throw new ArgumentException("Invalid comparison.", nameof(obj))
        };
    }

    public readonly record struct RecordStruct(int X) : IComparable
    {
        public int CompareTo(object? obj) => obj switch
        {
            null => 1,
            RecordStruct(var otherX) => X.CompareTo(otherX),
            _ => throw new ArgumentException("Invalid comparison.", nameof(obj))
        };
    }

    public sealed class EqualException(string message) : Exception(message) { }

    public sealed class TrueException(string message) : Exception(message) { }

    public static class Assert
    {
        public static void Equal<T>(T expected, T actual)
        {
            switch ((expected, actual))
            {
                case (null, null): return;
                case (null, not null):
                case (not null, null):
                case var _ when !expected.Equals(actual): throw new EqualException($"Expected '{expected}' but got '{actual}'.");
            }
        }

        public static void True(bool b)
        {
            if (!b)
            {
                throw new TrueException("Expected true but got false.");
            }
        }
    }
    """

[<Fact>]
let ``FSharpList<int>: can create using C# collection expression`` () =
    CSharp $$"""
    {{csUsings}}

    var expected = ListModule.OfArray([1, 2, 3]);
    FSharpList<int> actual = [1, 2, 3];

    Assert.Equal(expected, actual);
    Assert.Equal(expected.Length, actual.Length);

    for (var i = 0; i < expected.Length; i++)
    {
        Assert.Equal(expected[i], actual[i]);
    }

    Assert.True(actual is [1, 2, 3]);

    {{csUtils}}
    """
    |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
    |> withName "Test"
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``FSharpList<RecordClass>: can create using C# collection expression`` () =
    CSharp $$"""
    {{csUsings}}

    var expected = ListModule.OfArray([new RecordClass(1), new RecordClass(2), new RecordClass(3)]);
    FSharpList<RecordClass> actual = [new RecordClass(1), new RecordClass(2), new RecordClass(3)];

    Assert.Equal(expected, actual);
    Assert.Equal(expected.Length, actual.Length);

    for (var i = 0; i < expected.Length; i++)
    {
        Assert.Equal(expected[i], actual[i]);
    }

    Assert.True(actual is [RecordClass(1), RecordClass(2), RecordClass(3)]);

    {{csUtils}}
    """
    |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
    |> withName "Test"
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``FSharpList<RecordStruct>: can create using C# collection expression`` () =
    CSharp $$"""
    {{csUsings}}

    var expected = ListModule.OfArray([new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)]);
    FSharpList<RecordStruct> actual = [new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)];

    Assert.Equal(expected, actual);
    Assert.Equal(expected.Length, actual.Length);

    for (var i = 0; i < expected.Length; i++)
    {
        Assert.Equal(expected[i], actual[i]);
    }

    Assert.True(actual is [RecordStruct(1), RecordStruct(2), RecordStruct(3)]);

    {{csUtils}}
    """
    |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
    |> withName "Test"
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``FSharpSet<int>: can create using C# collection expression`` () =
    CSharp $$"""
    {{csUsings}}

    var expected = SetModule.OfArray([1, 2, 3]);
    FSharpSet<int> actual = [1, 2, 3];

    Assert.Equal(expected, actual);
    Assert.Equal(expected.Count, actual.Count);

    foreach (var (e, a) in expected.Zip(actual))
    {
        Assert.Equal(e, a);
    }

    {{csUtils}}
    """
    |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
    |> withName "Test"
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``FSharpSet<RecordClass>: can create using C# collection expression`` () =
    CSharp $$"""
    {{csUsings}}

    var expected = SetModule.OfArray([new RecordClass(1), new RecordClass(2), new RecordClass(3)]);
    FSharpSet<RecordClass> actual = [new RecordClass(1), new RecordClass(2), new RecordClass(3)];

    Assert.Equal(expected, actual);
    Assert.Equal(expected.Count, actual.Count);

    foreach (var (e, a) in expected.Zip(actual))
    {
        Assert.Equal(e, a);
    }

    {{csUtils}}
    """
    |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
    |> withName "Test"
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``FSharpSet<RecordStruct>: can create using C# collection expression`` () =
    CSharp $$"""
    {{csUsings}}

    var expected = SetModule.OfArray([new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)]);
    FSharpSet<RecordStruct> actual = [new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)];

    Assert.Equal(expected, actual);
    Assert.Equal(expected.Count, actual.Count);

    foreach (var (e, a) in expected.Zip(actual))
    {
        Assert.Equal(e, a);
    }

    {{csUtils}}
    """
    |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
    |> withName "Test"
    |> compileExeAndRun
    |> shouldSucceed

#endif
