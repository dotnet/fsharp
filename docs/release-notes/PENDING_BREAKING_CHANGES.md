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

### A `for` or join after two `for`s continues the chain

Over an `IQueryable`, a query that goes on after two `for`s no longer nests the rest inside the collection selector of the
first `SelectMany`; like C#, it pairs the elements of the first two sources and continues from the pairs.

```fsharp
query { for c in db.Customers do for o in c.Orders do for l in o.Lines do select (c.Name, l.Id) }
// Was: Customers.SelectMany(c => c.Orders.SelectMany(o => o.Lines, ...), (c, x) => x)
// Now: Customers.SelectMany(c => c.Orders, (c, o) => new AnonymousObject(c, o)).SelectMany(pair => pair.Item2.Lines, ...)
```

**Impact:** Code that walks the expression tree of such a query sees a longer chain and no nested `SelectMany`, `Join`
or `GroupJoin`. The rows are the same.

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
