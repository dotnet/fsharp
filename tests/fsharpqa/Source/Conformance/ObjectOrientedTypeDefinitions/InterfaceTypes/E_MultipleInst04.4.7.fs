// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5540
// It is forbidden to implement an interface at multiple instantiations
//<Expects status="error" id="FS3350" span="(12,6-12,7)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>


type IA<'a, 'b> =
    interface 
        abstract X : 'a -> 'b
    end

type C<'a>() = 
    interface IA<int,char> with
        member m.X(x) = 'a'
        
    interface IA<char, int> with
        member m.X(c) = 10
    
    
