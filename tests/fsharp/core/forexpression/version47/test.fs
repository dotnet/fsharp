//<Expects status="success">usingWildcards expected to produce sum of 4 : sum='4'</Expects>
#if TESTS_AS_APP
module Core_forexpression_47
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

// usingWildcard counts using a wildcard in a for loop
let usingWildcard () =
    let mutable sum = 0
    for _ = 0 to count do
        sum <- sum + 1

    printfn "usingWildcards expected to produce sum of 4 : sum='%d'"sum

do test "wildCard"          (4 = usingWildcard () )

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

