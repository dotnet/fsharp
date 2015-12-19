// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Unittests

open System
open System.Text
open NUnit.Framework
open Microsoft.FSharp.Compiler

[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
type ManglingNamesOfProvidedTypesWithSingleParameter() = 
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| "xyz" |], [| "Foo", Some "abc" |])
        Assert.AreEqual("MyNamespace.Test,Foo=\"xyz\"", mangled)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| "xyz" |], [| "Foo", Some "xyz" |])
        Assert.AreEqual("MyNamespace.Test", mangled)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.DemangleNonDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test,Foo=\"xyz\""
        Assert.AreEqual("MyNamespace.Test", name)
        Assert.AreEqual([| "Foo", "xyz" |], parameters)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.DemangleDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test,"
        Assert.AreEqual("MyNamespace.Test", name)
        Assert.AreEqual([||], parameters)

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.DemangleNewDefaultValue() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "MyNamespace.Test"
        Assert.AreEqual("MyNamespace.Test", name)
        Assert.AreEqual([||], parameters)

[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
type ManglingNamesOfProvidedTypesWithMultipleParameter() = 
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| "xyz"; "abc" |], 
                    [| "Foo", Some "foo"
                       "Foo2", Some "foo2" |])
        Assert.AreEqual("MyNamespace.Test,Foo=\"xyz\",Foo2=\"abc\"", mangled)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.computeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| "xyz"; "abc" |], 
                    [| "Foo", Some "xyz"
                       "Foo2", Some "abc" |])
        Assert.AreEqual("MyNamespace.Test", mangled)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.DemangleMultiParameter() = 
        let name, parameters = PrettyNaming.demangleProvidedTypeName "TestType,Foo=\"xyz\",Foo2=\"abc\""
        Assert.AreEqual("TestType", name)
        Assert.AreEqual([| "Foo", "xyz"
                           "Foo2", "abc" |], parameters)