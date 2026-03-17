// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// There should be no method-name duplication

// we need to ensure there are no collisions between (for example)
// - ``IB<GlobalType>`` (non-generic)
// - IB<'T> instantiated with 'T = GlobalType
// This is only an issue for types inside the global namespace, because '.' is invalid even in a quoted identifier.
// So if the type is in the global namespace, prepend 'global`', because '`' is also illegal -> there can be no quoted identifer with that name.

// without the prefix, the compiler would error out with
//> output error FS2014: A problem occurred writing the binary 'FsTest.exe': Error in pass2 for type C, error: duplicate entry 'IB<GlobalType>.X' in method table

namespace global

type GlobalType() = class end


type ``IB<GlobalType>`` =
    interface 
        abstract X : unit -> int
    end

type IB<'a> =
    interface 
        abstract X : unit -> int
    end

type C() = 
    interface ``IB<GlobalType>`` with
        member x.X() = 1
    interface IB<GlobalType> with
        member x.X() = 2
    
module M =
   
    let c = C()
    let x1 = (c :> ``IB<GlobalType>``).X()
    let x2 = (c :> IB<GlobalType>).X()

    if x1 <> 1 then
        failwithf "expected 1, but got %i" x1
    
    if x2 <> 2 then
        failwithf "expected 2, but got %i" x2

