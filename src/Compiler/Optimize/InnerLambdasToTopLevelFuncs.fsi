// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.InnerLambdasToTopLevelFuncs

open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

val MakeTopLevelRepresentationDecisions: CcuThunk -> TcGlobals -> CheckedImplFile -> CheckedImplFile
