// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

[<System.Obsolete("This module is not for external use and may be removed in a future release of FSharp.Compiler.Service")>]
module internal FSharp.Compiler.CodeAnalysis.SimulatedMSBuildReferenceResolver

val getResolver: unit -> LegacyReferenceResolver

