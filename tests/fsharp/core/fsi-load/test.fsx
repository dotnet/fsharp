//namespace global

//See https://github.com/Microsoft/visualfsharp/issues/2871
module MyModule =
    type MyType = A of string

module OtherModule = 
  open MyModule

  open System.Collections.Generic

  let foo () = [ for (k: KeyValuePair<string,MyType>) in []    -> () ]

  let testAnonRecordInFsi1 (x : {| X : int |}) = ()

  let testAnonRecordInFsi2 () : {| X : int |} = failwith "ok"

  let _ =
          stdout.WriteLine "Test Passed"
          printf "TEST PASSED OK" ;
          exit 0

