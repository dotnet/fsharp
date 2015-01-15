open System.Text.RegularExpressions
open System.IO

[<EntryPoint>]
let main (argv : string array) =
    let fn1 = argv.[0]
    let fn2 = argv.[1]

    // Read file into an array
    let File2List (filename:string) = 
        use s = new StreamReader(filename)
        let mutable l = []
        while not s.EndOfStream do
            let line = s.ReadLine()
            let isblank_or_comment = Regex.IsMatch(line, @"^[ \t]*//") || Regex.IsMatch(line, @"^[ \t]*$")
            if not isblank_or_comment then l <- List.append l ( line :: [])
        l

    let f1 = File2List fn1
    let f2 = File2List fn2

    let mutable i = 0
    let compare (f1:string list) (f2:string list) =
        try
            List.forall2 (fun (a:string) (b:string) ->
                let aa = Regex.Replace(a, @"(.*\.line[^'$]*)('.+)?", "$1")
                let bb = Regex.Replace(b, @"(.*\.line[^'$]*)('.+)?", "$1")

                let aa = Regex.Replace(aa, @"(.*\.ver )(.+)", "$1")
                let bb = Regex.Replace(bb, @"(.*\.ver )(.+)", "$1")

                let aa = Regex.Replace(aa, @"(.*\.publickeytoken )(.+)", "$1")
                let bb = Regex.Replace(bb, @"(.*\.publickeytoken )(.+)", "$1")

                let aa = Regex.Replace(aa, @"\s+//.+$", "").TrimEnd(' ', '\t')
                let bb = Regex.Replace(bb, @"\s+//.+$", "").TrimEnd(' ', '\t')

                // generated identifier names can sometimes differ when using hosted compiler due to polluted environment
                // e.g. fresh compiler would generate identifier x@8
                //      but hosted compiler will generate 'x@8-3'
                // these are functionally the same, merely a naming difference
                // strip off the single quotes and -N tag.
                let aa = Regex.Replace(aa, @"'(\w+@\d+)-\d+'", "$1")
                let bb = Regex.Replace(bb, @"'(\w+@\d+)-\d+'", "$1")

                // strip out leading whitespace, as sometimes this differs purely due to ildasm formatting
                let aa = Regex.Replace(aa, @"^\s+", "")
                let bb = Regex.Replace(bb, @"^\s+", "")

                i <- i + 1
                if ((if Regex.IsMatch(aa, @"^[ \t]*//") then "//" else aa) = (if Regex.IsMatch(bb, @"^[ \t]*//") then "//" else bb)) then
                    true
                else
                    printfn "Files differ at line %d:" i
                    printfn "\t>> %s" a
                    printfn "\t<< %s" b
                    false
            ) f1 f2
        with
        | e ->
            printfn "%s" (e.ToString())
            false

    exit (if compare f1 f2 then 0 else 1)
