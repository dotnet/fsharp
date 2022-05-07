// #Regression #Conformance #DataExpressions #ObjectConstructors 
// Regression test for FSharp1.0:4593 - Internal compiler error on typechecking object expressions with duplicate overrides

//<Expects id="FS0359" status="error" span="(16,11-19,12)">More than one override implements 'Next: StrongToWeakEntry<'a> array -> int when 'a: not struct'</Expects>

#light

[<AbstractClass>]
type BaseHashtable<'Entry, 'Key>(initialCapacity) =
    abstract member Next : entries : array<'Entry> -> int

[<Struct>]    
type StrongToWeakEntry<'Value when 'Value : not struct> =
    val mutable public next : int

let f() = { new BaseHashtable<_,_>(2) with
            override this.Next (entries:array<StrongToWeakEntry<_>>) = 1
            override this.Next entries = 1
          }
