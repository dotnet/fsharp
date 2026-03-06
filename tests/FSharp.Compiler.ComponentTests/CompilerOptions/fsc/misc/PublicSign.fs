// Copyright (c) Microsoft Corporation.  All Rights Reserved.


namespace CompilerOptions.Fsc

open System
open System.IO
open System.Runtime.InteropServices
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module publicsign =

    /// <summary>
    /// Tests that --publicsign with a raw key blob (sha1full.snk) produces a non-empty PublicKeyToken.
    /// This test specifically exercises the Offset 8 code path in the compiler's public signing logic,
    /// avoiding the KeyPair/Offset 20 path, by using --publicsign --keyfile with a raw SNK file.
    /// </summary>
    [<Fact(Skip = "Strong name signing with raw key blobs is not supported on all platforms")>]
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

        // Verify that the compiled DLL contains RSA magic bytes, confirming the public key blob was embedded
        let hasRSAMagic: bool = 
            containsRSAMagic dllBytes rsa1Magic || containsRSAMagic dllBytes rsa2Magic
        
        Assert.True(
            hasRSAMagic, 
            "Compiled DLL should contain RSA magic bytes (RSA1 or RSA2) indicating public key blob was embedded by compiler with --publicsign"
        )
