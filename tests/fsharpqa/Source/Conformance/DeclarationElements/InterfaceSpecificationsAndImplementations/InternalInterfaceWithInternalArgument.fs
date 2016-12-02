type internal Foo() = class end

type internal IMyInterface =
    abstract member Method1 : Foo -> unit

type Class1() =
    interface IMyInterface with
        // Bug: https://github.com/Microsoft/visualfsharp/issues/557
        // error FS0410: The type 'Foo' is less accessible than
        // the value, member or type 'override Class1.Method1 : v:Foo -> unit' it is used in
        // but should be fine because interface implementations are explicit and interface is
        // internal too.
        member this.Method1(v : Foo) = ()