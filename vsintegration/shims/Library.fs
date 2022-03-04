namespace shims

module Say =
    let hello name =
        printfn "Hello %s" name
