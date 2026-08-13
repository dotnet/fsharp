namespace LegacyInline

type Lens<'a, 'b> =
    ('a -> 'b) * ('b -> 'a -> 'a)

type Prism<'a, 'b> =
    ('a -> 'b option) * ('b -> 'a -> 'a)

module Library =
    type Set =
        | Set with
            static member (^=) (Set, (_, set): Lens<'a, 'b>) =
                fun value -> set value

            static member (^=) (Set, (_, set): Prism<'a, 'b>) =
                fun value -> set value

    let inline invoke optic value =
        (Set ^= optic) value
