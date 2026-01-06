// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Symbols.XmlDocInheritance

open FSharp.Compiler.Text
open FSharp.Compiler.Xml

/// Expands `<inheritdoc>` elements in XML documentation (currently a placeholder)
/// Returns the original documentation unchanged
/// TODO: Implement full inheritdoc expansion
let expandInheritDoc (_m: range) (doc: XmlDoc) : XmlDoc =
    // Placeholder implementation - just return the original doc
    // Full implementation would:
    // 1. Check for <inheritdoc> elements in the XML
    // 2. Resolve the target (from cref or implicit from override/interface)
    // 3. Retrieve and merge the inherited documentation
    // 4. Apply path filters if specified
    doc
