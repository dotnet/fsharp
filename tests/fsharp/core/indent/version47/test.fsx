//<Expects id="FS0058" status="warning" span="(42,3)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(41:5\).*</Expects>
//<Expects id="FS0058" status="warning" span="(42,3)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(41:5\).*</Expects>
//<Expects id="FS0058" status="warning" span="(50,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(49:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(50,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(49:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(61,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(60:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(61,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(60:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(42,3)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(41:5\).*</Expects>
//<Expects id="FS0058" status="warning" span="(42,3)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(41:5\).*</Expects>
//<Expects id="FS0058" status="warning" span="(50,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(49:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(50,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(49:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(61,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(60:9\).*</Expects>
//<Expects id="FS0058" status="warning" span="(61,7)">Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(60:9\).*</Expects>


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

type OffsideCheck(a:int,
        b:int, c:int, // no warning
        d:int, e:int,
        f:int) =
    static member M(a:int,
        b:int, c:int, // no warning
        d:int, e:int,
        f:int) = 1

module M =
    type OffsideCheck(a:int,
  b:int, c:int, // 4.6 warning: 4.7 warning
  d:int, e:int,
  f:int) =
        class end

module M2 =
    type OffsideCheck() =
        static member M(a:int,
      b:int, c:int, // 4.6 warning: 4.7 warning
      d:int, e:int,
      f:int) = 1

type C() =
    static member P with get() =
      1 // 4.6 warning: 4.7 no warning

module M3 =
    type C() =
        static member P with get() =
      1 // warning

type OffsideCheck2(a:int,
    b:int, c:int, // 4.6 warning: 4.7 no warning
    d:int, e:int,
    f:int) =
    static member M(a:int,
        b:int, c:int, // 4.6 warning: 4.7 no warning
        d:int, e:int,
        f:int) = 1

type OffsideCheck3(a:int,
 b:int, c:int, // no warning
 d:int, e:int,
 f:int) =
    static member M(a:int,
     b:int, c:int, // 4.6 warning: 4.7 no warning
     d:int, e:int,
     f:int) = 1

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
