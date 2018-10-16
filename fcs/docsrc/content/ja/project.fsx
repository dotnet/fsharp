(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラサービス: プロジェクトの分析
======================================

このチュートリアルではF#コンパイラによって提供されるサービスを使用して
プロジェクト全体を分析する方法について紹介します。

> **注意:** 以下で使用しているAPIは試験的なもので、
  最新のnugetパッケージの公開に伴って変更されることがあります。


プロジェクト全体の結果を取得する
--------------------------------

[以前の(型無しASTを使った)チュートリアル](untypedtree.html) と同じく、
まずは `FSharp.Compiler.Service.dll` への参照追加と、適切な名前空間のオープン、
`FSharpChecker` インスタンスの作成を行います:

*)
// F#コンパイラAPIへの参照
#r "FSharp.Compiler.Service.dll"

open System
open System.Collections.Generic
open Microsoft.FSharp.Compiler.SourceCodeServices

// インタラクティブチェッカーのインスタンスを作成
let checker = FSharpChecker.Create()

(**
今回のサンプル入力は以下の通りです:
*)

module Inputs = 
    open System.IO

    let base1 = Path.GetTempFileName()
    let fileName1 = Path.ChangeExtension(base1, ".fs")
    let base2 = Path.GetTempFileName()
    let fileName2 = Path.ChangeExtension(base2, ".fs")
    let dllName = Path.ChangeExtension(base2, ".dll")
    let projFileName = Path.ChangeExtension(base2, ".fsproj")
    let fileSource1 = """
module M

type C() = 
    member x.P = 1

let xxx = 3 + 4
let fff () = xxx + xxx
    """
    File.WriteAllText(fileName1, fileSource1)

    let fileSource2 = """
module N

open M

type D1() = 
    member x.SomeProperty = M.xxx

type D2() = 
    member x.SomeProperty = M.fff()

// 警告を発生させる
let y2 = match 1 with 1 -> M.xxx
    """
    File.WriteAllText(fileName2, fileSource2)


(**
`GetProjectOptionsFromCommandLineArgs` を使用して、
2つのファイルを1つのプロジェクトとして扱えるようにします:
*)

let projectOptions = 
    checker.GetProjectOptionsFromCommandLineArgs
       (Inputs.projFileName,
        [| yield "--simpleresolution" 
           yield "--noframework" 
           yield "--debug:full" 
           yield "--define:DEBUG" 
           yield "--optimize-" 
           yield "--out:" + Inputs.dllName
           yield "--doc:test.xml" 
           yield "--warn:3" 
           yield "--fullpaths" 
           yield "--flaterrors" 
           yield "--target:library" 
           yield Inputs.fileName1
           yield Inputs.fileName2
           let references = 
             [ @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\mscorlib.dll" 
               @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll" 
               @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Core.dll" 
               @"C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\FSharp.Core.dll"]  
           for r in references do
                 yield "-r:" + r |])

(**
そして(ディスク上に保存されたファイルを使用して)
プロジェクト全体をチェックします:
*)

let wholeProjectResults = checker.ParseAndCheckProject(projectOptions) |> Async.RunSynchronously

(**
発生したエラーと警告は以下のようにしてチェックできます:
*)
wholeProjectResults.Errors.Length // 1
wholeProjectResults.Errors.[0].Message.Contains("Incomplete pattern matches on this expression") // true

wholeProjectResults.Errors.[0].StartLineAlternate // 13
wholeProjectResults.Errors.[0].EndLineAlternate // 13
wholeProjectResults.Errors.[0].StartColumn // 15
wholeProjectResults.Errors.[0].EndColumn // 16

(**
推測されたプロジェクトのシグネチャをチェックします:
*)
[ for x in wholeProjectResults.AssemblySignature.Entities -> x.DisplayName ] // ["N"; "M"]
[ for x in wholeProjectResults.AssemblySignature.Entities.[0].NestedEntities -> x.DisplayName ] // ["D1"; "D2"]
[ for x in wholeProjectResults.AssemblySignature.Entities.[1].NestedEntities -> x.DisplayName ] // ["C"]
[ for x in wholeProjectResults.AssemblySignature.Entities.[0].MembersFunctionsAndValues -> x.DisplayName ] // ["y2"]

(**
プロジェクト内の全シンボルを取得することもできます:
*)
let rec allSymbolsInEntities (entities: IList<FSharpEntity>) = 
    [ for e in entities do 
          yield (e :> FSharpSymbol) 
          for x in e.MembersFunctionsAndValues do
             yield (x :> FSharpSymbol)
          for x in e.UnionCases do
             yield (x :> FSharpSymbol)
          for x in e.FSharpFields do
             yield (x :> FSharpSymbol)
          yield! allSymbolsInEntities e.NestedEntities ]

let allSymbols = allSymbolsInEntities wholeProjectResults.AssemblySignature.Entities
(**
プロジェクト全体のチェックが完了した後は、
プロジェクト内の各ファイルに対する個別の結果を取得することもできます。
この処理は即座に完了し、改めてチェックが実行されることもありません。
*)

let backgroundParseResults1, backgroundTypedParse1 = 
    checker.GetBackgroundCheckResultsForFileInProject(Inputs.fileName1, projectOptions) 
    |> Async.RunSynchronously


(**
そしてそれぞれのファイル内にあるシンボルを解決できます:
*)

let xSymbol = 
    backgroundTypedParse1.GetSymbolUseAtLocation(9,9,"",["xxx"])
    |> Async.RunSynchronously

(**
それぞれのシンボルに対して、シンボルへの参照を検索することもできます:
*)
let usesOfXSymbol = wholeProjectResults.GetUsesOfSymbol(xSymbol.Value.Symbol)

(**
推測されたシグネチャ内にあるすべての定義済みシンボルに対して、
それらがどこで使用されているのかを探し出すこともできます:
*)
let allUsesOfAllSignatureSymbols = 
    [ for s in allSymbols do 
         yield s.ToString(), wholeProjectResults.GetUsesOfSymbol(s) ]

(**
(ローカルスコープで使用されているものも含めて)
プロジェクト全体で使用されているすべてのシンボルを確認することもできます：
*)
let allUsesOfAllSymbols = wholeProjectResults.GetAllUsesOfAllSymbols()

(**
また、プロジェクト内のファイルに対して、更新後のバージョンに対して
チェックを実行するようにリクエストすることもできます
(なお [FileSystem API](filesystem.html) を使用していない場合には、
プロジェクト内のその他のファイルがまだディスクから
読み取り中であることに注意してください):

*)
let parseResults1, checkAnswer1 = 
    checker.ParseAndCheckFileInProject(Inputs.fileName1, 0, Inputs.fileSource1, projectOptions) 
    |> Async.RunSynchronously

let checkResults1 = 
    match checkAnswer1 with 
    | FSharpCheckFileAnswer.Succeeded x ->  x 
    | _ -> failwith "想定外の終了状態です"

let parseResults2, checkAnswer2 = 
    checker.ParseAndCheckFileInProject(Inputs.fileName2, 0, Inputs.fileSource2, projectOptions)
    |> Async.RunSynchronously

let checkResults2 = 
    match checkAnswer2 with 
    | FSharpCheckFileAnswer.Succeeded x ->  x 
    | _ -> failwith "想定外の終了状態です"

(**
そして再びシンボルを解決したり、参照を検索したりすることができます:
*)

let xSymbol2 = 
    checkResults1.GetSymbolUseAtLocation(9,9,"",["xxx"]) 
    |> Async.RunSynchronously

let usesOfXSymbol2 = wholeProjectResults.GetUsesOfSymbol(xSymbol2.Value.Symbol)

(**
あるいは(ローカルスコープで使用されているシンボルも含めて)
ファイル中で使用されているすべてのシンボルを検索することもできます：
*)
let allUsesOfAllSymbolsInFile1 = checkResults1.GetAllUsesOfAllSymbolsInFile()

(**
あるいは特定のファイル中で使用されているシンボルを検索することもできます：
*)
let allUsesOfXSymbolInFile1 = checkResults1.GetUsesOfSymbolInFile(xSymbol2.Value.Symbol)

let allUsesOfXSymbolInFile2 = checkResults2.GetUsesOfSymbolInFile(xSymbol2.Value.Symbol)

(**

複数プロジェクトの分析
----------------------

複数のプロジェクトにまたがった参照があるような、
複数のF# プロジェクトを分析したい場合、
それらのプロジェクトを一旦ビルドして、
ProjectOptionsで `-r:プロジェクト-出力-までの-パス.dll` 引数を指定して
プロジェクトの相互参照を設定すると一番簡単です。
しかしこの場合、それぞれのプロジェクトが正しくビルド出来、
DLLファイルが参照可能なディスク上に生成されなければいけません。

たとえばIDEを操作している場合など、状況によっては
DLLのコンパイルが通るようになる前に
プロジェクトを参照したいことがあるでしょう。
この場合はProjectOptionsのReferencedProjectsを設定します。
この値には依存するプロジェクトのオプションを再帰的に指定します。
それぞれのプロジェクト参照にはやはり、
ReferencedProjectsのエントリそれぞれに対応する
`-r:プロジェクト-出力-までの-パス.dll` というコマンドライン引数を
ProjectOptionsに設定する必要があります。

プロジェクト参照が設定されると、ソースファイルからのF#プロジェクト分析処理が
インクリメンタル分析の結果を使用して行われるようになります。
その際にはソースファイルファイルをDLLへとコンパイルする必要はありません。

相互参照を含むようなF#プロジェクトを効率よく分析するには、
ReferencedProjectsを正しく設定した後、
それぞれのプロジェクトを順番通りに分析していくとよいでしょう。

> **注意：** プロジェクトの参照機能は試作段階です。
  プロジェクトの参照を使用すると、依存先のプロジェクトがまだ分析中で、
  要求したサービスがまだ利用できないことがあるため、
  コンパイラサービスの性能が低下することがあります。

> **注意：** アセンブリが型プロバイダーのコンポーネントを含む場合、
  プロジェクト参照機能は利用できません。
  プロジェクトの分析処理を強制しない限りはプロジェクト参照を設定しても
  効果がありません。
  また、分析を強制する場合にはディスク上にDLLが存在しなければいけません。

*)

(**
まとめ
------

これまで説明してきた通り、 `ParseAndCheckProject` を使用すると
シンボルの参照などのようなプロジェクト全体の解析結果にアクセスできるようになります。
シンボルに対する処理の詳細については [シンボル](symbols.html) のページを参照してください。

*)
