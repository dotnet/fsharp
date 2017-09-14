// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

    open Microsoft.FSharp.Core.Operators

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Option = 

        [<CompiledName("GetValue")>]
        let get option = match option with None -> invalidArg "option" (SR.GetString(SR.optionValueWasNone)) | Some x -> x

        [<CompiledName("IsSome")>]
        let inline isSome option = match option with None -> false | Some _ -> true

        [<CompiledName("IsNone")>]
        let inline isNone option = match option with None -> true | Some _ -> false

        [<CompiledName("DefaultValue")>]
        let defaultValue value option = match option with None -> value | Some v -> v

        [<CompiledName("DefaultWith")>]
        let defaultWith defThunk option = match option with None -> defThunk () | Some v -> v

        [<CompiledName("OrElse")>]
        let orElse ifNone option = match option with None -> ifNone | Some _ -> option

        [<CompiledName("OrElseWith")>]
        let orElseWith ifNoneThunk option = match option with None -> ifNoneThunk () | Some _ -> option

        [<CompiledName("Count")>]
        let count option = match option with None -> 0 | Some _ -> 1

        [<CompiledName("Fold")>]
        let fold<'T,'State> folder (state:'State) (option: option<'T>) = match option with None -> state | Some x -> folder state x

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State> folder (option: option<'T>) (state:'State) =  match option with None -> state | Some x -> folder x state

        [<CompiledName("Exists")>]
        let exists predicate option = match option with None -> false | Some x -> predicate x

        [<CompiledName("ForAll")>]
        let forall predicate option = match option with None -> true | Some x -> predicate x

        [<CompiledName("Contains")>]
        let inline contains value option = match option with None -> false | Some v -> v = value

        [<CompiledName("Iterate")>]
        let iter action option = match option with None -> () | Some x -> action x

        [<CompiledName("Map")>]
        let map mapping option = match option with None -> None | Some x -> Some (mapping x)

        [<CompiledName("Map2")>]
        let map2 mapping option1 option2 = 
            match option1, option2 with
            | Some x, Some y -> Some (mapping x y)
            | _ -> None

        [<CompiledName("Map3")>]
        let map3 mapping option1 option2 option3 = 
            match option1, option2, option3 with
            | Some x, Some y, Some z -> Some (mapping x y z)
            | _ -> None

        [<CompiledName("Bind")>]
        let bind binder option = match option with None -> None | Some x -> binder x

        [<CompiledName("Flatten")>]
        let flatten option = match option with None -> None | Some x -> x

        [<CompiledName("Filter")>]
        let filter predicate option = match option with None -> None | Some x -> if predicate x then Some x else None

        [<CompiledName("ToArray")>]
        let toArray option = match option with  None -> [| |] | Some x -> [| x |]

        [<CompiledName("ToList")>]
        let toList option = match option with  None -> [ ] | Some x -> [ x ]

        [<CompiledName("ToNullable")>]
        let toNullable option = match option with None -> System.Nullable() | Some v -> System.Nullable(v)

        [<CompiledName("OfNullable")>]
        let ofNullable (value:System.Nullable<'T>) = if value.HasValue then Some value.Value else None

        [<CompiledName("OfObj")>]
        let ofObj value = match value with null -> None | _ -> Some value

        [<CompiledName("ToObj")>]
        let toObj value = match value with None -> null | Some x -> x
