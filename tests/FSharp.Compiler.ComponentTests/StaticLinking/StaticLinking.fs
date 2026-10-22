namespace EmittedIL

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StaticLinking =

    let myRecordLibrary =
        FSharp """
module First
    type MyRecord =
        {
            A: string
            B: decimal
            C: int
            D: float
        }
    let getMyRecord () = { A = "Hello, World!"; B = 1.027m; C = 1028; D = 1.029 }
        """
        |> withOptimize
        |> asLibrary

    let myDiscriminatedUnionLibrary =
        FSharp """
module Second
    type Number = IntNumber of int | DoubleNumber of double
    let getMyRecord () = First.getMyRecord()

    let getMyIntDU() = IntNumber 10

    let getMyDoubleDU() = DoubleNumber 12.0
            """
            |> withOptimize
            |> asLibrary

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``staticlinking_multiple_fs_libraries`` chained =
        let withSubstitutions name compilation =
            let path = TestFramework.getTemporaryFileName()
            File.WriteAllText(path, $"""<linker><assembly fullname="{name}"><resource name="FSharpSignatureData.{name}" action="remove" /></assembly></linker>""")
            compilation |> withName name |> withOptions [ "--compressmetadata-"; $"--resource:{path},ILLink.Substitutions.xml" ]

        let first = myRecordLibrary |> withSubstitutions "First"
        let second =
            myDiscriminatedUnionLibrary |> withSubstitutions "Second"
            |> withReferences [ first.WithStaticLink(chained) ]

        FSharp """open System
open Second

let check expected value =
    let actual = (sprintf "%A" value).Replace("\r\n", "\n").Replace("\n", ";")
    if actual <> expected then failwithf "Expected %s, got %s" expected actual
check "{ A = \"Hello, World!\";  B = 1.027M;  C = 1028;  D = 1.029 }" (getMyRecord())
check "IntNumber 10" (getMyIntDU())
check "DoubleNumber 12.0" (getMyDoubleDU())
let resources = Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()
if Array.filter ((=) "ILLink.Substitutions.xml") resources |> Array.length <> 1 then
    failwith "Static linking must produce one substitutions resource"
let xml =
    use reader = new IO.StreamReader(Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ILLink.Substitutions.xml"))
    reader.ReadToEnd()
for name in ["First"; "Second"; "Final"] do
    if not (xml.Contains("FSharpSignatureData." + name)) then failwith "A linked library lost its removal rule"
        """
        |> asExe
        |> withOptimize
        |> withSubstitutions "Final"
        |> withReferences [
            if not chained then first.WithStaticLink(true)
            second.WithStaticLink(true)
        ]
        |> compileExeAndRun
        |> shouldSucceed
