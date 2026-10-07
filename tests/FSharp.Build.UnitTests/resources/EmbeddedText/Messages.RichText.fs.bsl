// This is a generated file; the original input is 'Messages.txt'
namespace Messages

open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
// (namespaces below for specific case of using the tool to compile FSharp.Core itself)
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators

#nowarn "1182" // Generated boilerplate may include helper functions not referenced when the resource file has no entries
#nowarn "3262" // The call to Option.ofObj below is applied in multiple compilation modes for GetString, sometimes the value is typed as a non-nullable string

open FSharp.Compiler.Text
type internal SR private() =

    // BEGIN BOILERPLATE
    static let resources = lazy (new System.Resources.ResourceManager("Messages", System.Reflection.Assembly.GetExecutingAssembly()))

    static let GetString(name:string) =
        let s = resources.Value.GetString(name, System.Globalization.CultureInfo.CurrentUICulture)
    #if DEBUG
        if isNull s then
            System.Diagnostics.Debug.Assert(false, $"**RESOURCE ERROR**: Resource token {name} does not exist!")
    #endif
    #if NULLABLE
        Unchecked.nonNull s
    #else
        s
    #endif


    // newlines and tabs get converted to strings when read from a resource file
    // this will preserve their original intention
    static let postProcessString (s: string) =
        s.Replace("\\n","\n").Replace("\\t","\t").Replace("\\r","\r").Replace("\\\"", "\"")

    static let mutable swallowResourceText = false

    static let FormatMessage(messageID: string, swallowedFormat: string, args: objnull array) : string =
        if swallowResourceText then
            System.String.Format(System.Globalization.CultureInfo.InvariantCulture, swallowedFormat, args)
        else
            System.String.Format(postProcessString (GetString messageID), args)

    static member GetTextOpt(key:string) : string option = GetString(key) |> Option.ofObj

    /// If set to true, then all error messages will just return the filled 'holes' delimited by ',,,'s - this is for language-neutral testing (e.g. localization-invariant baselines).
    static member SwallowResourceText with get () = swallowResourceText
                                        and set (b) = swallowResourceText <- b
    // END BOILERPLATE

    /// No arguments
    /// (Originally from Messages.txt:1)
    static member zero() = (FormatMessage("zero", ",,,", [|  |]))
    /// Integer %d
    /// (Originally from Messages.txt:2)
    static member integer(a0: System.Int32) = (FormatMessage("integer", ",,,{0},,,", [| box a0 |]))
    /// Floating %f
    /// (Originally from Messages.txt:3)
    static member floating(a0: System.Double) = (FormatMessage("floating", ",,,{0:F6},,,", [| box a0 |]))
    /// Text %s
    /// (Originally from Messages.txt:4)
    static member text(a0: System.String) = (FormatMessage("text", ",,,{0},,,", [| box a0 |]))
    /// Text %s
    /// (Originally from Messages.txt:4)
    static member text(a0: RichText) = RichMessage.text (fun rich -> SR.text(rich a0))
    /// Hex %x
    /// (Originally from Messages.txt:5)
    static member hex(a0: System.UInt32) = (FormatMessage("hex", ",,,{0:x},,,", [| box a0 |]))
    /// Mixed %d / %f / %s
    /// (Originally from Messages.txt:6)
    static member mixed(a0: System.Int32, a1: System.Double, a2: System.String) = (FormatMessage("mixed", ",,,{0},,,{1:F6},,,{2},,,", [| box a0; box a1; box a2 |]))
    /// Mixed %d / %f / %s
    /// (Originally from Messages.txt:6)
    static member mixed(a0: System.Int32, a1: System.Double, a2: RichText) = RichMessage.text (fun rich -> SR.mixed(a0, a1, rich a2))
    /// Numbered without arguments
    /// (Originally from Messages.txt:7)
    static member numberedZero() = (41, RichText.mkText (FormatMessage("numberedZero", ",,,", [|  |])))
    /// Numbered %d / %f / %s
    /// (Originally from Messages.txt:8)
    static member numberedMixed(a0: System.Int32, a1: System.Double, a2: System.String) = (42, RichText.mkText (FormatMessage("numberedMixed", ",,,{0},,,{1:F6},,,{2},,,", [| box a0; box a1; box a2 |])))
    /// Numbered %d / %f / %s
    /// (Originally from Messages.txt:8)
    static member numberedMixed(a0: System.Int32, a1: System.Double, a2: RichText) = RichMessage.numbered (fun rich -> SR.numberedMixed(a0, a1, rich a2))
    /// Escaped {literal}, 100%% and %s\n\t\"quoted\"
    /// (Originally from Messages.txt:9)
    static member escaped(a0: System.String) = (FormatMessage("escaped", ",,,{0},,,", [| box a0 |]))
    /// Escaped {literal}, 100%% and %s\n\t\"quoted\"
    /// (Originally from Messages.txt:9)
    static member escaped(a0: RichText) = RichMessage.text (fun rich -> SR.escaped(rich a0))

    /// Call this method once to validate that all known resources are valid; throws if not
    static member RunStartupValidation() =
        ignore(GetString("zero"))
        ignore(GetString("integer"))
        ignore(GetString("floating"))
        ignore(GetString("text"))
        ignore(GetString("hex"))
        ignore(GetString("mixed"))
        ignore(GetString("numberedZero"))
        ignore(GetString("numberedMixed"))
        ignore(GetString("escaped"))
        ()
