module FSharpTestSuiteTypes

open PlatformHelpers

type RunError = 
    | GenericError of string
    | ProcessExecError of (string * int * string)
    | Skipped of string

type Permutation = 
    | FSI_FILE
    | FSI_STDIN
    | FSI_STDIN_OPT
    | FSI_STDIN_GUI
    | FSC_BASIC
    | FSC_BASIC_64
    | GENERATED_SIGNATURE
    | FSC_OPT_MINUS_DEBUG
    | FSC_OPT_PLUS_DEBUG
    | SPANISH
    | AS_DLL
    override this.ToString() = (sprintf "%A" this)

type TestConfig = 
    { EnvironmentVariables : Map<string, string>
      ALINK : string
      CORDIR : string
      CORSDK : string
      CSC : string
      csc_flags : string
      FSC : string
      fsc_flags : string
      FSCBinPath : string
      FSCOREDLL20PATH : string
      FSCOREDLLPATH : string
      FSCOREDLLPORTABLEPATH : string
      FSCOREDLLNETCOREPATH : string
      FSCOREDLLNETCORE78PATH : string
      FSCOREDLLNETCORE259PATH : string
      FSDATATPPATH : string
      FSCOREDLLVPREVPATH : string
      FSDIFF : string
      FSI : string
      fsi_flags : string
      GACUTIL : string
      ILDASM : string
      INSTALL_SKU : INSTALL_SKU option
      MSBUILDTOOLSPATH : string option
      NGEN : string
      PEVERIFY : string
      RESGEN : string
      MSBUILD : string option }

and INSTALL_SKU = 
    | Clean
    | DesktopExpress
    | WebExpress
    | Ultimate

type TestRunContext = 
    { Directory: string; 
      Config: TestConfig }
