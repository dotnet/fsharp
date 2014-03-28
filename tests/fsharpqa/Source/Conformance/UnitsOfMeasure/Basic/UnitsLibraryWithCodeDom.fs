// #FSharpQA #Conformance #UnitsOfMeasure #Regression #RequiresPowerPack
// Regression for 4824, this used to fail at runtime unable to find MyLibrary

#light
module Main
open System
open System.Reflection

[<Measure>] type mm
type MainType = A

let myLibraryCode = @"
#light
module MyLibrary
open System
open Main

let tanUnits x = Math.Tan(x / 1.0<mm>)
let atan2 x y = Math.Atan2(x, y)
let atan2Units x y = Math.Atan2(x / 1.0<mm>, y / 1.0<mm>) "

let myLibraryLocation =
   let parameters = new System.CodeDom.Compiler.CompilerParameters() in
   parameters.ReferencedAssemblies.AddRange [| typeof<int>.Assembly.Location; typeof<MainType>.Assembly.Location |]

   parameters.OutputAssembly <- System.IO.Path.GetTempPath() + "MyLibrary.dll"
   (new Microsoft.FSharp.Compiler.CodeDom.FSharpCodeProvider()).CompileAssemblyFromSource(parameters, [| myLibraryCode |]).CompiledAssembly.Location

let inMemoryCode = @"
#light
module InMemory
open Main
open MyLibrary

let testTanUnits() = tanUnits 0.0<mm>
let testAtan2() = atan2 0.0 1.0
let testAtan2Units() = atan2Units 0.0<mm> 1.0<mm> "

let inMemory =
   let parameters = new System.CodeDom.Compiler.CompilerParameters() in
   parameters.ReferencedAssemblies.AddRange [| typeof<int>.Assembly.Location; typeof<MainType>.Assembly.Location; myLibraryLocation |]

   parameters.GenerateInMemory <- true
   let fcp = (new Microsoft.FSharp.Compiler.CodeDom.FSharpCodeProvider())
   fcp.CompileAssemblyFromSource(parameters, [| inMemoryCode |]).CompiledAssembly.GetType("InMemory")

let testTanUnits = inMemory.InvokeMember("testTanUnits", BindingFlags.InvokeMethod, null, null, [| |])
let testATan2 = inMemory.InvokeMember("testAtan2", BindingFlags.InvokeMethod, null, null, [| |])
let testATan2Units = inMemory.InvokeMember("testAtan2Units", System.Reflection.BindingFlags.InvokeMethod, null, null, [| |])

printfn "%A" testTanUnits
printfn "%A" testATan2
printfn "%A" testATan2Units
exit <| (if unbox testTanUnits <> 0.0 || unbox testATan2 <> 0.0 || unbox testATan2Units <> 0.0 then 1 else 0)