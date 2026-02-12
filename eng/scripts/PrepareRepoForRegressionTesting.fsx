/// Script to inject UseLocalCompiler.Directory.Build.props import into a third-party repository's Directory.Build.props
/// Usage: dotnet fsi PrepareRepoForRegressionTesting.fsx <path-to-UseLocalCompiler.Directory.Build.props>
/// 
/// This script is designed to be run in the root of a third-party repository
/// It modifies the Directory.Build.props to import the UseLocalCompiler.Directory.Build.props

open System
open System.IO
open System.Xml

let propsFilePath = "Directory.Build.props"

// Get the path to UseLocalCompiler.Directory.Build.props from command line args
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

// Verify the UseLocalCompiler props file exists
if not (File.Exists(useLocalCompilerPropsPath)) then
    failwithf "UseLocalCompiler.Directory.Build.props not found at: %s" useLocalCompilerPropsPath

printfn "✓ UseLocalCompiler.Directory.Build.props found"

// Convert to absolute path and normalize slashes for MSBuild
let absolutePropsPath = 
    Path.GetFullPath(useLocalCompilerPropsPath).Replace("\\", "/")
printfn "Absolute path: %s" absolutePropsPath

if File.Exists(propsFilePath) then
    printfn "Directory.Build.props exists, modifying it..."
    
    // Load the existing XML
    let doc = XmlDocument()
    doc.PreserveWhitespace <- true
    doc.Load(propsFilePath)
    
    // Find the Project element
    let projectElement = doc.SelectSingleNode("/Project")
    if isNull projectElement then
        failwith "Could not find Project element in Directory.Build.props"
    
    // Check if our import already exists (look for any import with this exact path)
    let xpath = sprintf "//Import[@Project='%s']" absolutePropsPath
    let existingImport = doc.SelectSingleNode(xpath)
    
    if isNull existingImport then
        // Create Import element
        let importElement = doc.CreateElement("Import")
        importElement.SetAttribute("Project", absolutePropsPath)
        
        // Insert as first child of Project element
        if projectElement.HasChildNodes then
            projectElement.InsertBefore(importElement, projectElement.FirstChild) |> ignore
        else
            projectElement.AppendChild(importElement) |> ignore
        
        // Add newline for formatting
        let newline = doc.CreateTextNode("\n  ")
        projectElement.InsertAfter(newline, importElement) |> ignore
        
        doc.Save(propsFilePath)
        printfn "✓ Added UseLocalCompiler import to Directory.Build.props"
    else
        printfn "✓ UseLocalCompiler import already exists"
    
    // Check if --times flag already exists in any OtherFlags element
    let otherFlagsWithTimes = doc.SelectSingleNode("//OtherFlags[contains(text(), '--times')]")
    
    if isNull otherFlagsWithTimes then
        // Create PropertyGroup with OtherFlags element
        let propertyGroup = doc.CreateElement("PropertyGroup")
        let otherFlags = doc.CreateElement("OtherFlags")
        otherFlags.InnerText <- "$(OtherFlags) --times"
        propertyGroup.AppendChild(otherFlags) |> ignore

        let noWarn = doc.CreateElement("NoWarn")
        noWarn.InnerText <- "$(NoWarn);0075"
        propertyGroup.AppendChild(noWarn) |> ignore
        
        // Reuse the import node we already found (or find it again with the same xpath)
        let importNode = doc.SelectSingleNode(xpath)
        
        // Find the text node after the import (if it exists)
        let nodeAfterImport = 
            if not (isNull importNode) && not (isNull importNode.NextSibling) && importNode.NextSibling.NodeType = XmlNodeType.Text then
                importNode.NextSibling
            else
                null
        
        // Insert PropertyGroup after the import's trailing newline (if present) or after the import itself
        if not (isNull nodeAfterImport) then
            projectElement.InsertAfter(propertyGroup, nodeAfterImport) |> ignore
        else
            projectElement.InsertAfter(propertyGroup, importNode) |> ignore
        
        // Add newline for formatting after PropertyGroup
        let newlineAfter = doc.CreateTextNode("\n  ")
        projectElement.InsertAfter(newlineAfter, propertyGroup) |> ignore
        
        doc.Save(propsFilePath)
        printfn "✓ Added --times flag to OtherFlags"
    else
        printfn "✓ --times flag already exists in OtherFlags"

        let noWarnWith0075 = doc.SelectSingleNode("//NoWarn[contains(text(), '0075')]")
        if isNull noWarnWith0075 then
            let parentPG = otherFlagsWithTimes.ParentNode
            let noWarn = doc.CreateElement("NoWarn")
            noWarn.InnerText <- "$(NoWarn);0075"
            parentPG.AppendChild(noWarn) |> ignore
            doc.Save(propsFilePath)
            printfn "✓ Added NoWarn for FS0075"
        else
            printfn "✓ NoWarn for FS0075 already exists"
else
    printfn "Directory.Build.props does not exist, creating it..."
    let newContent = sprintf "<Project>\n  <Import Project=\"%s\" />\n  <PropertyGroup>\n    <OtherFlags>$(OtherFlags) --times</OtherFlags>\n    <NoWarn>$(NoWarn);0075</NoWarn>\n  </PropertyGroup>\n</Project>\n" absolutePropsPath
    File.WriteAllText(propsFilePath, newContent)
    printfn "✓ Created Directory.Build.props with UseLocalCompiler import and --times flag"

// Print the final content
printfn ""
printfn "Final Directory.Build.props content:"
printfn "-----------------------------------"
let content = File.ReadAllText(propsFilePath)
printfn "%s" content
printfn "-----------------------------------"
printfn "✓ Repository prepared for regression testing"
