// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open Microsoft.Build.Framework
open FSharp.Build
open Xunit

/// Verifies that the FSharp.Build tasks which are pure (their outputs depend only on their inputs,
/// with no reliance on ambient process state such as the current directory or environment variables)
/// are marked directly with MSBuildMultiThreadableTaskAttribute, opting them into MSBuild's
/// multi-threaded build scheduler instead of the single-threaded task queue.
type MultiThreadedTaskTests() =

    static member MultiThreadableTaskTypes: obj[] seq =
        seq {
            yield [| typeof<CreateFSharpManifestResourceName> |]
            yield [| typeof<MapSourceRoots> |]
            yield [| typeof<WriteCodeFragment> |]
            yield [| typeof<GenerateILLinkSubstitutions> |]
            yield [| typeof<FSharpEmbedResourceText> |]
            yield [| typeof<FSharpEmbedResXSource> |]
            yield [| typeof<SubstituteText> |]
        }

    [<Theory>]
    [<MemberData(nameof MultiThreadedTaskTests.MultiThreadableTaskTypes)>]
    member _.``task is marked directly multithreadable`` (taskType: Type) =
        let attributes = taskType.GetCustomAttributes(typeof<MSBuildMultiThreadableTaskAttribute>, false)

        Assert.Equal(1, attributes.Length)
