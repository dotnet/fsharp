#load "perf.fsx"

open Perf
open System

printfn "%A" fsi.CommandLineArgs
if fsi.CommandLineArgs.Length <> 3 then printfn "usage: fsi gen.fsx <directory> <size>"
let D = string fsi.CommandLineArgs.[1] 
let N = int fsi.CommandLineArgs.[2] 

try System.IO.Directory.Delete(D, true) with _ -> ()
System.IO.Directory.CreateDirectory D
System.Environment.CurrentDirectory <- D

let writeDense (dir : string) (projectType : ProjectType) (count : int) =

    let extension = match projectType with FSharp -> "fsproj" | CSharp -> "csproj"

    let projects =
        let projectName = sprintf "%sSharpTest%i" (match projectType with FSharp -> "F" | CSharp -> "C")
        [1..count] |> List.map (fun i -> projectName i, Guid.NewGuid ())

    let writeProject i (name, guid) =
        let path = sprintf @"%s\%s\%s.%s" dir name name extension
        let references =
            let makeRef (name, guid) = { Name = name ; Guid = guid ; RelativePath = sprintf @"..\%s\%s.%s" name name extension }
            projects.[0..i-1] |> List.map makeRef
        let files = 
            [ if extension = "fsproj" then 
                let fileName = sprintf "%s.fs" name
                yield fileName ]
        let project = { Name = name ; Guid = guid ; Files = files ; References = references ; BinaryReferences = [] }
        let writer = match projectType with FSharp -> FSharpProject.write | CSharp -> CSharpProject.write
        writer path project

    projects |> List.iteri writeProject

    let solution =
        let makeProjectRef (name, guid) =
            let path = sprintf @"%s\%s.%s" name name extension
            { Name = name ; Guid = guid ; RelativePath  = path }, projectType
        projects |> List.map makeProjectRef |> (fun prs -> { Projects = prs })

    Solution.write (sprintf @"%s\Dense.sln" dir) solution

// Produces (N * 99) / 2 = 4950 references
writeDense "dense" FSharp N
writeDense "denseCSharp" CSharp N

let writeShallow (dir : string) (projectType : ProjectType) (count1 : int) (count2 : int) =

    let extension = match projectType with FSharp -> "fsproj" | CSharp -> "csproj"

    let aProjects =
        let projectName = sprintf "%sSharpTestA%i" (match projectType with FSharp -> "F" | CSharp -> "C")
        [1..count1] |> List.map (fun i -> projectName i, Guid.NewGuid ())

    let bProjects =
        let projectName = sprintf "%sSharpTestB%i" (match projectType with FSharp -> "F" | CSharp -> "C")
        [1..count2] |> List.map (fun i -> projectName i, Guid.NewGuid ())

    let writeAProject (name, guid) =
        let path = sprintf @"%s\%s\%s.%s" dir name name extension
        let project = { Name = name ; Guid = guid ; Files = [] ; References = [] ; BinaryReferences = [] }
        let writer = match projectType with FSharp -> FSharpProject.write | CSharp -> CSharpProject.write
        writer path project

    let writeBProject (name, guid) =
        let path = sprintf @"%s\%s\%s.%s" dir name name extension
        let references =
            let makeRef (name, guid) = { Name = name ; Guid = guid ; RelativePath = sprintf @"..\%s\%s.%s" name name extension }
            aProjects |> List.map makeRef
        let project = { Name = name ; Guid = guid ; Files = [] ; References = references ; BinaryReferences = [] }
        let writer = match projectType with FSharp -> FSharpProject.write | CSharp -> CSharpProject.write
        writer path project

    aProjects |> List.iter writeAProject
    bProjects |> List.iter writeBProject

    let solution =
        let makeProjectRef (name, guid) =
            let path = sprintf @"%s\%s.%s" name name extension
            { Name = name ; Guid = guid ; RelativePath  = path }, projectType
        (aProjects @ bProjects) |> List.map makeProjectRef |> (fun prs -> { Projects = prs })

    Solution.write (sprintf @"%s\Shallow.sln" dir) solution

// Produces (N/2) * N = 5000 references
writeShallow "shallow" FSharp (N/2) N
writeShallow "shallowCSharp" CSharp (N/2) N

let writeDenseBin (dir : string) (projectType : ProjectType) (count : int) =

    let extension = match projectType with FSharp -> "fsproj" | CSharp -> "csproj"

    let projects =
        let projectName = sprintf "%sSharpTest%i" (match projectType with FSharp -> "F" | CSharp -> "C")
        [1..count] |> List.map (fun i -> projectName i, Guid.NewGuid ())

    let writeProject i (name, guid) =
        let path = sprintf @"%s\%s\%s.%s" dir name name extension
        let references =
            let makeRef (name, guid) : BinaryRef = { Name = name ; RelativePath = sprintf @"..\%s\bin\Debug\%s.dll" name name }
            projects.[0..i-1] |> List.map makeRef
        let project = { Name = name ; Guid = guid ; Files = [] ; References = [] ; BinaryReferences = references }
        let writer = match projectType with FSharp -> FSharpProject.write | CSharp -> CSharpProject.write
        writer path project

    projects |> List.iteri writeProject

    let solution =
        let makeProjectRef (name, guid) =
            let path = sprintf @"%s\%s.%s" name name extension
            { Name = name ; Guid = guid ; RelativePath  = path }, projectType
        projects |> List.map makeProjectRef |> (fun prs -> { Projects = prs })

    Solution.write (sprintf @"%s\DenseBin.sln" dir) solution

// Produces (N * 99) / 2 = 4950 references
writeDenseBin "denseBin" FSharp N
writeDenseBin "denseBinCSharp" CSharp N
