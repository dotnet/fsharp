// #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments 
#light

type Ballad() =
    static member MethWithOptParams (?param1:int, ?param2:string list, ?param3:int list option) =
        let actualParam1 = 
            match param1 with
            | Some x -> x
            | None   -> 0
        let actualParam2 = 
            match param2 with
            | Some(slist) -> List.length slist
            | None        -> 0
        let actualParam3 =
            match param3 with
            | Some(ilo) ->  match ilo with
                            | Some(il) -> List.length il
                            | None     -> 0
            | None      -> 0
        (actualParam1 + actualParam2 + actualParam3)

// As regular params
let r1 = Ballad.MethWithOptParams(1, ["just"; "regular"; "args"], Some([1..2]))
if r1 <> 6 then failwith "Failed: 1"

// As 'None' params
let r2 = Ballad.MethWithOptParams(?param3=None, ?param1=None, ?param2=None)


// As 'Some' params
let r3 = Ballad.MethWithOptParams(?param1=Some(1), ?param2=Some(["option"; "Some/None"; "args"]), ?param3=Some(Some([1..2])))
if r3 <> 6 then failwith "Failed: 2"

// As just regular, named parameters
let r4 = Ballad.MethWithOptParams(param2=[], param1=0, param3=Some([1..10]))
if r4 <> 10 then failwith "Failed: 3"

// All missing parameters
let r5 = Ballad.MethWithOptParams()
if r5 <> 0 then failwith "Failed: 4"

// Some missing, some provided
let r6 = Ballad.MethWithOptParams(1, param2=["one"], param3=None)
if r6 <> 2 then failwith "Failed: 5"
