module Types

type IStaticProperty<'T when 'T :> IStaticProperty<'T>> =
    static abstract StaticProperty: 'T

type IStaticMethod<'T when 'T :> IStaticMethod<'T>> =
    static abstract StaticMethod: 'T -> 'T

type IUnitMethod<'T when 'T :> IUnitMethod<'T>> =
    static abstract UnitMethod: unit -> unit

type IAdditionOperator<'T when 'T :> IAdditionOperator<'T>> =
    static abstract op_Addition: 'T * 'T -> 'T

type ISinOperator<'T when 'T :> ISinOperator<'T>> =
    static abstract Sin: 'T -> 'T

type IDelegateConversion<'T when 'T :> IDelegateConversion<'T>> =
    static abstract FuncConversion: System.Func<int, int> -> 'T
    static abstract ExpressionConversion: System.Linq.Expressions.Expression<System.Func<int, int>> -> 'T

type C(c: int) =
    member _.Value = c

    interface IAdditionOperator<C> with
        static member op_Addition(x, y) = C(x.Value + y.Value)

    interface IStaticProperty<C> with
        static member StaticProperty = C(7)

    interface IStaticMethod<C> with
        static member StaticMethod(x) = C(x.Value + 4)

    interface IUnitMethod<C> with
        static member UnitMethod() = ()

    interface IDelegateConversion<C> with
        static member FuncConversion(f) = C(f.Invoke(3))
        static member ExpressionConversion(e) = C(e.Compile().Invoke(3))
