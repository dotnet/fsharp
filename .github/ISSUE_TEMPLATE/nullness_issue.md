---
name: Nullness-related report
about: Create a report related to nullable reference types handling
title: ''
labels:  [Bug, Needs-Triage,Area-Nullness]
assignees: 'T-Gro'

---


Please check at [Nullable Reference Types RFC](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1060-nullable-reference-types.md) if this issue isn't a known limitation, such as missing flow-control analysis after branching constructs like `if` or `while`.
Please provide a succinct description of the issue and choose 1 or more from the following categories of impact

- [ ]  Unexpected nullness warning (false positive in nullness checking, code uses --checknulls and langversion:preview)
- [ ]  Missing nullness warning in a case which can produce nulls (false negative, code uses --checknulls and langversion:preview)
- [ ]  Breaking change related to older `null` constructs in code not using the checknulls switch
- [ ]  Breaking change related to generic code and explicit type constraints (`null`, `not null`)
- [ ]  Type inference issue (i.e. code worked without type annotations before, and applying the --checknulls enforces type annotatins)
- [ ]  C#/F# interop issue related to nullness metadata
- [ ]  None of the categories above apply

**Reproducible code snippet**

Provide a small code snippet demonstrating the issue.
If referenced code is needed for the repro and cannot be shared (e.g. a private C# nuget package), try to share at least the metadata annotations from the called type+member as seen in `ilspy.exe`. C# compiler produces attributes like `[Nullable]` and `[NullableContext]`, which is what F# compiler tries to load and interpret in C#/F# interop scenarios.


**Expected behavior**
(if not clear from category selection above)

**Actual behavior**
(if not clear from category selection above)

**Known workarounds**

Are there any language constructs (typically pattern matching, library constructs, Null/NonNull active patterns, explicit type annotations) allowing the same logic be expressed differently in order to mitigate the experienced nullness issue?

**Related information**

Provide any related information (optional):

* Operating system
* .NET Runtime kind (.NET Core, .NET Framework, Mono)
* Editing Tools (e.g. Visual Studio Version, Visual Studio)
