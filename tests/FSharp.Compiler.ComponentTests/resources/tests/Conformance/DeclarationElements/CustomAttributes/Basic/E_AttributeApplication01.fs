// #Regression #Conformance #DeclarationElements #Attributes 
#light

// FSB 1036, Assembly-level attributes attached to non-do bindings
//<Expects id="FS0841" status="error">This attribute is not valid for use on this language element\. Assembly attributes should be attached to a 'do \(\)' declaration, if necessary within an F# module</Expects>

[<assembly:System.Reflection.AssemblyDescription("Assembly description attribute applied to a let binding...")>]
let f = 1
