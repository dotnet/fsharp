// #Conformance #PatternMatching #Records 
#light

// Verify ability to have nested patterns in record types

type Type = Plant | Animal | Mineral
[<Struct>] type Thing = {Name : string; Age : int; Type : Type}

let isYoung thing =
    match thing with
    | {Age = x; Type = (Plant|Animal|Mineral)} when x < 10 -> true
    | _ -> false

let isMatch thing (thingName : string option) (thingAge : int option) =
    // We can't extract the option value in a match and use it in the same statement, 
    // so we first break open the optional values.
    let name = match thingName with Some(name) -> name | None -> ""
    let age = match thingAge with Some(age) -> age | None -> 0
    // Finally match it all
    match thingName, thingAge, thing with
    | None, None, _ -> false
    | Some(_), None, {Name=itsName} when itsName = name -> true
    | None, Some(_), {Age=itsAge} when itsAge = age -> true
    | Some(_), Some(_), {Name=name; Age=age} -> true
    | _ -> false

let animal = {Name = "Steve"; Age = 2; Type = Animal}
let plant  = {Name = "Sunflower"; Age = 5; Type = Plant}
let rock   = {Name = "Gold"; Age = 500000; Type = Mineral}

if isYoung animal  <> true  then exit 1
if isYoung rock    <> false then exit 1

if isMatch animal (Some("Steve")) (Some(2)) <> true then exit 1   
if isMatch animal (Some("Steve")) None <> true then exit 1

if isMatch animal (Some("NotSteve")) None <> false then exit 1

exit 0
