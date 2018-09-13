#if INTERACTIVE
#load "../../src/ProvidedTypes.fsi" "../../src/ProvidedTypes.fs"
#endif

namespace BasicProvider.Helpers

type SomeRuntimeHelper() = 
    static member Help() = "help"

#if !IS_DESIGNTIME
// Put the TypeProviderAssemblyAttribute in the runtime DLL, pointing to the design-time DLL
[<assembly:CompilerServices.TypeProviderAssembly("BasicProvider.DesignTime.dll")>]
do ()
#endif

