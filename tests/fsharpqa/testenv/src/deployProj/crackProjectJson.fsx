module CrackProjectJson

#load "../../../../../src/buildtools/scriptlib.fsx"
#r "../../../../../packages/FSharp.Data.2.2.5/lib/net40/FSharp.Data.dll"

open FSharp.Data
open FSharp.Data.JsonExtensions 

/// Collects references from project.json.lock
let collectReferences (isVerbose, packagesDir, targetPlatformName, lockFile:string, isForExecute) = 
    let setPathSeperators (path:string) = path.Replace('/', '\\')

    let splitNameAndVersion (ref:string) =
        let elements = ref.Split [| '/' |]
        if elements.Length >= 2 then
            Some(elements.[0], elements.[1])
        else
            None

    let getReferencedFiles (referencedFiles:JsonValue) =
        seq {
            for path, _ in referencedFiles.Properties do
                let path = setPathSeperators path
                if getFilename path = "_._" then ()
                else yield setPathSeperators path
        }

    let buildReferencePaths name version paths =
        seq {
            for path in paths do
                yield sprintf @"%s\%s\%s\%s" packagesDir name version path
        }

    let getAssemblyReferenciesFromTargets (targets:JsonValue) =
        seq {
            let target = targets.TryGetProperty(targetPlatformName)
            match target with 
            | Some t ->
                for ref, value in  t.Properties do
                    match splitNameAndVersion ref with
                    | Some(name, version) -> 
                        if isVerbose then
                            printfn "name:              %A" name
                            printfn "version:           %A" version
                        if not isForExecute then 
                            match value.TryGetProperty("compile") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles x)
                        else 
                            match value.TryGetProperty("runtime") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles value?runtime)
                            match value.TryGetProperty("native") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles value?native)
                    | _ -> ()
            | _  -> ()
        }

    if isVerbose then 
        printfn "lockFile:           %A" lockFile
        printfn "targetPlatformName: %A" targetPlatformName
        printfn "packagesDir:        %A" packagesDir
    let projectJson = JsonValue.Load(lockFile)
    getAssemblyReferenciesFromTargets projectJson?targets |> Seq.distinct

