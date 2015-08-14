// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5540
// It is forbidden to implement an interface at multiple instantiations
//<Expects status="error" id="FS0443" span="(11,6-11,7)">This type implements the same interface at different generic instantiations 'IA<string>' and 'IA<int>'\. This is not permitted in this version of F#\.$</Expects>

type IA<'a> =
    interface 
        //abstract X : unit -> 'a
    end

type C() = 
    interface IA<int>
    interface IA<string>
