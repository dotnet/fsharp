// #Regression #NoMT #FSI 

// Regression test for FSHARP1.0:2549
// See also CL:14579
//<Expects status="success">type T =</Expects>
//<Expects status="success">member M1: x: int \* y: string -> \('a -> unit\)</Expects>
//<Expects status="success">member M2: \(int \* string\) -> \('a -> unit\)</Expects>
//<Expects status="success">exception ExnType of int \* string</Expects>
//<Expects status="success">type DiscUnion = \| DataTag of int \* string</Expects>
//<Expects status="success">val f: x: int -> y: int -> int</Expects>


type T = 
    member z.M1 ((x : int), (y: string)) = ignore
    member z.M2 ((x, y) : int * string) = ignore          // we used to display "member M2 : _arg11:(int * string) -> ('a0 -> unit)" - yikes!
;;



exception ExnType of int * string
;;

type DiscUnion = | DataTag of int * string
;;


let f x y = x + y
;;

exit 0
;;
