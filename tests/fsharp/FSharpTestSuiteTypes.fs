module FSharpTestSuiteTypes

open PlatformHelpers

type RunError = 
    | GenericError of string
    | ProcessExecError of (int * string)
    | Skipped of string

type Permutation = 
    | FSI_FILE
    | FSI_STDIN
    | FSI_STDIN_OPT
    | FSI_STDIN_GUI
    | FSC_BASIC
    | FSC_BASIC_64
    | FSC_HW
    | FSC_O3
    | GENERATED_SIGNATURE
    | EMPTY_SIGNATURE
    | EMPTY_SIGNATURE_OPT
    | FSC_OPT_MINUS_DEBUG
    | FSC_OPT_PLUS_DEBUG
    | FRENCH
    | SPANISH
    | AS_DLL
    | WRAPPER_NAMESPACE
    | WRAPPER_NAMESPACE_OPT
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
