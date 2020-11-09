// #Conformance #DeclarationElements #ObjectConstructors 
#light

// Verify the ability to define mutually recursive types.

type Foo(i : int) =
    // Normally this would fail, because DayOfWeek nor IndexOf have been defined yet
    member this.WeekIndex = DayOfWeek.IndexOf(i % 7)
and DayOfWeek =
    | Sunday
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday
    static member IndexOf i =
        match i with
        | 0 -> Sunday
        | 1 -> Monday
        | 2 -> Tuesday
        | 3 -> Wednesday
        | 4 -> Thursday
        | 5 -> Friday
        | 6 -> Saturday
        | _ -> failwith "Unknown"

let test = Foo(2)
if test.WeekIndex <> Tuesday then exit 1

exit 0
