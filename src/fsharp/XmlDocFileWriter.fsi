// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocFileWriter

open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

module XmlDocWriter =
    /// Writes the XML document signature to the XmlDocSig property of each 
    /// element (field, union case, etc) of the specified compilation unit.
    val ComputeXmlDocSigs: tcGlobals: TcGlobals * generatedCcu: CcuThunk -> unit

    /// Writes the XmlDocSig property of each element (field, union case, etc) 
    /// of the specified compilation unit to an XML document in a new text file.
    val WriteXmlDocFile: assemblyName: string * generatedCcu: CcuThunk * xmlfile: string -> unit
