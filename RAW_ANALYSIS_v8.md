# LangVersion 8.0 Usage Analysis

## Semantic Categories

| Icon | Category | Meaning |
|------|----------|---------|
| ✅ | **PASS** | Test expects code to **COMPILE & RUN** successfully - feature IS available in v8 |
| ❌ | **FAIL** | Test expects **COMPILER ERROR** - testing error messages, type errors, invalid code |
| ⚠️ | **WARN** | Test expects **COMPILER WARNING** - testing warning messages |
| ❓ | **UNKNOWN** | Could not determine from context (needs manual review) |

---

## Overall Summary

| Category | Count | % | Meaning |
|----------|-------|---|---------|
| ✅ PASS | 241 | 44.2% | Feature works, code compiles/runs |
| ❌ FAIL | 212 | 38.9% | Error expected (type errors, invalid code) |
| ❓ UNKNOWN | 82 | 15.0% | Unclear from context |
| ⚠️ WARN | 10 | 1.8% | Warning expected |
| **TOTAL** | **545** | 100% | |

---

## By Test Project

### 📦 FSharp.Compiler.ComponentTests (361 occurrences)

| Subfolder | ✅ PASS | ❌ FAIL | ⚠️ WARN | ❓ UNK | Total |
|-----------|---------|---------|---------|--------|-------|
| Language | 68 | 34 | 0 | 16 | 118 |
| Conformance | 46 | 28 | 1 | 6 | 81 |
| ErrorMessages | 25 | 46 | 0 | 0 | 71 |
| Interop | 16 | 11 | 0 | 1 | 28 |
| EmittedIL | 16 | 4 | 1 | 2 | 23 |
| Miscellaneous | 2 | 3 | 0 | 13 | 18 |
| ConstraintSolver | 6 | 1 | 0 | 0 | 7 |
| Diagnostics | 2 | 4 | 0 | 0 | 6 |
| CompilerOptions | 5 | 0 | 0 | 0 | 5 |
| CompilerDirectives | 0 | 2 | 0 | 0 | 2 |
| Signatures | 1 | 0 | 1 | 0 | 2 |
| **SUBTOTAL** | **187** | **133** | **3** | **38** | **361** |

---

### 📦 fsharp (FSharpSuite.Tests) (174 occurrences)

| Subfolder | ✅ PASS | ❌ FAIL | ⚠️ WARN | ❓ UNK | Total |
|-----------|---------|---------|---------|--------|-------|
| Compiler/Language | 41 | 76 | 7 | 35 | 159 |
| Compiler/Regressions | 5 | 0 | 0 | 0 | 5 |
| Compiler/Conformance | 4 | 1 | 0 | 1 | 6 |
| Compiler/Libraries | 2 | 0 | 0 | 0 | 2 |
| Compiler/Service | 0 | 0 | 0 | 1 | 1 |
| tests.fs | 0 | 0 | 0 | 1 | 1 |
| **SUBTOTAL** | **52** | **77** | **7** | **38** | **174** |

---

### 📦 Other Projects (10 occurrences)

| Project | ✅ PASS | ❌ FAIL | ⚠️ WARN | ❓ UNK | Total |
|---------|---------|---------|---------|--------|-------|
| FSharp.Compiler.Service.Tests | 2 | 0 | 0 | 4 | 6 |
| FSharp.Test.Utilities | 0 | 2 | 0 | 1 | 3 |
| FSharp.Compiler.Private.Scripting | 0 | 0 | 0 | 1 | 1 |
| **SUBTOTAL** | **2** | **2** | **0** | **6** | **10** |

---

## By Language Feature (LanguageFeatures.fsi mapping)

| File | Language Feature | ✅ PASS | ❌ FAIL | ⚠️ WARN | ❓ UNK | Total | Test Intent |
|------|-----------------|---------|---------|---------|--------|-------|-------------|
| OpenTypeDeclarationTests | OpenTypeDeclaration | 30 | 28 | 7 | 9 | 74 | Mixed: feature + errors |
| DefaultInterfaceMemberTests | DefaultInterfaceMemberConsumption | 0 | 33 | 0 | 23 | 56 | Error validation |
| TailCallAttribute | WarningWhenTailCallAttrOnNonRec | 19 | 33 | 0 | 0 | 52 | Mixed: pass & errors |
| IWSAMsAndSRTPsTests | InterfacesWithAbstractStaticMembers | 13 | 14 | 1 | 4 | 32 | Mixed |
| StaticClassTests | ErrorReportingOnStaticClasses | 14 | 12 | 0 | 0 | 26 | Mixed |
| DotLambdaTests | AccessorFunctionShorthand | 15 | 9 | 0 | 0 | 24 | Mostly works |
| ExtensionMethodTests | CSharpExtensionAttributeNotRequired | 4 | 0 | 0 | 14 | 18 | Feature works |
| StringInterpolation | StringInterpolation | 1 | 15 | 0 | 0 | 16 | Error validation |
| CopyAndUpdateTests | NestedCopyAndUpdate | 11 | 4 | 0 | 1 | 16 | Mostly works |
| RequiredAndInitOnlyProperties | RequiredPropertiesSupport | 8 | 7 | 0 | 0 | 15 | Mixed |
| Literals | ArithmeticInLiterals | 10 | 2 | 0 | 0 | 12 | Feature works |
| ClassesTests | ErrorForNonVirtualMembersOverrides | 5 | 6 | 0 | 0 | 11 | Mixed |
| StaticsInInterfaces | StaticMembersInInterfaces | 8 | 0 | 0 | 1 | 9 | Feature works |
| StaticLetInUnionsAndRecords | StaticLetInRecordsDusEmptyTypes | 8 | 0 | 0 | 1 | 9 | Feature works |
| OffsideExceptions | RelaxWhitespace2 | 9 | 0 | 0 | 0 | 9 | Feature works |
| ComputationExpressions | AndBang/OverloadsForCustomOperations | 4 | 4 | 0 | 1 | 9 | Mixed |
| FixedIndexSliceTests | FixedIndexSlice3d4d | 8 | 0 | 0 | 0 | 8 | Feature works |
| UnionCasePatternMatchingErrors | MatchNotAllowedForUnionCaseWithNoData | 0 | 7 | 0 | 0 | 7 | Error validation |
| ObjInference | DiagnosticForObjInference | 6 | 1 | 0 | 0 | 7 | Feature works |
| InterpolatedStringsTests | ExtendedStringInterpolation | 6 | 1 | 0 | 0 | 7 | Feature works |
| SequenceExpressionTests | TryWithInSeqExpression | 2 | 3 | 0 | 1 | 6 | Mixed |
| Records | WarningWhenCopyAndUpdateRecordChangesAllFields | 2 | 4 | 0 | 0 | 6 | Error validation |
| UnionStructTypes | ReuseSameFieldsInStructUnions | 0 | 5 | 0 | 0 | 5 | Error validation |
| AttributeUsage | EnforceAttributeTargets | 5 | 0 | 0 | 0 | 5 | Feature works |
| ArgumentNames | ImprovedImpliedArgumentNames | 5 | 0 | 0 | 0 | 5 | Feature works |
| WhileBangTests | WhileBang | 4 | 0 | 0 | 0 | 4 | Feature works |
| NullableOptionalRegressionTests | NullableOptionalInterop | 4 | 0 | 0 | 0 | 4 | Feature works |
| BasicConstants | ArithmeticInLiterals/PrintfBinaryFormat | 4 | 0 | 0 | 0 | 4 | Feature works |
| ConstraintIntersectionTests | ConstraintIntersectionOnFlexibleTypes | 3 | 1 | 0 | 0 | 4 | Mostly works |
| langversion | (Meta: langversion options) | 4 | 0 | 0 | 0 | 4 | Infrastructure |
| VisibilityTests | (Interop) | 0 | 4 | 0 | 0 | 4 | Error validation |

---

## Executive Summary

```
╔══════════════════════════════════════════════════════════════════════════════════════════════════════╗
║                         LANGVERSION 8.0 TEST USAGE - SEMANTIC ANALYSIS                               ║
╠══════════════════════════════════════════════════════════════════════════════════════════════════════╣
║ TOTAL OCCURRENCES: 545                                                                               ║
╠══════════════════════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                                      ║
║  WHAT ARE THESE TESTS DOING?                                                                         ║
║  ┌──────────────────────────────────────────────────────────────────┬─────────┬──────────┐           ║
║  │ Test Intent                                                      │ Count   │ %        │           ║
║  ├──────────────────────────────────────────────────────────────────┼─────────┼──────────┤           ║
║  │ ✅ PASS - Feature WORKS, code compiles & runs                    │ 241     │ 44.2%    │           ║
║  │ ❌ FAIL - Error EXPECTED (type errors, invalid syntax)           │ 212     │ 38.9%    │           ║
║  │ ❓ UNKNOWN - Unclear from static analysis                        │ 82      │ 15.0%    │           ║
║  │ ⚠️ WARN - Warning EXPECTED                                        │ 10      │ 1.8%     │           ║
║  └──────────────────────────────────────────────────────────────────┴─────────┴──────────┘           ║
║                                                                                                      ║
║  BY TEST PROJECT                                                                                     ║
║  ┌──────────────────────────────────────────────────┬─────────┬─────────┬─────────┬─────────┐        ║
║  │ Project                                          │ ✅ PASS │ ❌ FAIL │ ⚠️ WARN │ ❓ UNK  │        ║
║  ├──────────────────────────────────────────────────┼─────────┼─────────┼─────────┼─────────┤        ║
║  │ 📦 FSharp.Compiler.ComponentTests                │ 187     │ 133     │ 3       │ 38      │        ║
║  │ 📦 fsharp (FSharpSuite.Tests)                    │ 52      │ 77      │ 7       │ 38      │        ║
║  │ 📦 Other (Service, Utilities, Scripting)         │ 2       │ 2       │ 0       │ 6       │        ║
║  └──────────────────────────────────────────────────┴─────────┴─────────┴─────────┴─────────┘        ║
║                                                                                                      ║
║  TOP FEATURES - "IT WORKS" TESTS (✅ PASS)                                                           ║
║  ┌────────────────────────────────────────────────┬─────────┐                                        ║
║  │ Feature                                        │ ✅ PASS │                                        ║
║  ├────────────────────────────────────────────────┼─────────┤                                        ║
║  │ OpenTypeDeclaration                            │ 30      │                                        ║
║  │ TailCallAttribute                              │ 19      │                                        ║
║  │ DotLambdaTests (AccessorFunctionShorthand)     │ 15      │                                        ║
║  │ StaticClassTests                               │ 14      │                                        ║
║  │ IWSAMsAndSRTPsTests                            │ 13      │                                        ║
║  │ CopyAndUpdateTests (NestedCopyAndUpdate)       │ 11      │                                        ║
║  │ Literals (ArithmeticInLiterals)                │ 10      │                                        ║
║  │ OffsideExceptions (RelaxWhitespace2)           │ 9       │                                        ║
║  │ StaticsInInterfaces                            │ 8       │                                        ║
║  │ StaticLetInUnionsAndRecords                    │ 8       │                                        ║
║  │ FixedIndexSliceTests (FixedIndexSlice3d4d)     │ 8       │                                        ║
║  │ RequiredAndInitOnlyProperties                  │ 8       │                                        ║
║  └────────────────────────────────────────────────┴─────────┘                                        ║
║                                                                                                      ║
║  TOP FEATURES - "ERROR EXPECTED" TESTS (❌ FAIL)                                                     ║
║  ┌────────────────────────────────────────────────┬─────────┐                                        ║
║  │ Feature                                        │ ❌ FAIL │                                        ║
║  ├────────────────────────────────────────────────┼─────────┤                                        ║
║  │ TailCallAttribute                              │ 33      │                                        ║
║  │ DefaultInterfaceMemberTests                    │ 33      │                                        ║
║  │ OpenTypeDeclarationTests                       │ 28      │                                        ║
║  │ StringInterpolation                            │ 15      │                                        ║
║  │ IWSAMsAndSRTPsTests                            │ 14      │                                        ║
║  │ StaticClassTests                               │ 12      │                                        ║
║  │ DotLambdaTests                                 │ 9       │                                        ║
║  │ RequiredAndInitOnlyProperties                  │ 7       │                                        ║
║  │ UnionCasePatternMatchingErrors                 │ 7       │                                        ║
║  │ ClassesTests                                   │ 6       │                                        ║
║  └────────────────────────────────────────────────┴─────────┘                                        ║
║                                                                                                      ║
╠══════════════════════════════════════════════════════════════════════════════════════════════════════╣
║ INTERPRETATION:                                                                                      ║
║                                                                                                      ║
║ • 44% of v8 usages test that NEW FEATURES WORK correctly in F# 8.0                                   ║
║ • 39% of v8 usages test that ERRORS ARE PROPERLY REPORTED for invalid code                           ║
║ • 2% test WARNINGS are properly emitted                                                              ║
║ • 15% need manual review (complex test patterns)                                                     ║
║                                                                                                      ║
║ This is a HEALTHY test distribution - testing both positive and negative cases.                     ║
╚══════════════════════════════════════════════════════════════════════════════════════════════════════╝
```

---

## Notes on Categories

### ✅ PASS Tests
These tests use `--langversion:8.0` and expect the code to **compile and/or run successfully**.
This means: "F# 8.0 supports this feature, verify it works."

Examples:
- `FixedIndexSliceTests` - 3D/4D array slicing works in v8
- `OpenTypeDeclarationTests` - `open type System.Math` compiles
- `StaticsInInterfaces` - static members in interfaces work

### ❌ FAIL Tests  
These tests use `--langversion:8.0` and expect a **compiler error**.
This means: "Even in F# 8.0, this code is invalid - verify proper error message."

Examples:
- `TailCallAttribute` - `[<TailCall>]` on non-recursive function → error
- `StringInterpolation` - Invalid interpolation syntax → error
- `UnionCasePatternMatchingErrors` - Pattern match invalid cases → error

### ⚠️ WARN Tests
These tests expect a **compiler warning** to be emitted.

### ❓ UNKNOWN
Static analysis couldn't determine intent - these use patterns like:
- Custom assertion helpers
- Complex test frameworks  
- Non-standard patterns
