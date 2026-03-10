// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5540
// It is forbidden to implement an interface at multiple instantiations
//<Expects status="error" id="FS0362" span="(15,15-15,21)">Duplicate or redundant interface$</Expects>
//<Expects status="error" id="FS0362" span="(13,15-13,21)">Duplicate or redundant interface$</Expects>

type IB<'a> =
    interface 
        //abstract X : unit -> 'a
    end

type C<'a,'b>() = 
    interface IB<'a> 
        
    interface IB<'b> 
    
    
