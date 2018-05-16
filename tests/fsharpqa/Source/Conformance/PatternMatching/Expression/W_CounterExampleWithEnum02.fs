// #Regression #Conformance #PatternMatching 
// Regression test for DevDiv:198999 ("Warning messages for incomplete matches involving enum types are wrong")
//<Expects status="warning" span="(14,10-14,18)" id="FS0104">Enums may take values outside known cases\. For example, the value 'enum<T> \(2\)' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(18,10-18,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(21,10-21,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(24,10-24,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(27,10-27,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects status="warning" span="(30,10-30,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$</Expects>

module M

type T = | X = 0 | Y = 1
 
let g1 = function
         | T.X -> "X" 
         | T.Y -> "y" 

let g2 = function
         | T.X -> 1.

let g3 = function
         | T.X -> 'a'

let g4 = function
         | T.X -> 2y

let g5 = function
         | T.X -> [1 ; 2]

let g6 = function
         | T.X -> [|1 ; 2|] 

