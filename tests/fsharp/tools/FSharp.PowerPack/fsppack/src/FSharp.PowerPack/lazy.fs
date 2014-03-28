module Microsoft.FSharp.Compatibility.Lazy

type 'a t = 'a Microsoft.FSharp.Control.Lazy

let force (x: Microsoft.FSharp.Control.Lazy<'T>) = x.Force()
let force_val (x: Microsoft.FSharp.Control.Lazy<'T>) = x.Force()
let lazy_from_fun f = Microsoft.FSharp.Control.Lazy.Create(f)
let create f = Microsoft.FSharp.Control.Lazy.Create(f)
let lazy_from_val v = Microsoft.FSharp.Control.Lazy.CreateFromValue(v)
let lazy_is_val (x: Microsoft.FSharp.Control.Lazy<'T>) = x.IsValueCreated
