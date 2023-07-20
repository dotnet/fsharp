// #Regression #Conformance #DeclarationElements #Accessibility 
// On member
//<Expects id="FS0531" span="(8,26-8,32)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
#light

module M =
          type Foo() = class 
                         public member this.Stuff() = 42     // here
                       end  

