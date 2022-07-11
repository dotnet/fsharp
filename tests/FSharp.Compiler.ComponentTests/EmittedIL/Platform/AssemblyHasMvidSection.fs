module AssemblyHasMvidSection

open System
open System.IO
open System.Reflection
open FSharp.Compiler.ComponentTests.EmittedIL

let  pathToDll =
    let d = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
    Path.Combine(d, "SimpleFsProgram.dll")

Console.WriteLine($"Verify mvid section for: {pathToDll}");
let stream = File.OpenRead(pathToDll);
let mvid = MvidReader.ReadAssemblyMvidOrEmpty(stream)

let message = $"Mvid for {pathToDll} = {mvid}"
printfn $"{message}"

if mvid = Guid.Empty then failwith message