(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
インタラクティブサービス: F# Interactiveの組み込み
==================================================

このチュートリアルでは、独自のアプリケーションに
F# Interactiveを組み込む方法について紹介します。
F# Interactiveは対話式のスクリプティング環境で、
F#コードを高度に最適化されたILコードへとコンパイルしつつ、
それを即座に実行することができます。
F# Interactiveサービスを使用すると、独自のアプリケーションに
F#の評価機能を追加できます。

> **注意:** F# Interactiveは様々な方法で組み込むことができます。
  最も簡単な方法は `fsi.exe` プロセスとの間で標準入出力経由でやりとりする方法です。
  このチュートリアルではF# Interactiveの機能を.NET APIで
  直接呼び出す方法について紹介します。
  ただし入力用のコントロールを備えていない場合、別プロセスでF# Interactiveを
  起動するのはよい方法だといえます。
  理由の1つとしては `StackOverflowException` を処理する方法がないため、
  出来の悪いスクリプトによってはホストプロセスが停止させられてしまう
  場合があるからです。
  **.NET APIを通じてF# Interactiveを呼び出すとしても、 `--shadowcopyreferences`
  オプションは無視されることを覚えておきましょう。**
  詳細な議論については、[このスレッド](https://github.com/fsharp/FSharp.Compiler.Service/issues/292)
  に目を通してみてください。
  **注意:** もし`FSharp.Core.dll` が見つからないというエラーが出て `FsiEvaluationSession.Create`
  に失敗した場合、 `FSharp.Core.sigdata` と `FSharp.Core.optdata` というファイルを追加してください。
  詳しい内容は[こちら](https://fsharp.github.io/FSharp.Compiler.Service/ja/corelib.html)
  にあります。

しかしそれでもF# InteractiveサービスにはF# Interactiveを実行ファイルに埋め込んで
実行出来る(そしてアプリケーションの各機能とやりとり出来る)、あるいは
機能限定されたF#コード(たとえば独自のDSLによって生成されたコード)だけを
実行させることが出来るという便利さがあります。

F# Interactiveの開始
--------------------

まずF# Interactiveサービスを含むライブラリへの参照を追加します:
*)

#r "FSharp.Compiler.Service.dll"
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Interactive.Shell

(**
F# Interactiveとやりとりするには、入出力を表すストリームを作成する必要があります。
これらのストリームを使用することで、
いくつかのF#コードに対する評価結果を後から出力することができます:
*)
open System
open System.IO
open System.Text

// 入出力のストリームを初期化
let sbOut = new StringBuilder()
let sbErr = new StringBuilder()
let inStream = new StringReader("")
let outStream = new StringWriter(sbOut)
let errStream = new StringWriter(sbErr)

// コマンドライン引数を組み立てて、FSIセッションを開始する
let argv = [| "C:\\fsi.exe" |]
let allArgs = Array.append argv [|"--noninteractive"|]

let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
let fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream)  

(**
コードの評価および実行
----------------------

F# Interactiveサービスにはコードを評価するためのメソッドがいくつか用意されています。
最初の1つは `EvalExpression` で、式を評価してその結果を返します。
結果には戻り値が( `obj` として)含まれる他、値に対して静的に推論された型も含まれます:
*)
/// 式を評価して結果を返す
let evalExpression text =
  match fsiSession.EvalExpression(text) with
  | Some value -> printfn "%A" value.ReflectionValue
  | None -> printfn "結果が得られませんでした！"

(**
これは引数に文字列を取り、それをF#コードとして評価(つまり実行)します。
*)
evalExpression "42+1" // '43' を表示する

(**
これは以下のように強く型付けされた方法で使うことができます:
*)

/// 式を評価して、強く型付けされた結果を返す
let evalExpressionTyped<'T> (text) =
    match fsiSession.EvalExpression(text) with
    | Some value -> value.ReflectionValue |> unbox<'T>
    | None -> failwith "結果が得られませんでした！"

evalExpressionTyped<int> "42+1"  // '43' になる


(**
`EvalInteraction` メソッドは画面出力機能や宣言、
F#の式としては不正なものの、F# Interactiveコンソールには入力できるようなものなど、
副作用を伴う命令を評価する場合に使用できます。
たとえば `#time "on"` (あるいはその他のディレクティブ)や `open System` 、
その他の宣言やトップレベルステートメントなどが該当します。
指定するコードの終端に `;;` を入力する必要はありません。
実行したいコードだけを入力します:
*)
fsiSession.EvalInteraction "printfn \"bye\""


(**
`EvalScript` メソッドを使用すると、完全な .fsx スクリプトを評価することができます。
*)

File.WriteAllText("sample.fsx", "let twenty = 10 + 10")
fsiSession.EvalScript "sample.fsx"

(**
例外処理
--------

コードに型チェックの警告やエラーがあった場合、または評価して例外で失敗した場合、
`EvalExpression` 、 `EvalInteraction` そして `EvalScript` ではあまりうまく処理されません。
これらのケースでは、 `EvalExpressionNonThrowing` 、 `EvalInteractionNonThrowing`
そして `EvalScriptNonThrowing` を使うことが出来ます。
これらは結果と `FSharpErrorInfo` 値の配列の組を返します。
これらはエラーと警告を表します。結果の部分は実際の結果と例外のいずれかを表す
`Choice<_,_>` です。

`EvalExpression` および `EvalExpressionNonThrowing` の結果部分は
オプションの `FSharpValue` 値です。
その値が存在しない場合、式が .NET オブジェクトとして表現できる具体的な結果を
持っていなかったということを指し示しています。
この状況は実際には入力されたどんな通常の式に対しても発生すべきではなく、
ライブラリ内で使われるプリミティブ値に対してのみ発生すべきです。
*)

File.WriteAllText("sample.fsx", "let twenty = 'a' + 10.0")
let result, warnings = fsiSession.EvalScriptNonThrowing "sample.fsx"

// 結果を表示する
match result with 
| Choice1Of2 () -> printfn "チェックと実行はOKでした"
| Choice2Of2 exn -> printfn "実行例外: %s" exn.Message


(**
は次のようになります:

    実行例外: Operation could not be completed due to earlier error
*)

// エラーと警告を表示する
for w in warnings do 
   printfn "警告 %s 場所 %d,%d" w.Message w.StartLineAlternate w.StartColumn

(**
は次のようになります:

    警告 The type 'float' does not match the type 'char' 場所 1,19
    警告 The type 'float' does not match the type 'char' 場所 1,17

式に対しては:
*)


let evalExpressionTyped2<'T> text =
   let res, warnings = fsiSession.EvalExpressionNonThrowing(text)
   for w in warnings do 
       printfn "警告 %s 場所 %d,%d" w.Message w.StartLineAlternate w.StartColumn 
   match res with 
   | Choice1Of2 (Some value) -> value.ReflectionValue |> unbox<'T>
   | Choice1Of2 None -> failwith "null または結果がありません"
   | Choice2Of2 (exn:exn) -> failwith (sprintf "例外 %s" exn.Message)

evalExpressionTyped2<int> "42+1"  // '43' になる


(**
並列実行
--------

デフォルトでは `EvalExpression` に渡したコードは即時実行されます。
並列に実行するために、タスクを開始する計算を投入します:
*)

open System.Threading.Tasks

let sampleLongRunningExpr = 
    """
async { 
    // 実行したいコード
    do System.Threading.Thread.Sleep 5000
    return 10 
}
  |> Async.StartAsTask"""

let task1 = evalExpressionTyped<Task<int>>(sampleLongRunningExpr)  
let task2 = evalExpressionTyped<Task<int>>(sampleLongRunningExpr)

(**
両方の計算がいま開始しました。結果を取得することが出来ます:
*)


task1.Result // 完了後に結果が出てくる (最大5秒)
task2.Result // 完了後に結果が出てくる (最大5秒)

(**
評価コンテキスト内での型チェック
--------------------------------

F# Interactiveの一連のスクリプティングセッション中で
コードの型チェックを実行したいような状況を考えてみましょう。
たとえばまず宣言を評価します:
*)

fsiSession.EvalInteraction "let xxx = 1 + 1"

(**

次に部分的に完全な `xxx + xx` というコードの型チェックを実行したいとします:
*)

let parseResults, checkResults, checkProjectResults = fsiSession.ParseAndCheckInteraction("xxx + xx")

(** 
`parseResults` と `checkResults` はそれぞれ [エディタ](editor.html)
のページで説明している `ParseFileResults` と `CheckFileResults` 型です。
たとえば以下のようなコードでエラーを確認出来ます:
*)
checkResults.Errors.Length // 1

(** 
コードはF# Interactiveセッション内において、その時点までに実行された
有効な宣言からなる論理的な型コンテキストと結びつく形でチェックされます。

また、宣言リスト情報やツールチップテキスト、シンボルの解決といった処理を
要求することもできます:

*)
open Microsoft.FSharp.Compiler

// ツールチップを取得する
checkResults.GetToolTipTextAlternate(1, 2, "xxx + xx", ["xxx"], FSharpTokenTag.IDENT) 

checkResults.GetSymbolUseAtLocation(1, 2, "xxx + xx", ["xxx"]) // シンボル xxx
  
(**
'fsi'オブジェクト
-----------------

スクリプトのコードが'fsi'オブジェクトにアクセスできるようにしたい場合、
このオブジェクトの実装を明示的に渡さなければなりません。
通常、FSharp.Compiler.Interactive.Settings.dll由来の1つが使われます。
*)

let fsiConfig2 = FsiEvaluationSession.GetDefaultConfiguration(fsi)

(**
収集可能なコード生成
--------------------

FsiEvaluationSessionを使用してコードを評価すると、
.NET の動的アセンブリを生成し、他のリソースを使用します。
`collectible=true` を渡すことで、生成されたコードを収集可能に出来ます。
しかしながら、例えば `EvalExpression` から返される `FsiValue` のような型を必要とする未解放のオブジェクト参照が無く、
かつ `FsiEvaluationSession` を破棄したに違いない場合に限ってコードが収集されます。
[収集可能なアセンブリに対する制限](https://msdn.microsoft.com/ja-jp/library/dd554932%28v=vs.110%29.aspx#Anchor_1)
も参照してください。

以下の例は200個の評価セッションを生成しています。 `collectible=true` と `use session = ...`
の両方を使っていることに気をつけてください。

収集可能なコードが正しく動いた場合、全体としてのリソース使用量は
評価が進んでも線形には増加しないでしょう。
*)

let collectionTest() = 

    for i in 1 .. 200 do
        let defaultArgs = [|"fsi.exe";"--noninteractive";"--nologo";"--gui-"|]
        use inStream = new StringReader("")
        use outStream = new StringWriter()
        use errStream = new StringWriter()

        let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
        use session = FsiEvaluationSession.Create(fsiConfig, defaultArgs, inStream, outStream, errStream, collectible=true)
        
        session.EvalInteraction (sprintf "type D = { v : int }")
        let v = session.EvalExpression (sprintf "{ v = 42 * %d }" i)
        printfn "その %d, 結果 = %A" i v.Value.ReflectionValue

// collectionTest()  <-- このようにテストを実行する