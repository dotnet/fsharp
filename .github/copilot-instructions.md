# GitHub Copilot Instructions for F# Compiler

## Language and Environment
- Use modern F# with functional-first approach
- Target .NET Standard 2.0 for compatibility
- Avoid external dependencies - the codebase is self-contained

## Code Style and Standards
- Follow [coding standards](docs/coding-standards.md), especially:
  - Prefer immutability and pure functions
  - Use consistent naming (see abbreviation guide)
  - Process complex types recursively with pattern matching
  - Avoid single-character identifiers except in established patterns
- This project uses .fsi signature files. When adding a new public API to a namespace/module which will be consumed from other file, add it to the respective .fsi signature file as well
- Follow [overview](docs/overview.md) for key compiler data formats and representations

## Type System Handling
- When working with `TType` instances, use appropriate stripping functions:
  - `stripTyparEqns` - Removes inference equations from type parameters
  - `stripTyEqns` - Removes type equations and type abbreviations
  - `stripTyEqnsAndErase` - Also erases compilation representations
  - Match the appropriate stripper to the context (e.g., display vs compilation)
  - Check equality with `typeEquiv` after stripping rather than direct comparison

## Core Data Structures
- Use F# discriminated unions for type representations
- Respect representation hiding through access control
- Use `remapX` functions when transforming types across boundaries
- Be aware of performance implications for recursive type operations

## Documentation and Naming
- Document public APIs with XML comments
- Use descriptive function names that indicate transformation direction
- Follow established naming conventions in error messages
