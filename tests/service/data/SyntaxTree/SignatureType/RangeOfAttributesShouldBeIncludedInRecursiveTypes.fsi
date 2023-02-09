
namespace SomeNamespace

type Foo =
    | Bar

and [<CustomEquality>] Bang =
    internal
        {
            LongNameBarBarBarBarBarBarBar: int
        }
        override GetHashCode : unit -> int
