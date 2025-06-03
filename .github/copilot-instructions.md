# GitHub Copilot Instructions for F# Compiler

## Build and test steps (Linux)

Always build and test the project before submitting your solution.

1. As an initial smoke test for a quick build, run `dotnet build ./FSharp.Compiler.Service.sln --verbosity quiet --no-restore` from the repository root. 
Only if this succeeds, move on.
2. From the repository root, run `./build.sh -c Release --norestore`. This creates a prototype compiler, and then the proto compiler builds itself again.
3. Only if two previous steps suceed, run automated tests with `./build.sh -c Release --testcoreclr --norestore`.


If the build fails with errors or non-zero exit code, fix it based on the error messages given and repeat the build step.

If either of the steps 1/2/3 fails, fix the errors and repeat from step 1 - up to 3 times.
After that, report all relevant build errors, error messages and specific details about failing tests and their test test failure details.

### Fixing tests

- If any of the tests fail: Check if the test, test expectation (either inline in the test or a reference file configured for the test) or implementation needs updating, and fix it
- If you see test failures for Surface area baselines: Refer to "Updating FCS surface area baselines" in devguide.md
- If you see test failures for IL baselines: Refer to "Updating ILVerify baselines" in devguide.md


## Acceptance criteria

- Code is formatted using `dotnet fantomas .` executed at the repo root.
- Builds without errors.
- Runs tests without errors. If some tests needed adjustments, those test expectations/baseline adjustments were done.
- If the acceptance criteria was not met, collect the error messages (build failures or failing tests) and report them.

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
