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

## 2018-09-04
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

```F#
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
```F#
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

## 2018-09-05

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

## 2018-09-06

[Useful description](https://fsprojects.github.io/FSharpPlus/computation-expressions.html) of the various kinds of `monad`, as F# sees things.

The first thing I want to have work it the `option` applicative, which I think is a strict (no delaying required), monad-plus (plus in this case means take the first `Some` value) CE.

Questions to #general:
> tomd [2:43 PM]  
When writing a computation expression builder, is the choice in which of `Yield` and `Return` to implement purely down to whether you want the CE to make the `yield` or `return` keyword available. I can't really imagine a world where they're have different implementations (since they'd imply different monad instances, right?)  
>  
> The context of this question is working out how `pure` would work for `let! ... and! ...` computation expressions (I think `apply` is entirely new, so no prior art to decipher in that case).

Proposal for desugaring, a la [the existing CE docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions):

| Expression                    | Translation                            |
|-------------------------------|----------------------------------------|
| `let! pattern1 = expr1 in and! pattern2 = expr2 in cexpr` | `builder.apply()`