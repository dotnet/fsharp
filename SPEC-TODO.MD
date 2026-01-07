# `<inheritdoc>` XML Documentation Implementation Specification

## Overview
This document outlines the implementation plan for `<inheritdoc>` XML documentation support in the F# compiler. The feature allows documentation to be inherited from base classes, implemented interfaces, or explicitly referenced symbols.

## Research Summary

### C# Behavior (from Microsoft Learn)
- `<inheritdoc />` allows inheriting documentation from base members or interfaces
- Supports `cref` attribute to explicitly specify the source: `<inheritdoc cref="TypeOrMember"/>`
- Supports `path` attribute for XPath filtering: `<inheritdoc path="/summary"/>`
- C# compiler copies the tag literally to XML output files (doesn't expand it)
- IDE (Visual Studio) expands inheritdoc for IntelliSense tooltips at design time
- External tools (DocFX, Sandcastle) expand inheritdoc in generated documentation

### Roslyn Implementation (from GitHub research)
Located in: `src/Workspaces/Core/Portable/Shared/Extensions/ISymbolExtensions.cs`
- `GetDocumentationComment()` method with `expandIncludes` and `expandInheritdoc` parameters
- Expansion happens in Workspace layer, not compiler layer
- Supports both implicit inheritance (from overrides/interfaces) and explicit (via `cref`)
- XPath support for `path` attribute using `System.Xml.XPath`
- Cycle detection to prevent infinite recursion

## Requirements

### 1. Must Support Both IL and Current Compilation
- **IL symbols**: Documentation from referenced assemblies (`.xml` files)
- **Current compilation**: Documentation from symbols in the same project
- Use `InfoReader.TryFindXmlDocByAssemblyNameAndSig` for external symbols
- Access `.XmlDoc` property directly for internal symbols

### 2. Must Work in Two Contexts

#### A. XML File Production (`WriteXmlDocFile`)
- Expand `<inheritdoc>` when writing `.xml` documentation files
- This is the compile-time path (when building with `--doc` flag)
- Location: `src/Compiler/Driver/XmlDocFileWriter.fs`

#### B. Design-Time Tooltips (`GetXmlCommentForItem`)
- Expand `<inheritdoc>` for IDE tooltips/IntelliSense
- Works even when no `.dll` or `.xml` is written
- Location: `src/Compiler/Symbols/SymbolHelpers.fs`

### 3. Inheritance Resolution Rules
- **Implicit** (no `cref`): Inherit from overridden base method or implemented interface member
- **Explicit** (`cref` specified): Inherit from specifically referenced symbol
- **Filtered** (`path` specified): Use XPath to select specific parts (e.g., `<summary>` only)

## Implementation Checklist

### Phase 1: Parser Implementation ✅
- [x] Create `XmlDocSigParser.fsi` and `XmlDocSigParser.fs`
- [x] Parse doc comment IDs (format: `M:Namespace.Type.Method`, `T:Namespace.Type`, etc.)
- [x] Add to project file before SymbolHelpers
- [ ] **TEST**: Add unit tests for parsing various doc comment ID formats
  - [ ] Type references: `T:Namespace.Type`
  - [ ] Method references: `M:Namespace.Type.Method`
  - [ ] Property references: `P:Namespace.Type.Property`
  - [ ] Field references: `F:Namespace.Type.Field`
  - [ ] Generic types: `T:Namespace.Generic\`1`
  - [ ] Generic methods: `M:Namespace.Type.Method\`\`1`

### Phase 2: Core Expansion Logic ✅ (Placeholder only)
- [x] Create `XmlDocInheritance.fsi` and `XmlDocInheritance.fs`
- [ ] **IMPLEMENT**: Replace placeholder with real expansion logic
  - [ ] Parse XML to find `<inheritdoc>` elements
  - [ ] Extract `cref` and `path` attributes
  - [ ] Resolve target symbol
  - [ ] Retrieve target documentation
  - [ ] Apply XPath filter if `path` specified
  - [ ] Replace `<inheritdoc>` with inherited content
  - [ ] Handle cycle detection
- [ ] **TEST**: Unit tests for expansion logic
  - [ ] Implicit inheritance from base class method
  - [ ] Implicit inheritance from interface method
  - [ ] Explicit inheritance via `cref`
  - [ ] Partial inheritance via `path` (summary only)
  - [ ] Partial inheritance via `path` (remarks only)
  - [ ] Nested inheritdoc (A inherits from B, B inherits from C)
  - [ ] Cycle detection (A inherits from B, B inherits from A)
  - [ ] Multiple `<inheritdoc>` in same comment

### Phase 3: Symbol Resolution
- [ ] **IMPLEMENT**: `resolveSymbolByCref` function
  - [ ] Parse cref using `XmlDocSigParser`
  - [ ] For internal symbols: Walk CCU entities to find match
  - [ ] For external symbols: Use `TryFindXmlDocByAssemblyNameAndSig`
  - [ ] Handle generic type parameters
  - [ ] Handle nested types
- [ ] **TEST**: Symbol resolution tests
  - [ ] Resolve internal F# type
  - [ ] Resolve internal F# method
  - [ ] Resolve external .NET type (from System)
  - [ ] Resolve external .NET method
  - [ ] Resolve generic type
  - [ ] Resolve generic method
  - [ ] Handle unresolvable cref (emit warning)

### Phase 4: Implicit Target Resolution
- [ ] **IMPLEMENT**: `findImplicitTarget` function
  - [ ] For `ValRef`: Check `MemberInfo.ImplementedSlotSigs` for interface implementations
  - [ ] For `ValRef`: Check `IsDefiniteFSharpOverrideMember` for overrides
  - [ ] For `TyconRef`: Check `tcaug_super` for base class
  - [ ] Return `None` if no implicit target found
- [ ] **TEST**: Implicit target resolution tests
  - [ ] Find interface method being implemented
  - [ ] Find base class method being overridden
  - [ ] Find base class for type
  - [ ] Return None when no base/interface exists

### Phase 5: Integration with XmlDocFileWriter
- [x] Update `WriteXmlDocFile` signature to accept `InfoReader`
- [x] Update call site in `fsc.fs`
- [ ] **IMPLEMENT**: Create proper `XmlDocTarget` from `Val`/`Tycon`/etc
  - [ ] Build ValRef from Val (need to track parent entity)
  - [ ] Build TyconRef from Tycon
  - [ ] Build UnionCaseRef from UnionCase
  - [ ] Build RecdFieldRef from RecdField
- [ ] **IMPLEMENT**: Call `expandInheritDoc` before writing XML
- [ ] **TEST**: End-to-end XML generation tests
  - [ ] Compile project with inheritdoc, verify .xml output
  - [ ] Interface implementation inherits docs in .xml
  - [ ] Override inherits docs in .xml
  - [ ] Explicit cref inherits docs in .xml
  - [ ] Path filter works in .xml output

### Phase 6: Integration with SymbolHelpers (Tooltips)
- [ ] **IMPLEMENT**: Convert `Item` to `XmlDocTarget`
  - [ ] Handle `Item.Value` -> `XmlDocTarget.Val`
  - [ ] Handle `Item.Types` -> `XmlDocTarget.Type`
  - [ ] Handle `Item.UnionCase` -> `XmlDocTarget.UnionCase`
  - [ ] Handle `Item.RecdField` -> `XmlDocTarget.RecdField`
- [ ] **IMPLEMENT**: Call `expandInheritDoc` in `GetXmlCommentForItemAux`
- [ ] **TEST**: Design-time tooltip tests
  - [ ] Hover over interface implementation shows inherited docs
  - [ ] Hover over override shows inherited docs
  - [ ] Works without building project
  - [ ] Works in IDE scenarios

### Phase 7: XPath Support
- [ ] **IMPLEMENT**: Apply XPath filters from `path` attribute
  - [ ] Use `System.Xml.XPath.Extensions.XPathSelectElements`
  - [ ] Handle absolute paths (prepend `/*` wrapper)
  - [ ] Handle relative paths
  - [ ] Handle invalid XPath (emit warning)
- [ ] **TEST**: XPath filtering tests
  - [ ] `path="/summary"` returns only summary
  - [ ] `path="/remarks"` returns only remarks
  - [ ] `path="/param[@name='x']"` returns specific param
  - [ ] Invalid XPath emits warning
  - [ ] Empty result handled gracefully

### Phase 8: Error Handling
- [x] Add `xmlDocInheritDocError` message to FSComp.txt
- [ ] **IMPLEMENT**: Warning emissions
  - [ ] Warn on unresolvable cref
  - [ ] Warn on circular inheritance
  - [ ] Warn on invalid XPath
  - [ ] Warn on malformed XML in inheritdoc
- [ ] **TEST**: Error handling tests
  - [ ] Unresolvable cref produces warning
  - [ ] Circular inheritance produces warning and doesn't hang
  - [ ] Invalid XPath produces warning
  - [ ] Malformed XML handled gracefully

### Phase 9: Component Tests
- [ ] **CREATE**: `tests/FSharp.Compiler.ComponentTests/Miscellaneous/XmlDocInheritance.fs`
- [ ] **TEST**: Real-world scenarios
  - [ ] Interface implementation scenario
  - [ ] Base class override scenario
  - [ ] Multi-level inheritance (A->B->C)
  - [ ] Generic type inheritance
  - [ ] Explicit cref to different type
  - [ ] Partial doc with path filter
  - [ ] Cross-assembly inheritance (from System types)
  - [ ] F# inheriting from C# documented type

### Phase 10: Documentation and Cleanup
- [ ] Add XML comments to all public functions
- [ ] Update PR description with final status
- [ ] Add usage examples to F# documentation
- [ ] Consider performance implications
- [ ] Run full test suite
- [ ] Format code with Fantomas

## Technical Challenges to Resolve

### Challenge 1: Type Accessibility
**Problem**: `ValRef`, `TyconRef`, `InfoReader` are internal types
**Solution**: Keep expansion logic internal, don't expose in public module signatures

### Challenge 2: Val -> ValRef Conversion
**Problem**: In `WriteXmlDocFile` context, we have `Val` but need `ValRef`
**Solution**: 
- Track parent entity context during traversal
- Construct ValRef using parent TyconRef
- Alternative: Pass ValRef from call sites instead of Val

### Challenge 3: Namespace Issues
**Problem**: `FSharp.Compiler.Symbols` not accessible from Driver
**Solution**: Move types to TypedTree or use fully qualified names

### Challenge 4: Phase Mismatch
**Problem**: XML generation is post-type-checking
**Solution**: Pass InfoReader through the call chain to enable symbol resolution

## Testing Strategy

### Unit Tests
- Test parser in isolation
- Test expansion logic with mock XML
- Test symbol resolution with known types
- Test XPath filtering

### Component Tests  
- Test realistic F# code scenarios
- Test cross-assembly scenarios
- Test IDE tooltip scenarios
- Test XML file generation

### Integration Tests
- Build real projects with inheritdoc
- Verify .xml output matches expectations
- Test in VS Code/Visual Studio

## Success Criteria
1. ✅ Code builds without errors or warnings
2. ❌ All unit tests pass
3. ❌ All component tests pass
4. ❌ XML files generated correctly at compile time
5. ❌ Tooltips show inherited docs at design time
6. ❌ Works with both IL and F# symbols
7. ❌ Handles cycles without hanging
8. ❌ Emits appropriate warnings for errors

## References
- Microsoft Learn: [Recommended XML documentation tags](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags)
- Roslyn: `src/Workspaces/Core/Portable/Shared/Extensions/ISymbolExtensions.cs`
- Roslyn Issues: [#68879](https://github.com/dotnet/roslyn/issues/68879), [#50192](https://github.com/dotnet/roslyn/discussions/50192)
- F# Compiler: `src/Compiler/Driver/XmlDocFileWriter.fs`, `src/Compiler/Symbols/SymbolHelpers.fs`
