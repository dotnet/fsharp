// #Regression #Conformance #DeclarationElements #Accessibility 
// On member

#light

module M =
          type Foo() = class 
                         public member this.Stuff() = 42     // here
                       end  

