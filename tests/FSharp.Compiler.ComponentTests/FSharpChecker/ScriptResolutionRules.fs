module FSharpChecker.ScriptResolutionRules

open System.Threading.Tasks
open Xunit
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.CodeAnalysis

// Checked with script resolution rules, a namespace starting with an F# Interactive fragment name (FSI_0001) is
// implicitly opened without that prefix, so the sibling module N is not in scope in M.
let private fsiFragmentNamespace name useScriptResolutionRules =
    { SyntheticProject.Create(name: string) with
        SourceFiles =
            [ { sourceFile "N" [] with Source = "module FSI_0001.Outer.N\nlet x = 1" }
              { sourceFile "M" [] with Source = "module FSI_0001.Outer.M\nlet y = N.x" } ]
        AutoAddModules = false
        UseScriptResolutionRules = useScriptResolutionRules }

let private errorsInM (checker: FSharpChecker) (project: SyntheticProject) =
    async {
        do! saveProject project false checker
        let! results = checkFile "M" project checker

        return (getTypeCheckResult results).Diagnostics |> filterErrors |> Array.map _.Message
    }

/// The framework imports, and the TcGlobals with them, are cached per framework set, so the project that fills the
/// cache must not decide whether the projects checked after it use script resolution rules.
[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``each project on a shared framework set is checked with its own script resolution rules`` (useTransparentCompiler: bool, scriptFirst: bool) : Task =
    task {
        let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
        let script = fsiFragmentNamespace "Script" true
        let plain = fsiFragmentNamespace "Plain" false

        let scriptIsChecked =
            async {
                let! errors = errorsInM checker script
                Assert.Contains("'N'", Assert.Single errors)
            }

        let plainIsChecked =
            async {
                let! errors = errorsInM checker plain
                Assert.Empty errors
            }

        if scriptFirst then
            do! scriptIsChecked
            do! plainIsChecked
        else
            do! plainIsChecked
            do! scriptIsChecked
    }
