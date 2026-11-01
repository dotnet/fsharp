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

---

## IQueryable Type Preservation 🟡

Tuple projections now preserve the provider's `IQueryable` type.

```fsharp
query { for p in db.Products do select (p.Id, p.Name) }
// Was: EnumerableQuery<_>
// Now: Provider's IQueryable<_>
```

**Impact:** Code checking `is EnumerableQuery<_>` may need adjustment.

After a `join`, a second `for` or a `groupBy` over tuples, the query no longer re-projects each element into a copy of itself, and a group used later in the query is read from the provider's grouping:

```fsharp
query { for c in db.Customers do join o in db.Orders on (c.Id = o.CustomerId); groupBy c.City into g; select (g.Key, g.Count()) }
// Was: ...Join(..).Select(_arg1 => new AnonymousObject`2(Item1 = _arg1.Item1, Item2 = _arg1.Item2)).GroupBy(..)
//        .Select(_arg2 => new Grouping`2(_arg2.Key, _arg2.Select(v => new Tuple`2(v.Item1, v.Item2)))).Select(g => ..g.Key..g.Count()..)
// Now: ...Join(..).GroupBy(..).Select(g => new AnonymousObject`2(Item1 = g.Key, Item2 = g.Count()))
```

**Impact:** A group of tuples or records that a query uses as a value in the middle of a query, for example one that is passed on whole to a later `select` or `where`, is rebuilt from the provider's grouping by `RebuiltGroupings.Of`, so that it stays one object in every operator that mentions it. linq2db, which built such a group on the client, cannot be handed the grouping itself and no longer translates that call: such a query no longer translates with linq2db. Reading the key of a group, or counting, summing, averaging or searching it, is not affected: those read the provider's grouping.

---

## FCS API: Query Variables 🟢

Query variables now report `IsCompilerGenerated = true`. Fixes #422 (FS1182 false positives).
