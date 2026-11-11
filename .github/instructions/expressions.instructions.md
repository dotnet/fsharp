---
applyTo: 'src/Compiler/Checking/Expressions/**'
---

Expression and computation-expression elaboration: CheckExpressions (left-to-right checking, generalization), CheckComputationExpressions (builder desugaring).

- `CheckExpressions` must preserve left-to-right checking and generalization points; expression order is part of the typechecker contract.
- Keep the real `Expr.Let` for discard/unit bindings when the RHS is byref-like; `PostInferenceChecks` relies on that node to enforce byref scope rules.
- Call `requireBuilderMethod` before translating each computation-expression form, so translation fails early when the builder lacks `Bind`, `Delay`, or `Using` rather than emitting a broken shape.
