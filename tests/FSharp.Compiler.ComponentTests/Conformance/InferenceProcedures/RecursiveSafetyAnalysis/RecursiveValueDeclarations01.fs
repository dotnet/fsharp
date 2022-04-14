// #Regression #Conformance #TypeInference #Recursion 
// FSharp1.0:4821
// Copying of "when" bindings is losing a recursive fixup point (was: ICE on code with mutually recursive functions used in pattern matching guards?)

#light

type Foo = 
    | CSt of int*int*Foo
    | NCst of int

let lunify_cty x y z = 1

let instances x y = []

(* remove any assumed predicates *)
let rec discharge_predicates pty tenv =
 match pty with
 | CSt (x, ty, pty') when can_discharge x ty tenv -> discharge_predicates pty' tenv
 | CSt (x, ty, pty') -> CSt (x, ty, discharge_predicates pty' tenv)
 | NCst cty -> NCst cty
and
 lunifies t1 t2 = (try let _ = lunify_cty t1 t2 [] in true with _ -> false)
and
 can_discharge x ty tenv =
  List.exists (fun ty' -> lunifies ty ty') (instances x tenv)
  

let rec f x tenv =
 match x with
 | _ when g tenv -> 1
 | _ -> 2
and g tenv = true

