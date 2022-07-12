// #Regression #Conformance #Accessibility #ObjectOrientedTypes
// Dev11 40334 and 47156

open TestBaseClass

type DerivedClass() = class
    inherit BaseClass()
        
    member x.SomeMethod() = x.ProtectedInstance()
    static member AnotherMethod() = 
        let x = DerivedClass()
        x.ProtectedInstance()

end

let r1 = DerivedClass().SomeMethod()
let r2 = DerivedClass.AnotherMethod()

if r1 <> 3 && r2 <> 4 then failwith "Failed: 1"
