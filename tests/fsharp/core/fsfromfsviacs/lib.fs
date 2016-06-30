module Lib
(* An F# library which we use in a C# library, where we in turn use both the F# component and the C# library together from F# *)

type recd1 = { recd1field1: int }
type recd2 = { recd2field1: int; recd2field2: string }
type 'a recd3 = { recd3field1: int; recd3field2: 'a; mutable recd3field3: 'a recd3 }

(* recd2 with fields declared in other order *)
type rrecd2 = {  rrecd2field2: string; rrecd2field1: int; }

type discr1_0 = Discr1_0_A
type discr1_1 = Discr1_1_A of int
type discr1_2 = Discr1_2_A of int * int

type discr2_0_0 = Discr2_0_0_A | Discr2_0_0_B
type discr2_0_1 = Discr2_0_1_A | Discr2_0_1_B of int
type discr2_1_0 = Discr2_1_0_A of int | Discr2_1_0_B
type discr2_1_1 = Discr2_1_1_A of int | Discr2_1_1_B of int

type discr3_0_0_0 = Discr3_0_0_0_A | Discr3_0_0_0_B | Discr3_0_0_0_C
type discr3_0_1_0 = Discr3_0_1_0_A | Discr3_0_1_0_B of int | Discr3_0_0_0_C
type discr3_1_0_0 = Discr3_1_0_0_A of int | Discr3_1_0_0_B | Discr3_0_0_0_C
type discr3_1_1_0 = Discr3_1_1_0_A of int | Discr3_1_1_0_B of int | Discr3_0_0_0_C
type discr3_0_0_1 = Discr3_0_0_0_A | Discr3_0_0_0_B | Discr3_0_0_0_C of string
type discr3_0_1_1 = Discr3_0_1_0_A | Discr3_0_1_0_B of int | Discr3_0_0_0_C of string
type discr3_1_0_1 = Discr3_1_0_0_A of int | Discr3_1_0_0_B | Discr3_0_0_0_C of string
type discr3_1_1_1 = Discr3_1_1_0_A of int | Discr3_1_1_0_B of int | Discr3_0_0_0_C of string

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





module StructUnionsTests =

    [<Struct>]
    type U0 = U0

    let f0 x = match x with U0 -> 1

    let v0 = f0 U0

    [<Struct>]
    type U1 = U1 of int

    let f1 x = match x with U1(x) -> x + x

    let v1 = f1 (U1(3))

    [<Struct>]
    type U2 = U2 of int * int

    let f2 x = match x with U2(x,y) -> x + y

    let v2 = f2 (U2(3,4))

    [<Struct>]
    type Ok3 = Ok3 of int * Ok3 list

/// Nesting structs inside struct unions means taking the address of things during pattern matching
module NestedStructUnionsTests =

    [<Struct>]
    type U1 = U1 of System.DateTime * string

    [<Struct>]
    type U2 = U2 of U1 * U1


    let testPattern1(u2:U2) = 
        match u2 with
        | U2(u1a,u1b) ->
            match u1a, u1b with 
            | U1(dt1,s1), U1(dt2,s2)  -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern2(u2:U2) = 
        match u2 with
        | U2(U1(dt1,s1),U1(dt2,s2)) -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern3(u2:U2) = 
        match u2 with
        | U2(U1(dt1,"a"),U1(dt2,"b")) -> (dt1 = dt2) 


    let testPattern1mut(u2:U2) = 
        let mutable u2 = u2
        match u2 with
        | U2(u1a,u1b) ->
            match u1a, u1b with 
            | U1(dt1,s1), U1(dt2,s2)  -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern2mut(u2:U2) = 
        let mutable u2 = u2
        match u2 with
        | U2(U1(dt1,s1),U1(dt2,s2)) -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern3mut(u2:U2) = 
        let mutable u2 = u2
        match u2 with
        | U2(U1(dt1,"a"),U1(dt2,"b")) -> (dt1 = dt2) 


