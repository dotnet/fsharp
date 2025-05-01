// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.Compiler.InternalCollections

open System
open System.IO
open Xunit
open Internal.Utilities.Collections
       
                
type MruCache = 
    new() = { }        

    member private rb.NumToString = function
        | 0->"Zero"  | 1->"One"
        | 2->"Two"   | 3->"Three"
        | 4->"Four"  | 5->"Five"
        | 6->"Six"   | 7->"Seven"
        | 8->"Eight" | 9->"Nine"
        | _ -> failwith "Out of range"
        
    member private rb.NumToStringBox n = box (rb.NumToString n)

#if DISABLED_OLD_UNITTESTS
    [<Fact>]
    member public rb.Basic() = 
        let m = new MruCache<int,string>(3, (fun (x,y) -> x = y))
        let s = m.Get(5)
        Assert.True("Five"=s)
        let s = m.Get(6)
        Assert.True("Six"=s)
        let s = m.Get(7)
        Assert.True("Seven"=s)
        let s = m.Get(8)
        Assert.True("Eight"=s)
        let (i,s) = Option.get m.MostRecent
        Assert.Equal(8,i)
        Assert.True("Eight"=s)
        ()

    [<Fact>]
    member public rb.MostRecentOfEmpty() = 
        let m = new MruCache<int,string>(3, rb.NumToString, (fun (x,y) -> x = y))
        match m.MostRecent with
            | Some _->failwith "Expected None"
            | None->()


    [<Fact>]
    member public rb.SetAlternate() = 
        let m = new MruCache<int,string>(3, rb.NumToString, (fun (x,y) -> x = y))
        m.SetAlternate(2,"Banana")
        let (i,s) = Option.get m.MostRecent
        Assert.Equal(2,i)
        Assert.True("Banana"=s)
            
    member private rb.AddBanana(m:MruCache<int,obj>) = 
        let banana = new obj()
        m.SetAlternate(2,banana)
        let s = m.Get(2)
        Assert.Equal(banana,s)                    
            
    [<Fact>]
    member public rb.CacheDepthIsHonored() = 
        let m = new MruCache<int,obj>(3, rb.NumToStringBox, (fun (x,y) -> x = y))
        rb.AddBanana(m) // Separate function to keep 'banana' out of registers
        let _ = m.Get(3)
        let _ = m.Get(4)
        let _ = m.Get(5)
        GC.Collect()
        let s = m.Get(2)
        Assert.True("Two"=downcast s)
            
    [<Fact>]
    member public rb.SubsumptionIsHonored() = 
        let PairToString (s,n) = rb.NumToString n
        let AreSameForSubsumption((s1,n1),(s2,n2)) = n1=n2
                
        let m = new MruCache<string*int,string>(3, PairToString, (fun (x,y) -> x = y), areSimilar=AreSameForSubsumption)
        m.SetAlternate(("x",2),"Banana")
        let s = m.Get (("x",2))
        Assert.True("Banana"=s, "Check1")                                      
        let s = m.Get (("y",2))
        Assert.True("Two"=s, "Check2")                                      
        let s = m.Get (("x",2))
        Assert.True("Two"=s, "Check3") // Not banana because it was subsumed

    [<Fact>]
    member public rb.OnDiscardIsHonored() = 
        
        let AreSameForSubsumption((s1,n1),(s2,n2)) = s1=s2
                
        let discarded = ref [] 
        let m = new MruCache<string*int,string>(compute=fst, areSimilar=(fun (x,y) -> x = y), areSimilar=AreSameForSubsumption, keepStrongly=2, keepMax=2, onDiscard=(fun s -> discarded := s :: !discarded))
        m.SetAlternate(("x",1),"Banana") // no discard
        printfn "discarded = %A" discarded.Value
        Assert.True(discarded.Value = [], "Check1")                                      
        m.SetAlternate(("x",2),"Apple") // forces discard of x --> Banana
        printfn "discarded = %A" discarded.Value
        Assert.True(discarded.Value = ["Banana"], "Check2")                                      
        let s = m.Get (("x",3))
        printfn "discarded = %A" discarded.Value
        Assert.True(discarded.Value = ["Apple"; "Banana"], "Check3")                                      
        let s = m.Get (("y",4))
        printfn "discarded = %A" discarded.Value
        Assert.True(discarded.Value = ["Apple"; "Banana"], "Check4")                                      
        let s = m.Get (("z",5)) // forces discard of x --> Bananas
        printfn "discarded = %A" discarded.Value
        Assert.True(discarded.Value = ["x"; "Apple";"Banana"], "Check5")                                      
        let s = m.Get (("w",6)) // forces discard of y
        printfn "discarded = %A" discarded.Value
        Assert.True(discarded.Value = ["y";"x";"Apple";"Banana"], "Check6")                                      
#endif

type AccessToken() = class end
            
type AgedLookup() = 
    let mutable hold197 : byte [] = null
    let mutable hold198 : byte [] = null
    let mutable hold199 : byte [] = null

    let atok = AccessToken()
    let WeakRefTest n = 
        let al = AgedLookup<AccessToken,int,byte[]>(n, (fun (x,y) -> x = y))
            
        let AssertCached(i,o:byte array) = 
            match al.TryPeekKeyValue(atok,i) with
            | Some(_,x) -> Assert.True(obj.ReferenceEquals(o,x), sprintf "Object in cache (%d) does not agree with expectation (%d)" x.[0] i)
            | None -> Assert.True(false, "Object fell out of cache")
                
        let AssertExistsInCached(i) = 
            match al.TryPeekKeyValue(atok,i) with
            | Some _ -> ()
            | None -> Assert.True(false, "Object fell out of cache")                
                
        let AssertNotCached(i) = 
            match al.TryPeekKeyValue(atok,i) with
            | Some _ -> Assert.True(false, "Expected key to have fallen out of cache")     
            | None -> ()         
            
        let f() =
            try
                // Add some large objects
                for i in 150..199 do 
                    let mutable large : byte array = Array.create (5 * 1024 * 1024) (byte i)
                    if i = 197 then hold197<-large
                    if i = 198 then hold198<-large
                    if i = 199 then hold199<-large
                    al.Put(atok, i, large)
                    large<-null
            finally
                printfn "ensure these objects are never on the stack of the top-level test"
        f()    

        // At this point, item 0 should be long gone.
        AssertNotCached(0)
            
        // Also, hold197-hold199 may be strongly held depending on the value of 'n' passed to this test.
        GC.Collect() 
        let f() =
            try
                AssertCached(197,hold197)
                AssertCached(198,hold198)
                AssertCached(199,hold199)
            finally
                printfn "ensure these objects are never on the stack of the top-level test"
        f()            

        // Release a strongly held item (unless n=0) and see that it hasn't fallen out
        hold199 <- null
        GC.Collect()
        let f() =
            try
                AssertCached(197,hold197)
                AssertCached(198,hold198)
                if n>0 then 
                    AssertExistsInCached(199) // hold19 should be held
                else 
                    AssertNotCached(199)
            finally
                printfn "ensure these objects are never on the stack of the top-level test"
        f()            
            
        // Release a strongly held item (unless n<=1) and see that it hasn't fallen out
        hold198 <- null
        GC.Collect()
        let f() =
            try
                AssertCached(197,hold197)
                if n>1 then 
                    AssertExistsInCached(198) // hold198 should be held
                else 
                    AssertNotCached(198)         
            finally
                printfn "ensure these objects are never on the stack of the top-level test"
        f()            
            
        // Release a strongly held item (unless n<=2) and see that it hasn't fallen out
        hold197 <- null
        GC.Collect()
        let f() =
            try
                if n>2 then 
                    AssertExistsInCached(197) // hold197 should be held
                else
                    AssertNotCached(197)      
            finally
                printfn "ensure these objects are never on the stack of the top-level test"
        f()            
            
        // Let go of everything else.
        al.Clear(atok)
        GC.Collect()
        
        
    [<Fact>] member public rb.WeakRef0() = WeakRefTest 0
    [<Fact>] member public rb.WeakRef1() = WeakRefTest 1
    [<Fact>] member public rb.WeakRef2() = WeakRefTest 2
    [<Fact>] member public rb.WeakRef3() = WeakRefTest 3
