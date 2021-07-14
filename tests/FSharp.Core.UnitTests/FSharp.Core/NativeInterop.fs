// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the NativeInterop namespace

namespace FSharp.Core.UnitTests.NativeInterop

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open Microsoft.FSharp.NativeInterop

#nowarn "9" // FS0009	Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.

type NativePtr() =
    [<Theory>]
    [<InlineData(0y, 123y, SByte.MinValue, 0x11y)>]
    [<InlineData(0uy, 123uy, Byte.MaxValue, 0x11uy)>]
    [<InlineData(0s, 123s, Int16.MinValue, 0x1111s)>]
    [<InlineData(0us, 123us, UInt16.MaxValue, 0x1111us)>]
    [<InlineData(0, 123, Int32.MinValue, 0x1111_1111)>]
    [<InlineData(0u, 123u, UInt32.MaxValue, 0x1111_1111u)>]
    [<InlineData(0L, 123L, Int64.MinValue, 0x1111_1111_1111_1111L)>]
    [<InlineData(0UL, 123UL, UInt64.MaxValue, 0x1111_1111_1111_1111UL)>]
    [<InlineData(0f, 123f, Single.MaxValue, 0x1111_1111lf)>]
    [<InlineData(0., 123., Double.MaxValue, 0x1111_1111_1111_1111LF)>]
    [<InlineData('\000', '\123', Char.MaxValue, '\u1111')>]
    member _.Test<'T when 'T : unmanaged>(n0 : 'T, nusual : 'T, nextreme : 'T, nhex11 : 'T) =
        let size = sizeof<'T> |> nativeint
        let a0 = NativePtr.stackalloc<'T> 4
        let a1 = NativePtr.add a0 1
        let a2 = NativePtr.add a0 2
        let a3 = NativePtr.add a0 3
        let i0 = NativePtr.toNativeInt a0
        let assertStack v0 v1 v2 v3 =
            Assert.Equal(v0, NativePtr.get a0 0)
            Assert.Equal(v1, NativePtr.get a0 1)
            Assert.Equal(v2, NativePtr.get a0 2)
            Assert.Equal(v3, NativePtr.get a0 3)
            Assert.Equal(v0, NativePtr.read a0)
            Assert.Equal(v1, NativePtr.read a1)
            Assert.Equal(v2, NativePtr.read a2)
            Assert.Equal(v3, NativePtr.read a3)
        Assert.Equal(i0 + 1n * size, a1 |> NativePtr.toNativeInt)
        Assert.Equal(i0 + 2n * size, a2 |> NativePtr.toNativeInt)
        Assert.Equal(i0 + 3n * size, a3 |> NativePtr.toNativeInt)
        Assert.Equal(i0 - 5n * size, NativePtr.add a0 -5 |> NativePtr.toNativeInt)
        Assert.Equal(a2,
            a2
            |> NativePtr.toVoidPtr
            |> NativePtr.ofVoidPtr<'T>
            |> NativePtr.toILSigPtr
            |> NativePtr.ofILSigPtr
        )
        NativePtr.set a0 3 nusual
        Assert.Equal(nusual, NativePtr.get a0 3)
        Assert.Equal(nusual, NativePtr.get a3 0)
        NativePtr.copy a2 a3
        Assert.Equal(nusual, NativePtr.get a0 3)
        Assert.Equal(nusual, NativePtr.get a0 2)
        NativePtr.copyBlock a0 a2 2
        assertStack nusual nusual nusual nusual
        NativePtr.clear a0
        assertStack n0 nusual nusual nusual
        NativePtr.initBlock a1 0x11uy (2u * uint size)
        assertStack n0 nhex11 nhex11 nusual
        NativePtr.write a2 nextreme
        assertStack n0 nhex11 nextreme nusual
        let nullPtr = NativePtr.ofNativeInt<'T> 0n
        Assert.Equal(NativePtr.nullPtr, nullPtr)
        Assert.True(NativePtr.isNullPtr nullPtr)
        let notNullPtr = NativePtr.ofNativeInt<'T> 1n
        Assert.NotEqual(NativePtr.nullPtr, notNullPtr)
        Assert.False(NativePtr.isNullPtr notNullPtr)
