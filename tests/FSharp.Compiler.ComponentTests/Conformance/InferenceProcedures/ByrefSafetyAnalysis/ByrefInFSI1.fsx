// #Regression #Conformance #TypeInference #ByRef #ReqNOMT 
// Tests for legal usage of byrefs in VFSI
// Regression for FSB 5578, Trouble in FSI taking address of a top level mutable

let arr = Array.init 3 (fun x -> x);;

let repro (x:byref<int>)  = x <- 111; let temp = x in x <- 999; temp;;

let result = repro (&arr[0]);;

if result <> 111 then failwith "Failed: 1";;

// -------------------------------------------------

let Swap<'a> (left : 'a byref, right : 'a byref) : unit =    
    let temp = left    
    left  <- right    
    right <- temp;;

let mutable a = 1
let mutable b = 2;;

Swap (&a, &b);;

if a <> 2 || b <> 1 then failwith "Failed: 2";;
