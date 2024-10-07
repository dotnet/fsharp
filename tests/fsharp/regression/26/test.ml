// #Regression 

let fail m = System.Console.Error.WriteLine (m:string); exit 99

let _ = if (compare [| 1 |] [| 2 |] <> -1) then fail "Test Failed (cew90u98f)"
let _ = if (compare [| 2 |] [| 1 |] <> 1) then fail "Test Failed (d23v902)"
let _ = if (compare [| 2 |] [| 2 |] <> 0) then fail "Test Failed (few0vwrlk)"
let _ = if (compare [| |] [| 2 |] <> -1) then fail "Test Failed (cwe0vr9)"
let _ = if (compare [| 2 |] [| |] <> 1) then fail "Test Failed (fwelkm23)"
let _ = if (compare [| |] [| |] <> 0) then fail "Test Failed (cwlvero02)"

let _ = if (compare [| "1" |] [| "2" |] <> -1) then fail "Test Failed (scew90u98f)"
let _ = if (compare [| "2" |] [| "1" |] <> 1) then fail "Test Failed (sd23v902)"
let _ = if (compare [| "2" |] [| "2" |] <> 0) then fail "Test Failed (sfew0vwrlk)"
let _ = if (compare [| |] [| "2" |] <> -1) then fail "Test Failed (scwe0vr9)"
let _ = if (compare [| "2" |] [| |] <> 1) then fail "Test Failed (sfwelkm23)"
let _ = if (compare [| |] [| |] <> 0) then fail "Test Failed (scwlvero02)"

type a = A | B
let _ = if (compare [| A |] [| B |] <> -1) then fail "Test Failed (abcew90u98f)"
let _ = if (compare [| B |] [| A |] <> 1) then fail "Test Failed (abd23v902)"
let _ = if (compare [| B |] [| B |] <> 0) then fail "Test Failed (abfew0vwrlk)"
let _ = if (compare [| |] [| B |] <> -1) then fail "Test Failed (abcwe0vr9)"
let _ = if (compare [| B |] [| |] <> 1) then fail "Test Failed (abfwelkm23)"
let _ = if (compare [| |] [| |] <> 0) then fail "Test Failed (abcwlvero02)"

let _ = System.Console.Error.WriteLine "Test Passed"

do (System.Console.Out.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)

