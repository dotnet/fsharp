// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5825

//<Expects status="succesS">type I =</Expects>
//<Expects status="succesS">  abstract m: unit</Expects>
//<Expects status="succesS">type C =</Expects>
//<Expects status="succesS">  interface I</Expects>
//<Expects status="succesS">  new: unit -> C</Expects>
//<Expects status="succesS">val f: c: #C -> unit</Expects>

type I = 
    abstract member m : unit 
type C() = 
    interface I with 
        member this.m = () 
let f (c : #C) = ();;

0 |> exit;;
