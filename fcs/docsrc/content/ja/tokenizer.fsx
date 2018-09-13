(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラサービス：F#トークナイザを使用する
============================================

このチュートリアルではF#言語トークナイザの呼び出し方を紹介します。
F#のソースコードに対して、トークナイザは
コードの各行にあるトークンに関する情報を含んだソースコード行のリストを生成します。
各トークンに対してはトークンの種類や位置を取得したり、
トークンの種類(キーワード、識別子、数値、演算子など)に応じた
色を取得したりすることができます。

> **注意:** 以下で使用しているAPIは実験的なもので、
  新しいnugetパッケージの公開に伴って変更される可能性があります。

トークナイザの作成
------------------

トークナイザを使用するには、 `FSharp.Compiler.Service.dll` への参照を追加した後に
`SourceCodeServices` 名前空間をオープンします：
*)
#r "FSharp.Compiler.Service.dll"
open Microsoft.FSharp.Compiler.SourceCodeServices
(**
すると `FSharpSourceTokenizer` のインスタンスを作成できるようになります。
このクラスには2つの引数を指定します。
最初の引数には定義済みのシンボルのリスト、
2番目の引数にはソースコードのファイル名を指定します。
定義済みのシンボルのリストを指定するのは、
トークナイザが `#if` ディレクティブを処理する必要があるからです。
ファイル名はソースコードの位置を特定する場合にのみ指定する必要があります
(存在しないファイル名でも指定できます):
*)
let sourceTok = FSharpSourceTokenizer([], "C:\\test.fsx")
(**
`sourceTok` オブジェクトを使用することでF#ソースコードの各行を
(繰り返し)トークン化することができます。

F#コードのトークン化
--------------------

トークナイザはソースファイル全体ではなく、行単位で処理を行います。
トークンを取得した後、トークナイザは新しいステートを( `int64` 値として)返します。
この値を使うとF#コードをより効率的にトークン化できます。
つまり、ソースコードが変更された場合もファイル全体を
再度トークン化する必要はありません。
変更された部分だけをトークン化すればよいのです。

### 1行をトークン化する

1行をトークン化するには、先ほど作成した `FSharpSourceTokenizer` オブジェクトに対して
`CreateLineTokenizer` を呼び、 `FSharpLineTokenizer` を作成します:
*)
let tokenizer = sourceTok.CreateLineTokenizer("let answer=42")
(**
そして `tokenizer` の `ScanToken` を繰り返し `None` を返すまで
(つまり最終行に到達するまで)繰り返し呼び出すような単純な再帰関数を用意します。
この関数が成功すると、必要な詳細情報をすべて含んだ `FSharpTokenInfo` オブジェクトが
返されます:
*)
/// F#コード1行をトークン化します
let rec tokenizeLine (tokenizer:FSharpLineTokenizer) state =
  match tokenizer.ScanToken(state) with
  | Some tok, state ->
      // トークン名を表示
      printf "%s " tok.TokenName
      // 新しい状態で残りをトークン化
      tokenizeLine tokenizer state
  | None, state -> state
(**
この関数は、複数行コードや複数行コメント内の前方の行をトークン化する場合に
必要となるような新しい状態を返します。
初期値としては `0L` を指定します:
*)
tokenizeLine tokenizer 0L
(**
この結果は LET WHITESPACE IDENT EQUALS INT32 という
トークン名のシーケンスになります。
`FSharpTokenInfo` にはたとえば以下のような興味深いプロパティが多数あります:

 - `CharClass` および `ColorClass` はF#コードを色づけする場合に使用できるような、
   トークンのカテゴリに関する情報を返します。
 - `LeftColumn` および `RightColumn` は行内におけるトークンの位置を返します。
 - `TokenName` は(F# レキサ内で定義された)トークンの名前を返します。

なおトークナイザはステートフルであることに注意してください。
つまり、1行を複数回トークン化したい場合にはその都度 `CreateLineTokenizer` を
呼び出す必要があります。

### サンプルコードのトークン化

トークナイザをもっと長いサンプルコードやファイル全体に対して実行する場合、
サンプル入力を `string` のコレクションとして読み取る必要があります:
*)
let lines = """
  // Hello world
  let hello() =
     printfn "Hello world!" """.Split('\r','\n')
(**
複数行の入力値をトークン化する場合も、現在の状態を保持するような
再帰関数が必要になります。
以下の関数はソースコード行を文字列のリストとして受け取ります
(また、行番号および現在の状態も受け取ります)。
各行に対して新しいトークナイザを作成して、
直前の行における **最後** の状態を使って `tokenizeLine` を呼び出します:
*)
/// 複数行のコードに対してトークンの名前を表示します
let rec tokenizeLines state count lines = 
  match lines with
  | line::lines ->
      // トークナイザを作成して1行をトークン化
      printfn "\nLine %d" count
      let tokenizer = sourceTok.CreateLineTokenizer(line)
      let state = tokenizeLine tokenizer state
      // 新しい状態を使って残りをトークン化
      tokenizeLines state (count+1) lines
  | [] -> ()
(**
ここでは単に(先ほど定義した) `tokenizeLine` を呼び出して、
各行にあるすべてのトークンの名前を表示しています。
この関数は先と同じく、初期状態の値 `0L` と、1行目を表す `1` を
指定して呼び出すことができます:
*)
lines
|> List.ofSeq
|> tokenizeLines 0L 1
(**
重要ではない部分(各行の先頭にある空白文字や、1行目のように空白文字しかない行)
を除けば、このコードを実行すると以下のような出力になります:

    [lang=text]
    Line 1
      LINE_COMMENT LINE_COMMENT (...) LINE_COMMENT 
    Line 2
      LET WHITESPACE IDENT LPAREN RPAREN WHITESPACE EQUALS 
    Line 3
      IDENT WHITESPACE STRING_TEXT (...) STRING_TEXT STRING 

注目すべきは、単一行コメントや文字列に対して、
トークナイザが複数回(大まかにいって単語単位で) `LINE_COMMENT` や
`STRING_TEXT` を返しているところです。
したがって、コメントや文字列全体をテキストとして取得したい場合には
それぞれのトークンを連結する必要があります。
*)