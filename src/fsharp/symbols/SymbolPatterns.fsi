// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
module Symbol =
#else
module internal Symbol =
#endif
    open System.Text.RegularExpressions
    open System

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
    val (|ProvidedType|_|) : FSharpEntity -> unit option
    val (|ByRef|_|) : FSharpEntity -> unit option
    val (|Array|_|) : FSharpEntity -> unit option
    val (|FSharpModule|_|) : FSharpEntity -> unit option
    val (|Namespace|_|) : FSharpEntity -> unit option
    val (|ProvidedAndErasedType|_|) : FSharpEntity -> unit option
    val (|Enum|_|) : FSharpEntity -> unit option
    val (|Tuple|_|) : FSharpType option -> unit option
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

[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
module SymbolUse =
#else
module internal SymbolUse =
#endif
    val (|ActivePatternCase|_|) : FSharpSymbolUse -> FSharpActivePatternCase option
    val (|Entity|_|) : FSharpSymbolUse -> (FSharpEntity * (* cleanFullNames *) string list) option
    val (|Field|_|) : FSharpSymbolUse -> FSharpField option
    val (|GenericParameter|_|) : FSharpSymbolUse -> FSharpGenericParameter option
    val (|MemberFunctionOrValue|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|ActivePattern|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Parameter|_|) : FSharpSymbolUse -> FSharpParameter option
    val (|StaticParameter|_|) : FSharpSymbolUse -> FSharpStaticParameter option
    val (|UnionCase|_|) : FSharpSymbolUse -> FSharpUnionCase option
    val (|Constructor|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|TypeAbbreviation|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Class|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Delegate|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Event|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Property|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Method|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Function|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Operator|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Pattern|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|ClosureOrNestedFunction|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Val|_|) : FSharpSymbolUse -> FSharpMemberOrFunctionOrValue option
    val (|Enum|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Interface|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Module|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Namespace|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Record|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|Union|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|ValueType|_|) : FSharpSymbolUse -> FSharpEntity option
    val (|ComputationExpression|_|) : FSharpSymbolUse -> FSharpSymbolUse option
    val (|Attribute|_|) : FSharpSymbolUse -> FSharpEntity option