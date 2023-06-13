// #Regression #Conformance #DeclarationElements #Accessibility 
// On module
//<Expects id="FS0531" span="(6,1-6,7)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
#light

public module ModuleDef =   // here
                            let x = 42

