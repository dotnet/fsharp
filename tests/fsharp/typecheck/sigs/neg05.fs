module Test

type Foo = 
    interface 
        new() = 1 
    end

[<Struct>]
type N3 = 
    abstract M : int

[<Struct>]
type N4 = 
    interface
    end

[<Class>]
type N5 = 
    interface
    end

[<Struct>]
type N6 = 
    class
    end

[<Interface>]
type N7 = 
    class
    end

[<Class>]
type N8 = 
    struct
    end

[<Interface>]
type N9 = 
    struct
    end

type N15 =
    abstract M : int
    member x.P = x.M

let f (c: 'a when 'a : (new : unit -> 'a)) = new 'a("AAAA")



// NOTE: error message should appear on type variable declaration
type Test<'a> = class
     val mutable buffer: 'a[]
     val mutable idx: int

     member t.Dec() = t.idx <- t.idx - 1

     member t.Pop() =
         t.Dec()
         t.buffer.[t.idx]  // NOTE: <-- it would actually be better for it to appear here
         1
end



