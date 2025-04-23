module Net8Lib

type Default3 = class end
type Default2 = class inherit Default3 end
type Default1 = class inherit Default2 end

type IsAltLeftZero =
    inherit Default1

    static member inline IsAltLeftZero (_: ref<'T>   when 'T : struct    , _mthd: Default3) = false
    static member inline IsAltLeftZero (_: ref<'T>   when 'T : not struct, _mthd: Default2) = false

    static member inline IsAltLeftZero (t: ref<'At>                               , _mthd: Default1) = (^At : (static member IsAltLeftZero : _ -> _) t.Value)
    static member inline IsAltLeftZero (_: ref< ^t> when ^t: null and ^t: struct , _mthd: Default1) = ()

    static member        IsAltLeftZero (t: ref<option<_>  > , _mthd: IsAltLeftZero) = Option.isSome t.Value
    static member        IsAltLeftZero (t: ref<voption<_>  >, _mthd: IsAltLeftZero) = ValueOption.isSome t.Value


    static member inline Invoke (x: 'At) : bool =
        let inline call (mthd : ^M, input: ^I) =
            ((^M or ^I) : (static member IsAltLeftZero : _*_ -> _) (ref input), mthd)
        call(Unchecked.defaultof<IsAltLeftZero>, x)