module Program

open System
open System.Globalization
open Messages

let check expected actual =
    if expected <> actual then
        failwith $"Expected '{expected}', got '{actual}'"

[<EntryPoint>]
let main _ =
    CultureInfo.CurrentCulture <- CultureInfo("fr-FR")
    SR.SwallowResourceText <- false
    SR.RunStartupValidation()
    check (Some "No arguments") (SR.GetTextOpt("zero"))
    check "No arguments" (SR.zero())
    check "Integer -42" (SR.integer(-42))
    check "Floating 1,25" (SR.floating(1.25))
    check "Text text" (SR.text("text"))
    check "Text " (SR.text(Unchecked.defaultof<string>))
    check "Hex ff" (SR.hex(255u))
    check "Mixed -42 / 1,25 / text" (SR.mixed(-42, 1.25, "text"))
    check (41, "Numbered without arguments") (SR.numberedZero())
    check (42, "Numbered -42 / 1,25 / text") (SR.numberedMixed(-42, 1.25, "text"))
    check "Escaped {literal}, 100% and text\n\t\"quoted\"" (SR.escaped("text"))
    SR.SwallowResourceText <- true
    check ",,," (SR.zero())
    check ",,,-42,,," (SR.integer(-42))
    check ",,,1.250000,,," (SR.floating(1.25))
    check ",,,text,,," (SR.text("text"))
    check ",,,,,," (SR.text(Unchecked.defaultof<string>))
    check ",,,ff,,," (SR.hex(255u))
    check ",,,-42,,,1.250000,,,text,,," (SR.mixed(-42, 1.25, "text"))
    check (41, ",,,") (SR.numberedZero())
    check (42, ",,,-42,,,1.250000,,,text,,,") (SR.numberedMixed(-42, 1.25, "text"))
#if NULLABLE
    Console.WriteLine("enable")
#else
    Console.WriteLine("disable")
#endif
    0
