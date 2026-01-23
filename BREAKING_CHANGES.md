# Breaking Changes in F# Query Expression Fixes

This document describes breaking changes introduced by the query expression bug fixes in this release. These changes fix long-standing bugs but may affect code that depends on previous (incorrect) behavior.

---

## Summary

| Change | Severity | Component |
|--------|----------|-----------|
| [AnonymousObject structural equality](#anonymousobject-structural-equality) | 🔴 High | Runtime |
| [Expression tree structure for let-bindings](#expression-tree-structure-for-let-bindings) | 🟡 Medium | Expression Trees |
| [Expression tree structure for array indexing](#expression-tree-structure-for-array-indexing) | 🟡 Medium | Expression Trees |
| [IQueryable type preservation](#iqueryable-type-preservation) | 🟡 Medium | Query Builder |

---

## AnonymousObject Structural Equality

**Severity:** 🔴 High  
**Component:** `Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<T1..T8>`

### What Changed

The `AnonymousObject<T>` types (used internally for tuple representation in LINQ queries) now implement structural equality via `Equals` and `GetHashCode` overrides.

### Previous Behavior

```fsharp
open Microsoft.FSharp.Linq.RuntimeHelpers

let a = AnonymousObject<int, string>(1, "x")
let b = AnonymousObject<int, string>(1, "x")

a.Equals(b)  // false (reference equality)
a.GetHashCode() = b.GetHashCode()  // usually false
```

### New Behavior

```fsharp
let a = AnonymousObject<int, string>(1, "x")
let b = AnonymousObject<int, string>(1, "x")

a.Equals(b)  // true (structural equality)
a.GetHashCode() = b.GetHashCode()  // true
```

### Impact

Code that relies on `AnonymousObject` instances being distinct based on reference identity will behave differently:

```fsharp
// ⚠️ AFFECTED: Using AnonymousObject as dictionary key
let dict = Dictionary<AnonymousObject<int, int>, string>()
let key1 = AnonymousObject(1, 2)
let key2 = AnonymousObject(1, 2)
dict.[key1] <- "first"
dict.[key2] <- "second"

// BEFORE: dict.Count = 2 (two distinct keys)
// AFTER:  dict.Count = 1 (same key, second overwrites first)
```

### Why This Changed

This change fixes [#7885](https://github.com/dotnet/fsharp/issues/7885) and [#47](https://github.com/dotnet/fsharp/issues/47). Previously, `join` and `groupBy` operations with tuple keys failed because the join keys were compared by reference instead of value.

### Migration

If your code uses `AnonymousObject` directly (which is not recommended—these are internal types):

1. **For identity-based comparisons:** Use `Object.ReferenceEquals(a, b)` explicitly
2. **For dictionary keys requiring instance identity:** Use a wrapper type or a different key strategy

### Who Is Affected

- Code directly using `AnonymousObject<T>` types from `Microsoft.FSharp.Linq.RuntimeHelpers`
- Code using these types as dictionary keys or in hash-based collections with identity semantics

Most F# code is **not affected** because these types are internal implementation details of the query builder.

---

## Expression Tree Structure for Let-Bindings

**Severity:** 🟡 Medium  
**Component:** `LeafExpressionConverter.ConvExprToLinq`

### What Changed

When converting F# quotations containing `let` bindings to LINQ expression trees, the structure of the generated tree has changed.

### Previous Behavior

```fsharp
<@ let x = 1 in x + 1 @>
// Generated: Lambda(x => x + 1).Invoke(1)
// Tree: MethodCallExpression(Invoke, LambdaExpression, [Constant(1)])
```

### New Behavior

```fsharp
<@ let x = 1 in x + 1 @>
// Generated: 1 + 1 (inlined)
// Tree: BinaryExpression(Add, Constant(1), Constant(1))
```

### Impact

Code that inspects LINQ expression trees and specifically looks for the `Lambda.Invoke` pattern to detect let-bindings will no longer match:

```csharp
// ⚠️ AFFECTED: Custom ExpressionVisitor
public override Expression VisitMethodCall(MethodCallExpression node) {
    if (node.Method.Name == "Invoke" && node.Object is LambdaExpression lambda) {
        // Detect F# let binding...  ← NO LONGER WORKS
    }
    return base.VisitMethodCall(node);
}
```

### Why This Changed

This change fixes [#11131](https://github.com/dotnet/fsharp/issues/11131), [#15648](https://github.com/dotnet/fsharp/issues/15648), and related issues. The `Lambda.Invoke` pattern cannot be translated by most LINQ providers (Entity Framework Core, Azure Cosmos DB, etc.), causing runtime failures.

### Migration

If you have custom expression tree processing that detects let-bindings:

1. **For LINQ providers:** No action needed—the new trees are more compatible
2. **For custom analysis:** Update pattern matching to handle inlined expressions

### Who Is Affected

- Custom LINQ providers that explicitly handle F#-generated expression patterns
- Expression tree analysis/transformation tools
- Code that compares expression tree structures for equality

Standard LINQ providers like Entity Framework Core will work **better** with the new behavior.

---

## Expression Tree Structure for Array Indexing

**Severity:** 🟡 Medium  
**Component:** `LeafExpressionConverter.ConvExprToLinq`

### What Changed

Array indexing in F# quotations now generates the standard LINQ `ArrayIndex` expression instead of a method call.

### Previous Behavior

```fsharp
<@ arr.[0] @>
// Generated: Call(null, GetArray, [arr, 0])
// Tree: MethodCallExpression for FSharp.Core.LanguagePrimitives.IntrinsicFunctions.GetArray
```

### New Behavior

```fsharp
<@ arr.[0] @>
// Generated: ArrayIndex(arr, 0)
// Tree: BinaryExpression(ArrayIndex, arr, Constant(0))
```

### Impact

Code that looks for `GetArray` method calls in expression trees will no longer match:

```csharp
// ⚠️ AFFECTED: Custom ExpressionVisitor
public override Expression VisitMethodCall(MethodCallExpression node) {
    if (node.Method.Name == "GetArray") {
        // Handle array access...  ← NO LONGER WORKS
    }
    return base.VisitMethodCall(node);
}
```

### Why This Changed

This change fixes [#16918](https://github.com/dotnet/fsharp/issues/16918). LINQ providers like Azure Cosmos DB could not translate the `GetArray` method call, causing "Method 'GetArray' is not supported" errors.

### Migration

If you have custom expression tree processing that looks for `GetArray`:

```csharp
// BEFORE
if (node.Method.Name == "GetArray") { ... }

// AFTER
// Handle ArrayIndex expressions instead:
public override Expression VisitBinary(BinaryExpression node) {
    if (node.NodeType == ExpressionType.ArrayIndex) {
        // Handle array access
    }
    return base.VisitBinary(node);
}
```

### Who Is Affected

- Custom LINQ providers handling F#-generated expressions
- Expression tree analysis tools looking for `GetArray` pattern

---

## IQueryable Type Preservation

**Severity:** 🟡 Medium  
**Component:** `Query.fs` (Query Builder)

### What Changed

Queries with tuple projections now preserve the original `IQueryable<T>` provider type instead of wrapping in `EnumerableQuery<T>`.

### Previous Behavior

```fsharp
let result = query { 
    for p in dbContext.Products do 
    select (p.Id, p.Name) 
}
// result.GetType() = EnumerableQuery<int * string>
```

### New Behavior

```fsharp
let result = query { 
    for p in dbContext.Products do 
    select (p.Id, p.Name) 
}
// result.GetType() = <provider's IQueryable type>
```

### Impact

Code that checks for `EnumerableQuery<T>` type will behave differently:

```fsharp
// ⚠️ AFFECTED: Type-based dispatch
match result with
| :? EnumerableQuery<_> -> 
    // Handle locally-executed query...  ← MAY NO LONGER MATCH
| :? IQueryable<_> -> 
    // Handle remote query
```

### Why This Changed

This change fixes [#3782](https://github.com/dotnet/fsharp/issues/3782) and [#15133](https://github.com/dotnet/fsharp/issues/15133). The previous behavior broke Entity Framework Core's ability to translate queries with tuple projections and prevented async query operations like `ToListAsync()`.

### Migration

If you detect local vs. remote queries by type:

```fsharp
// Instead of checking for EnumerableQuery, check the provider
let isLocalQuery (q: IQueryable<_>) =
    q.Provider.GetType().Name.Contains("EnumerableQuery")
```

### Who Is Affected

- Code using type checks to distinguish local vs. remote queries
- Code expecting specific `IQueryable` wrapper types

---

## FSharp.Compiler.Service API Change

**Severity:** 🟢 Low  
**Component:** FSharp.Compiler.Service Symbol API

### What Changed

Query expression variables (e.g., `x` in `for x in source do`) now report `IsCompilerGenerated = true` via the FCS Symbol API.

### Previous Behavior

```fsharp
// query { for x in source do ... }
symbol.IsCompilerGenerated  // false
```

### New Behavior

```fsharp
// query { for x in source do ... }
symbol.IsCompilerGenerated  // true
```

### Impact

- IDE plugins that color compiler-generated symbols differently
- Static analysis tools that filter out compiler-generated symbols

### Why This Changed

This change fixes [#422](https://github.com/dotnet/fsharp/issues/422). Query variables were incorrectly triggering FS1182 "unused variable" warnings when `--warnon:1182` was enabled.

---

## Questions?

If you encounter issues with these changes, please:

1. Check if your code falls into the affected scenarios described above
2. Review the linked GitHub issues for context
3. File a new issue if you believe there's an unintended regression

These changes fix real bugs that have affected F# developers for years. We believe the benefits significantly outweigh the compatibility impact for the small number of affected edge cases.
