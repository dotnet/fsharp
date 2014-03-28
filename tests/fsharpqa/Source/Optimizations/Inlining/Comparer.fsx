open System
open System.Text.RegularExpressions
open System.IO

module M =
    type State = | Before
                 | Entered
                 | Exited

    // Read IL into an array (only lines between the specified token)
    let File2ListWithinTokens (filename:string, token:string) = 
        use s = new StreamReader(filename)
        let mutable l = []
        let mutable state = Before
        while not s.EndOfStream do
            let line = s.ReadLine()            
            
            match state with
            | Before -> 
                let pattern = @"\s+\.method.+" + token + ".*"
                if Regex.IsMatch(line, pattern) then 
                    state <- Entered
                    l <- List.append l ( line :: [])
            | Entered ->  
                let pattern = @"\s+}.+" + token + ".*"
                l <- List.append l ( line :: [])
                if Regex.IsMatch(line, pattern) then 
                    state <- Exited   
            | Exited -> ()
        l

let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]
let tok = fsi.CommandLineArgs.[3]

// Read file into an array
let File2List (filename:string) = 
    use s = new StreamReader(filename)
    let mutable l = []
    while not s.EndOfStream do
        l <- List.append l ( s.ReadLine() :: [])
    l

let f1 = File2List fn1
let f2 = M.File2ListWithinTokens(fn2, tok)

if f1.Length <> f2.Length then 
    printfn "WARNING: Files have different lengths %d %d" f1.Length f2.Length

let mutable i = 0
let compare (f1:string list) (f2:string list) =
    List.forall2 (fun (a:string) (b:string) ->
        let aa = Regex.Replace(a, @"(.*\.line[^']*)('.+)", "$1")
        let bb = Regex.Replace(b, @"(.*\.line[^']*)('.+)", "$1")

        let aa = Regex.Replace(aa, @"(.*\.ver )(.+)", "$1")
        let bb = Regex.Replace(bb, @"(.*\.ver )(.+)", "$1")

        let aa = Regex.Replace(aa, @"(.*\.publickeytoken )(.+)", "$1")
        let bb = Regex.Replace(bb, @"(.*\.publickeytoken )(.+)", "$1")

        let aa = Regex.Replace(aa, @"'(\w+@\d+)-\d+'", "$1")
        let bb = Regex.Replace(bb, @"'(\w+@\d+)-\d+'", "$1")

        let aa = Regex.Replace(aa, @"^\s+", "")
        let bb = Regex.Replace(bb, @"^\s+", "")

        i <- i+1
        if ((if Regex.IsMatch(aa, @"^[ \t]*//") then "//" else aa) = (if Regex.IsMatch(bb, @"^[ \t]*//") then "//" else bb)) then
            true 
        else
            use s = new StreamWriter(fn2 + ".cmp")
            f1 |> List.iter (fun l -> s.WriteLine(l))
            printfn "Files differ at line %d:" i
            printfn "\t>> %s" a
            printfn "\t<< %s" b
            printfn "Files being compared: '%s' and '%s.cmp'" fn1 fn2
            false
    ) f1 f2

exit (if compare f1 f2 then 0 else 1)
