namespace Project

type DU = A | B

// See https://github.com/Microsoft/visualfsharp/issues/237
namespace Foo

type Name = Name of string

namespace Bar

open Foo
type Person = Person of Name
