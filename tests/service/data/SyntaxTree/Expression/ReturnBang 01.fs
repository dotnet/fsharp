module Module

let _ =
    async {
        return! new MyType() : IDisposable
    }

