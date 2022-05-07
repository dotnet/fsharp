// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Printer for the abstract syntax.
module internal FSharp.Compiler.AbstractIL.ILAsciiWriter

open FSharp.Compiler.AbstractIL.IL
open System.IO

#if DEBUG
val public output_module: TextWriter -> ilg: ILGlobals -> ILModuleDef -> unit
#endif
