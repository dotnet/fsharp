// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test

module SuggestionBuffer =
    open FSharp.Compiler.ErrorResolutionHints
    
    [<Fact>]
    let NewBufferShouldBeEmpty() =
        let buffer = SuggestionBuffer("abdef")
    
        Assert.shouldBeFalse buffer.Disabled
        Assert.shouldBeEmpty buffer
        
    [<Fact>]
    let BufferShouldOnlyAcceptSimilarElements() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("somethingcompletelyunrelated")
        
        let results = Array.ofSeq buffer
        
        Assert.shouldBeEquivalentTo [| "abce" |] results
    
    [<Fact>]
    let SmallIdentifierShouldBeIgnored() =
        let buffer = SuggestionBuffer("ab")

        Assert.shouldBeTrue buffer.Disabled

        buffer.Add("abce")
        buffer.Add("somethingcompletelyunrelated")
        buffer.Add("abcg")
        buffer.Add("abch")
        buffer.Add("abcde")
        buffer.Add("abci")
        buffer.Add("abcj")

        let results = Array.ofSeq buffer

        Assert.shouldBeTrue buffer.Disabled
        Assert.shouldBeEquivalentTo [||] results

    [<Fact>]
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

        Assert.shouldBeEquivalentTo [| "abce"; "abcg"; "abch"; "abci"; "abcj"|] results

    [<Fact>]
    let BufferShouldUseEarlierElementsIfTheyHaveSameScore() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("abcf")
        buffer.Add("abcg")
        buffer.Add("abch")
        buffer.Add("abci")
        buffer.Add("abcj")

        let results = Array.ofSeq buffer

        Assert.shouldBeEquivalentTo [| "abce"; "abcf"; "abcg"; "abch"; "abci"|] results


    [<Fact>]
    let BufferShouldDisableItselfIfItSeesTheOriginalIdentifier() =
        let buffer = SuggestionBuffer("abcd")
        buffer.Add("abce")
        buffer.Add("abcf")
        buffer.Add("abcg")
        buffer.Add("abch")

        Assert.shouldBeFalse buffer.Disabled
        Assert.shouldNotBeEmpty buffer

        buffer.Add("abcd")  // original Ident
        buffer.Add("abcj")

        Assert.shouldBeTrue buffer.Disabled
        Assert.shouldBeEmpty buffer

    [<Fact>]
    let BufferShouldIgnoreSmallIdentifiers() =
        let buffer = SuggestionBuffer("abd")
        buffer.Add("abce")
        buffer.Add("abc")
        buffer.Add("ab")
        buffer.Add("ad")
        
        let results = Array.ofSeq buffer
        
        Assert.shouldBeEquivalentTo [| "abc"; "abce" |] results
