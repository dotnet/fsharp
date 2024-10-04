// #Conformance #Regression 
#if TESTS_AS_APP
module Core_fileorder
#endif


open Ploeh.Weird.Repro

let failures = ref false
let report_failure s  = 
  stderr.WriteLine ("NO: test "+s+" failed"); failures := true


let b = {
    Value = 42
    Text = "Ploeh" }
    
let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)