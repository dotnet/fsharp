
type Foo =
    static member ReadWrite2 
        with set  x = lastUsed <- ("ReadWrite2", x)
        and  get () = lastUsed <- ("ReadWrite2", 0); 4
