// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.SourceCodeServices

[<RequireQualifiedAccess>]
module CheckerSingleton =

    let checker = FSharpChecker.Create()