//<Expects status="success" />
#if TESTS_AS_APP
module TestSuite_FSharpCore_nameof_47
#endif

//<Expects status="success" />
#load @"..\testcases.fsx"
open TestSuite_FSharpCore_nameof

#if TESTS_AS_APP
let RUN() = 
  match !failures with 
  | [] -> stdout.WriteLine "Test Passed"
  | _ ->  stdout.WriteLine "Test Failed"
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

