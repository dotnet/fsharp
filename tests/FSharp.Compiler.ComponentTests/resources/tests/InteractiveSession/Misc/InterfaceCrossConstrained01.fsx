// #NoMT #FSI 
// Interesting and odd testcases
// Interfaces cross-constrained via method gps

//<Expects status="success">type IA =</Expects>
//<Expects status="success">  abstract M: #IB -> int</Expects>
//<Expects status="success">and IB =</Expects>
//<Expects status="success">  abstract M: #IA -> int</Expects>

type IA = 
    abstract M : 'a -> int when 'a :> IB 
and  IB = 
    abstract M : 'b -> int when 'b :> IA

;;
#q;;
