// #NoMono #CodeGen #Optimizations
// Regression baseline for the StackGuard.Guard closure-elimination shape (dotnet/fsharp#20368):
// an inline Guard with an [<InlineIfLambda>] function whose rare slow path (RunOnNewStack) is a
// separate, non-inline method the function is handed to.
//
//   callDirect: direct application. The lambda body is inlined on the common path; the closure
//               'newobj' appears only in the cold else-branch (reached only when the stack is
//               insufficient), so the common path allocates nothing.
//   callPiped:  '<|' eta-expands the member operand, which defeats [<InlineIfLambda>] and hoists
//               the closure 'newobj' to method entry -> allocated on every call. Kept as a contrast
//               so a regression that makes callDirect look like callPiped shows up in the baseline.
module StackGuardInlineIfLambda

open System.Runtime.CompilerServices

type StackGuard() =
    // The lambda escapes into this non-inline method, so it must be materialized as a closure -
    // but only where the call happens (the cold branch), never on the common path.
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member _.RunOnNewStack(f: unit -> 'T) : 'T = f ()

    member inline this.Guard([<InlineIfLambda>] f: unit -> 'T) : 'T =
        if RuntimeHelpers.TryEnsureSufficientExecutionStack() then f ()
        else this.RunOnNewStack f

let callDirect (sg: StackGuard) (env: int) : int = sg.Guard(fun () -> env + 1)

let callPiped (sg: StackGuard) (env: int) : int = sg.Guard <| fun () -> env + 1
