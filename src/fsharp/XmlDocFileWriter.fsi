// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocFileWriter

open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

module XmlDocWriter =

    val computeXmlDocSigs: tcGlobals: TcGlobals * generatedCcu: CcuThunk -> unit

    val writeXmlDoc: assemblyName: string * generatedCcu: CcuThunk * xmlfile: string -> unit
