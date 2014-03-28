

(* Check that a F# library module has its initialization code run *)
(* before a field is accessed. *)
let x = ref 6
let addToRef (y:int) = x := !x + y; !x

(* Note that if the above works then just about anything will work *)
(* before a field is accessed. *)
let observableApp = (let r = ref 7 in  fun x -> (r := !r + x; !r))
let addToRef2 (y:int) = observableApp y


