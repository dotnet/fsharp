module M
        
open System  
open System.Text  
open Microsoft.FSharp.Collections  


type PrettyString = 
    | StringLiteral of String 
    | Vertical of PrettyString * PrettyString 
    | Horizontal of PrettyString * PrettyString 

let rec Height ps = 
    match ps with 
    | StringLiteral(_) -> 1 
    | Vertical(top, bottom) -> (Height top) + (Height bottom) 
    | Horizontal(left, right) -> max (Height left) (Height right) 

let rec Width ps = 
    match ps with 
    | StringLiteral(s) -> s.Length 
    | Vertical(top, bottom) -> max (Width top) (Width bottom) 
    | Horizontal(left, right) -> (Width left) + (Width right) 

let rec Line ps n = 
    match ps with 
    | StringLiteral(s) -> if n <> 0 then "" else s 
    | Vertical(top, bottom) ->  
        if n < 0 || n >= Height ps 
        then ""  
        else if n < Height top )
             then Line top n  
             else Line bottom (n - Height top)  
    | Horizontal(left, right) ->  
        String.Concat((Line left n).PadRight(Width left),  
                      Line right n)  

let ToString ps = 
    let sb = new StringBuilder() 
    for i in 0 .. Height ps do  
        sb.Append(Line ps i) |> ignore 
        sb.Append("\n") |> ignore 
    sb.ToString()  
     
let pretty s = StringLiteral(s) 
let (%%) x y = Vertical(x,y) 
let (++) x y = Horizontal(x,y) 

let blank = pretty " " 

let calendar year month =  
    let maxDays = DateTime.DaysInMonth(year, month)  
    // make the column that starts with day i (e.g. 1, 8, 15, 22, 29) 
    let makeColumn i =  
        let prettyNum (i:int) = pretty(i.ToString().PadLeft(2))  
        let mutable r = prettyNum i  
        let mutable j = i + 7  
        while j <= maxDays do  
            r <- r %% prettyNum j  
            j <- j + 7  
        r  
          
    let firstOfMonth = new DateTime(year, month, 1)  
    let firstDayOfWeek = int firstOfMonth.DayOfWeek 
    // create all the columns for this month's calendar, with Sundays in columns[0] 
    let columns = Array.create 7 blank  
    for i in 0 .. 6 do  
        columns.[(i + firstDayOfWeek) % 7] <- makeColumn (i+1)  
    // if, for example, the first of the month is a Tuesday, then we need Sunday and Monday 
    // to have a blank in the first row of the calendar    
    if firstDayOfWeek <> 0 then  
        for i in 0 .. firstDayOfWeek-1 do  
            columns.[i] <- blank %% columns.[i]  
    // put the weekday headers at the top of each column 
    let dayAbbrs = [| "Su"; "Mo"; "Tu"; "We"; "Th"; "Fr"; "Sa" |]  
    for i in 0 .. 6 do  
        columns.[i] <- pretty(dayAbbrs.[i]) %% columns.[i]  
    // Horizontally concatenate all of the columns together, with blank space between columns 
    let calendarBody = Array.reduceBack (fun x y -> x ++ blank ++ y) columns  
    // Surely there must be a .NET call that turns a month number into its name, 
    // but I couldn't find it when I was typing this up.  
    let monthNames = [| "January" ; "February"; "March"; "April"; "May"; "June";   
        "July"; "August"; "September"; "October"; "November"; "December" |]  
    let titleBar = pretty(sprintf "%s %d" monthNames.[month-1] year)   
    titleBar %% calendarBody 

let c = calendar 2008 4 
#if CLASSES 
printfn "%s" (c.ToString()) 
#else 
printfn "%s" (ToString c) 
#endif 

