#if INTERACTIVE
#r "../../bin/v4.5/FSharp.Compiler.Service.dll"
#r "../../bin/v4.5/FSharp.Compiler.Service.ProjectCracker.dll"
#r "../../packages/NUnit/lib/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.ProjectOptionsTests
#endif

let runningOnMono = try System.Type.GetType("Mono.Runtime") <> null with e ->  false

open System
open System.IO
open NUnit.Framework
open FsUnit
open Microsoft.FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.Service.Tests.Common

#if !NO_PROJECTCRACKER

let normalizePath s = (new Uri(s)).LocalPath

let checkOption (opts:string[]) s = 
    let found = "Found '"+s+"'"
    (if opts |> Array.exists (fun o -> o.EndsWith(s)) then found else "Failed to find '"+s+"'")
       |> shouldEqual found

let checkOptionNotPresent (opts:string[]) s = 
    let found = "Found '"+s+"'"
    let notFound = "Did not expect to find '"+s+"'"
    (if opts |> Array.exists (fun o -> o.EndsWith(s)) then found else notFound)
       |> shouldEqual notFound

let getReferencedFilenames = Array.choose (fun (o:string) -> if o.StartsWith("-r:") then o.[3..] |> (Path.GetFileName >> Some) else None)
let getReferencedFilenamesAndContainingFolders = Array.choose (fun (o:string) -> if o.StartsWith("-r:") then o.[3..] |> (fun r -> ((r |> Path.GetFileName), (r |> Path.GetDirectoryName |> Path.GetFileName)) |> Some) else None)
let getOutputFile = Array.pick (fun (o:string) -> if o.StartsWith("--out:") then o.[6..] |> Some else None)
let getCompiledFilenames = Array.choose (fun (o:string) -> if o.EndsWith(".fs") then o |> (Path.GetFileName >> Some) else None)

[<Test>]
let ``Project file parsing example 1 Default Configuration`` () = 
    let projectFile = __SOURCE_DIRECTORY__ + @"/FSharp.Compiler.Service.Tests.fsproj"
    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile)

    checkOption options.OtherOptions "FSharp.Compiler.Service.dll"
    checkOption options.OtherOptions "FileSystemTests.fs"
    checkOption options.OtherOptions "--define:TRACE"
    checkOption options.OtherOptions "--define:DEBUG"
    checkOption options.OtherOptions "--flaterrors"
    checkOption options.OtherOptions "--simpleresolution"
    checkOption options.OtherOptions "--noframework"

[<Test>]
let ``Project file parsing example 1 Release Configuration`` () = 
    let projectFile = __SOURCE_DIRECTORY__ + @"/FSharp.Compiler.Service.Tests.fsproj"
    // Check with Configuration = Release
    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile, [("Configuration", "Release")])

    checkOption options.OtherOptions "FSharp.Compiler.Service.dll"
    checkOption options.OtherOptions "FileSystemTests.fs"
    checkOption options.OtherOptions "--define:TRACE"
    checkOptionNotPresent options.OtherOptions "--define:DEBUG"
    checkOption options.OtherOptions "--debug:pdbonly"

[<Test>]
let ``Project file parsing example 1 Default configuration relative path`` () = 
    let projectFile = "FSharp.Compiler.Service.Tests.fsproj"
    Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)

    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile)

    checkOption options.OtherOptions "FSharp.Compiler.Service.dll"
    checkOption options.OtherOptions "FileSystemTests.fs"
    checkOption options.OtherOptions "--define:TRACE"
    checkOption options.OtherOptions "--define:DEBUG"
    checkOption options.OtherOptions "--flaterrors"
    checkOption options.OtherOptions "--simpleresolution"
    checkOption options.OtherOptions "--noframework"

[<Test>]
let ``Project file parsing VS2013_FSharp_Portable_Library_net45``() = 
    let projectFile = __SOURCE_DIRECTORY__ + @"/../projects/Sample_VS2013_FSharp_Portable_Library_net45/Sample_VS2013_FSharp_Portable_Library_net45.fsproj"
    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile, [])

    checkOption options.OtherOptions "--targetprofile:netcore"
    checkOption options.OtherOptions "--tailcalls-"

    checkOption options.OtherOptions "FSharp.Core.dll"
    checkOption options.OtherOptions "Microsoft.CSharp.dll"
    checkOption options.OtherOptions "System.Runtime.dll"
    checkOption options.OtherOptions "System.Net.Requests.dll"
    checkOption options.OtherOptions "System.Xml.XmlSerializer.dll"

[<Test>]
let ``Project file parsing Sample_VS2013_FSharp_Portable_Library_net451_adjusted_to_profile78``() = 
    let projectFile = __SOURCE_DIRECTORY__ + @"/../projects/Sample_VS2013_FSharp_Portable_Library_net451_adjusted_to_profile78/Sample_VS2013_FSharp_Portable_Library_net451.fsproj"
    Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__ + @"/../projects/Sample_VS2013_FSharp_Portable_Library_net451_adjusted_to_profile78/")
    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile, [])

    checkOption options.OtherOptions "--targetprofile:netcore"
    checkOption options.OtherOptions "--tailcalls-"

    checkOption options.OtherOptions "FSharp.Core.dll"
    checkOption options.OtherOptions "Microsoft.CSharp.dll"
    checkOption options.OtherOptions "System.Runtime.dll"
    checkOption options.OtherOptions "System.Net.Requests.dll"
    checkOption options.OtherOptions "System.Xml.XmlSerializer.dll"

[<Test>]
let ``Project file parsing -- compile files 1``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/Test1.fsproj")

  p.OtherOptions
  |> getCompiledFilenames
  |> set
  |> should equal (set [ "Test1File1.fs"; "Test1File2.fs" ])

[<Test>]
let ``Project file parsing -- compile files 2``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/Test2.fsproj")

  p.OtherOptions
  |> getCompiledFilenames
  |> set
  |> should equal (set [ "Test2File1.fs"; "Test2File2.fs" ])

[<Test>]
let ``Project file parsing -- bad project file``() =
  let f = normalizePath (__SOURCE_DIRECTORY__ + @"/data/Malformed.fsproj")
  let log = snd (ProjectCracker.GetProjectOptionsFromProjectFileLogged(f))
  log.[f] |> should contain "Microsoft.Build.Exceptions.InvalidProjectFileException"

[<Test>]
let ``Project file parsing -- non-existent project file``() =
  let f = normalizePath (__SOURCE_DIRECTORY__ + @"/data/DoesNotExist.fsproj")
  let log = snd (ProjectCracker.GetProjectOptionsFromProjectFileLogged(f, enableLogging=true))
  log.[f] |> should contain "System.IO.FileNotFoundException"

[<Test>]
let ``Project file parsing -- output file``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/Test1.fsproj")

  let expectedOutputPath =
    normalizePath (__SOURCE_DIRECTORY__ + "/data/Test1/bin/Debug/Test1.dll")

  p.OtherOptions
  |> getOutputFile
  |> should equal expectedOutputPath

[<Test>]
let ``Project file parsing -- references``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/Test1.fsproj")

  let references = getReferencedFilenames p.OtherOptions
  checkOption references "FSharp.Core.dll"
  checkOption references "mscorlib.dll"
  checkOption references "System.Core.dll"
  checkOption references "System.dll"
  printfn "Project file parsing -- references: references = %A" references
  references |> should haveLength 4
  p.ReferencedProjects |> should be Empty

[<Test>]
let ``Project file parsing -- 2nd level references``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/Test2.fsproj")

  let references = getReferencedFilenames p.OtherOptions
  checkOption references "FSharp.Core.dll"
  checkOption references "mscorlib.dll"
  checkOption references "System.Core.dll"
  checkOption references "System.dll"
  checkOption references "Test1.dll"
  printfn "Project file parsing -- references: references = %A" references
  references |> should haveLength 5
  p.ReferencedProjects |> should haveLength 1
  (snd p.ReferencedProjects.[0]).ProjectFileName |> should contain (normalizePath (__SOURCE_DIRECTORY__ + @"/data/Test1.fsproj"))

[<Test>]
let ``Project file parsing -- reference project output file``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/DifferingOutputDir/Dir2/Test2.fsproj")

  let expectedOutputPath =
    normalizePath (__SOURCE_DIRECTORY__ + "/data/DifferingOutputDir/Dir2/OutputDir2/Test2.exe")

  p.OtherOptions
  |> getOutputFile
  |> should equal expectedOutputPath

  p.OtherOptions
  |> Array.choose (fun (o:string) -> if o.StartsWith("-r:") then o.[3..] |> Some else None)
  |> should contain (normalizePath (__SOURCE_DIRECTORY__ + @"/data/DifferingOutputDir/Dir1/OutputDir1/Test1.dll"))


[<Test>]
let ``Project file parsing -- Tools Version 12``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/ToolsVersion12.fsproj")

  checkOption (getReferencedFilenames p.OtherOptions) "System.Core.dll"

[<Test>]
let ``Project file parsing -- Logging``() =
  let f = normalizePath (__SOURCE_DIRECTORY__ + @"/data/ToolsVersion12.fsproj")
  let p, logMap = ProjectCracker.GetProjectOptionsFromProjectFileLogged(f, enableLogging=true)
  let log = logMap.[f]

  if runningOnMono then
    Assert.That(log, Is.StringContaining("Reference System.Core resolved"))
    Assert.That(log, Is.StringContaining("Using task ResolveAssemblyReference from Microsoft.Build.Tasks.ResolveAssemblyReference"))
  else
    Assert.That(log, Is.StringContaining("""Using "ResolveAssemblyReference" task from assembly "Microsoft.Build.Tasks.Core, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"."""))

[<Test>]
let ``Project file parsing -- Full path``() =
  let f = normalizePath (__SOURCE_DIRECTORY__ + @"/data/ToolsVersion12.fsproj")
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(f)
  p.ProjectFileName |> should equal f

[<Test>]
let ``Project file parsing -- project references``() =
  let f1 = normalizePath (__SOURCE_DIRECTORY__ + @"/data/Test1.fsproj")
  let f2 = normalizePath (__SOURCE_DIRECTORY__ + @"/data/Test2.fsproj")
  let options = ProjectCracker.GetProjectOptionsFromProjectFile(f2)

  options.ReferencedProjects |> should haveLength 1
  fst options.ReferencedProjects.[0] |> should endWith "Test1.dll"
  snd options.ReferencedProjects.[0] |> should equal (ProjectCracker.GetProjectOptionsFromProjectFile(f1))

[<Test>]
let ``Project file parsing -- multi language project``() =
  let f = normalizePath (__SOURCE_DIRECTORY__ + @"/data/MultiLanguageProject/ConsoleApplication1.fsproj")

  let options = ProjectCracker.GetProjectOptionsFromProjectFile(f)

  options.ReferencedProjects |> should haveLength 1
  options.ReferencedProjects.[0] |> fst |> should endWith "ConsoleApplication2.exe"

  checkOption options.OtherOptions "ConsoleApplication2.exe"
  checkOption options.OtherOptions "ConsoleApplication3.exe"

[<Test>]
let ``Project file parsing -- PCL profile7 project``() =

    let f = normalizePath (__SOURCE_DIRECTORY__ + @"/../projects/Sample_VS2013_FSharp_Portable_Library_net45/Sample_VS2013_FSharp_Portable_Library_net45.fsproj")

    let options = ProjectCracker.GetProjectOptionsFromProjectFile(f)
    let references =
      options.OtherOptions
      |> getReferencedFilenames
      |> Set.ofArray
    references
    |> shouldEqual
        (set [|"FSharp.Core.dll"; "Microsoft.CSharp.dll"; "Microsoft.VisualBasic.dll";
               "System.Collections.Concurrent.dll"; "System.Collections.dll";
               "System.ComponentModel.Annotations.dll";
               "System.ComponentModel.DataAnnotations.dll";
               "System.ComponentModel.EventBasedAsync.dll"; "System.ComponentModel.dll";
               "System.Core.dll"; "System.Diagnostics.Contracts.dll";
               "System.Diagnostics.Debug.dll"; "System.Diagnostics.Tools.dll";
               "System.Diagnostics.Tracing.dll"; "System.Dynamic.Runtime.dll";
               "System.Globalization.dll"; "System.IO.Compression.dll"; "System.IO.dll";
               "System.Linq.Expressions.dll"; "System.Linq.Parallel.dll";
               "System.Linq.Queryable.dll"; "System.Linq.dll"; "System.Net.Http.dll";
               "System.Net.NetworkInformation.dll"; "System.Net.Primitives.dll";
               "System.Net.Requests.dll"; "System.Net.dll"; "System.Numerics.dll";
               "System.ObjectModel.dll"; "System.Reflection.Context.dll";
               "System.Reflection.Extensions.dll"; "System.Reflection.Primitives.dll";
               "System.Reflection.dll"; "System.Resources.ResourceManager.dll";
               "System.Runtime.Extensions.dll";
               "System.Runtime.InteropServices.WindowsRuntime.dll";
               "System.Runtime.InteropServices.dll"; "System.Runtime.Numerics.dll";
               "System.Runtime.Serialization.Json.dll";
               "System.Runtime.Serialization.Primitives.dll";
               "System.Runtime.Serialization.Xml.dll"; "System.Runtime.Serialization.dll";
               "System.Runtime.dll"; "System.Security.Principal.dll";
               "System.ServiceModel.Duplex.dll"; "System.ServiceModel.Http.dll";
               "System.ServiceModel.NetTcp.dll"; "System.ServiceModel.Primitives.dll";
               "System.ServiceModel.Security.dll"; "System.ServiceModel.Web.dll";
               "System.ServiceModel.dll"; "System.Text.Encoding.Extensions.dll";
               "System.Text.Encoding.dll"; "System.Text.RegularExpressions.dll";
               "System.Threading.Tasks.Parallel.dll"; "System.Threading.Tasks.dll";
               "System.Threading.dll"; "System.Windows.dll"; "System.Xml.Linq.dll";
               "System.Xml.ReaderWriter.dll"; "System.Xml.Serialization.dll";
               "System.Xml.XDocument.dll"; "System.Xml.XmlSerializer.dll"; "System.Xml.dll";
               "System.dll"; "mscorlib.dll"|])

    checkOption options.OtherOptions "--targetprofile:netcore"

[<Test>]
let ``Project file parsing -- PCL profile78 project``() =

    let f = normalizePath (__SOURCE_DIRECTORY__ + @"/../projects/Sample_VS2013_FSharp_Portable_Library_net451_adjusted_to_profile78/Sample_VS2013_FSharp_Portable_Library_net451.fsproj")

    let options = ProjectCracker.GetProjectOptionsFromProjectFile(f)
    let references =
      options.OtherOptions
      |> getReferencedFilenames
      |> Set.ofArray
    references
    |> shouldEqual
        (set [|"FSharp.Core.dll"; "Microsoft.CSharp.dll"; "System.Collections.dll";
               "System.ComponentModel.EventBasedAsync.dll"; "System.ComponentModel.dll";
               "System.Core.dll"; "System.Diagnostics.Contracts.dll";
               "System.Diagnostics.Debug.dll"; "System.Diagnostics.Tools.dll";
               "System.Dynamic.Runtime.dll"; "System.Globalization.dll"; "System.IO.dll";
               "System.Linq.Expressions.dll"; "System.Linq.Queryable.dll"; "System.Linq.dll";
               "System.Net.NetworkInformation.dll"; "System.Net.Primitives.dll";
               "System.Net.Requests.dll"; "System.Net.dll"; "System.ObjectModel.dll";
               "System.Reflection.Extensions.dll"; "System.Reflection.Primitives.dll";
               "System.Reflection.dll"; "System.Resources.ResourceManager.dll";
               "System.Runtime.Extensions.dll";
               "System.Runtime.InteropServices.WindowsRuntime.dll";
               "System.Runtime.Serialization.Json.dll";
               "System.Runtime.Serialization.Primitives.dll";
               "System.Runtime.Serialization.Xml.dll"; "System.Runtime.Serialization.dll";
               "System.Runtime.dll"; "System.Security.Principal.dll";
               "System.ServiceModel.Http.dll"; "System.ServiceModel.Primitives.dll";
               "System.ServiceModel.Security.dll"; "System.ServiceModel.Web.dll";
               "System.ServiceModel.dll"; "System.Text.Encoding.Extensions.dll";
               "System.Text.Encoding.dll"; "System.Text.RegularExpressions.dll";
               "System.Threading.Tasks.dll"; "System.Threading.dll"; "System.Windows.dll";
               "System.Xml.Linq.dll"; "System.Xml.ReaderWriter.dll";
               "System.Xml.Serialization.dll"; "System.Xml.XDocument.dll";
               "System.Xml.XmlSerializer.dll"; "System.Xml.dll"; "System.dll"; "mscorlib.dll"|])

    checkOption options.OtherOptions "--targetprofile:netcore"

[<Test>]
let ``Project file parsing -- PCL profile259 project``() =

    let f = normalizePath (__SOURCE_DIRECTORY__ + @"/../projects/Sample_VS2013_FSharp_Portable_Library_net451_adjusted_to_profile259/Sample_VS2013_FSharp_Portable_Library_net451.fsproj")

    let options = ProjectCracker.GetProjectOptionsFromProjectFile(f)
    let references =
      options.OtherOptions
      |> getReferencedFilenames
      |> Set.ofArray
    references
    |> shouldEqual
        (set [|"FSharp.Core.dll"; "Microsoft.CSharp.dll"; "System.Collections.dll";
               "System.ComponentModel.EventBasedAsync.dll"; "System.ComponentModel.dll";
               "System.Core.dll"; "System.Diagnostics.Contracts.dll";
               "System.Diagnostics.Debug.dll"; "System.Diagnostics.Tools.dll";
               "System.Dynamic.Runtime.dll"; "System.Globalization.dll"; "System.IO.dll";
               "System.Linq.Expressions.dll"; "System.Linq.Queryable.dll"; "System.Linq.dll";
               "System.Net.NetworkInformation.dll"; "System.Net.Primitives.dll";
               "System.Net.Requests.dll"; "System.Net.dll"; "System.ObjectModel.dll";
               "System.Reflection.Extensions.dll"; "System.Reflection.Primitives.dll";
               "System.Reflection.dll"; "System.Resources.ResourceManager.dll";
               "System.Runtime.Extensions.dll";
               "System.Runtime.InteropServices.WindowsRuntime.dll";
               "System.Runtime.Serialization.Json.dll";
               "System.Runtime.Serialization.Primitives.dll";
               "System.Runtime.Serialization.Xml.dll"; "System.Runtime.Serialization.dll";
               "System.Runtime.dll"; "System.Security.Principal.dll";
               "System.ServiceModel.Web.dll"; "System.Text.Encoding.Extensions.dll";
               "System.Text.Encoding.dll"; "System.Text.RegularExpressions.dll";
               "System.Threading.Tasks.dll"; "System.Threading.dll"; "System.Windows.dll";
               "System.Xml.Linq.dll"; "System.Xml.ReaderWriter.dll";
               "System.Xml.Serialization.dll"; "System.Xml.XDocument.dll";
               "System.Xml.XmlSerializer.dll"; "System.Xml.dll"; "System.dll"; "mscorlib.dll"|])

    checkOption options.OtherOptions "--targetprofile:netcore"

[<Test>]
let ``Project file parsing -- Exe with a PCL reference``() =

    let f = normalizePath(__SOURCE_DIRECTORY__ + @"/data/sqlite-net-spike/sqlite-net-spike.fsproj")

    let p = ProjectCracker.GetProjectOptionsFromProjectFile(f)
    let references = getReferencedFilenames p.OtherOptions |> set
    references |> should contain "FSharp.Core.dll"
    references |> should contain "SQLite.Net.Platform.Generic.dll"
    references |> should contain "SQLite.Net.Platform.Win32.dll"
    references |> should contain "SQLite.Net.dll"
    references |> should contain "System.Collections.Concurrent.dll"
    references |> should contain "System.Linq.Queryable.dll"
    references |> should contain "System.Resources.ResourceManager.dll"
    references |> should contain "System.Threading.dll"
    references |> should contain "System.dll"
    references |> should contain "mscorlib.dll"
    references |> should contain "System.Reflection.dll"
    references |> should contain "System.Reflection.Emit.Lightweight.dll"


[<Test>]
let ``Project file parsing -- project file contains project reference to out-of-solution project and is used in release mode``() =

    let f = normalizePath(__SOURCE_DIRECTORY__ + @"/data/TestProject/TestProject.fsproj")
    let p = ProjectCracker.GetProjectOptionsFromProjectFile(f,[("Configuration","Release")])
    let references = getReferencedFilenamesAndContainingFolders p.OtherOptions |> set
    // Check the reference is to a release DLL
    references |> should contain ("TestTP.dll", "Release")

[<Test>]
let ``Project file parsing -- project file contains project reference to out-of-solution project and is used in debug mode``() =

    let f = normalizePath(__SOURCE_DIRECTORY__ + @"/data/TestProject/TestProject.fsproj")
    let p = ProjectCracker.GetProjectOptionsFromProjectFile(f,[("Configuration","Debug")])
    let references = getReferencedFilenamesAndContainingFolders p.OtherOptions |> set
    // Check the reference is to a debug DLL
    references |> should contain ("TestTP.dll", "Debug")

[<Test>]
let ``Project file parsing -- space in file name``() =
  let p = ProjectCracker.GetProjectOptionsFromProjectFile(__SOURCE_DIRECTORY__ + @"/data/Space in name.fsproj")

  p.OtherOptions
  |> getCompiledFilenames
  |> set
  |> should equal (set [ "Test2File1.fs"; "Test2File2.fs" ])

[<Test>]
let ``Project file parsing -- report files``() =
  let programFilesx86Folder = System.Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
  if not runningOnMono then
   for f in Directory.EnumerateFiles(programFilesx86Folder + @"\Reference Assemblies\Microsoft\FSharp\","*",SearchOption.AllDirectories) do 
     printfn "File: %s" f
   for f in Directory.EnumerateFiles(programFilesx86Folder + @"\Microsoft SDKs\F#\4.1\","*",SearchOption.AllDirectories) do 
     printfn "File: %s" f

#endif

[<Test>]
let ``Test ProjectFileNames order for GetProjectOptionsFromScript`` () = // See #594
    let test scriptName expected =
        let scriptPath = __SOURCE_DIRECTORY__ + @"/data/ScriptProject/" + scriptName + ".fsx"
        let scriptSource = File.ReadAllText scriptPath
        let projOpts =
            checker.GetProjectOptionsFromScript(scriptPath, scriptSource)
            |> Async.RunSynchronously
        projOpts.ProjectFileNames
        |> Array.map Path.GetFileNameWithoutExtension
        |> (=) expected
        |> shouldEqual true
    test "Main1" [|"BaseLib1"; "Lib1"; "Lib2"; "Main1"|]
    test "Main2" [|"BaseLib1"; "Lib1"; "Lib2"; "Lib3"; "Main2"|]
    test "Main3" [|"Lib3"; "Lib4"; "Main3"|]
    test "Main4" [|"BaseLib2"; "Lib5"; "BaseLib1"; "Lib1"; "Lib2"; "Main4"|]




