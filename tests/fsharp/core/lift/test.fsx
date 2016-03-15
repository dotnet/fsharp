// #Conformance #Regression 
#if Portable
module Core_lift
#endif

let failures = ref false
let report_failure s  = 
  stderr.WriteLine ("NO: test "^s^" failed"); failures := true


#if NetCore
#else
let argv = System.Environment.GetCommandLineArgs() 
let SetCulture() = 
  if argv.Length > 2 && argv.[1] = "--culture" then  begin
    let cultureString = argv.[2] in 
    let culture = new System.Globalization.CultureInfo(cultureString) in 
    stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
    System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
  end 
  
do SetCulture()    
#endif

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

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)