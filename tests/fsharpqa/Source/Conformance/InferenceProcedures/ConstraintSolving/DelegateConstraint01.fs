// #Conformance #TypeInference #TypeConstraints 
#light

// Sanity check delegate constraints

type CallbackAlpha = delegate of obj * unit * unit -> int
type CallbackBravo = delegate of obj * unit * unit -> int

type DelegateUtils<'del when 'del : delegate<unit * unit, int> and 'del :> System.Delegate> =
    static member Invoke (x : 'del) = 
        // null is the 'sender', the two units are normal parameters
        x.DynamicInvoke([| null; box (); box () |])

// ---------------------------------------

let timesInvoked = ref 0
let lambda = fun (_ : obj) (_ : unit) (_ : unit) -> 
                printfn "Lamda invoked!"
                timesInvoked := !timesInvoked + 1
                !timesInvoked
                
let alpha = new CallbackAlpha(lambda)
let bravo = new CallbackBravo(lambda)

// ---------------------------------------

if !timesInvoked <> 0 then exit 1

let result1 = DelegateUtils<CallbackAlpha>.Invoke(alpha)
if !timesInvoked <> 1 then exit 1

let result2 = DelegateUtils<CallbackBravo>.Invoke(bravo)
if !timesInvoked <> 2 then exit 1

// ---------------------------------------

exit 0
