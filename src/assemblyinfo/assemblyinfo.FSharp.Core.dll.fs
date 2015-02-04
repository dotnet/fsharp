// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp
open System.Reflection
open System.Runtime.InteropServices

[<assembly:AssemblyDescription("FSharp.Core.dll")>]
[<assembly:AssemblyCompany("Microsoft Corporation")>]
[<assembly:AssemblyTitle("FSharp.Core.dll")>]
[<assembly:AssemblyCopyright("\169 Microsoft Corporation.  Apache 2.0 License.")>]
[<assembly:AssemblyProduct("Microsoft\174 F#")>]

#if PORTABLE
[<assembly:ComVisible(false)>]
[<assembly:AssemblyProduct("Microsoft\174 F#")>]
[<assembly:AssemblyFlags(System.Reflection.AssemblyNameFlags.Retargetable)>] // ensure we replace any 4.0.30319.* or 4.0.31105.* versions in the GAC. These are the FileVersions for RTM VS2010 and SP1 VS2010
#endif

do()

