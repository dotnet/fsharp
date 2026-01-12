// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocInheritance

open FSharp.Compiler.InfoReader
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.Xml

/// Expands `<inheritdoc>` elements in XML documentation
/// Takes an optional InfoReader for resolving cref targets to their documentation
/// Takes an optional CCU for resolving same-compilation types
/// Takes an optional ModuleOrNamespaceType for accessing the current compilation's typed content
/// Takes an optional implicit target cref for resolving <inheritdoc/> without cref attribute
/// Takes a set of visited signatures to prevent cycles
val expandInheritDoc: infoReaderOpt: InfoReader option -> ccuOpt: CcuThunk option -> currentModuleTypeOpt: ModuleOrNamespaceType option -> implicitTargetCrefOpt: string option -> m: range -> visited: Set<string> -> doc: XmlDoc -> XmlDoc
