// #Regression #Conformance #DeclarationElements #Attributes 
// Regression: FSB 4014
// Need to tighten up our imlementation of StructLayout.Sequential.
//<Expects id="FS0009" span="(9,10-9,12)" status="warning">Uses of this construct may result in the generation of unverifiable \.NET IL code\. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'\.</Expects>

module PositiveTests = 

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type P1 =  { r : int option ; name : string}

    exit 0
