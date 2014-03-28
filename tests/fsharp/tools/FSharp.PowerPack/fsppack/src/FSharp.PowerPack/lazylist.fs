namespace Microsoft.FSharp.Collections

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
    type LazyItem<'T> = Cons of 'T * LazyList<'T> | Empty
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


    let of_list l = ofList l

    let of_seq l = ofSeq l

    let of_array l = ofArray l

    let to_seq l = toSeq l

    let to_list l = toList l

    let to_array l = toArray l


