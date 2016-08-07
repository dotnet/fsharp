// #Warnings
//<Expects status="Error" span="(11,17)" id="FS0495">The object constructor 'MyClass' has no argument or settable return property 'Property'.</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>MyProperty</Expects>

type MyClass() =
    member val MyProperty = "" with get, set 
    member val MyProperty2 = "" with get, set
    member val ABigProperty = "" with get, set

let c = MyClass(Property = "")
    
exit 0