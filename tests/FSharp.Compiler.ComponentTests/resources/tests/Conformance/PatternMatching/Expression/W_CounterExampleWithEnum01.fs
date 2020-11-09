// #Regression #Conformance #PatternMatching 
// Regression test for DevDiv:198999 ("Warning messages for incomplete matches involving enum types are wrong")
//<Expects status="warning" span="(14,10-14,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '"a"' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(18,10-18,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '0\.0' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(22,10-22,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '' '' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(26,10-26,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '1y' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(30,10-30,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '\[_;_;_\]' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(34,10-34,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '\[|_; 1|\]' may indicate a case not covered by the pattern\(s\)\.$</Expects>

module M

type T = | X = 0 | Y = 1
 
let f1 = function
         | "X" -> T.X
         | "Y" -> T.Y

let f2 = function
         | 1. -> T.X
         | 2. -> T.Y

let f3 = function
         | 'a' -> T.X
         | 'b' -> T.Y

let f4 = function
         | 0y -> T.X
         | 2y -> T.Y

let f5 = function
         | [1 ; 2] -> T.X
         | [_] -> T.Y

let f6 = function
         | [|1 ; 2|] -> T.X
         | [|_;0|] -> T.Y
