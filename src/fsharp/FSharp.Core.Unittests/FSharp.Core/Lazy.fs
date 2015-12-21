// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace SystematicUnitTests.FSharp_Core.Microsoft_FSharp_Control

open System
open NUnit.Framework

open Microsoft.FSharp.Collections
open SystematicUnitTests.LibraryTestFx

// Various tests for the:
// Microsoft.FSharp.Collections prim types

(*
[Test Strategy]
Make sure each method works on:
* Integer  (value type)
* String  (reference type)
* Null  
*)

[<Parallelizable(ParallelScope.Fixtures)>][<TestFixture>]
type LazyType() =
   
    [<Test>]
    member this.Create() =
        
        // int 
        let intLazy  = Lazy.Create(fun () -> 2)
        Assert.AreEqual(intLazy.Value,2)
        
        // string
        let stringLazy = Lazy.Create(fun () -> "string")
        Assert.AreEqual(stringLazy.Value,"string")
        
        //null
        let nullLazy = Lazy.Create(fun () -> ())
        Assert.AreEqual(nullLazy.Value,null)
        
    [<Test>]
    member this.CreateFromValue() =
        
        // int 
        let intLazy  = Lazy.CreateFromValue( 2)
        Assert.AreEqual(intLazy.Value,2)
        
        // string
        let stringLazy = Lazy.CreateFromValue( "string")
        Assert.AreEqual(stringLazy.Value,"string")
        
        //null
        let nullLazy = Lazy.CreateFromValue(null)
        Assert.AreEqual(nullLazy.Value,null)
         
        
    [<Test>]
    member this.Force() =
        
        // int 
        let intLazy  = Lazy.CreateFromValue( 2)
        let intForce = intLazy.Force()
        Assert.AreEqual(intForce,2)
        
        // string
        let stringLazy = Lazy.CreateFromValue( "string")
        let stringForce = stringLazy.Force()
        Assert.AreEqual(stringForce,"string")
        
        //null
        let nullLazy = Lazy.CreateFromValue(null)
        let nullForce = nullLazy.Force()
        Assert.AreEqual(nullForce,null)
        
    
    [<Test; Ignore("See FSB #9999 – Need way to validate schronized data access and locking")>]
    member this.SynchronizedForce() = 
  
        // int 
        let intLazy  = Lazy.CreateFromValue( 2)
        let intSynchronizedForce = intLazy.SynchronizedForce()
        Assert.AreEqual(intSynchronizedForce,2)
        
        // string
        let stringLazy = Lazy.CreateFromValue( "string")
        let stringSynchronizedForce = stringLazy.SynchronizedForce()
        Assert.AreEqual(stringSynchronizedForce,"string")
        
        //null
        let nullLazy = Lazy.CreateFromValue(null)
        let nullSynchronizedForce = nullLazy.SynchronizedForce()
        Assert.AreEqual(nullSynchronizedForce,null)
        
    
    [<Test; Ignore("See FSB #9999 – Need way to validate schronized data access and locking")>]
    member this.UnsynchronizedForce() =
        
        // int 
        let intLazy  = Lazy.CreateFromValue( 2)
        let intUnsynchronizedForce = intLazy.UnsynchronizedForce()
        Assert.AreEqual(intUnsynchronizedForce,2)
        
        // string
        let stringLazy = Lazy.CreateFromValue( "string")
        let stringUnsynchronizedForce = stringLazy.UnsynchronizedForce()
        Assert.AreEqual(stringUnsynchronizedForce,"string")
        
        //null
        let nullLazy = Lazy.CreateFromValue(null)
        let nullUnsynchronizedForce = nullLazy.UnsynchronizedForce()
        Assert.AreEqual(nullUnsynchronizedForce,null)
        
        
        
    [<Test>]
    member this.Value() =
        
        // int 
        let intLazy  = Lazy.CreateFromValue( 2)
        Assert.AreEqual(intLazy.Value,2)
        
        // string
        let stringLazy = Lazy.CreateFromValue( "string")
        Assert.AreEqual(stringLazy.Value,"string")
        
        //null
        let nullLazy = Lazy.CreateFromValue(null)
        Assert.AreEqual(nullLazy.Value,null)
        
    [<Test>]
    member this.IsDelayed() =
        
        // int 
        let intLazy  = Lazy.Create( fun () -> 1)
        Assert.AreEqual( intLazy.IsDelayed,true)
        let resultIsDelayed = intLazy.Force()
        Assert.AreEqual( intLazy.IsDelayed,false)
        
        // string
        let stringLazy = Lazy.Create( fun () -> "string")
        Assert.AreEqual( stringLazy.IsDelayed,true)
        let resultIsDelayed = stringLazy.Force()
        Assert.AreEqual( stringLazy.IsDelayed,false)
        
        
        //null
        let nullLazy = Lazy.Create(fun () -> null)
        Assert.AreEqual( nullLazy.IsDelayed,true)
        let resultIsDelayed = nullLazy.Force()
        Assert.AreEqual( nullLazy.IsDelayed,false)
        
    [<Test>]
    member this.IsForced() =
        
        // int 
        let intLazy  = Lazy.Create( fun () -> 1)
        Assert.AreEqual( intLazy.IsForced,false)
        let resultIsForced = intLazy.Force()
        Assert.AreEqual( intLazy.IsForced,true)
        
        // string
        let stringLazy = Lazy.Create( fun () -> "string")
        Assert.AreEqual( stringLazy.IsForced,false)
        let resultIsForced = stringLazy.Force()
        Assert.AreEqual( stringLazy.IsForced,true)
        
        
        //null
        let nullLazy = Lazy.Create(fun () -> null)
        Assert.AreEqual( nullLazy.IsForced,false)
        let resultIsForced = nullLazy.Force()
        Assert.AreEqual( nullLazy.IsForced,true)
        
