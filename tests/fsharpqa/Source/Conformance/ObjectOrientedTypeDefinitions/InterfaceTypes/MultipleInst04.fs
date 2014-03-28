// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5540
// It is forbidden to implement an interface at multiple instantiations
//<Expects status="error" id="FS0443" span="(12,6-12,7)">This type implements or inherits the same interface at different generic instantiations 'IA<char,int>' and 'IA<int,char>'\. This is not permitted in this version of F#\.$</Expects>


type IA<'a, 'b> =
    interface 
        abstract X : 'a -> 'b
    end

type C<'a>() = 
    interface IA<int,char> with
        member m.X(x) = 'a'
        
    interface IA<char, int> with
        member m.X(c) = 10
    
    
