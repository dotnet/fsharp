module Test.Lib

#nowarn "9"

open System.Runtime.InteropServices

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3 =
    val x : single
    val y : single
    val z : single

    new (x, y, z) = { x = x; y = y; z = z }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3 =
    let inline dot (v1: Vector3) (v2: Vector3) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3MutableField =
    val x : single
    val mutable y : single
    val z : single

    new (x, y, z) = { x = x; y = y; z = z }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3MutableField =
    let inline dot (v1: Vector3MutableField) (v2: Vector3MutableField) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

[<StructLayout (LayoutKind.Sequential)>]
type Vector3NestedMutableField =
    val x : single
    val mutable y : Vector3MutableField
    val z : single

    new (x, y, z) = { x = x; y = y; z = z }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3NestedMutableField =
    let inline test (v1: Vector3NestedMutableField) (v2: Vector3NestedMutableField) =
        v1.x * v2.x + v1.y.y * v2.y.y + v1.z * v2.z

[<StructLayout (LayoutKind.Sequential)>]
type Vector3Generic<'T> =
    val x : 'T
    val mutable y : 'T
    val z : 'T

    new (x, y, z) = { x = x; y = y; z = z }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3Generic =
    let inline test (v1: Vector3Generic<int>) (v2: Vector3Generic<int>) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3Record  =
    {
        x: single
        y: single
        z: single
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3Record =
    let inline dot (v1: Vector3Record) (v2: Vector3Record) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

    let inline dot2 ({ x = x1; y = y1; z = z1}: Vector3Record) ({ x = x2; y = y2; z = z2}: Vector3Record) =
        x1 * x2 + y1 * y2 + z1 * z2

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3RecordMutableField  =
    {
        x: single
        mutable y: single
        z: single
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3RecordMutableField =
    let inline dot (v1: Vector3RecordMutableField) (v2: Vector3RecordMutableField) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

    let inline dot2 ({ x = x1; y = y1; z = z1}: Vector3RecordMutableField) ({ x = x2; y = y2; z = z2}: Vector3RecordMutableField) =
        x1 * x2 + y1 * y2 + z1 * z2

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3RecordGeneric<'T>  =
    {
        x: 'T
        mutable y: 'T
        z: 'T
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3RecordGeneric =
    let inline dotObj (v1: Vector3RecordGeneric<obj>) (v2: Vector3RecordGeneric<obj>) =
        v2.y

    let inline dot ({ x = x1; y = y1; z = z1}: Vector3RecordGeneric<single>) ({ x = x2; y = y2; z = z2}: Vector3RecordGeneric<single>) =
        x1 * x2 + y1 * y2 + z1 * z2