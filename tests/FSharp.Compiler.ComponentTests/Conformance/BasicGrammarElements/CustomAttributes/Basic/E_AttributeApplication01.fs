// #Regression #Conformance #DeclarationElements #Attributes 
#light

// FSB 1036, Assembly-level attributes attached to non-do bindings


[<assembly:System.Reflection.AssemblyDescription("Assembly description attribute applied to a let binding...")>]
let f = 1
