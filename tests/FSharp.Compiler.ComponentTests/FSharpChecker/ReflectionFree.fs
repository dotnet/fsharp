module FSharpChecker.ReflectionFree

open System.Threading.Tasks
open Xunit
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.CodeAnalysis

let private usingPercentA name otherOptions =
    { SyntheticProject.Create(name, { sourceFile name [] with Source = "let s = sprintf \"%A\" 1" }) with
        OtherOptions = otherOptions }

let private errorsIn (checker: FSharpChecker) (project: SyntheticProject) =
    async {
        do! saveProject project false checker
        let! results = checkFile project.SourceFiles.Head.Id project checker

        return (getTypeCheckResult results).Diagnostics |> filterErrors |> Array.map _.Message
    }

/// The framework imports, and the TcGlobals with them, are cached per framework set, so the project that fills the
/// cache must not decide `--reflectionfree` for the projects checked after it.
[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``each project on a shared framework set is checked with its own --reflectionfree`` (useTransparentCompiler: bool, reflectionFreeFirst: bool) : Task =
    task {
        let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
        let reflectionFree = usingPercentA "ReflectionFree" [ "--reflectionfree" ]
        let plain = usingPercentA "Plain" []

        let reflectionFreeIsChecked =
            async {
                let! errors = errorsIn checker reflectionFree
                Assert.Contains("--reflectionfree", Assert.Single errors)
            }

        let plainIsChecked =
            async {
                let! errors = errorsIn checker plain
                Assert.Empty errors
            }

        if reflectionFreeFirst then
            do! reflectionFreeIsChecked
            do! plainIsChecked
        else
            do! plainIsChecked
            do! reflectionFreeIsChecked
    }

/// A referenced project checked inside its dependent's check must not inherit the dependent's `--reflectionfree`.
[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``a referenced project is not checked with its dependent's --reflectionfree`` (useTransparentCompiler: bool) : Task =
    task {
        let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
        let library = usingPercentA "Library" []

        let app =
            { SyntheticProject.Create("App", { sourceFile "App" [] with Source = $"let t : string = {library.Name}.ModuleLibrary.s" }) with
                DependsOn = [ library ]
                OtherOptions = [ "--reflectionfree" ] }

        let! appErrors = errorsIn checker app

        Assert.Empty appErrors
    }
