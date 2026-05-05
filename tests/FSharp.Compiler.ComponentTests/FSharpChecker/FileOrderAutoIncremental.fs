module FSharpChecker.FileOrderAutoIncremental

// Incremental-compilation coverage for --file-order-auto+. Mirrors the
// CommonWorkflows.fs scenarios but with each project configured for
// auto-mode. The point is to confirm auto-mode does not regress any
// of the IncrementalBuilder invariants the upstream tests guard.
//
// Each test sets OtherOptions = [ "--file-order-auto+" ], which the
// IncrementalBuilder hook (Track 05 Phase 2) reads to enable
// dependency-ordered file scheduling.

open Xunit
open FSharp.Test.ProjectGeneration

let private withAutoOrder (p: SyntheticProject) =
    { p with OtherOptions = "--file-order-auto+" :: p.OtherOptions }

// ── Misordered project — auto-mode unscrambles the topological order ──

[<Fact>]
let ``misordered project type-checks under --file-order-auto+`` () =
    // "Last" depends transitively on "First", but is listed BEFORE its deps
    // in SourceFiles. Without the flag, this would fail with the classic
    // "X is not defined". With auto-order, dependencies are sorted and the
    // project type-checks cleanly.
    let project =
        SyntheticProject.Create(
            { sourceFile "Last" ["Second"; "Third"] with EntryPoint = true },
            sourceFile "Second" ["First"],
            sourceFile "Third" ["First"],
            sourceFile "First" [])
        |> withAutoOrder

    project.Workflow {
        checkFile "Last" expectOk
    }

// ── Edit propagation matches manual mode ──

let private makeAutoProject () =
    SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        { sourceFile "Last" ["Second"; "Third"] with EntryPoint = true })
    |> withAutoOrder

[<Fact>]
let ``edit + check + dependent re-check under --file-order-auto+`` () =
    makeAutoProject().Workflow {
        updateFile "First" breakDependentFiles
        checkFile "First" expectSignatureChanged
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``transitive dependency check under --file-order-auto+`` () =
    makeAutoProject().Workflow {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Last" expectSignatureChanged
    }

[<Fact>]
let ``signature file shields dependents from impl-only changes under auto-mode`` () =
    (makeAutoProject()
     |> updateFile "First" addSignatureFile
     |> projectWorkflow) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectNoChanges
    }

// ── Graph mutations: add / remove file ──

[<Fact>]
let ``adding a file above a misordered file builds under --file-order-auto+`` () =
    // Listing position of the new file is irrelevant under auto-mode;
    // only its declared deps matter. Adding "New" with no deps then
    // making "Second" depend on it should work even though "New" lives
    // below files that already use it after the move.
    makeAutoProject().Workflow {
        addFileAbove "Second" (sourceFile "New" [])
        updateFile "Second" (addDependency "New")
        saveAll
        checkFile "Last" expectOk
    }

[<Fact>]
let ``removing a file leaves dependents broken under --file-order-auto+`` () =
    // "Second" is depended on by "Last". Removing "Second" should fail
    // checking "Last" exactly as it does in manual mode.
    makeAutoProject().Workflow {
        removeFile "Second"
        saveAll
        checkFile "Last" expectErrors
    }

// ── Cross-file dependency edge addition ──

[<Fact>]
let ``adding a dependency edge picks up the new edge in next check`` () =
    // "Third" doesn't initially depend on "Second". Edit it to add the
    // dependency; "Third" should see Second's surface on next check.
    makeAutoProject().Workflow {
        updateFile "Third" (addDependency "Second")
        saveFile "Third"
        checkFile "Third" expectOk
    }

// ── fsproj reorder is a no-op under auto-mode ──

[<Fact>]
let ``moving a file in fsproj order is a no-op under --file-order-auto+`` () =
    // Under manual mode, moveFile changes the type-check outcome (the
    // "wrong order" failure). Under auto-mode the dependency graph is
    // recomputed, so the move has no observable effect on type checking.
    makeAutoProject().Workflow {
        moveFile "First" 2 Down  // shove "First" down past "Second" and "Third"
        saveAll
        checkFile "Last" expectOk
    }
