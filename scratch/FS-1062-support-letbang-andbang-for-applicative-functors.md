 F# RFC FS-1062 - Support let! .. and... for applicative functors

The design suggestion [Support let! .. and... for applicative functors](https://github.com/fsharp/fslang-suggestions/issues/579) has been marked "approved in principle".
This RFC covers the detailed proposal for this suggestion.

* [x] [Approved in principle](https://github.com/fsharp/fslang-suggestions/issues/579#event-1345361104) & [prioritised](https://github.com/fsharp/fslang-suggestions/issues/579#event-1501977428)
* [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/579)
* [ ] Implementation: [In progress](https://github.com/Microsoft/visualfsharp/pull/FILL-ME-IN)


# Summary
[summary]: #summary

Extend computation expressions to support applicative functors via a new `let! ... and! ... return ...` syntax.

# Motivation
[motivation]: #motivation

Applicative functors (or just "applicatives", for short) have been growing in popularity as a way to build applications and model certain domains over the last decade or so, since McBride and Paterson published [Applicative Programming with Effects](http://www.staff.city.ac.uk/~ross/papers/Applicative.html). Applicatives are now reaching a level of popularity within the community that supporting them with a convenient and readable syntax, as we do for monads, is beginning to make sense.

## Why Applicatives?

_Applicative functors_ sit, in terms of power, somewhere between _functors_ (i.e. types which support `Map`), and _monads_ (i.e. types which support `Bind`).

If we consider `Bind : M<'T> * ('T -> M<'U>) -> M<'U>`, we can see that the second element of the input is a function that requires a value to create the resulting "wrapped value".

In contrast, `Apply : M<'T -> 'U> * M<'T> -> M<'U>` only needs a wrapped function, which is something we have whilst building our computation.

So, importantly, applicatives allow us the power to use functions which are "wrapped up" inside a functor, but [preserve our ability to analyse the structure of the computation](https://paolocapriotti.com/assets/applicative.pdf) - and hence introduce optimisations or alternative interpretations - without any values present. This is a critical distinction which can have a huge impact on performance, and indeed on what is possible to construct at all, so has very tangible implications. 

## Examples of Useful Applicatives

The examples below all make use of types which are applicatives, but explicitly _not_ monads, to allow a powerful model for building a particular kind of computation, whilst preserving enough constraints to offer useful guarantees.

[Tomas Petricek's formlets blog post](http://tomasp.net/blog/formlets-in-linq.aspx/) introduces the idea that we can use applicatives to build web forms. The guarantee of a static structure of the formlet applicative is used to render forms, but its powerful behaviours still allow useful processing of requests.

[Pauan's comment about Observables](https://github.com/fsharp/fslang-suggestions/issues/579#issuecomment-310799948) points out that applicatives allow us to avoid frequent resubscriptions to `Observable` values because we know precisely how they'll be hooked up ahead of time, and that it won't change within the lifetime of the applicative.

[McBride & Paterson's paper](http://www.staff.city.ac.uk/~ross/papers/Applicative.html) introduces a type very much like F#'s `Result<'T,'TError>` which can be used to stitch together functions and values which might fail, but conveniently accumulating up all of the errors which can then be helpfully presented at once, as opposed to immediately presenting the first error. This allows you to take [Scott Wlaschin's Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) to the next level!

[Capriotti & Kaposi's paper](https://paolocapriotti.com/assets/applicative.pdf) introduce an example of creating an command line argument parser, where a single applicative can both statically generate help text for the parser, and dynamically parse options given to an application.

More advanced usages include self-adjusting computations, such as [Jane Street's Incremental](https://blog.janestreet.com/introducing-incremental/), but with even greater opportunity for efficiencies via the careful balance of power and guarantees that applicatives give (such as the ability to optimise the computation, and avoid recreating the rest of the computation every time a `Bind` is reached).

# Detailed design
[design]: #detailed-design

This RFC introduces a desugaring for applicative computation expressions, much like that which exists for monads and related constructs.

Example desugaring:

```fsharp
ce {
    let! x = foo
    and! y = bar
    and! z = baz
    return x + y + z
 }
```

⇒

```fsharp
ce.Apply(
    ce.Apply(
        ce.Apply(
            ce.Return(
                (fun x ->
                    (fun y ->
                        (fun z ->
                            x + y + z
                        )
                    )
                )),
            foo),
        bar), 
    baz)
```

To be accepted as an applicative computation expression (CE), the CE must be of the form `let! ... and! ... return ...` (with one or more `and!`s). This means, for example, no `let!`s after an `and!` in the same CE, no normal `let`s in between the `let!` and `and!`s, and no usages of `yield` in place of `return`.

This may sound very constrained, but it is for good reason. The structure imposed by this rule forces the CE to be in a canonical form ([McBride & Paterson](http://www.staff.city.ac.uk/~ross/papers/Applicative.html)):

> Any expression built from the Applicative combinators can be transformed to a canonical form in which a single pure function is ‘applied’ to the effectful parts in depth-first order:  
`pure f <*> arg1 <*> ... <*> argN`  
This canonical form captures the essence of Applicative programming: computations have a fixed structure, given by the pure function, and a sequence of subcomputations, given by the effectful arguments.

In our case, the expression to the right of `return` (i.e. `pure`) becomes the body of a lambda, whose parameters are introduced by the `let! ... and! ...` preceding it. This leads to a very straightforward desugaring of the syntax, which therefore ensures both using and understanding the feature is only as complex as is inherently required by the abstraction.

Despite requiring the canonical form, there are still many ways to build more complex and useful expressions from this syntax. The rest of this section aims to give a tour around these various features.

## Pattern Matching

```fsharp
let (|Quad|) (i : int) =
    i * i * i * i

type SingleCaseDu<'a> = SingleCaseDu of 'a

ce {
    let! Quad(x)          = foo
    and! (y,_)            = bar
    and! (SingleCaseDu z) = baz
    return x + y + z
 }
```

⇒

```fsharp
ce.Apply(
    ce.Apply(
        ce.Apply(
            ce.Return(
                (fun Quad(x) ->
                    (fun (y,_) ->
                        (fun (SingleCaseDu z) ->
                            x + y + z
                        )
                    )
                )),
            foo),
        bar), 
    baz)
```

Pattern matching on the left-hand side of of a `let! ... and! ...` binding is valid, and desugars into the same pattern, but now as part of the lambda corresponding to that binding.

## Using Monadic and Applicative Styles Simultaneously

Recall that `let! ... and! ... return ...` syntax precludes an additional `let!` anywhere in the CE. In the case where your applicative happens also to be a monad, and you want to leverage the benefits of an applicative in some places (e.g. for performance reasons) but also use a `let!` (e.g. for convenience, or to do something a pure applicative doesn't support), you must do so inside a different CE context, e.g.:

```fsharp
ce {
    let! quux =
        ce {
            let! x                = foo
            and! (y,_)            = bar
            and! (SingleCaseDu z) = baz
            return x + y + z
        }
    if quux > 6
    then
        return quux
    else
        return 5
}
```

⇒

```fsharp
ce.Bind(
    ce.Apply(
        ce.Apply(
            ce.Apply(
                ce.Return(
                    (fun x ->
                        (fun (y,_) ->
                            (fun (SingleCaseDu z) ->
                                x + y + z
                            )
                        )
                    )),
                foo),
            bar),
        baz),
    (fun quux ->
        if quux > 6
        then
            return quux
        else
            return 5)
)
```

## Monoids

The existing `let!` CE syntax allows us to use `Combine` (typically in conjunction with `yield`) to define monad plus instances (i.e. also interpret the type as a monoid). [Just as monad plus is to a monad, alternatives are to applicatives](https://en.wikibooks.org/wiki/Haskell/Alternative_and_MonadPlus), so we can do a similar thing for our applicative CE syntax. One motivation for this might be the command line argument example from earlier, where alternatives allow parsing discriminated unions in a way that translates to something akin to "try this case, else try this case, else try this case, ...".

One might assume that the syntax would be something such as:

```fsharp
ce {
    let! x = foo
    and! y = bar
    and! z = baz
    yield x + y + z
    yield x + y
    yield y + z
 }
```

Unfortunately, the naive desugaring of this can make it very easy to build a resulting chain of method calls which unintentionally ends up being very large:

```fsharp
ce.Combine(
    ce.Combine(
        ce.Apply(
            ce.Apply(
                ce.Apply(
                    ce.Return(
                        (fun x ->
                            (fun y ->
                                (fun z ->
                                    x + y + z
                                )
                            )
                        )),
                    foo),
                bar),
            baz),
        ce.Apply(
            ce.Apply(
                ce.Apply(
                    ce.Return(
                        (fun x ->
                            (fun y ->
                                (fun z ->
                                    x + y
                                )
                            )
                        )),
                    foo),
                bar),
            baz)
    ),
    ce.Apply(
        ce.Apply(
            ce.Apply(
                ce.Return(
                    (fun x ->
                        (fun y ->
                            (fun z ->
                                y + z
                            )
                        )
                    )),
                foo),
            bar), 
        baz)
)
```

*N.B.* How the size of the desugared expression grows with the product of the number of bindings introduced by the `let! ... and! ...` syntax and the number calls to `Combine` implied by the alternative cases.

An attempt at a very smart desugaring which tries to cut down the resulting expression might, on the face of it, seem like a reasonable option, however, beyond the cost of analysing which values which are introduced by `let! ... and! ...` actually go on to be used, we must also consider the right-hand sides of the `let! ... and! ...` bindings and the pattern matching: Do we evaluated these once up front? Or recompute them in each alternative case at the leaf of the tree of calls to `Combine`? What if the expressions on the right-hand sides have side-effects, or the left-hand side utilises active patterns with side-effects? At that point we either make complex, unintuitive rules, or force the CE writer to be explicit. Continuing in the spirit of CEs generally being straightforward desugarings, we choose the latter make make the writer clearly state their desire.

In order to keep things simple, then, we keep the canonical form introduced earlier, which forces precisely one `return` after a `let! ... and! ...`. However, we can still express alternative applicatives when using the `let! ... and! ...` syntax, we just need to use the same trick as with additional `let!`s and leave the scope of the canonical applicative syntax and therefore leave the additional constraints it places upon us:

```fsharp
ce {
    yield
        ce {
            let! x = foo
            and! y = bar
            and! z = baz
            return x + y + z
        }
    yield
        ce {
            let! x = foo
            and! y = bar
            return x + y
        }
    yield
        ce {
            let! x = foo
            and! y = bar
            return y + z
        }
}
```

⇒

```fsharp
ce.Combine(
    ce.Combine(
        ce.Apply(
            ce.Apply(
                ce.Apply(
                    ce.Return(
                        (fun x ->
                            (fun y ->
                                (fun z ->
                                    x + y + z
                                )
                            )
                        )),
                    foo),
                bar),
            baz),
        ce.Apply(
            ce.Apply(
                ce.Return(
                    (fun x ->
                        (fun y ->
                            x + y
                        )
                    )),
                foo),
            bar)
    ),
    ce.Apply(
        ce.Apply(
            ce.Return(
                (fun y ->
                    (fun z ->
                        y + z
                    )
                )),
            bar),
        baz)
)
```

*N.B.* How this syntax forces the writer to be explicit about how many times `Apply` should be called, and with which arguments, for each call to `Combine`. Notice also how the right-hand sides are still repeated for each alternative case in order to keep the occurrence of potential side-effects from evaluating them predictable, and also occur before the pattern matching _each time_ a new alternative case is explored.

If this syntax feels a bit heavy, remember that the `yield` keyword is not required, and the new syntax can be mixed with other styles (e.g. a custom `<|>` alternation operator) to strike an appropriate balance as each situation requires.

## Using

Just as monads support `Using` via `use!`, applicatives supports it via `use! ... anduse! ...`. Each binding can be either `and!` or `anduse!` (unless it is the first, in which case it must be either `let!` or `use!`), i.e. you can mix-and-match to describe which bindings should be covered by a call to `Using`:

```fsharp
ce {
     use! x    = foo // x is wrapped in a call to ce.Using(...)
     and! y    = bar // y is _not_ wrapped in a call to ce.Using(...)
     anduse! z = baz // z is wrapped in a call to ce.Using(...)
     return x + y + z
 }
```

⇒

```fsharp
ce.Apply(
    ce.Apply(
        ce.Apply(
            ce.Return(
                (fun x ->
                    ce.Using(x, fun x ->
                        // N.B. No ce.Using(...) call here because we used `and!` instead of `anduse!` for `y` in the CE. Similarly, we could have chose to use `let!` instead of `use!` for the first binding to avoid a call to Using
                        (fun y ->
                            (fun z ->
                                ce.Using(z, fun z ->
                                    x + y + z
                                )
                            )
                        )
                    )
                )),
            foo), 
        bar),
    baz)
```

## Ambiguities surrounding a `let! .. return ...`
[singlelet]: #singlelet

Some CEs could be validly desugared in multiple ways, depending on which methods are implemented on a builder (assuming the implementations follow the standard laws relating these functions).

For example:

```fsharp
ce {
    let! x = foo
    return x + 1
 }
```

Can be desugared via `Bind`:

```fsharp
ce.Bind(
    (fun x -> ce.Return(x + 1)),
    foo)
```

Or via `Apply`:

```fsharp
ce.Apply(
    ce.Return(fun x -> x + 1),
    foo)
```

This is because the operation is really equivalent to a `Map`, something which can be implemented in terms of `Return` and either `Bind` or `Apply`. We mentioned that these functions were in some sense more powerful than a plain functor's `Map`, and we are seeing an example of that here.

In order to avoid breaking backwards compatibility, the default resolution is to desugar via `Bind`, _failing if it is not defined on the builder_ (even though, conceptually, it should be implemented via `Apply`). This is consistent with in previous F# versions. [Later work on supporting `Map`](https://github.com/fsharp/fslang-design/blob/master/RFCs/FS-1048-ce-builder-map.md) can then make the choice about how to resolve this in a way which works with that in mind too.

# Drawbacks
[drawbacks]: #drawbacks

The new applicative computation expressions are quite constrained, and as has been discussed, that is precisely what allows them to be so useful. However, these constraints are potentially somewhat unintuitive to the beginner. Computation expressions already involve one of the steeper learning curves of the F# language features, so the added complexity from this feature needs to be carefully weighed against the potential guarantees, expressiveness and performance gains that they can offer.

# Alternative Designs
[alternative-designs]: #alternative-designs

[Tomas Petricek's Joinads](http://tomasp.net/blog/fsharp-variations-joinads.aspx/) offered a superset of the features proposed here, but was [rejected](https://github.com/fsharp/fslang-suggestions/issues/172) due to its complexity. This RFC is of much smaller scope, so should be a much less risky change.

# Compatibility
[compatibility]: #compatibility

## Is this a breaking change?

This change should be backwards compatible.

* Uses of `let!` without `and!` should still desugar to use `Bind`. See [the discussion of `let! ... return ...`](#singlelet) computation expressions for more details.

* Existing computation expression builders with an `Apply` method should not change in behaviour, since usages of the builder would still need to add the new `let! ... and! ...` syntax to activate it.

## What happens when previous versions of the F# compiler encounter this design addition as source code?

Previous compiler versions reject the new `and!` keyword:

```
error FS1141: Identifiers followed by '!' are reserved for future use
```

## What happens when previous versions of the F# compiler encounter this design addition in compiled binaries?

Since the syntax is desugared into a method call on the builder object, after compilation usages of this feature will be usable with previous compiler versions.

# Unresolved questions
[unresolved]: #unresolved-questions

Is `anduse!` an acceptable keyword for when `and!` must also imply a call to `Using`?

There are various ways of desugaring `let! ... return ...` via one of `Map`, `Apply` or `Bind`. [The above](#singlelet) assumes that we are aiming for backward compatibility and hence doesn't consider desugaring to `Apply` at all. Is this the best choice? Alternatives include:

* Using `Apply` in preference to `Bind` for `let! ... return` whenever `Apply` is defined (on the basis that that any reasonable `Apply` implementation will be functionally equivalent, but more efficient than the corresponding `Bind`)
* Subsuming [the RFC for desugaring this to `Map`](https://github.com/fsharp/fslang-design/blob/master/RFCs/FS-1048-ce-builder-map.md) and defining a hierarchy between `Map`, `Apply` and `Bind` and some attributes for those methods to allow the creators of builders to opt-in to "optimisations" that pick the least powerful (and hence hopefully most efficient) desugaring.
* Using `Apply` in place of `Bind` only in those instances where `Bind` is not defined (no existing code should break, but this goes somewhat to the contrary of the previous proposal, which we may want to consider in the future)