module Neg119

// This is an example provided by Gustavo Leon in https://github.com/dotnet/fsharp/pull/4173
// The code is potentially valid and, if that PR had been accepted, would compile.
// It's being added as a negative test case to capture the fact that it currently
// fails to compile.

module Applicatives =
    open System

    type Ap = Ap with
        static member inline Invoke (x:'T) : '``Applicative<'T>`` =
            let inline call (mthd : ^M, output : ^R) = ((^M or ^R) : (static member Return: _*_ -> _) output, mthd)
            call (Ap, Unchecked.defaultof<'``Applicative<'T>``>) x 
        static member inline InvokeOnInstance (x:'T) = (^``Applicative<'T>`` : (static member Return: ^T -> ^``Applicative<'T>``) x)
        static member inline Return (r:'R       , _:obj) = Ap.InvokeOnInstance      :_ -> 'R
        static member        Return (_:seq<'a>  , Ap   ) = fun x -> Seq.singleton x : seq<'a>
        static member        Return (_:Tuple<'a>, Ap   ) = fun x -> Tuple x         : Tuple<'a>
        static member        Return (_:'r -> 'a , Ap   ) = fun k _ -> k             : 'a  -> 'r -> _

    let inline result (x:'T) = Ap.Invoke x

    let inline (<*>) (f:'``Applicative<'T->'U>``) (x:'``Applicative<'T>``) : '``Applicative<'U>`` = 
        (( ^``Applicative<'T->'U>`` or ^``Applicative<'T>`` or ^``Applicative<'U>``) : (static member (<*>): _*_ -> _) f, x)

    let inline (+) (a:'Num) (b:'Num) :'Num = a + b

    type ZipList<'s> = ZipList of 's seq with
        static member Return (x:'a)                              = ZipList (Seq.initInfinite (fun _ -> x))
        static member (<*>) (ZipList (f:seq<'a->'b>), ZipList x) = ZipList (Seq.zip f x |> Seq.map (fun (f, x) -> f x)) :ZipList<'b>

    type Ii = Ii
    type Idiomatic = Idiomatic with
        static member inline ($) (Idiomatic, si) = fun sfi x -> (Idiomatic $ x) (sfi <*> si)
        static member        ($) (Idiomatic, Ii) = id
    let inline idiomatic a b = (Idiomatic $ b) a
    let inline iI x = (idiomatic << result) x

    let res1n2n3 = iI (+) (result          0M                  ) (ZipList [1M;2M;3M]) Ii
    let res2n3n4 = iI (+) (result LanguagePrimitives.GenericOne) (ZipList [1 ;2 ;3 ]) Ii
