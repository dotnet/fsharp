// #Regression #Conformance #TypeConstraints 
// Verify error if the delegate constraint isn't met
// Note this constraint requires that the delegate obey the
// standard .NET idiom (obj sender, EventArgs args) -> unit
//<Expects id="FS0001" status="error">The type 'D1' has a non-standard delegate type</Expects>

type D1 = delegate of int -> unit
type D2 = delegate of obj * System.EventArgs -> unit
type D3 = delegate of obj * System.EventArgs * float -> unit


let isDelegate2 (x : 'a when 'a : delegate<System.EventArgs, unit>) = ()
do isDelegate2 (D2(fun _ _ -> ()))

let isDelegate3 (x : 'a when 'a : delegate<System.EventArgs * float, unit>) = ()
do isDelegate3 (D3(fun _ _ _ -> ()))

// Fails
let isDelegate1 (x : 'a when 'a : delegate<int, unit>) = ()
do isDelegate1 (D1(fun _ -> ()))

exit 1
