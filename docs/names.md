---
title: Display names, logical names and compiled names
category: Compiler Internals
categoryindex: 200
index: 350
---
# Names of entities and values in the F# Compiler

The F# tooling distinguishes between the following concepts of "name" for values, union cases, class/record fields and entities:

* Display names as they appear in code

  Characteristics:
  - For most identifiers, have double backticks, e.g. ` ``Module name with spaces`` `
  - For operator names, are short and parenthesized, e.g. `(+)` for the logical name `op_Addition`
  - For active patterns, are parenthesized, e.g. `(|A|_|)`
  - etc., see exact specification below

  Used in:
  - Code outputs, e.g. signature files
  - Diagnostics (BUG: not consistently the case today)

  Current aliases in code:
  - `vref.DisplayName`
  - `entity.DisplayName`
  - `entity.DisplayNameWithStaticParameters`
  - `entity.DisplayNameWithStaticParametersAndUnderscoreTypars`
  - `minfo.DisplayName`
  - `pinfo.DisplayName`
  - `einfo.DisplayName`
  - etc.

* Display names as they appear in declaration lists, navigation etc.

  Characteristics:
  - Same as above without the double backticks or parentheses

  Current aliases in code:
  - `vref.DisplayNameCore`
  - `entity.DisplayNameCore`
  - `minfo.DisplayNameCore`
  - `pinfo.DisplayNameCore`
  - `einfo.DisplayNameCore`
  - etc.

* Logical names

  Characteristics:
  - Are used in `TypedTree`, often "canonical"
  - Sometimes require extra flags to qualify the meaning of the name

  Current aliases in code:
  - `vref.LogicalName`
  - `entity.LogicalName`
  - `minfo.LogicalName`
  - `pinfo.PropertyName`
  - `einfo.EventName`
  - etc.

* Compiled names

  Characterists: 
  - Mark the names that appear in the .NET IL

  Current aliases in code:
  - `vref.CompiledName`
  - `entity.CompiledName`
  - etc.


## Specification of all logical names

The following tables loosely characterise the variations in logical names, how
they correspond to F# source constructs and the `SyntaxTree`/`TypedTree` metadata for these.

Entities:

Display name in code        | Logical name   | Notes 
----------------------------|----------------|-------
C                           | C              | type definition
C                           | C`1            | e.g. generic type, see notes below for variations of display names
M                           | M              | module definition
M                           | MModule        | "ModuleSuffix" attribute for F# modules, now somewhat legacy, rarely used, but still allowed; also where "ModuleSuffix" is implied because type and module have the same name
JsonProvider<"foo.json">    | JsonProvider,Schema=\"xyz\" |   static parameters, see notes below for variations of display names

Values:

 Display name in code | Relation | Logical name         | Notes
 ---------------------|----------|----------------------|------
 (+)                  | <-->     | op_Addition          |
 (+ )                 |  -->     | op_Addition          | not reversed
 op_Addition          |  -->     | op_Addition          | not reversed
 (*)                  | <-->     | op_Multiply          | 
 ( * )                |  -->     | op_Multiply          | not reversed
 op_Multiply          |  -->     | op_Multiply          | not reversed
 ( *+ )               | <-->     | op_MultiplyPlus      | 
 ( *+  )              |  -->     | op_MultiplyPlus      | not reversed
 op_MultiplyPlus      |  -->     | op_MultiplyPlus      | not reversed
 (+++)                | <-->     | op_PlusPlusPlus      |
 op_PlusPlusPlus      |  -->     | op_PlusPlusPlus      | not reversed
 (%)                  | <-->     | op_Modulus           |
 op_Modulus           |  -->     | op_Modulus           |
 (?)                  | <-->     | op_Dynamic           | not defined by default, for x?SomeThing
 (?<-)                | <-->     | op_DynamicAssignment | not defined by default, for x?SomeThing <- "a"
 (..)                 | <-->     | op_Range             |       for "3 .. 5"
 (.. ..)              | <-->     | op_RangeStep         |       for "5 .. -1 .. 3"
 or                   | <-->     | or                   |
 mod                  | <-->     | mod                  |
 ``let``              | <-->     | let                  | this is a keyword, in code it appears as ``let``
 ``type``             | <-->     | type                 | this is a keyword, in code it appears as ``type``
 base                 | <-->     | base                 | for IsBaseVal=true only. Base is a keyword, this is a special base val
 ``base``             | <-->     | base                 | for IsBaseVal=false only. Base is a keyword, this is not a special base val
 SomeClass            | <-->     | .ctor                | IsConstructor=true
 ``.ctor``            | <-->     | .ctor                | IsConstructor=false, this is only allowed for let-definitions, e.g. let ``.ctor`` x = 1
 <not-shown>          | <-->     | .cctor               | IsClassConstructor=true, should never really appear in diagnostics or user-facing output
 ``.cctor``           | <-->     | .cctor               | IsClassConstructor=false, this is only allowed for let-definitions, e.g. let ``.cctor`` x = 1
 (\|A\|_\|)           | <-->     | \|A\|_\|             |
 (\|A  \|_  \|)       |  -->     | \|A\|_\|             | not reversed
 P                    | <-->     | get_P                | IsPropertyGetterMethod = true
 P                    | <-->     | set_P                | IsPropertySetterMethod = true

Other Val constructs less problematic for naming are:

Display name in code | Relation | Logical name         | Notes
---------------------|----------|----------------------|------
this                 | <-->     | this                 | IsCtorThisVal = true; From `type C() as this`; Can have any name, not particularly special with regard to names; This has a 'ref' type for initialization checks
this                 | <-->     | this                 | IsMemberThisVal = true; From `member this.M() = ...`; This can have a 'ref' type for initialization checks; Can have any name, not particularly special with regard to names
\<not-shown\>        | <-->     | System.IDisposable.Dispose | ImplementedSlotSigs is non-empty, i.e. length 1, should never really appear in diagnostics or user-facing output

Union cases:

Display name in code | Relation | Logical name         | Notes
---------------------|----------|----------------------|------
SomeCase             | <-->     | SomeCase
` ``Case with space`` `  | <-->     | Case with space
` ``type`` `             | <-->     | type                 | This is a keyword
(::)                 | <-->     | op_ColonColon        | This is the logical name for the cons union case on `FSharpList` only
[]                   | <-->     | op_Nil               | This is the logical name for the nil case on `FSharpList` only

Class and record fields, enum cases, active pattern cases, anonymous record fields:

Display name in code | Relation | Logical name         | Notes
---------------------|----------|----------------------|------
SomeField            | <-->     | SomeField
` ``Field with space`` `| <-->     | Field with space
` ``type`` `            | <-->     | type                 | This is a keyword

Generic parameters:

Display name in code | Relation | Logical name         | Notes
---------------------|----------|----------------------|------
'T                   | <-->     | T
'` ``T T T`` `           | <-->     | T T T                | BUG: the backticks are not currently added
'` ``type`` `            | <-->     | type                 | This is a keyword, BUG: the backticks are not currently added

## Variations on display names

In different display settings, Entities/Types/Modules can have some variations on display names. For example, when showing some kinds of output we may set `shortTypeNames=true` which will never show full names.

* `SomeTypeProvider`
  - Used for omitting static parameters
  - Alias in code: `entity.CompiledName`

* `SomeTypeProvider<...>`   
  - Used for eliding static parameters

* `SomeTypeProvider<"foo.json">`   
  - Used for showing static parameters. These can be very large, e.g. entire connection strings, so better to elide or omit.
  - Alias in code: `entity.DisplayNameWithStaticParameters`

* `List<_>`   
  - Used with underscore typars
  - Alias in code: `entity.DisplayNameWithStaticParametersAndUnderscoreTypars`

* `Dictionary<'TKey,'TResult>`
  - Used with general typars

* Full name

Examples:
  - `SomeNamespace.OtherNamespace.SomeType`
  - ``` ``Some Namespace With Spaces``.SomeType```  <-- BUG: not double-ticks today
  - `SomeEnclosingType<_>.SomeStaticMethod`         <-- BUG: the mangled generic type counts are shown today


## Compiled names

The name that appears in the .NET IL.

Affected by:
- `CompiledName` attribute
- some heuristics for generic type parameters

Also the name from signature is generally preferred; if there is any difference, a warning is emitted.

Example of how signature affects compiled names

```fsharp
Foo.fsi 

  val SomeFunction: x: int -> y: int -> int 

Foo.fs

  let SomeFunction a b = a + b // compiled name of parameters is x, y - warning emitted
```
