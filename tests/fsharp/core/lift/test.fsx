// #Conformance #Regression 
#if TESTS_AS_APP
module Core_lift
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

let check s b1 b2 = test s (b1 = b2)

(* one lifted binding, one lifted expression *)
let test2924 () = 
  let constt = [2;3;4]  (* closed *) in 
  List.map (fun i -> i+2)  (* closed *) constt

let _ = if test2924 () <> [4;5;6]  then report_failure "iniiu9"

(* two lifted expressions *)
let test2925 () = 
  List.map (fun i -> i + 6)  (* closed *) [2;3;4]  (* closed *)

let _ = if test2925 () <> [8;9;10] then report_failure "iniiu9h39"

(* one lifted binding, one lifted expression *)
let test2926 () = 
  let f = fun i -> i+i+i  (* closed *) in 
  List.map f [2;3;4]  (* closed *)

let _ = if test2926 () <> [6;9;12] then report_failure "iui2iu284"

(* one lifted binding, one lifted nested binding, one lifted expression *)
let test2946 () = 
  let f = fun i -> i+i+i  (* closed *) in 
  List.map f ((let g = (fun j -> j+j) (* closed *) in  g 1):: [3;4;])  (* closed *)

let _ = if test2946 () <> [6;9;12] then report_failure "72uiu284"

(* make sure references don't get lifted *)
let test2947 () = 
  let f () = let x = ref 1 (* not closed *) in (fun i -> x := !x + i; !x  (* not closed *)) in 
  if (f () 3 <> 4) then report_failure "jd23er84";
  (* this would fail if we had listed the "ref" expression *)
  if (f () 3 <> 4) then report_failure "jdbtr284";
  (* these check we don't do any silly optimizations in the presence of side effects *)
  let f2 = f () in 
  if (f2 3 <> 4) then report_failure "jd2dvr4";
  if (f2 3 <> 7) then report_failure "jd232d"

let _ = test2947()


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

