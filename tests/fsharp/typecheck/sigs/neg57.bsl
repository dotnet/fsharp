
neg57.fs(4,6,4,9): typecheck error FS0314: The type definitions for type 'Foo' in the signature and implementation are not compatible because the field 'offset' was present in the implementation but not in the signature. Struct types must now reveal their fields in the signature for the type, though the fields may still be labelled 'private' or 'internal'.

neg57.fs(1,8,1,9): typecheck error FS0193: Module 'M' requires a value 'new: unit -> Foo<'T>'
