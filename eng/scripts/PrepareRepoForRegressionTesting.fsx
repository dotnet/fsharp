/// Script to inject UseLocalCompiler.Directory.Build.props import into a third-party repository's Directory.Build.props
/// Usage: dotnet fsi PrepareRepoForRegressionTesting.fsx <path-to-UseLocalCompiler.Directory.Build.props>

open System
open System.IO
open System.Xml

let propsFilePath = "Directory.Build.props"

let useLocalCompilerPropsPath = 
    let args = Environment.GetCommandLineArgs()
    // When running with dotnet fsi, args are: [0]=dotnet; [1]=fsi.dll; [2]=script.fsx; [3...]=args
    let scriptArgs = args |> Array.skipWhile (fun a -> not (a.EndsWith(".fsx"))) |> Array.skip 1
    if scriptArgs.Length > 0 then
        scriptArgs.[0]
    else
        failwith "Usage: dotnet fsi PrepareRepoForRegressionTesting.fsx <path-to-UseLocalCompiler.Directory.Build.props>"

printfn "PrepareRepoForRegressionTesting.fsx"
printfn "==================================="
printfn "UseLocalCompiler props path: %s" useLocalCompilerPropsPath

if not (File.Exists(useLocalCompilerPropsPath)) then
    failwithf "UseLocalCompiler.Directory.Build.props not found at: %s" useLocalCompilerPropsPath

printfn "✓ UseLocalCompiler.Directory.Build.props found"

let absolutePropsPath = 
    Path.GetFullPath(useLocalCompilerPropsPath).Replace("\\", "/")
printfn "Absolute path: %s" absolutePropsPath

if File.Exists(propsFilePath) then
    printfn "Directory.Build.props exists, modifying it..."
    
    let doc = XmlDocument()
    doc.PreserveWhitespace <- true
    doc.Load(propsFilePath)
    
    let projectElement = doc.SelectSingleNode("/Project")
    if isNull projectElement then
        failwith "Could not find Project element in Directory.Build.props"
    
    let xpath = "//Import[contains(@Project, 'UseLocalCompiler.Directory.Build.props')]"
    let existingImport = doc.SelectSingleNode(xpath)
    
    if isNull existingImport then
        let importElement = doc.CreateElement("Import")
        importElement.SetAttribute("Project", absolutePropsPath)
        
        if projectElement.HasChildNodes then
            projectElement.InsertBefore(importElement, projectElement.FirstChild) |> ignore
        else
            projectElement.AppendChild(importElement) |> ignore
        
        let newline = doc.CreateTextNode("\n  ")
        projectElement.InsertAfter(newline, importElement) |> ignore
        
        doc.Save(propsFilePath)
        printfn "✓ Added UseLocalCompiler import to Directory.Build.props"
    else
        printfn "✓ UseLocalCompiler import already exists"
    
    let otherFlagsWithTimes = doc.SelectSingleNode("//OtherFlags[contains(text(), '--times')]")
    
    if isNull otherFlagsWithTimes then
        let propertyGroup = doc.CreateElement("PropertyGroup")
        let otherFlags = doc.CreateElement("OtherFlags")
        otherFlags.InnerText <- "$(OtherFlags) --nowarn:75 --times"
        propertyGroup.AppendChild(otherFlags) |> ignore
        
        let importNode = doc.SelectSingleNode(xpath)
        
        // PreserveWhitespace=true causes XML DOM to keep text nodes (newlines/indentation) between elements;
        // skip past the whitespace text node after the import to position the PropertyGroup correctly
        let nodeAfterImport = 
            if not (isNull importNode) && not (isNull importNode.NextSibling) && importNode.NextSibling.NodeType = XmlNodeType.Text then
                importNode.NextSibling
            else
                null
        
        if not (isNull nodeAfterImport) then
            projectElement.InsertAfter(propertyGroup, nodeAfterImport) |> ignore
        else
            projectElement.InsertAfter(propertyGroup, importNode) |> ignore
        
        let newlineAfter = doc.CreateTextNode("\n  ")
        projectElement.InsertAfter(newlineAfter, propertyGroup) |> ignore
        
        doc.Save(propsFilePath)
        printfn "✓ Added --times flag to OtherFlags"
    else
        if not (otherFlagsWithTimes.InnerText.Contains("--nowarn:75")) then
            otherFlagsWithTimes.InnerText <- otherFlagsWithTimes.InnerText.Replace("--times", "--nowarn:75 --times")
            doc.Save(propsFilePath)
            printfn "✓ Added --nowarn:75 to existing OtherFlags"
        else
            printfn "✓ --times and --nowarn:75 already exist in OtherFlags"
else
    printfn "Directory.Build.props does not exist, creating it..."
    let newContent = sprintf "<Project>\n  <Import Project=\"%s\" />\n  <PropertyGroup>\n    <OtherFlags>$(OtherFlags) --nowarn:75 --times</OtherFlags>\n  </PropertyGroup>\n</Project>\n" absolutePropsPath
    File.WriteAllText(propsFilePath, newContent)
    printfn "✓ Created Directory.Build.props with UseLocalCompiler import and --times flag"

printfn ""
printfn "Final Directory.Build.props content:"
printfn "-----------------------------------"
let content = File.ReadAllText(propsFilePath)
printfn "%s" content
printfn "-----------------------------------"
printfn "✓ Repository prepared for regression testing"
