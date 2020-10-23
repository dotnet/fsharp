type ImmutableStack<'a> private(items: 'a list) = 
   
    member this.Push item = ImmutableStack(item::items)
    member this.Pop = match items with | [] -> failwith "No elements in stack" | x::xs -> x,ImmutableStack(xs)
    
    // Notice type annotation is commented out, which results in an error
    new(col (*: seq<'a>*)) = ImmutableStack(List.ofSeq col)
