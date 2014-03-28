// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#light
namespace Microsoft.FSharp
open System.Reflection
[<assembly:AssemblyDescription("FSharp.Data.TypeProviders.dll")>]
[<assembly:AssemblyCompany("Microsoft Corporation")>]
[<assembly:AssemblyTitle("FSharp.Data.TypeProviders.dll")>]
[<assembly:AssemblyCopyright("\169 Microsoft Corporation.  All rights reserved.")>]
[<assembly:AssemblyProduct("Microsoft\174 F#")>]
do()

#if NO_STRONG_NAMES
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Unittests")>]
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("SystematicUnitTests")>]
#else
[<assembly:AssemblyFileVersion("4.0.40401.666")>] // ensure we replace any 4.0.30319.* or 4.0.31105.* versions in the GAC. These are the FileVersions for RTM VS2010 and SP1 VS2010
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Unittests, PublicKey=002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293")>]
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("SystematicUnitTests, PublicKey=002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293")>]
#endif
do()
