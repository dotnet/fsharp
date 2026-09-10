open System
open System.IO

let packagesDir = 
    match Environment.GetEnvironmentVariable("NUGET_PACKAGES") with
    | null -> Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".nuget", "packages")
    | path -> path
File.WriteAllText("refs.generated.fsx", sprintf @"#r @""%s\System.Memory\4.6.3\lib\netstandard2.0\System.Memory.dll""" packagesDir)
