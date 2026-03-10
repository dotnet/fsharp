// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 

type IA<'a, 'b> =
    interface 
        abstract X : 'a -> 'b
    end

type C<'a>() = 
    interface IA<int,char> with
        member m.X(x) = 'a'
        
    interface IA<char, int> with
        member m.X(c) = 10
    
    
