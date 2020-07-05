// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5540
// See also FSHARP1.0:5597
// It is forbidden to implement an interface at multiple instantiations
//<Expects status="error" id="FS0888" span="(21,15-21,22)">Duplicate specification of an interface$</Expects>
//<Expects status="error" id="FS0362" span="(17,15-17,22)">Duplicate or redundant interface$</Expects>
//<Expects status="error" id="FS0362" span="(21,15-21,22)">Duplicate or redundant interface$</Expects>
//<Expects status="error" id="FS0855" span="(19,18-19,19)">No abstract or interface member was found that corresponds to this override$</Expects>
//<Expects status="error" id="FS0855" span="(23,18-23,19)">No abstract or interface member was found that corresponds to this override$</Expects>

type IB<'a> =
    interface 
        abstract X : 'a -> char
    end

type C<'a>() = 
    interface IB<int> 
     with
        member m.X(x) = 'a'
        
    interface IB<int> 
     with
        member m.X(c) = 'a'
    
    
