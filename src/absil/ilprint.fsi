// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Printer for the abstract syntax.
module internal Microsoft.FSharp.Compiler.AbstractIL.ILAsciiWriter 

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open System.IO

#if DEBUG
val public output_module      : TextWriter -> ILModuleDef -> unit
#endif

