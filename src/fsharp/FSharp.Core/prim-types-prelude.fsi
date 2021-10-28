// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "35" // This construct is deprecated: the treatment of this operator is now handled directly by the F# compiler and its meaning may not be redefined.
#nowarn "61" // The containing type can use <c>null</c> as a representation value for its nullary union case. This member will be compiled as a static member.
#nowarn "62" // This construct is for ML compatibility. The syntax <c>module ... : sig .. end</c> is deprecated unless OCaml compatibility is enabled. Consider using <c>module ... = begin .. end'.

/// <summary>Basic F# type definitions, functions and operators.</summary> 
namespace Microsoft.FSharp.Core

    open System

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Object"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type obj = System.Object

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Exception"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type exn = System.Exception

    /// <summary>An abbreviation for the CLI type <see cref="T:System.IntPtr"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type nativeint = System.IntPtr

    /// <summary>An abbreviation for the CLI type <see cref="T:System.UIntPtr"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type unativeint = System.UIntPtr

    /// <summary>An abbreviation for the CLI type <see cref="T:System.String"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type string = System.String

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Single"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type float32 = System.Single

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Double"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type float = System.Double

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Single"/>. Identical to <see cref="T:Microsoft.FSharp.Core.float32"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type single = System.Single

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Double"/>. Identical to <see cref="T:Microsoft.FSharp.Core.float"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type double = System.Double

    /// <summary>An abbreviation for the CLI type <see cref="T:System.SByte"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type sbyte = System.SByte

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Byte"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type byte = System.Byte

    /// <summary>An abbreviation for the CLI type <see cref="T:System.SByte"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type int8 = System.SByte

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Byte"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type uint8 = System.Byte

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Int16"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type int16 = System.Int16

    /// <summary>An abbreviation for the CLI type <see cref="T:System.UInt16"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type uint16 = System.UInt16

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Int32"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type int32 = System.Int32

    /// <summary>An abbreviation for the CLI type <see cref="T:System.UInt32"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type uint32 = System.UInt32

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Int64"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type int64 = System.Int64

    /// <summary>An abbreviation for the CLI type <see cref="T:System.UInt64"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type uint64 = System.UInt64

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Char"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type char = System.Char

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Boolean"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type bool = System.Boolean

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Decimal"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type decimal = System.Decimal

    /// <summary>An abbreviation for the CLI type <see cref="T:System.Int32"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type int = int32

    /// <summary>An abbreviation for the CLI type <see cref="T:System.UInt32"/>.</summary>
    ///
    /// <category>Basic Types</category>
    type uint = uint32

    /// <summary>Single dimensional, zero-based arrays, written <c>int[]</c>, <c>string[]</c> etc.</summary>
    ///
    /// <remarks>Use the values in the <c>Array</c> module to manipulate values 
    /// of this type, or the notation <c>arr.[x]</c> to get/set array
    /// values.</remarks>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[]`` = (# "!0[]" #)

    /// <summary>Two dimensional arrays, typically zero-based.</summary> 
    ///
    /// <remarks>Use the values in the <c>Array2D</c> module
    /// to manipulate values of this type, or the notation <c>arr.[x,y]</c> to get/set array
    /// values.   
    ///
    /// Non-zero-based arrays can also be created using methods on the System.Array type.</remarks>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,]`` = (# "!0[0 ... , 0 ... ]" #)

    /// <summary>Three dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <remarks>Use the values in the <c>Array3D</c> module
    /// to manipulate values of this type, or the notation <c>arr.[x1,x2,x3]</c> to get and set array
    /// values.</remarks>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,]`` = (# "!0[0 ...,0 ...,0 ...]" #)

    /// <summary>Four dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <remarks>Use the values in the <c>Array4D</c> module
    /// to manipulate values of this type, or the notation <c>arr.[x1,x2,x3,x4]</c> to get and set array
    /// values.</remarks>  
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...]" #)
    
    /// <summary>Five dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Six dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Seven dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Eight dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Nine dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,]`` = (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Ten dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Eleven dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twelve dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Fourteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Fifteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Sixteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Seventeen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Eighteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Nineteen dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-one dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-two dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-three dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-four dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-five dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-six dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-seven dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-eight dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Twenty-nine dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirty dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirty-one dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Thirty-two dimensional arrays, typically zero-based. Non-zero-based arrays
    /// can be created using methods on the System.Array type.</summary>
    ///
    /// <category>Basic Types</category>
    /// <exclude />
    type 'T ``[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]`` =
        (# "!0[0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...,0 ...]" #)

    /// <summary>Single dimensional, zero-based arrays, written <c>int[]</c>, <c>string[]</c> etc.</summary>
    /// 
    /// <remarks>Use the values in the <see cref="T:Microsoft.FSharp.Collections.ArrayModule" /> module to manipulate values 
    /// of this type, or the notation <c>arr.[x]</c> to get/set array
    /// values.</remarks>   
    ///
    /// <category>Basic Types</category>
    type 'T array = 'T[]
            
    /// <summary>Represents an unmanaged pointer in F# code.</summary>
    ///
    /// <remarks>This type should only be used when writing F# code that interoperates
    /// with native code. Use of this type in F# code may result in
    /// unverifiable code being generated. Conversions to and from the 
    /// <see cref="T:Microsoft.FSharp.Core.nativeint" /> type may be required. Values of this type can be generated
    /// by the functions in the <c>NativeInterop.NativePtr</c> module.</remarks>
    ///
    /// <category>ByRef and Pointer Types</category>
    type nativeptr<'T when 'T : unmanaged> = (# "native int" #)

    /// <summary>Represents an untyped unmanaged pointer in F# code.</summary>
    ///
    /// <remarks>This type should only be used when writing F# code that interoperates
    /// with native code. Use of this type in F# code may result in
    /// unverifiable code being generated. Conversions to and from the 
    /// <see cref="T:Microsoft.FSharp.Core.nativeint" /> type may be required. Values of this type can be generated
    /// by the functions in the <c>NativeInterop.NativePtr</c> module.</remarks>
    ///
    /// <category>ByRef and Pointer Types</category>
    type voidptr = (# "void*" #)

    /// <summary>Represents an Common IL (Intermediate Language) Signature Pointer.</summary>
    ///
    /// <remarks>This type should only be used when writing F# code that interoperates
    /// with other .NET languages that use generic Common IL Signature Pointers.
    /// Use of this type in F# code may result in unverifiable code being generated.
    /// Because of the rules of Common IL Signature Pointers, you cannot use this type in generic type parameters,
    /// resulting in compiler errors. As a result, you should convert this type to <see cref="T:Microsoft.FSharp.Core.nativeptr{T}" />
    /// for use in F#. Note that Common IL Signature Pointers exposed by other .NET languages are converted to
    /// <see cref="T:Microsoft.FSharp.Core.nativeptr{T}" /> or <see cref="T:Microsoft.FSharp.Core.voidptr" /> automatically by F#,
    /// and F# also shows generic-specialized typed native pointers correctly to other .NET languages as Common IL Signature Pointers.
    /// However, generic typed native pointers are shown as <see cref="T:System.IntPtr"/> to other .NET languages.
    /// For other languages to interpret generic F# typed native pointers correctly, you should expose this type or
    /// <see cref="T:Microsoft.FSharp.Core.voidptr" /> instead of <see cref="T:Microsoft.FSharp.Core.nativeptr{T}" />.
    /// Values of this type can be generated by the functions in the <c>NativeInterop.NativePtr</c> module.</remarks>
    ///
    /// <category>ByRef and Pointer Types</category>
    type ilsigptr<'T> = (# "!0*" #)

