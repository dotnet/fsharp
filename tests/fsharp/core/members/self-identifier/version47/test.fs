module Global

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

//--------------------------------------------------------------
// Test using "_" as the self identifier introduced in F# 4.7
type MyTypeWithUnderscoreIdentifier () =
    member _.MethodWithUnderscoreSelf() = true
    member __.MethodWithDoubleUnderscoreSelf() = true
    member _E.MethodWithUnderscoreESelf() = true

[<Struct>]
type MyStructWithUnderscoreIdentifier =
    member _.MethodWithUnderscoreSelf() = true
    member __.MethodWithDoubleUnderscoreSelf() = true
    member _E.MethodWithUnderscoreESelf() = true
    member private _.PrivateMethodWithUnderscoreSelf() = true
    member private __.PrivateMethodWithDoubleUnderscoreSelf() = true
    member private _E.PrivateMethodWithUnderscoreESelf() = true
    member inline _.InlineMethodWithUnderscoreSelf() = true
    member inline __.InlineMethodWithDoubleUnderscoreSelf() = true
    member inline _E.InlineMethodWithUnderscoreESelf() = true
    member inline private _.InlinePrivateMethodWithUnderscoreSelf() = true
    member inline private __.InlinePrivateMethodWithDoubleUnderscoreSelf() = true
    member inline private _E.InlinePrivateMethodWithUnderscoreESelf() = true

type MyClassWithUnderscoreIdentifier () =
    class
        member _.MethodWithUnderscoreSelf() = true
        member __.MethodWithDoubleUnderscoreSelf() = true
        member _E.MethodWithUnderscoreESelf() = true
        member private _.PrivateMethodWithUnderscoreSelf() = true
        member private __.PrivateMethodWithDoubleUnderscoreSelf() = true
        member private _E.PrivateMethodWithUnderscoreESelf() = true
        member inline _.InlineMethodWithUnderscoreSelf() = true
        member inline __.InlineMethodWithDoubleUnderscoreSelf() = true
        member inline _E.InlineMethodWithUnderscoreESelf() = true
        member inline private _.InlinePrivateMethodWithUnderscoreSelf() = true
        member inline private __.InlinePrivateMethodWithDoubleUnderscoreSelf() = true
        member inline private _E.InlinePrivateMethodWithUnderscoreESelf() = true
    end

type MyStructTypeWithUnderscoreIdentifier =
    struct
        member _.MethodWithUnderscoreSelf() = true
        member __.MethodWithDoubleUnderscoreSelf() = true
        member _E.MethodWithUnderscoreESelf() = true
        member private _.PrivateMethodWithUnderscoreSelf() = true
        member private __.PrivateMethodWithDoubleUnderscoreSelf() = true
        member private _E.PrivateMethodWithUnderscoreESelf() = true
        member inline _.InlineMethodWithUnderscoreSelf() = true
        member inline __.InlineMethodWithDoubleUnderscoreSelf() = true
        member inline _E.InlineMethodWithUnderscoreESelf() = true
        member inline private _.InlinePrivateMethodWithUnderscoreSelf() = true
        member inline private __.InlinePrivateMethodWithDoubleUnderscoreSelf() = true
        member inline private _E.InlinePrivateMethodWithUnderscoreESelf() = true
    end


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

