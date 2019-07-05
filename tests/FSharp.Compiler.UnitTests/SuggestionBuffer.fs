// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module SuggestionBuffer =
    open FSharp.Compiler.ErrorResolutionHints
    
    [<Test>]
    let NewBufferShouldBeEmpty() =
        let buffer = SuggestionBuffer("abdef")
    
        Assert.IsFalse buffer.Disabled
        Assert.IsEmpty buffer
        
    [<Test>]
    let BufferShouldOnlyAcceptSimilarElements() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("somethingcompletelyunrelated")
        
        let results = Array.ofSeq buffer
        
        Assert.areEqual [| "abce" |] results
    
    [<Test>]
    let SmallIdentifierShouldBeIgnored() =
        let buffer = SuggestionBuffer("ab")

        Assert.IsTrue buffer.Disabled

        buffer.Add("abce")
        buffer.Add("somethingcompletelyunrelated")
        buffer.Add("abcg")
        buffer.Add("abch")
        buffer.Add("abcde")
        buffer.Add("abci")
        buffer.Add("abcj")

        let results = Array.ofSeq buffer

        Assert.IsTrue buffer.Disabled
        Assert.areEqual [||] results

    [<Test>]
    let BufferShouldOnlyTakeTop5Elements() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("somethingcompletelyunrelated")
        buffer.Add("abcg")
        buffer.Add("abch")
        buffer.Add("abcde")
        buffer.Add("abci")
        buffer.Add("abcj")

        let results = Array.ofSeq buffer

        Assert.areEqual [| "abce"; "abcg"; "abch"; "abci"; "abcj"|] results

    [<Test>]
    let BufferShouldUseEarlierElementsIfTheyHaveSameScore() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("abcf")
        buffer.Add("abcg")
        buffer.Add("abch")
        buffer.Add("abci")
        buffer.Add("abcj")

        let results = Array.ofSeq buffer

        Assert.areEqual [| "abce"; "abcf"; "abcg"; "abch"; "abci"|] results


    [<Test>]
    let BufferShouldDisableItselfIfItSeesTheOriginalIdentifier() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("abcf")
        buffer.Add("abcg")
        buffer.Add("abch")

        Assert.IsFalse buffer.Disabled
        Assert.IsNotEmpty buffer

        buffer.Add("abcd")  // original Ident
        buffer.Add("abcj")

        Assert.IsTrue buffer.Disabled
        Assert.IsEmpty buffer

    [<Test>]
    let BufferShouldIgnoreSmallIdentifiers() =
        let buffer = SuggestionBuffer("abd")
        buffer.Add("abce")
        buffer.Add("abc")
        buffer.Add("ab")
        buffer.Add("ad")
        
        let results = Array.ofSeq buffer
        
        Assert.areEqual [| "abc"; "abce" |] results