// #Conformance 
#if TESTS_AS_APP
module Core_nested
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

let f () = 3

let wher = ref []
let spot x = wher := !wher @ [x]; stderr.WriteLine(x:string)
do spot "Initialized before X1 OK"

module X1 = begin 
  type x = X | Y
  let y = 3

  do spot "Initialized X1 OK";

end


module X2 = begin 
  type x = X | Y
  let x = 3
  let y () = X
  let z = x + (match y() with X -> 4 | Y -> 5)
  do spot "Initialized X2 OK";
end


module X3 = begin 
  let y = X2.X
  do spot "Initialized X3 OK";
end


do spot "Initialized after X3 OK"

let _ = X2.z + X2.x + X1.y 

do test "uyf78" (!wher = [ "Initialized before X1 OK";
                           "Initialized X1 OK";
                           "Initialized X2 OK";
                           "Initialized X3 OK";
                           "Initialized after X3 OK" ])


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

