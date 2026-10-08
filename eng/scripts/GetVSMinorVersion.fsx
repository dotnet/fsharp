open System
open System.Globalization

let dateBefore predicate (basis: DateOnly) =
    Seq.initInfinite (fun days -> basis.AddDays(-(days + 1)))
    |> Seq.find predicate

let isFirstTuesday (date: DateOnly) =
    date.DayOfWeek = DayOfWeek.Tuesday && date.Day <= 7

let isFriday (date: DateOnly) =
    date.DayOfWeek = DayOfWeek.Friday

let monthsSince (basis: DateOnly) (date: DateOnly) =
    Seq.initInfinite basis.AddMonths
    |> Seq.findIndex (fun month ->
        month.Year = date.Year && month.Month = date.Month)

let currentDate =
    DateOnly.Parse(fsi.CommandLineArgs[1], CultureInfo.InvariantCulture)

currentDate.AddMonths(1)
|> dateBefore (fun tuesday ->
    isFirstTuesday tuesday
    && (tuesday |> dateBefore isFriday) <= currentDate)
|> monthsSince (DateOnly(2025, 9, 1))
|> printfn "%d"
