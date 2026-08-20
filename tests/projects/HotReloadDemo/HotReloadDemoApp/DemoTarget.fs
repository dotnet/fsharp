namespace HotReloadDemo.Target

/// <summary>
/// Baseline code used by the hot reload demo. Update the return value or body of
/// <c>Demo.GetMessage</c>, then return to the console and press Enter to emit a
/// delta. Stick to method-body edits (no signature changes) to avoid rude edits.
/// </summary>
module Demo =
    let mutable private counter = 0

    let GetMessage() =
        counter <- counter + 1
        $"Hello from generation 0 (invocation #{counter})"
