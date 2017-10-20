// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range

#if COMPILER_PUBLIC_API
module UnusedOpens =
#else
module internal UnusedOpens =
#endif
    val getUnusedOpens : symbolUses: FSharpSymbolUse[] * parsedInput: ParsedInput * getSourceLineStr: (int -> string) -> range list