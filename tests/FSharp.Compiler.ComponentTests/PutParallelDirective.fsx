open System
open System.IO
open System.Xml.Linq

do Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)

let hashDirective = """#paralell_compilation_group "independent_component_tests" """

let fsproj = XDocument.Load("FSharp.Compiler.ComponentTests.fsproj")
let compiledFiles = 
    fsproj.Descendants("Compile")
    |> Seq.map (fun e -> e.Attribute("Include").Value)
    |> Seq.toArray


for f in compiledFiles do
    let source = File.ReadAllText f
    let nsIdx = source.IndexOf "namespace"
    let modIdx = source.IndexOf "module"

    if nsIdx > -1 && (modIdx = -1 || nsIdx < modIdx) then
        let newSource = hashDirective + Environment.NewLine + source
        File.WriteAllText(f,newSource)
    else
        printfn "Bad: %s" f