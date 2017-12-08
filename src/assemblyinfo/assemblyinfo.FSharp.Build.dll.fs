// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp
open System.Reflection
[<assembly:AssemblyDescription("FSharp.Build.dll")>]
[<assembly:AssemblyCompany("Microsoft Corporation")>]
[<assembly:AssemblyTitle("FSharp.Build.dll")>]
[<assembly:AssemblyCopyright("\169 Microsoft Corporation.  All Rights Reserved.")>]
[<assembly:AssemblyProduct("Microsoft\174 F#")>]
do()

// Until dotnet sdk can version assemblies, use this
#if BUILD_FROM_SOURCE
[<assembly: System.Reflection.AssemblyInformationalVersion("4.4.1.0")>]
[<assembly: System.Reflection.AssemblyVersion("4.4.1.0")>]
[<assembly: System.Reflection.AssemblyFileVersion("2017.06.27.0")>]
#endif

do()
