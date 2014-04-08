module Test.Test

open Test.Lib

let testVector3DotInline (v1: Vector3) =
    Vector3.dot v1 v1

let testVector3MutableFieldDotInline (v1: Vector3MutableField) =
    Vector3MutableField.dot v1 v1

let testVector3NestedMutableFieldTestInline (v1: Vector3NestedMutableField) =
    Vector3NestedMutableField.test v1 v1

let testVector3GenericInline (v1: Vector3Generic<'T>) =
    Vector3Generic.test v1 v1