module Test.Test

open ThisNamespaceHasToBeTheSame
open ASecondLibrary

let testVector3DotInline (v1: Vector3) =
    Vector3.dot v1 v1

let testVector3MutableFieldDotInline (v1: Vector3MutableField) =
    Vector3MutableField.dot v1 v1

let testVector3NestedMutableFieldTestInline (v1: Vector3NestedMutableField) =
    Vector3NestedMutableField.test v1 v1

let testVector3GenericInline (v1: Vector3Generic<int>) =
    Vector3GenericInt.test v1 v1

let testVector3GenericInline2 (v1: Vector3Generic<obj>) =
    Vector3GenericObj.test v1 v1

// This was the failing case for the first bug reported in https://github.com/Microsoft/visualfsharp/issues/532
//
let testAccessingSomethingInlinableThatUsesAPrivateInlinedConstructFromAThirdModule = 
    let boom1 = ThisNamespaceHasToBeTheSame.Factory.NewRecord ()
    let boom2 = ThisNamespaceHasToBeTheSame.Factory.NewUnionA ()
    let boom3 = ThisNamespaceHasToBeTheSame.Factory.NewUnionB ()
    boom1.X, boom2.X, boom3.X

// This is the failing case for the second bug reported in https://github.com/Microsoft/visualfsharp/issues/532
//
//let testAccessingSomethingInlinableThatUsesAInternalConstructFromAnInternalsVisibleToAssembly = 
//    Bar().BarMethod()

let testVector3StructRecord v1 v2 =
    Vector3StructRecord.dot v1 v2

let testVector3StructRecordMutableField v1 v2 =
    Vector3StructRecordMutableField.dot v1 v2

let testVector3StructRecordGeneric v1 v2 =
    Vector3StructRecordGeneric.dot v1 v2
