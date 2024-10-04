//<Expects id="FS0058" status="warning" span="(78,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(77:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(84,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(83:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(84,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(83:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(89,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(88:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(89,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(88:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(92,5)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(91:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(92,5)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(91:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(96,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(95:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(96,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(95:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(101,2)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(100:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(101,2)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(100:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(105,6)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(104:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(105,6)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(104:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(60,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(59:19\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(60,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(59:19\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(64,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(63:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(64,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(63:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(70,3)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(69:23\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(70,3)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(69:23\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(78,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(77:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(78,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(77:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(84,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(83:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(84,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(83:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(89,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(88:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(89,7)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(88:25\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(92,5)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(91:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(92,5)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(91:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(96,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(95:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(96,9)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(95:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(101,2)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(100:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(101,2)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(100:20\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(105,6)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(104:21\). Try indenting this further.*"</Expects>
//<Expects id="FS0058" status="warning" span="(105,6)">"Unexpected syntax or possible incorrect indentation: this token is offside of context started at position \(104:21\). Try indenting this further.*"</Expects>

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
        b:int, c:int, // 4.6 warning: 4.7 no warning
        d:int, e:int,
        f:int) =
    static member M(a:int,
        b:int, c:int, // 4.6 warning: 4.7 no warning
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
