// This is a generated file; the original input is '..\fsi\FSIstrings.txt'
namespace FSIstrings

open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Reflection
open System.Reflection
// (namespaces below for specific case of using the tool to compile FSharp.Core itself)
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Text
open Microsoft.FSharp.Collections
open Printf

type internal SR private() =

    // BEGIN BOILERPLATE

    static let getCurrentAssembly () =
    #if FX_RESHAPED_REFLECTION
        typeof<SR>.GetTypeInfo().Assembly
    #else
        System.Reflection.Assembly.GetExecutingAssembly()
    #endif

    static let getTypeInfo (t: System.Type) =
    #if FX_RESHAPED_REFLECTION
        t.GetTypeInfo()
    #else
        t
    #endif

    static let resources = lazy (new System.Resources.ResourceManager("FSIstrings", getCurrentAssembly()))

    static let GetString(name:string) =
        let s = resources.Value.GetString(name, System.Globalization.CultureInfo.CurrentUICulture)
    #if DEBUG
        if null = s then
            System.Diagnostics.Debug.Assert(false, sprintf "**RESOURCE ERROR**: Resource token %s does not exist!" name)
    #endif
        s

    static let mkFunctionValue (tys: System.Type[]) (impl:obj->obj) = 
        FSharpValue.MakeFunction(FSharpType.MakeFunctionType(tys.[0],tys.[1]), impl)

    static let funTyC = typeof<(obj -> obj)>.GetGenericTypeDefinition()  

    static let isNamedType(ty:System.Type) = not (ty.IsArray ||  ty.IsByRef ||  ty.IsPointer)
    static let isFunctionType (ty1:System.Type)  =
        isNamedType(ty1) && getTypeInfo(ty1).IsGenericType && (ty1.GetGenericTypeDefinition()).Equals(funTyC)

    static let rec destFunTy (ty:System.Type) =
        if isFunctionType ty then
            ty, ty.GetGenericArguments() 
        else
            match getTypeInfo(ty).BaseType with 
            | null -> failwith "destFunTy: not a function type"
            | b -> destFunTy b 

    static let buildFunctionForOneArgPat (ty: System.Type) impl =
        let _,tys = destFunTy ty
        let rty = tys.[1]
        // PERF: this technique is a bit slow (e.g. in simple cases, like 'sprintf "%x"')
        mkFunctionValue tys (fun inp -> impl rty inp)

    static let capture1 (fmt:string) i args ty (go : obj list -> System.Type -> int -> obj) : obj =
        match fmt.[i] with
        | '%' -> go args ty (i+1)
        | 'd'
        | 'f'
        | 's' -> buildFunctionForOneArgPat ty (fun rty n -> go (n :: args) rty (i+1))
        | _ -> failwith "bad format specifier"

    // newlines and tabs get converted to strings when read from a resource file
    // this will preserve their original intention
    static let postProcessString (s : string) =
        s.Replace("\\n","\n").Replace("\\t","\t").Replace("\\r","\r").Replace("\\\"", "\"")

    static let createMessageString (messageString : string) (fmt : Printf.StringFormat<'T>) : 'T =
        let fmt = fmt.Value // here, we use the actual error string, as opposed to the one stored as fmt
        let len = fmt.Length 

        /// Function to capture the arguments and then run.
        let rec capture args ty i =
            if i >= len ||  (fmt.[i] = '%' && i+1 >= len) then
                let b = new System.Text.StringBuilder()
                b.AppendFormat(messageString, [| for x in List.rev args -> x |]) |> ignore
                box(b.ToString())
            // REVIEW: For these purposes, this should be a nop, but I'm leaving it
            // in incase we ever decide to support labels for the error format string
            // E.g., "<name>%s<foo>%d"
            elif System.Char.IsSurrogatePair(fmt,i) then
                capture args ty (i+2)
            else
                match fmt.[i] with
                | '%' ->
                    let i = i+1
                    capture1 fmt i args ty capture
                | _ ->
                    capture args ty (i+1)

        (unbox (capture [] (typeof<'T>) 0) : 'T)

    static let mutable swallowResourceText = false

    static let GetStringFunc((messageID : string),(fmt : Printf.StringFormat<'T>)) : 'T =
        if swallowResourceText then
            sprintf fmt
        else
            let mutable messageString = GetString(messageID)
            messageString <- postProcessString messageString
            createMessageString messageString fmt

    /// If set to true, then all error messages will just return the filled 'holes' delimited by ',,,'s - this is for language-neutral testing (e.g. localization-invariant baselines).
    static member SwallowResourceText with get () = swallowResourceText
                                        and set (b) = swallowResourceText <- b
    // END BOILERPLATE

    /// Stopped due to error\n
    /// (Originally from ..\fsi\FSIstrings.txt:2)
    static member stoppedDueToError() = (GetStringFunc("stoppedDueToError",",,,") )
    /// Usage: %s <options> [script.fsx [<arguments>]]
    /// (Originally from ..\fsi\FSIstrings.txt:3)
    static member fsiUsage(a0 : System.String) = (GetStringFunc("fsiUsage",",,,%s,,,") a0)
    /// - INPUT FILES -
    /// (Originally from ..\fsi\FSIstrings.txt:4)
    static member fsiInputFiles() = (GetStringFunc("fsiInputFiles",",,,") )
    /// - CODE GENERATION -
    /// (Originally from ..\fsi\FSIstrings.txt:5)
    static member fsiCodeGeneration() = (GetStringFunc("fsiCodeGeneration",",,,") )
    /// - ERRORS AND WARNINGS -
    /// (Originally from ..\fsi\FSIstrings.txt:6)
    static member fsiErrorsAndWarnings() = (GetStringFunc("fsiErrorsAndWarnings",",,,") )
    /// - LANGUAGE -
    /// (Originally from ..\fsi\FSIstrings.txt:7)
    static member fsiLanguage() = (GetStringFunc("fsiLanguage",",,,") )
    /// - MISCELLANEOUS -
    /// (Originally from ..\fsi\FSIstrings.txt:8)
    static member fsiMiscellaneous() = (GetStringFunc("fsiMiscellaneous",",,,") )
    /// - ADVANCED -
    /// (Originally from ..\fsi\FSIstrings.txt:9)
    static member fsiAdvanced() = (GetStringFunc("fsiAdvanced",",,,") )
    /// Exception raised when starting remoting server.\n%s
    /// (Originally from ..\fsi\FSIstrings.txt:10)
    static member fsiExceptionRaisedStartingServer(a0 : System.String) = (GetStringFunc("fsiExceptionRaisedStartingServer",",,,%s,,,") a0)
    /// Use the given file on startup as initial input
    /// (Originally from ..\fsi\FSIstrings.txt:11)
    static member fsiUse() = (GetStringFunc("fsiUse",",,,") )
    /// #load the given file on startup
    /// (Originally from ..\fsi\FSIstrings.txt:12)
    static member fsiLoad() = (GetStringFunc("fsiLoad",",,,") )
    /// Treat remaining arguments as command line arguments, accessed using fsi.CommandLineArgs
    /// (Originally from ..\fsi\FSIstrings.txt:13)
    static member fsiRemaining() = (GetStringFunc("fsiRemaining",",,,") )
    /// Display this usage message (Short form: -?)
    /// (Originally from ..\fsi\FSIstrings.txt:14)
    static member fsiHelp() = (GetStringFunc("fsiHelp",",,,") )
    /// Exit fsi after loading the files or running the .fsx script given on the command line
    /// (Originally from ..\fsi\FSIstrings.txt:15)
    static member fsiExec() = (GetStringFunc("fsiExec",",,,") )
    /// Execute interactions on a Windows Forms event loop (on by default)
    /// (Originally from ..\fsi\FSIstrings.txt:16)
    static member fsiGui() = (GetStringFunc("fsiGui",",,,") )
    /// Suppress fsi writing to stdout
    /// (Originally from ..\fsi\FSIstrings.txt:17)
    static member fsiQuiet() = (GetStringFunc("fsiQuiet",",,,") )
    /// Support TAB completion in console (on by default)
    /// (Originally from ..\fsi\FSIstrings.txt:18)
    static member fsiReadline() = (GetStringFunc("fsiReadline",",,,") )
    /// Emit debug information in quotations
    /// (Originally from ..\fsi\FSIstrings.txt:19)
    static member fsiEmitDebugInfoInQuotations() = (GetStringFunc("fsiEmitDebugInfoInQuotations",",,,") )
    /// For help type #help;;
    /// (Originally from ..\fsi\FSIstrings.txt:20)
    static member fsiBanner3() = (GetStringFunc("fsiBanner3",",,,") )
    /// A problem occurred starting the F# Interactive process. This may be due to a known problem with background process console support for Unicode-enabled applications on some Windows systems. Try selecting Tools->Options->F# Interactive for Visual Studio and enter '--fsi-server-no-unicode'.
    /// (Originally from ..\fsi\FSIstrings.txt:21)
    static member fsiConsoleProblem() = (GetStringFunc("fsiConsoleProblem",",,,") )
    /// '%s' is not a valid assembly name
    /// (Originally from ..\fsi\FSIstrings.txt:22)
    static member fsiInvalidAssembly(a0 : System.String) = (2301, GetStringFunc("fsiInvalidAssembly",",,,%s,,,") a0)
    /// Directory '%s' doesn't exist
    /// (Originally from ..\fsi\FSIstrings.txt:23)
    static member fsiDirectoryDoesNotExist(a0 : System.String) = (2302, GetStringFunc("fsiDirectoryDoesNotExist",",,,%s,,,") a0)
    /// Invalid directive '#%s %s'
    /// (Originally from ..\fsi\FSIstrings.txt:24)
    static member fsiInvalidDirective(a0 : System.String, a1 : System.String) = (GetStringFunc("fsiInvalidDirective",",,,%s,,,%s,,,") a0 a1)
    /// Warning: line too long, ignoring some characters\n
    /// (Originally from ..\fsi\FSIstrings.txt:25)
    static member fsiLineTooLong() = (GetStringFunc("fsiLineTooLong",",,,") )
    /// Real: %s, CPU: %s, GC %s
    /// (Originally from ..\fsi\FSIstrings.txt:26)
    static member fsiTimeInfoMainString(a0 : System.String, a1 : System.String, a2 : System.String) = (GetStringFunc("fsiTimeInfoMainString",",,,%s,,,%s,,,%s,,,") a0 a1 a2)
    /// gen
    /// (Originally from ..\fsi\FSIstrings.txt:27)
    static member fsiTimeInfoGCGenerationLabelSomeShorthandForTheWordGeneration() = (GetStringFunc("fsiTimeInfoGCGenerationLabelSomeShorthandForTheWordGeneration",",,,") )
    /// \n\nException raised during pretty printing.\nPlease report this so it can be fixed.\nTrace: %s\n
    /// (Originally from ..\fsi\FSIstrings.txt:28)
    static member fsiExceptionDuringPrettyPrinting(a0 : System.String) = (GetStringFunc("fsiExceptionDuringPrettyPrinting",",,,%s,,,") a0)
    ///   F# Interactive directives:
    /// (Originally from ..\fsi\FSIstrings.txt:29)
    static member fsiIntroTextHeader1directives() = (GetStringFunc("fsiIntroTextHeader1directives",",,,") )
    /// Reference (dynamically load) the given DLL
    /// (Originally from ..\fsi\FSIstrings.txt:30)
    static member fsiIntroTextHashrInfo() = (GetStringFunc("fsiIntroTextHashrInfo",",,,") )
    /// Add the given search path for referenced DLLs
    /// (Originally from ..\fsi\FSIstrings.txt:31)
    static member fsiIntroTextHashIInfo() = (GetStringFunc("fsiIntroTextHashIInfo",",,,") )
    /// Load the given file(s) as if compiled and referenced
    /// (Originally from ..\fsi\FSIstrings.txt:32)
    static member fsiIntroTextHashloadInfo() = (GetStringFunc("fsiIntroTextHashloadInfo",",,,") )
    /// Toggle timing on/off
    /// (Originally from ..\fsi\FSIstrings.txt:33)
    static member fsiIntroTextHashtimeInfo() = (GetStringFunc("fsiIntroTextHashtimeInfo",",,,") )
    /// Display help
    /// (Originally from ..\fsi\FSIstrings.txt:34)
    static member fsiIntroTextHashhelpInfo() = (GetStringFunc("fsiIntroTextHashhelpInfo",",,,") )
    /// Exit
    /// (Originally from ..\fsi\FSIstrings.txt:35)
    static member fsiIntroTextHashquitInfo() = (GetStringFunc("fsiIntroTextHashquitInfo",",,,") )
    ///   F# Interactive command line options:
    /// (Originally from ..\fsi\FSIstrings.txt:36)
    static member fsiIntroTextHeader2commandLine() = (GetStringFunc("fsiIntroTextHeader2commandLine",",,,") )
    ///       See '%s' for options
    /// (Originally from ..\fsi\FSIstrings.txt:37)
    static member fsiIntroTextHeader3(a0 : System.String) = (GetStringFunc("fsiIntroTextHeader3",",,,%s,,,") a0)
    /// Loading
    /// (Originally from ..\fsi\FSIstrings.txt:38)
    static member fsiLoadingFilesPrefixText() = (GetStringFunc("fsiLoadingFilesPrefixText",",,,") )
    /// \n- Interrupt\n
    /// (Originally from ..\fsi\FSIstrings.txt:39)
    static member fsiInterrupt() = (GetStringFunc("fsiInterrupt",",,,") )
    /// \n- Exit...\n
    /// (Originally from ..\fsi\FSIstrings.txt:40)
    static member fsiExit() = (GetStringFunc("fsiExit",",,,") )
    /// - Aborting main thread...
    /// (Originally from ..\fsi\FSIstrings.txt:41)
    static member fsiAbortingMainThread() = (GetStringFunc("fsiAbortingMainThread",",,,") )
    /// Failed to install ctrl-c handler - Ctrl-C handling will not be available. Error was:\n\t%s
    /// (Originally from ..\fsi\FSIstrings.txt:42)
    static member fsiCouldNotInstallCtrlCHandler(a0 : System.String) = (GetStringFunc("fsiCouldNotInstallCtrlCHandler",",,,%s,,,") a0)
    /// --> Referenced '%s'
    /// (Originally from ..\fsi\FSIstrings.txt:43)
    static member fsiDidAHashr(a0 : System.String) = (GetStringFunc("fsiDidAHashr",",,,%s,,,") a0)
    /// --> Referenced '%s' (file may be locked by F# Interactive process)
    /// (Originally from ..\fsi\FSIstrings.txt:44)
    static member fsiDidAHashrWithLockWarning(a0 : System.String) = (GetStringFunc("fsiDidAHashrWithLockWarning",",,,%s,,,") a0)
    /// --> Referenced '%s' (an assembly with a different timestamp has already been referenced from this location, reset fsi to load the updated assembly)
    /// (Originally from ..\fsi\FSIstrings.txt:45)
    static member fsiDidAHashrWithStaleWarning(a0 : System.String) = (GetStringFunc("fsiDidAHashrWithStaleWarning",",,,%s,,,") a0)
    /// --> Added '%s' to library include path
    /// (Originally from ..\fsi\FSIstrings.txt:46)
    static member fsiDidAHashI(a0 : System.String) = (GetStringFunc("fsiDidAHashI",",,,%s,,,") a0)
    /// --> Timing now on
    /// (Originally from ..\fsi\FSIstrings.txt:47)
    static member fsiTurnedTimingOn() = (GetStringFunc("fsiTurnedTimingOn",",,,") )
    /// --> Timing now off
    /// (Originally from ..\fsi\FSIstrings.txt:48)
    static member fsiTurnedTimingOff() = (GetStringFunc("fsiTurnedTimingOff",",,,") )
    /// - Unexpected ThreadAbortException (Ctrl-C) during event handling: Trying to restart...
    /// (Originally from ..\fsi\FSIstrings.txt:49)
    static member fsiUnexpectedThreadAbortException() = (GetStringFunc("fsiUnexpectedThreadAbortException",",,,") )
    /// Failed to resolve assembly '%s'
    /// (Originally from ..\fsi\FSIstrings.txt:50)
    static member fsiFailedToResolveAssembly(a0 : System.String) = (GetStringFunc("fsiFailedToResolveAssembly",",,,%s,,,") a0)
    /// Binding session to '%s'...
    /// (Originally from ..\fsi\FSIstrings.txt:51)
    static member fsiBindingSessionTo(a0 : System.String) = (GetStringFunc("fsiBindingSessionTo",",,,%s,,,") a0)
    /// Microsoft (R) F# Interactive version %s
    /// (Originally from ..\fsi\FSIstrings.txt:52)
    static member fsiProductName(a0 : System.String) = (GetStringFunc("fsiProductName",",,,%s,,,") a0)
    /// F# Interactive for F# %s
    /// (Originally from ..\fsi\FSIstrings.txt:53)
    static member fsiProductNameCommunity(a0 : System.String) = (GetStringFunc("fsiProductNameCommunity",",,,%s,,,") a0)
    /// Prevents references from being locked by the F# Interactive process
    /// (Originally from ..\fsi\FSIstrings.txt:54)
    static member shadowCopyReferences() = (GetStringFunc("shadowCopyReferences",",,,") )

    /// Call this method once to validate that all known resources are valid; throws if not
    static member RunStartupValidation() =
        ignore(GetString("stoppedDueToError"))
        ignore(GetString("fsiUsage"))
        ignore(GetString("fsiInputFiles"))
        ignore(GetString("fsiCodeGeneration"))
        ignore(GetString("fsiErrorsAndWarnings"))
        ignore(GetString("fsiLanguage"))
        ignore(GetString("fsiMiscellaneous"))
        ignore(GetString("fsiAdvanced"))
        ignore(GetString("fsiExceptionRaisedStartingServer"))
        ignore(GetString("fsiUse"))
        ignore(GetString("fsiLoad"))
        ignore(GetString("fsiRemaining"))
        ignore(GetString("fsiHelp"))
        ignore(GetString("fsiExec"))
        ignore(GetString("fsiGui"))
        ignore(GetString("fsiQuiet"))
        ignore(GetString("fsiReadline"))
        ignore(GetString("fsiEmitDebugInfoInQuotations"))
        ignore(GetString("fsiBanner3"))
        ignore(GetString("fsiConsoleProblem"))
        ignore(GetString("fsiInvalidAssembly"))
        ignore(GetString("fsiDirectoryDoesNotExist"))
        ignore(GetString("fsiInvalidDirective"))
        ignore(GetString("fsiLineTooLong"))
        ignore(GetString("fsiTimeInfoMainString"))
        ignore(GetString("fsiTimeInfoGCGenerationLabelSomeShorthandForTheWordGeneration"))
        ignore(GetString("fsiExceptionDuringPrettyPrinting"))
        ignore(GetString("fsiIntroTextHeader1directives"))
        ignore(GetString("fsiIntroTextHashrInfo"))
        ignore(GetString("fsiIntroTextHashIInfo"))
        ignore(GetString("fsiIntroTextHashloadInfo"))
        ignore(GetString("fsiIntroTextHashtimeInfo"))
        ignore(GetString("fsiIntroTextHashhelpInfo"))
        ignore(GetString("fsiIntroTextHashquitInfo"))
        ignore(GetString("fsiIntroTextHeader2commandLine"))
        ignore(GetString("fsiIntroTextHeader3"))
        ignore(GetString("fsiLoadingFilesPrefixText"))
        ignore(GetString("fsiInterrupt"))
        ignore(GetString("fsiExit"))
        ignore(GetString("fsiAbortingMainThread"))
        ignore(GetString("fsiCouldNotInstallCtrlCHandler"))
        ignore(GetString("fsiDidAHashr"))
        ignore(GetString("fsiDidAHashrWithLockWarning"))
        ignore(GetString("fsiDidAHashrWithStaleWarning"))
        ignore(GetString("fsiDidAHashI"))
        ignore(GetString("fsiTurnedTimingOn"))
        ignore(GetString("fsiTurnedTimingOff"))
        ignore(GetString("fsiUnexpectedThreadAbortException"))
        ignore(GetString("fsiFailedToResolveAssembly"))
        ignore(GetString("fsiBindingSessionTo"))
        ignore(GetString("fsiProductName"))
        ignore(GetString("fsiProductNameCommunity"))
        ignore(GetString("shadowCopyReferences"))
        ()
