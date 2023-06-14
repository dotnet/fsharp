// #Regression #Conformance #DeclarationElements #Import 
#light

// Verify error when opening an unknown module or namespace
//<Expects id="FS0039" status="error">The namespace or module 'SomeUnknownNamespace' is not defined</Expects>

open SomeUnknownNamespace.SomeUnknownModule

