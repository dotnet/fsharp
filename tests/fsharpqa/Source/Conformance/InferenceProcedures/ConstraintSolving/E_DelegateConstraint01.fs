// #Regression #Conformance #TypeInference #TypeConstraints 
#light

// Verify error message if delegate type doesn't have first parameter of type 'object'.
// That is, the delegate constraint requires the first parameter be the 'sender'

//<Expects id="FS0001" status="error">The type 'CallbackAlpha' has a non-standard delegate type</Expects>
//<Expects id="FS0001" status="error">The type 'CallbackBravo' has a non-standard delegate type</Expects>

type CallbackAlpha = delegate of unit * unit -> int
type CallbackBravo = delegate of unit * unit -> int

type DelegateUtils<'del when 'del : delegate<unit * unit, int> and 'del :> System.Delegate> =
    static member Invoke (x : 'del) = 
        x.DynamicInvoke([| box (); box () |])

// ---------------------------------------

let lambda = fun (_ : unit) (_ : unit) -> 0
                
let alpha = new CallbackAlpha(lambda)
let bravo = new CallbackBravo(lambda)

// ---------------------------------------

let result1 = DelegateUtils<CallbackAlpha>.Invoke(alpha)
let result2 = DelegateUtils<CallbackBravo>.Invoke(bravo)

// ---------------------------------------

exit 1
