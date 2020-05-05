// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

/// Patterns over FSharpSymbol and derivatives.
[<RequireQualifiedAccess>]
module public Symbol =

    val isAttribute<'T> : FSharpAttribute -> bool

    val tryGetAttribute<'T> : seq<FSharpAttribute> -> FSharpAttribute option

    val hasModuleSuffixAttribute : FSharpEntity -> bool

    val isOperator : name: string -> bool

    val isUnnamedUnionCaseField : FSharpField -> bool

    val (|AbbreviatedType|_|) : FSharpEntity -> FSharpType option

    val (|TypeWithDefinition|_|) : FSharpType -> FSharpEntity option

    val getEntityAbbreviatedType : FSharpEntity -> (FSharpEntity * FSharpType option)

    val getAbbreviatedType : FSharpType -> FSharpType

    val (|Attribute|_|) : FSharpEntity -> unit option

    val hasAttribute<'T> : seq<FSharpAttribute> -> bool

    val (|ValueType|_|) : FSharpEntity -> unit option

    val (|Class|_|) : original: FSharpEntity * abbreviated: FSharpEntity * 'a -> unit option 

    val (|Record|_|) : FSharpEntity -> unit option

    val (|UnionType|_|) : FSharpEntity -> unit option

    val (|Delegate|_|) : FSharpEntity -> unit option

    val (|FSharpException|_|) : FSharpEntity -> unit option

    val (|Interface|_|) : FSharpEntity -> unit option

    val (|AbstractClass|_|) : FSharpEntity -> unit option

    val (|FSharpType|_|) : FSharpEntity -> unit option

#if !NO_EXTENSIONTYPING    
    val (|ProvidedType|_|) : FSharpEntity -> unit option
#endif    

    val (|ByRef|_|) : FSharpEntity -> unit option

    val (|Array|_|) : FSharpEntity -> unit option

    val (|FSharpModule|_|) : FSharpEntity -> unit option

    val (|Namespace|_|) : FSharpEntity -> unit option

#if !NO_EXTENSIONTYPING    
    val (|ProvidedAndErasedType|_|) : FSharpEntity -> unit option
#endif

    val (|Enum|_|) : FSharpEntity -> unit option

    val (|Tuple|_|) : FSharpType -> unit option

    val (|RefCell|_|) : FSharpType -> unit option

    val (|FunctionType|_|) : FSharpType -> unit option

    val (|Pattern|_|) : FSharpSymbol -> unit option

    val (|Field|_|) : FSharpSymbol -> (FSharpField * FSharpType) option

    val (|MutableVar|_|) : FSharpSymbol -> unit option

    val (|FSharpEntity|_|) : FSharpSymbol -> (FSharpEntity * FSharpEntity * FSharpType option) option

    val (|Parameter|_|) : FSharpSymbol -> unit option

    val (|UnionCase|_|) : FSharpSymbol -> FSharpUnionCase option

    val (|RecordField|_|) : FSharpSymbol -> FSharpField option

    val (|ActivePatternCase|_|) : FSharpSymbol -> FSharpActivePatternCase option

    val (|MemberFunctionOrValue|_|) : FSharpSymbol -> FSharpMemberOrFunctionOrValue option

    val (|Constructor|_|) : FSharpMemberOrFunctionOrValue -> FSharpEntity option

    val (|Function|_|) : excluded: bool -> FSharpMemberOrFunctionOrValue -> unit option

    val (|ExtensionMember|_|) : FSharpMemberOrFunctionOrValue -> unit option

    val (|Event|_|) : FSharpMemberOrFunctionOrValue -> unit option