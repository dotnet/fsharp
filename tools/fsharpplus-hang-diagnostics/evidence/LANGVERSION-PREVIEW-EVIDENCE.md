# LangVersion=preview Diagnostic Evidence

## Test Configuration

| Property | Value |
|----------|-------|
| **LangVersion** | preview |
| **Compiler** | Local build from main branch |
| **Compiler Path** | `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll` |
| **Test Time** | 2025-11-26T08:35:00 UTC |
| **Timeout** | 90s |
| **Dump Captured At** | 65s |
| **Dump Size** | 851 MB |

## Result

❌ **HUNG** - Build timed out after 90 seconds (same behavior as LangVersion 8, 9, 10)

## Thread Analysis

**Total CLR threads:** 11
**Threads with FSharp frames:** 2

## All Threads with FSharp Frames

### Thread 12 (90 frames)

```
  1: [native]
  2: [native]
  3: System.RuntimeTypeHandle.<InternalAllocNoChecks>g__InternalAllocNoChecksWorker|37_0
  4: Microsoft.FSharp.Core.LanguagePrimitives+HashCompare.GenericHashArbArray
  5: Internal.Utilities.TypeHashing+StructuralUtilities+TypeStructure.GetHashCode
  6: FSharp.Compiler.TypeRelations.TypeFeasiblySubsumesType
  7: FSharp.Compiler.ConstraintSolver+compareTypes@3658-1.Invoke
  8: FSharp.Compiler.ConstraintSolver+compareCond@3654-1<System.__Canon>.Invoke
  9: [native]
 10: [native]
 11: FSharp.Compiler.ConstraintSolver+bestMethods@3819-1.Invoke
 12: FSharp.Compiler.ConstraintSolver+bestMethods@3818.Invoke
 13: FSharp.Compiler.ConstraintSolver.GetMostApplicableOverload
 14: FSharp.Compiler.ConstraintSolver.ResolveOverloading
 15: [native]
 16: FSharp.Compiler.ConstraintSolver.expr2@717-3
 17: FSharp.Compiler.ConstraintSolver.expr2@717-2
 18: FSharp.Compiler.ConstraintSolver.SolveMemberConstraint
 19: FSharp.Compiler.ConstraintSolver.SolveRelevantMemberConstraintsForTypar
 20: FSharp.Compiler.ConstraintSolver+SolveRelevantMemberConstraints@2312.Invoke
 21: FSharp.Compiler.DiagnosticsLogger.RepeatWhileD
 22: [native]
 23: FSharp.Compiler.ConstraintSolver.CanonicalizePartialInferenceProblem
 24: FSharp.Compiler.CheckExpressions.TcLetBinding
 25: FSharp.Compiler.CheckExpressions.TcLinearExprs
 26: [native]
 27: System.Runtime.CompilerServices.RuntimeHelpers.DispatchTailCalls(IntPtr, Void 
 28: FSharp.Compiler.CheckExpressions.TcExprNoRecover
 29: FSharp.Compiler.CheckExpressions+TcExpr@5392.Invoke
 30: [native]
 31: FSharp.Compiler.CheckExpressions.TcExprTypeAnnotated
 32: FSharp.Compiler.CheckExpressions.TcExprUndelayed
 33: [native]
 34: System.Runtime.CompilerServices.RuntimeHelpers.DispatchTailCalls(IntPtr, Void 
 35: FSharp.Compiler.CheckExpressions.TcExprNoRecover
 36: FSharp.Compiler.CheckExpressions+TcExpr@5392.Invoke
 37: FSharp.Compiler.CheckExpressions.TcIteratedLambdas
 38: FSharp.Compiler.CheckExpressions.TcIteratedLambdas
 39: FSharp.Compiler.CheckExpressions.TcExprUndelayed
 40: [native]
 41: System.Runtime.CompilerServices.RuntimeHelpers.DispatchTailCalls(IntPtr, Void 
 42: FSharp.Compiler.CheckExpressions.TcExprNoRecover
 43: FSharp.Compiler.CheckExpressions+TcExpr@5392.Invoke
 44: [native]
 45: FSharp.Compiler.CheckExpressions.TcNormalizedBinding
 46: FSharp.Compiler.CheckDeclarations+MutRecBindingChecking+TcMutRecBindings_Phase2B_TypeCheckAndIncrementalGeneralization@1330-2.Invoke
 47: [native]
 48: FSharp.Compiler.CheckDeclarations+MutRecBindingChecking.TcMutRecBindings_Phase2B_TypeCheckAndIncrementalGeneralization$cont@1310
 49: FSharp.Compiler.CheckDeclarations+MutRecBindingChecking+TcMutRecBindings_Phase2B_TypeCheckAndIncrementalGeneralization@1282.Invoke
 50: FSharp.Compiler.CheckDeclarations+MutRecShapes+mapFoldWithEnv@148<System.__Canon, System.__Canon, System.__Canon, System.__Canon, System.__Canon, System.__Canon, System.__Canon>.Invoke
 51: [native]
 52: FSharp.Compiler.CheckDeclarations+MutRecBindingChecking.TcMutRecDefns_Phase2_Bindings
 53: FSharp.Compiler.CheckDeclarations.TcMutRecDefns_Phase2
 54: FSharp.Compiler.CheckDeclarations+TcDeclarations.TcMutRecDefinitions
 55: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElementNonMutRec@5258-1.Invoke
 56: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElementNonMutRec@5258-16.Invoke
 57: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987-1<System.__Canon>.Invoke
 58: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987<System.__Canon>.Invoke
 59: FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec
 60: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5540-5.Invoke
 61: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5519-8.Invoke
 62: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987-1<System.__Canon>.Invoke
 63: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987<System.__Canon>.Invoke
 64: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElementNonMutRec@5453-15.Invoke
 65: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElementNonMutRec@5258-16.Invoke
 66: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987-1<System.__Canon>.Invoke
 67: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987<System.__Canon>.Invoke
 68: FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec
 69: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5540-5.Invoke
 70: FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5519-8.Invoke
 71: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987-1<System.__Canon>.Invoke
 72: FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987<System.__Canon>.Invoke
 73: FSharp.Compiler.CheckDeclarations+CheckOneImplFile@5788-2.Invoke
 74: FSharp.Compiler.CheckDeclarations+CheckOneImplFile@5766-1.Invoke
 75: FSharp.Compiler.ParseAndCheckInputs+CheckOneInputWithCallback@1528-12.Invoke
 76: FSharp.Compiler.ParseAndCheckInputs+CheckOneInputWithCallback@1450-14.Invoke
 77: FSharp.Compiler.ParseAndCheckInputs+processFile@1824-2.Invoke
 78: [native]
 79: FSharp.Compiler.ParseAndCheckInputs.processFile@1816
 80: FSharp.Compiler.ParseAndCheckInputs+processFile@1859-4.Invoke
 81: FSharp.Compiler.ParseAndCheckInputs+TypeCheckingGraphProcessing+workWrapper@1673.Invoke
 82: FSharp.Compiler.GraphChecking.GraphProcessing+queueNode@120-2<System.__Canon, System.__Canon>.Invoke
 83: [native]
 84: Microsoft.FSharp.Control.Trampoline.Execute
 85: <StartupCode$FSharp-Core>.$Async+clo@193-1.Invoke
 86: System.Threading.QueueUserWorkItemCallback.Execute
 87: System.Threading.ThreadPoolWorkQueue.Dispatch
 88: System.Threading.PortableThreadPool+WorkerThread.WorkerThreadStart
 89: System.Threading.Thread.StartCallback
 90: [native]
```

### Thread 1 (13 frames)

```
  1: [native]
  2: [native]
  3: System.Threading.WaitHandle.WaitOneNoCheck
  4: [native]
  5: FSharp.Compiler.ParseAndCheckInputs+TypeCheckingGraphProcessing.processTypeCheckingGraph
  6: FSharp.Compiler.ParseAndCheckInputs+CheckMultipleInputsUsingGraphMode@1849-1.Invoke
  7: [native]
  8: [native]
  9: FSharp.Compiler.ParseAndCheckInputs.CheckClosedInputSet
 10: FSharp.Compiler.Driver.TypeCheck
 11: FSharp.Compiler.Driver.main1
 12: FSharp.Compiler.Driver.CompileFromCommandLineArguments
 13: FSharp.Compiler.CommandLineMain.main
```

## Repeating Frame Patterns (Main Compiler Thread)

| Frame | Count |
|-------|-------|
| `[native]` | 16 |
| `FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987-1<System.__Canon>.Invoke` | 4 |
| `FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987<System.__Canon>.Invoke` | 4 |
| `System.Runtime.CompilerServices.RuntimeHelpers.DispatchTailCalls(IntPtr, Void ` | 3 |
| `FSharp.Compiler.CheckExpressions.TcExprNoRecover` | 3 |
| `FSharp.Compiler.CheckExpressions+TcExpr@5392.Invoke` | 3 |
| `FSharp.Compiler.CheckExpressions.TcExprUndelayed` | 2 |
| `FSharp.Compiler.CheckExpressions.TcIteratedLambdas` | 2 |
| `FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElementNonMutRec@5258-16.Invoke` | 2 |
| `FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec` | 2 |
| `FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5540-5.Invoke` | 2 |
| `FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5519-8.Invoke` | 2 |

## Global Frame Occurrence Counts

| Frame | Count |
|-------|-------|
| `FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987-1<System.__Canon>.Invoke` | 4 |
| `FSharp.Compiler.DiagnosticsLogger+GuardCancellable@987<System.__Canon>.Invoke` | 4 |
| `FSharp.Compiler.CheckExpressions.TcExprNoRecover` | 3 |
| `FSharp.Compiler.CheckExpressions+TcExpr@5392.Invoke` | 3 |
| `FSharp.Compiler.CheckExpressions.TcExprUndelayed` | 2 |
| `FSharp.Compiler.CheckExpressions.TcIteratedLambdas` | 2 |
| `FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElementNonMutRec@5258-16.Invoke` | 2 |
| `FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec` | 2 |
| `FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5540-5.Invoke` | 2 |
| `FSharp.Compiler.CheckDeclarations+TcModuleOrNamespaceElements@5519-8.Invoke` | 2 |
| `FSharp.Compiler.ParseAndCheckInputs+TypeCheckingGraphProcessing.processTypeCheckingGraph` | 1 |
| `FSharp.Compiler.ParseAndCheckInputs+CheckMultipleInputsUsingGraphMode@1849-1.Invoke` | 1 |
| `FSharp.Compiler.ParseAndCheckInputs.CheckClosedInputSet` | 1 |
| `FSharp.Compiler.Driver.TypeCheck` | 1 |
| `FSharp.Compiler.Driver.main1` | 1 |
| `FSharp.Compiler.Driver.CompileFromCommandLineArguments` | 1 |
| `FSharp.Compiler.CommandLineMain.main` | 1 |
| `Microsoft.FSharp.Core.LanguagePrimitives+HashCompare.GenericHashArbArray` | 1 |
| `FSharp.Compiler.TypeRelations.TypeFeasiblySubsumesType` | 1 |
| `FSharp.Compiler.ConstraintSolver+compareTypes@3658-1.Invoke` | 1 |
| `FSharp.Compiler.ConstraintSolver+compareCond@3654-1<System.__Canon>.Invoke` | 1 |
| `FSharp.Compiler.ConstraintSolver+bestMethods@3819-1.Invoke` | 1 |
| `FSharp.Compiler.ConstraintSolver+bestMethods@3818.Invoke` | 1 |
| `FSharp.Compiler.ConstraintSolver.GetMostApplicableOverload` | 1 |
| `FSharp.Compiler.ConstraintSolver.ResolveOverloading` | 1 |
| `FSharp.Compiler.ConstraintSolver.expr2@717-3` | 1 |
| `FSharp.Compiler.ConstraintSolver.expr2@717-2` | 1 |
| `FSharp.Compiler.ConstraintSolver.SolveMemberConstraint` | 1 |
| `FSharp.Compiler.ConstraintSolver.SolveRelevantMemberConstraintsForTypar` | 1 |
| `FSharp.Compiler.ConstraintSolver+SolveRelevantMemberConstraints@2312.Invoke` | 1 |

## Conclusion

The LangVersion=preview test shows **identical hang behavior** to LangVersion 8, 9, and 10:

- Main compiler thread stuck in `FSharp.Compiler.ConstraintSolver.ResolveOverloading`
- Same recursive pattern of `TcModuleOrNamespaceElementsNonMutRec`
- Same `SolveMemberConstraint` and `SolveRelevantMemberConstraintsForTypar` in call stack

**The hang is NOT caused by any specific LangVersion feature.**

