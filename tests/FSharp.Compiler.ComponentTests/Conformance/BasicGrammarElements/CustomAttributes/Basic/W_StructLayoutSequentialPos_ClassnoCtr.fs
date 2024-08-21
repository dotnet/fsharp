// #Regression #Conformance #DeclarationElements #Attributes
// Regression: FSB 4014
// Need to tighten up our imlementation of StructLayout.Sequential.

module PositiveTests =

    [<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>]
    type P3 =
        member this.M = 1


