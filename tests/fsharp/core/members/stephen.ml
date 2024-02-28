
(* Sample F# Source File  *)

open System


type 'a tree = 
  | Leaf of 'a
  | Node of 'a tree list
    
// BUG: 'a must be specified, but we don't check for it
//  type tree

let rec add t1 t2 = 
  match t1,t2 with 
  | Leaf a,Leaf b -> Node [Leaf a; Leaf b] 
  | Node n1, Node n2 -> Node (List.map2 add n1 n2)
  | _ -> failwith "add"

(*
type 't IComparable = 
    interface
      abstract CompareTo: 't -> int
    end

type class Comparison<'t> = 
    when 't :> interface IComparable<'t>

type class RightAddition<'a,'b> =
    when 'a :> static member (+) : ('a,'b) -> 'a
    
type class Num<'a,'b> =
    when 'a :> static member (+) : ('a,'b) -> 'a
    and  'a :> static member ( * ) : ('a,'b) -> 'a
    and  'a :> static member Zero : 'a
    and  'a :> static member One : 'a

type class ToString<'a> =
    when 'a :> member ToString: unit -> string

type class assert RightAddition<DateTime,TimeSpan>

interface IComparable<Self> = 
    T : member CompareTo: T -> T -> int
end

interface INum<Self,Other> = 
    Self :> member Add : Other -> Self
    Self :> member Multiply : Self -> Self
    Self :> static member Zero : Self
end
*)

type 'a tree
  with
    //static member (+) x y = add x y
    static member op_Addition(x,y) = add x y
    //static member op_Concatenation(x,y) = add x y
    static member (++) x y = add x y
    
    // member x.Add(y) = x + y
  end

(*
type 't IComparable = 
    interface
      abstract CompareTo: 't -> int
    end

*)


type class Comparison<'t> = 
    when 't :> interface IComparable<'t>

type class RightAddition<'a,'b> =
    when 'a :> static member (+) : ('a,'b) -> 'a
    

(*
type class Displayable<'a> =
    when val toString : 'a -> string
    
    
instance Displayable<System.Int32> with toString(i) = string_of_int i
instance Displayable<List<'a> > with toString(i) = string_of_int i
instance Displayable<List<int> > with toString(i) = string_of_int i
instance Displayable<'a> => GUIable<'a> with toString(i) = string_of_int i

type class Displayable<'a,'b> =
    when 'b :> static member toString : 'a -> string

type ShowList<'a,'b> = class static member toString(x: List<'a>  )  when Displayable<'a,'b>  = fold ('b.) .... x end
type ShowIntList = class static member toString(x: List<int> ) = fold .... x end


assert Displayable<List<'a>,ShowList<'a>>
assert Displayable<List<int>,ShowIntList>

let dup(x:'a when Displayable<'a,'b>) = 'b.toString(x)

do dup@ShowIntList [3]

assert Displayable<List<int>,ShowIntList>



type class Displayable<'a> =
    when member ToString : unit -> string

type List<'a> with static member x.ToString() = ... end
type List<int> with member x.ToString() = ... end
type List<int> with member x.ToString() = ... end

*)

val show : Displayable<'a> => 'a -> string
let show (x:'a) = toString(a)
    
do show(3)
    
type class Num<'a,'b> =
    when RightAddition<'a,'b>
    and  'a :> static member ( * ) : ('a,'b) -> 'a
    and  'a :> static member Zero : 'a
    and  'a :> static member One : 'a

type class ToString<'a> =
    when 'a :> member ToString: unit -> string

type class assert RightAddition<DateTime,TimeSpan>



//instance Num<tree<'a> > with
//  (+) x y = add x y 
//end


//BUG: empty object initializers not being permitted:
(*
//BUG: comments aren't being colored in red!!!!
module EmptyClassTest = begin
  type C =
    class
      new() = { }
    end
end
*)


(*
// BUG:
type ('a,'b) func =
  class
    //static member (+) x y = add x y
    abstract Invoke : 'a -> 'b option
    //static member op_Concatenation(x,y) = add x y
    static member FF (f: ('a,'b) func)  (g: ('b,'c) func)  = 
      {new func<'a,'c>() with Invoke(x) = match f.Invoke(x) with None -> None | Some b -> g.Invoke(b)}
    // BUG: "inherit obj()" should never be needed
    // BUG: inherit should never be needed for base classes that have a default constructor and no other
    // constructors
    new() = { inherit obj() }
  end

let konst a = {new func<_,_>() with Invoke(x) = Some a}

let morph f = {new func<_,_>() with Invoke(x) = Some (f x)}

let something = func.FF (morph (fun x -> x * x)) (morph (fun x -> x + x))
*)


(* SAME BUG: 
type ('a,'b) func =
  class
    //static member (+) x y = add x y
    val numCalls: int ref
    val impl: 'a -> 'b option
    member x.Invoke(y) = incr x.numCalls; x.impl(y)
    //static member op_Concatenation(x,y) = add x y
    // BUG:
    // new(f) = { inherit obj(); impl=f }
    new(f) = { inherit obj(); numCalls = ref 0; impl=f }
    static member FF (f: ('a,'b) func)  (g: ('b,'c) func)  = 
      new func<'a,'c>(fun x -> match f.Invoke(x) with None -> None | Some b -> g.Invoke(b))
  end
*)

module FuncTest4  = begin

// QUERY: can "inline" be inferred, perhaps with an optional warning
// QUERY: make it clear that "inline" is only for semantic use
// QUERY: can we give a warning if "inline" doesn't allow more type variables to be generalized

let inline (++) (x : $a) (y:$b) = $a.op_Concatenation(x,y)

// QUERY: List and generalized comprehensions

type func =
  class
    //static member (+) x y = add x y
    val numCalls: int ref
    val impl: int -> int option
    member x.Invoke(y) = incr x.numCalls; x.impl(y)
    //static member op_Concatenation(x,y) = add x y
    // BUG:
    // new(f) = { inherit obj(); impl=f }
    new(f) = {  numCalls = ref 0; impl=f }
    static member op_Concatenation ((f: func), (g: func))  = 
      new func(fun x -> match f.Invoke(x) with None -> None | Some b -> g.Invoke(b))
  end

let konst a = new func(fun x -> Some a)

let morph f = new func(fun x -> Some (f x))

let something = (morph (fun x -> x * x)) ++ (morph (fun x -> x + x))

end

//let (>>) f g x = g(f(x))


//let inline (++) (x : $a) (y:$b) = 
//  $a.op_Concatenation(x,y)


  
// BUG: Use of operator overloading on polymorphic types is leading to all type variables
// in those types being marked as compile-time polymorphism
// let f (x: 'a tree) = x + x

let f (x: int tree) = x + x

let f2 (x: int tree) = x ++ x

type 'a tree
  with
    member x.Size = 
      //match x with Leaf x -> 1 | Node l -> List.fold (fun acc (x:'a tree) -> x.Size + acc) 0 l
      let rec size acc x = match x with Leaf x -> 1+acc | Node l -> List.fold size acc l in
      size 0 x 
    member x.Map(f) = 
      let rec map f = function Leaf x -> Leaf (f x) | Node l -> Node (List.map (map f) l) in
      map f x
  end
  

let t1 = Node [Leaf 1;Leaf 2;Leaf 3]

let t1size = t1.Size
let t2 = t1.Map(fun i -> string_of_int i)



