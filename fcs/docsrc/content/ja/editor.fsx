(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラサービス: エディタサービス
====================================

このチュートリアルはF#コンパイラによって公開されるエディタサービスの
使用方法についてのデモです。
このAPIにより、Visual StudioやXamarin Studio、EmacsなどのF#エディタ内において、
自動補完機能やツールチップ表示、引数情報のヘルプ表示、括弧の補完などの機能を
実装することができます
(詳細については [fsharpbindings](https://github.com/fsharp/fsharpbinding) のプロジェクトを参照してください)。
[型無しASTを使用するチュートリアル](untypedtree.html) と同じく、
今回も `FSharpChecker` オブジェクトを作成するところから始めます。

> **注意:** 以下で使用しているAPIは試験的なもので、最新バージョンのnugetパッケージの
公開に伴って変更されることがあります。

サンプルソースコードの型チェック
--------------------------------

[前回の(型無しASTを使った)チュートリアル](untypedtree.html) と同じく、
`FSharp.Compiler.Service.dll` への参照を追加した後に特定の名前空間をオープンし、
`FSharpChecker` のインスタンスを作成します:

*)
// F#コンパイラAPIを参照
#r "FSharp.Compiler.Service.dll"

open System
open Microsoft.FSharp.Compiler.SourceCodeServices

// インタラクティブチェッカーのインスタンスを作成
let checker = FSharpChecker.Create()

(**

[前回](untypedtree.html) 同様、
コンパイラに渡されるファイルとしては特定の入力値だけであるという
コンテキストを想定するため、 `GetCheckOptionsFromScriptRoot` を使います
(この入力値はコンパイラによってスクリプトファイル、
あるいはスタンドアロンのF#ソースコードとみなされます)。

*)
// サンプルの入力となる複数行文字列
let input = 
"""
open System

let foo() = 
let msg = String.Concat("Hello"," ","world")
if true then 
printfn "%s" msg.
"""
// 入力値の分割とファイル名の定義
let inputLines = input.Split('\n')
let file = "/home/user/Test.fsx"

let projOptions = checker.GetProjectOptionsFromScript(file, input) |> Async.RunSynchronously

let parsingOptions, _errors = checker.GetParsingOptionsFromProjectOptions(projOptions)

(**

型チェックを実行するには、まず `ParseFile` を使って
入力値をパースする必要があります。
このメソッドを使うと [型無しAST](untypedtree.html) にアクセスできるようになります。
しかし今回は完全な型チェックを実行するため、続けて `CheckFileInProject`
を呼び出す必要があります。
このメソッドは `ParseFile` の結果も必要とするため、
たいていの場合にはこれら2つのメソッドをセットで呼び出すことになります。

*)
// パースを実行
let parseFileResults =
checker.ParseFile(file, input, parsingOptions)
|> Async.RunSynchronously
(**
`TypeCheckResults` に備えられた興味深い機能の紹介に入る前に、
サンプル入力に対して型チェッカーを実行する必要があります。
F#コードにエラーがあった場合も何らかの型チェックの結果が返されます
(ただし間違って「推測された」結果が含まれることがあります)。
*)        

// 型チェックを実行
let checkFileAnswer = 
checker.CheckFileInProject(parseFileResults, file, 0, input, projOptions) 
|> Async.RunSynchronously

(**
あるいは `ParseAndCheckFileInProject` を使用すれば1つの操作で両方のチェックを行うことができます：
*)

let parseResults2, checkFileAnswer2 =
checker.ParseAndCheckFileInProject(file, 0, input, projOptions)
|> Async.RunSynchronously

(**
この返り値は `CheckFileAnswer` 型で、この型に機能的に興味深いものが揃えられています...
*)

let checkFileResults = 
match checkFileAnswer with
| FSharpCheckFileAnswer.Succeeded(res) -> res
| res -> failwithf "パースが完了していません... (%A)" res

(**

今回は単に(状況に応じて)「Hello world」と表示するだけの
単純な関数の型をチェックしています。
最終行では値 `msg` に対する補完リストを表示することができるように、
`msg.` というようにドットを追加している点に注意してください
(今回の場合は文字列型に対する様々なメソッドが期待されます)。


型チェックの結果を使用する
--------------------------

では `TypeCheckResults` 型で公開されているAPIをいくつか見ていきましょう。
一般的に、F#ソースコードエディタサービスの実装に必要な機能は
ほとんどこの型に備えられています。

### ツールチップの取得

ツールチップを取得するには `GetToolTipTextAlternate` メソッドを使用します。
このメソッドには行数と文字オフセットを指定します。
いずれも0から始まる数値です。
サンプルコードでは3行目(0行目は空白行)、インデックス7にある文字 `f` から始まる関数
`foo` のツールチップを取得しています
(ツールチップは識別子の中であれば任意の位置で機能します)。

またこのメソッドにはトークンタグを指定する必要もあります。
トークンタグは一般的には `IDENT` を指定して、識別子に対する
ツールチップが取得できるようにします
(あるいは `#r "..."` を使用している場合にはアセンブリの完全パスを表示させるように
することもできるでしょう)。

*)
// 最後の引数に指定する、IDENTトークンのタグを取得
open Microsoft.FSharp.Compiler
let identToken = Parser.tagOfToken(Parser.token.IDENT("")) 

// 特定の位置におけるツールチップを取得
let tip = checkFileResults.GetToolTipTextAlternate(4, 7, inputLines.[1], ["foo"], identToken)
printfn "%A" tip

(**

> **注意：** `GetToolTipTextAlternate` は古い関数 `GetToolTipText` に代わるものです。
`GetToolTipText` は0から始まる行番号を受け取るようになっていたため、非推奨になりました。

この関数には位置とトークンの種類の他にも、
(ソースコードの変更時に役立つように)特定行の現在の内容と、
現時点における完全修飾された `名前` を表す文字列のリストを指定する必要があります。
たとえば完全修飾名 `System.Random` という名前を持った識別子 `Random` に対する
ツールチップを取得する場合、 `Random` という文字列が現れる場所の他に、
`["System"; "Random"]` という値を指定する必要があります。

返り値の型は `ToolTipText` で、この型には `ToolTipElement` という
判別共用体が含まれます。
この共用体は、コンパイラによって返されたツールチップの種類に応じて異なります。

### 自動補完リストの取得

次に紹介する `TypeCheckResults` のメソッドを使用すると、
特定の位置における自動補完機能を実装できます。
この機能は任意の識別子上、
あるいは(特定のスコープ内で利用可能な名前の一覧を取得する場合には)任意のスコープ、
あるいは特定のオブジェクトにおけるメンバーリストを取得する場合には
`.` の直後で呼び出すことができます。
今回は文字列の値 `msg` に対するメンバーリストを取得することにします。

そのためには最終行( `printfn "%s" msg.` で終わっている行)にある
シンボル `.` の位置を指定して `GetDeclarationListInfo` を呼び出します。
オフセットは1から始まるため、位置は `7, 23` になります。
また、テキストが変更されていないことを表す関数と、
現時点において補完する必要がある識別子を指定する必要もあります。
*)
// 特定の位置における宣言(自動補完)を取得する
let decls = 
checkFileResults.GetDeclarationListInfo
(Some parseFileResults, 7, 23, inputLines.[6], [], "msg", fun _ -> false)
|> Async.RunSynchronously

// 利用可能な項目を表示
for item in decls.Items do
printfn " - %s" item.Name
(**

> **注意：** `GetDeclarationListInfo` は古い関数 `GetDeclarations` に代わるものです。
`GetDeclarations` は0から始まる行番号を受け取るようになっていたため、非推奨になりました。
また、将来的には現在の `GetDeclarations` が削除され、 `GetDeclarationListInfo` が
`GetDeclarations` になる予定です。

コードを実行してみると、 `Substring` や `ToUpper` 、 `ToLower` といった
文字列に対するいつものメソッドのリストが取得できていることでしょう。
`GetDeclarations` の5,6番目の引数( `[]` および `"msg"` )には
自動補完用のコンテキストを指定します。
今回の場合は完全名 `msg` に対する補完を行いましたが、
たとえば `["System"; "Collections"]` と `"Generic"` というように
完全修飾された名前空間を指定して補完リストを取得することもできます。

### 引数の情報を取得する

次に一般的なエディタの機能としては、メソッドのオーバーロードに
関する情報を提供するというものでしょう。
サンプルコード中では多数のオーバーロードを持った `String.Concat` を使っています。
このオーバーロード一覧は `GetMethods` で取得できます。
先ほどと同じく、このメソッドには対象とする項目の位置を0基準のオフセットで指定し
(今回は `String.Concat` 識別子の右側の終端)、
識別子もやはり指定します
(これにより、コンパイラはソースコードが変更された場合でも最新の情報に追従できます):

*)
//String.Concatメソッドのオーバーロードを取得する
let methods = 
checkFileResults.GetMethodsAlternate(5, 27, inputLines.[4], Some ["String"; "Concat"]) |> Async.RunSynchronously

// 連結された引数リストを表示
for mi in methods.Methods do
[ for p in mi.Parameters -> p.Display ]
|> String.concat ", " 
|> printfn "%s(%s)" methods.MethodName
(**
ここでは `Display` プロパティを使用することで各引数に対する
アノテーションを取得しています。
このプロパティは `arg0: obj` あるいは `params args: obj[]` 、
`str0: string, str1: string` といった情報を返します。
これらの引数を連結した後、メソッド名とメソッドの型情報とともに表示させています。
*)

(** 

## 非同期操作と即時操作

`CheckFileInProject` が非同期操作であることを気にされる人もいるかもしれません。
これはつまり、F#コードの型チェックにはある程度時間がかかることを示唆しています。
F#コンパイラは型チェックを(自動的に)バックグラウンドで処理を進めているため、
`CheckFileInProject` メソッドを呼び出すと非同期操作が返されることになります。

また、 `CheckFileInProjectIfReady` というメソッドもあります。
このメソッドは、型チェックの操作が即座に開始できない場合、
つまりプロジェクト内の他のファイルがまだ型チェックされていない場合には
処理が即座に返されます。
この場合、バックグラウンドワーカーは一定期間他の作業を進めるか、
`FileTypeCheckStateIsDirty` イベントが発生するまでは
ファイルに対する型チェックを諦めるか、どちらか選択することになります。

> [fsharpbinding](https://github.com/fsharp/fsharpbinding) プロジェクトには
1つのF#エージェント経由ですべてのリクエストをバックグラウンドワークとして
処理するような、より複雑な具体例も含まれています。
エディタの機能を実装する方法としてはこちらのほうが適切です。

*)


(**
まとめ
------

`CheckFileAnswer` にはチュートリアルで紹介していないような便利なメソッドが
多数揃えられています。
これらを使用すれば特定の識別子に対する宣言の位置を取得したり、
付加的な色情報を取得したりすることができます
(F# 3.1では式ビルダーの識別子やクエリ演算子も着色表示されます)。

最後に、直接.NET APIを呼び出すことができないようなエディタに対するサポート機能を
実装する場合、ここで紹介した様々な機能を
[FSharp.AutoComplete](https://github.com/fsharp/fsharpbinding/tree/master/FSharp.AutoComplete)
プロジェクトのコマンドラインインターフェイス経由で呼び出すこともできます。
*)
