// #Regression #NoMT #FSI #RequiresENU 
// Regression test for DEV10#832789 
//<Expects status="success">type IA2<'a when 'a :> IB2<'a> and 'a :> IA2<'a>> =</Expects>
//<Expects status="success">  abstract M: int</Expects>
//<Expects status="success">and IB2<'b when 'b :> IA2<'b> and 'b :> IB2<'b>> =</Expects>
//<Expects status="success">  abstract M: int</Expects>

type IA2<'a when 'a :> IB2<'a> and 'a :> IA2<'a>> = 
    abstract M : int
and  IB2<'b when 'b :> IA2<'b> and 'b :> IB2<'b>> = 
    abstract M : int

;;
#q;;
