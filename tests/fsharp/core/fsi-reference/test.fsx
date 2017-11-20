#r @"ImplementationAssembly\ReferenceAssemblyExample.dll"
#r @"ReferenceAssembly\ReferenceAssemblyExample.dll"
let c = new ReferenceAssembly.MyClass()
let _ = c.X

// If this fails then the jit blows up so this file will not get written.
let os = System.IO.File.CreateText "test.ok" in os.Close() 
exit 0
