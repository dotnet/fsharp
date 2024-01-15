// #Regression #Conformance #Accessibility #ObjectOrientedTypes
//Dev11:211080, used to give an internal error here
//<Expects status="error" span="(14,13-14,28)" id="FS0491">The member or object constructor 'Impl' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>
namespace Tim 

module Correlation = 
    type SpecMulti() = 
        member private t.Impl _ _ = () 
    type SpecSet = { 
        Inner : SpecMulti 
    } with 
        member t.Generate arg = 
            let fn _ = () 
            t.Inner.Impl fn arg 