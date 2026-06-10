module internal FSharp.Compiler.CompilerEmitHookState

open FSharp.Compiler.CompilerConfig

/// Resolve the emit hook from explicit config only, defaulting to no-op.
/// This keeps hot reload behavior strictly opt-in per compilation invocation.
let resolveCompilerEmitHook (explicitHook: ICompilerEmitHook option) =
    explicitHook
    |> Option.defaultValue defaultCompilerEmitHook
