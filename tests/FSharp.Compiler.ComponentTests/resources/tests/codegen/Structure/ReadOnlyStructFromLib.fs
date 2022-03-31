// #Regression #NoMT #CodeGen #Interop 
// Regression for FSHARP1.0:5750
// Code gen issues with copy of readonly struct from a separate assembly
open ReadWriteLib

let myClass = new MyClass()
printfn "MyClass.ReadonlyFoo = %x" myClass.ReadonlyFoo

let myStruct = new MyStruct()
printfn "MyStruct.WriteableFoo = %x" myStruct.WriteableFoo
// Previous error: The address of the variable 'copyOfStruct' cannot be used at this point
printfn "MyStruct.ReadonlyFoo = %x" myStruct.ReadonlyFoo

exit 0
