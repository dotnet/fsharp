---
title: Pending breaking changes
category: Release Notes
categoryindex: 600
index: 5
---
# Breaking Changes in Query Expression Fixes

## AnonymousObject Structural Equality 🔴

`AnonymousObject<T>` types now implement `Equals`/`GetHashCode` with structural equality.

**Impact:** Code using these as Dictionary keys with identity semantics will see different behavior.

```fsharp
let a = AnonymousObject(1, 2)
let b = AnonymousObject(1, 2)
a.Equals(b)  // Was: false, Now: true
```

**Why:** Fixes #7885, #47 - join/groupBy with tuple keys now work correctly.

---

## Expression Tree Structure Changes 🟡

### Let-bindings use Block instead of Lambda.Invoke

```fsharp
<@ let x = 1 in x + 1 @>
// Was: Lambda(x => x+1).Invoke(1)
// Now: Block({ x = 1; x + 1 })
```

**Migration:** If you detected let-bindings via `Lambda.Invoke`:
```csharp
// Old: if (node.Method.Name == "Invoke" && node.Object is LambdaExpression)
// New: if (node.NodeType == ExpressionType.Block)
```

Inside a lambda handed to a LINQ provider (a query operator over `IQueryable`, or an F# lambda passed as `Expression<Func<..>>`) a let no longer makes a `Block` where leaving it out computes the same:

- its value only reads: a constant, the lambda's parameter or a member of it (`let age = p.Age`), or what a member or method of a string, number or date makes of those (`let upper = p.Name.ToUpper()`);
- or the variable is read once, as the first thing the rest of the lambda evaluates (`let orders = p.Orders.Count() in orders > 2`).

This covers the lets the compiler introduces for tupled lambda parameters and struct copies. For record and anonymous record fields written out of order, the lets are left out as well:

```fsharp
db.People.Select(fun p -> {| Name = p.Name; Age = p.Age |})
// Was: p => { var Name = p.Name; new <>f__AnonymousType(p.Age, Name) }
// Now: p => new <>f__AnonymousType(p.Age, p.Name)
```

**Impact:** When such a lambda is compiled and run, the fields of a record written out of order are evaluated in declaration order instead of the order they are written in, which shows when they have side effects. A let that only reads, such as `let age = p.Age` or `let upper = p.Name.ToUpper()`, is computed where the variable is used: once for each use, at the moment of that use, and not at all on a path that does not use it.

Every other let keeps its `Block` and is evaluated once, where it is written: a value that is computed by a call and read more than once, by a nested lambda or after something else has run, a struct of your own or a boxed value, a `mutable` variable, and every let of a lambda that assigns, sets a property or field, or runs a statement. `EvaluateQuotation` and `QuotationToExpression` are not affected.

### Array access uses ArrayIndex

```fsharp
<@ arr.[0] @>
// Was: Call(GetArray, arr, 0)
// Now: ArrayIndex(arr, 0)
```

**Migration:**
```csharp
// Old: if (node.Method.Name == "GetArray")
// New: if (node.NodeType == ExpressionType.ArrayIndex)
```

---

## IQueryable Type Preservation 🟡

Tuple projections now preserve the provider's `IQueryable` type.

```fsharp
query { for p in db.Products do select (p.Id, p.Name) }
// Was: EnumerableQuery<_>
// Now: Provider's IQueryable<_>
```

**Impact:** Code checking `is EnumerableQuery<_>` may need adjustment.

---

## FCS API: Query Variables 🟢

Query variables now report `IsCompilerGenerated = true`. Fixes #422 (FS1182 false positives).
