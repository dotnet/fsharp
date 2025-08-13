// Expected: No warning - keywords in strings are not declarations
module Module

type MyClass() =
    let message = "This string contains type and module keywords"
    let code = """
        type Example = int
        module Sample = 
            let x = 1
    """
    member _.GetMessage() = message
    member _.GetCode() = code
