// #Regression #Conformance #DeclarationElements #Accessibility 
// Verify error when inheriting from less accessible base class

//<Expects id="FS0410" status="error" span="(12,6)">The type 'I2' is less accessible than the value, member or type 'D' it is used in</Expects>

//<Expects id="FS0410" status="error" span="(18,6)">The type 'I3' is less accessible than the value, member or type 'IAmAnotherInterface' it is used in</Expects>

// Delegate signature: 
type private I2 =
    abstract P: int

type D = delegate of I2 -> unit

// Abstract slots:
type private I3=
    abstract P: int

type IAmAnotherInterface = 
    abstract Q : I3 -> unit

