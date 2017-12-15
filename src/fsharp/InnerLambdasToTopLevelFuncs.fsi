// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.InnerLambdasToTopLevelFuncs 

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.TcGlobals

val MakeTLRDecisions : Tast.CcuThunk -> TcGlobals -> Tast.TypedImplFile -> Tast.TypedImplFile
