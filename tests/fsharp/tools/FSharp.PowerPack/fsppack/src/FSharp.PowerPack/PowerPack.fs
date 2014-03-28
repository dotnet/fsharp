module internal Microsoft.FSharp.AssemblyAttributes

//[<assembly: System.Security.SecurityTransparent>]
[<assembly: AutoOpen("Microsoft.FSharp.Compatibility")>]
[<assembly: AutoOpen("Microsoft.FSharp.Compatibility.OCaml.Pervasives")>]
[<assembly: AutoOpen("Microsoft.FSharp.Compatibility.OCaml")>]
[<assembly: AutoOpen("Microsoft.FSharp.Text")>]
[<assembly: AutoOpen("Microsoft.FSharp.Control")>]
[<assembly: AutoOpen("Microsoft.FSharp.Collections")>]
[<assembly: AutoOpen("Microsoft.FSharp.Core")>]
do()

#if FX_NO_SECURITY_PERMISSIONS
#else
#if FX_SIMPLE_SECURITY_PERMISSIONS
[<assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum)>]
#else
#endif
#endif

[<assembly: System.Runtime.InteropServices.ComVisible(false)>]

[<assembly: System.CLSCompliant(true)>]


#if FX_NO_DEFAULT_DEPENDENCY_TYPE
#else
[<assembly: System.Runtime.CompilerServices.Dependency("FSharp.Core",System.Runtime.CompilerServices.LoadHint.Always)>] 
#endif

do ()
