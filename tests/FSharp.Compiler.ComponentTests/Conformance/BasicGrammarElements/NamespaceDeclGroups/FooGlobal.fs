// #Regression #Conformance #TypesAndModules #Namespaces 
// Regression for Dev10: 837511
// "namespace global" causes strange error in dependent code in different assembly
// This will be a library for other test files to consume

namespace global

type Foo() =
    member this.x = 1
