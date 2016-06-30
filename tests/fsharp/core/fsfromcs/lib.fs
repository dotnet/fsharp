module Lib
(* An F# library which we try to access from C# *)

type Recd1 = { recd1field1: int }
type Recd2 = { recd2field1: int; recd2field2: string }
type 'a Recd3 = { recd3field1: int; recd3field2: 'a; mutable recd3field3: 'a Recd3 }

(* Recd2 with fields declared in other order *)
type RevRecd2 = {  recd2field2: string; rrecd2field1: int; }

type One = One
type Int = Int of int
type IntPair = IntPair of int * int
type IntPear = | IntPear : Fst: int * Snd: int -> IntPear
type BigUnion = 
  | A1 of int
  | A2 of int
  | A3 of int
  | A4 of int
  | A5 of int
  | A6 of int
  | A7 of int
  | A8 of int
  | A9 of int

type BigEnum = 
  | E1 
  | E2 
  | E3 
  | E4 
  | E5 
  | E6 
  | E7 
  | E8 
  | E9 

type Bool = True | False
type IntOption = Nothing | Something : Item: int  -> IntOption
type OptionalInt = SOME of int | NONE
type Index = Index_A of int | Index_B of int

type GenericUnion<'T,'U> = 
    | Nothing 
    | Something of 'T * 'U
    | SomethingElse of 'T 
    | SomethingElseAgain of 'T 

type Discr3_0_0_0 = Discr3_0_0_0_A | Discr3_0_0_0_B | Discr3_0_0_0_C
type Discr3_0_1_0 = Discr3_0_1_0_A | Discr3_0_1_0_B of int | Discr3_0_0_0_C
type Discr3_1_0_0 = Discr3_1_0_0_A of int | Discr3_1_0_0_B | Discr3_0_0_0_C
type Discr3_1_1_0 = Discr3_1_1_0_A of int | Discr3_1_1_0_B of int | Discr3_0_0_0_C
type Discr3_0_0_1 = Discr3_0_0_0_A | Discr3_0_0_0_B | Discr3_0_0_0_C of string
type Discr3_0_1_1 = Discr3_0_1_0_A | Discr3_0_1_0_B of int | Discr3_0_0_0_C of string
type Discr3_1_0_1 = Discr3_1_0_0_A of int | Discr3_1_0_0_B | Discr3_0_0_0_C of string
type Discr3_1_1_1 = Discr3_1_1_0_A of int | Discr3_1_1_0_B of int | Discr3_0_0_0_C of string

(* Toplevel functions *)
let f_1 x = x+1
let f_1_1 x y = x+y
let f_1_1_1 x y z = x+y+z
let f_1_1_1_1 x1 x2 x3 x4 = x1+x2+x3+x4
let f_1_1_1_1_1 x1 x2 x3 x4 x5 = x1+x2+x3+x4+x5

(* Function returning a function *)
let f_1_effect_1 x = let x = ref 1 in fun y -> !x+y+1

(* Tuple value *)
let tup2 = (2,3)
let tup3 = (2,3,4)
let tup4 = (2,3,4,5)




