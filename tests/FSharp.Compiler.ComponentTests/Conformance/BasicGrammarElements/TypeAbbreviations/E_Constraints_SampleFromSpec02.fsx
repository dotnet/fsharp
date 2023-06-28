// #Regression #Conformance #TypesAndModules 
// Abbreviation: the constraints on the right-hand-side are not sufficient
//<Expects id="FS0001" span="(16,14-16,19)" status="error">A type parameter is missing a constraint 'when 'b :> IB'</Expects>
#light

type IA = 
    abstract AbstractMember : int -> int

type IB = 
    abstract AbstractMember : int -> int

type C<'a when 'a :> IB>() = 
    static member StaticMember(x:'a) = x.AbstractMember(1)

// invalid: missing constraint
type E<'b> = C<'b>      // error

exit 1

