open Prelude

module TestStructRecord =
    [<Struct>]
    type AnItem =
        { Link: string }

    let link item =
        { item with Link = "" } 