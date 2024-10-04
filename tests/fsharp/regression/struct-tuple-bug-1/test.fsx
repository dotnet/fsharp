// #Regression 
let failures = ref false
let report_failure msg = 
  printfn "%A" msg
  failures := true

try
    let struct (_x,_y) = struct (1,2)
    ()
with ex -> report_failure (ex.ToString())

let _ =
    if !failures then 
        printfn "Test Failed"
        exit 1
    else
        printf "TEST PASSED OK" 
        printf "TEST PASSED OK" ;
        exit 0
()