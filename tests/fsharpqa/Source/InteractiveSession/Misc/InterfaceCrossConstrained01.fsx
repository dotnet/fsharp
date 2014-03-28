// #NoMT #FSI 
// Interesting and odd testcases
// Interfaces cross-constrained via method gps

//<Expects status="success">type IA =</Expects>
//<Expects status="success">  interface</Expects>
//<Expects status="success">    abstract member M : #IB -> int</Expects>
//<Expects status="success">  end</Expects>
//<Expects status="success">and IB =</Expects>
//<Expects status="success">  interface</Expects>
//<Expects status="success">    abstract member M : #IA -> int</Expects>
//<Expects status="success">  end</Expects>

type IA = 
    abstract M : 'a -> int when 'a :> IB 
and  IB = 
    abstract M : 'b -> int when 'b :> IA

;;
#q;;
