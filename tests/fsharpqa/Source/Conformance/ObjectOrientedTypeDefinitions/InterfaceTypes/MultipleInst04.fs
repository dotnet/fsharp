// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// It is now allowed to implement the same interface multiple times (RFC FS-1031).

type IA<'a, 'b> =
    interface 
        abstract X : 'a -> 'b
    end

type C<'a>() = 
    interface IA<int,char> with
        member m.X(x) = 'a'
        
    interface IA<char, int> with
        member m.X(c) = 10
    
    
