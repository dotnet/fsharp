// #Conformance #DeclarationElements #PInvoke 
#light

// Verify we can place the ComVisible attribute on Records
// (We add a parameterless constructor)

[<System.Runtime.InteropServices.ComVisible(true)>]
type r = { x : int ; y : int }

exit 0
