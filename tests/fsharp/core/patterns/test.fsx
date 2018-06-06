// #Conformance #PatternMatching #Regression #Lists #ActivePatterns 
(* Pattern match tests.
 * Initially just some tests related to top-level let-pattern bug.
 * Later regression tests that patterns do project out the bits expected?
 *)
#if TESTS_AS_APP
module Core_patterns
#endif

open System
open System.Reflection

#light

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s x1 x2 = 
    if (x1 = x2) then 
        stderr.WriteLine ("test "+s+": ok")
    else 
        report_failure(s)

(* What kinds of top-leval let patterns are possible? *)

type r2 = {f1:int;f2:int}
type ('a,'b) either = This of 'a | That of 'b
exception XInt  of int
exception XBool of bool

(* Those with no variables *)
let () = ()
let (1) = (1)
let (1,2,3) = (1,2,3)  
let  1,2,3  = (1,2,3)
let [] = []
let [1] = [1]
let [1;2] = [1;2]
let (None) = None
let (Some 1) = Some 1
let {f1=f1;f2=f2} = {f1=1;f2=2}
let (XInt 1) = XInt 1  
let (XBool true) = XBool true

(* Those with some variables *)
let () = ()
let (v1) = (1)
let (v2,v3) = (2,3)  
let v4,v5,v6 = (4,5,6)
let v7 = []
let [v8] = [8]
let [v9;v10] = [9;10]
let (Some v11) = Some 11
let {f1=v12;f2=v13} = {f1=12;f2=13}
let (XInt v14) = XInt 14
let (XBool vTrue) = XBool true
let w1,[w2,Some (w3,XInt w4,XBool w5)] = 1,[2,Some (3,XInt 4,XBool true)]

(* Those with alternatives *)
let this_10 = This 10
let that_20 = That 20
let (This eiA | That eiA) = this_10
let (This eiB | That eiB) = that_20

(* polymorphic bound vars *)
let this_f1 = This (fun x -> x)
let that_f2 = That (fun x -> x)
let (This fA | That fA) = this_f1
let (This fB | That fB) = that_f2

(* If there are several polymorphic values matched,
   they get put into a tuple which has first class polymorphic field types...
   Users can not (directly!) create such tuples in source... need to check they are ok.
*)
let (Some xxxx),polyF1,polyF2,n =
  Some 1,
  (  (* ? forall alpha. *) fun (x:'alpha) -> x  ),
  (  (* ? forall beta.  *) fun (x:'beta) -> x   ),
  4

(* Relics *)
let aaa = []
let bb = match aaa with x::xs -> x | [] -> 0
let [p,q,r,(sA,sB)] = [1,2,3,(4,5)]
let [1],a,b,c = List.map (fun x -> x) [1],11,12,13
let y = 3
let x = try raise (Failure("a")) with e -> 12
let [x2] = [1]
let [1] = [1]

(* BUG: 438
 * null tests are split out,
 * but subsequent patterns need passing through the pattern simplifier,
 * specifically here to replace string patterns by equality test calls.
 *)
let bug438repro (tok:string) =
  match tok with
      null -> 11      
    | ""   -> 22
    | "aa" -> 33
    | str  -> 99

module ComplexNumbers_Example = begin
    // dummy type definition
    type complex = 
        { RealPart: float; ImaginaryPart: float }
        member x.Magnitude = 1.0
        member x.Phase = 1.0
    module Complex = 
        let mkRect (a,b) = { RealPart = a; ImaginaryPart = b }
        // dummy definition
        let mkPolar (a,b) = { RealPart = a; ImaginaryPart = b }
    
        
    open Microsoft.FSharp.Math
//    let Rect (x,y) = Complex.mkRect(x,y)
  //  let Polar (r,th) = Complex.mkPolar(r,th)
    
    let (|Rect|) (x:complex) = (x.RealPart, x.ImaginaryPart)
    let (|Polar|) (x:complex) = (x.Magnitude , x.Phase)

    let mulViaRect c1 c2 = 
        match c1,c2 with 
        | Rect(ar,ai), Rect(br,bi) -> Complex.mkRect(ar*br - ai*bi, ai*br + bi*ar)

    let mulViaPolar c1 c2 = 
        match c1,c2 with 
        | Polar(r1,th1),Polar(r2,th2) -> Complex.mkPolar(r1*r2, th1+th2)

    let mul1 (Rect(ar,ai)) (Rect(br,bi)) = Complex.mkRect(ar*br - ai*bi, ai*br + bi*ar)
    let mul2 (Polar(r1,th1)) (Polar(r2,th2)) = Complex.mkPolar(r1*r2, th1+th2)
end

module NaturalNumbers_Example = begin

    let Succ n = n+1
    
    let (|Zero|Succ|) n = if n = 0 then Zero else Succ(n-1)


    let rec fib n = 
        match n with
        | Succ (Succ m) -> fib m + fib (m+1)
        | Succ Zero -> 1
        | Zero -> 0

    let (|Even|Odd|) n = if n%2 = 0 then Even(n/2) else Odd(n-1)

    let rec power x n =
        match n with
        | Even m -> let p = power x m in p * p
        | Odd m -> x * power x m

end

// Queues 

module FunctionalQueue_Example = begin
    let (|Reversed|) l = List.rev l
    let (|NonEmpty|Empty|) q =
        match q with
        | (h::t),r               ->  NonEmpty(h,(t,r))
        | []    ,Reversed (h::t) ->  NonEmpty(h,(t,[]))
        | _                      -> Empty()

    let enqueue x (f,r) = (f,x::r)

    let dequeue2 q = 
        match q with
        | NonEmpty(x,NonEmpty(y,xs)) -> x,y,xs
        | NonEmpty(x,Empty) ->  failwith "singleton queue"
        | Empty -> failwith "empty queue"

    let q = ref ([1], [2;3;4])
    do System.Console.WriteLine("q = {0}", sprintf "%A" !q)
    do let x,y,rest = dequeue2 !q in System.Console.WriteLine("dequeue q = {0}", sprintf "%A" x); q := rest
    do let x,y,rest = dequeue2 !q in System.Console.WriteLine("dequeue q = {0}", sprintf "%A" x); q := rest
    do let x = enqueue 5 !q in System.Console.WriteLine("enqueue q = {0}", sprintf "%A" x); q := x
end

// CLR types 

module System_Type_Example2 = begin

    open System
    open System.Reflection
    
    let (|Named|Array|ByRef|Ptr|Param|) (typ : System.Type) =
        if typ.IsGenericType        then Named(typ.GetGenericTypeDefinition(), typ.GetGenericArguments())
        elif not typ.HasElementType then Named(typ, [| |])
        elif typ.IsArray            then Array(typ.GetElementType(), typ.GetArrayRank())
        elif typ.IsByRef            then ByRef(typ.GetElementType())
        elif typ.IsPointer          then Ptr(typ.GetElementType())
        elif typ.IsGenericParameter then Param(typ.GenericParameterPosition, typ.GetGenericParameterConstraints())
        else failwith "unexpected System.Type"

    //val widget : Type -> [ `Named of ... | `Array of ... | `ByRef of ... ]
    //val (|Named|Array|ByRef|Ptr|Param|) : Type -> Choices5< Type , .. , .. >

    let rec toString typ =
        match typ with
        | Named (con, args) -> "(" + con.Name + " " + String.Join(";",Array.map toString args) + ")"
        | Array (arg, rank) -> "(Array"  + rank.ToString() + " " + toString arg + ")"
        | ByRef arg         -> "(ByRef " + toString arg + ")"
        | Ptr arg           -> "(Ptr "   + toString arg + ")"
        | Param(pos,cxs)    -> "(Param " + sprintf "%A" (pos,cxs) + ")"

    do System.Console.WriteLine(typeof<(int list option ref)>)
end

// Join lists 

module JoinList_ExampleA = begin
    type ilist = 
        | Empty 
        | Single of int 
        | Join of ilist * ilist

    let rec (|Cons|Nil|) inp =
        match inp with  
        | Single x                -> Cons(x, Empty)
        | Join (Cons (x,xs), ys)  -> Cons(x, Join (xs, ys))
        | Join (Nil, Cons (y,ys)) -> Cons(y, Join (ys, Empty))
        | _                       -> Nil()

    let head js = 
        match js with 
        | Cons (x,_) -> x
        | _ -> failwith "empty list"

    do System.Console.WriteLine("JoinList_ExampleA1")
    do System.Console.WriteLine("1 = {0}", head (Single 1))
    do System.Console.WriteLine("JoinList_ExampleA2")
    do System.Console.WriteLine("true = {0}", head (Join (Single 1,Empty)))
    do System.Console.WriteLine("JoinList_ExampleA3")
    do System.Console.WriteLine("true = {0}", head (Join (Empty, Single 1)))
end

// Join lists 

module JoinList_Example = begin
    type ilist = 
        | Empty 
        | Single of int 
        | Join of ilist * ilist
    
    let rec (|Cons|Nil|) = function 
        | Single x                   -> Cons(x, Empty)
        | Join (Cons (x,xs), ys)     -> Cons(x, Join (xs, ys))
        | Join (Nil (), Cons (y,ys)) -> Cons(y, Join (ys, Empty))
        | _                          -> Nil()

    let head js = 
        match js with 
        | Cons (x,_) -> x
        | _ -> failwith "empty list"

    let rec map f xs =
        match xs with
        | Cons (y,ys) -> Join (Single (f y), map f ys)
        | Nil () -> Empty

    let rec to_list xs =
        match xs with
        | Cons (y,ys) -> y :: to_list ys
        | Nil () -> []

    let is = Join (Join (Single 0, Join (Single 1, Join (Empty, Empty))), Join (Empty, Join (Join (Single 2, Single 3), Single 4)))
    do System.Console.WriteLine("true = {0}", head (Join (Empty, Single 1)))
    do System.Console.WriteLine("true = {0}", head (Join (Join (Empty, Empty), Single 2)))
end

module PolyJoinList_Example = begin
    type 'a jlist = Empty | Single of 'a | Join of 'a jlist * 'a jlist

    let rec (|JCons|JNil|) = function 
        | Single x                     -> JCons(x, Empty)
        | Join (JCons (x,xs), ys)      -> JCons(x, Join (xs, ys))
        | Join (JNil (), JCons (y,ys)) -> JCons(y, Join (ys, Empty))
        | Empty 
        | Join (JNil (), JNil ()) -> JNil()

    let jhead js = 
        match js with 
        | JCons (x,_) -> x
        | JNil        -> failwith "empty list"

    let rec jmap f xs =
        match xs with
        | JCons (y,ys) -> Join (Single (f y), jmap f ys)
        | JNil () -> Empty

    let rec jlist_to_list xs =
        match xs with
        | JCons (y,ys) -> y :: jlist_to_list ys
        | JNil () -> []

    let js = Join (Join (Single 0, Join (Single 1, Join (Empty, Empty))), Join (Empty, Join (Join (Single 2, Single 3), Single 4)))
    do System.Console.WriteLine("js = {0}", sprintf "%A" (jlist_to_list js))
    do System.Console.WriteLine("jmap (+1) js = {0}", sprintf "%A" (jlist_to_list (jmap (fun x -> x+1) js)))
    do System.Console.WriteLine("true = {0}", jhead (Join (Empty, Single true)))
    do System.Console.WriteLine("true = {0}", jhead (Join (Join (Empty, Empty), Single true)))
end

module UnZip_Example = begin
    // Zip 
    let rec (|Unzipped|) = function 
        | ((x,y) :: Unzipped (xs, ys)) -> (x :: xs, y :: ys)
        | []                           -> ([], [])

    let unzip (Unzipped (xs, ys)) = xs, ys

    let zs = [(1,1);(2,4);(3,9);(4,16)]

    do System.Console.WriteLine("zs = {0}", sprintf "%A" zs)
    do System.Console.WriteLine("unzip zs = {0}", sprintf "%A" (unzip zs))
end


// Lazy lists 

module LazyList_Example = begin
    open System
    open System.Collections.Generic

    #nowarn "21" // recursive initialization
    #nowarn "40" // recursive initialization

    exception UndefinedException

    [<NoEquality; NoComparison>]
    type LazyList<'T> =
        { mutable status : LazyCellStatus< 'T > }
        
        member x.Value = 
            match x.status with 
            | LazyCellStatus.Value v -> v
            | _ -> 
                lock x (fun () -> 
                    match x.status with 
                    | LazyCellStatus.Delayed f -> 
                        x.status <- Exception UndefinedException; 
                        try 
                            let res = f () 
                            x.status <- LazyCellStatus.Value res; 
                            res 
                        with e -> 
                            x.status <- LazyCellStatus.Exception(e); 
                            reraise()
                    | LazyCellStatus.Value v -> v
                    | LazyCellStatus.Exception e -> raise e)
        
        member s.GetEnumeratorImpl() = 
            let getCell (x : LazyList<'T>) = x.Value
            let toSeq s = Seq.unfold (fun ll -> match getCell ll with CellEmpty -> None | CellCons(a,b) -> Some(a,b)) s 
            (toSeq s).GetEnumerator()
                
        interface IEnumerable<'T> with
            member s.GetEnumerator() = s.GetEnumeratorImpl()

        interface System.Collections.IEnumerable with
            override s.GetEnumerator() = (s.GetEnumeratorImpl() :> System.Collections.IEnumerator)


    and 
        [<NoEquality; NoComparison>]
        LazyCellStatus<'T> =
        | Delayed of (unit -> LazyListCell<'T> )
        | Value of LazyListCell<'T> 
        | Exception of System.Exception


    and 
        [<NoEquality; NoComparison>]
        LazyListCell<'T> = 
        | CellCons of 'T * LazyList<'T> 
        | CellEmpty

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module LazyList = 

        let lzy f = { status = Delayed f }
        let force (x: LazyList<'T>) = x.Value

        let notlazy v = { status = Value v }
        
        type EmptyValue<'T>() = 
            static let value : LazyList<'T> = notlazy CellEmpty
            static member Value : LazyList<'T> = value
            
        [<NoEquality; NoComparison>]
        type LazyItem<'T> = ItemCons of 'T * LazyList<'T> | ItemEmpty
        type 'T item = 'T LazyItem
        let get (x : LazyList<'T>) = match force x with CellCons (a,b) -> Some(a,b) | CellEmpty -> None
        let getCell (x : LazyList<'T>) = force x 
        let empty<'T> : LazyList<'T> = EmptyValue<'T>.Value
        let consc x l = CellCons(x,l)
        let cons x l = lzy(fun () -> (consc x l))
        let consDelayed x l = lzy(fun () -> (consc x (lzy(fun () ->  (force (l()))))))
        let consf x l = consDelayed x l

        let rec unfold f z = 
          lzy(fun () -> 
              match f z with
              | None       -> CellEmpty
              | Some (x,z) -> CellCons (x,unfold f z))

        let rec append l1  l2 = lzy(fun () ->  (appendc l1 l2))
        and appendc l1 l2 =
          match getCell l1 with
          | CellEmpty -> force l2
          | CellCons(a,b) -> consc a (append b l2)

        let delayed f = lzy(fun () ->  (getCell (f())))
        let repeat x = 
          let rec s = cons x (delayed (fun () -> s)) in s

        let rec map f s = 
          lzy(fun () ->  
            match getCell s with
            | CellEmpty -> CellEmpty
            | CellCons(a,b) -> consc (f a) (map f b))

        let rec map2 f s1 s2 =  
          lzy(fun () -> 
            match getCell s1, getCell s2  with
            | CellCons(a1,b1),CellCons(a2,b2) -> consc (f a1 a2) (map2 f b1 b2)
            | _ -> CellEmpty)

        let rec zip s1 s2 = 
          lzy(fun () -> 
            match getCell s1, getCell s2  with
            | CellCons(a1,b1),CellCons(a2,b2) -> consc (a1,a2) (zip b1 b2)
            | _ -> CellEmpty)
        let combine s1 s2 = zip s1 s2

        let rec concat s1 = 
          lzy(fun () -> 
            match getCell s1 with
            | CellCons(a,b) -> appendc a (concat b)
            | CellEmpty -> CellEmpty)
          
        let rec filter p s1= lzy(fun () ->  filterc p s1)
        and filterc p s1 =
            match getCell s1 with
            | CellCons(a,b) -> if p a then consc a (filter p b) else filterc p b
            | CellEmpty -> CellEmpty
          
        let rec tryFind p s1 =
            match getCell s1 with
            | CellCons(a,b) -> if p a then Some a else tryFind p b
            | CellEmpty -> None

        let first p s1 = tryFind p s1

        let indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException("An index satisfying the predicate was not found in the collection"))

        let find p s1 =
            match tryFind p s1 with
            | Some a -> a
            | None   -> indexNotFound()

        let rec scan f acc s1 = 
          lzy(fun () -> 
            match getCell s1 with
            | CellCons(a,b) -> let acc' = f acc a in consc acc' (scan f acc' b)
            | CellEmpty -> CellEmpty)

        let folds f acc s1 = scan f acc s1 // deprecated

        let head s = 
          match getCell s with
          | CellCons(a,_) -> a
          | CellEmpty -> invalidArg "s" "the list is empty"

        let tail s = 
          match getCell s with
          | CellCons(_,b) -> b
          | CellEmpty -> invalidArg "s" "the list is empty"

        let hd s = head s
        let tl s = tail s

        let isEmpty s =
          match getCell s with
          | CellCons _ -> false
          | CellEmpty -> true

        let nonempty s = not (isEmpty s)

        let rec take n s = 
          lzy(fun () -> 
            if n < 0 then invalidArg "n" "the number must not be negative"
            elif n = 0 then CellEmpty 
            else
              match getCell s with
              | CellCons(a,s) -> consc a (take (n-1) s)
              | CellEmpty -> invalidArg "n" "not enough items in the list" )

        let rec skipc n s =
          if n = 0 then force s 
          else  
            match getCell s with
            | CellCons(_,s) -> skipc (n-1) s
            | CellEmpty -> invalidArg "n" "not enough items in the list"

        let rec skip n s = 
          lzy(fun () -> 
            if n < 0 then invalidArg "n" "the value must not be negative"
            else skipc n s)

        let drop n s = skip n s

        let rec ofList l = 
          lzy(fun () -> 
            match l with [] -> CellEmpty | h :: t -> consc h (ofList t))
          
        let toList s = 
          let rec loop s acc = 
              match getCell s with
              | CellEmpty -> List.rev acc
              | CellCons(h,t) -> loop t (h::acc)
          loop s []
          
        let rec iter f s = 
          match getCell s with
          | CellEmpty -> ()
          | CellCons(h,t) -> f h; iter f t
          
        let rec copyFrom i a = 
          lzy(fun () -> 
            if i >= Array.length a then CellEmpty 
            else consc a.[i] (copyFrom (i+1) a))
          
        let rec copyTo (arr: _[]) s i = 
          match getCell s with
          | CellEmpty -> ()
          | CellCons(a,b) -> arr.[i] <- a; copyTo arr b (i+1)

        let ofArray a = copyFrom 0 a
        let toArray s = Array.ofList (toList s)
          
        let rec lengthAux n s = 
          match getCell s with
          | CellEmpty -> n
          | CellCons(_,b) -> lengthAux (n+1) b

        let length s = lengthAux 0 s

        let toSeq (s: LazyList<'T>) = (s :> IEnumerable<_>)

        // Note: this doesn't dispose of the IEnumerator if the iteration is not run to the end
        let rec ofFreshIEnumerator (e : IEnumerator<_>) = 
          lzy(fun () -> 
            if e.MoveNext() then 
              consc e.Current (ofFreshIEnumerator e)
            else 
               e.Dispose()
               CellEmpty)
          
        let ofSeq (c : IEnumerable<_>) =
          ofFreshIEnumerator (c.GetEnumerator()) 
          
        let (|Cons|Nil|) l = match getCell l with CellCons(a,b) -> Cons(a,b) | CellEmpty -> Nil


    let matchTwo(ll) = 
        match ll with 
        | LazyList.Cons(h1,LazyList.Cons(h2,t)) -> printf "%O,%O\n" h1 h2
        | LazyList.Cons(h1,t) -> printf "%O\n" h1
        | LazyList.Nil() -> printf "empty!\n" 

    open LazyList
    
    let rec pairReduce xs =
      match xs with
        | Cons (x, Cons (y,ys)) -> LazyList.consf (x+y) (fun () -> pairReduce ys)
        | Cons (x, Nil ())      -> LazyList.cons x LazyList.empty
        | Nil ()                -> LazyList.empty 

    let rec inf = LazyList.consf 0 (fun () -> LazyList.map (fun x -> x + 1) inf)

    let ll = LazyList.ofList [1;2;3;4]
    do System.Console.WriteLine(sprintf "%A" (LazyList.toList (LazyList.take 10 (pairReduce inf))))
end

(*
module RawQuotation_Examples1 = begin

    open Microsoft.FSharp.Quotations.Raw

    let (|Add|_|) = <@@| (_:int32) + (_:int32) |@@>
    let (|Mul|_|) = <@@| (_:int32) * (_:int32) |@@>
        
    let rec trans x =
        match x with 
        | Add(x,y) -> trans x + trans y
        | Mul(x,y) -> trans x * trans y
        | Int32(x) -> x
        | _ -> failwith "unrecognized"
        
    printf "res1 = %d\n" (trans <@@ 1+3+2 @@>)
end
*)


module PartialPattern_Examples = begin

     // Partial patterns are signfied by |_| and return 'option'.
     // They are most useful when dealing with repeatedly recurring queries
     // on very "heterogeneous" data sets, i.e. data sets able to represent 
     // a large range of possible entities, but where you're often interested in
     // focusing on a subset of the entities involved. Strings, term structures and XML
     // are common examples.
     let (|MulThree|_|) inp = 
        if inp % 3 = 0 then Some(inp/3) else None
     let (|MulSeven|_|) inp = 
        if inp % 7 = 0 then Some(inp/7) else None
        
     // Here is an example of their use.
     let example1 inp = 
         match 21 with 
         | MulThree(residue) -> printf "residue = %d!\n" residue
         | MulSeven(residue) -> printf "residue = %d!\n" residue
         | _ -> printf "no match!\n"

     example1 777
     example1 9
     example1 10
     example1 21

end
     

module ParameterizedPartialPattern_Examples = begin
     let (|Equal|_|) x y = 
        printf "x = %d!\n" x
        if x = y then Some() else None
        
     let example1 = 
         match 3 with 
         | Equal 4 () -> printf "3 = 4!\n"
         | Equal 3 () -> printf "3 = 3!\n"
         | _ -> printf "3 = ?!\n"

     let (|Lookup|_|) x map = Map.tryFind x map
        
     let example2 = 
         match Map.ofList [ "2", "Two" ; "3", "Three" ] with 
         | Lookup "4" v -> printf "4 should not be present!\n"
         | Lookup "3" v -> printf "map(3) = %s\n" v
         | Lookup "2" v -> printf "this should not be reached\n"
         | _ -> printf "3 = ?!\n"
end
     

module Combinator_Examples = begin

    type ('a,'b) query = 'a -> 'b option 
    let mapQ1 f (|P|_|) = function (P x) -> Some (f x) | _ -> None
    let app1 (|P|) (P x) = x
    let app2 (|P|_|) (P x) = x
    let mapQ2 f (|P|) (P x) = f x

    // Sets 

    // Given a partial pattern P find the first element in the list that satisfies P
    // This is obviously overkill but it's showing what's possible.
    let Find (|P|_|) =
        let rec (|E|_|) ys =
            match ys with 
            | (P x :: _  ) -> Some(x)
            | (_   :: E x) -> Some(x)
            | _ -> None
        (|E|_|)    

end

#if !NETCOREAPP1_0
module XmlPattern_Examples = begin


  open System.Xml
  open System.Collections
  open System.Collections.Generic

  let Select (|P|_|) (x: #XmlNode) = [ for P y as n in x.ChildNodes -> y ]

  let Select2 (|A|B|) (x: #XmlNode) = [ for (A y | B y) as n in x.ChildNodes -> y ]

  let (|Elem|_|) name (inp: #XmlNode) = 
      if inp.Name = name then Some(inp) 
      else None

  let (|Attr|_|) attr (inp: #XmlNode) = 
      match inp.Attributes.GetNamedItem(attr) with
      | null -> None
      | node -> Some(node.Value)

  let (|Num|_|) attr inp = 
      match inp with 
      | Attr attr v -> Some (float v) 
      | _           -> None

  type scene = 
      | Sphere of float * float * float * float
      | Intersect of scene list 
  
  let (|Vector|_|) = function (Num "x" x & Num "y" y & Num "z" z) -> Some(x,y,z) | _ -> None
  
  let rec (|ShapeElem|_|) inp = 
      match inp with 
      | Elem "Sphere" (Num "r" r  & Num "x" x & Num "y" y & Num "z" z) -> Some (Sphere (r,x,y,z)) 
      | Elem "Intersect" (ShapeElems(objs)) -> Some (Intersect objs) 
      | _ -> None

  and (|ShapeElems|) inp = Select (|ShapeElem|_|) inp 

  let parse inp = 
      match (inp :> XmlNode) with 
      | Elem "Scene" (ShapeElems elems) -> elems
      | _                               -> failwith "not a scene graph"

  let inp = "<Scene>
                <Intersect>
                  <Sphere r='2' x='1' y='0' z='0'/>
                  <Intersect>
                    <Sphere r='2' x='4' y='0' z='0'/>
                    <Sphere r='2' x='-3' y='0' z='0'/>
                  </Intersect>
                  <Sphere r='2' x='-2' y='1' z='0'/>
                </Intersect>
             </Scene>"
  let doc = new XmlDocument()
  doc.LoadXml(inp)
  //stdout.WriteLine doc.DocumentElement.Name
  printf "results = %A\n" (parse doc.DocumentElement)


end
#endif
module RegExp = 
    open System.IO
    
    let rec allFiles dir =
        [ for x in Directory.GetFiles(dir) do yield x
          for x in Directory.GetDirectories(dir) do yield! allFiles x ]

    open System.Text.RegularExpressions
    let (|IsMatch|_|) (pat:string) (inp:string) = if System.Text.RegularExpressions.Regex.IsMatch(inp,"^" + pat + "$") then Some(inp) else None
    let (|Match|_|) (pat:string) (inp:string) = 
        let m = Regex.Match(inp, "^" + pat + "$") in
        if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ]) else None

    let (|Match1|_|) (pat:string) (inp:string) = 
        match (|Match|_|) pat inp with 
        | Some [h] -> Some h
        | Some [] -> failwith "Match1 succeeded, but no groups found. Use '(.*)' to capture groups"
        | Some _ -> failwith "Match1 succeeded, but more than one group found. Use '(.*)' to capture groups"
        | None -> None

    let (|IsSubMatch|_|) (pat:string) (inp:string) = if Regex.IsMatch(inp,pat) then Some(inp) else None
    let (|SubMatch|_|) (pat:string) (inp:string) = 
        let m = Regex.Match(inp, pat) in
        if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ]) else None

    let check s b1 b2 = if b1 <> b2 then report_failure s
    check "fwhin3op1" ((|IsMatch|_|) ".*.ml" "abc.ml") (Some "abc.ml")
    check "fwhin3op2"((|Match|_|) ".*.ml" "abc.ml") (Some [])
    check "fwhin3op3" ((|Match|_|) ".*.ml" "abc.mlll") None
    check "fwhin3op4" ((|Match|_|) "(.*).ml" "abc.ml") (Some ["abc"])
    check "fwhin3op5" ((|Match|_|) "([abc]*).ml" "abc.ml") (Some ["abc"])
    check "fwhin3op6" ((|Match|_|) "([a-c]*).ml" "abc.ml") (Some ["abc"])
    check "fwhin3op7" ((|Match|_|) "([a-bc]*).ml" "abc.ml") (Some ["abc"])
    check "fwhin3op9" ((|Match|_|) "^.*.ml$" "abc.ml") (Some [])

    let testFun() = 
        File.WriteAllLines("test.fs", seq { for (IsMatch "(.*).fs" f) in allFiles (System.IO.Directory.GetCurrentDirectory()) do yield! "-------------------------------" :: "\n" :: "\n" :: ("// FILE: "+f) :: "" :: "module "+(f |> Path.GetDirectoryName |> Path.GetFileName |> (fun s -> s.ToUpper()))+ " =" :: [ for line in Array.toList (File.ReadAllLines(f)) -> "    "+line ] } |> Seq.toArray)

module RandomWalk = 
    let ran = new System.Random()
    let dice p = ran.NextDouble() <= p

    //! Random walk
    let randomWalk upP (nSteps,x) = if nSteps = 0 then None else
                                    if dice upP then Some (x+1,(nSteps-1,x+1))
                                                else Some (x-1,(nSteps-1,x-1))
    let n = 100

    let xs     = [| for i in 0 .. n -> i |]

    let walk initial upP = [| let state = ref initial 
                              for i in 0 .. n do
                                 if dice upP then incr state else decr state; 
                                 yield float !state |]

    let walkA1 = walk 10 0.54 
    let walkB1 = walk 14 0.46 

module RandomTEst = 
    type ICool =    
        abstract p : unit -> unit 
        
    type IEvenCooler =
        inherit ICool
    
#if !NETCOREAPP1_0
module RandomCodeFragment = 
    open System

    let createRemote<'T> (d: AppDomain) = 
        unbox<'T>(d.CreateInstanceAndUnwrap((typeof<'T>).Assembly.FullName,(typeof<'T>).FullName))

    //let o = createRemote<Inspector>(AppDomain.CurrentDomain)


    open System
    open System.Reflection
    open System.Threading

    type Remoter<'a>(f) =
      class
        inherit MarshalByRefObject()
        member x.Call : 'a = f()
      end

    let remoteCall (d:AppDomain) (f:unit->'a) =
      let t = (typeof<Remoter<'a>>)
      let r = d.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName, true, BindingFlags.CreateInstance, null, [| box f |], null, null, null)
              |> unbox<Remoter<'a>>
      r.Call

    let inspect (d:AppDomain) =
      let callingDomain = AppDomain.CurrentDomain.FriendlyName 
      remoteCall d 
        (fun() -> 
          printf "inspector called from: %s\nexecuted in: %s\non thread: %d\n\n\n"
            callingDomain 
            AppDomain.CurrentDomain.FriendlyName
            System.Threading.Thread.CurrentThread.ManagedThreadId)
            
 #endif
      
module ActivePatternsFromTheHub = 
    type callData = 
      { time : System.DateTime;
        duration : int;
        countryCode : string;
        areaCode : string;
        number : string }

    // Note: you ask for calling plans to be extensible. I've done this by using strings: in many ways
    // active patterns allow you to use more heterogeneous data (such as strings) and still use
    // functional programming (or at least pattern matching).
    //
    // The intention of the example appears to be that additional calling plans are given semantics
    // by new rules that resolve how they interact in a fairly adhoc way with the call data.
    // It's hard to see how you would permit "arbitrary" extensions in a modular way for
    // this example, since the devil is always in the resolution of potential conflicts and
    // ambiguities with other rules. If I've misunderstood the kind of extensibility you require
    // then please let me know. In any case it's a great example of adhoc matching.
    type userData = 
      { callingPlan : string }

    // Define patterns that extract features out of the above data structures. Once
    // these are defined we never access the internal data structures. 
    //
    //   Note for language geeks: we'll probably also add some kind of syntax for 
    //   automatically getting active patterns that extract properties, which would make
    //   these redundant.

    let (|CallTime|)   cd = cd.time
    let (|CountryCode|) cd = cd.countryCode
    let (|AreaCode|)    cd = cd.areaCode
    let (|CallingPlan|) ud = ud.callingPlan

    // Define some patterns that detect particular features relevant to the rules.
    let predicate b = if b then Some() else None

    let (|NightTime|_|) (time:System.DateTime) = 
        let hourOfDay = time.TimeOfDay.Hours in 
        predicate (hourOfDay < 5 or hourOfDay > 19)

    let (|International|_|) cc = 
        predicate (cc <> "")

    let perMinuteRate (cd,ud) = 
        match cd,ud with
        //Rule1: 1-800 numbers are free
        | AreaCode("800"), _ -> 0
        //Rule2: customers on 'Unlimited' plans don't pay per-minute charges
        | _,CallingPlan("Unlimited") -> 0
        //Rule3: International calls cost 20c/daytime and 10c/nighttime
        | CountryCode(International) & CallTime(NightTime), _ -> 10 
        | CountryCode(International), _ -> 20
        //Rule 4: anything else costs 1 dollar per minute
        | _ -> 100  

module AndrewKennedyFunkyActivePatternsBugRelatedToArityInference = 
    let (|Fun|) (x:int->int) =  x

    let (Fun(f)) = (fun x -> x)

module AndrewKennedyRatherEmarrassingSimpleBug1133 = 

    type a = A | B
    let f = fun A -> 1
    let g = fun [] -> 1
    check "vweioh3v209" (f A) 1
    check "vweioh3v209" (try f B with MatchFailureException _ -> 2) 2
    check "vweioh3v209" (g []) 1
    check "vweioh3v209" (try g [1] with MatchFailureException _ -> 2) 2
    check "vweioh3v209" (try g ["1"] with MatchFailureException _ -> 3) 3

module NameResolutionBug1134 = 
    module M1 = 
       type a = A | B
       let (|A2|) (x:a) = "a"

    module M2 = 
       let A = 1
       let A2 = 1


    open M1
    open M2
    let f x = match x with | A -> 4 | B -> 5 // the 'A' should resolve to the pattern identifier
    let g x = match x with | A2 _ -> 4 | B -> 5 // the 'A' should resolve to the pattern identifier

module CheckNameResoutionRules = 

    module M1a = 
        type t = C of int
        
    module M2a = 
        open M1a
        let (C x1) = C(3) 
        let (C x2) = C(3)
        let f1 = fun (C _) -> C
        let f2 = fun (C _) -> C
        let f3 = function C _ -> C
        let f4 = C
        let v = C 3
        let C = 1
        

    module M1b = 
        type t = C 

    module M2b = 
        open M1b
        let (C) = C
        let f1 = fun (C _) -> C
        let f2 = fun (C _) -> C
        let f3 = function C _ -> C
        let f4 = C
        let C x1 = x1

    module M3a1 = 
        type t = C 

    module M3a2 = 
        let C = 3

    module M3b = 
        open M3a1
        open M3a2
        let (C) = M3a1.C
        let f1 = fun (C _) -> C
        let f2 = fun (C _) -> C
        let f3 = function C _ -> C
        let f4 = C
        let C x1 = x1  // ok to define a function because "M3a1.C" is zero arity
    module Adhoc1 = 
        let Exit(args) = printfn "exit!"  // Exit is an OCaml-compatible exception in the F# library. This is a common case where people try to redefine it.
    module Adhoc2 = 
        let Exit() = printfn "exit!"  // Exit is an OCaml-compatible exception in the F# library. This is a common case where people try to redefine it.
        
(*
module BigIntAndBigNumPatternMatching = begin

    let test (m) =
        match m with
        | 0I -> "zero"
        | 1I -> "one"
        | _ -> "more"

    do check "v4j-042p91" (test 0I) "zero"
    do check "v4j-042p92" (test 1I) "one"
    do check "v4j-042p93" (test 2I) "more"


    let test2 (m) =
        match m with
        | 0N -> "zero"
        | 1N -> "one"
        | _ -> "more"

    do check "v4j-042p94" (test2 0N) "zero"
    do check "v4j-042p95" (test2 1N) "one"
    do check "v4j-042p96" (test2 2N) "more"
end
*)

module ActivePatternsWithIndeterminateReturnType = 

    let (|Cast|_|) (x:obj) : 'T option = match x with | :? 'T as t ->  Some t | _ -> None
    let (|CastIron|) (x:obj) : 'T = match x with | :? 'T as t ->  t | _ -> failwith "CastIron"

    let test inp = 
        match inp with 
        | Cast (x:int) -> 1
        | Cast "1" -> 2
        | Cast (x:string) -> 3
        | _ -> 4

    check "ckwenwe0" (test (box 1)) 1
    check "ckwenwe0" (test (box "1")) 2
    check "ckwenwe0" (test (box "2")) 3
    check "ckwenwe0" (test (box 1.0)) 4

    let test2 inp = 
        match inp with 
        | CastIron (x:int) -> 1

    check "ckwenwe0" (test2 (box 1)) 1
    check "ckwenwe0" ((try test2 (box "1") |> ignore; 1 with Failure _ -> 2)) 2
    check "ckwenwe0" ((try test2 (box 1.0) |> ignore; 1 with Failure _ -> 2)) 2

module TypecheckingBug_FSharp_1_0_6389 = 
    type Nullary<'T> = | Nullary 

    (* Separate bug: Nullary<int>.Nullary *)

    let f1 Nullary = ()
    let f2 Nullary Nullary = ()
    let f3 Nullary = Nullary
    let f4 Nullary Nullary = Nullary

    let f5<'T,'U> (Nullary: Nullary<'T>) = (Nullary: Nullary<'U>)
    let f6<'T,'U,'V> (Nullary: Nullary<'T>) (Nullary: Nullary<'U>)= (Nullary: Nullary<'V>)

    // check f3 is properly generic with type f3 : Nullary<'T> -> Nullary<'U>
    let v3 : Nullary<string> = f3 (Nullary : Nullary<int>) 
    let v4 : Nullary<string> = f4 (Nullary : Nullary<int>) (Nullary : Nullary<decimal>) 
    let v5 : Nullary<string> = f5<int,string> (Nullary : Nullary<int>) 
    let v6 : Nullary<string> = f6<int,decimal,string> (Nullary : Nullary<int>) (Nullary : Nullary<decimal>) 

    type C() = 
        static member M1 Nullary = ()
        static member M2 Nullary Nullary = ()
        static member M3 Nullary = Nullary
        static member M4 Nullary Nullary = Nullary

        static member M5<'T,'U> (Nullary: Nullary<'T>) = (Nullary: Nullary<'U>)
        static member M6<'T,'U,'V> (Nullary: Nullary<'T>) (Nullary: Nullary<'U>) = (Nullary: Nullary<'V>)

        // check f3 is properly generic with type M3 : Nullary<'T> -> Nullary<'U>
        static member P3 : Nullary<string> = C.M3 (Nullary : Nullary<int>) 
        static member P4 : Nullary<string> = C.M4 (Nullary : Nullary<int>) (Nullary : Nullary<decimal>) 
        static member P5 : Nullary<string> = C.M5<int,string> (Nullary : Nullary<int>) 
        static member P6 : Nullary<string> = C.M6<int,decimal,string> (Nullary : Nullary<int>) (Nullary : Nullary<decimal>) 


    type C2() = 
        member c.M1 Nullary = ()
        member c.M2 Nullary Nullary = ()
        member c.M3 Nullary = Nullary
        member c.M4 Nullary Nullary = Nullary

        member c.M5<'T,'U> (Nullary: Nullary<'T>) = (Nullary: Nullary<'U>)
        member c.M6<'T,'U,'V> (Nullary: Nullary<'T>) (Nullary: Nullary<'U>) = (Nullary: Nullary<'V>)

        // check f3 is properly generic with type M3 : Nullary<'T> -> Nullary<'U>
        member c.P3 : Nullary<string> = c.M3 (Nullary : Nullary<int>) 
        member c.P4 : Nullary<string> = c.M4 (Nullary : Nullary<int>) (Nullary : Nullary<decimal>) 
        member c.P5 : Nullary<string> = c.M5<int,string> (Nullary : Nullary<int>) 
        member c.P6 : Nullary<string> = c.M6<int,decimal,string> (Nullary : Nullary<int>) (Nullary : Nullary<decimal>) 


module StructUnionMultiCase = 
    open System
    open FSharp.Reflection

    [<Struct>]
    type X = 
        | Success of result: string 
        | Fail of Exn: DateTime

    check "ckwjhf24091" typeof<X>.IsValueType true

    for uc in FSharpType.GetUnionCases(typeof<X>) do
        check "ckwjhf2409" [ for p in uc.GetFields() -> p.Name ] [ match uc.Name with "Success" -> yield "result"  | "Fail" -> yield "Exn" ]

    check "ckwjhf24091" typeof<Result<int,string>>.IsValueType true

    for uc in FSharpType.GetUnionCases(typeof<Result<int,string>>) do
        check "ckwjhf2409" [ for p in uc.GetFields() -> p.Name ] [ match uc.Name with "Ok" -> yield "ResultValue"  | "Error" -> yield "ErrorValue" ]

module StructUnionMultiCaseLibDefns = 

    /// <summary>The type of optional values, represented as structs.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<Struct>]
    [<RequireQualifiedAccess>]
    type StructOption<'T> =
        /// <summary>The representation of "No value"</summary>
        | None :       StructOption<'T> 

        /// <summary>The representation of "Value of type 'T"</summary>
        /// <param name="Value">The input value.</param>
        /// <returns>An option representing the value.</returns>
        | Some : Value:'T -> StructOption<'T>

    /// <summary>Helper types for active patterns with 2 choices.</summary>
    //[<UnqualfiedLabels(false)>]
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpStructChoice`2")>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice<'T1,'T2> = 
      /// <summary>Choice 1 of 2 choices</summary>
      | Choice1Of2 of Item1: 'T1 
      /// <summary>Choice 2 of 2 choices</summary>
      | Choice2Of2 of Item2: 'T2
    
    /// <summary>Helper types for active patterns with 3 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpStructChoice`3")>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice<'T1,'T2,'T3> = 
      /// <summary>Choice 1 of 3 choices</summary>
      | Choice1Of3 of Item1: 'T1 
      /// <summary>Choice 2 of 3 choices</summary>
      | Choice2Of3 of Item2: 'T2
      /// <summary>Choice 3 of 3 choices</summary>
      | Choice3Of3 of Item3: 'T3
    
    /// <summary>Helper types for active patterns with 4 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpStructChoice`4")>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice<'T1,'T2,'T3,'T4> = 
      /// <summary>Choice 1 of 4 choices</summary>
      | Choice1Of4 of Item1: 'T1 
      /// <summary>Choice 2 of 4 choices</summary>
      | Choice2Of4 of Item2: 'T2
      /// <summary>Choice 3 of 4 choices</summary>
      | Choice3Of4 of Item3: 'T3
      /// <summary>Choice 4 of 4 choices</summary>
      | Choice4Of4 of Item4: 'T4
    
    /// <summary>Helper types for active patterns with 5 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpStructChoice`5")>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice<'T1,'T2,'T3,'T4,'T5> = 
      /// <summary>Choice 1 of 5 choices</summary>
      | Choice1Of5 of Item1: 'T1 
      /// <summary>Choice 2 of 5 choices</summary>
      | Choice2Of5 of Item2: 'T2
      /// <summary>Choice 3 of 5 choices</summary>
      | Choice3Of5 of Item3: 'T3
      /// <summary>Choice 4 of 5 choices</summary>
      | Choice4Of5 of Item4: 'T4
      /// <summary>Choice 5 of 5 choices</summary>
      | Choice5Of5 of Item5: 'T5
    
    /// <summary>Helper types for active patterns with 6 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpStructChoice`6")>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice<'T1,'T2,'T3,'T4,'T5,'T6> = 
      /// <summary>Choice 1 of 6 choices</summary>
      | Choice1Of6 of Item1: 'T1 
      /// <summary>Choice 2 of 6 choices</summary>
      | Choice2Of6 of Item2: 'T2
      /// <summary>Choice 3 of 6 choices</summary>
      | Choice3Of6 of Item3: 'T3
      /// <summary>Choice 4 of 6 choices</summary>
      | Choice4Of6 of Item4: 'T4
      /// <summary>Choice 5 of 6 choices</summary>
      | Choice5Of6 of Item5: 'T5
      /// <summary>Choice 6 of 6 choices</summary>
      | Choice6Of6 of Item6: 'T6
    
    /// <summary>Helper types for active patterns with 7 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpStructChoice`7")>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice<'T1,'T2,'T3,'T4,'T5,'T6,'T7> = 
      /// <summary>Choice 1 of 7 choices</summary>
      | Choice1Of7 of Item1: 'T1 
      /// <summary>Choice 2 of 7 choices</summary>
      | Choice2Of7 of Item2: 'T2
      /// <summary>Choice 3 of 7 choices</summary>
      | Choice3Of7 of Item3: 'T3
      /// <summary>Choice 4 of 7 choices</summary>
      | Choice4Of7 of Item4: 'T4
      /// <summary>Choice 5 of 7 choices</summary>
      | Choice5Of7 of Item5: 'T5
      /// <summary>Choice 6 of 7 choices</summary>
      | Choice6Of7 of Item6: 'T6
      /// <summary>Choice 7 of 7 choices</summary>
      | Choice7Of7 of Item7: 'T7

module StructUnionsWithConflictingConstructors = 

    [<StructuralEquality; StructuralComparison>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice = 
      | Choice1Of2 of Item1: double
      | Choice2Of2 of Item2: double
    
    [<StructuralEquality; StructuralComparison>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice3 = 
      | Choice1Of3 of Item1: double
      | Choice2Of3 of Item2: double
      | Choice3Of3 of Item3: double
    
    [<StructuralEquality; StructuralComparison>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice4 = 
      | Choice1Of4 of Item1: int
      | Choice2Of4 of Item2: int
      | Choice3Of4 of Item3: int
      | Choice4Of4 of Item4: float
    
    [<StructuralEquality; StructuralComparison>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice5 = 
      | Choice1Of5 of Item1: string
      | Choice2Of5 of Item2: string
      | Choice3Of5 of Item3: string
      | Choice4Of5 of Item4: string
      | Choice5Of5 of Item5: string
    
    [<StructuralEquality; StructuralComparison>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice6<'T1> = 
      | Choice1Of6 of Item1: 'T1 
      | Choice2Of6 of Item2: 'T1
      | Choice3Of6 of Item3: 'T1
      | Choice4Of6 of Item4: 'T1
      | Choice5Of6 of Item5: 'T1
      | Choice6Of6 of Item6: 'T1
    
    [<StructuralEquality; StructuralComparison>]
    [<RequireQualifiedAccess>]
    [<Struct>]
    type StructChoice7 = 
      | Choice1Of7 of Item1: byte 
      | Choice2Of7 of Item2: byte
      | Choice3Of7 of Item3: byte
      | Choice4Of7 of Item4: byte
      | Choice5Of7 of Item5: byte
      | Choice6Of7 of Item6: byte
      | Choice7Of7 of Item7: byte

module StructUnionMarshalingBug = 
    [<Struct>]
    type Msg0 = 
      | Zero of key :int

    [<Struct>]
    type Msg1 = 
      | One of name :string
      | Two of key :int

    [<Struct>]
    type Msg2 = 
      { name :string
        key :int
        tag :int }

    open System.Runtime.InteropServices

    let msg0 = Zero 42
    let size0 = Marshal.SizeOf(msg0)  
    check "clcejefdw" size0 (sizeof<int>)

    let msg1 = Two 42
    let size1 = Marshal.SizeOf(msg1)  
    check "clcejefdw2" size1 (sizeof<string> + 2*sizeof<int>) // this size may be bigger than expected

    let msg2 = { name = null; key = 42; tag=1 }
    let size2 = Marshal.SizeOf(msg2)  
    check "clceje" size2 (sizeof<string> + 2*sizeof<int>)

    // ... alternately ...
    let buffer = Marshal.AllocHGlobal(64) // HACK: just assumed a much larger size
    Marshal.StructureToPtr<Msg1>(msg1, buffer, false) 

module MatchBangSimple =
    type CardSuit = | Hearts | Diamonds | Clubs | Spades
    let fetchSuit () = async {
        // do something in order to not allow optimizing things away
        Async.Sleep 1
        return Some Hearts }
    
    async {
        match! fetchSuit () with
        | Some Hearts -> printfn "hearts"
        | Some Diamonds | Some Clubs | Some Spades | None -> report_failure "match! matched the wrong case" }
    |> Async.RunSynchronously

module MatchBangActivePattern =
    type CardSuit = | Hearts | Diamonds | Clubs | Spades

    let (|RedSuit|BlackSuit|) suit =
        match suit with
        | Hearts | Diamonds -> RedSuit
        | Clubs | Spades -> BlackSuit

    let fetchSuit () = async {
        Async.Sleep 1
        return Hearts }

    async {
        // make sure other syntactic elements nearby parse fine
        let! x = async.Return 42
        match! fetchSuit () with
        | RedSuit as suit -> printfn "%A suit is red" suit
        | BlackSuit as suit -> printfn "%A suit is black" suit }
    |> Async.RunSynchronously



(* check for failure else sign off "ok" *)


#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

