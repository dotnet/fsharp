// #Regression #Conformance #DeclarationElements #Accessibility 
// On type
//<Expects id="FS0531" span="(8,13-8,20)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>

#light

module M =
            private type Foo() = class 
                                 end   

