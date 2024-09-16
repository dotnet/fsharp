namespace Language.FixedBindings

open Xunit
open FSharp.Test.Compiler

module Legacy =
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked string`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedString.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked array`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedArray.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked array with mismatching type`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedArrayWithMismatchingType.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 31, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 1, Line 5, Col 9, Line 5, Col 31, """Type mismatch. Expecting a
    'nativeptr<int>'    
but given a
    'nativeptr<byte>'    
The type 'int' does not match the type 'byte'""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked string with mismatching type`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedStringWithMismatchingType.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 31, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 1, Line 5, Col 9, Line 5, Col 31, """Type mismatch. Expecting a
    'nativeptr<char>'    
but given a
    'nativeptr<byte>'    
The type 'char' does not match the type 'byte'""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked Tuple -- illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedTuple.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked ValueTuple -- illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedValueTuple.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked discriminated union -- illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedDU.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 7, Col 9, Line 7, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 7, Col 9, Line 7, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked struct discriminated union -- illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedStructDU.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 8, Col 9, Line 8, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 8, Col 9, Line 8, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of array element`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfArrayElement.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of record field`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfRecordField.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 7, Col 9, Line 7, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 8, Col 5, Line 8, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of explicit field on this`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfExplicitFieldOnThis.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 11, Col 13, Line 11, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 12, Col 9, Line 12, Col 22, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked object - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedObject.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked int - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinNakedInt.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin generic - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinGeneric.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin generic with unmanaged - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinGenericWithUnmanagedConstraint.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin int byref parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntByrefParameter.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 5, Col 9, Line 5, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
    
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin int inref parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntInrefParameter.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 5, Col 9, Line 5, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin int outref parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntOutrefParameter.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 5, Col 9, Line 5, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin address of explicit field on this with default constructor class syntax`` langVersion =
        // I think F# 7 and lower should have allowed this and that this was really just a bug, but we should preserve the existing behavior
        // when turning the feature off
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfExplicitFieldOnThisWithDefaultCtorClassSyntax.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 8, Col 13, Line 8, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 8, Col 13, Line 8, Col 16, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 9, Col 9, Line 9, Col 22, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin int byref local variable`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntByrefLocalVariable.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 6, Col 9, Line 6, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

#if NETCOREAPP
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin Span`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinSpan.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 6, Col 9, Line 6, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
#endif
    
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin custom struct byref type without GetPinnableReference method - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinCustomStructByrefTypeWithoutGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 10, Col 9, Line 10, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 10, Col 9, Line 10, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with method GetPinnableReference : unit -> byref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 9, Col 9, Line 9, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with method GetPinnableReference : unit -> inref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceReturningInref.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 9, Col 9, Line 9, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with extension method GetPinnableReference : unit -> byref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithExtensionGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 13, Col 9, Line 13, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 14, Col 5, Line 14, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with method GetPinnableReference with parameters - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceWithParameters.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with method GetPinnableReference with non-byref return type - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceWithNonByrefReturnType.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with valid and invalid GetPinnableReference overloads`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithValidAndInvalidGetPinnableReferenceOverloads.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 11, Col 9, Line 11, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 11, Col 9, Line 11, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 12, Col 5, Line 12, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with private method GetPinnableReference - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithPrivateGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with static method GetPinnableReference - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithStaticGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
    
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with mismatching GetPinnableReference return type`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithMismatchingGetPinnableReferenceReturnType.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 31, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 9, Col 9, Line 9, Col 31, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Error 1, Line 9, Col 9, Line 9, Col 31, "Type mismatch. Expecting a
    'nativeptr<int>'    
but given a
    'nativeptr<char>'    
The type 'int' does not match the type 'char'")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    let ``Pin type with mismatching extension GetPinnableReference return type`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithMismatchingExtensionGetPinnableReferenceReturnType.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3350, Line 13, Col 9, Line 13, Col 12, """Feature 'extended fixed bindings for byref and GetPinnableReference' is not available in F# 7.0. Please use language version 8.0 or greater.""")
            (Warning 9, Line 14, Col 5, Line 14, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

// FS-1081 - Extend fixed bindings
module ExtendedFixedBindings =
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin int byref parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntByrefParameter.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin int inref parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntInrefParameter.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin int outref parameter`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntOutrefParameter.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin address of explicit field on this with default constructor class syntax`` langVersion =
        // I think F# 7 and lower should have allowed this and that this was really just a bug, but we should preserve the existing behavior
        // when turning the feature off
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinAddressOfExplicitFieldOnThisWithDefaultCtorClassSyntax.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 8, Col 13, Line 8, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 9, Col 9, Line 9, Col 22, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin int byref local variable`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinIntByrefLocalVariable.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

#if NETCOREAPP
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin Span`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinSpan.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
#endif
    
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin custom struct byref type without GetPinnableReference method - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinCustomStructByrefTypeWithoutGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 10, Col 9, Line 10, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 10, Col 9, Line 10, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with method GetPinnableReference : unit -> byref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ]
    
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with method GetPinnableReference : unit -> inref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceReturningInref.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with extension method GetPinnableReference : unit -> byref<T>`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithExtensionGetPinnableReferenceReturningByref.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 14, Col 5, Line 14, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with method GetPinnableReference with parameters - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceWithParameters.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with method GetPinnableReference with non-byref return type - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithGetPinnableReferenceWithNonByrefReturnType.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with a valid GetPinnableReference method and several invalid overloads`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithValidAndInvalidGetPinnableReferenceOverloads.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 11, Col 9, Line 11, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 12, Col 5, Line 12, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with private method GetPinnableReference - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithPrivateGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with static method GetPinnableReference - illegal`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithStaticGetPinnableReference.fs")
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, the address of an array element, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
    ]

    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with mismatching GetPinnableReference return type`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithMismatchingGetPinnableReferenceReturnType.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 31, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 1, Line 9, Col 9, Line 9, Col 31, "Type mismatch. Expecting a
    'nativeptr<int>'    
but given a
    'nativeptr<char>'    
The type 'int' does not match the type 'char'")
        ]
        
    [<Theory>]
    [<InlineData("preview")>]
    let ``Pin type with mismatching extension GetPinnableReference return type`` langVersion =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PinTypeWithMismatchingExtensionGetPinnableReferenceReturnType.fs")
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 14, Col 5, Line 14, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
