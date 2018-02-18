// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp
open System.Reflection
open System.Runtime.CompilerServices
open Microsoft.VisualStudio.Shell

[<assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\FSharp.ProjectSystem.FSharp.dll")>]

[<assembly:ProvideCodeBase(AssemblyName = "FSharp.Compiler.Private", CodeBase = @"$PackageFolder$\FSharp.Compiler.Private.dll")>]
[<assembly:ProvideCodeBase(AssemblyName = "FSharp.Compiler.Server.Shared", CodeBase = @"$PackageFolder$\FSharp.Compiler.Server.Shared.dll")>]
[<assembly:ProvideCodeBase(AssemblyName = "FSharp.UIResources", CodeBase = @"$PackageFolder$\FSharp.UIResources.dll")>]
[<assembly:ProvideBindingRedirection(AssemblyName = "FSharp.Core", OldVersionLowerBound = "2.0.0.0", OldVersionUpperBound = "4.4.0.0", NewVersion = "4.4.1.0", CodeBase = @"$PackageFolder$\FSharp.Core.dll")>]

do()

