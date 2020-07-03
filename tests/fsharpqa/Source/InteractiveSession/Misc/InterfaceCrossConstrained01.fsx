// #NoMT #FSI 
// Interesting and odd testcases
// Interfaces cross-constrained via method gps

//<Expects status="success">type IA =</Expects>
//<Expects status="success">  abstract member M<'a \(requires 'a :> IB\)> : #IB -> int</Expects>
//<Expects status="success">and IB =</Expects>
//<Expects status="success">  abstract member M<'b \(requires 'b :> IA\)> : #IA -> int</Expects>

type IA = 
    abstract M : 'a -> int when 'a :> IB 
and  IB = 
    abstract M : 'b -> int when 'b :> IA

;;
#q;;
