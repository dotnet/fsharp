// #Regression #Conformance #DeclarationElements #Attributes 
// Regression: FSB 4014
// Need to tighten up our imlementation of StructLayout.Sequential.
//<Expects status="error" span="(9,10-9,12)" id="FS0937">Only structs and classes without primary constructors may be given the 'StructLayout' attribute$</Expects>

module NegativeTests = 
    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    [<AbstractClass>]
    type N2() = 
        abstract M : unit -> 'a
