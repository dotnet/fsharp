module Test

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


let testVector3DotInline (v1: Vector3) =
    Vector3.dot v1 v1