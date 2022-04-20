// #Regression #Conformance #Accessibility #ObjectOrientedTypes
// Regression for Dev11 40334 and 47156
open TestBaseClass

type DerivedClass() = class
    inherit BaseClass()
        
    member x.SomeMethod() = BaseClass.ProtectedStatic()
    static member AnotherMethod() = BaseClass.ProtectedStatic()
end

try
    let r1 = DerivedClass().SomeMethod()
    let r2 = DerivedClass.AnotherMethod()
    if r1 <> 3 && r2 <> 3 then failwith "Failed: 1"
with
    | :? System.MethodAccessException -> failwith "Failed: 1"
