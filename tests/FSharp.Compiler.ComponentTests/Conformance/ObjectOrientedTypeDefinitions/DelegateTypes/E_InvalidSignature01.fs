// #Regression #Conformance #ObjectOrientedTypes #Delegates 


// Verify error when given an invalid delegate signature type
//<Expects id="FS0949" status="error">Delegate specifications must be of the form 'typ -> typ'</Expects>

type InvalidDelegate = delegate of int

exit 1
