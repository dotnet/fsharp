open System
open Types

module CheckSelfConstrainedIWSAM =

    let f_IWSAM_explicit_operator_name<'T when IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.op_Addition(x, y)

    let f_IWSAM_pretty_operator_name<'T when IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.(+)(x, y)

    let f_IWSAM_StaticProperty<'T when IStaticProperty<'T>>() =
        'T.StaticProperty

    let f_IWSAM_declared_StaticMethod<'T when IStaticMethod<'T>>(x: 'T) =
        'T.StaticMethod(x)

    let f_IWSAM_declared_UnitMethod<'T when IUnitMethod<'T>>() =
        'T.UnitMethod()

    let f_IWSAM_declared_UnitMethod_list<'T when IUnitMethod<'T>>() =
        let v = 'T.UnitMethod()
        [ v ]

    let inline f3<'T when IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.op_Addition(x,y)

    type WithStaticProperty<'T when 'T : (static member StaticProperty: int)> = 'T
    type WithStaticMethod<'T when 'T : (static member StaticMethod: int -> int)> = 'T
    type WithBoth<'T when WithStaticProperty<'T> and WithStaticMethod<'T>> = 'T

    let inline f_StaticProperty<'T when WithStaticProperty<'T>>() = 'T.StaticProperty
    let inline f_StaticMethod<'T when WithStaticMethod<'T>>() = 'T.StaticMethod(3)
    let inline f_Both<'T when WithBoth<'T> >() =
        let v1 = 'T.StaticProperty
        let v2 = 'T.StaticMethod(3)
        v1 + v2

    let inline f_OK1<'T when WithBoth<'T>>() =
        'T.StaticMethod(3) |> ignore
        'T.StaticMethod(3)

    let inline f_OK2<'T when WithBoth<'T>>() =
        'T.StaticMethod(3) |> ignore
        'T.StaticMethod(3)

    let inline f_OK3<'T when WithBoth<'T>>() =
        printfn ""
        'T.StaticMethod(3)

    printfn ""