# Object Browser and Class View for F#

Implemented in `vsintegration/src/FSharp.Editor/ObjectBrowser/`. This note records how it works and
what is deliberately left out.

Object Browser and Class View are served by **one** library object. The shell asks the same
`IVsSimpleLibrary2` for both and distinguishes them only by passing `LLF_TRUENESTING` for Class View.
The design follows Roslyn's (`roslyn/src/VisualStudio/Core/Def/Library/**`), because the window
protocol — list types, category fields, `IVsNavInfo` paths — is fixed by the shell, not by the
language. Where Roslyn queries a `Compilation` for `ISymbol`s, we query FCS for `FSharpEntity` and
friends.

## Files

| File | Contents |
|---|---|
| `NavInfo.fs` | `IVsNavInfo`/`IVsNavInfo2`/`IVsNavInfoNode` and their enumerator: the canonical path `[owner, "Project References"] → library → namespace → class → member`, rendered two ways (dotted names expanded for canonical, whole for presentation). |
| `ObjectBrowserModel.fs` | List kinds, list-type/flag helpers, the item record, the glyph mapping, and `IObjectBrowserHost` — the seam that lets lists build children without depending on the manager that creates them. |
| `ObjectBrowserItems.fs` | FCS symbol graph → immutable item arrays. Every symbol access is guarded, because an entity imported from a broken reference throws rather than reporting itself unresolved. |
| `ObjectBrowserDescription.fs` | The description pane: F# declaration, "Member of …" hyperlink, and the XML summary via `XmlDocumentation.AppendXmlComment` — the same path QuickInfo uses, so plain-text doc comments and metadata symbols' companion XML files both work. |
| `ObjectBrowserList.fs` | `IVsSimpleObjectList2` — display data, category fields, expandability, child-list dispatch, Go To Definition, per-item NavInfo. |
| `ObjectBrowserLibrary.fs` | `IVsSimpleLibrary2` — library flags, root list, `CreateNavInfo`, update counters, background project checks, navigation, Find Symbol. |

Registration is the two `protected virtual` hooks Roslyn's `AbstractPackage` already provides:
`FSharpPackage.RegisterObjectBrowserLibraryManager` / `UnregisterObjectBrowserLibraryManager`
(`LanguageService/LanguageService.fs`). The base class calls them on the main thread after package
load and skips them in command-line mode, so there is no pkgdef entry and no scheduling of our own.
The library GUID is `Guids.fsharpLibraryId` in `Common/Constants.fs`; it ends up in persisted window
state, so it must never change.

## The one problem that is not Roslyn's

The whole `IVsSimpleObjectList2` surface is synchronous COM on the UI thread, and Roslyn blocks on
`GetCompilationAsync` inside it. F# cannot: populating a project means `ParseAndCheckProject`, which
can take seconds to minutes on a cold solution. Blocking there would hang Visual Studio.

Instead:

- A project node whose symbols are not cached yet renders a single non-expandable placeholder item
  and starts the check in the background.
- When the check lands, the update counters move; the shell re-reads the list and the placeholder is
  replaced by real content. `FSharpObjectList` rebuilds its items exactly when its counter moves.
  Lists sourced from a reference node ride the package counter instead — an assembly's contents never
  change with edits, so they are not re-walked per keystroke.
- At most one check per project is in flight, the stale mark is consumed before computing (an edit
  arriving mid-check re-marks it), and a re-check after an edit is delayed to coalesce typing bursts.
  A failed check leaves the placeholder without bumping the counters, so there is no retry loop; a
  reset generation stops an in-flight check from resurrecting symbols after the solution was cleared.

Everything below the project node — namespaces, types, members, base types — is an in-memory walk of
symbols the check already produced, so those expansions are immediate.

## Where the data comes from

`Project.GetFSharpCompilationOptionsAsync()` → `FSharpChecker.ParseAndCheckProject` →

- `AssemblySignature.Entities`, flattened, for namespaces, modules and types (nested types are listed
  beside their containers and qualified by them, `Outer.Inner`, as Object Browser does for C#);
- `MembersFunctionsAndValues`, `FSharpFields`, `UnionCases` for members, plus members walked up the
  base chain and marked `LCMI_INHERITED`;
- `BaseType` and `DeclaredInterfaces` for the Base Types folder;
Reference rows deliberately do **not** come from that check. C#/VB read them from the Roslyn
project's `MetadataReferences`, which costs nothing; F# only populates those for the legacy project
system, so for SDK projects they are read from the `-r:` flags of `FSharpProjectOptions` instead —
equally cheap, and already cached per Roslyn project in `ProjectCache.Projects`. A cache miss is
warmed by one background pass over the solution rather than blocking the COM call. Expanding a
reference then resolves the `FSharpAssembly` by name from the owning project's
`ProjectContext.GetReferencedAssemblies()`, and its contents come from `FSharpAssembly.Contents`.

Go To Definition uses `DeclarationLocation` and the existing `IFSharpDocumentNavigationService` for
source symbols, and `FSharpMetadataAsSourceService` for symbols from metadata.

## Not implemented

- **Find All References from an Object Browser node.** The shell's Class View context menus are
  returned and routed through the library as an `IOleCommandTarget`, but it forwards everything to
  the shell rather than handling `FindReferences` itself.
- **Sync with Class View** (`VSStd2KCmdID.SYNCCLASSVIEW` from the editor).
- **Browse containers** (`IVsBrowseContainersList`), drag/drop, rename, delete, the Properties window
  browse object, and `LF_SUPPORTSCLASSDESIGNER` — F# has no class designer.
- **Wildcard search.** Find Symbol matches case-insensitive substrings, as Roslyn's does.

## Relationship to Roslyn issue 85089

Navigating from a metadata node currently opens the generated `.fsi` stub, the same one F12 has
always produced, positioned at the top of the file rather than at the symbol. `dotnet/roslyn#85089`
proposes a symbol-free MetadataAsSource seam keyed on `(assembly path, documentation comment id)`.
Object Browser items already carry both, so adopting it is a replacement of the call in
`ObjectBrowserLibrary.showMetadata` — nothing about the tree changes. It is an improvement to this
feature, not a prerequisite for it.

## Testing

`vsintegration/tests/FSharp.Editor.Tests/ObjectBrowserNavInfoTests.fs` covers the pure logic: NavInfo
canonical vs. presentation node construction, the Class-View-only reference-owner prefix, symbol
type, and the list-type/flag mapping. The COM surface itself needs the VSIX in an experimental
instance — check Class View and Object Browser, Find Symbol, Go To Definition on source and metadata
nodes, and that a list refreshes while a project is still being checked.
