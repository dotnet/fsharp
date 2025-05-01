// #Regression #Conformance #DeclarationElements #Accessibility 
// Verify error when inheriting from less accessible base class





// Delegate signature: 
type private I2 =
    abstract P: int

type D = delegate of I2 -> unit

// Abstract slots:
type private I3=
    abstract P: int

type IAmAnotherInterface = 
    abstract Q : I3 -> unit

