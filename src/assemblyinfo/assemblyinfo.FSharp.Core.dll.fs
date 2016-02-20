// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp
open System.Reflection
open System.Runtime.InteropServices

[<assembly:AssemblyDescription("FSharp.Core.dll")>]
[<assembly:AssemblyCompany("Microsoft Corporation")>]
[<assembly:AssemblyTitle("FSharp.Core.dll")>]
[<assembly:AssemblyCopyright("\169 Microsoft Corporation.  Apache 2.0 License.")>]
[<assembly:AssemblyProduct("Microsoft\174 F#")>]
#if !FSHARP_CORE_PORTABLE
[<assembly:ComVisible(false)>]
#endif

#if PORTABLE
[<assembly:AssemblyProduct("Microsoft\174 F#")>]
[<assembly:AssemblyFlags(System.Reflection.AssemblyNameFlags.Retargetable)>]
#endif

do()

