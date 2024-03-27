type Option<'T> =
    | None:       'T option
    | Some: Value:'T -> 'T option 