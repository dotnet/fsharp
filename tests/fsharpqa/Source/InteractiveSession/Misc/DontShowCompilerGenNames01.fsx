// #Regression #NoMT #FSI 

// Regression test for FSHARP1.0:2549
// See also CL:14579
//<Expect status="success">type T =</Expects>
//<Expect status="success">class</Expects>
//<Expect status="success">member M1 : x:int \* y:string -> \('a -> unit\)</Expects>
//<Expect status="success">member M2 : \(int \* string\) -> \('a -> unit\)</Expects>
//<Expect status="success">exception ExnType of int \* string</Expects>
//<Expect status="success">type DiscUnion = \| DataTag of int \* string</Expects>
//<Expect status="success">val f : x:int -> y:int -> int</Expects>


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
