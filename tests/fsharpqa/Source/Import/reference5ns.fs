// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:3200
// Aux library that defines a type inside a module inside a namespace
#light
namespace N
module M =
    type T = string
