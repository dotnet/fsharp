# F# Testing proposal

## Why do we test

* To prevent regressions (behavioral, performance).
* To have a quicker debug feedback (thus, find problems quicker).
* To verify conformance to language spec (API contract testing).
* To have IL verification (both read and write).
* To have a quicker design feedback.
* To document behavior.

## Goals

* Use one standardized testing framework across all of test projects, and get rid of custom old solutions (FSharpQA and Cambridge suites).
* Have tests restructured the way, that they are easy to discover.
* Have tests building and running on all supported platforms (Windows, macOS and Linux) and different frameworks (with exceptions when this is not applicable).
* Make it easy to run tests using standard .NET instruments (dotnet cli, test explorer, etc.).
* Leverage standard .NET testing platform and use all its benefits, suck as live unit testing, code coverage collecting, dead code elimination, etc.

## Framework for testing

The following test frameworks and libraries will be used for new test projects **[xUnit Test Framework](https://xunit.net/), [FluentAssertions](https://fluentassertions.com/) (+ [FsUnit](https://fsprojects.github.io/FsUnit/) and [FsCheck](https://github.com/fscheck/FsCheck) when needed)**. All existing NUnit test suites will be migrated to xUnit.

**Justification:**

* **xUnit** is an extensible, TDD adherent, testing framework, which was successfully adopted by many .NET engineering teams, including Roslyn, AspNetCore, EFcore, etc, has a "cleaner" approach for writing test suites (i.e. class constructor for setup, implementing IDisposable for teardown, as oppose to custom attributes). More info [here](https://xunit.net/docs/comparisons).
* **FluentAssertions** makes it easier to write scoped assertions, provides better error messages.

**Alternatives:** NUnit, MSBuild, Expecto

### Tests categorization

#### New tests should be grouped based on two factors: test type (1) + test category and subcategory (2)

1. **Test type**:
**Determines what type of test is it:**
   * __Functional tests__:
        * __Unit Tests__: a lightweight testing for smaller modules, functions, etc.
          * __Examples__: Testing individual parts/functions of lexer, parser, syntax tree, standard library modules, etc.
          * __Subgroups__: there should be a separation between testing private and public parts of each module (i.e. compiler tests for private and public API should be in separate test projects).
        * __Component Tests__: testing for bigger parts of compiler.
          * __Examples__: Tests for the compiler components as whole, such as Code generation, IL Generation, Compiler optimizations, Type Checker, Type Providers, Conformance, etc.
        * __Integration and End2End Tests__: testing of F# compiler & tooling integration, as well as e2e experiences.
          * __Examples__: VS Integration, .NET Interactive integration, LSP integration. Integration with dotnet CLI, project creation, building, running.
   * __Non-functional tests__:
        * __Load and Stress Tests__: testing for high level modules/components to understand peak performance and potentially catch any performance regressions.
          * __Examples__: measuring compile, build, link times for the compiler, individual functions (i.e. data structures sorting, traversing, etc.).
1. **Test category and subcategory**: Tests (sub)categories shall be determined by the project, library, module, and functionality tests are covering.

#### Examples

* F# compiler component test which is verifying generated IL for computation expression will have category `Compiler` and subcategories `EmittedIL` and `ComputationExpressions`.
* F# compiler service unit test which is testing F# tokenizer, will have category `Compiler.Service` and subcategory `Tokenizer`.

Please, refer to [File and project structure](#File-and-project-structure) for more information on how tests will be organized on the filesystem.

## File and project structure

### Naming schema

The proposed naming schema for test projects is: `FSharp.Category.Subcategory.TestType`, where
`Category.Subcategory` is either a corresponding source project, or a more generic component (e.g. `Compiler`, `Compiler.Private` or more granular `Compiler.CodeGen`, `Compiler.CodeGen.EmittedIL` if category or subcategory project becomes too big, etc.) and `TestType` is the type of the test (one of `UnitTests`, `ComponentTests`, `IntegrationTests`, `LoadTests`).

### Projects organization

Please refer to the "[Naming schema](#Naming-schema)" section above for more information on the projects naming.

New test projects will be grouped by category and test type, all subcategories are just test folders/files in the test project.

* __Examples__: Having test project organized like:
    > `tests/FSharp.Compiler.ComponentTests/CodeGen/EmittedIL/BasicTests.fs`
    > `tests/FSharp.Compiler.ComponentTests/CodeGen/StringEncoding/StringTests.fs`
    > `tests/FSharp.Compiler.ComponentTests/Optimizations/Inlining/InliningTests.fs`

    Will result in one test dll "`FSharp.Compiler.ComponentTests.dll`" which will contain all the subcategories of tests.
* **Notes**:
  * This will result in reduced fragmentation of tests, all the tests files are under one big category, easier to understand what each component/unit test suite covers, less confusion in test classification for new tests.
  * If some categories (or subcategories) will become big enough - they can be factored out to a separate project.

### Test Utilities/Helpers

For all new and migrated tests, any common/helper functionality shall be factored out to a separate project - `FSharp.Test.Utilities`.

## New tests

* All new tests should be created in the new projects only.
* All new tests should contain a brief docstring description of what is being tested, link to an issue if applicable.
* All new tests should be categorized using xUnit's `Trait`, based on their `Category` and `Subcategories`.

## Migrating existing tests

Existing FSharpQA and Cambridge need to be migrated to corresponding test projects: component-style tests to the `FSharp.Compiler.ComponentTests` and unittest-style tests - `FSharp.Compiler.UnitTests`, `FSharp.Compiler.Private.Scripting.UnitTests`, `FSharp.Build.UnitTests`, etc.

## Next steps

* [**In Progress**] Migrate existing `NUnit` tests to xUnit.
* Clean up CompilerAssert.
* Make PEVerify tests work in netcore/non-windows environment.
* Start migration of existing (namely, FSharpQA and Cambridge) suites to xUnit-based projects.

## Open questions:

* As far as I know, [FSharp.Compiler.Service](https://github.com/fsharp/FSharp.Compiler.Service) is dependant on some of the F# compiler tests. Does it have to be changed as well?

## Other

Related issues: (https://github.com/dotnet/fsharp/issues/7075)

You can find this document under 'tests/README.md'.
