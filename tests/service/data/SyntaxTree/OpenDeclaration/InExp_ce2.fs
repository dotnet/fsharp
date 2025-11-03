let q = 
    query {
        open type System.Linq.Enumerable
        for i in Range(1, 10) do
            open type int
            yield MinValue + i
    } |> Seq.toArray
