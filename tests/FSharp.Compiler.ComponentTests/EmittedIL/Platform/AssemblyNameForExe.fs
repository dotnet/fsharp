open System
open System.IO
open System.Reflection

let path = Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location)
let asm = AssemblyName.GetAssemblyName(Path.Combine(path, "PlatformedExe.exe"))
printfn "%s" (asm.ToString())