// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 

type IA<'a> =
    interface 
        //abstract X : unit -> 'a
    end

type C() = 
    interface IA<int>
    interface IA<string>
