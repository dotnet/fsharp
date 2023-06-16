module Foo

type Foo() =
    member f.X with internal get (key1, key2) = true and private set (key1, key2) value = ()
    member internal f.Y with get (key1, key2) = true and private set (key1, key2) value = ()
