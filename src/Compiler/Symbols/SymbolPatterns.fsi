// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Symbols

/// Patterns over FSharpSymbol and derivatives.
[<Experimental("This module is subject to future redesign. Consider using patterns checking for properties of symbols directly, e.g. entity.IsFSharpRecord or entity.IsFSharpUnion")>]
module public FSharpSymbolPatterns =

    [<return: Struct>]
    val (|AbbreviatedType|_|): FSharpEntity -> FSharpType voption

    [<return: Struct>]
    val (|TypeWithDefinition|_|): FSharpType -> FSharpEntity voption

    [<return: Struct>]
    val (|Attribute|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|ValueType|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|Class|_|): original: FSharpEntity * abbreviated: FSharpEntity * 'a -> unit voption

    [<return: Struct>]
    val (|Record|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|UnionType|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|Delegate|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|FSharpException|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|Interface|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|AbstractClass|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|FSharpType|_|): FSharpEntity -> unit voption

#if !NO_TYPEPROVIDERS
    [<return: Struct>]
    val (|ProvidedType|_|): FSharpEntity -> unit voption
#endif

    [<return: Struct>]
    val (|ByRef|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|Array|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|FSharpModule|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|Namespace|_|): FSharpEntity -> unit voption

#if !NO_TYPEPROVIDERS
    [<return: Struct>]
    val (|ProvidedAndErasedType|_|): FSharpEntity -> unit voption
#endif

    [<return: Struct>]
    val (|Enum|_|): FSharpEntity -> unit voption

    [<return: Struct>]
    val (|Tuple|_|): FSharpType -> unit voption

    [<return: Struct>]
    val (|RefCell|_|): FSharpType -> unit voption

    [<return: Struct>]
    val (|FunctionType|_|): FSharpType -> unit voption

    [<return: Struct>]
    val (|Pattern|_|): FSharpSymbol -> unit voption

    [<return: Struct>]
    val (|Field|_|): FSharpSymbol -> (FSharpField * FSharpType) voption

    [<return: Struct>]
    val (|MutableVar|_|): FSharpSymbol -> unit voption

    /// Returns (originalEntity, abbreviatedEntity, abbreviatedType)
    [<return: Struct>]
    val (|FSharpEntity|_|): FSharpSymbol -> (FSharpEntity * FSharpEntity * FSharpType option) voption

    [<return: Struct>]
    val (|Parameter|_|): FSharpSymbol -> unit voption

    [<return: Struct>]
    val (|UnionCase|_|): FSharpSymbol -> FSharpUnionCase voption

    [<return: Struct>]
    val (|RecordField|_|): FSharpSymbol -> FSharpField voption

    [<return: Struct>]
    val (|ActivePatternCase|_|): FSharpSymbol -> FSharpActivePatternCase voption

    [<return: Struct>]
    val (|MemberFunctionOrValue|_|): FSharpSymbol -> FSharpMemberOrFunctionOrValue voption

    [<return: Struct>]
    val (|Constructor|_|): FSharpMemberOrFunctionOrValue -> FSharpEntity voption

    [<return: Struct>]
    val (|Function|_|): excluded: bool -> FSharpMemberOrFunctionOrValue -> unit voption

    [<return: Struct>]
    val (|ExtensionMember|_|): FSharpMemberOrFunctionOrValue -> unit voption

    [<return: Struct>]
    val (|Event|_|): FSharpMemberOrFunctionOrValue -> unit voption

    val internal hasModuleSuffixAttribute: FSharpEntity -> bool
