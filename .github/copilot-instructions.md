# GitHub Copilot Instructions for F# Compiler

## Build and test steps (only on Linux)

Always build and test the project before submitting your final solution.

In order to build and test, run  `./build.sh -c Release --testcoreclr` .

If the build fails with errors or non-zero exit code, fix it based on the error messages given and repeat the build step.

If either of the steps 3 fails, fix the errors and repeat - up to 3 times.
After that, report all relevant build errors, error messages and specific details about failing tests and their test test failure details.

### Fixing tests

- If any of the tests fail: Check if the test, test expectation (either inline in the test or a reference file configured for the test) or implementation needs updating, and fix it
- If you see test failures for Surface area baselines: Refer to "Updating FCS surface area baselines" in devguide.md
- If you see test failures for IL baselines: Refer to "Updating ILVerify baselines" in devguide.md


## Acceptance criteria

- Code is formatted using `dotnet fantomas .` executed at the repo root.
- Builds without errors.
- Runs tests without errors. If some tests needed adjustments, those test expectations/baseline adjustments were done.
- Follow the docs/release-notes folder by adding release notes. The guidelines are in docs/release-notes/About.md.
- If the acceptance criteria was not met, collect the error messages (build failures or failing tests) and report them.

## Release notes
- Each PR must have release notes attached. Those are saved in the `docs` folder, split by version and product aspect. Follow the existing notes to figure out the right format.
- Follow the docs/release-notes structure and writing style. The guidelines are in docs/release-notes/About.md.

## Coding standards

### Language and Environment
- Use modern F# with functional-first approach
- Target .NET Standard 2.0 for compatibility
- Avoid external dependencies - the codebase is self-contained

### Code Style and Standards
- Follow docs/coding-standards.md, especially:
  - Prefer immutability and pure functions
  - Use consistent naming (see abbreviation guide)
  - Process complex types recursively with pattern matching
  - Avoid single-character identifiers except in established patterns
- This project uses .fsi signature files. When adding a new public API to a namespace/module which will be consumed from other file, add it to the respective .fsi signature file as well
- Follow docs/overview.md for key compiler data formats and representations

### Type System Handling
- When working with `TType` instances, use appropriate stripping functions:
  - `stripTyparEqns` - Removes inference equations from type parameters
  - `stripTyEqns` - Removes type equations and type abbreviations
  - `stripTyEqnsAndErase` - Also erases compilation representations
  - Match the appropriate stripper to the context (e.g., display vs compilation)
  - Check equality with `typeEquiv` after stripping rather than direct comparison

### Core Data Structures
- Use F# discriminated unions for type representations
- Respect representation hiding through access control
- Use `remapX` functions when transforming types across boundaries
- Be aware of performance implications for recursive type operations

### Documentation and Naming
- Document public APIs with XML comments
- Use descriptive function names that indicate transformation direction
- Follow established naming conventions for error messages based on FSComp.txt file. Put all error messages into the FSComp.txt file to ensure localisation
