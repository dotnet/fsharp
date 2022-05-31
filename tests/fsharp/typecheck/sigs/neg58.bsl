
neg58.fs(5,6,5,9): typecheck error FS0314: The type definitions for type 'Foo' in the signature and implementation are not compatible because the field 'offset' was present in the implementation but not in the signature. Struct types must now reveal their fields in the signature for the type, though the fields may still be labelled 'private' or 'internal'.

neg58.fs(9,17,9,30): typecheck error FS0034: Module 'FooBarSoftware' contains
    override Foo.GetEnumerator: unit -> IEnumerator<'T>    
but its signature specifies
    member Foo.GetEnumerator: unit -> IEnumerator<'T>    
The compiled names differ
