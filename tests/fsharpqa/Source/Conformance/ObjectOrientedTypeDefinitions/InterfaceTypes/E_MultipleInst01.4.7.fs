// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5540
// Prior to F# 5.0 it was forbidden to implement an interface at multiple instantiations
//<Expects status="error" id="FS3350" span="(11,6-11,7)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>

type IA<'a> =
    interface 
        //abstract X : unit -> 'a
    end

type C() = 
    interface IA<int>
    interface IA<string>
