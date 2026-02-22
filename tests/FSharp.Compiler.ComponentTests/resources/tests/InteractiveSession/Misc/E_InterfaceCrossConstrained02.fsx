// #Regression #NoMT #FSI #RequiresENU 
// Regression test for DEV10#832789 
//<Expects status="success" id="FS0001">A type parameter is missing a constraint 'when 'a :> IA2<'a>'</Expects>

type IA2<'a when 'a :> IB2<'a>> = 
    abstract M : int
and  IB2<'b when 'b :> IA2<'b>> = 
    abstract M : int

;;
#q;;
