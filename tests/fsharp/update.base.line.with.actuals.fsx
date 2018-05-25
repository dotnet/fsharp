open System.IO

// this script is usefull for tests using a .bsl file (baseline) containing expected compiler output
// which is matched against .vserr or .err file aside once the test has run
// the script replaces all the .bsl/.bslpp with either .err or .vserr

let directories =
    [
      "typecheck/sigs"
      "typeProviders/negTests"
    ]
    |> List.map (fun d -> Path.Combine(__SOURCE_DIRECTORY__, d) |> DirectoryInfo)

let extensionPatterns = ["*.err"; "*.vserr"]
for d in directories do
    for p in extensionPatterns do 
        for errFile in d.GetFiles p do
            let baseLineFile = FileInfo(Path.ChangeExtension(errFile.FullName, "bsl"))
            let baseLineFilePreProcess = FileInfo(Path.ChangeExtension(errFile.FullName, "bslpp"))

            if File.ReadAllText(errFile.FullName) <> File.ReadAllText(baseLineFile.FullName) then
                let expectedFile =
                    if baseLineFilePreProcess.Exists then baseLineFilePreProcess
                    else baseLineFile

                printfn "%s not matching, replacing with %s" expectedFile.FullName errFile.FullName
                errFile.CopyTo(expectedFile.FullName, true) |> ignore
