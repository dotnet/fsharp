module Test
#nowarn "57"

[<Measure>] type kg
[<Measure>] type s
[<Measure>] type m

let f (x:float<'u>) : float<'v> = 1.0<_> * x
let incr1 x = 1.0<_> + (x:float<_>)
let incr2 x = (x:float<_>) + 1.0<_>

let x1 = incr1 2.0<kg>
let x1a = incr1 2.0<m>
let x2 = incr2 3.0<m>
let x2a = incr2 3.0<kg>
let x3:float<kg> = f 2.0<m>
let x3a:float<m> = f 3.0<kg>

module ExtensionMemberOfGenericTypeWrongNumberOfTypeArgsTest = 

    type LibGen<'T when 'T :> System.ValueType>() =
        class
          [<DefaultValue(false)>]
          val mutable instanceField : 'T 
       end

    module N =
      type LibGen with
            member x.P  = 1

module ExtensionMemberOfGenericTypeWrongConstraintsTest = 

    type LibGen<'T when 'T :> System.ValueType>() =
        class
          [<DefaultValue(false)>]
          val mutable instanceField : 'T 
       end

    module N =
      type LibGen<'T> with
            member x.P  = 1
