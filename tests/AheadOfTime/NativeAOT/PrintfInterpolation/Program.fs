module Program

[<EntryPoint>]
let main _ =
    Formatting.runChecks ()
    let now = System.DateTime(2026, 10, 13, 1, 2, 3)
    let startAt = Formatting.timestamp now
    Formatting.check "01:02:03" startAt
    System.Console.WriteLine(startAt)
    0
