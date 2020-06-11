// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// It is now allowed to implement the same interface multiple times (RFC FS-1031).

type IA<'a> =
    interface 
        //abstract X : unit -> 'a
    end

type C() = 
    interface IA<int>
    interface IA<string>
