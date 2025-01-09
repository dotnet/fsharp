// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module UnionTypes =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compileAndRun

    //SOURCE=BeginWithUppercase01.fsx SCFLAGS=                                                    # BeginWithUppercase01.fsx
    [<Theory; FileInlineData("BeginWithUppercase01.fsx")>]
    let ``BeginWithUppercase01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //# Renaming the .exe because for some weird reason on some OSes having 'DispatchSlot' in the .exe
    //# seems to trigger the UAC dialog... (e.g. Win7 x86)
    //SOURCE=DispatchSlot_Equals01.fsx SCFLAGS="-o dl_equals01.exe"                               # DispatchSlot_Equals01.fsx
    [<Theory; FileInlineData("DispatchSlot_Equals01.fsx")>]
    let ``DispatchSlot_Equals01_fsx`` compilation =
        compilation
        |> getCompilation
        |> withName "dl_equals01"
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=DispatchSlot_GetHashCode.fsx SCFLAGS="-o dl_gethashcode01.exe"                       # DispatchSlot_GetHashCode.fsx
    [<Theory; FileInlineData("DispatchSlot_GetHashCode.fsx")>]
    let ``DispatchSlot_GetHashCode_fsx`` compilation =
        compilation
        |> getCompilation
        |> withName "dl_gethashcode01"
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=EqualAndBoxing01.fs                                                                  # EqualAndBoxing01.fs
    [<Theory; FileInlineData("EqualAndBoxing01.fs")>]
    let ``EqualAndBoxing01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=E_BeginWithUppercase01.fsx SCFLAGS="--test:ErrorRanges"                              # E_BeginWithUppercase01.fsx
    [<Theory; FileInlineData("E_BeginWithUppercase01.fsx")>]
    let ``E_BeginWithUppercase01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 53, Line 9, Col 12, Line 9, Col 13, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute")
            (Error 53, Line 10, Col 12, Line 10, Col 13, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute")
        ]

    //SOURCE=E_BeginWithUppercase02.fsx SCFLAGS="--test:ErrorRanges"                              # E_BeginWithUppercase02.fsx
    [<Theory; FileInlineData("E_BeginWithUppercase02.fsx")>]
    let ``E_BeginWithUppercase02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 8, Col 12, Line 8, Col 13, "Unexpected integer literal in union case")
        ]

    //SOURCE=E_BeginWithUppercase03.fsx SCFLAGS="--test:ErrorRanges"                              # E_BeginWithUppercase03.fsx
    [<Theory; FileInlineData("E_BeginWithUppercase03.fsx")>]
    let ``E_BeginWithUppercase03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 9, Col 12, Line 9, Col 15, "Unexpected string literal in union case")
        ]

    //SOURCE=E_BeginWithUppercase04.fsx SCFLAGS="--test:ErrorRanges"                              # E_BeginWithUppercase04.fsx
    [<Theory; FileInlineData("E_BeginWithUppercase04.fsx")>]
    let ``E_BeginWithUppercase04_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3563, Line 9, Col 12, Line 9, Col 18, "This is not a valid identifier")
        ]

    //SOURCE=E_BeginWithUppercaseNoPipe01.fsx SCFLAGS="--test:ErrorRanges"                        # E_BeginWithUppercaseNoPipe01.fsx
    [<Theory; FileInlineData("E_BeginWithUppercaseNoPipe01.fsx")>]
    let ``E_BeginWithUppercaseNoPipe01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 883, Line 7, Col 8, Line 7, Col 13, "Invalid namespace, module, type or union case name")
            (Error 53, Line 11, Col 18, Line 11, Col 19, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute")
            (Error 53, Line 12, Col 18, Line 12, Col 19, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute")
        ]

    //SOURCE=E_GenericFunctionValuedStaticProp01.fs SCFLAGS="--test:ErrorRanges --warnaserror-"   # E_GenericFunctionValuedStaticProp01.fs
    [<Theory; FileInlineData("E_GenericFunctionValuedStaticProp01.fs")>]
    let ``E_GenericFunctionValuedStaticProp01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1125, Line 11, Col 29, Line 11, Col 41, "The instantiation of the generic type 'list1' is missing and can't be inferred from the arguments or return type of this member. Consider providing a type instantiation when accessing this type, e.g. 'list1<_>'.")
            (Error 3068, Line 15, Col 10, Line 15, Col 25, "The function or member 'toList' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types. The inferred signature is 'static member private list1.toList: ('a list1 -> 'a list)'.")
        ]

    //SOURCE=E_Interface_IComparable.fsx SCFLAGS="--test:ErrorRanges"                             # E_Interface_IComparable.fsx
    [<Theory; FileInlineData("E_Interface_IComparable.fsx")>]
    let ``E_Interface_IComparable_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 67, Line 12, Col 3, Line 12, Col 24, "This type test or downcast will always hold")
            (Error 16, Line 12, Col 3, Line 12, Col 24, "The type 'I' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion.")
        ]

    //SOURCE=E_Member_Duplicate01.fsx SCFLAGS="--test:ErrorRanges"                                # E_Member_Duplicate01.fsx
    [<Theory; FileInlineData("E_Member_Duplicate01.fsx")>]
    let ``E_Member_Duplicate01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 434, Line 7, Col 12, Line 7, Col 13, "The property 'IsC' has the same name as a method in type 'T'.")
            (Error 23, Line 9, Col 19, Line 9, Col 22, "The member 'IsC' cannot be defined because the name 'IsC' clashes with the default augmentation of the union case 'C' in this type or module")
            (Error 23, Line 13, Col 24, Line 13, Col 27, "The member 'IsC' cannot be defined because the name 'IsC' clashes with the default augmentation of the union case 'C' in this type or module")
        ]

    //SOURCE=E_ScopeAndDataConstrAndPattern01.fsx SCFLAGS="--test:ErrorRanges"	                # E_ScopeAndDataConstrAndPattern01.fsx
    [<Theory; FileInlineData("E_ScopeAndDataConstrAndPattern01.fsx")>]
    let ``E_ScopeAndDataConstrAndPattern01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 37, Col 19, Line 37, Col 29, "The value or constructor 'CaseLabel1' is not defined.")
        ]

    //SOURCE=E_Interface_IStructuralHash.fsx                                                      # E_Interface_IStructuralHash.fsx
    [<Theory; FileInlineData("E_Interface_IStructuralHash.fsx")>]
    let ``E_Interface_IStructuralHash_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 17, Col 15, Line 17, Col 30, "The type 'IStructuralHash' is not defined.")
            (Error 72, Line 18, Col 1, Line 18, Col 25, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
            (Error 39, Line 21, Col 15, Line 21, Col 30, "The type 'IStructuralHash' is not defined.")
            (Error 72, Line 22, Col 1, Line 22, Col 25, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
        ]

    //SOURCE=E_FieldNameUsedMulti.fs SCFLAGS="--test:ErrorRanges"                                 # E_FieldNameUsedMulti.fs
    [<Theory; FileInlineData("E_FieldNameUsedMulti.fs")>]
    let ``E_FieldNameUsedMulti_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3175, Line 10, Col 23, Line 10, Col 25, "Union case/exception field 'V1' cannot be used more than once.")
            (Error 3175, Line 15, Col 18, Line 15, Col 20, "Union case/exception field 'V2' cannot be used more than once.")
            (Error 3175, Line 18, Col 20, Line 18, Col 22, "Union case/exception field 'V1' cannot be used more than once.")
        ]

    //SOURCE=E_FieldMemberClash.fs SCFLAGS="--test:ErrorRanges"                                   # E_FieldMemberClash.fs
    [<Theory; FileInlineData("E_FieldMemberClash.fs")>]
    let ``E_FieldMemberClash_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 12, Col 17, Line 12, Col 22, "The member 'Item1' cannot be defined because the name 'Item1' clashes with the generated property 'Item1' in this type or module")
            (Error 23, Line 13, Col 17, Line 13, Col 22, "The member 'Item2' cannot be defined because the name 'Item2' clashes with the generated property 'Item2' in this type or module")
            (Error 23, Line 14, Col 17, Line 14, Col 19, "The member 'V3' cannot be defined because the name 'V3' clashes with the generated property 'V3' in this type or module")
            (Error 23, Line 19, Col 17, Line 19, Col 21, "The member 'Item' cannot be defined because the name 'Item' clashes with the generated property 'Item' in this type or module")
        ]

    //SOURCE=E_InheritUnion.fs                                                                    # E_InheritUnion.fs
    [<Theory; FileInlineData("E_InheritUnion.fs")>]
    let ``E_InheritUnion_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 961, Line 10, Col 5, Line 10, Col 12, "This 'inherit' declaration specifies the inherited type but no arguments. Consider supplying arguments, e.g. 'inherit BaseType(args)'.")
            (Error 945, Line 10, Col 13, Line 10, Col 22, "Cannot inherit a sealed type")
        ]

    //SOURCE=E_LowercaseDT.fs                                                                     # E_LowercaseDT.fs
    [<Theory; FileInlineData("E_LowercaseDT.fs")>]
    let ``E_LowercaseDT_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 53, Line 7, Col 13, Line 7, Col 17, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute")
        ]

    //SOURCE=E_Overload_Equals.fs SCFLAGS="--test:ErrorRanges"                                    # E_Overload_Equals.fs
    [<Theory; FileInlineData("E_Overload_Equals.fs")>]
    let ``E_Overload_Equals_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 438, Line 6, Col 6, Line 6, Col 8, "Duplicate method. The method 'Equals' has the same name and signature as another method in type 'DU'.")
            (Error 438, Line 7, Col 23, Line 7, Col 29, "Duplicate method. The method 'Equals' has the same name and signature as another method in type 'DU'.")
        ]

    //SOURCE=E_Overload_GetHashCode.fs SCFLAGS="--test:ErrorRanges"                               # E_Overload_GetHashCode.fs
    [<Theory; FileInlineData("E_Overload_GetHashCode.fs")>]
    let ``E_Overload_GetHashCode_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 438, Line 7, Col 6, Line 7, Col 8, "Duplicate method. The method 'GetHashCode' has the same name and signature as another method in type 'DU'.")
            (Error 438, Line 8, Col 25, Line 8, Col 36, "Duplicate method. The method 'GetHashCode' has the same name and signature as another method in type 'DU'.")
        ]

    //SOURCE=E_SampleFromSpec01d.fsx SCFLAGS="--test:ErrorRanges"                                 # E_SampleFromSpec01d.fsx
    [<Theory; FileInlineData("E_SampleFromSpec01d.fsx")>]
    let ``E_SampleFromSpec01d_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 13, Col 19, Line 13, Col 25, "Unexpected keyword 'member' in implementation file")
        ]

    //SOURCE=E_SampleFromSpec01d2.fsx SCFLAGS="--test:ErrorRanges"                                # E_SampleFromSpec01d2.fsx
    [<Theory; FileInlineData("E_SampleFromSpec01d2.fsx")>]
    let ``E_SampleFromSpec01d2_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 13, Col 1, Line 13, Col 7, "Unexpected keyword 'member' in implementation file")
        ]

    //SOURCE=E_UnionConstructorBadFieldName.fs SCFLAGS="--test:ErrorRanges"                       # E_UnionConstructorBadFieldName.fs
    [<Theory; FileInlineData("E_UnionConstructorBadFieldName.fs")>]
    let ``E_UnionConstructorBadFieldName_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3174, Line 10, Col 18, Line 10, Col 20, "The union case 'Case1' does not have a field named 'V3'.")
            (Error 3174, Line 14, Col 9, Line 14, Col 11, "The union case 'Case1' does not have a field named 'V3'.")
            (Warning 26, Line 15, Col 3, Line 15, Col 4, "This rule will never be matched")
            (Error 3174, Line 17, Col 12, Line 17, Col 14, "The union case 'Case1' does not have a field named 'V4'.")
            (Error 3174, Line 19, Col 25, Line 19, Col 26, "The union case 'Some' does not have a field named 'a'.")
        ]

    //SOURCE=E_UnionFieldConflictingName.fs SCFLAGS="--test:ErrorRanges"                          # E_UnionFieldConflictingName.fs
    [<Theory; FileInlineData("E_UnionFieldConflictingName.fs")>]
    let ``E_UnionFieldConflictingName_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 7, Col 16, Line 7, Col 21, "Named field 'Item2' conflicts with autogenerated name for anonymous field.");
            (Error 3176, Line 10, Col 16, Line 10, Col 17, "Named field 'A' is used more than once.");
            (Error 3176, Line 10, Col 26, Line 10, Col 27, "Named field 'A' is used more than once.")
        ]

    //SOURCE=E_UnionFieldNamedTag.fs SCFLAGS="--test:ErrorRanges"                                 # E_UnionFieldNamedTag.fs
    [<Theory; FileInlineData("E_UnionFieldNamedTag.fs")>]
    let ``E_UnionFieldNamedTag_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1219, Line 6, Col 7, Line 6, Col 11, "The union case named 'Tags' conflicts with the generated type 'Tags'")
        ]

    //SOURCE=E_UnionFieldNamedTagNoDefault.fs SCFLAGS="--test:ErrorRanges"                        # E_UnionFieldNamedTagNoDefault.fs
    [<Theory; FileInlineData("E_UnionFieldNamedTagNoDefault.fs")>]
    let ``E_UnionFieldNamedTagNoDefault_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1219, Line 7, Col 7, Line 7, Col 11, "The union case named 'Tags' conflicts with the generated type 'Tags'")
        ]

    //SOURCE=E_UnionMemberNamedTag.fs SCFLAGS="--test:ErrorRanges"                                # E_UnionMemberNamedTag.fs
    [<Theory; FileInlineData("E_UnionMemberNamedTag.fs")>]
    let ``E_UnionMemberNamedTag_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1219, Line 9, Col 7, Line 9, Col 11, "The union case named 'Tags' conflicts with the generated type 'Tags'")
        ]

    //SOURCE=E_UnionMemberNamedTagNoDefault.fs SCFLAGS="--test:ErrorRanges"                       # E_UnionMemberNamedTagNoDefault.fs
    [<Theory; FileInlineData("E_UnionMemberNamedTagNoDefault.fs")>]
    let ``E_UnionMemberNamedTagNoDefault_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 19, Col 14, Line 19, Col 17, "The member 'Tag' cannot be defined because the name 'Tag' clashes with the generated property 'Tag' in this type or module")
        ]

    //SOURCE=E_UnionMemberNamedTags.fs SCFLAGS="--test:ErrorRanges"                               # E_UnionMemberNamedTags.fs
    [<Theory; FileInlineData("E_UnionMemberNamedTags.fs")>]
    let ``E_UnionMemberNamedTags_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 19, Col 14, Line 19, Col 18, "The member 'Tags' cannot be defined because the name 'Tags' clashes with the generated type 'Tags' in this type or module")
        ]

    //SOURCE=E_UnionMemberNamedTagsNoDefault.fs SCFLAGS="--test:ErrorRanges"                      # E_UnionMemberNamedTagsNoDefault.fs
    [<Theory; FileInlineData("E_UnionMemberNamedTagsNoDefault.fs")>]
    let ``E_UnionMemberNamedTagsNoDefault_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 19, Col 14, Line 19, Col 18, "The member 'Tags' cannot be defined because the name 'Tags' clashes with the generated type 'Tags' in this type or module")
        ]

    //SOURCE=E_UnionsNotNull01.fs                                                                 # E_UnionsNotNull01.fs
    [<Theory; FileInlineData("E_UnionsNotNull01.fs")>]
    let ``E_UnionsNotNull01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 43, Line 9, Col 14, Line 9, Col 18, "The type 'DU' does not have 'null' as a proper value")
        ]

    //SOURCE=ImplicitEquals001.fs                                                                 # ImplicitEquals001.fs
    [<Theory; FileInlineData("ImplicitEquals001.fs")>]
    let ``ImplicitEquals001_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Interface01.fsx SCFLAGS=                                                             # Interface01.fsx
    [<Theory; FileInlineData("Interface01.fsx")>]
    let ``Interface01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Interface_IComparable.fsx SCFLAGS=                                                   # Interface_IComparable.fsx
    [<Theory; FileInlineData("Interface_IComparable.fsx")>]
    let ``Interface_IComparable_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Member01.fsx                                                                         # Member01.fsx
    [<Theory; FileInlineData("Member01.fsx")>]
    let ``Member01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=NamedFields01.fsx SCFLAGS=                                                           # NamedFields01.fsx
    [<Theory; FileInlineData("NamedFields01.fsx")>]
    let ``NamedFields01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=NamedFields02.fsx SCFLAGS=                                                           # NamedFields02.fsx
    [<Theory; FileInlineData("NamedFields02.fsx")>]
    let ``NamedFields02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=NamedFields03.fsx SCFLAGS=                                                           # NamedFields03.fsx
    [<Theory; FileInlineData("NamedFields03.fsx")>]
    let ``NamedFields03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; FileInlineData("Overload_Equals.fs")>]
    let ``Overload_Equals_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; FileInlineData("Overload_GetHashCode.fs")>]
    let ``Overload_GetHashCode_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; FileInlineData("Overload_ToString.fs")>]
    let ``Overload_ToString_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompile
        |> shouldSucceed

    //SOURCE=Overrides01.fsx                                                                      # Overrides01.fsx
    [<Theory; FileInlineData("Overrides01.fsx")>]
    let ``Overrides01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Parenthesis01.fsx                                                                    # Parenthesis01.fsx
    [<Theory; FileInlineData("Parenthesis01.fsx")>]
    let ``Parenthesis01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Parenthesis02.fsx                                                                    # Parenthesis02.fsx
    [<Theory; FileInlineData("Parenthesis02.fsx")>]
    let ``Parenthesis02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Parenthesis03.fsx                                                                    # Parenthesis03.fsx
    [<Theory; FileInlineData("Parenthesis03.fsx")>]
    let ``Parenthesis03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=Recursive01.fsx SCFLAGS=                                                             # Recursive01.fsx
    [<Theory; FileInlineData("Recursive01.fsx")>]
    let ``Recursive01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=ReflectionOnUnionTypes01.fs SCFLAGS=                                                 # ReflectionOnUnionTypes01.fs
    [<Theory; FileInlineData("ReflectionOnUnionTypes01.fs")>]
    let ``ReflectionOnUnionTypes01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=SampleFromSpec01.fsx SCFLAGS	                                                        # SampleFromSpec01.fsx
    [<Theory; FileInlineData("SampleFromSpec01.fsx")>]
    let ``SampleFromSpec01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=SampleFromSpec01b.fsx SCFLAGS=                                                       # SampleFromSpec01b.fsx
    [<Theory; FileInlineData("SampleFromSpec01b.fsx")>]
    let ``SampleFromSpec01b_fsx`` compilation =
        compilation
        |> getCompilation
        |> withOcamlCompat
        |> withLangVersion50
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=SampleFromSpec01d.fsx SCFLAGS=                                                       # SampleFromSpec01d.fsx
    [<Theory; FileInlineData("SampleFromSpec01d.fsx")>]
    let ``SampleFromSpec01d_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=ScopeAndDataConstrAndPattern01.fsx SCFLAGS=                                          # ScopeAndDataConstrAndPattern01.fsx
    [<Theory; FileInlineData("ScopeAndDataConstrAndPattern01.fsx")>]
    let ``ScopeAndDataConstrAndPattern01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=UnionCaseProduction01.fsx SCFLAGS=-a                                                 # UnionCaseProduction01.fsx
    [<Theory; FileInlineData("UnionCaseProduction01.fsx")>]
    let ``UnionCaseProduction01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=UnionCasesProduction01.fsx SCFLAGS=-a                                                # UnionCasesProduction01.fsx
    [<Theory; FileInlineData("UnionCasesProduction01.fsx")>]
    let ``UnionCasesProduction01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=UnionsNotNull02.fs                                                                 # UnionsNotNull02.fs
    [<Theory; FileInlineData("UnionsNotNull02.fs")>]
    let ``UnionsNotNull02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=LowercaseWhenRequireQualifiedAccess.fsx                                                                 # LowercaseWhenRequireQualifiedAccess.fsx
    [<Theory; FileInlineData("LowercaseWhenRequireQualifiedAccess.fsx")>]
    let ``LowercaseWhenRequireQualifiedAccess_fs in langversion 6`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion60
        |> verifyCompile
        |> shouldFail

    //SOURCE=LowercaseWhenRequireQualifiedAccess.fsx                                                                 # LowercaseWhenRequireQualifiedAccess.fsx
    [<Theory; FileInlineData("LowercaseWhenRequireQualifiedAccess.fsx")>]
    let ``LowercaseWhenRequireQualifiedAccess_fs in preview`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion70
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=E_LowercaseWhenRequireQualifiedAccess.fsx                                                                 # E_LowercaseWhenRequireQualifiedAccess.fsx
    [<Theory; FileInlineData("E_LowercaseWhenRequireQualifiedAccess.fsx")>]
    let ``E_LowercaseWhenRequireQualifiedAccess_fs in preview`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion70
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 883, Line 6, Col 14, Line 6, Col 29, "Invalid namespace, module, type or union case name");
            (Error 53, Line 6, Col 14, Line 6, Col 29, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute");
            (Error 883, Line 8, Col 12, Line 8, Col 27, "Invalid namespace, module, type or union case name");
            (Error 53, Line 8, Col 12, Line 8, Col 27, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute");
            (Error 883, Line 11, Col 14, Line 11, Col 29, "Invalid namespace, module, type or union case name");
            (Error 883, Line 14, Col 12, Line 14, Col 27, "Invalid namespace, module, type or union case name");
            (Error 53, Line 16, Col 14, Line 16, Col 15, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute");
            (Error 53, Line 18, Col 12, Line 18, Col 13, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute");
            (Error 53, Line 20, Col 12, Line 20, Col 15, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute");
            (Error 53, Line 22, Col 14, Line 22, Col 17, "Lowercase discriminated union cases are only allowed when using RequireQualifiedAccess attribute")
            (Error 883, Line 32, Col 7, Line 32, Col 14, "Invalid namespace, module, type or union case name");
            (Error 883, Line 37, Col 7, Line 37, Col 14, "Invalid namespace, module, type or union case name")
        ]

    //SOURCE=E_LowercaseWhenRequireQualifiedAccess.fsx                                                                 # E_LowercaseWhenRequireQualifiedAccess.fsx
    [<Theory; FileInlineData("E_LowercaseWhenRequireQualifiedAccess.fsx")>]
    let ``E_LowercaseWhenRequireQualifiedAccess_fs in langversion 6`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion60
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 883, Line 6, Col 14, Line 6, Col 29, "Invalid namespace, module, type or union case name");
            (Error 53, Line 6, Col 14, Line 6, Col 29, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 883, Line 8, Col 12, Line 8, Col 27, "Invalid namespace, module, type or union case name");
            (Error 53, Line 8, Col 12, Line 8, Col 27, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 883, Line 11, Col 14, Line 11, Col 29, "Invalid namespace, module, type or union case name");
            (Error 53, Line 11, Col 14, Line 11, Col 29, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 883, Line 14, Col 12, Line 14, Col 27, "Invalid namespace, module, type or union case name");
            (Error 53, Line 14, Col 12, Line 14, Col 27, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 53, Line 16, Col 14, Line 16, Col 15, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 53, Line 18, Col 12, Line 18, Col 13, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 53, Line 20, Col 12, Line 20, Col 15, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 53, Line 22, Col 14, Line 22, Col 17, "Discriminated union cases and exception labels must be uppercase identifiers")
            (Error 53, Line 26, Col 7, Line 26, Col 8, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 53, Line 27, Col 7, Line 27, Col 12, "Discriminated union cases and exception labels must be uppercase identifiers")
            (Error 53, Line 31, Col 7, Line 31, Col 8, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 883, Line 32, Col 7, Line 32, Col 14, "Invalid namespace, module, type or union case name");
            (Error 53, Line 32, Col 7, Line 32, Col 14, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 53, Line 36, Col 7, Line 36, Col 8, "Discriminated union cases and exception labels must be uppercase identifiers");
            (Error 883, Line 37, Col 7, Line 37, Col 14, "Invalid namespace, module, type or union case name")
        ]

    //SOURCE=W_GenericFunctionValuedStaticProp02.fs SCFLAGS="--test:ErrorRanges --warnaserror-"   # W_GenericFunctionValuedStaticProp02.fs
    [<Theory; FileInlineData("W_GenericFunctionValuedStaticProp02.fs")>]
    let ``W_GenericFunctionValuedStaticProp02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1125, Line 12, Col 29, Line 12, Col 41, "The instantiation of the generic type 'list1' is missing and can't be inferred from the arguments or return type of this member. Consider providing a type instantiation when accessing this type, e.g. 'list1<_>'.")
        ]

    //SOURCE=W_SampleFromSpec01c.fsx SCFLAGS="--test:ErrorRanges"                                 # W_SampleFromSpec01c.fsx
    [<Theory; FileInlineData("W_SampleFromSpec01c.fsx")>]
    let ``W_SampleFromSpec01c_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 58, Line 9, Col 1, Line 9, Col 2, "Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (8:19). Try indenting this further.\nTo continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.")
            (Error 547, Line 8, Col 24, Line 8, Col 33, "A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.")
            (Error 10, Line 9, Col 1, Line 9, Col 2, "Unexpected symbol '|' in implementation file")
        ]

    //SOURCE=W_UnionCaseProduction01.fsx SCFLAGS="-a --test:ErrorRanges"                          # W_UnionCaseProduction01.fsx
    [<Theory; FileInlineData("W_UnionCaseProduction01.fsx")>]
    let ``W_UnionCaseProduction01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 42, Line 11, Col 12, Line 11, Col 24, "This construct is deprecated: it is only for use in the F# library")
        ]

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``UnionCaseIsTester inlined and SignatureData`` userec =

        let kwrec = if userec then "rec" else ""
        let myLibraryFsi =
            SourceCodeFileKind.Create(
                "myLibrary.fsi",
                $"""
module {kwrec} MyLibrary

    [<RequireQualifiedAccess>]
    type PrimaryAssembly =
        | Mscorlib
        | System_Runtime
        | NetStandard""")

        let myLibraryFs =
            SourceCodeFileKind.Create(
                "myLibrary.fs",
                $"""
module {kwrec} MyLibrary

    [<RequireQualifiedAccess>]
    type PrimaryAssembly =
        | Mscorlib
        | System_Runtime
        | NetStandard
                """)

        let myFileFs =
            SourceCodeFileKind.Create(
                "myFile.fs",
                $"""
module {kwrec} FileName

    open MyLibrary
    let inline getAssemblyType () = PrimaryAssembly.NetStandard
    let inline isNetStandard () = (PrimaryAssembly.NetStandard).IsNetStandard
                """)

        let myLibrary =
            (fsFromString myLibraryFsi) |> FS
            |> withAdditionalSourceFiles [myLibraryFs; myFileFs]
            |> asLibrary
            |> withName "MyLibrary"

        Fs """
let x = FileName.getAssemblyType().IsNetStandard
let y = FileName.getAssemblyType()
let z = FileName.isNetStandard()
printfn "%b %A %b" x y z
           """
            |> asExe
            |> withReferences [myLibrary]
            |> compileAndRun
            |> shouldSucceed

    //SOURCE=W_UnionCaseProduction01.fsx SCFLAGS="-a --test:ErrorRanges"                          # W_UnionCaseProduction01.fsx
    [<Fact>]
    let ``UnionCaseInitialization_repro16431`` () =

        let testFs =
            SourceCodeFileKind.Create(
                "testFs.fs",
                $"""
module Test

type ABC =
    | A
    | B
    | C of int

    static let c75' = ABC.C 75
    static member c75 = c75'

    static let ab' = [ A; B ]
    static member ab = ab'
                 """)

        let programFs =
            SourceCodeFileKind.Create(
                "programFs.fs",
                $"""
open Test

if (sprintf "%%A" ABC.c75) <> "C 75" then failwith (sprintf "Failed: printing 'ABC.c75': Expected output: 'C 75'  Actual output: '%%A'" ABC.c75)
if (sprintf "%%A" ABC.ab) <> "[A; B]" then failwith (sprintf "Failed: printing 'ABC.ab: Expected: '[A; B]'  Actual: '%%A'" ABC.ab)
                 """)

        (fsFromString testFs)
        |> FS
        |> withAdditionalSourceFiles [programFs]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        
    [<Fact>]
    let ``Error when declaring an abstract member in union type`` () =
        Fsx """
type U = 
  | A | B
  abstract M : unit -> unit
       """
        |> typecheck 
        |> shouldFail
        |>  withSingleDiagnostic (Error 912, Line 4, Col 3, Line 4, Col 28, "This declaration element is not permitted in an augmentation")
        
        
    [<Fact>]
    let ``Error when property has same name as DU case`` () =
        Fsx """
type MyId =
    | IdA of int
    | IdB of string
    | IdC of float

    member this.IdA =
        match this with
        | IdA x -> Some x
        | _ -> None
        
    member this.IdX =
        match this with
        | IdB x -> Some x
        | _ -> None

    member this.IdC =
        match this with
        | IdC x -> Some x
        | _ -> None
       """
        |> typecheck 
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 7, Col 17, Line 7, Col 20, "The member 'IdA' cannot be defined because the name 'IdA' clashes with the union case 'IdA' in this type or module")
            (Error 23, Line 17, Col 17, Line 17, Col 20, "The member 'IdC' cannot be defined because the name 'IdC' clashes with the union case 'IdC' in this type or module")
        ]
        

    [<Fact>]
    let ``Union field appears multiple times in union declaration`` () =
        Fsx """
type X =
    | A of a: int * a: int
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 3, Col 12, Line 3, Col 13, "Named field 'a' is used more than once.")
        ]

    [<Fact>]
    let ``Union field appears multiple times in union declaration 2`` () =
        Fsx """
type X =
    | A of a: int * a: int
    | B of a: int * a: int
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 3, Col 12, Line 3, Col 13, "Named field 'a' is used more than once.")
            (Error 3176, Line 4, Col 12, Line 4, Col 13, "Named field 'a' is used more than once.")
        ]

    [<Fact>]
    let ``Union field appears multiple times in union declaration 3`` () =
        Fsx """
type X =
    | A of a: int * a: int * a: int
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 3, Col 12, Line 3, Col 13, "Named field 'a' is used more than once.")
            (Error 3176, Line 3, Col 21, Line 3, Col 22, "Named field 'a' is used more than once.")
        ]

    [<Fact>]
    let ``Union field appears multiple times in union declaration 4`` () =
        Fsx """
type X =
    | A of a: int * a: int
let x = A (1, 2)
match x with
| A(a = 1) -> ()
| _ -> ()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 3, Col 12, Line 3, Col 13, "Named field 'a' is used more than once.")
        ]
