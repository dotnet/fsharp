// #Regression #Conformance #TypesAndModules #Namespaces 
// Regression for Dev10: 837511
// "namespace global" causes strange error in dependent code in different assembly

let a = Foo()
let b = a.x
