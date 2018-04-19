// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Text
open NUnit.Framework
open Microsoft.FSharp.Compiler
open PrettyNaming

[<TestFixture>]
type ManglingNamesOfProvidedTypesWithSingleParameter() = 
    
    [<Test>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| StaticArg "xyz" |], [| "Foo", Some "abc" |])
        Assert.AreEqual("MyNamespace.Test,Foo=\"xyz\"", mangled)
    
    [<Test>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| StaticArg "xyz" |], [| "Foo", Some "xyz" |])
        Assert.AreEqual("MyNamespace.Test", mangled)
    
    [<Test>]
    member this.DemangleNonDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test,Foo=\"xyz\""
        Assert.AreEqual("MyNamespace.Test", name)
        Assert.AreEqual([| "Foo", "xyz" |], parameters)
    
    [<Test>]
    member this.DemangleDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test,"
        Assert.AreEqual("MyNamespace.Test", name)
        Assert.AreEqual([||], parameters)

    [<Test>]
    member this.DemangleNewDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test"
        Assert.AreEqual("MyNamespace.Test", name)
        Assert.AreEqual([||], parameters)

[<TestFixture>]
type ManglingNamesOfProvidedTypesWithMultipleParameter() = 
    
    [<Test>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| StaticArg "xyz"; StaticArg "abc" |], 
                    [| "Foo", Some "foo"
                       "Foo2", Some "foo2" |])
        Assert.AreEqual("MyNamespace.Test,Foo=\"xyz\",Foo2=\"abc\"", mangled)
    
    [<Test>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| StaticArg "xyz"; StaticArg "abc" |], 
                    [| "Foo", Some "xyz"
                       "Foo2", Some "abc" |])
        Assert.AreEqual("MyNamespace.Test", mangled)
    
    [<Test>]
    member this.DemangleMultiParameter() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "TestType,Foo=\"xyz\",Foo2=\"abc\""
        Assert.AreEqual("TestType", name)
        Assert.AreEqual([| "Foo", "xyz"
                           "Foo2", "abc" |], parameters)