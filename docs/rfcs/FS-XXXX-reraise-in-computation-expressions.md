# F# RFC FS-XXXX - `reraise` in computation expressions

The design suggestion [Allow `return reraise()` in `async { }`](https://github.com/fsharp/fslang-suggestions/issues/660) has been marked "approved in principle".

This RFC covers the detailed proposal for this suggestion.

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/660)
- [x] Approved in principle
- [x] Implementation
- [ ] Design Review Meeting(s) with @dsyme and others invitees
- [ ] Discussion

## Summary

`reraise ()` becomes legal in the `with` handler of a `try ... with` written inside a computation expression:

```fsharp
async {
    try
        return! callService ()
    with e ->
        logger.LogError(e, "call failed")
        return reraise ()
}
```

Today this is rejected with FS0413, *"Calls to 'reraise' may only occur directly in a handler of a try-with"*.

## Motivation

A computation expression's `try ... with` is translated to `builder.TryWith(builder.Delay(fun () -> body), function <clauses>)`. The handler is an ordinary function that the builder calls, not a .NET exception handler, so the IL `rethrow` instruction that `reraise ()` compiles to is not valid there.

This forces the observability pattern above to be written as `raise (Exception("call failed", e))`, which allocates a new exception, changes the exception's type, buries the original one in `InnerException`, and resets the stack trace; or as `raise e`, which keeps the type but discards the original stack trace.

.NET has provided `System.Runtime.ExceptionServices.ExceptionDispatchInfo` for exactly this situation since .NET Framework 4.5: it rethrows the original exception object with its stack trace preserved, from anywhere. The compiler already uses it for this exact reason — the unmatched clause of a computation expression handler compiles to `ExceptionDispatchInfo.Capture(e).Throw()` rather than `rethrow`. This RFC extends that treatment to `reraise ()` written by the user.

## Detailed design

### Semantics

`reraise ()` rethrows the exception caught by the **nearest lexically enclosing exception handler**.

When that handler is a real `try ... with`, nothing changes: the call compiles to the IL `rethrow` instruction as it does today.

When it is a computation expression handler, the call compiles to `ExceptionDispatchInfo.Capture(<caught exception>).Throw()`. The observable consequences are:

- The exception object rethrown is the same instance that the handler caught, so its type, message, and data are preserved and `Object.ReferenceEquals` holds against the value bound by the `with` pattern.
- Its stack trace is preserved, with the rethrow site appended — the same fidelity `rethrow` gives, and the same the compiler already provides for unmatched clauses.
- `Async` associates the original exception with an `ExceptionDispatchInfo` as it flows through the trampoline, and hands the handler the original exception, so the trace observed at the top level covers the original throw site.

Because binding is lexical, `reraise ()` also works in these positions inside a computation expression handler:

```fsharp
async {
    try
        return! callService ()
    with e ->
        do! logAsync e          // after a bind: the continuation is a generated lambda
        let rethrow () = reraise ()
        return rethrow ()       // inside a user-written function
}
```

and in a nested computation expression that has no handler of its own:

```fsharp
with e -> return! async { return reraise () }   // rethrows e
```

and in a `when` guard:

```fsharp
with
| e when shouldRethrow e -> return reraise ()
```

A nested real `try ... with` inside a computation expression handler rebinds `reraise ()` to its own exception:

```fsharp
async {
    try
        raise Outer
    with _ ->
        try
            raise Inner
        with _ ->
            reraise ()          // rethrows Inner, through IL 'rethrow'
}
```

The rewrite requires the value the handler catches to have type `exn`. Builders whose `TryWith` takes a handler of any other type keep today's FS0413.

### Scope

The change applies to every computation expression: builder-based ones (`async`, `task`, `taskSeq`, custom builders) and the built-in `seq`, list, and array comprehensions.

Positions that remain errors, unchanged:

| Code | Diagnostic |
|---|---|
| `reraise ()` outside any handler | FS0413 |
| `reraise ()` in the `try` body | FS0413 |
| `let f = reraise` (first-class use) | FS0417 |
| `reraise ()` in a builder whose handler does not take an `exn` | FS0413 |

### Compilation

The type checker records the value holding the caught exception while checking the clause bodies and guards of a computation expression handler, and rewrites `reraise ()` against it. The handler of a real `try ... with` clears that record, which is what makes a nested real handler win.

No FSharp.Core change is required — `ExceptionDispatchInfo` is a BCL type, and the compiler already resolves it for the unmatched-clause path, falling back to a plain `throw` on frameworks that lack it.

## Drawbacks

`reraise ()` acquires a second lowering, so the same syntax now means "IL rethrow" in one context and "`ExceptionDispatchInfo` rethrow" in another. The observable behavior is intended to be the same, but the two are not identical at the IL level, and an `ExceptionDispatchInfo` rethrow appends a frame that `rethrow` does not.

Allowing `reraise ()` in closures written inside a handler means a closure can outlive its handler and rethrow the exception later, from an unrelated stack. This is well-defined but is not something a real `try ... with` permits.

## Alternatives

**Do nothing.** Users keep wrapping exceptions in `raise (Exception(msg, e))`, losing the exception type and the original trace, which is the complaint the suggestion opens with.

**A library function such as `reraiseWith e`.** This works today as user code, needs no compiler change, and is what several workarounds in the suggestion thread do. It does not address the request: the point of the suggestion is that the existing `reraise ()` should mean the obvious thing inside a computation expression, rather than every library and application defining its own helper.

**Relax the FS0413 check without changing the lowering.** Not possible — the IL `rethrow` instruction is invalid outside a handler block and would produce unverifiable code.

## Compatibility

- **Is this a breaking change?** No. Every program this accepts is one that previously failed to compile.
- **What happens when previous versions of the F# compiler encounter this design addition as source code?** They report FS0413, as they do today.
- **What happens when previous versions of the F# compiler encounter this design addition as compiled binaries?** Nothing; the feature leaves no trace in the assembly surface. The generated code calls only BCL methods.
- **If this is a change or extension to FSharp.Core, what happens when previous versions of FSharp.Core are used by a compiler that supports this feature?** FSharp.Core is unchanged, so there is nothing to guard.

## Unresolved questions

- Should `reraise ()` be allowed in closures that can escape the handler, or should the binding stop at user-written lambda boundaries? This RFC proposes allowing it, on the grounds that the rule "nearest lexically enclosing handler" is simpler to state and the escaping case is well-defined.
- Should the FS0413 message point at the language version when the code would be accepted under `preview`?
