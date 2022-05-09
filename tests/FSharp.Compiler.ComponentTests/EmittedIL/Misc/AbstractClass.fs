// regression test for https://github.com/dotnet/fsharp/issues/420

[<AbstractClass>]
type X public (i : int) =
    internal new() = X(1)
    private new(f : float32) = X(1)

exit 0