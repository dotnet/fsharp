module FixedBindings
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop
open System

[<Struct; IsByRefLike>]
type ArrayElementRef<'T> =
    private { Values: 'T[]; Index: int }
    
    static member Create(values: 'T[], index) =
        if index > values.Length then
            raise (ArgumentOutOfRangeException(nameof(index), ""))
        { Values = values; Index = index }
    member this.Value = this.Values[this.Index]
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.GetPinnableReference () : byref<'T> = &this.Values[this.Index]

let pinIt (thing: ArrayElementRef<'a>) =
    use ptr = fixed thing
    NativePtr.get ptr 0

[<EntryPoint>]
let main _ =
    let arr = [|'a';'b';'c'|]
    let x = ArrayElementRef.Create(arr, 1)
    let y = pinIt x
    if y <> x.Value then failwith "y did not equal x value"
    0