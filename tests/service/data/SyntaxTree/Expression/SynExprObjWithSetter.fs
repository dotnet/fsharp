
[<AbstractClass>]
type CFoo() =
    abstract AbstractClassPropertySet: string with set

{ new CFoo() with
    override this.AbstractClassPropertySet with set (v:string) = () }
