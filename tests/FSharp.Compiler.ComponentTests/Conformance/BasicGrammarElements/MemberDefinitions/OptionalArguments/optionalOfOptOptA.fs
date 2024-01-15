// #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments 
#light

// Test an optional parameter of type 'option option option a'
type Foo<'a>() =
    member this.Test (?param1:'a option option option) =
        match param1 with
        | Some(a) -> 
                    match a with
                    | Some(b) ->
                                 match b with
                                 | Some(c) ->
                                                match c with
                                                | Some x -> true
                                                | None   -> false
                                 | None     -> false
                    | None    -> false
        | None -> false

let tester = new Foo<int>()
if tester.Test() = true then failwith "Failed: 1"
if tester.Test(Some(Some(Some(1)))) = false then failwith "Failed: 2"
if tester.Test(?param1=Some(Some(Some(Some(1))))) = false then failwith "Failed: 3"
