// #Regression #Conformance #DeclarationElements #Accessibility 
// On member



module M =
          type Foo() = class 
                         public member this.Stuff() = 42     // here
                       end  

