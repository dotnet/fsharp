#light

namespace FSharp
  module internal BuildProperties = begin
    val fsProductVersion : string
    val fsLanguageVersion : string
  end

namespace InteractiveDependencyManager
  type internal SR =
    class
      private new : unit -> SR
      static member RunStartupValidation : unit -> unit
      static member
        couldNotLoadDependencyManagerExtension : a0:System.String *
                                                 a1:System.String ->
                                                   int * string
      static member SwallowResourceText : bool
      static member packageManagerError : a0:System.String -> int * string
      static member
        packageManagerUnknown : a0:System.String * a1:System.String *
                                a2:System.String -> int * string
      static member SwallowResourceText : bool with set
    end







namespace Internal.Utilities
  module internal FSharpEnvironment = begin
    val FSharpBannerVersion : string
    val versionOf<'t> : string
    val FSharpCoreLibRunningVersion : string option
    val FSharpBinaryMetadataFormatRevision : string
    val RegOpenKeyExW :
      _hKey:System.UIntPtr * _lpSubKey:string * _ulOptions:uint32 *
      _samDesired:int * _phkResult:byref<System.UIntPtr> -> uint32
    val RegQueryValueExW :
      _hKey:System.UIntPtr * _lpValueName:string * _lpReserved:uint32 *
      _lpType:byref<uint32> * _lpData:System.IntPtr * _lpchData:byref<int> ->
        uint32
    val RegCloseKey : _hKey:System.UIntPtr -> uint32
    module Option = begin
      val ofString : s:string -> string option
    end
    val maxPath : int
    val maxDataLength : int
    val KEY_WOW64_DEFAULT : int
    val KEY_WOW64_32KEY : int
    val HKEY_LOCAL_MACHINE : System.UIntPtr
    val KEY_QUERY_VALUE : int
    val REG_SZ : uint32
    val GetDefaultRegistryStringValueViaDotNet : subKey:string -> string option
    val Get32BitRegistryStringValueViaPInvoke : subKey:string -> string option
    val is32Bit : bool
    val runningOnMono : bool
    val tryRegKey : subKey:string -> string option
    val tryCurrentDomain : unit -> string option
    val tryAppConfig : _appConfigKey:string -> string option
    val BinFolderOfDefaultFSharpCompiler :
      probePoint:string option -> string option
    val useKey : subKey:string -> f:(Microsoft.Win32.RegistryKey -> 'a) -> 'a
    val IsNetFx45OrAboveInstalledAt : subKey:string -> bool
    val IsNetFx45OrAboveInstalled : bool
    val IsRunningOnNetFx45OrAbove : bool
    val toolingCompatibleTypeProviderProtocolMonikers : unit -> string list
    val toolingCompatibleVersions : string []
    val toolPaths : string []
    val toolingCompatiblePaths : unit -> string list
    val searchToolPaths :
      path:string option -> compilerToolPaths:seq<string> -> seq<string>
    val getTypeProviderAssembly :
      runTimeAssemblyFileName:string * designTimeAssemblyName:string *
      compilerToolPaths:string list *
      raiseError:(exn -> System.Reflection.Assembly option) ->
        System.Reflection.Assembly option
    val getCompilerToolsDesignTimeAssemblyPaths :
      compilerToolPaths:seq<string> -> seq<string>
  end

namespace Interactive.DependencyManager
  type AssemblyResolutionProbe =
    System.Func<System.Collections.Generic.IEnumerable<string>>
  type AssemblyResolveHandler =
    class
      interface System.IDisposable
      new : assemblyProbingPaths:AssemblyResolutionProbe ->
              AssemblyResolveHandler
    end

namespace Interactive.DependencyManager
  type NativeResolutionProbe =
    System.Func<System.Collections.Generic.IEnumerable<string>>
  type NativeDllResolveHandler =
    class
      interface System.IDisposable
      new : nativeProbingRoots:NativeResolutionProbe -> NativeDllResolveHandler
    end

namespace Interactive.DependencyManager
  module ReflectionHelper = begin
    val dependencyManagerPattern : string
    val dependencyManagerAttributeName : string
    val resolveDependenciesMethodName : string
    val namePropertyName : string
    val keyPropertyName : string
    val seqEmpty : seq<string>
    val assemblyHasAttribute :
      theAssembly:System.Reflection.Assembly -> attributeName:string -> bool
    val getAttributeNamed :
      theType:System.Type -> attributeName:string -> obj option
    val getInstanceProperty<'treturn> :
      theType:System.Type ->
        propertyName:string -> System.Reflection.PropertyInfo option
    val getInstanceMethod<'treturn> :
      theType:System.Type ->
        parameterTypes:System.Type array ->
          methodName:string -> System.Reflection.MethodInfo option
    val stripTieWrapper : e:System.Exception -> exn
  end
  [<RequireQualifiedAccessAttribute ()>]
  type ErrorReportType =
    | Warning
    | Error
  type ResolvingErrorReport =
    delegate of (ErrorReportType * int * string) -> unit
  [<AllowNullLiteralAttribute ()>]
  type IDependencyManagerProvider =
    interface
      abstract member
        ResolveDependencies : scriptDir:string * mainScriptName:string *
                              scriptName:string * scriptExt:string *
                              packageManagerTextLines:seq<string> * tfm:string ->
                                bool * seq<string> * seq<string> * seq<string>
      abstract member Key : string
      abstract member Name : string
    end
  type ReflectionDependencyManagerProvider =
    class
      interface IDependencyManagerProvider
      new : theType:System.Type * nameProperty:System.Reflection.PropertyInfo *
            keyProperty:System.Reflection.PropertyInfo *
            resolveDeps:System.Reflection.MethodInfo option *
            resolveDepsEx:System.Reflection.MethodInfo option *
            outputDir:string option -> ReflectionDependencyManagerProvider
      static member
        InstanceMaker : theType:System.Type * outputDir:string option ->
                          (unit -> IDependencyManagerProvider) option
    end
  type DependencyProvider =
    class
      interface System.IDisposable
      new : nativeProbingRoots:NativeResolutionProbe -> DependencyProvider
      new : assemblyProbingPaths:AssemblyResolutionProbe *
            nativeProbingRoots:NativeResolutionProbe -> DependencyProvider
      member
        CreatePackageManagerUnknownError : compilerTools:seq<string> *
                                           outputDir:string *
                                           packageManagerKey:string *
                                           reportError:ResolvingErrorReport ->
                                             int * string
      member
        RemoveDependencyManagerKey : packageManagerKey:string * path:string ->
                                       string
      member
        Resolve : packageManager:IDependencyManagerProvider *
                  implicitIncludeDir:string * mainScriptName:string *
                  fileName:string * scriptExt:string *
                  packageManagerTextLines:seq<string> *
                  reportError:(ErrorReportType -> int * string -> unit) *
                  executionTfm:string ->
                    bool * seq<string> * seq<string> * seq<string>
      member
        TryFindDependencyManagerByKey : compilerTools:seq<string> *
                                        outputDir:string *
                                        reportError:ResolvingErrorReport *
                                        key:string -> IDependencyManagerProvider
      member
        TryFindDependencyManagerInPath : compilerTools:seq<string> *
                                         outputDir:string *
                                         reportError:ResolvingErrorReport *
                                         path:string ->
                                           string * IDependencyManagerProvider
    end

