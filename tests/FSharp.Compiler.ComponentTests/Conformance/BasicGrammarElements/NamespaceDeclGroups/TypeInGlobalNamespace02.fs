// #Regression #Conformance #TypesAndModules #Namespaces 
// See FSHARP1.0:6251
// "namespace global" causes strange error in dependent code in different assembly
// Can't use fully qualified name
//<Expects status="success"></Expects>

let a = global.Foo()
