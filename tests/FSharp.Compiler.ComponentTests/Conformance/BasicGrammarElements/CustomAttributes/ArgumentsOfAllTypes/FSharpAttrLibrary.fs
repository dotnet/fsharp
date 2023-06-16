namespace FSAttributes

open System

[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Int16(p1 : System.Int16) =
    [<DefaultValue>]
    val mutable _n1 : System.Int16

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Int32(p1 : System.Int32) =
    [<DefaultValue>]
    val mutable _n1 : System.Int32

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Int64(p1 : System.Int64) =
    [<DefaultValue>]
    val mutable _n1 : System.Int64

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_UInt16(p1 : System.UInt16) =
    [<DefaultValue>]
    val mutable _n1 : System.UInt16

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_UInt32(p1 : System.UInt32) =
    [<DefaultValue>]
    val mutable _n1 : System.UInt32

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_UInt64(p1 : System.UInt64) =
    [<DefaultValue>]
    val mutable _n1 : System.UInt64

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Char(p1 : System.Char) =
    [<DefaultValue>]
    val mutable _n1 : System.Char

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Byte(p1 : System.Byte) =
    [<DefaultValue>]
    val mutable _n1 : System.Byte

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_SByte(p1 : System.SByte) =
    [<DefaultValue>]
    val mutable _n1 : System.SByte

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Single(p1 : System.Single) =
    [<DefaultValue>]
    val mutable _n1 : System.Single

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Double(p1 : System.Double) =
    [<DefaultValue>]
    val mutable _n1 : System.Double

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_String(p1 : System.String) =
    [<DefaultValue>]
    val mutable _n1 : System.String

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_DateTimeKind(p1 : System.DateTimeKind) =
    [<DefaultValue>]
    val mutable _n1 : System.DateTimeKind

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Type(p1 : System.Type) =
    [<DefaultValue>]
    val mutable _n1 : System.Type

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_Object(p1 : System.Object) =
    [<DefaultValue>]
    val mutable _n1 : System.Object

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)


[<AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)>]
type System_TypeArray(p1 : System.Type[]) =
    [<DefaultValue>]
    val mutable _n1 : System.Type[]

    inherit Attribute()
    member this._p1 = p1

    member this.N1 with get() = this._n1
                   and set(v) = System.Diagnostics.Debug.Assert(this._p1 = v)
                                this._n1 <- v
    (* * * * *)

[<System_Int16(10s, N1 = 10s)>]
[<System_Int32(2147483647, N1 = 2147483647)>]
[<System_Int64(9223372036854775807L, N1 = 9223372036854775807L)>]
[<System_UInt16(65535us, N1 = 65535us)>]
[<System_UInt32(4294967295u, N1 = 4294967295u)>]
[<System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)>]
[<System_Char('A', N1 = 'A')>]
[<System_Byte(255uy, N1 = 255uy)>]
[<System_SByte(127y, N1 = 127y)>]
[<System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)>]
[<System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)>]
[<System_String("hello", N1 = "hello")>]
[<System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)>]
[<System_Type(typedefof<System.Func<_,_>>, N1 = typedefof<System.Func<_,_>>)>]
[<System_Object(110, N1 = 110)>]
[<System_TypeArray([| typeof<int> |], N1 = [| typeof<int> |])>]
type ClassWithAttrs() = 
    [<System_Int16(10s, N1 = 10s)>]
    [<System_Int32(2147483647, N1 = 2147483647)>]
    [<System_Int64(9223372036854775807L, N1 = 9223372036854775807L)>]
    [<System_UInt16(65535us, N1 = 65535us)>]
    [<System_UInt32(4294967295u, N1 = 4294967295u)>]
    [<System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)>]
    [<System_Char('A', N1 = 'A')>]
    [<System_Byte(255uy, N1 = 255uy)>]
    [<System_SByte(127y, N1 = 127y)>]
    [<System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)>]
    [<System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)>]
    [<System_String("hello", N1 = "hello")>]
    [<System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)>]
    [<System_Type(typedefof<System.Func<_,_>>, N1 = typedefof<System.Func<_,_>>)>]
    [<System_Object(110, N1 = 110)>]
    [<System_TypeArray([| typeof<int> |], N1 = [| typeof<int> |])>]
    static member MethodWithAttrs() = seq {for i in [1 .. 10] -> System.Guid.Empty}
    [<System_Int16(10s, N1 = 10s)>]
    [<System_Int32(2147483647, N1 = 2147483647)>]
    [<System_Int64(9223372036854775807L, N1 = 9223372036854775807L)>]
    [<System_UInt16(65535us, N1 = 65535us)>]
    [<System_UInt32(4294967295u, N1 = 4294967295u)>]
    [<System_UInt64(18446744073709551615UL, N1 = 18446744073709551615UL)>]
    [<System_Char('A', N1 = 'A')>]
    [<System_Byte(255uy, N1 = 255uy)>]
    [<System_SByte(127y, N1 = 127y)>]
    [<System_Single(System.Single.MaxValue, N1 = System.Single.MaxValue)>]
    [<System_Double(System.Double.MaxValue, N1 = System.Double.MaxValue)>]
    [<System_String("hello", N1 = "hello")>]
    [<System_DateTimeKind(System.DateTimeKind.Local, N1 = System.DateTimeKind.Local)>]
    [<System_Type(typedefof<System.Func<_,_>>, N1 = typedefof<System.Func<_,_>>)>]
    [<System_Object(110, N1 = 110)>]
    [<System_TypeArray([| typeof<int> |], N1 = [| typeof<int> |])>]
    static member MethodWithAttrsThatReturnsAType() = typeof<ClassWithAttrs>
