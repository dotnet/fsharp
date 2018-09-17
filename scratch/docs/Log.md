# F# Contribtions Log

## 2018-09-03
Got set up with Toby's help. I put together his key points below. There are a few different getting started guides that I can find, but Toby pointed me to [DEVGUIDE.md](https://github.com/Microsoft/visualfsharp/blob/master/DEVGUIDE.md) which seems to be the most helpful.

### Notes for Getting Started
1. git clone https://github.com/Microsoft/visualfsharp - There are multiple mirrors, forks, ports, etc, but this is the one you fork and make changes to. There are dev branches, but you probably want to branch from master is this is a new feature. You'll find `DEVGUIDE.md` in the root of that repo.
1. To build, you'll want to run the `build.cmd` script (it sets up some env vars etc, then kicks off msbuild) in the root of the repo, **but** you don't want to run it with `cmd`! You'll want to use the "Developer Command Prompt for VS 2017" and run that as admin. The F# contributors generally seem to refer to this as just "the admin prompt" so don't be confused by that. On my machine, I can find the developer prompt by hitting the Windows key and typing it's name. No doubt you can find it alongside your VS install if Windows search disappoints.
1. Before you can open the F# compiler in Visual Studio, you'll probably to grab a few more SDKs and targeting packs. The F# compiler is multi-target, that is, you can build binaries with the compiler that target a different framework version to that used to build the compiler itself. As such, to build the compiler, you need to pull in a ton of extra framework stuff. It was an additional 3GB or so for me. I used Windows search to find "Visual Studio Installer", hit "Modify" on my VS install, opened the "Individual Components" tab and grabbed _everything_ under the ".NET" heading.
1. Now you can try opening `FSharp.sln`, which you'll find in the root of the repo. You'll also see `VisualFSharp.sln` file in there, but that includes the Visual Studio integrations etc, I think, and is much bigger. You probably won't need it if you're working on core compiler stuff.
1. To test making changes to the compiler and explore what it does, Toby recommended the following:
    * Create an example `.fsx` file to build whatever you are insterested in
    * Set `Fsc` as the start-up project, with your `.fsx`'s path as its only argument
    * Run `Fsc` from inside VS, with will build only the dependencies you require, then will kick off compilation of your script.
    * Debug your version of the compiler or whatever to see what's going on (in my case, I have a small computation expression builder which includes various custom operators, etc)

### Interesting Jumping Off Points
* Docs (in no particular order):
    * Toby's thesis
    * https://github.com/Microsoft/visualfsharp/blob/master/DEVGUIDE.md
    * https://github.com/fsharp/fslang-suggestions/issues/579
    * https://github.com/fsharp/fslang-suggestions/issues/36
    * https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/computation-zoo.pdf
    * http://www.staff.city.ac.uk/~ross/papers/Applicative.pdf
    * http://delivery.acm.org/10.1145/2980000/2976007/p92-marlow.pdf
    * https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
    * https://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html
    * https://fsharp.github.io/2016/09/26/fsharp-rfc-process.html
* The `MatchBang` implementation pull request - https://github.com/Microsoft/visualfsharp/pull/4427
* `TcComputationExpression` in `TypeChecker.fs`
* `LetOrUseBang` usages
* `fsharpqafiles` - how does this all get picked up for running the tests?

## 2018-09-04
Explored the compiler, documentation and surrounding literature.

[Tomas Petricek's "The F# Computation Expression Zoo"](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/computation-zoo.pdf), linked by Don Syme in the `let! ... and! ...` fslang suggestion is very helpful because it defines much of the core of the change very clearly (see part 3, "Semantics of computation expressions", and part 4.4). Interestingly, Petricek seems to prefer the less common "Monoidal" definition of applicatives (see Part 4 of McBride & Patersons's ["Applicative programming with effects"](http://www.staff.city.ac.uk/~ross/papers/Applicative.pdf) and subsequent [explanations by people on the internet](https://argumatronic.com/posts/2017-03-08-applicative-instances.html)) that defines `map` and `merge` rather than `pure` and `apply`. I am starting to wonder now... should we allow `let! ... and! ...` when the user has only defined bind? I suppose it'd allow for future implementations of `pure` & `apply` / `map` & `merge` to make it more efficient (providing the relvant laws relating monads and applicatives hold).

So now we have prior art for some of both the theory (papers from Petricek and McBride & Patterson on applicatives) and the practice (`match!` pull request, existing `zip` custom operation, Petricek's - rejected! - earlier work on joinads).

Questions:
* Why does Petricek prefer the monoidal (semigroupal?) definition of applicatives? What different does it make which we pick?
    * `merge : f a * f b -> f (a*b)` which is a bit odd because it involves creating a tuple, which doesn't seem very elegant (does it imply allocating a tuple for each application of a value?!). Previously I was wondering about `liftA2 : (a -> b -> c) -> f a -> f b -> f c`, although that wouldn't require `map` since `let map f x = merge (<|) (pure f) x` by that definition, so the `map` would just be optional as an optimisation, just as adding both `merge` and `map` could be an optimisation for when we already have `bind`. I guess we don't have curried functions when defining builders? I'll have to check.
* Why was Petricek's work on joinads rejected by Don Syme? Am I at risk of falling into the same trap?
* Should we allow mixing applicatives and monads, as long as the rules that no RHS of an apply is the LHS of a `let!`?
* What should we do if someone tries to use `let! ... and! ...` if the builder only defines `Bind`?
* Is using `map` and `merge` perferable because we get `map` which gives an optimisation for some usages of existing monadic CEs?
* `map` & `merge` / `apply` vs. `zip`?

Standard place for tests (see `match!` PR) seems to be: https://github.com/Microsoft/visualfsharp/tree/master/tests/fsharp/core
`match!` commit: https://github.com/Microsoft/visualfsharp/commit/e33831a596bc2858c9311280212b559318272ee4

### Next Up
* Answer questions from today (I've already made a post to #langdesign)
* Try to understand the `match!` commit by reading/debugging
* Trying to change the `let!` syntax, e.g. to `foo!` and see it work

## 2018-09-05
Still waiting on a response on #langdesign.

Good news! The [RFC process](https://fsharp.github.io/2016/09/26/fsharp-rfc-process.html) isn't very heavyweight.

After reading the [Extend ComputationExpression builders with Map](https://github.com/fsharp/fslang-design/issues/258) discussion, I am wondering how `pure` will work.

Parts of compiler to explore following on from the `match!` PR:
* src/fsharp/LexFilter.fs
* src/fsharp/TypeChecker.fs
* src/fsharp/ast.fs
* src/fsharp/lex.fsl
* src/fsharp/pars.fsy
* src/fsharp/service/ServiceLexing.fs
* src/fsharp/service/ServiceParseTreeWalk.fs
* src/fsharp/service/ServiceUntypedParse.fs

### Messing with the `match!` syntax

Interesting existing annoyance, no idea if it's easily fixable: I tried to use a `match!` to do some `printf`-ing, but got the "You haven't defined `Zero`" error. Okay, so I implement that, then I get the same for `Combine`. I'd have liked to have had all of the errors at once to avoid multiple build & run iterations.

...Now I need a `Delay` too! 😄

Managed to mangle the new `match!` syntax to use `letmatch!` on a branch.

```fsharp
let x = Some 1
let y = opt { return "A" }
let z = Some 3.5

let foo : string option =
    opt {
        let! x' = x
        letmatch! y with
        | yValue -> printfn "y was set to %s!" yValue
        let! z' = z
        return sprintf "x = %d, z = %f" x' z'
    }
```

The semantics are iff `y` is `None` then bomb out of the CE with `None` else continue down the CE, printing the "y was set..." stuff to `stdout`.

### Have a go at adding a shonky `let! ... and! ...`

Arbitrary list of things to do / remember:
* Update AST (not sure how the AST/TAST handle custom operators and built-in CE  methods yet)
* Update TAST
* Add `and!` to the frontend
* Add logic for grabbing `.Apply` off the given builder and validating its signature etc (won't both with `.Map` or `.Merge` at this point because that's yet more functions to have to wire up, validate, etc)
* Should there be an `andUse!` or something? What usecases could there be? What would that look like?

What I've done so far makes `and!` only valid within a `let!`:
```fsharp
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    SynAndBangExpr = 
    /// AndBang(isUse, bindings, body, wholeRange)
    ///
    /// F# syntax: and! pat = expr in expr (Must follow on directly from a let! / and!)
    | AndBang of bindSeqPoint:SequencePointInfoForBinding * isUse:bool * isFromSource:bool * SynPat * SynExpr * SynAndBangExpr * range:range
    | NotAndBang of SynExpr
and
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    SynExpr =

(* ... *)

    /// SynExpr.LetOrUseBang(spBind, isUse, isFromSource, pat, rhsExpr, bodyExpr, mWholeExpr).
    ///
    /// F# syntax: let! pat = expr in expr'
    /// F# syntax: use! pat = expr in expr'
    /// where expr' admits an immediate and!
    /// Computation expressions only
    | LetOrUseBang    of bindSeqPoint:SequencePointInfoForBinding * isUse:bool * isFromSource:bool * SynPat * SynExpr * SynAndBangExpr * range:range
```

This is kind of what we want, but its type safety is actually a bit annoying - now `and!`s are a different expression type, and the normal functions, e.g. to test whether something is a control-flow expression, doesn't directly apply to it. I feel like this will need revisiting - will we want a `let!` followed by a list of `and!`s (, i.e. the string "and!" could be like an expression list separator - see `listPatternElements` in `pars.fsy`, started with a "let!" and terminated with a "return". If the list is non-empty => its an applicative expression)?

## 2018-09-06

I am trying to work off `type ... and ...` and `let ... and ...` to work out how to make this fit `let!`. Looks like there some logic in `lexfilter.fs` for mapping from light to verbose syntax that I'll want to understand well.

### Light And Verbose Syntax

Why, CE section of the parser, do we have a rule for `BINDER` when `BINDER`s should have been mapped to `OBINDER`s by `lexfilter.fs`

I should probably get to grips with fslex and fsyacc in a more principled way:
http://courses.softlab.ntua.gr/compilers/2015a/ocamllex-tutorial.pdf
http://courses.softlab.ntua.gr/compilers/2015a/ocamlyacc-tutorial.pdf

> tomd [2:12 PM]  
Question about parsing `let!`:
Is `lexfilter.fs` mean to transform all instaces of `BINDER` to `OBINDER`? I would have naively assumed light syntax works by mapping first to verbose syntax. Yet, `pars.fsy` still seems to have rules for `BINDER` - why is this?

> dsyme [2:13 PM]  
@tomd use of light syntax is optional, so BINDER is used for `#light "off"`

> tomd [2:15 PM]  
Okay, so in `lexfilter.fs` why do we map some `BINDER`s to `OBINDER`s? Is the light syntax the canonical one, in some sense?

> dsyme [2:16 PM]  
Yes, the syntaxes are effectively separate.  e.g. we could split pars.fsy into two parsers - one for `#light on` (the default, containing `OBINDER`) and one for #light off (containing `BINDER`)

### High-Level Questions

* Should `anduse!` be a thing? If so, what should its final name be?
* How should the ranges work for `let! ... and! ...` groups? For each binding range over the full chain of bindings and the final body? I'd kind of like just the current binding and the body, but ranges have to be continuous - it's baked in pretty deeply. I think I'll just do what let does and accept that it looks like earlier `and!`'s bindings are in scope of later ones.

### Low-Level Questions

#### Parsing complexity vs. AST complexity

The `binders` non-terminal awkwardly contains the final body with a `let!` rather than waiting until after the zero-or-more `and!`s. This makes creating the range more awkward. On the other hand, that keeps the structure of the AST largely as it is, whilst still preventing more `let!`s half way through. I think keeping the AST as it is so we don't get a new `and!` expression type wins here.

> Toby Shaw [6:57 PM]
I had naively only thought about the other way, but I like your idea more
catches the error in parsing, which is way nicer

> tomd [6:57 PM]  
Yep!

> Toby Shaw [6:58 PM]  
the only problem is that you might find it harder to give a descriptive error message

> tomd [6:58 PM]  
Shoe horning the latter way into parsing is hard too  
Yeah, I am no where near thinking about that too hard though  
Why do you say that?

> Toby Shaw [7:04 PM]  
the parser has auto-generated error messages, I think  
Whereas if you throw the error in type checking, then you can choose the error message

#### What is allowed to follow a `let! ... and! ...`?

It's probably worth giving some thought to what should be allowed to come after a `let! ... and! ...` chain. Only allowing `return` might be one simple way to get started?

## 2018-09-07

[Useful description](https://fsprojects.github.io/FSharpPlus/computation-expressions.html) of the various kinds of `monad`, as F# sees things.

The first thing I want to have work it the `option` applicative, which I think is a strict (no delaying required), monad-plus (plus in this case means take the first `Some` value) CE.

### Question to #general:

> tomd [2:43 PM]  
When writing a computation expression builder, is the choice in which of `Yield` and `Return` to implement purely down to whether you want the CE to make the `yield` or `return` keyword available? I can't really imagine a world where they're have different implementations (since they'd imply different monad instances, right?)  
>  
> The context of this question is working out how `pure` would work for `let! ... and! ...` computation expressions (I think `apply` is entirely new, so no prior art to decipher in that case).
>
> tomasp [11:10 PM]  
@tomd I think one reason was that `let! a = x and b = y` maps pretty directly to `Merge`, so the translation is simpler
and it also works nicely if you have something that is also a monad, because then you can translate this:  

```fsharp
m {
  let! a = x
  and b = y
  let! c = z
  return a + b + c }
```

> into this:  

```fsharp
m.Bind(m.Merge(x, y), fun (a, b) -> 
  m.Bind(z, fun c -> 
    m.Return(a + b + c) ) )
```
> For reference, if you do not have `Bind` and have just an applicative, you will not be able to do the second `let! c = z`. You can only write, say:

```fsharp
m {
  let! a = x
  and b = y
  return a + b }
```

> which can be translated using just `Map`:

```fsharp
m.Map(m.Merge(x, y), fun (a, b) -> a + b)
```

### Choosing `return` vs. `yield`

Assuming the difference is in which keyword becomes available inside te CE, as person writing a CE, I supppose I'd use `return` for plain old applicatives and `yield` for [alternative applicatives](https://hackage.haskell.org/package/base-4.6.0.1/docs/Control-Applicative.html#t:Alternative), and so on that basis, I think my change should use, say, support `return` initially and `yield` later (when the CE builder writer has defined `Combine`, which is really the alternation operator).
Alternative applicative support via `let! ... and! ...` and `yield` keywords would make writing things with alternatives, e.g. an argument parser, both efficient (no rebuilding the compuation on every application) and convenient (syntax is lightweight and readable).

### A Note on Alternation
`Combine`, the alternation method, is called when sequencing CE expressions. e.g. for options, we can pick the first in the `Some` case by just listing them inside the CE and providing a left-biased `Combine` method on the builder.

### Further uses of `Delay`
Note from @Nick: `Delay` is useful even in non-side-effectful CE builders as a way to "hide" evaluating a CE behind a thunk - useful when building the CE itself is expensive.

### Desugaring

Proposal for desugaring, a la [the existing CE docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions):

| Method  | Typical Signature(s)        | Description |
|---------|-----------------------------|-------------|
| `Apply` | `M<'T -> 'U> * M<'T> -> M<'U>` | Called for `let! ... and! ...` and computation expressions. Allows certain operations that do not require the full power of bind to be performed potentially more efficiently, or where they do not even have a sensible bind operation (but to have an apply) to be performed at all.

| Expression                    | Translation | And using operators... |
|-------------------------------|-------------|------------------------|
| `let! pattern1 = expr1 and! pattern2 = expr2 and! ... in cexpr` | `builder.apply(builder.apply(builder.apply(fun expr1 expr2 ... -> cexpr, expr1), expr2), ...)` | `cexpr <*> expr1 <*> expr2 <*> ... <*> exprN`

Given the above, I think it would _not_ require extra work to allow all of the other usual CE keywords to work alongside `let! ... and! ...`, since the desugaring only requires the context of what `and!`s correspond to what `let!`s and otherwise doesn't interact, which (right now) is easily done in the parser/AST.

This also means we can add optimisations to reduce current desugarings which use `Bind` to use the least powerful that applies of `Map`, `Apply` and `Bind`. Even in a world where these magic optimisations exist (where `let! ... and! ...` syntax wouldn't strictly be necessary for `Apply` to be called, the syntax would be handy because it would be a means for the CE user to assert "I expect this to be possible with only `apply`, I do _not_ want to be using `Bind` here", e.g. because of the runtime cost or because I don't want to care about whether or not `Bind` even exists on this builder).

## 2018-09-10

I don't think we need `in` between `let!`s and `and!`s in the same chain, because any later `and!` in unambiguously part of the chain (a new chain would need to be started with a fresh `let!`). EDIT: That was clearly rubbish, one example of that being wrong is a `let!` nested in another `let!`. Which does the following `and!` belong to without the offside rule or `in`? Seems ambiguous to me!

### Should we desugar to `Bind`, `Apply` or `Map`?

Right now, we can only ever desugar `let!` to a `Bind`. This isn't ideal because `Bind` can be expensive and can make statically examining the structure of the full computation impossible (the can't know what will happen "behind a bind" until a value is present for us to use to generate the subsequent computation).

If `Apply` is available on the builder, we can use it as a (potentially) more efficient alternative for `Bind` where the RHS of a `let!` does not make reference to the LHS of a previous `let!`. In a sense, it allows us to bind multiple names which are not interdependent simultaneously. It also allows us to apply functions on the LHS of a `let!` to values on the LHS of a `let!` without `Bind`. This is useful because implementing `Bind` isn't possible for as many interesting structures as for `Apply`.

Similarly, if `Map` is available, we can use is as an alternative to `Apply` and `Bind` if we take the value on the LHS of a `let!` and transform it with reference to the LHS of any other `let!` at any point. Again, `Map` is possible to implement for more structures than both `Bind` and `Apply`, and it is potentially more efficient to use too.

`Bind`, `Apply` and `Map` are related via various laws that govern how they "should" behave - i.e. we expect any "sensible" CE builder writer will produce methods for which these hold. For example, `builder.Bind(fun x -> builder.Return(f x))` and `builder.Apply(builder.Return(f), x)` should both produce the same result as `builder.Map(f, x)`.

The most minimal change we could make is to just make `let! ... and! ...` desugar to `Apply` as defined earlier. This makes CE builders useful for strutures for which we can't or haven't defined `Bind`. It also allows the more efficient `Apply` to be used even where `Bind` is defined, and potentially allows more static examination of the resulting value.

Beyond that, we could start more aggressively using the most constrained (and hence generally most efficient) desugaring we can. e.g. even if the CE writer does not explicitly use `let! ... and! ...`, we may notice that no LHS of a `let!` is used on the RHS of another, so we can desugar to `Apply` instead. If the laws hold, this would probably create a more efficient program which behaves identically, however, _it will do something different to the `Bind` desugaring if the laws don't hold_! e.g. If `Apply` calls `System.Environment.Exit()` but `Bind` does not, the program will do something drastically different. As such, the more aggressive, optimising desugarings are potentially dangerous so we probably want to consider these separately to the `let! ... and! ...` proposal. Interestingly, however, if a CE writer was using a builder which defined `Bind` but not `Apply` or `Map`, and later the builder was updated to implement `Apply` and `Map` in a way that held with respect to the laws, then without a change of their source code, they could get some new optimisations after recompiling.

Even in a world with these auto-magical, optimising desugarings, having the `let! ... and! ...` syntax is still useful, since it offers a way for the CE writer to declare "I expect this should be possible with `Apply` and therefore do not require `Bind`, so please verify that is true and use only `Apply`". This makes it possible to be confident that the desugaring will avoid using `Bind` e.g. because it is expensive.

Assuming we used the full suite of optimising desugarings (i.e. using `Map` in preference to `Apply`, in preference to `Bind` when possible), we would get this:

| Expression                    | Translation | Optimisation |
|-------------------------------|-------------|--------------|
|`let! pat1 = exp1 and! pat2 = exp2 in let! pat3 = exp1 in return pat1 + pat2 + pat3`| `builder.Apply(builder.Apply(builder.Apply(builder.Return(fun pat1 pat2 pat3 -> pat1 + pat2 + pat3), exp1), exp2), exp3)` | Use `Apply` rather than `Bind` despite final binding not being part of preceding `and!` chain.
| `let! x = y in return x + 1` | `builder.Map((fun x -> x + 1), y)` | Use `Map` rather than `Bind` or `Apply` despite `let!`

### `liftA2` vs. `apply` vs. `merge`

There's already been plenty of chat about the `apply` vs. `merge` way of doing things, but some people also claim `liftA2` is easier to write (`apply` often prompts the question "why would I end up with a function in my functor" and then people get hung up on that).

> From Hackage:  
> `liftA2 :: (a -> b -> c) -> f a -> f b -> f c`  
>  
> Lift a binary function to actions.  
>  
> Some functors support an implementation of `liftA2` that is more efficient than the default one. In particular, if fmap is an expensive operation, it is likely better to use `liftA2` than to fmap over the structure and then use `apply`. We can also look at `liftA2` are a generalised `Merge` that avoids arbitrarily creating a tuple. With `liftA2` we can nicely introduce it along the lines of "it's like `map`, but where we can have 2 parameters". Or maybe that obscures the essence of the contribution an applicative provides over a functor?

Comparing `liftA2` and `apply` because `merge` introduces a tuple, which I find arbitrary and inelegant.

```fsharp
val liftA2 : ('a -> 'b -> 'c) -> f 'a -> f 'b -> f 'c
```
```fsharp
val apply : f ('a -> 'b) -> f 'a -> f 'b
```
```fsharp
val merge : f 'a -> f 'b -> f('a * 'b)
```

Imagine a simple but presumably represenative example CE:
```fsharp
option {
    let! a = aExpr
    and! b = bExpr
    and! c = cExpr
    return a + b + c // We're explicitly returning here (i.e. calling `pure`, so this neatly maps into the `apply` desugaring - liftA2 presumes the function is unwrapped, so we'd be passing in (<|) to apply the lambda doing the summation anyway)
}
```

```fsharp
builder.Apply(
    builder.Apply(
        builder.Apply(
            builder.Return(fun a b c -> a + b + c),
            aExpr), 
        bExpr), 
    cExpr)
```

And with some extra body in the middle:
```fsharp
option {
    let! a = aExpr
    and! b = bExpr
    and! c = cExpr
    let d = 3 + 4
    return (a + b + c) * d
}
```
I presume it would desugar (have yet to confirm this is how `let`s work yet) as so:
```fsharp
builder.Apply(
    builder.Apply(
        builder.Apply(
            builder.Return(
                fun a b c ->
                    let d = 3 + 4
                    (a + b + c) * d),
            aExpr), 
        bExpr), 
    cExpr)
```

I assert `Apply` is the best way because the desugaring is most natural. The only unintuitive thing, perhaps, is that the first binding goes into the inner most `Apply` and we work outwards from there, which is the reverse of how you read the top-to-bottom CE.

### Answers from @dsyme

Why was joinads rejected? It made various things significantly more complicated, e.g. pattern matching.

`Map` can be added as an efficient desugaring of `let! x = y in return x + 1`, but `Map` will need an attribute on it to indicate the backwards incompatible optimisation is allowed by this builder.

Smart CE builder optimisations, such as swapping out simple `Apply` usages for `Map` where possible, etc, won't be accepted because it'll make the "simple" desugaring not so simple.

`Map` & `Merge` vs. `Pure` & `Apply` vs. `Pure` & `LiftA2`: The choice is largely arbitrary, pick whichever.

### Decision Record

* Nothing to worry about from the rejection of the joinads proposal - this change does not introduce the level of complexity that caused the issues in that case.

* I will use `apply` because I am most familiar with this encoding and I think it desugars very clearly.

* I will avoid `map` altogether - there is a separate suggestion and RFC for that and given @dsyme's points about not wanting more magic in the desugaring (e.g. using `apply` to implement `map` if `map` is not given explicitly, etc), these two changes should be orthogonal (although my work here is related in the sense that the skills needed to do one are very much like the skills needed to do the other).

## 2018-09-11

Beyond making `let! ... and! ...` appear to work, I'll need to:
* Come up with a comprehensive set of tests
* Preempt some common errors and make sure the messages are and least somewhat helpful
* Think about how to update the CE docs on MSDN and whatever hover-over tips VS gives, etc
* Explore the debugging, step-through experience
* Explore the "find definition" experience
* Chase down "The result of this equality expression is discarded..."
* Make sure both light and verbose syntax look sensible

Still trying to make the syntax parse properly. I suspect this is to do with the different syntaxes: https://fsharpforfunandprofit.com/posts/fsharp-syntax/

From the debugging: `LET: entering CtxtLetDecl(blockLet=%b), awaiting EQUALS to go to CtxtSeqBlock (%a)\n` when filtering `BINDER`. How does thhis need to change to accomodate `and!`? Need to explore `CtxtLetDecl` to get started.

Okay, I think this:
```fsharp
    //  let!  ... ~~~> CtxtLetDecl 
    | BINDER b, (ctxt :: _) -> 
        let blockLet = match ctxt with CtxtSeqBlock _ -> true | _ -> false
        if debug then dprintf "LET: entering CtxtLetDecl(blockLet=%b), awaiting EQUALS to go to CtxtSeqBlock (%a)\n" blockLet outputPos tokenStartPos
        pushCtxt tokenTup (CtxtLetDecl(blockLet,tokenStartPos))
        returnToken tokenLexbufState (if blockLet then OBINDER b else token)
```
is about runs of let bindings preserving the light syntax. `SeqBlock` is the name for a series of let-bindings in a row?

By the looks of it, the corollery is that I probably do want to support light and verbose syntax, and then I can largely copy the above lex filter stuff for `and!`.

[The spec](https://fsharp.org/specs/language-spec/4.0/FSharpSpec-4.0-latest.pdf) covers offside contexts (section 15.1.6, page 282).

> The `SeqBlock` context is the primary context of the analysis. It indicates a sequence of items that must be column-aligned. Where necessary for parsing, the compiler automatically inserts a delimiter that replaces the regular `in` and `;` tokens between the syntax elements.

Does `isLetContinuator` need `AND_BANG` and `OAND_BANG` adding to it? I think no, else we keep the offside position from the first `let!`?

Editor index lines and columns from 1. Compiler debug output indexs from 0. Consider putting in an issue for this bjorn and chethusk seem to be in favour.

For posterity, I am currently invoking the compiler as so:
`fsc.exe --parseonly --debug+ --debug-parse SimpleBuilder.fsx`

Once theory I have about what I was doing wrong was having `and!` mess with the offisde rule. Maybe it should work just like `and` does? That seems sensible intutively modulo that `and!`s exist "inside" the `let!` that precedes it. EDIT: Either way, my logic fails, so I guess the error is somewhere else in the lexer or the subsequent filtering. Is there a way to just print the tokens post-filtering?

`--tokenize` seems to be working okay. From what I can see, an `OBLOCKEND` is inserted before the `and!`. Is that correct?

```
offside token at column 8 indicates end of CtxtSeqBlock started at (34:12)!
<-- popping Context(seqblock(subsequent,(34:12))), stack = [let(true,(33:8)); seqblock(subsequent,(33:8)); paren; vanilla((32:4));
 seqblock(subsequent,(32:4)); let(true,(31:0)); seqblock(subsequent,(2:0))]
end of CtxtSeqBlock, insert OBLOCKEND
inserting OBLOCKEND
```

Looks like we get as far as reading the identifier after the `and!` too:
```
AND!: entering CtxtLetDecl(blockLet=true), awaiting EQUALS to go to CtxtSeqBlock ((35:8))
--> pushing, stack = [let(true,(35:8)); seqblock(subsequent,(33:8)); paren; vanilla((32:4));
 seqblock(subsequent,(32:4)); let(true,(31:0)); seqblock(subsequent,(2:0))]
tokenize - got OAND_BANG @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(36,8)-(36,12)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: delayed token, tokenStartPos = (35:15)
tokenize - got IDENT @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(36,13)-(36,14)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: delayed token, tokenStartPos = (35:15)
CtxtLetDecl: EQUALS, pushing CtxtSeqBlock
popNextTokenTup: no delayed tokens, running lexer...
--> insert OBLOCKBEGIN
--> pushing, stack = [seqblock(first,(36:12)); let(true,(35:8)); seqblock(subsequent,(33:8)); paren;
 vanilla((32:4)); seqblock(subsequent,(32:4)); let(true,(31:0));
 seqblock(subsequent,(2:0))]
 ```

 Then the rest, which seems to agree with what I see:
 ```
 pushing CtxtVanilla at tokenStartPos = (37:8)
tokenize - got YIELD @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(38,8)-(38,14)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: delayed token, tokenStartPos = (37:23)
tokenize - got IDENT @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(38,15)-(38,22)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: delayed token, tokenStartPos = (37:23)
tokenize - got STRING @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(38,23)-(38,39)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: delayed token, tokenStartPos = (37:42)
tokenize - got IDENT @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(38,40)-(38,41)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: delayed token, tokenStartPos = (37:42)
popNextTokenTup: no delayed tokens, running lexer...
popNextTokenTup: delayed token, tokenStartPos = (38:4)
tokenize - got IDENT @ C:\Users\Tom\Documents\FSharpContribs\visualfsharp\scratch\data\SimpleBuilder.fsx(38,42)-(38,43)
tokenize - getting one token from scratch\data\SimpleBuilder.fsx
popNextTokenTup: delayed token, tokenStartPos = (38:4)
IN/ELSE/ELIF/DONE/RPAREN/RBRACE/END at (38:4) terminates context at position (37:8)
```

So is the offside stuff adding/missing something weird that I've not noticed, or is the parsing logic not doing what I expect?

One conflict: `isLetContinuator` currently lets the `and!` be "inside" the `let!`, but the parser expects `hardwhiteDefnBindingsTerminator`, i.e. `ODECLEND`!

How much of a problem is it that `let! ... and! ...` is using right-recursion? (We seem to use it for `and`...)

Okay, I think it is time to take a step back. The easiest thing to make `and!` is presumably to emulate an existing construct. Up to and excluding parsing, `and!` works like `and` (after that there's a difference because, semantically, the `and!` is very much a part of the larger `let!` construct).

NFO `OAND_BANG` - there's no `OAND`!

One day lost to `build.cmd` not actually building everything. From what I can tell, you *must* use VS or msbuild to rebuild some artefacts.

## 2018-09-13

Parsing seem to at least accept my examples. Working on type checking now. One of the core issue right now if when to call `trans` and `translatedCtxt` on desugared CEs.

...which may, in turn, be due to me not updating `varSpace` correctly.

16.3.10 of the spec seems to correspond to this, so time for some bookwork!

Looks like I forgot to insert a call to `Return` between creating the lambdas that introduce the newly-bound names and calling `Apply`.

My example almost works now, in that it builds and runs, but I accidentally doubly wrap the result of the builder in the functor. I think the explicit `Return` the user gives needs to be "moved" to outside the lambdas, as opposed to kept and a "new" `Return` added between them and the calls to `Apply`. The solution is to think about precisely how the desugaring should work a bit more.

## 2018-09-17

I've been thinking about the slight annoyance of the desugaring that happens in `let! ... and! ...`, specifically, how the `Return` gets "lifted" to cover the whole block below the last `and!`, including whatever expression is handed to `Return`. The tricky examples I can think of right now are:

### Just moving `Return` to cover the lambda that represents everything after the last `and!` is hard enough!

```fsharp
option {
    let! (a,_)            = aExpr
    and! (_,b)            = bExpr
    and! (SingleCaseDu c) = cExpr
    let d = 3 + 4
    return (a + b + c) * d
}

// desugars to:

let aExprEvaluated = aExpr
let bExprEvaluated = bExpr
let cExprEvaluated = cExpr
let d = 3 + 4 // Code outside the `Return` does not have names bound via `let! ... and! ...` in scope
builder.Apply(
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (_,b) ->
                        (fun (SingleCaseDu c) ->
                            (a + b + c) * d)))),
            aExprEvaluated), 
        bExprEvaluated), 
    cExprEvaluated)
```

### What if the user calls `Yield` more than once? Do we disallow it? If we did allow it, what would the semantics/desugaring be? (This corresponds to "alternative applicatives", I think). Follow ups:
  * What if an active pattern appears on the LHS - do we call it up to (number of `yield` occurances) times?
  * What if `Return` isn't defined? Should we support semi-groups as well as monoids?

```fsharp
option {
    let! (a,_)            = aExpr
    and! (_,b)            = bExpr
    and! (SingleCaseDu c) = cExpr // Can the pattern be an active pattern? Does that mean it's potentially called once per occurance of `yield` below?
    yield (a + b + c) // `yield` implies alternation, i.e. a monoid (maybe we should support semi-groups too?)
    yield (b + 1)
    yield (a + c)
}

// desugars to:

let aExprEvaluated = aExpr
let bExprEvaluated = bExpr
let cExprEvaluated = cExpr

let alt1 =
    builder.Apply(
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun (a,_) ->
                        (fun (_,b) ->
                            (fun (SingleCaseDu c) ->
                                (a + b + c))))),
                aExprEvaluated), 
            bExprEvaluated), 
        cExprEvaluated)

let alt2 =
    builder.Apply(
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun (a,_) ->
                        (fun (_,b) ->
                            (fun (SingleCaseDu c) ->
                                (b + 1))))),
                aExprEvaluated), 
            bExprEvaluated), 
        cExprEvaluated)

let alt3 =
    builder.Apply(
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun (a,_) ->
                        (fun (_,b) ->
                            (fun (SingleCaseDu c) ->
                                (a + c))))),
                aExprEvaluated), 
            bExprEvaluated), 
        cExprEvaluated)

builder.Combine(
    builder.Combine(alt1,alt2),
    alt3)
```

Then thinking about more code outside of `yield` and `return` everythere it could conceivably be valid:

```fsharp
option {
    let! (a,_)            = aExpr
    and! (b,b2)           = bExpr
    and! (SingleCaseDu c) = cExpr
    
    let n = 7
    yield (a + b + c + n)
    let m = 100
    yield (b + m)
    let o = -1
    yield (a + b2 + c + n + m + o)
}

// desugars to:

let aExprEvaluated = aExpr
let bExprEvaluated = bExpr
let cExprEvaluated = cExpr

let n = 7

let alt1 =
    builder.Apply(
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun (a,_) ->
                        (fun (b,b2) ->
                            (fun (SingleCaseDu c) ->
                                (a + b + c + n))))),
                aExprEvaluated), 
            bExprEvaluated), 
        cExprEvaluated)

let m = 100 // By `let` binding here, we keep the expecting scoping and ordering of side-effects

let alt2 =
    builder.Apply(
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun (a,_) ->
                        (fun (b,b2) ->
                            (fun (SingleCaseDu c) ->
                                (b + m))))),
                aExprEvaluated), 
            bExprEvaluated), 
        cExprEvaluated)

let o = -1

let alt3 =
    builder.Apply(
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun (a,_) ->
                        (fun (b,b2) ->
                            (fun (SingleCaseDu c) ->
                                (a + b2 + c + n + m + o))))),
                aExprEvaluated), 
            bExprEvaluated), 
        cExprEvaluated)

builder.Combine(
    builder.Combine(alt1,alt2),
    alt3)

/// Where `alt1`, `alt2`, `alt3` are just fresh variables
```

### Nesting of applicative CEs & `yield!` / `return!`

```fsharp
option {
    let! (a,_) = aExpr
    and! (b,_) = bExpr
    
    yield a

    let n = 7

    yield! (
        option {
            let! c = cExpr
            and! d = dExpr
            return (b + c + d + n)
        })
}

// desugars to:

let aExprEvaluated = aExpr
let bExprEvaluated = bExpr

let alt1 =
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (b,_) ->
                        a))),
            aExprEvaluated), 
        bExprEvaluated)

let n = 7

let alt2 =
    builder.YieldFrom( // What would a sensible implementation be except `id`?
        let cExprEvaluated = cExpr
        let dExprEvaluated = dExpr
        builder.Apply(
            builder.Apply(
                builder.Return(
                    (fun c ->
                        (fun d ->
                            b + c + d + n))),
                cExprEvaluated), 
            dExprEvaluated))

builder.Combine(alt1, alt2)
```

### `let!` when `Apply` and `Return` are defined, but not `Bind`

**N.B. This is highly related to the `Map` RFC for `let! ... return ...`**

_If_ we chose to implement `Map` from `Apply` (in order to make a single `let!` for when `Apply` and `Return` are defined but not `Bind`), it could look like this:

```fsharp
option {
    let! (a,_) = aExpr
    let d = 3
    return (a * d)
}

// desugars to:

let aExprEvaluated = aExpr
let d = 3
builder.Apply(
    builder.Return(
        (fun (a,_) ->
            (a * d))),
    aExprEvaluated)

// This desugaring is just a corollary of the earlier, more complex desugarings
```

My gut says we should use `Map` if it is available, else `Apply`, else `Bind`.

### Active Patterns

```fsharp
let mutable spy = 0

let (|NumAndDoubleNum|) (x : int) =
    spy <- spy + 1
    (x, x + x)

option {
    let! (a,_) = aExpr
    and! (NumAndDoubleNum(b, doubleb)) = bExpr
    
    yield a + doubleb

    let n = 7

    yield a + b + n
}

// desugars to:

let aExprEvaluated = aExpr
let bExprEvaluated = bExpr

let alt1 =
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (NumAndDoubleNum(b, doubleb)) ->
                        a + doubleb))),
            aExprEvaluated), 
        bExprEvaluated)

let n = 7

let alt2 =
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (NumAndDoubleNum(b, doubleb)) ->
                        a + b + n))),
            aExprEvaluated), 
        bExprEvaluated)

builder.Combine(alt1, alt2)

// spy = 2, is this unacceptably surprising?
```

### Overall concepts for the full desugaring

One claim might be: _We should evaluate the RHS of each of the bindings only once_ - i.e. don't naively stamp out the expression n-times for every alternative implied by a `yield`. Although pattern matching has to happen for each `yield`, since until we are inside the `yield`'s lambda, we only have a functor in hand, and can't inspect the contents directly. This is only interesting since: pattern matching implies some runtime cost, and active patterns can have side-effects (arguably, that's enough of an abuse that it doesn't matter too much, but I don't like that it is perhaps surprising given the syntax of the CE).

Another perspective is that by "factoring out" the bindings, we've actually done away with some of the notion that the application implied by each `return` or `yield` is orthogonal, and it's everything else which is the exception, and active patterns are the ones following the rules and doing what is expected. From that perspective, `let! ... and! ...` can be simply considered as each `yield` and each `binding` happening separately/in parallel - of course that does make it easy to build something which looks terse but implies a very large computation (not that we can't do that already...).

Might need to consult the literature on this one, although the introduction of side-effects might be what is taking us off-piste.

#### "Commonising" approach

Potentially more efficient. Side-effects from active patterns happen N times, but normal `let`s and evaluation of the RHS of the `let! ... and! ...` happens only once. One could argue that, apart from the side-effect of active patterns, this more closely agrees with the syntax.

```fsharp
let mutable spy = 0

let (|NastySideEffectfulNumAndDoubleNum|) (x : int) =
    spy <- spy + 1
    (x, x + x)

option {
    let! (a,_) = aExpr
    and! (NastySideEffectfulNumAndDoubleNum(b, doubleb)) = bExpr
    
    yield a + doubleb

    let n = nastySideEffectfulComputation ()

    yield a + b + n
}

// desugars to:

let aExprEvaluated = aExpr
let bExprEvaluated = bExpr

let alt1 =
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (NastySideEffectfulNumAndDoubleNum(b, doubleb)) ->
                        a + doubleb))),
            aExprEvaluated), 
        bExprEvaluated)

let n = nastySideEffectfulComputation ()

let alt2 =
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (NastySideEffectfulNumAndDoubleNum(b, doubleb)) -> // N.B. That the RHS of the bindings are each evaluated once, but the pattern match is done once _per yield_
                        a + b + n))),
            aExprEvaluated), 
        bExprEvaluated)

builder.Combine(alt1, alt2)

// spy = 2, is this unacceptably surprising?
```

#### "Duplication" approach

Evaluation of names bound in the `let! ... and! ...` happens once for each yield (so potentially a lot of times!), but at least this agrees with the number of times the active pattern is called. One could argue that things happening for each `yield` is surprising given the syntax, but perhaps the fact that `and!` is there is enough to signal that we've shifted to a different "mode". I think this approach is simpler to implement and reason about.

Prior art in loops: If a function with side-effects exists in a loop, we don't automagically pull it out so it's only called once! Yes, you can shoot your foot off by leaving something nasty in a hot loop, but you can also factor it out and save yourself. At least it's clear what is going to happen.

```fsharp
let mutable spy = 0

let (|NastySideEffectfulNumAndDoubleNum|) (x : int) =
    spy <- spy + 1
    (x, x + x)

option {
    let! (a,_) = aExpr
    and! (NastySideEffectfulNumAndDoubleNum(b, doubleb)) = bExpr
    
    yield a + doubleb

    let n = nastySideEffectfulComputation ()

    yield a + b + n
}

// desugars to:

let alt1 =
    let n = nastySideEffectfulComputation () // N.B. This has been duplicated in each alternative, so everything happens as many times as a `yield` occurs in the CE
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (NastySideEffectfulNumAndDoubleNum(b, doubleb)) ->
                        a + doubleb))),
            aExpr), 
        bExpr)


let alt2 =
    let n = nastySideEffectfulComputation ()
    builder.Apply(
        builder.Apply(
            builder.Return(
                (fun (a,_) ->
                    (fun (NastySideEffectfulNumAndDoubleNum(b, doubleb)) ->
                        a + b + n))),
            aExpr), 
        bExpr) // N.B. Second evaluation of bExpr here

builder.Combine(alt1, alt2)

// spy = 2, but that should make sense if we imagine that `and!` signals, amongst other things, that each yield corresponds to another stamping out
```

#### "Just don't support yield" approach

If we support only `Return` and not `Yield` inside `let! ... and! ...` compuation expressions, then this problem goes away.

#### Rules
1. We create a structure of nested calls to `Apply` with a `Return` on the inside wrapping a lambda at every `return` or `yield` that appears in the CE. (We can create a function up-front which "wraps" any `Return` in the calls to `Apply` and bind that to a fresh variable for later calls to `Combine` if necessary, so that other scoping rules and orderings are preserved)
1. A `Map` implementation just falls out of the `Apply` desugaring we have, so I think we should keep that, but allow it to be overriden by an explicit `Map` definition that has been appropriately annotated (similarly, if there is an existing `Bind`, `Apply` needs an annotation to trump `Bind` in the case where both effectively implement `Map`).

#### Remaining Questions
* `do!` - how do we handle that with `Apply` and `Return`, etc.?
* I won't bother implementing support for semi-groups (i.e. when there are >1 usages of `yield` but no `Zero` defined), although I haven't actually checked what happens in that case right now...
* Need to check this vs. the existing implementation for `Bind`, but I assume an applicative CE should end in exactly one `return` or `return!`, else contain >= 1 `yield` or `yield!`, and the last line of the expression is a `yield` or `yield!`. Is that explicitly handled currently?
* `Using : 'T * ('T -> M<'U>) -> M<'U> when 'U :> IDisposable` i.e. kind of a `pure` and a `map` in one. Valid for use inside an `apply` as far as I can tell at first glance (in `use!` right now, we get a `Bind` with a `Using` nested directly inside it).