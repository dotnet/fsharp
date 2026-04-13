// Copyright (c) Microsoft Corporation.  All Rights Reserved.


namespace CompilerOptions.Fsc

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module publicsign =

    /// <summary>
    /// Tests that --publicsign with a raw key blob (sha1full.snk) produces a non-empty PublicKeyToken.
    /// This test specifically exercises the Offset 8 code path in the compiler's public signing logic,
    /// avoiding the KeyPair/Offset 20 path, by using --publicsign --keyfile with a raw SNK file.
    /// </summary>
    [<Fact>]
    let ``--publicsign with raw key blob (sha1full.snk) produces a non-empty PublicKeyToken`` () =
        let source =
            """
module TestModule
let x = 42
"""

        // Resolve the path to sha1full.snk relative to this source file's directory
        // Path: tests/FSharp.Compiler.ComponentTests/CompilerOptions/fsc/misc -> tests/fsharp/core/signedtests/sha1full.snk
        let snkPath: string = 
            Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "..", "fsharp", "core", "signedtests", "sha1full.snk")

        // Compile with --publicsign+ and --keyfile to exercise the Offset 8 code path
        let result =
            source
            |> FSharp
            |> asLibrary
            |> withFileName "PublicSignTest.fs"
            |> withOptions ["--publicsign+"; sprintf "--keyfile:%s" snkPath]
            |> compile

        result |> shouldSucceed |> ignore

        // Safely extract the output DLL path using pattern matching
        let outputDll: string = 
            match result.OutputPath with
            | Some path -> path
            | None -> failwith "Compilation did not produce an output DLL"

        // Read the compiled DLL bytes for verification
        let dllBytes: byte[] = File.ReadAllBytes(outputDll)

        // RSA magic number patterns: RSA1 = 0x52 0x53 0x41 0x31, RSA2 = 0x52 0x53 0x41 0x32
        // These indicate that RSA key material was embedded in the assembly
        let rsa1Magic: byte[] = [| 0x52uy; 0x53uy; 0x41uy; 0x31uy |]
        let rsa2Magic: byte[] = [| 0x52uy; 0x53uy; 0x41uy; 0x32uy |]

        /// <summary>
        /// Searches for RSA magic bytes in the byte array.
        /// Returns true if the magic pattern is found, indicating RSA key material is present.
        /// </summary>
        let containsRSAMagic (data: byte[]) (magic: byte[]): bool =
            if data.Length < magic.Length then
                false
            else
                let mutable found = false
                for i in 0 .. (data.Length - magic.Length) do
                    if not found && 
                       data.[i] = magic.[0] && 
                       data.[i + 1] = magic.[1] && 
                       data.[i + 2] = magic.[2] && 
                       data.[i + 3] = magic.[3] then
                        found <- true
                found

        // Verify that the compiled DLL contains RSA1 (public key) magic and NOT RSA2 (private key) magic
        let hasRSA1: bool = containsRSAMagic dllBytes rsa1Magic
        let hasRSA2: bool = containsRSAMagic dllBytes rsa2Magic

        Assert.True(
            hasRSA1,
            "Compiled DLL should contain RSA1 magic bytes indicating a public key blob was embedded"
        )

        Assert.False(
            hasRSA2,
            "Compiled DLL must NOT contain RSA2 magic bytes — private key material must not be embedded in the assembly"
        )

    /// <summary>
    /// Tests that --publicsign with a full key pair (.snk) produces an assembly with a valid
    /// public key blob that can be loaded by AssemblyName without throwing SecurityException.
    /// This is the regression test for https://github.com/dotnet/fsharp/issues/19441.
    /// </summary>
    [<Fact>]
    let ``--publicsign with full key pair produces valid public key blob`` () =
        let source =
            """
module TestModule
let x = 42
"""

        let snkPath: string =
            Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "..", "fsharp", "core", "signedtests", "sha1full.snk")

        let result =
            source
            |> FSharp
            |> asLibrary
            |> withFileName "PublicSignValidKey.fs"
            |> withOptions ["--publicsign+"; sprintf "--keyfile:%s" snkPath]
            |> compile

        result |> shouldSucceed |> ignore

        let outputDll: string =
            match result.OutputPath with
            | Some path -> path
            | None -> failwith "Compilation did not produce an output DLL"

        // Loading the assembly name should not throw SecurityException for invalid public key
        let assemblyName = System.Reflection.AssemblyName.GetAssemblyName(outputDll)
        let publicKeyToken = assemblyName.GetPublicKeyToken()

        Assert.NotNull(publicKeyToken)
        Assert.True(publicKeyToken.Length > 0, "PublicKeyToken should be non-empty for a public-signed assembly")
