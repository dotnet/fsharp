namespace Foo

type ProcessError =
    | CannotStart of string * exn
    | DoesNotExist of int
