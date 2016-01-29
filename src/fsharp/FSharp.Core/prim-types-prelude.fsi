// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nowarn "35" // This construct is deprecated: the treatment of this operator is now handled directly by the F# compiler and its meaning may not be redefined.
#nowarn "61" // The containing type can use <c>null</c> as a representation value for its nullary union case. This member will be compiled as a static member.
#nowarn "62" // This construct is for ML compatibility. The syntax <c>module ... : sig .. end</c> is deprecated unless OCaml compatibility is enabled. Consider using <c>module ... = begin .. end'.

/// <summary>Basic F# type definitions, functions and operators.</summary> 
namespace Microsoft.FSharp.Core

    open System

    /// <summary>An abbreviation for the CLI type <c>System.Object</c>.</summary>
    type obj = System.Object

    /// <summary>An abbreviation for the CLI type <c>System.Exception</c>.</summary>
    type exn = System.Exception

    /// <summary>An abbreviation for the CLI type <c>System.IntPtr</c>.</summary>
    type nativeint = System.IntPtr
    /// <summary>An abbreviation for the CLI type <c>System.UIntPtr</c>.</summary>
    type unativeint = System.UIntPtr

    /// <summary>An abbreviation for the CLI type <c>System.String</c>.</summary>
    type string = System.String

    /// <summary>An abbreviation for the CLI type <c>System.Single</c>.</summary>
    type float32 = System.Single
    /// <summary>An abbreviation for the CLI type <c>System.Double</c>.</summary>
    type float = System.Double
    /// <summary>An abbreviation for the CLI type <c>System.Single</c>.</summary>
    type single = System.Single
    /// <summary>An abbreviation for the CLI type <c>System.Double</c>.</summary>
    type double = System.Double

    /// <summary>An abbreviation for the CLI type <c>System.SByte</c>.</summary>
    type sbyte = System.SByte
    /// <summary>An abbreviation for the CLI type <c>System.Byte</c>.</summary>
    type byte = System.Byte
    /// <summary>An abbreviation for the CLI type <c>System.SByte</c>.</summary>
    type int8 = System.SByte
    /// <summary>An abbreviation for the CLI type <c>System.Byte</c>.</summary>
    type uint8 = System.Byte

    /// <summary>An abbreviation for the CLI type <c>System.Int16</c>.</summary>
    type int16 = System.Int16
    /// <summary>An abbreviation for the CLI type <c>System.UInt16</c>.</summary>
    type uint16 = System.UInt16

    /// <summary>An abbreviation for the CLI type <c>System.Int32</c>.</summary>
    type int32 = System.Int32
    /// <summary>An abbreviation for the CLI type <c>System.UInt32</c>.</summary>
    type uint32 = System.UInt32

    /// <summary>An abbreviation for the CLI type <c>System.Int64</c>.</summary>
    type int64 = System.Int64
    /// <summary>An abbreviation for the CLI type <c>System.UInt64</c>.</summary>
    type uint64 = System.UInt64

    /// <summary>An abbreviation for the CLI type <c>System.Char</c>.</summary>
    type char = System.Char
    /// <summary>An abbreviation for the CLI type <c>System.Boolean</c>.</summary>
    type bool = System.Boolean
    /// <summary>An abbreviation for the CLI type <c>System.Decimal</c>.</summary>
    type decimal = System.Decimal

    /// <summary>An abbreviation for the CLI type <c>System.Int32</c>.</summary>
    type int = int32

    /// <summary>Single dimensional, zero-based arrays, written <c>int[]</c>, <c>string[]</c> etc.</summary>
    /// <remarks>Use the values in the <c>Array</c> module to manipulate values 
    /// of this type, or the notation <c>arr.[x]</c> to get/set array
    /// values.</remarks>
    type 'T ``[]`` = (# "!0[]" #)

    /// <summary>Two dimensional arrays, typically zero-based.</summary> 
    ///
    /// <remarks>Use the values in the <c>Array2D</c> module
    /// to manipulate values of this type, or the notation <c>arr.[x,y]</c> to get/set array
    /// values.   
    ///
    /// Non-zero-based arrays can also be created using methods on the System.Array type.</remarks>
    type 'T ``[,]`` = (# "!0[0 ... , 0 ... ]" #)

    /// <summary>Three dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <remarks>Use the values in the <c>Array3D</c> module
    /// to manipulate values of this type, or the notation <c>arr.[x1,x2,x3]</c> to get and set array
    /// values.</remarks>
    type 'T ``[,,]`` = (# "!0[0 ...,0 ...,0 ...]" #)

    /// <summary>Four dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <remarks>Use the values in the <c>Array4D</c> module
    /// to manipulate values of this type, or the notation <c>arr.[x1,x2,x3,x4]</c> to get and set array
    /// values.</remarks>  
    type 'T ``[,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...]" #)
    
    /// <summary>Five dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Six dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Seven dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Eight dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Nine dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Ten dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Eleven dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twelve dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Fourteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Fifteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Sixteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Seventeen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Eighteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Nineteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-one dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-two dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-three dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-four dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-five dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-six dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-seven dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-eight dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-nine dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirty dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirty-one dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirty-two dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Single dimensional, zero-based arrays, written <c>int[]</c>, <c>string[]</c> etc.</summary>
    /// 
    /// <remarks>Use the values in the <c>Array</c> module to manipulate values 
    /// of this type, or the notation <c>arr.[x]</c> to get/set array
    /// values.</remarks>   
    type 'T array = 'T[]
            
           
    /// <summary>Represents a managed pointer in F# code.</summary>
    type byref<'T> = (# "!0&" #)

    /// <summary>Represents an unmanaged pointer in F# code.</summary>
    ///
    /// <remarks>This type should only be used when writing F# code that interoperates
    /// with native code.  Use of this type in F# code may result in
    /// unverifiable code being generated.  Conversions to and from the 
    /// <c>nativeint</c> type may be required. Values of this type can be generated
    /// by the functions in the <c>NativeInterop.NativePtr</c> module.</remarks>
    type nativeptr<'T when 'T : unmanaged> = (# "native int" #)

    /// <summary>This type is for internal use by the F# code generator.</summary>
    type ilsigptr<'T> = (# "!0*" #)

