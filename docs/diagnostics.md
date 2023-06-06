---
title: Diagnostics
category: Compiler Internals
categoryindex: 200
index: 300
---
# Diagnostics

The key types are:

* `ErrorLogger`
* `FSharpDiagnosticSeverity`
* `FSharpDiagnostic`
* `DiagnosticWithText`

and functions

* `warning` - emit a warning
* `errorR` - emit an error and continue
* `error` - emit an error and throw an exception
* `errorRecovery` - recover from an exception

For the compiler, a key file is `https://github.com/dotnet/fsharp/blob/main/src/Compiler/FSComp.txt` holding most of the messages. There are also a few other similar files including some old error messages in `FSStrings.resx`.

## Adding Diagnostics

Adding or adjusting diagnostics emitted by the compiler is usually straightforward (though it can sometimes imply deeper compiler work). Here's the general process:

1. Reproduce the compiler error or warning with the latest F# compiler built from the [F# compiler repository](https://github.com/dotnet/fsharp).
2. Find the error code (such as `FS0020`) in the message.
3. Use a search tool and search for a part of the message. You should find it in `FSComp.fs` with a title, such as `parsMissingTypeArgs`.
4. Use another search tool or a tool like Find All References / Find Usages to see where it's used in the compiler source code.
5. Set a breakpoint at the location in source you found. If you debug the compiler with the same steps, it should trigger the breakpoint you set. This verifies that the location you found is the one that emits the error or warning you want to improve.

From here, you can either simply update the error text, or you can use some of the information at the point in the source code you identified to see if there is more information to include in the error message. For example, if the error message doesn't contain information about the identifier the user is using incorrectly, you may be able to include the name of the identifier based on data the compiler has available at that stage of compilation.

If you're including data from user code in an error message, it's important to also write a test that verifies the exact error message for a given string of F# code.

Note that the .NET SDK generally follows the policy that new diagnostics are introduced only alongside a language version increment, and F# now tries to follow the same policy (though historically this has not always happened).
This is true even if your diagnostic is off by default.
So if you create a new diagnostic by adding it to [FsComp.txt](https://github.com/dotnet/fsharp/blob/main/src/Compiler/FSComp.txt), you should add a new language feature at the same time.
Concretely, consider following this procedure:

1. Create a pull request that reserves an error message. (That way, you can avoid other people trampling over your change to FSComp.txt while you're implementing the actual feature.) [Here's a historical example.](https://github.com/dotnet/fsharp/pull/14642)
1. In the same pull request, reserve the new feature. [Here's a historical example](https://github.com/dotnet/fsharp/pull/15315/) (although it would have been better practice to combine this into the previous pull request, and also this pull request [should not have registered the feature](https://github.com/dotnet/fsharp/pull/15315/files#r1220330247) as belonging to a particular language version).
1. Now you can go ahead and implement the diagnostic. This is the point at which you would register the language feature in some particular language version (likely the preview language version). It is often best practice to introduce it at a lower level than you ultimately intend (e.g. informational rather than warning, or warning rather than error), to permit a more gradual rollout. See the section on "[Enabling a warning or error by default](#enabling-a-warning-or-error-by-default)" for more information.
1. When the next language version comes along, consider creating another language feature which increases the default diagnostic level, remembering not to change the existing level for people who are pinned to the older language version. [Here's an example](https://github.com/dotnet/fsharp/blob/2133c3404186f3ddcafa10c943e9451358412ab3/src/Compiler/Checking/CheckPatterns.fs#L570).

## Formatting Typed Tree items in Diagnostics

Diagnostics must often format TAST items as user text. When formatting these, you normally use either

* The functions in the `NicePrint` module such as `NicePrint.outputTyconRef`. These take a `DisplayEnv` that records the context in which a type was referenced, for example, the open namespaces. Opened namespaces are not shown in the displayed output.

* The `DisplayName` properties on the relevant object. This drops the `'n` text that .NET adds to the compiled name of a type, and uses the F#-facing name for a type rather than the compiled name for a type (for example, the name given in a `CompiledName` attribute).

When formatting "info" objects, see the functions in the `NicePrint` module.

## Notes on displaying types

Diagnostics must often format types.

* When displaying a type, you will normally want to "prettify" the type first. This converts any remaining type inference variables to new, better user-friendly type variables with names like `'a`. Various functions prettify types prior to display, for example, `NicePrint.layoutPrettifiedTypes` and others.

* When displaying multiple types in a comparative way, for example, two types that didn't match, you will want to display the minimal amount of infomation to convey the fact that the two types are different, for example, `NicePrint.minimalStringsOfTwoTypes`.

* When displaying a type, you have the option of displaying the constraints implied by any type variables mentioned in the types, appended as `when ...`. For example, `NicePrint.layoutPrettifiedTypeAndConstraints`.

## Localization

The file `FSComp.txt` contains the canonical listing of diagnostic messages, but there are also `.xlf` localization files for various languages.
See [the DEVGUIDE](../DEVGUIDE.md#updating-fscompfs-fscompresx-and-xlf) for more details.

## Enabling a warning or error by default

The file `CompilerDiagnostics.fs` contains the function `IsWarningOrInfoEnabled`, which determines whether a given diagnostic is emitted.
If you have created a language feature to accompany the diagnostic, and you intend the diagnostic to be on by default in any language version supporting that language feature, you should use `DiagnosticEnabledWithLanguageFeature` to control the diagnostic.

For example:

```fsharp
// Several contexts give you access to the current language version.
let supported = g.langVersion.SupportsFeature LanguageFeature.MyLanguageFeature
informationalWarning(DiagnosticEnabledWithLanguageFeature(FSComp.SR.typrelNeverRefinedAwayFromTop(), m, supported))`
```
