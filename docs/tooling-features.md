---
title: Overview
category: Language Service Internals
categoryindex: 300
index: 100
---
# Overview

The F# Language Service (FSharp.Editor, using FSharp.Compiler.Service) is designed to support tooling in Visual Studio and other IDEs. This document gives an overview of the features supported and notes on their technical characteristics.

## Kinds of data processed and served in F# tooling

The following tables are split into two categories: syntactic and semantic. They contain common kinds of information requested, the kind of data that is involved, and roughly how expensive the operation is in terms of expected memory allocation and CPU processing.

### IDE actions based on syntax

|  Action                                | Data inspected | Data returned | Expected CPU/Allocations (S/M/L/XL) |
|----------------------------------------|----------------|---------------|-----------------|
| Syntactic Classification               | Current doc's source text | Text span and classification type for each token in the document | S |
| Breakpoint Resolution                  | Current doc's syntax tree | Text span representing where breakpoints were resolved | S |
| Debugging data tip info                | Current doc's source text | Text span representing the token being inspected | S |
| Brace pair matching                    | Current doc's source text | Text spans representing brace pairs that match in the input document | S |
| "Smart" indentation                    | Current doc's source text | Indentation location in a document | S |
| Code fixes operating only on syntax    | Current doc's source text | Small text change for document | S |
| XML doc template generation            | Current doc's syntax tree | Small (usually) text change for document | S |
| Brace pair completion                  | Current doc's source text | Additional brace pair inserted into source text | S |
| Souce document navigation              | Current doc's syntax tree | "Navigation Items" with optional child navigation items containing ranges in source code | S |
| Code outlining                         | Current doc's source text | Text spans representing blocks of F# code that are collapsable as a group | S - M |
| Editor formatting                      | Current doc's source text | New source text for the document | S - L |
| Syntax diagnostics                     | Current doc's source text | List of diagnostic data including the span of text corresponding to the diagnostic | S |
| Global construct search and navigation | All syntax trees for all projects | All items that match a user's search pattern with spans of text that represent where a given item is located | S-L |

You likely noticed that nearly all of the syntactical operations are marked `S`. Aside from extreme cases, like files with 50k lines or higher, syntax-only operations typically finish very quickly. In addition to being computationally inexpensive, they are also run asynchronously and free-threaded.

Editor formatting is a bit of an exception. Most IDEs offer common commands for format an entire document, and although they also offer commands to format a given text selection, users typically choose to format the whole document. This means an entire document has to be inspected and potentially rewritten based on often complex rules. In practice this isn't bad when working with a document that has already been formatted, but it can be expensive for larger documents with strange stylistic choices.

Most of the syntax operations require an entire document's source text or parse tree. It stands to reason that this could be improved by operating on a diff of a parse tree instead of the whole thing. This is likely a very complex thing to implement though, since none of the F# compiler infrastructure works in this way today.

### IDE actions based on semantics

|  Action | Data inspected | Data returned | Expected CPU/Allocations (S/M/L/XL) |
|---------|---------------|---------------|-----------------|
| Most code fixes | Current document's typecheck data | Set (1 or more) of suggested text replacements | S-M |
| Semantic classification | Current document's typecheck data | Spans of text with semantic classification type for all constructs in a document | S-L |
| Lenses | Current document's typecheck data and top-level declarations (for showing signatures); graph of all projects that reference the current one (for showing references) | Signature data for each top-level construct; spans of text for each reference to a top-level construct with navigation information | S-XL |
| Code generation / refactorings | Current document's typecheck data and/or current resolved symbol/symbols | Text replacement(s) | S-L |
| Code completion | Current document's typecheck data and currently-resolved symbol user is typing at | List of all symbols in scope that are "completable" based on where completion is invoked | S-L |
| Editor tooltips | Current document's typecheck data and resolved symbol where user invoked a tooltip | F# tooltip data based on inspecting a type and its declarations, then pretty-printing them | S-XL |
| Diagnostics based on F# semantics | Current document's typecheck data | Diagnostic info for each symbol with diagnostics to show, including the range of text associated with the diagnostic | M-XL |
| Symbol highlighting in a document | Current document's typecheck data and currently-resolved symbol where user's caret is located | Ranges of text representing instances of that symbol in the document | S-M |
| Semantic navigation (for example, Go to Definition) | Current document's typecheck data and currently-resolved symbol where the user invoked navigation | Location of a symbol's declaration | S-M |
| Rename | Graph of all projects that use the symbol that rename is triggered on and the typecheck data for each of those projects | List of all uses of all symbols that are to be renamed | S-XL |
| Find all references | Graph of all projects that Find References is triggered on and the typecheck data for each of those projects | List of all uses of all symbols that are found | S-XL |
| Unused value/symbol analysis | Typecheck data for the current document | List of all symbols that aren't a public API and are unused | S-M |
| Unused `open` analysis | Typecheck data for the current document and all symbol data brought into scope by each `open` declaration | List of `open` declarations whose symbols it exposes aren't used in the current document | S-L |
| Missing `open` analysis | Typecheck data for the current document, resolved symbol with an error, and list of available namespaces or modules | List of candidate namespaces or modules that can be opened | S-M |
| Misspelled name suggestion analysis | Typecheck data for the current document and resolved symbol with an error | List of candidates that are in scope and best match the misspelled name based on a string distance algorithm | S-M |
| Name simplification analysis | Typecheck data for the current document and all symbol data brought into scope by each `open` declaration | List of text changes available for any fully- or partially-qualified symbol that can be simplified | S-XL |

You likely noticed that every cost associated with an action has a range. This is based on two factors:

1. If the semantic data being operated on is cached
2. How much semantic data must be processed for the action to be completed

Most actions are `S` if they operate on cached data and the compiler determines that no data needs to be re-computed. The size of their range is influenced largely by the _kind_ of semantic operations each action has to do, such as:

* Typechecking a single document and processing the resulting data
* Typechecking a document and its containing project and then processing the resulting data
* Resolving a single symbol in a document
* Resolving the definition of a single symbol in a codebase
* Inspecting all symbols brought into scope by a given `open` declaration
* Inspecting all symbols in a document
* Inspecting all symbols in all documents contained in a graph of projects

For example, commands like Find All References and Rename can be cheap if a codebase is small, hence the lower bound being `S`. But if the symbol in question is used across many documents in a large project graph, they are very expensive because the entire graph must be crawled and all symbols contained in its documents must be inspected.

In contrast, actions like highlighting all symbols in a document aren't terribly expensive even for very large files. That's because the symbols to be inspected are ultimately only in a single document.

