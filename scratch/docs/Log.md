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
    * https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/computation-zoo.pdf
    * http://www.staff.city.ac.uk/~ross/papers/Applicative.pdf
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

So now we have prior art for some of both the theory (papers from Petricek and McBride & Patterson on applicatives) and the practice (`match!` pull request, Petricek's - rejected! - earlier work on joinads).

Questions:
* Why does Petricek prefer the monoidal (semigroupal?) definition of applicatives? What different does it make which we pick?
    * What is the signature of `merge` as Petricek intends it? (maybe `(a -> b -> c) -> f a -> f b -> f c`? Although that wouldn't require `map` since `let map f x = merge (<|) (pure f) x` by that definition, so the `map` would just be optional as an optimisation, just as adding both `merge` and `map` could be an optimisation for when we already have `bind`)
* Why was Petricek's work on joinads rejected by Don Syme? Am I at risk of falling into the same trap?