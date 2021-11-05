---
title: Public Surface Area
category: Compiler
categoryindex: 1
index: 8
---
# The F# Compiler Service Public Surface Area

The "intended" FCS API is the parts under the namespaces

* FSharp.Compiler.SourceCodeServices.* (analysis, compilation, tooling, lexing)
* FSharp.Compiler.Interactive.Shell.*  (scripting support)
* FSharp.Compiler.AbstractIL.*  (for ILAssemblyReader hook for Rider)
* FSharp.Compiler.Syntax.*  (direct access to full untyped tree)

These sections are generally designed with F#/.NET design conventions (e.g. types in namespaces, not modules, no nesting of modules etc.)
and we will continue to iterate to make this so.

In contrast, the public parts of the compiler directly under `FSharp.Compiler.*` and `FSharp.AbstractIL.*` are
"incidental" and not really designed for public use apart from the hook for JetBrains Rider
(Aside: In theory all these other parts could be renamed to FSharp.Compiler though there's no need to do that right now).  
These internal parts tend to be implemented with the "module containing lots of stuff in one big file" approach for layers of the compiler.

