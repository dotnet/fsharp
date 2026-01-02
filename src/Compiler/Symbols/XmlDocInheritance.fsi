// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Symbols

open FSharp.Compiler.InfoReader
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.Xml

/// Represents a target for XML documentation expansion
[<RequireQualifiedAccess>]
type XmlDocTarget =
    /// Value or member reference
    | Val of ValRef
    /// Type reference
    | Type of TyconRef
    /// Union case reference
    | UnionCase of UnionCaseRef
    /// Record field reference
    | RecdField of RecdFieldRef

module XmlDocInheritance =
    /// Expands `<inheritdoc>` elements in XML documentation
    /// Returns the expanded documentation or the original if no inheritdoc is found
    val expandInheritDoc: infoReader: InfoReader -> m: range -> target: XmlDocTarget -> doc: XmlDoc -> XmlDoc
