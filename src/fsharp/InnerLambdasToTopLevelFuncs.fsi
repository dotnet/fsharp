// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.InnerLambdasToTopLevelFuncs 

open FSharp.Compiler 
open FSharp.Compiler.TcGlobals

val MakeTLRDecisions : Tast.CcuThunk -> TcGlobals -> Tast.TypedImplFile -> Tast.TypedImplFile
