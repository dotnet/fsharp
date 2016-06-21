module CheckHighEntropyASLR

open NUnit.Framework

open NUnitConf
open PlatformHelpers
open System.IO

let run cwd envVars assemblyToCheck = attempt {

    let env key = envVars |> Map.tryFind key
    let envOrDefault key def = env key |> Option.fold (fun s t -> t) def

    let fileExists = Commands.fileExists cwd

    let execOutToString p args = attempt {
        let tempFile = Path.GetTempFileName()
        do! Command.exec cwd envVars { Output = Output(Overwrite(tempFile)); Input = None } p args 
            |> checkResult
        return File.ReadAllText(tempFile)
        }

    // @echo off
    ignore "unused"

    // REM Search for the Linker
    // REM Use VS2015 or fall back to VS2014
    let! linkExe = attempt {
        // SET LINK_EXE="%VS150COMNTOOLS%\..\..\VC\bin\link.exe"
        // IF NOT EXIST %LINK_EXE% ( SET LINK_EXE="%VS140COMNTOOLS%..\..\VC\bin\link.exe" )
        // IF NOT EXIST %LINK_EXE% ( 
        //     @echo "Test Requires LINK.EXE" --- Not found 
        //     @echo "When installing VS please select "Select Visual C++ / Common Tools For Visual C++"
        // )

        let linkOpt =
            [ "VS150COMNTOOLS"; "VS140COMNTOOLS" ]
            |> List.choose env
            |> List.map (sprintf @"%s\..\..\VC\bin\link.exe")
            |> List.tryPick fileExists
        return! match linkOpt with
                | Some path -> succeed path
                | None -> NUnitConf.genericError (
                           "Test Requires LINK.EXE, --- Not found" + System.Environment.NewLine +
                           "When installing VS please select \"Select Visual C++ / Common Tools For Visual C++\"" )
        }

    //REM %LINK_EXE% -- Path to link.exe
    //REM %1 -- assembly to check
    ignore "from arguments"
    //REM %2 -- expected value ("yes" or "no")
    ignore "ignored, now the function return a bool"

    //%LINK_EXE% /dump /headers %1 | find "High Entropy Virtual Addresses" > NUL
    //IF /I "%2"=="yes" IF     ERRORLEVEL 1 EXIT /B 1
    //IF /I "%2"=="no"  IF NOT ERRORLEVEL 1 EXIT /B 1
    let! link = Commands.link execOutToString linkExe "/dump /headers" [ assemblyToCheck ]
    return link.Contains("High Entropy Virtual Addresses")

    }
