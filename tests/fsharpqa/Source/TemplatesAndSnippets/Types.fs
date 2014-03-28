// Types

// Tuplues
let t1 = ()
let t2 = (1)
let t3 = (1,2)
let t4 = ((1,2),(3,4))

// Lists
let l1    = []     		// empty list
let l2    = [1]    		// list with 1 element
let l3    = [1;2]  		// list with 2 elements
let l4    = [[1;2];[3;4]]	// nested lists

// Options
let o1    = None						// None
let o2    = Some("cow")						// Some
let o3    = Some(None)						// option of option
let o4    = Some(Some(Some(Some(1))), Some(Some(Some(2))))	// nested

// Arrays
// Lists
let a1    = [||]     		// empty array
let a2    = [|1|]    		// array with 1 element
let a3    = [|1;2|]  		// array with 2 elements
let a4    = [|[|1;2|];[|3;4|]|]	// nested array

// User-defined records
type R1    = {}			// error!
type R2    = {I : int}
let r2 = { I = 10 }
type R3    = {F : int -> int }
let r3 = { F = fun x -> x + 1 }
type R4    = {I : float list}
let r4 = { I = [1.] }
type R5    = {F : int list -> int seq }
let r5    = {F = fun (x :: _) -> seq { 1..x } } // warning, but ok

// structs
[<Struct>]
type S1 = struct
            val v : int
          end

[<Struct>]
type S2() = struct
              [<DefaultValue>]
              val mutable  v : int
            end

// Unions

