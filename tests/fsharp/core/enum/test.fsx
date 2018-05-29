// #Conformance #Regression 
#if TESTS_AS_APP
module Core_enum
#endif

open System.Reflection
let failures = ref []

let report_failure (s : string) = 
    stderr.Write" Failed: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " Passed"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)

type internal internalEnum = Red=0 | Yellow=1 | Blue=2
type public publicEnum = Red=0 | Yellow=1 | Blue=2

module enum =

    let AreCasesPublic (t:System.Type) =
        let bindingFlags = BindingFlags.Static ||| BindingFlags.Public
        try
            let red = t.GetField("Red", bindingFlags)
            let yellow = t.GetField("Yellow", bindingFlags)
            let blue = t.GetField("Blue", bindingFlags)
            let value__ = t.GetField("value__", bindingFlags ||| BindingFlags.Instance)

            if isNull red || not (red.IsPublic) then failwith (sprintf "Type: %s) - Red is not public.  All enum cases should always be public" t.FullName)
            if isNull yellow || not (yellow.IsPublic) then failwith (sprintf "Type: %s) - Yellow is not public.  All enum cases should always be public" t.FullName)
            if isNull blue || not (blue.IsPublic) then failwith (sprintf "Type: %s) - Blue is not public.  All enum cases should always be public" t.FullName)
            if isNull value__ || not (value__.IsPublic) then failwith (sprintf "Type: %s) - value__ is not public.  value__ should always be public" t.FullName)
            true
        with _ -> false

    do check "publicEnum" (AreCasesPublic typeof<publicEnum>) true
    do check "internalEnum" (AreCasesPublic typeof<internalEnum>) true


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

