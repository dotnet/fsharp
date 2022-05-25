open System
open System.IO
open System.Reflection

let path = Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location)
let asm=Assembly.ReflectionOnlyLoadFrom(Path.Combine(path, "platformedExe.exe"))
