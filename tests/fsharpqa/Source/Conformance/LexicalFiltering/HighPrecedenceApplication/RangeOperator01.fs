// #Regression #Conformance #LexFilter #Precedence #ReqNOMT 
// Regression test for FSHARP1.0:4161 - Error when trying to lex/parse a range involving biggest negative number
//<Expects status="success"></Expects>

module TestModule

let _ = {-128y..1y}
let _ = {-128y.. 1y}
let _ = {-128y ..1y}
let _ = {-128y .. 1y}

let _ = {-32768s..1s}
let _ = {-32768s.. 1s}
let _ = {-32768s ..1s}
let _ = {-32768s .. 1s}

let _ = {-2147483648..1}
let _ = {-2147483648.. 1}
let _ = {-2147483648 ..1}
let _ = {-2147483648 .. 1}

let _ = {-9223372036854775808L..1L}
let _ = {-9223372036854775808L.. 1L}
let _ = {-9223372036854775808L ..1L}
let _ = {-9223372036854775808L .. 1L}

