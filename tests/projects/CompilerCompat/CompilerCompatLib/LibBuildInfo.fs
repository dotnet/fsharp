namespace CompilerCompatLib

open System
open System.Reflection
open System.Diagnostics

module LibBuildInfo =
    let sdkVersion = 
        try 
            // Try to get .NET version from runtime
            Environment.Version.ToString()
        with _ -> "Unknown"
    
    let fscPath = 
        try 
            // Check if we're using local compiler by looking at assembly location
            let assembly = Assembly.GetExecutingAssembly()
            let location = assembly.Location
            if location.Contains("artifacts") then
                $"Local compiler (artifacts): {location}"
            else
                $"SDK compiler: {location}"
        with _ -> "Unknown"
    
    let isLocalBuild = 
        try
            // Additional check to see if we're using local build
            let assembly = Assembly.GetExecutingAssembly()
            assembly.Location.Contains("artifacts")
        with _ -> false
