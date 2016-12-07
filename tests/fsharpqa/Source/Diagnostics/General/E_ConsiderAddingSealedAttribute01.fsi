// #Regression #Diagnostics 
//<Expects status="error" span="(4,6-4,7)" id="FS0297">The type definitions for type 'T' in the signature and implementation are not compatible because the implementation type is not sealed but signature implies it is\. Consider adding the \[<Sealed>\] attribute to the implementation\.$</Expects>
module M
[<Sealed>]
type T =
    member D : decimal
