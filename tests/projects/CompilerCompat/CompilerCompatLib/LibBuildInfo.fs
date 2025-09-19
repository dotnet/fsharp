namespace CompilerCompatLib

open System
open System.Reflection

module LibBuildInfo =
    let sdkVersion = 
        try Environment.Version.ToString() 
        with _ -> "Unknown"
    
    let fscPath = 
        try 
            let assembly = Assembly.GetExecutingAssembly()
            let location = assembly.Location
            if location.Contains("artifacts") then $"Local compiler (artifacts): {location}"
            else $"SDK compiler: {location}"
        with _ -> "Unknown"
