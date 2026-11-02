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

### Tuple and record constructions name their members

Constructing a tuple, struct tuple, record or anonymous record now gives a `NewExpression` whose `Members` say which property (or `ValueTuple` field) each argument becomes, as C# anonymous types do, so providers can bind a later read such as `t.Item1` or `r.Name` to the argument.

```fsharp
<@ fun (c: Customer) -> (c.Id, c.Name) @>
// Was: new Tuple`2(c.Id, c.Name)
// Now: new Tuple`2(Item1 = c.Id, Item2 = c.Name)
```

A record whose fields have no public getter is still built without `Members`.

The SQL that a provider writes names its columns after the members (`SELECT "b"."Id" AS "Item1", ...`), which shows in tests that compare SQL text.

A `groupBy` on an item of a tuple that an earlier operator selected, `for (city, id) in (select (b.Address.City, b.Id)) do groupBy city into g; select (g.Key, g.Count())`, failed in EF Core with "could not be translated" and now fails inside EF Core with `KeyNotFoundException: The given key 'Item1' was not present in the dictionary`. It translates together with the change to groupings in #20899.

Together with the change to F# operators in #20900, a `leftOuterJoin` that follows two `for`s and a `join`, and tests the joined row with `isNull`, fails in EF Core 10 for SQL Server with "Nullable object must have a value": `for c in db.Customers do for o in c.Orders do join d in db.Customers on (c.City = d.City); leftOuterJoin p in db.Products on (d.Id = p.Id) into products; for p in products do select (c.Name, o.Id, d.Name, (if isNull p then "" else p.Name))`. EF Core translates it with either change alone.

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
