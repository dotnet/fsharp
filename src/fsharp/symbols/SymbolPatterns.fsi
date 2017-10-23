// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

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