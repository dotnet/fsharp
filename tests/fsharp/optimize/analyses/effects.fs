
open System

module BasicSizeTests = 
    let constantUnit () = ()           // note, size 0 - seems reasonable 
    let constantInteger () = 1         // note, size 0 - seems reasonable 
    let constantValue x = x            // note, size 1 - seems reasonable (this may be a field lookup to access an environment variable)
    let fieldLookup1 x = x.contents   // note, size 2 - seems reasonable 
    let libraryCall1 x = System.Console.WriteLine(x:string) // size 2, seems reasonable
    let libraryCall2 x = new System.Object() // size 1, seems reasonable
    let constantData1 () = Some 1      // note, size 2, seems reasonable, perhaps too low
    let constantData2 () = None        // note, size 2, seems reasonable, perhaps too high 
    let constantTuple1 () = (1,2)      // note, size 1, seems reasonable, perhaps too low
    let constantTuple2 () = (1,2,3,4)  // note, size 1, seems reasonable, perhaps too low

    let indirectCall1 f = f()          // note, size 1, seems reasonable
    let indirectCall2 f x = f x        // note, size 2, seems reasonable
    let indirectCall3 f x y = f x y    // note, size 3, seems reasonable
    let sequenceOfIndirectCalls1 f x = f x; f x  // note, size 4 = 2 + 2
    let ifThenElse x  = if x then 1 else 2  // note, size 5, seems reasonable
    let patternMatch1 x  = match x with 1 -> 2 | 2 -> 1 | _ -> 3 // note, size 8, seems reasonable
    let forLoop1 f x  = for i = 1 to 5 do f () // note, size 7, seems reasonable
    let whileLoop1 f x  = while true do f () // note, size 6, seems reasonable

module BasicAnalysisTests = 

    let indirectCallInNonTailcallPosition f x y = f x y; 1+2  

    let rec infiniteLoop (x:int) : int = infiniteLoop x
    let rec genericInfiniteLoop (x:'a) : 'a = genericInfiniteLoop x
    let callInfiniteLoop (x:int) : int = infiniteLoop x
    let rec infiniteLoop2 (x1:int) (x2:int) : int = infiniteLoop2 x1 x2
    let callGenericInfiniteLoop (x:'a) : 'a = genericInfiniteLoop x

    let rec loopViaModuleFunction1 (f: 'T -> bool) (arr: 'T array) i =
        loopViaModuleFunction1 f arr (i+1)

    let rec loopViaModuleFunction (f: 'T -> bool) (arr: 'T array) i =
        i >= arr.Length || (f arr.[i] && loopViaModuleFunction f arr (i+1))

    let loopViaInnerFunction (f: 'T -> bool) (array: 'T array) =
        let len = array.Length
        let rec loop i = i >= len || (f array.[i] && loop (i+1))
        loop 0

    let simpleLibraryUse1 s = raise s
    let simpleLibraryCall2 inps = List.map id inps
    let simpleLibraryCall3 inps = List.forall id inps
    let simpleLibraryCall4 inps = List.exists id inps
    let simpleLibraryCall5 inps = List.iter id inps

    let simpleLibraryCall6 inps = Array.map id inps
    let simpleLibraryCall7 inps = Array.forall id inps
    let simpleLibraryCall8 inps = Array.exists id inps
    let simpleLibraryCall9 inps = Array.iter id inps

    let simpleLibraryCall10 inps = Seq.map id inps
    let simpleLibraryCall11 inps = Seq.forall id inps
    let simpleLibraryCall12 inps = Seq.exists id inps
    let simpleLibraryCall13 inps = Seq.iter id inps
    let simpleLibraryUse14 x = x + x
    let simpleLibraryUse15 x = x - x
    let simpleLibraryUse16 x = x * x
    let simpleLibraryUse17 x = x / x
    let simpleLibraryUse18 x = x % x
    let simpleLibraryUse19 (x:string) = x + x

    type SetTree<'T> = 
        | SetEmpty                                          // complexDataAnalysisFunction = 0   
        | SetNode of 'T * SetTree<'T> *  SetTree<'T> * int    // complexDataAnalysisFunction = int 
        | SetOne  of 'T                                     // complexDataAnalysisFunction = 1   

    let complexDataAnalysisFunction t = 
        match t with 
        | SetEmpty -> 0
        | SetOne _ -> 1
        | SetNode (_,_,_,h) -> h

    let tolerance = 2

    let complexDataConstructionFunction l k r = 
        match l,r with 
        | SetEmpty,SetEmpty -> SetOne (k)
        | _ -> 
          let hl = complexDataAnalysisFunction l 
          let hr = complexDataAnalysisFunction r 
          let m = if hl < hr then hr else hl 
          SetNode(k,l,r,m+1)

    let veryComplexDataConstructionFunction t1 k t2 =
        let t1h = complexDataAnalysisFunction t1 
        let t2h = complexDataAnalysisFunction t2 
        if  t2h > t1h + tolerance then // right is heavier than left 
            match t2 with 
            | SetNode(t2k,t2l,t2r,t2h) -> 
                if complexDataAnalysisFunction t2l > t1h + 1 then  
                    match t2l with 
                    | SetNode(t2lk,t2ll,t2lr,t2lh) ->
                        complexDataConstructionFunction (complexDataConstructionFunction t1 k t2ll) t2lk (complexDataConstructionFunction t2lr t2k t2r) 
                    | _ -> failwith "veryComplexDataConstructionFunction"
                else // rotate left 
                    complexDataConstructionFunction (complexDataConstructionFunction t1 k t2l) t2k t2r
            | _ -> failwith "veryComplexDataConstructionFunction"
        else
            if  t1h > t2h + tolerance then // left is heavier than right 
                match t1 with 
                | SetNode(t1k,t1l,t1r,t1h) -> 
                    if complexDataAnalysisFunction t1r > t2h + 1 then 
                        match t1r with 
                        | SetNode(t1rk,t1rl,t1rr,t1rh) ->
                            complexDataConstructionFunction (complexDataConstructionFunction t1l t1k t1rl) t1rk (complexDataConstructionFunction t1rr k t2)
                        | _ -> failwith "veryComplexDataConstructionFunction"
                    else
                        complexDataConstructionFunction t1l t1k (complexDataConstructionFunction t1r k t2)
                | _ -> failwith "veryComplexDataConstructionFunction"
            else complexDataConstructionFunction t1 k t2
            
    type NullNotPossible(i:int) =
        member __.X = i   // no effects
        abstract member A : int
        default __.A = i  // no effects

    [<AllowNullLiteral(false)>]
    type NullNotPossibleAttr(i:int) =
        member __.X = i   // no effects
        abstract member A : int
        default __.A = i  // no effects

    [<AllowNullLiteral>]
    type NullPossible(i:int) =
        member __.X = i   // yes effects
        abstract member A : int
        default __.A = i  // yes effects
        
    type DerivedFromNullPossible(i:int) =
        inherit NullPossible(i)
        member __.Y = i   // no effects
        member __.Z = base.X   // yes effects
        override __.A = i  // no effects
      
    [<AllowNullLiteral(false)>]
    type DerivedFromNullPossibleAttrFalse(i:int) =
        inherit NullPossible(i)
        member __.Y = i   // no effects
        member __.Z = base.X   // yes effects
        override __.A = i  // no effects

    [<AllowNullLiteral>]
    type DerivedFromNullPossibleAlsoNullPossible(i:int) =
        inherit NullPossible(i)
        member __.Y = i   // yes effects
        member __.Z = base.X   // yes effects
        override __.A = i  // yes effects

printfn "Test run"