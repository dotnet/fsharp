open Types

module ``Test basic IWSAM generic code`` =

    let f_IWSAM_explicit_operator_name<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.op_Addition(x, y)

    let f_IWSAM_pretty_operator_name<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.(+)(x, y)

    let f_IWSAM_StaticProperty<'T when 'T :> IStaticProperty<'T>>() =
        'T.StaticProperty

    let f_IWSAM_declared_StaticMethod<'T when 'T :> IStaticMethod<'T>>(x: 'T) =
        'T.StaticMethod(x)

    let f_IWSAM_declared_UnitMethod<'T when 'T :> IUnitMethod<'T>>() =
        'T.UnitMethod()

    let f_IWSAM_declared_UnitMethod_list<'T when 'T :> IUnitMethod<'T>>() =
        let v = 'T.UnitMethod()
        [ v ]

    let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) =
        'T.StaticProperty

    let f_IWSAM_flex_StaticMethod(x: #IStaticMethod<'T>) =
        'T.StaticMethod(x)


    let inline f3<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.op_Addition(x,y)

    let inline f4<'T when 'T : (static member (+): 'T * 'T -> 'T)>(x: 'T, y: 'T) =
        'T.op_Addition(x,y)

    let inline f5<'T when 'T : (static member (+): 'T * 'T -> 'T)>(x: 'T, y: 'T) =
        'T.(+)(x,y)

    let inline f6<'T when 'T : (static member (+): 'T * 'T -> 'T)>(x: 'T, y: 'T) =
        x + y

    let inline f_StaticProperty_IWSAM<'T when 'T :> IStaticProperty<'T>>() =
        'T.StaticProperty

    let inline f_StaticProperty_SRTP<'T when 'T : (static member StaticProperty: 'T) >() =
        'T.StaticProperty

    let inline f_StaticProperty_BOTH<'T when 'T :> IStaticProperty<'T> and 'T : (static member StaticProperty: 'T) >() =
        'T.StaticProperty


    module CheckExecution =
        if f_IWSAM_explicit_operator_name<C>(C(3), C(4)).Value <> 7 then
            failwith "incorrect value"

        if f_IWSAM_pretty_operator_name<C>(C(3), C(4)).Value <> 7 then
            failwith "incorrect value"

        if f_IWSAM_StaticProperty<C>().Value <> 7 then
            failwith "incorrect value"
