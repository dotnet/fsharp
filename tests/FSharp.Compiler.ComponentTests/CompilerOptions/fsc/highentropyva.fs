// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

open System
open System.Reflection.PortableExecutable

module highentropyva =

    let shouldHaveFlag (expected: DllCharacteristics) (result: DllCharacteristics) =
        if not (result.HasFlag expected) then
            raise (new Exception $"CoffHeader.Characteristics does not contain expected flag:\nFound: {result}\n Expected: {expected}")

    let shouldNotHaveFlag (notexpected: DllCharacteristics) (result: DllCharacteristics) =
        if result.HasFlag notexpected then
            raise (new Exception $"DllCharacteristics contains the unexpected flag:\nFound: {result}\nNot expected: {notexpected}")

    let shouldNotGenerateHighEntropyVirtualAddressSpace platform options =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withPlatform platform
        |> withOptions options
        |> compile
        |> shouldSucceed
        |> withPeReader(fun rdr -> rdr.PEHeaders.PEHeader.DllCharacteristics)
        |> shouldNotHaveFlag DllCharacteristics.HighEntropyVirtualAddressSpace

    let shouldGenerateHighEntropyVirtualAddressSpace platform options =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withPlatform platform
        |> withOptions options
        |> compile
        |> shouldSucceed
        |> withPeReader(fun rdr -> rdr.PEHeaders.PEHeader.DllCharacteristics)
        |> shouldHaveFlag DllCharacteristics.HighEntropyVirtualAddressSpace


    //# Default behavior ==========================================================================================================================

    // SOURCE=dummy.fs SCFLAGS="--platform:x64"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe no"	# CheckHighEntropyALSR - x64
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x64 no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X64 []

    // SOURCE=dummy.fs SCFLAGS="--platform:x86"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe no"	# CheckHighEntropyALSR - x86
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x86 no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X86 []

    // SOURCE=dummy.fs SCFLAGS="--platform:arm86"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe no"	# CheckHighEntropyALSR - x86
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm86 no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm []

    // SOURCE=dummy.fs SCFLAGS="--platform:arm64"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe no"	# CheckHighEntropyALSR - x86
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm64 no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm64 []


    //# --highentropyva  /  --highentropyva+  /  --highentropyva  ================================================================================

    //# SOURCE=dummy.fs SCFLAGS="--platform:x64 --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x64 --highentropyva no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X64 ["--highentropyva"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:x64 --highentropyva+"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x64 --highentropyva+ no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X64 ["--highentropyva+"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:x64 --highentropyva-"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x64 --highentropyva- no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X64 ["--highentropyva-"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:x64 --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x86 --highentropyva no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X86 ["--highentropyva"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:x86 --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x86 --highentropyva+ no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X86 ["--highentropyva+"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:x86 --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:x86 --highentropyva- no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.X86 ["--highentropyva-"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:Arm64 --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm64 --highentropyva no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm64 ["--highentropyva"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:arm64 --highentropyva+"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm64 --highentropyva+ no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm64 ["--highentropyva+"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:arm64 --highentropyva-"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm64 --highentropyva- no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm64 ["--highentropyva-"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:arm --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm --highentropyva no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm ["--highentropyva"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:arm --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm --highentropyva+ no``() =
        shouldGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm ["--highentropyva+"]

    //# SOURCE=dummy.fs SCFLAGS="--platform:arm --highentropyva"    COMPILE_ONLY=1  POSTCMD="CheckHighEntropyASLR.bat dummy.exe yes"		# CheckHighEntropyALSR - x86 highentropyva
    [<Fact>]
    let ``CheckHighEntropyASLR_fs --platform:arm --highentropyva- no``() =
        shouldNotGenerateHighEntropyVirtualAddressSpace ExecutionPlatform.Arm ["--highentropyva-"]
