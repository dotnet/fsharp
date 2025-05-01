// #Regression #Conformance #Accessibility #ObjectOrientedTypes
// Dev11 40334 and 47156
// <Expects status="error" id="FS0629" span="(11,30-11,59)">Method 'ProtectedInstance' is not accessible from this code location</Expects>
// <Expects status="error" id="FS0629" span="(12,38-12,67)">Method 'ProtectedInstance' is not accessible from this code location</Expects>

open TestBaseClass

type DerivedClass() = class
    inherit BaseClass()
        
    member x.SomeMethod2() = BaseClass().ProtectedInstance()
    static member AnotherMethod2() = BaseClass().ProtectedInstance()

end

let r3 = DerivedClass().SomeMethod2()
let r4 = DerivedClass.AnotherMethod2()
