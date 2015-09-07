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

let testVector3Record v1 v2 =
    Vector3Record.dot v1 v2

let testVector3Record2 v1 v2 =
    Vector3Record.dot2 v1 v2

let testVector3RecordMutableField v1 v2 =
    Vector3RecordMutableField.dot v1 v2

let testVector3RecordMutableField2 v1 v2 =
    Vector3RecordMutableField.dot2 v1 v2

let testVector3RecordGenericObj v1 v2 =
    Vector3RecordGeneric.dotObj v1 v2

let testVector3RecordGeneric v1 v2 =
    Vector3RecordGeneric.dot v1 v2