---
title: Diagnostics
category: Compiler
categoryindex: 1
index: 3
---

## Adding Error Messages

Adding or adjusting errors emitted by the compiler is usually straightforward (though it can sometimes imply deeper compiler work). Here's the general process:

1. Reproduce the compiler error or warning with the latest F# compiler built from the [F# compiler repository](https://github.com/dotnet/fsharp).
2. Find the error code (such as `FS0020`) in the message.
3. Use a search tool and search for a part of the message. You should find it in `FSComp.fs` with a title, such as `parsMissingTypeArgs`.
4. Use another search tool or a tool like Find All References / Find Usages to see where it's used in the compiler source code.
5. Set a breakpoint at the location in source you found. If you debug the compiler with the same steps, it should trigger the breakpoint you set. This verifies that the location you found is the one that emits the error or warning you want to improve.

From here, you can either simply update the error test, or you can use some of the information at the point in the source code you identified to see if there is more information to include in the error message. For example, if the error message doesn't contain information about the identifier the user is using incorrectly, you may be able to include the name of the identifier based on data the compiler has available at that stage of compilation.

If you're including data from user code in an error message, it's important to also write a test that verifies the exact error message for a given string of F# code.

## Formatting User Text from Typed Tree items

When formatting Typed Tree objects such as `TyconRef`s as text, you normally use either

* The functions in the `NicePrint` module such as `NicePrint.outputTyconRef`. These take a `DisplayEnv` that records the context in which a type was referenced, for example, the open namespaces. Opened namespaces are not shown in the displayed output.

* The `DisplayName` properties on the relevant object. This drops the `'n` text that .NET adds to the compiled name of a type, and uses the F#-facing name for a type rather than the compiled name for a type (for example, the name given in a `CompiledName` attribute).

* The functions such as `Tastops.fullTextOfTyconRef`, used to show the full, qualified name of an item.

When formatting "info" objects, see the functions in the `NicePrint` module.

## Notes on displaying types

When displaying a type, you will normally want to "prettify" the type first. This converts any remaining type inference variables to new, better user-friendly type variables with names like `'a`. Various functions prettify types prior to display, for example, `NicePrint.layoutPrettifiedTypes` and others.

When displaying multiple types in a comparative way, for example, two types that didn't match, you will want to display the minimal amount of infomation to convey the fact that the two types are different, for example, `NicePrint.minimalStringsOfTwoTypes`.

When displaying a type, you have the option of displaying the constraints implied by any type variables mentioned in the types, appended as `when ...`. For example, `NicePrint.layoutPrettifiedTypeAndConstraints`.

