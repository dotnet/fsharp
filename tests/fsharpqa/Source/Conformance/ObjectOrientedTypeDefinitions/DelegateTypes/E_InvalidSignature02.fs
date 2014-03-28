// #Regression #Conformance #ObjectOrientedTypes #Delegates 
// Verify error when given an invalid delegate signature type
//<Expects id="FS0950" status="error">Delegate specifications must not be curried types\. Use 'typ \* \.\.\. \* typ -> typ' for multi-argument delegates, and 'typ -> \(typ -> typ\)' for delegates returning function values</Expects>

type InvalidDelegate = delegate of int -> string -> unit

exit 1
