(**
---
title: Large inputs and stack overflows
category: Compiler
categoryindex: 1
index: 3
---
*)
## Processing large inputs

The compiler accepts large inputs such as:

* Large literals, such as `let str = "a1" + "a2" + ... + "a1000"`
* Large array expressions
* Large list expressions
* Long lists of sequential expressions
* Long lists of bindings, such as `let v1 = e1 in let v2 = e2 in ....`
* Long sequences of `if .. then ... else` expressions
* Long sequences of `match x with ... | ...` expressions
* Combinations of these

The compiler performs constant folding for large constants so there are no costs to using them at runtime. However, this is subject to a machine's stack size when compiling, leading to `StackOverflow` exceptions if those constants are very large. The same can be observed for certain kinds of array, list, or sequence expressions. This appears to be more prominent when compiling on macOS because macOS has a smaller stack size.

Many sources of `StackOverflow` exceptions prior to F# 4.7 when processing these kinds of constructs were resolved by processing them on the heap via continuation passing techniques. This avoids filling data on the stack and appears to have negligible effects on overall throughout or memory usage of the compiler.

Aside from array expressions, most of the previously-listed inputs are called "linear" expressions. This means that there is a single linear hole in the shape of expressions. For example:

* `expr :: HOLE` (list expressions or other right-linear constructions)
* `expr; HOLE` (sequential expressions)
* `let v = expr in HOLE` (let expressions)
* `if expr then expr else HOLE` (conditional expression)
* `match expr with pat[vs] -> e1[vs] | pat2 -> HOLE` (for example, `match expr with Some x -> ... | None -> ...`)

Processing these constructs with continuation passing is more difficult than a more "natural" approach that would use the stack.

For example, consider the following contrived example:

```fsharp
and remapLinearExpr g compgen tmenv expr contf =
    match expr with
    | Expr.Let (bind, bodyExpr, m, _) ->
        ...
        // tailcall for the linear position
        remapLinearExpr g compgen tmenvinner bodyExpr (contf << (fun bodyExpr' ->
            ...))

    | Expr.Sequential (expr1, expr2, dir, spSeq, m)  ->
        ...
        // tailcall for the linear position
        remapLinearExpr g compgen tmenv expr2 (contf << (fun expr2' ->
            ...))

    | LinearMatchExpr (spBind, exprm, dtree, tg1, expr2, sp2, m2, ty) ->
        ...
        // tailcall for the linear position
        remapLinearExpr g compgen tmenv expr2 (contf << (fun expr2' ->  ...))

    | LinearOpExpr (op, tyargs, argsFront, argLast, m) ->
        ...
        // tailcall for the linear position
        remapLinearExpr g compgen tmenv argLast (contf << (fun argLast' -> ...))

    | _ -> contf (remapExpr g compgen tmenv e)

and remapExpr (g: TcGlobals) (compgen:ValCopyFlag) (tmenv:Remap) expr =
    match expr with
    ...
    | LinearOpExpr _
    | LinearMatchExpr _
    | Expr.Sequential _
    | Expr.Let _ -> remapLinearExpr g compgen tmenv expr (fun x -> x)
```

The `remapExpr` operation becomes two functions, `remapExpr` (for non-linear cases) and `remapLinearExpr` (for linear cases). `remapLinearExpr` uses tailcalls for constructs in the `HOLE` positions mentioned previously, passing the result to the continuation.

Some common aspects of this style of programming are:

* The tell-tale use of `contf` (continuation function)
* The processing of the body expression `e` of a let-expression is tail-recursive, if the next construct is also a let-expression.
* The processing of the `e2` expression of a sequential-expression is tail-recursive
* The processing of the second expression in a cons is tail-recursive

The previous example is considered incomplete, because arbitrary _combinations_ of `let` and sequential expressions aren't going to be dealt with in a tail-recursive way. The compiler generally tries to do these combinations as well.

