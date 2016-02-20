namespace ThisNamespaceHasToBeTheSame

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

[<Struct>]
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

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3Generic<'T> =
    val x : 'T
    val mutable y : 'T
    val z : 'T

    new (x, y, z) = { x = x; y = y; z = z }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3GenericInt =
    let inline test (v1: Vector3Generic<int>) (v2: Vector3Generic<int>) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3GenericObj =
    let inline test (v1: Vector3Generic<obj>) (v2: Vector3Generic<obj>) =
        v1.x

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3StructRecord  =
    {
        x: single
        y: single
        z: single
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3StructRecord =
    let inline dot (v1: Vector3StructRecord) (v2: Vector3StructRecord) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3StructRecordMutableField  =
    {
        x: single
        mutable y: single
        z: single
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3StructRecordMutableField =
    let inline dot (v1: Vector3StructRecordMutableField) (v2: Vector3StructRecordMutableField) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

[<Struct>]
[<StructLayout (LayoutKind.Sequential)>]
type Vector3StructRecordGeneric<'T>  =
    {
        x: 'T
        mutable y: 'T
        z: 'T
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Vector3StructRecordGeneric =
    let inline dot (v1: Vector3StructRecordGeneric<single>) (v2: Vector3StructRecordGeneric<single>) =
        v1.x * v2.x + v1.y * v2.y + v1.z * v2.z

type HiddenRecord = 
    private { x : int } 
    member this.X = this.x

type HiddenUnion = 
    private A of int | B of string
    member this.X = match this with A x -> x | B s -> s.Length

type internal Foo private () = 
    static member FooMethod() = ()

[<System.Runtime.CompilerServices.InternalsVisibleToAttribute("lib3")>]
do()

[<System.Runtime.CompilerServices.InternalsVisibleToAttribute("lib3--optimize")>]
do()
