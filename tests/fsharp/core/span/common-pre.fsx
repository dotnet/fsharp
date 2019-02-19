open System
open System.IO

File.WriteAllText("refs.generated.fsx", sprintf @"#r @""%s\.nuget\packages\System.Memory\4.5.0\lib\netstandard2.0\System.Memory.dll""" (Environment.GetEnvironmentVariable("USERPROFILE")))
