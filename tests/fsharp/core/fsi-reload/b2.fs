namespace Project

type B = { Prop : DU }

// See https://github.com/Microsoft/visualfsharp/issues/237
namespace Foo2

type Name = Name of string

namespace Bar2

open Foo2
type Person = Person of Name
