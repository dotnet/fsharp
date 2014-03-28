
namespace Foo
  [<CompilationRepresentationAttribute
    (enum<CompilationRepresentationFlags> (8))>]
  type DU =
    | A
    | B of string
    with
      override ToString : unit -> string
    end

namespace Foo2
  type DU =
    | A
    | B of string
    with
      override ToString : unit -> string
    end