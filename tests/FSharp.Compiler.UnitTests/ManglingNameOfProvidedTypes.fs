// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Text
open Xunit
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Compiler.Syntax

type ManglingNamesOfProvidedTypesWithSingleParameter() = 
    
    [<Fact>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| "xyz" |], [| "Foo", Some "abc" |])
        Assert.shouldBe "MyNamespace.Test,Foo=\"xyz\"" mangled
    
    [<Fact>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| "xyz" |], [| "Foo", Some "xyz" |])
        Assert.shouldBe "MyNamespace.Test" mangled
    
    [<Fact>]
    member this.DemangleNonDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test,Foo=\"xyz\""
        Assert.shouldBe "MyNamespace.Test" name
        Assert.shouldBeEquivalentTo [| "Foo", "xyz" |] parameters
    
    [<Fact>]
    member this.DemangleDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test,"
        Assert.shouldBe "MyNamespace.Test" name
        Assert.shouldBeEquivalentTo [||] parameters

    [<Fact>]
    member this.DemangleNewDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test"
        Assert.shouldBe "MyNamespace.Test" name
        Assert.shouldBeEquivalentTo [||] parameters


type ManglingNamesOfProvidedTypesWithMultipleParameter() = 
    
    [<Fact>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| "xyz"; "abc" |], 
                    [| "Foo", Some "foo"
                       "Foo2", Some "foo2" |])
        Assert.shouldBe "MyNamespace.Test,Foo=\"xyz\",Foo2=\"abc\"" mangled
    
    [<Fact>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| "xyz"; "abc" |], 
                    [| "Foo", Some "xyz"
                       "Foo2", Some "abc" |])
        Assert.shouldBe "MyNamespace.Test" mangled
    
    [<Fact>]
    member this.DemangleMultiParameter() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "TestType,Foo=\"xyz\",Foo2=\"abc\""
        Assert.shouldBe "TestType" name
        Assert.shouldBe([| "Foo", "xyz"
                           "Foo2", "abc" |], parameters)
