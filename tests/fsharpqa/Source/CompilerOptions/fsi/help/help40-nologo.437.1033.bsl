
Usage: fsiAnyCpu <options> [script.fsx [<arguments>]]


                - INPUT FILES -
--use:<file>                             Use the given file on startup as
                                         initial input
--load:<file>                            #load the given file on startup
--reference:<file>                       Reference an assembly (Short form:
                                         -r)
--compilertool:<file>                    Reference an assembly or directory
                                         containing a design time tool (Short
                                         form: -t)
-- ...                                   Treat remaining arguments as command
                                         line arguments, accessed using
                                         fsi.CommandLineArgs


                - CODE GENERATION -
--debug[+|-]                             Emit debug information (Short form:
                                         -g) (on by default)
--debug:{full|pdbonly|portable|embedded} Specify debugging type: full,
                                         portable, embedded, pdbonly.
                                         ('pdbonly' is the default if no
                                         debugging type specified and enables
                                         attaching a debugger to a running
                                         program, 'portable' is a
                                         cross-platform format, 'embedded' is
                                         a cross-platform format embedded
                                         into the output file).
--optimize[+|-]                          Enable optimizations (Short form:
                                         -O) (on by default)
--tailcalls[+|-]                         Enable or disable tailcalls (on by
                                         default)
--deterministic[+|-]                     Produce a deterministic assembly
                                         (including module version GUID and
                                         timestamp) (off by default)
--realsig[+|-]                           Generate assembly with IL visibility
                                         that matches the source code
                                         visibility (off by default)
--pathmap:<path=sourcePath;...>          Maps physical paths to source path
                                         names output by the compiler
--crossoptimize[+|-]                     Enable or disable cross-module
                                         optimizations (on by default)
--reflectionfree                         Disable implicit generation of
                                         constructs using reflection


                - ERRORS AND WARNINGS -
--warnaserror[+|-]                       Report all warnings as errors (off
                                         by default)
--warnaserror[+|-]:<warn;...>            Report specific warnings as errors
--warn:<n>                               Set a warning level (0-5)
--nowarn:<warn;...>                      Disable specific warning messages
--warnon:<warn;...>                      Enable specific warnings that may be
                                         off by default
--checknulls[+|-]                        Enable nullness declarations and
                                         checks (off by default)
--consolecolors[+|-]                     Output warning and error messages in
                                         color (on by default)


                - LANGUAGE -
--langversion:?                          Display the allowed values for
                                         language version.
--langversion:{version|latest|preview}   Specify language version such as
                                         'latest' or 'preview'.
--checked[+|-]                           Generate overflow checks (off by
                                         default)
--define:<string>                        Define conditional compilation
                                         symbols (Short form: -d)
--mlcompatibility                        Ignore ML compatibility warnings
--strict-indentation[+|-]                Override indentation rules implied
                                         by the language version (off by
                                         default)


                - MISCELLANEOUS -
--nologo                                 Suppress compiler copyright message
--version                                Display compiler version banner and
                                         exit
--help                                   Display this usage message (Short
                                         form: -?)


                - ADVANCED -
--codepage:<n>                           Specify the codepage used to read
                                         source files
--utf8output                             Output messages in UTF-8 encoding
--preferreduilang:<string>               Specify the preferred output
                                         language culture name (e.g. es-ES,
                                         ja-JP)
--fullpaths                              Output messages with fully qualified
                                         paths
--lib:<dir;...>                          Specify a directory for the include
                                         path which is used to resolve source
                                         files and assemblies (Short form:
                                         -I)
--simpleresolution                       Resolve assembly references using
                                         directory-based rules rather than
                                         MSBuild resolution
--targetprofile:<string>                 Specify target framework profile of
                                         this assembly. Valid values are
                                         mscorlib, netcore or netstandard.
                                         Default - mscorlib
--clearResultsCache                      Clear the package manager results
                                         cache
--exec                                   Exit fsi after loading the files or
                                         running the .fsx script given on the
                                         command line
--gui[+|-]                               Execute interactions on a Windows
                                         Forms event loop (on by default)
--quiet                                  Suppress fsi writing to stdout
--readline[+|-]                          Support TAB completion in console
                                         (on by default)
--quotations-debug[+|-]                  Emit debug information in quotations
                                         (off by default)
--shadowcopyreferences[+|-]              Prevents references from being
                                         locked by the F# Interactive process
                                         (off by default)
--multiemit[+|-]                         Emit multiple assemblies (on by
                                         default)

See https://learn.microsoft.com/dotnet/fsharp/language-reference/fsharp-interactive-options for more details.
