namespace Internal.Utilities.Library

[<RequireQualifiedAccess>]
module internal AsyncLazy =

    /// Allows to specify the language for error messages
    val SetPreferredUILang : preferredUiLang: string option -> unit

/// Lazily evaluate the computation asynchronously, then strongly cache the result.
/// Once the result has been cached, the computation function will also be removed, or 'null'ed out, 
///     as to prevent any references captured by the computation from being strongly held.
/// The computation will only be canceled if there are no outstanding requests awaiting a response.
[<Sealed>]
type internal AsyncLazy<'T> =

    new : computation: Async<'T> -> AsyncLazy<'T>

    member GetValueAsync: unit -> Async<'T>

    member TryGetValue: unit -> 'T voption

    member RequestCount: int