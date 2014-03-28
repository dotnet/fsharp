// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
#light
// Regression for FSharp 1.0:1794

//Define a new abstract type (interface)
type public IA =
    abstract value : int

//create instance IA AND override ToString 
let x = 
    { new System.Object() with
        member i.ToString() = "anonymous"
      interface IA with
        member i.value = 0  
    }

//since the type is anonymous, I can't find a programmatic way to verify that 'x' is a 'IA'.
//Intellisense says it is.

exit x.value
