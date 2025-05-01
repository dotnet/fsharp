module V

type 'a Foo =
    {
        Bar: 'a array
        D: int
    }
    member _.Make1<'b> (array: 'a array) : 'a Foo = failwith "meh"
    member _.Make2<'a> (array: 'a array) : 'a Foo = failwith "meh"
