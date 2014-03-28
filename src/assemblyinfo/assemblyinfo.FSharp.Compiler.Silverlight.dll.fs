// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#light
namespace Microsoft.FSharp
open System.Reflection

[<assembly:AssemblyDescription("FSharp.Compiler.Silverlight.dll")>]
[<assembly:AssemblyCompany("Microsoft Corporation")>]
[<assembly:AssemblyTitle("FSharp.Compiler.Silverlight.dll")>]
[<assembly:AssemblyCopyright("\169 Microsoft Corporation.  Apache 2.0 License.")>]
[<assembly:AssemblyProduct("Microsoft\174 F#")>]

// Note: internals visible to unit test DLLs in Retail (and all) builds.
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Salsa")>]
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Unittests")>]
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("SystematicUnitTests")>]
[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Test")>]

do()
