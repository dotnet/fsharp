// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.InnerLambdasToTopLevelFuncs 

open FSharp.Compiler.TypedAST 
open FSharp.Compiler.TcGlobals

val MakeTLRDecisions : CcuThunk -> TcGlobals -> TypedImplFile -> TypedImplFile
