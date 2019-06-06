// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.Build.Tasks
namespace Microsoft.Build.Utilities
namespace Microsoft.Build.Framework
namespace Microsoft.Build.BuildEngine

#if FX_RESHAPED_MSBUILD

namespace Microsoft.Build.Framework
open System.Collections

type ITaskItem =
    abstract member ItemSpec : string with get, set
    abstract member MetadataNames : ICollection with get
    abstract member MetadataCount : int with get

    abstract member GetMetadata : string -> string
    abstract member SetMetadata : string*string -> unit
    abstract member RemoveMetadata : string -> unit
    abstract member CopyMetadataTo : ITaskItem -> unit
    abstract member CloneCustomMetadata : IDictionary

namespace Microsoft.Build.Utilities
open Microsoft.Build.Framework
open Microsoft.FSharp.Core.ReflectionAdapters
open System
open System.Collections
open System.Reflection

type TaskItem (itemSpec:string) =
    let assembly = Assembly.Load(new AssemblyName("Microsoft.Build.Utilities.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"))
    let buildUtilitiesTaskType = assembly.GetType("Microsoft.Build.Utilities.Task")
    let instance = Activator.CreateInstance(buildUtilitiesTaskType, [|itemSpec|])

    interface ITaskItem with
        member this.ItemSpec
            with get () :string = (instance.GetPropertyValue("ItemSpec") :?> string)
            and set (value:string) =  (instance.SetPropertyValue("ItemSpec", value)); ()
        member this.MetadataNames
            with get () :ICollection = (instance.GetPropertyValue("MetadataNames") :?> ICollection)
        member this.MetadataCount
            with get () :int = (instance.GetPropertyValue("MetadataCount") :?> int)
        member this.CopyMetadataTo(iTaskItem) =
            let m = buildUtilitiesTaskType.GetMethod("CopyMetadataTo", [| typeof<ITaskItem> |])
            m.Invoke(instance, [|iTaskItem :>obj|]) |> ignore
        member this.CloneCustomMetadata =
            let m = buildUtilitiesTaskType.GetMethod("CloneCustomMetadata", [||])
            (m.Invoke(instance,[||])) :?>IDictionary
        member this.GetMetadata(metadataName) =
            let m = buildUtilitiesTaskType.GetMethod("GetMetadata", [|typeof<string>|])
            (m.Invoke(instance,[|metadataName|])) :?>string
        member this.RemoveMetadata(metadataName) =
            let m = buildUtilitiesTaskType.GetMethod("RemoveMetadata", [|typeof<string>|])
            (m.Invoke(instance,[|metadataName|])) :?>string |>ignore
        member this.SetMetadata(metadataName, metadataValue) =
            let m = buildUtilitiesTaskType.GetMethod("SetMetadata", [|typeof<string>;typeof<string>|])
            (m.Invoke(instance,[|metadataName; metadataValue|])) |>ignore

namespace FSharp.Compiler
open System
open System.Collections
open System.Collections.Concurrent
open System.IO
open System.Linq
open System.Runtime.Versioning
open FSComp
open Microsoft.Win32

module internal MsBuildAdapters = 

    open Microsoft.FSharp.Core.ReflectionAdapters

    /// <summary>
    /// Used to specify the targeted version of the .NET Framework for some methods of ToolLocationHelper.  This is meant to mimic
    /// the official version here: https://source.dot.net/#q=TargetDotNetFrameworkVersion.
    /// </summary>
    type public TargetDotNetFrameworkVersion =
    | Version11 = 0
    | Version20 = 1
    | Version30 = 2
    | Version35 = 3
    | Version40 = 4
    | Version45 = 5
    | Version451 = 6
    | Version46 = 7
    | Version461 = 8
    | Version452 = 9
    | Version462 = 10
    | Version47 = 11
    | Version471 = 12
    | Version472 = 13
    | VersionLatest = 13  //TargetDotNetFrameworkVersion.Version472

    /// <summary>
    /// Used to specify the targeted bitness of the .NET Framework for some methods of ToolLocationHelper
    /// </summary>
    type DotNetFrameworkArchitecture =
    | Current = 0                                   // Indicates the .NET Framework that is currently being run under
    | Bitness32 = 1                                 // Indicates the 32-bit .NET Framework
    | Bitness64 = 2                                 // Indicates the 64-bit .NET Framework

module internal ToolLocationHelper =
    open Microsoft.Build.Framework
    open Microsoft.FSharp.Core.ReflectionAdapters
    open System.Linq
    open System.Reflection
    open MsBuildAdapters

    let dotNetFrameworkIdentifier = ".NETFramework"

    // .net versions.
    let dotNetFrameworkVersion11  = Version(1, 1)
    let dotNetFrameworkVersion20  = Version(2, 0)
    let dotNetFrameworkVersion30  = Version(3, 0)
    let dotNetFrameworkVersion35  = Version(3, 5)
    let dotNetFrameworkVersion40  = Version(4, 0)
    let dotNetFrameworkVersion45  = Version(4, 5)
    let dotNetFrameworkVersion451 = Version(4, 5, 1)
    let dotNetFrameworkVersion452 = Version(4, 5, 2)
    let dotNetFrameworkVersion46  = Version(4, 6)
    let dotNetFrameworkVersion461 = Version(4, 6, 1)
    let dotNetFrameworkVersion462 = Version(4, 6, 2)
    let dotNetFrameworkVersion47  = Version(4, 7)
    let dotNetFrameworkVersion471 = Version(4, 7, 1)
    let dotNetFrameworkVersion472 = Version(4, 7, 2)

    // visual studio versions.
    let visualStudioVersion100 = new Version(10, 0);
    let visualStudioVersion110 = new Version(11, 0);
    let visualStudioVersion120 = new Version(12, 0);
    let visualStudioVersion140 = new Version(14, 0);
    let visualStudioVersion150 = new Version(15, 0);

    // keep this up-to-date; always point to the latest visual studio version.
    let visualStudioVersionLatest = visualStudioVersion150;

    let dotNetFrameworkRegistryPath = "SOFTWARE\\Microsoft\\.NETFramework";
    let dotNetFrameworkSetupRegistryPath = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";
    let dotNetFrameworkSetupRegistryInstalledName = "Install";

    let fullDotNetFrameworkRegistryKey = "HKEY_LOCAL_MACHINE\\" + dotNetFrameworkRegistryPath;
    let dotNetFrameworkAssemblyFoldersRegistryPath = dotNetFrameworkRegistryPath + "\\AssemblyFolders";
    let referenceAssembliesRegistryValueName = "All Assemblies In";

    let dotNetFrameworkSdkInstallKeyValueV11 = "SDKInstallRootv1.1";
    let dotNetFrameworkVersionFolderPrefixV11 = "v1.1"; // v1.1 is for Everett.
    let dotNetFrameworkVersionV11 = "v1.1.4322";       // full Everett version to pass to NativeMethodsShared.GetRequestedRuntimeInfo().
    let dotNetFrameworkRegistryKeyV11 = dotNetFrameworkSetupRegistryPath + "\\" + dotNetFrameworkVersionV11;

    let dotNetFrameworkSdkInstallKeyValueV20 = "SDKInstallRootv2.0";
    let dotNetFrameworkVersionFolderPrefixV20 = "v2.0"; // v2.0 is for Whidbey.
    let dotNetFrameworkVersionV20 = "v2.0.50727"; // full Whidbey version to pass to NativeMethodsShared.GetRequestedRuntimeInfo().
    let dotNetFrameworkRegistryKeyV20 = dotNetFrameworkSetupRegistryPath + "\\" + dotNetFrameworkVersionV20;

    let dotNetFrameworkVersionFolderPrefixV30 = "v3.0"; // v3.0 is for WinFx.
    let dotNetFrameworkVersionV30 = "v3.0"; // full WinFx version to pass to NativeMethodsShared.GetRequestedRuntimeInfo().
    let dotNetFrameworkAssemblyFoldersRegistryKeyV30 = dotNetFrameworkAssemblyFoldersRegistryPath + "\\" + dotNetFrameworkVersionFolderPrefixV30;
    let dotNetFrameworkRegistryKeyV30 = dotNetFrameworkSetupRegistryPath + "\\" + dotNetFrameworkVersionFolderPrefixV30 + "\\Setup";

    let fallbackDotNetFrameworkSdkRegistryInstallPath = "SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows";
    let fallbackDotNetFrameworkSdkInstallKeyValue = "CurrentInstallFolder";

    let dotNetFrameworkSdkRegistryPathForV35ToolsOnWinSDK70A = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx35Tools-x86";
    let fullDotNetFrameworkSdkRegistryPathForV35ToolsOnWinSDK70A = "HKEY_LOCAL_MACHINE\\" + dotNetFrameworkSdkRegistryPathForV35ToolsOnWinSDK70A;

    let dotNetFrameworkSdkRegistryPathForV35ToolsOnManagedToolsSDK80A = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx35Tools-x86";
    let fullDotNetFrameworkSdkRegistryPathForV35ToolsOnManagedToolsSDK80A = "HKEY_LOCAL_MACHINE\\" + dotNetFrameworkSdkRegistryPathForV35ToolsOnManagedToolsSDK80A;

    let dotNetFrameworkVersionFolderPrefixV35 = "v3.5"; // v3.5 is for Orcas.
    let dotNetFrameworkRegistryKeyV35 = dotNetFrameworkSetupRegistryPath + "\\" + dotNetFrameworkVersionFolderPrefixV35;

    let fullDotNetFrameworkSdkRegistryKeyV35OnVS10 = fullDotNetFrameworkSdkRegistryPathForV35ToolsOnWinSDK70A;
    let fullDotNetFrameworkSdkRegistryKeyV35OnVS11 = fullDotNetFrameworkSdkRegistryPathForV35ToolsOnManagedToolsSDK80A;

    let dotNetFrameworkVersionFolderPrefixV40 = "v4.0";
    let ToolsVersionsRegistryPath = @"SOFTWARE\Microsoft\MSBuild\ToolsVersions";       // Path to the ToolsVersion definitions in the registry


    let programFiles = Environment.GetEnvironmentVariable("ProgramFiles")

    let programFiles32 =
        // On a 64 bit machine we always want to use the program files x86.  If we are running as a 64 bit process then this variable will be set correctly
        // If we are on a 32 bit machine or running as a 32 bit process then this variable will be null and the programFiles variable will be correct.
        let programFilesX86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        if programFilesX86 = null then 
            programFiles 
        else 
            programFilesX86

    let programFilesX64 =
        if String.Equals(programFiles, programFiles32) then
            // either we're in a 32-bit window, or we're on a 32-bit machine.  
            // if we're on a 32-bit machine, ProgramW6432 won't exist
            // if we're on a 64-bit machine, ProgramW6432 will point to the correct Program Files. 
            Environment.GetEnvironmentVariable("ProgramW6432");
        else
            // 64-bit window on a 64-bit machine; %ProgramFiles% points to the 64-bit 
            // Program Files already. 
            programFiles;

    let getArgumentException version =
        let _, msg = SR.toolLocationHelperUnsupportedFrameworkVersion(version.ToString())
        new ArgumentException(msg)

    let TargetDotNetFrameworkVersionToSystemVersion version =
        match version with
        | TargetDotNetFrameworkVersion.Version11 -> dotNetFrameworkVersion11
        | TargetDotNetFrameworkVersion.Version20 -> dotNetFrameworkVersion20
        | TargetDotNetFrameworkVersion.Version30 -> dotNetFrameworkVersion30
        | TargetDotNetFrameworkVersion.Version35 -> dotNetFrameworkVersion35
        | TargetDotNetFrameworkVersion.Version40 -> dotNetFrameworkVersion40
        | TargetDotNetFrameworkVersion.Version45 -> dotNetFrameworkVersion45
        | TargetDotNetFrameworkVersion.Version451 -> dotNetFrameworkVersion451
        | TargetDotNetFrameworkVersion.Version452 -> dotNetFrameworkVersion452
        | TargetDotNetFrameworkVersion.Version46 -> dotNetFrameworkVersion46
        | TargetDotNetFrameworkVersion.Version461 -> dotNetFrameworkVersion461
        | TargetDotNetFrameworkVersion.Version462 -> dotNetFrameworkVersion462
        | TargetDotNetFrameworkVersion.Version47 -> dotNetFrameworkVersion47
        | TargetDotNetFrameworkVersion.Version471 -> dotNetFrameworkVersion471
        | TargetDotNetFrameworkVersion.Version472 -> dotNetFrameworkVersion472
        | _ -> raise (getArgumentException version)

    let complusInstallRoot = Environment.GetEnvironmentVariable("COMPLUS_INSTALLROOT")
    let complusVersion = Environment.GetEnvironmentVariable("COMPLUS_VERSION")

    type DotNetFrameworkSpec (version, dotNetFrameworkRegistryKey, dotNetFrameworkSetupRegistryInstalledName, dotNetFrameworkVersionFolderPrefix, dotNetFrameworkSdkRegistryToolsKey, dotNetFrameworkSdkRegistryInstallationFolderName, hasMSBuild, _vsVersion) =

        let _HKLM = "HKEY_LOCAL_MACHINE"
        let _microsoftSDKsRegistryKey = @"SOFTWARE\Microsoft\Microsoft SDKs"
        let dotNetFrameworkFolderPrefix = dotNetFrameworkVersionFolderPrefix
        let frameworkName = FrameworkName(dotNetFrameworkIdentifier, version)

#if !FX_NO_WIN_REGISTRY
        let findRegistryValueUnderKey registryBaseKeyName registryKeyName registryView =
         try
            use baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)
            use subKey = baseKey.OpenSubKey(registryBaseKeyName)
            match subKey with
            | null -> None
            | _ as x -> 
                let keyValue = x.GetValue(registryKeyName)
                match keyValue with
                | null -> None
                | _ as x -> Some (x.ToString())
         with _ -> None
#endif

        let findRegistryValueUnderKey registryBaseKeyName registryKeyName =
#if FX_NO_WIN_REGISTRY
            ignore registryBaseKeyName 
            ignore registryKeyName 
            None
#else
            findRegistryValueUnderKey registryBaseKeyName registryKeyName RegistryView.Default
#endif
        let CheckForFrameworkInstallation registryEntryToCheckInstall registryValueToCheckInstall =
            // Complus is not set we need to make sure the framework we are targeting is installed. Check the registry key before trying to find the directory.
            // If complus is set then we will return that directory as the framework directory, there is no need to check the registry value for the framework and it may not even be installed.

            if (String.IsNullOrEmpty(complusInstallRoot) && String.IsNullOrEmpty(complusVersion)) then

                // If the registry entry is 1 then the framework is installed. Go ahead and find the directory. If it is not 1 then the framework is not installed, return null

                match findRegistryValueUnderKey registryEntryToCheckInstall registryValueToCheckInstall with
                | None -> false
                | Some x -> if String.Compare("1", x, StringComparison.OrdinalIgnoreCase) = 0 then true else false

            else true

        let PickDirectoryFromInstallRoot prefix (installRoot:string)  arch =
            let searchPattern = prefix + "*"
            let calculatePath =
                let bitness s = if arch = DotNetFrameworkArchitecture.Bitness64 then s + @"64\" else s + @"\"
                let trim = if installRoot.EndsWith(@"\") then installRoot.Substring(0, installRoot.Length - 1) else installRoot
                let i64 = trim.IndexOf("Framework64", StringComparison.OrdinalIgnoreCase)
                if i64 = -1 then bitness trim else bitness (trim.Substring(0, i64 + 9))

            if Directory.Exists(calculatePath) then
                let directories = Directory.GetDirectories(calculatePath, searchPattern) |> Array.sort
                if directories.Length = 0 then None
                else
                    // We don't care which one we choose, but we want to be predictible.
                    // The intention here is to choose the alphabetical maximum.
                    let mutable max = directories |> Array.last
                    Some max
            else
                None

        let FindDotNetFrameworkPath prefix registryEntryToCheckInstall registryValueToCheckInstall arch =
            // If the COMPLUS variables are set, they override everything -- that's the directory we want.
            if String.IsNullOrEmpty(complusInstallRoot) || String.IsNullOrEmpty(complusVersion) then
                // We haven't managed to use exact methods to locate the FX, Since we run on coreclr 
                // we can't guess where by using the currently executing runtime
                let installRootFromReg = findRegistryValueUnderKey registryEntryToCheckInstall registryValueToCheckInstall
                match installRootFromReg with
                | None -> None
                | Some x -> PickDirectoryFromInstallRoot prefix x arch
            else 
                Some (Path.Combine(complusInstallRoot, complusVersion))


        /// <summary>
        /// Take the parts of the Target framework moniker and formulate the reference assembly path based on the the following pattern:
        /// For a framework and version:
        ///     $(TargetFrameworkRootPath)\$(TargetFrameworkIdentifier)\$(TargetFrameworkVersion)
        /// For a subtype:
        ///     $(TargetFrameworkRootPath)\$(TargetFrameworkIdentifier)\$(TargetFrameworkVersion)\SubType\$(TargetFrameworkSubType)
        /// e.g.NET Framework v4.0 would locate its reference assemblies in:
        ///     \Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0
        /// e.g.Silverlight v2.0 would locate its reference assemblies in:
        ///     \Program Files\Reference Assemblies\Microsoft\Framework\Silverlight\v2.0
        /// e.g.NET Compact Framework v3.5, subtype PocketPC would locate its reference assemblies in:
        ///     \Program Files\Reference Assemblies\Microsoft\Framework\.NETCompactFramework\v3.5\SubType\PocketPC
        /// </summary>
        /// <returns>The path to the reference assembly location</returns>
        let GenerateReferenceAssemblyPath targetFrameworkRootPath (frameworkName:FrameworkName) =
            match targetFrameworkRootPath with
            | Some x ->
                try
                    let basePath = Path.Combine(x, frameworkName.Identifier, "v" + frameworkName.Version.ToString())
                    let withProfile root =
                        if not (String.IsNullOrEmpty(frameworkName.Profile)) then
                            Path.Combine(root, "Profile", frameworkName.Profile)
                        else root
                    Some (Path.GetFullPath(withProfile basePath) + @"\")
                with _ ->
                    // The compiler does not see the massage above an as exception;
                    None
            | _ -> None


        /// <summary>
        /// Generate the path to the program files reference assembly location by taking in the program files special folder and then 
        /// using that path to generate the path to the reference assemblies location.
        /// </summary>
        let generateProgramFilesReferenceAssemblyRoot =
            try
                Some(Path.GetFullPath( Path.Combine(programFiles32, "Reference Assemblies\\Microsoft\\Framework") ))
            with _ ->
                None

        let pathToDotNetFrameworkReferenceAssemblies =
            match GenerateReferenceAssemblyPath generateProgramFilesReferenceAssemblyRoot frameworkName with
            | Some x when Directory.Exists(x) -> x + @"\"
            | _ -> ""


        member this.Version = version
        member this.dotNetFrameworkRegistryKey = dotNetFrameworkRegistryKey
        member this.dotNetFrameworkSetupRegistryInstalledName = dotNetFrameworkSetupRegistryInstalledName
        member this.dotNetFrameworkSdkRegistryToolsKey = dotNetFrameworkSdkRegistryToolsKey
        member this.DotNetFrameworkSdkRegistryInstallationFolderName = dotNetFrameworkSdkRegistryInstallationFolderName
        member this.HasMSBuild = hasMSBuild

        member this.pathsToDotNetFramework = new ConcurrentDictionary<DotNetFrameworkArchitecture, string>()
        member this.pathsToDotNetFrameworkSdkTools = new ConcurrentDictionary<Version, string>()
        member this.pathToWindowsSdk = "Todo:   Review dow we really need this"

//            /// <summary>
//            /// Gets the full registry key of this .net framework Sdk for the given visual studio version.
//            /// i.e. "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86" for .net v4.5 on VS11.
//            /// </summary>
//            public virtual string GetDotNetFrameworkSdkRootRegistryKey(VisualStudioSpec visualStudioSpec)
//            {
//                return string.Join(@"\", HKLM, MicrosoftSDKsRegistryKey, visualStudioSpec.DotNetFrameworkSdkRegistryKey, this.dotNetFrameworkSdkRegistryToolsKey);
//            }

        // Doesn't need to be virtual @@@@@
        abstract member GetPathToDotNetFramework: DotNetFrameworkArchitecture -> string
        default this.GetPathToDotNetFramework arch =
            match this.pathsToDotNetFramework.TryGetValue arch with
            | true, x -> x
            | _ ->
                if not (CheckForFrameworkInstallation this.dotNetFrameworkRegistryKey this.dotNetFrameworkSetupRegistryInstalledName) then null
                else
                    // We're not installed and we haven't found this framework path yet -- so find it!
                    let fwp:string option = (FindDotNetFrameworkPath dotNetFrameworkFolderPrefix dotNetFrameworkRegistryKey this.dotNetFrameworkSetupRegistryInstalledName arch)
                    match fwp with
                    | Some x ->
                        // For .net frameworks that should have msbuild.exe is it there
                        if hasMSBuild && not (File.Exists(Path.Combine(x, "msbuild.exe"))) then null
                        else this.pathsToDotNetFramework.[arch] <- x; x
                    | _ -> null

        // Doesn't need to be virtual @@@@@
        /// <summary>
        /// Gets the full path of reference assemblies folder.
        /// i.e. "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\" for .net v4.5.
        /// </summary>
        abstract member GetPathToDotNetFrameworkReferenceAssemblies: string
        default this.GetPathToDotNetFrameworkReferenceAssemblies = pathToDotNetFrameworkReferenceAssemblies

//            /// <summary>
//            /// Gets the full path of .net framework sdk tools for the given visual studio version.
//            /// i.e. "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\" for .net v4.5 on VS11.
//            /// </summary>
//            public virtual string GetPathToDotNetFrameworkSdkTools(VisualStudioSpec visualStudioSpec)
//            {
//                string cachedPath;
//                if (this.pathsToDotNetFrameworkSdkTools.TryGetValue(visualStudioSpec.Version, out cachedPath))
//                {
//                    return cachedPath;
//                }
//
//                string registryPath = string.Join(@"\", MicrosoftSDKsRegistryKey, visualStudioSpec.DotNetFrameworkSdkRegistryKey, this.dotNetFrameworkSdkRegistryToolsKey);
//
//                // For the Dev10 SDK, we check the registry that corresponds to the current process' bitness, rather than
//                // always the 32-bit one the way we do for Dev11 and onward, since that's what we did in Dev10 as well.
//                // As of Dev11, the SDK reg keys are installed in the 32-bit registry. 
//                RegistryView registryView = visualStudioSpec.Version == visualStudioVersion100 ? RegistryView.Default : RegistryView.Registry32;
//
//                string generatedPathToDotNetFrameworkSdkTools = FindRegistryValueUnderKey(
//                    registryPath,
//                    this.dotNetFrameworkSdkRegistryInstallationFolderName,
//                    registryView);
//
//                if (string.IsNullOrEmpty(generatedPathToDotNetFrameworkSdkTools))
//                {
//                    // Fallback mechanisms.
//
//                    // Try to find explicit fallback rule.
//                    // i.e. v4.5.1 on VS12 fallbacks to v4.5 on VS12.
//                    bool foundExplicitRule = false;
//                    for (int i = 0; i < s_explicitFallbackRulesForPathToDotNetFrameworkSdkTools.GetLength(0); ++i)
//                    {
//                        var trigger = s_explicitFallbackRulesForPathToDotNetFrameworkSdkTools[i, 0];
//                        if (trigger.Item1 == this.version && trigger.Item2 == visualStudioSpec.Version)
//                        {
//                            foundExplicitRule = true;
//                            var fallback = s_explicitFallbackRulesForPathToDotNetFrameworkSdkTools[i, 1];
//                            generatedPathToDotNetFrameworkSdkTools = FallbackToPathToDotNetFrameworkSdkToolsInPreviousVersion(fallback.Item1, fallback.Item2);
//                            break;
//                        }
//                    }
//
//                    // Otherwise, fallback to previous VS.
//                    // i.e. fallback to v110 if the current visual studio version is v120.
//                    if (!foundExplicitRule)
//                    {
//                        int index = Array.IndexOf(s_visualStudioSpecs, visualStudioSpec);
//                        if (index > 0)
//                        {
//                            // The items in the array "visualStudioSpecs" must be ordered by version. That would allow us to fallback to the previous visual studio version easily.
//                            VisualStudioSpec fallbackVisualStudioSpec = s_visualStudioSpecs[index - 1];
//                            generatedPathToDotNetFrameworkSdkTools = FallbackToPathToDotNetFrameworkSdkToolsInPreviousVersion(this.version, fallbackVisualStudioSpec.Version);
//                        }
//                    }
//                }
//
//                if (string.IsNullOrEmpty(generatedPathToDotNetFrameworkSdkTools))
//                {
//                    // Fallback to "default" ultimately.
//                    generatedPathToDotNetFrameworkSdkTools = FallbackToDefaultPathToDotNetFrameworkSdkTools(this.version);
//                }
//
//                if (!string.IsNullOrEmpty(generatedPathToDotNetFrameworkSdkTools))
//                {
//                    this.pathsToDotNetFrameworkSdkTools[visualStudioSpec.Version] = generatedPathToDotNetFrameworkSdkTools;
//                }
//
//                return generatedPathToDotNetFrameworkSdkTools;
//            }
//
//            /// <summary>
//            /// Gets the full path of .net framework sdk.
//            /// i.e. "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\" for .net v4.5 on VS11.
//            /// </summary>
//            public virtual string GetPathToDotNetFrameworkSdk(VisualStudioSpec visualStudioSpec)
//            {
//                string pathToBinRoot = this.GetPathToDotNetFrameworkSdkTools(visualStudioSpec);
//                pathToBinRoot = RemoveDirectories(pathToBinRoot, 2);
//                return pathToBinRoot;
//            }
//
//            /// <summary>
//            /// Gets the full path of reference assemblies folder.
//            /// i.e. "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\" for .net v4.5.
//            /// </summary>
//            public virtual string GetPathToDotNetFrameworkReferenceAssemblies()
//            {
//                if (this.pathToDotNetFrameworkReferenceAssemblies == null)
//                {
//                    // when a user requests the 40 reference assembly path we don't need to read the redist list because we will not be chaining so we may as well just
//                    // generate the path and save us some time.
//                    string referencePath = GenerateReferenceAssemblyPath(FrameworkLocationHelper.programFilesReferenceAssemblyLocation, this.FrameworkName);
//                    if (Directory.Exists(referencePath))
//                    {
//                        this.pathToDotNetFrameworkReferenceAssemblies = FileUtilities.EnsureTrailingSlash(referencePath);
//                    }//                }
//
//                return this.pathToDotNetFrameworkReferenceAssemblies;
//            }
//
//            /// <summary>
//            /// Gets the full path of the corresponding windows sdk shipped with this .net framework.
//            /// i.e. "C:\Program Files (x86)\Windows Kits\8.0\" for v8.0 (shipped with .net v4.5 and VS11).
//            /// </summary>
//            public virtual string GetPathToWindowsSdk()
//            {
//                if (this.pathToWindowsSdk == null)
//                {
//                    ErrorUtilities.VerifyThrowArgument(this.visualStudioVersion != null, "FrameworkLocationHelper.UnsupportedFrameworkVersionForWindowsSdk", this.version);
//
//                    var visualStudioSpec = GetVisualStudioSpec(this.visualStudioVersion);
//
//                    if (string.IsNullOrEmpty(visualStudioSpec.WindowsSdkRegistryKey) || string.IsNullOrEmpty(visualStudioSpec.WindowsSdkRegistryInstallationFolderName))
//                    {
//                        ErrorUtilities.ThrowArgument("FrameworkLocationHelper.UnsupportedFrameworkVersionForWindowsSdk", this.version);
//                    }
//
//                    string registryPath = string.Join(@"\", MicrosoftSDKsRegistryKey, "Windows", visualStudioSpec.WindowsSdkRegistryKey);
//
//                    // As of Dev11, the SDK reg keys are installed in the 32-bit registry. 
//                    this.pathToWindowsSdk = FindRegistryValueUnderKey(
//                        registryPath,
//                        visualStudioSpec.WindowsSdkRegistryInstallationFolderName,
//                        RegistryView.Registry32);
//                }
//
//                return this.pathToWindowsSdk;
//            }
//
//            protected static string FallbackToPathToDotNetFrameworkSdkToolsInPreviousVersion(Version dotNetFrameworkVersion, Version visualStudioVersion)
//            {
//                VisualStudioSpec visualStudioSpec;
//                DotNetFrameworkSpec dotNetFrameworkSpec;
//                if (s_visualStudioSpecDict.TryGetValue(visualStudioVersion, out visualStudioSpec)
//                    && s_dotNetFrameworkSpecDict.TryGetValue(dotNetFrameworkVersion, out dotNetFrameworkSpec)
//                    && visualStudioSpec.SupportedDotNetFrameworkVersions.Contains(dotNetFrameworkVersion))
//                {
//                    return dotNetFrameworkSpec.GetPathToDotNetFrameworkSdkTools(visualStudioSpec);
//                }
//
//                return null;
//            }
//
//            protected static string FallbackToDefaultPathToDotNetFrameworkSdkTools(Version dotNetFrameworkVersion)
//            {
//                if (dotNetFrameworkVersion.Major == 4)
//                {
//                    return FrameworkLocationHelper.PathToV4ToolsInFallbackDotNetFrameworkSdk;
//                }
//
//                if (dotNetFrameworkVersion == dotNetFrameworkVersion35)
//                {
//                    return FrameworkLocationHelper.PathToV35ToolsInFallbackDotNetFrameworkSdk;
//                }
//
//                return null;
//            }
//        }

    type DotNetFrameworkSpecLegacy (version,
                                    dotNetFrameworkRegistryKey,
                                    dotNetFrameworkSetupRegistryInstalledName,
                                    dotNetFrameworkVersionFolderPrefix,
                                    dotNetFrameworkSdkRegistryToolsKey,
                                    dotNetFrameworkSdkRegistryInstallationFolderName,
                                    hasMSBuild, 
                                    vsBuild) =
        inherit DotNetFrameworkSpec (version,
                                     dotNetFrameworkRegistryKey,
                                     dotNetFrameworkSetupRegistryInstalledName,
                                     dotNetFrameworkVersionFolderPrefix,
                                     dotNetFrameworkSdkRegistryToolsKey,
                                     dotNetFrameworkSdkRegistryInstallationFolderName,
                                     hasMSBuild, 
                                     vsBuild)

    type DotNetFrameworkSpecV3 (version,
                                dotNetFrameworkRegistryKey,
                                dotNetFrameworkSetupRegistryInstalledName,
                                dotNetFrameworkVersionFolderPrefix,
                                dotNetFrameworkSdkRegistryToolsKey,
                                dotNetFrameworkSdkRegistryInstallationFolderName,
                                hasMSBuild, 
                                vsBuild) =
        inherit DotNetFrameworkSpec(version,
                                    dotNetFrameworkRegistryKey,
                                    dotNetFrameworkSetupRegistryInstalledName,
                                    dotNetFrameworkVersionFolderPrefix,
                                    dotNetFrameworkSdkRegistryToolsKey,
                                    dotNetFrameworkSdkRegistryInstallationFolderName,
                                    hasMSBuild, 
                                    vsBuild)


//        {
//            private string _pathToDotNetFrameworkSdkTools;
//
//            public DotNetFrameworkSpecLegacy(
//                Version version,
//                string dotNetFrameworkRegistryKey,
//                string dotNetFrameworkSetupRegistryInstalledName,
//                string dotNetFrameworkVersionFolderPrefix,
//                string dotNetFrameworkSdkRegistryInstallationFolderName,
//                bool hasMSBuild)
//                : base(version,
//                      dotNetFrameworkRegistryKey,
//                      dotNetFrameworkSetupRegistryInstalledName,
//                      dotNetFrameworkVersionFolderPrefix,
//                      null,
//                      dotNetFrameworkSdkRegistryInstallationFolderName,
//                      hasMSBuild)
//            {
//            }
//
//            /// <summary>
//            /// Gets the full registry key of this .net framework Sdk for the given visual studio version.
//            /// i.e. "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework" for v1.1 and v2.0.
//            /// </summary>
//            public override string GetDotNetFrameworkSdkRootRegistryKey(VisualStudioSpec visualStudioSpec)
//            {
//                return FrameworkLocationHelper.fullDotNetFrameworkRegistryKey;
//            }
//
//            /// <summary>
//            /// Gets the full path of .net framework sdk tools for the given visual studio version.
//            /// </summary>
//            public override string GetPathToDotNetFrameworkSdkTools(VisualStudioSpec visualStudioSpec)
//            {
//                if (_pathToDotNetFrameworkSdkTools == null)
//                {
//                    _pathToDotNetFrameworkSdkTools = FindRegistryValueUnderKey(
//                        dotNetFrameworkRegistryPath,
//                        this.dotNetFrameworkSdkRegistryInstallationFolderName);
//                }
//
//                return _pathToDotNetFrameworkSdkTools;
//            }
//
//            /// <summary>
//            /// Gets the full path of .net framework sdk, which is the full path of .net framework sdk tools for v1.1 and v2.0.
//            /// </summary>
//            public override string GetPathToDotNetFrameworkSdk(VisualStudioSpec visualStudioSpec)
//            {
//                return this.GetPathToDotNetFrameworkSdkTools(visualStudioSpec);
//            }
//
//            /// <summary>
//            /// Gets the full path of reference assemblies folder, which is the full path of .net framework for v1.1 and v2.0.
//            /// </summary>
//            public override string GetPathToDotNetFrameworkReferenceAssemblies()
//            {
//                return this.GetPathToDotNetFramework(DotNetFrameworkArchitecture.Current);
//            }
//        }
//
//        /// <summary>
//        /// Specialized implementation for legacy .net framework v3.0 and v3.5.
//        /// </summary>
//        private class DotNetFrameworkSpecV3 : DotNetFrameworkSpec
//        {
//            public DotNetFrameworkSpecV3(
//                Version version,
//                string dotNetFrameworkRegistryKey,
//                string dotNetFrameworkSetupRegistryInstalledName,
//                string dotNetFrameworkVersionFolderPrefix,
//                string dotNetFrameworkSdkRegistryToolsKey,
//                string dotNetFrameworkSdkRegistryInstallationFolderName,
//                bool hasMSBuild)
//                : base(version,
//                      dotNetFrameworkRegistryKey,
//                      dotNetFrameworkSetupRegistryInstalledName,
//                      dotNetFrameworkVersionFolderPrefix,
//                      dotNetFrameworkSdkRegistryToolsKey,
//                      dotNetFrameworkSdkRegistryInstallationFolderName,
//                      hasMSBuild)
//            {
//            }
//
//            /// <summary>
//            /// Gets the full path of .net framework sdk.
//            /// i.e. "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\" for .net v3.5 on VS11.
//            /// </summary>
//            public override string GetPathToDotNetFrameworkSdk(VisualStudioSpec visualStudioSpec)
//            {
//                string pathToBinRoot = this.GetPathToDotNetFrameworkSdkTools(visualStudioSpec);
//                pathToBinRoot = RemoveDirectories(pathToBinRoot, 1);
//                return pathToBinRoot;
//            }
//
//            /// <summary>
//            /// Gets the full path of reference assemblies folder.
//            /// i.e. "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\" for v3.5.
//            /// </summary>
//            public override string GetPathToDotNetFrameworkReferenceAssemblies()
//            {
//                if (this.pathToDotNetFrameworkReferenceAssemblies == null)
//                {
//                    this.pathToDotNetFrameworkReferenceAssemblies = FindRegistryValueUnderKey(
//                        dotNetFrameworkAssemblyFoldersRegistryPath + "\\" + this.dotNetFrameworkFolderPrefix,
//                        referenceAssembliesRegistryValueName);
//
//                    if (this.pathToDotNetFrameworkReferenceAssemblies == null)
//                    {
//                        this.pathToDotNetFrameworkReferenceAssemblies = GenerateReferenceAssemblyDirectory(dotNetFrameworkFolderPrefix);
//                    }
//                }
//
//                return this.pathToDotNetFrameworkReferenceAssemblies;
//            }
//        }
//

    let CreateDotNetFrameworkSpecForV4 version visualStudioVersion =
        new DotNetFrameworkSpec(
            version,
            dotNetFrameworkSetupRegistryPath + "\\v4\\Full",
            "Install",
            "v4.0",
            "WinSDK-NetFx40Tools-x86",
            "InstallationFolder",
            true,
            Some visualStudioVersion)

    let dotNetFrameworkSpecDict = 
        let array = [|
            new DotNetFrameworkSpecLegacy(dotNetFrameworkVersion11,                             // v1.1
                dotNetFrameworkRegistryKeyV11,
                dotNetFrameworkSetupRegistryInstalledName,
                dotNetFrameworkVersionFolderPrefixV11,
                dotNetFrameworkSdkInstallKeyValueV11,
                "",
                false,
                None) :> DotNetFrameworkSpec
            new DotNetFrameworkSpecLegacy(                                                      // v2.0
                dotNetFrameworkVersion20,
                dotNetFrameworkRegistryKeyV20,
                dotNetFrameworkSetupRegistryInstalledName,
                dotNetFrameworkVersionFolderPrefixV20,
                dotNetFrameworkSdkInstallKeyValueV20,
                "",
                true,
                None) :> DotNetFrameworkSpec
            new DotNetFrameworkSpecV3(                                                          // v3.0
                dotNetFrameworkVersion30,
                dotNetFrameworkRegistryKeyV30,
                "InstallSuccess",
                dotNetFrameworkVersionFolderPrefixV30,
                "",
                "",
                false,
                None) :> DotNetFrameworkSpec
            new DotNetFrameworkSpecV3(                                                          // v3.5
                dotNetFrameworkVersion35,
                dotNetFrameworkRegistryKeyV35,
                dotNetFrameworkSetupRegistryInstalledName,
                dotNetFrameworkVersionFolderPrefixV35,
                "WinSDK-NetFx35Tools-x86",
                "InstallationFolder",
                true,
                None) :> DotNetFrameworkSpec
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion40  visualStudioVersion100     // v4.0
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion45  visualStudioVersion110     // v4.5
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion451 visualStudioVersion120     // v4.5.1
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion452 visualStudioVersion150     // v4.5.2
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion46  visualStudioVersion140     // v4.6
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion461 visualStudioVersion150     // v4.6.1
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion462 visualStudioVersion150     // v4.6.2
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion47  visualStudioVersion150     // v4.7
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion471 visualStudioVersion150     // v4.7.1
            CreateDotNetFrameworkSpecForV4 dotNetFrameworkVersion472 visualStudioVersion150     // v4.7.2
        |]
        array.ToDictionary<DotNetFrameworkSpec, Version>(fun spec -> spec.Version)

    let getDotNetFrameworkSpec version =
        match dotNetFrameworkSpecDict.TryGetValue version with
        | true, x -> x
        | _ -> raise (getArgumentException version)

    // Get a fully qualified path to the framework's root directory. 
//    let GetPathToDotNetFramework version architecture =
//        let frameworkVersion = TargetDotNetFrameworkVersionToSystemVersion version
//        (getDotNetFrameworkSpec frameworkVersion).GetPathToDotNetFramework(architecture)


    // Get a fully qualified path to the frameworks root directory.
    let GetPathToDotNetFramework version =
//        GetPathToDotNetFramework version DotNetFrameworkArchitecture.Current
        let frameworkVersion = TargetDotNetFrameworkVersionToSystemVersion version
        (getDotNetFrameworkSpec frameworkVersion).GetPathToDotNetFramework(DotNetFrameworkArchitecture.Current)

    /// <summary>
    /// Returns the path to the reference assemblies location for the given framework version.
    /// </summary>
    /// <param name="version">Version of the targeted .NET Framework</param>
    /// <returns>Path string.</returns>
    let GetPathToDotNetFrameworkReferenceAssemblies version =
        let frameworkVersion = TargetDotNetFrameworkVersionToSystemVersion version
        (getDotNetFrameworkSpec frameworkVersion).GetPathToDotNetFrameworkReferenceAssemblies

    type IBuildEngine =
        abstract member BuildProjectFile : string*string[]*IDictionary*IDictionary -> bool
        abstract member LogCustomEvent : (*CustomBuildEventArgs*) obj -> unit
        abstract member LogErrorEvent : (*BuildErrorEventArgs*) obj -> unit
        abstract member LogMessageEvent : (*BuildMessageEventArgs*) obj -> unit
        abstract member LogWarningEvent : (*BuildMessageEventArgs*) obj -> unit

        // Properties
        abstract member ColumnNumberOfTaskNode : int with get
        abstract member ContinueOnError : bool with get
        abstract member LineNumberOfTaskNode : int with get
        abstract member ProjectFileOfTaskNode : string with get

//    let getPropertyValue instance propName =
//        instance.GetType().GetProperty(propName, BindingFlags.Public).GetValue(instance, null)
//
//    let setPropertyValue instance propName propValue=
//        instance.GetType().GetPropserty(propName, BindingFlags.Public).SetValue(instance, propValue, null)

    type ResolveAssemblyReference () =
        let assembly = Assembly.Load(new AssemblyName("Microsoft.Build.Tasks.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"))
        let resolveAssemblyReferenceType = assembly.GetType("Microsoft.Build.Tasks.ResolveAssemblyReference")
        let instance = Activator.CreateInstance(resolveAssemblyReferenceType)

        member this.BuildEngine
            with get ():IBuildEngine = (instance.GetPropertyValue("BuildEngine") :?> IBuildEngine)
            and set (value:IBuildEngine) = (instance.SetPropertyValue("BuildEngine", value)); ()

        member this.TargetFrameworkDirectories
            with get ():string[] = (instance.GetPropertyValue("TargetFrameworkDirectories") :?> string[])
            and set (value:string[]) = (instance.SetPropertyValue("TargetFrameworkDirectories", value)); ()

        member this.FindRelatedFiles
            with get () :bool = (instance.GetPropertyValue("FindRelatedFiles") :?> bool)
            and set (value:bool) = (instance.SetPropertyValue("FindRelatedFiles", value)); ()

        member this.FindDependencies
            with get ():bool = (instance.GetPropertyValue("FindDependencies") :?> bool)
            and set (value:bool) = (instance.SetPropertyValue("FindDependencies", value)); ()

        member this.FindSatellites
            with get ():bool = (instance.GetPropertyValue("FindSatellites") :?> bool)
            and set (value:bool) = (instance.SetPropertyValue("FindSatellites", value)); ()

        member this.FindSerializationAssemblies
            with get ():bool = (instance.GetPropertyValue("FindSerializationAssemblies") :?> bool)
            and set (value:bool) = (instance.SetPropertyValue("FindSerializationAssemblies", value)); ()

        member this.TargetedRuntimeVersion
            with get ():string = (instance.GetPropertyValue("TargetedRuntimeVersion") :?> string)
            and set (value:string) = (instance.SetPropertyValue("TargetedRuntimeVersion", value)); ()

        member this.TargetProcessorArchitecture
            with get ():string = (instance.GetPropertyValue("TargetProcessorArchitecture") :?> string)
            and set (value:string) = (instance.SetPropertyValue("TargetProcessorArchitecture", value)); ()

        member this.CopyLocalDependenciesWhenParentReferenceInGac
            with get ():bool = (instance.GetPropertyValue("CopyLocalDependenciesWhenParentReferenceInGac") :?> bool)
            and set (value:bool) = (instance.SetPropertyValue("CopyLocalDependenciesWhenParentReferenceInGac", value)); ()

        member this.AllowedAssemblyExtensions
            with get () :string[] = (instance.GetPropertyValue("AllowedAssemblyExtensions") :?> string[])
            and set (value:string[]) =  (instance.SetPropertyValue("AllowedAssemblyExtensions", value)); ()

        member this.Assemblies
            with get ():ITaskItem[] = (instance.GetPropertyValue("Assemblies") :?> ITaskItem[])
            and set (value:ITaskItem[]) = (instance.SetPropertyValue("Assemblies", value)); ()

        member this.CopyLocalFiles = (instance.GetPropertyValue("CopyLocalFiles") :?> ITaskItem[])

        member this.RelatedFiles = (instance.GetPropertyValue("RelatedFiles") :?> ITaskItem[])

        member this.ResolvedFiles = (instance.GetPropertyValue("ResolvedFiles") :?> ITaskItem[])

        member this.ResolvedDependencyFiles = (instance.GetPropertyValue("ResolvedDependencyFiles") :?> ITaskItem[])

        member this.SatelliteFiles = (instance.GetPropertyValue("SatelliteFiles") :?> ITaskItem[])

        member this.ScatterFiles = (instance.GetPropertyValue("ScatterFiles") :?> ITaskItem[])

        member this.SuggestedRedirects = (instance.GetPropertyValue("SuggestedRedirects") :?> ITaskItem[])

        member this.SearchPaths
            with get () :string[] = (instance.GetPropertyValue("SearchPaths") :?> string[])
            and set (value:string[]) =  (instance.SetPropertyValue("SearchPaths", value)); ()

        member this.Execute () =
            let m = instance.GetType().GetMethod("Execute", [| |])
            m.Invoke(instance, [||]) :?> bool

#endif
