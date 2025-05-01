// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open System
open System.Reflection.PortableExecutable

open Xunit
open FSharp.Test.Compiler


module highentropyva =

    let shouldHaveFlag (expected: DllCharacteristics) (result: DllCharacteristics) =
        if not (result.HasFlag expected) then
            raise (new Exception $"CoffHeader.Characteristics does not contain expected flag:\nFound: {result}\n Expected: {expected}")

    let shouldNotHaveFlag (notexpected: DllCharacteristics) (result: DllCharacteristics) =
        if result.HasFlag notexpected then
            raise (new Exception $"DllCharacteristics contains the unexpected flag:\nFound: {result}\nNot expected: {notexpected}")

    [<InlineData(ExecutionPlatform.X64, null)>]
    [<InlineData(ExecutionPlatform.X86, null)>]
    [<InlineData(ExecutionPlatform.Arm64, null)>]
    [<InlineData(ExecutionPlatform.Arm, null)>]
    [<InlineData(ExecutionPlatform.X64, "--highentropyva-")>]
    [<InlineData(ExecutionPlatform.X86, "--highentropyva-")>]
    [<InlineData(ExecutionPlatform.Arm64, "--highentropyva-")>]
    [<InlineData(ExecutionPlatform.Arm, "--highentropyva-")>]
    [<Theory>]
    let shouldNotGenerateHighEntropyVirtualAddressSpace platform options =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withPlatform platform
        |> withOptions (if String.IsNullOrWhiteSpace options then [] else [options])
        |> compile
        |> shouldSucceed
        |> withPeReader(fun rdr -> rdr.PEHeaders.PEHeader.DllCharacteristics)
        |> shouldNotHaveFlag DllCharacteristics.HighEntropyVirtualAddressSpace

    [<InlineData(ExecutionPlatform.X64, "--highentropyva")>]
    [<InlineData(ExecutionPlatform.X64, "--highentropyva+")>]
    [<InlineData(ExecutionPlatform.X86, "--highentropyva")>]
    [<InlineData(ExecutionPlatform.X86, "--highentropyva+")>]
    [<InlineData(ExecutionPlatform.Arm64, "--highentropyva+")>]
    [<InlineData(ExecutionPlatform.Arm64, "--highentropyva")>]
    [<InlineData(ExecutionPlatform.Arm, "--highentropyva")>]
    [<InlineData(ExecutionPlatform.Arm, "--highentropyva+")>]
    [<Theory>]
    let shouldGenerateHighEntropyVirtualAddressSpace platform options =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withPlatform platform
        |> withOptions  (if String.IsNullOrWhiteSpace options then [] else [options])
        |> compile
        |> shouldSucceed
        |> withPeReader(fun rdr -> rdr.PEHeaders.PEHeader.DllCharacteristics)
        |> shouldHaveFlag DllCharacteristics.HighEntropyVirtualAddressSpace
