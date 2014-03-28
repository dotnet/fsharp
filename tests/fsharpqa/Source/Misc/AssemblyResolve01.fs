// #Misc #Reflection #Events
// Test issue 3322, try hooking AssemblyResolve event and using a global store to find some assemblies

open System
open System.IO
open System.Reflection

let handler = new ResolveEventHandler(fun sender args -> 
                                        match args.Name with 
                                        | @"FSLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" -> Assembly.LoadFile(Environment.CurrentDirectory + "\Test\FSLib.dll")
                                        | @"CSLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" -> Assembly.LoadFile(Environment.CurrentDirectory + "\Test\CSLib.dll")
                                        | _ -> null
                                     )

System.AppDomain.CurrentDomain.add_AssemblyResolve(handler)

let x = System.AppDomain.CurrentDomain.CreateInstance(@"FSLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "FSLib.FSType")
let y = System.AppDomain.CurrentDomain.CreateInstance(@"CSLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "CSLib.CSType")
try
    System.AppDomain.CurrentDomain.CreateInstance(@"OtherLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "OtherLib.OtherType") |> ignore
    exit 1
with
    | :? FileNotFoundException as e -> ()

if x.Unwrap().GetType().ToString() <> "FSLib.FSType" then exit 1
if y.Unwrap().GetType().ToString() <> "CSLib.CSType" then exit 1

exit 0
