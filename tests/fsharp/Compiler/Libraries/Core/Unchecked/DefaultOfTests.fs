// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``DefaultOf Tests`` =

    type DUType = 
        | A
        | B of int
        | C of DUType * DUType
        
    type RecordType = { A : int; B : string; C : DUType }
    
    type ClassType = string
       
    type InterfaceType =
        abstract DoStuff : unit -> unit
        
    type EnumType =
        | A = 1
        | B = 2
        | C = 4
            
    type StructType = struct
        val m_ivalue : int
        val m_svalue : string
        member this.IValue = this.m_ivalue
        member this.SValue = this.m_svalue
    end

    [<Test>]
    let `` Unchecked defaultof reference types``() =
        Assert.areEqual Unchecked.defaultof<ClassType> null
        Assert.areEqual (box Unchecked.defaultof<DUType>) null
        Assert.areEqual (box Unchecked.defaultof<RecordType>) null
        Assert.areEqual (box Unchecked.defaultof<InterfaceType>) null

    [<Test>]
    let ``Unchecked defaultof stack types``() =
        Assert.areEqual Unchecked.defaultof<int> 0
        Assert.areEqual Unchecked.defaultof<float> 0.0
        Assert.areEqual Unchecked.defaultof<EnumType> (enum 0)
        Assert.areEqual Unchecked.defaultof<StructType>.IValue 0
        Assert.areEqual Unchecked.defaultof<StructType>.SValue null

    type R = { x : int; y : string }
    type U = | A of int | B of string
    type S = struct val mutable x : int end
    type C() = class end

    [<Test>]
    let ``Unchecked defaultof and equality``() =
        // FSharp1.0:5417 - Unchecked.defaultof<_> on records/unions can cause structural equality check to throw
        // Check that Unchecked.defaultof<_> works correctly on various types, mostly structs/unions/records

        Assert.areEqual Unchecked.defaultof<R> Unchecked.defaultof<R>
        Assert.areEqual Unchecked.defaultof<U> Unchecked.defaultof<U>
        Assert.areEqual Unchecked.defaultof<S> Unchecked.defaultof<S>
        Assert.areEqual Unchecked.defaultof<C> Unchecked.defaultof<C>
        Assert.areEqual Unchecked.defaultof<int> Unchecked.defaultof<int>