// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// This namespace contains FSharp.PowerPack extensions for FSharp.Build.dll. MSBuild tasks for the FsYacc and FsLex tools.
namespace Microsoft.FSharp.Build
type Fsc = class
             inherit Microsoft.Build.Utilities.ToolTask
             new : unit -> Fsc
             override GenerateCommandLineCommands : unit -> System.String
             override GenerateFullPathToTool : unit -> System.String
             override ToolName : System.String
             override StandardErrorEncoding : System.Text.Encoding
             override StandardOutputEncoding : System.Text.Encoding

             member internal InternalGenerateFullPathToTool : unit -> System.String
             member internal InternalGenerateCommandLineCommands : unit -> System.String
             member internal InternalGenerateResponseFileCommands : unit -> System.String
             member internal InternalExecuteTool : string * string * string -> int
             member internal GetCapturedArguments : unit -> string[]
             member BaseAddress : string with get,set
             member CodePage : string with get,set
             member DebugSymbols : bool with get,set
             member DebugType : string with get,set
             member DefineConstants : Microsoft.Build.Framework.ITaskItem [] with get,set
             member DisabledWarnings : string with get,set
             member DocumentationFile : string with get,set
             member Embed : string with get,set
             member EmbedAllSources : bool with get,set
             member GenerateInterfaceFile : string with get,set
             member KeyFile : string with get,set
             member NoFramework : bool with get,set
             member Optimize : bool with get,set
             member Tailcalls : bool with get,set
             member OtherFlags : string with get,set
             member OutputAssembly : string with get,set
             member PdbFile : string with get,set
             member Platform : string with get,set
             member Prefer32Bit : bool with get,set
             member VersionFile : string with get,set
             member References : Microsoft.Build.Framework.ITaskItem [] with get,set
             member ReferencePath : string with get,set
             member Resources : Microsoft.Build.Framework.ITaskItem [] with get,set
             member SourceLink : string with get,set
             member Sources : Microsoft.Build.Framework.ITaskItem [] with get,set
             member TargetType : string with get,set
#if FX_ATLEAST_35
#else
             member ToolExe : string with get,set
#endif             
             member ToolPath : string with get,set
             member TreatWarningsAsErrors : bool with get,set
             member Utf8Output : bool with get,set
             member VisualStudioStyleErrors : bool with get,set
             member LCID : string with get,set
             member WarningLevel : string with get,set
             member WarningsAsErrors : string with get,set
             member Win32ResourceFile : string with get,set
             member Win32ManifestFile : string with get,set
             member SubsystemVersion : string with get,set
             member HighEntropyVA : bool with get,set
             member TargetProfile : string with get,set
             member DotnetFscCompilerPath : string with get,set
           end
