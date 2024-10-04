// #Conformance #Regression #Recursion #LetBindings 
#if TESTS_AS_APP
module Core_tlr
#endif

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)



(*-------------------------------------------------------------------------
 *INDEX: tlr constants
 *-------------------------------------------------------------------------*)

(* mainly compile tests *)

module CompilationTests = begin 
    let (+)   x y = (x:int)+y
    let (-)   x y = (x:int)-y    
    let ( * ) x y = (x:int)*y    

    (* test cases for TLR code *)
    let consume x = x
    let (|>) x f = f x    

    (* not TLR - constant - trivial expr *)  
    let notSinceTrivial1 = 1
    let notSinceTrivial2 = 1.2
    let notSinceTrivial3 = true

    (* TLR constants - non-trivial (e.g. allocating) *)
    type ('a,'b) xy = {x:'a;y:'b}    
    let tlrValList   = [1;2;3;4]  
    let tlrValTuple  = (1,2,3,4)
    let tlrValRecord = {x=1;y=2}

    (* TLR constants - transitively *)
    let tlrValTransitiveList   = [ tlrValList; tlrValList ]
    let tlrValTransitiveTuple  = ("transitively a TLR constant",tlrValList)
    let tlrValTransitiveRecord = { x = "transitively a TLR constant"; y = tlrValList}

    (* TLR constants - polymorphic *)
    type 'a node = INT of int | ALPHA of 'a
    (*let tlrValPolymorphic : 'a node = INT 4*)


    (*-------------------------------------------------------------------------
     *INDEX: tlr lambdas
     *-------------------------------------------------------------------------*)

    let tlrLambdaTests () =
      (* TLR lambdas? - non rec *)  
      let     tlrNonRecAppliedAll3Args (x:int) (y:int) (z:int) = x+y+z
      in
      let _ = tlrNonRecAppliedAll3Args 1 2 3
      in
        
      let     tlrNonRecApplied2of3Args (x:int) (y:int) (z:int) = x+y+z
      in
      let _ = tlrNonRecApplied2of3Args 1 2
      in

      let     rejectNonRecApplied0of3Args (x:int) (y:int) (z:int) = x+y+z
      in
      let _ = rejectNonRecApplied0of3Args
      in

      (* TLR lambdas? - rec *)
      let rec tlrRecAppliedAll3Args (x:bool) (y:int) (z:int) = if x then tlrRecAppliedAll3Args false y z else y+z
      in
      let _ = tlrRecAppliedAll3Args true 2 3
      in

      let rec tlrRecApplied2of3Args (x:bool) (y:int) (z:int) = if x then let f = tlrRecApplied2of3Args false y in f z else y+z
      in
      let _ = tlrRecApplied2of3Args true 2
      in

      let rec rejectRecApplied0of3Args (x:bool) (y:int) (z:int) = if x then let f = rejectRecApplied0of3Args in f false y z else y+z
      in
      let _ = rejectRecApplied0of3Args
      in
      ()



    (*-------------------------------------------------------------------------
     *INDEX: tlr polymorphic constants?
     *-------------------------------------------------------------------------*)    

    (* Concerned about polymorphic constants. Arity 0, but in fact type-functions. *)
    let enclosing1 (a:int) =
      let tlrInnerFreePolymorphicConstant = None in
      if  tlrInnerFreePolymorphicConstant = None then 0 else 1

    let enclosing2 (a:'a) =
      let tlrInnerPolymorphicConstant = (None : 'a option) in
      if  tlrInnerPolymorphicConstant = None then 0 else 1


    (*-------------------------------------------------------------------------
     *INDEX: env tests
     *-------------------------------------------------------------------------*)    
        
    (* env tests *)

    let xC = 1,2,3
    let yC = 3,2,1

    let envTestFreesUnitArg ()      = xC,yC
    let envTestFreesNArg    (n:int) = xC,yC,n
    let uses            = envTestFreesUnitArg (),envTestFreesNArg 1

    let dependent1 id (xa:'alpha) =
      let envTestFreesUnitArgOpen ()      = id xC,yC   in
      let envTestFreesNArgOpen    (n:int) = id xC,yC,n in
      let envPolymorphicSelf arg = if arg=xa then 1 else 2 in  (* has freetypars *)
      let envPolymorphicViaCall () = envPolymorphicSelf xa in  (* the envPolymorphicSelf call will contribute typar to closure *)
      let uses =
        envTestFreesUnitArgOpen (),
        envTestFreesNArgOpen 1,
        envPolymorphicSelf xa,
        envPolymorphicViaCall ()
      in
      12


    (*-------------------------------------------------------------------------
     *INDEX: mixed recursion (inner recursing with outer)
     *-------------------------------------------------------------------------*)    

    (* test closure determination for inner functions recursing with outer functions *)
    let mixedRecursionTest (z:int) =
      (* What are the env closures?
       *   env(g2) = {x2,z} and envForDirectCallTo(g1)
       *   env(g1) = {z}
       * Note,
       *   env(g1) does not require the envForDirectCallTo(g2) since
       *   g2 is defined inside g1, and it's actual env will be defined at that binding point,
       *   so at any direct calls to g2, it's actual env will be available.
       * In general,
       *   Where-ever "h" is available to be called (for h chosen TLR),
       *   Then the actual-environment needed to pass to "h" will be available.
       *------
       * SUMMARY:
       * For g1 being made TLR, require sub-envs for direct calls only to the freevars of the g1 defn.
       *)
      let rec mixed_g1 (x1:int) (x2:int) =
        let rec mixed_g2 (y2:int) = let r1,r2 = mixed_g1 x2 y2 in
                                    r1 + mixed_g2 z in
        let res1 = mixed_g2 (x1+x2) in
        let res2 = mixed_g2         in
        res1,res2
      in
      mixed_g1 1 2

    (* test:
     * inner definition (g2) has direct call to g,
     * so etps(g2) must include etps(g),
     * but etps(g) are still being determined,
     * because later includesBeta causes beta to be included in etps(g),
     * since it follows from the etps(freeBeta).
     *)
    let innerOuterCallBeforeETpsKnown (xx1:'alpha) (y:'beta) =
       let freeBeta() = let (uses:'beta) = y in ()
       in
       let rec innerOuter_g (x1:'alpha) =
         let innerOuter_g2 (x2:'alpha2) =
           innerOuter_g x1                     (* direct call to g, so etps(g2) need etps(g) *)
         in 
         let (includesBeta:unit) = freeBeta () (* direct call, etps(g) includes etps(freeBeta) = {beta} *)
         in
         (innerOuter_g2 12 : int)
       in
       innerOuter_g 


    (*-------------------------------------------------------------------------
     *INDEX: arity 0 tests
     *-------------------------------------------------------------------------*)    

    (* concerned about arity 0 test cases, esp if they have a type closure *)
    let arityZeroTests (xalpha:'alpha) (xbeta:'beta) =
      let arityZeroMono      = (1,2,true) in
      let arityZeroAlpha     = (1,2,(None : 'alpha option)) in
      let arityZeroAlphaBeta = (1,2,(None : 'alpha option),(None : 'beta option)) in
      arityZeroMono,arityZeroAlpha,arityZeroAlphaBeta

    (* free occurrence, but at a type instance *)
    let freeOccurrenceAtInstanceTest (u:unit) (b:'beta) =
      let freeOccurrenceTestPolyFun (x:'alpha) = x in 
      let useAtInt      = freeOccurrenceTestPolyFun 3 in
      let useAtIntList  = freeOccurrenceTestPolyFun [3] in
      let useAtBool     = freeOccurrenceTestPolyFun true in
      let instAtInt     = (freeOccurrenceTestPolyFun : int -> int) in
      let instAtIntList = (freeOccurrenceTestPolyFun : int list -> int list) in
      let instAtBeta    = (freeOccurrenceTestPolyFun : 'beta -> 'beta) in
      ()


    (*-------------------------------------------------------------------------
     *INDEX: value recursion
     *-------------------------------------------------------------------------*)    

    (* Hit problems in letrec's with value recursions,
       because packing a recursive value into an environment failed,
       since uses to valrecs are required to be delayed.

       Solutions?
       (a) skip TLR if fclass has a valrec? (they are not common case)
       (b) if a valrec item needs to be carried for a closure,
           use it directly (no packing) carried by itself,
           so avoiding rebinding it into the environment.
     *)

    type func = {f:(bool -> int)}

    let inner1 () =
      let rec nextFun = {f=(fun x -> if x then 0 else next x)}
      and next x = nextFun.f x
      in
      ()

    let inner2 () =
      let rec nextFun next = {f=(fun x -> if x then 0 else next x)}
      and next x = (nextFun next).f x
      in
      ()

    let inner3 () =
      let rec env = (*pack*) next
      and nextFun env = let next = (*unpack*) env in
                        {f=(fun x -> if x then 0 else next x)}
      and next x = (nextFun env).f x
      in
      ()


    (*-------------------------------------------------------------------------
     *INDEX: inner constant
     *-------------------------------------------------------------------------*)    

    (* Creates cctor if needed *)
    let innerConst () =
      let localconst = ("cctor",0) in 
      let capture tag = if tag then localconst else "a3",3
      in
      capture true


    (*-------------------------------------------------------------------------
     *INDEX: lifting tests
     *-------------------------------------------------------------------------*)    
        

    (* Test cases for explicit lifting of inner TLR bindings to top-level *)
    let add (x:int) (y:int) = (x+y:int)

    (* lifting over a lambda *)  
    let liftOverLambda =
      fun (x:int) ->
        let liftOverLambdaExpectConst  = Some (1,2,3,4) in    
        let liftOverLambdaExpectFunc y = add x y,liftOverLambdaExpectConst in
        let res =
          liftOverLambdaExpectConst,
          liftOverLambdaExpectFunc 1
        in
        res

    (* lifting over a tlambda *)
    let overTLambda (* forall a' *) () =
      let xJustAnIntNotExpectedToBeTLR = 12 in
      let x = xJustAnIntNotExpectedToBeTLR in
      let overTLambda_ExpectConst  = Some (5,6) in    
      let overTLambda_ExpectFunc y = add y x,overTLambda_ExpectConst in
      let res =
        overTLambda_ExpectConst,
        overTLambda_ExpectFunc 3
      in
        ((raise (Failure "alpha return type")) : 'alpha)

    (* lifting over letrec *)
    let overLetrec (b:bool) = 
      let rec overLetrec_f1 x = overLetrec_f2 x
      and overLetrec_f2 x = overLetrec_f3 x
      and overLetrec_f3 x = 
          let overLetrec_expectConst = (7,8) in
          let overLetrec_expectFunc a = add a x,overLetrec_expectConst in
            overLetrec_expectConst,overLetrec_expectFunc x
          in
        overLetrec_f1 9

    (* lifting over let *)
    let overLet (b:bool) = 
      let overlet_x1 = 11 in
      let overlet_x2 = let overLet_expectConst2 = (2,2,2) in
                       12 in
      let overlet_x3 = 13 in
      let overlet_x4 = 14 in
      let overLet_expectFunc a = add a (overlet_x1)
      in
        overlet_x2,overlet_x1
          

    (* let test *)
    let letTest2 =
      let a = 1 in
      let b = 2 in
      (* swap *)
      let a = b in
      let b = a in
      a,b (* expect 2,1 *)

    let letTest3 =
      let rec v = fun n -> 1 + w (n-1) 
      and w = fun n -> if n=0 then 0 else v (n-1)
      in
      v,w


    (*-------------------------------------------------------------------------
     *INDEX: lifting
     *-------------------------------------------------------------------------*)

    let _ =
      fun x -> let liftOverTopLambda = [(1,2,3)] in 12

    let _ =
      match [] with
          []    -> let liftOverNilMatchNil  = [902] in 12
        | x::xs -> let liftOverNilMatchCons = [x]   in 12
end


module MiscDetupleTestFromAndyRay = begin

  type LutInitAst =
   | LutInput of int
   | LutAnd of LutInitAst * LutInitAst

  let i0, i1 = LutInput(0), LutInput(1)

  let rec eval n (s : LutInitAst) =
    match s with
    | LutInput(a) -> ((n >>> a) &&& 1)
    | LutAnd(a,b) -> (eval n a) &&& (eval n b)

  let eval_lut lut_n (ops : LutInitAst) =
    let rec eval_n n : string =
      if n = (1 <<< lut_n) then ""
      else (eval_n (n + 1)) + (if (eval n ops) = 1 then "1" else "0")
    in
    eval_n 0

  let test() =
      let g = eval_lut 2 (LutAnd (i0,i1)) in
      printf "%s\n" g

  do test()
end


(*-------------------------------------------------------------------------
 *INDEX: wrap up
 *-------------------------------------------------------------------------*)    

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

