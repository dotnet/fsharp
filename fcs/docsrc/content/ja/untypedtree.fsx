(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラサービス：型無し構文木の処理
======================================

このチュートリアルではF#コードに対する型無し抽象構文木
(untyped abstract syntax tree: untyped AST)
を取得する方法、および木全体を走査する方法を紹介します。
この処理を行うことによって、コードフォーマットツールや
基本的なリファクタリングツール、コードナビゲーションツールなどを作成できます。
型無し構文木にはコードの構造に関する情報が含まれていますが、
型情報が含まれていないだけでなく、後で型チェッカーを通すまでは
解決されないような曖昧さも残されています。
また、 [エディタサービス](editor.html) として提供されているAPIと
型無しASTの情報を組み合わせることもできます。

> **注釈:** 以下で使用しているAPIは試験的なもので、将来的に変更される場合があります。
  つまりFSharp.Compiler.Service.dll には既存のものと重複する機能が多数あるため、
  将来的にはもっときちんとした形に変更されます。
  そのため、これらのサービスを使用するAPIには破壊的変更が加えられる可能性があります。


型無しASTの取得
---------------


型無しASTにアクセスするには、 `FSharpChecker` のインスタンスを作成します。
これは型チェックおよびパース用のコンテキストを表す型で、、
スタンドアロンのF#スクリプトファイル(たとえばVisual Studioで開いたファイル)、
あるいは複数ファイルで構成されたロード済みのプロジェクトファイルの
いずれかと結びつきます。
このインスタンスを作成すると、型チェックの最初のステップである
「型無しパース」を実行できます。
次のフェーズは「型有りパース」で、これは [エディタサービス](editor.html) で
使用されるものです。

インタラクティブチェッカーを使用するには、
`FSharp.Compiler.Service.dll` への参照を追加した後、
`SourceCodeServices` 名前空間をオープンします：
*)
#r "FSharp.Compiler.Service.dll"
open System
open Microsoft.FSharp.Compiler.SourceCodeServices
(**

### 型無しパースの実行

型無しパース処理は(それなりの時間がかかる型チェック処理と比較すると)
かなり高速なため、同期的に実行できます。
まず `FSharpChecker` を作成します。

*)
// インタラクティブチェッカーのインスタンスを作成
let checker = FSharpChecker.Create()
(**

ASTを取得するために、ファイル名とソースコードを受け取る関数を用意します
(ファイル名は位置情報のためだけに使用されるもので、存在しなくても構いません)。
まず、コンテキストを表す「インタラクティブチェッカーオプション」を
用意する必要があります。
単純な処理に対しては、 `GetCheckOptionsFromScriptRoot` を使えば
スクリプトファイルのコンテキストを推測させることができます。
そして `UntypedParse` メソッドを呼び出した後、
`ParseTree` プロパティの値を返します:

*)
/// 特定の入力に対する型無し構文木を取得する
let getUntypedTree (file, input) = 
  // 1つのスクリプトファイルから推測される「プロジェクト」用の
  // コンパイラオプションを取得する
  let projectOptions =
      checker.GetProjectOptionsFromScript(file, input) 
      |> Async.RunSynchronously

  let parsingOptions, _errors = checker.GetParsingOptionsFromProjectOptions(projOptions)

  // コンパイラの第1フェーズを実行する
  let untypedRes = 
      checker.ParseFile(file, input, parsingOptions) 
      |> Async.RunSynchronously

  match untypedRes.ParseTree with
  | Some tree -> tree
  | None -> failwith "パース中に何らかの問題が発生しました!"

(**
`FSharpChecker` の詳細については
[ APIドキュメント](../reference/microsoft-fsharp-compiler-sourcecodeservices-FSharpChecker.html)
の他に、F# ソースコードのインラインコメントも参考になるでしょう
( [`service.fsi` のソースコードを参照](https://github.com/fsharp/fsharp/blob/fsharp_31/src/fsharp/service/service.fsi) )。

ASTの走査
---------

抽象構文木は(式やパターン、宣言など)それぞれ異なる文法的要素を表現する、
多数の判別共用体として定義されています。
ASTを理解するには
[`ast.fs`内にあるソースコード](https://github.com/fsharp/fsharp/blob/master/src/fsharp/ast.fs#L464)
の定義を確認する方法が一番よいでしょう。

ASTに関連する要素は以下の名前空間に含まれています:
*)
open Microsoft.FSharp.Compiler.Ast
(**

ASTを処理する場合、異なる文法的要素に対するパターンマッチを行うような
相互再帰関数を多数用意することになります。
サポートすべき要素は非常に多種多様です。
たとえばトップレベル要素としてはモジュールや名前空間の宣言、
モジュール内における(letバインディングや型などの)宣言などがあります。
モジュール内のlet宣言には式が含まれ、さらにこの式に
パターンが含まれていることもあります。

### パターンと式を走査する

まずは式とパターンを走査する関数から始めます。
この関数は要素を走査しつつ、要素に関する情報を画面に表示します。
パターンの場合、入力は `SynPat` 型であり、この型には `Wild` ( `_` パターンを表す)や
`Named` ( `<pat> という名前` のパターン)、
`LongIdent` ( `Foo.Bar` 形式の名前)など、多数のケースがあります。
なお、基本的にパース後のパターンは元のソースコードの見た目よりも複雑になります
(具体的には `Named` がかなり多数現れます):
*)
/// パターンの走査
/// これは let <pat> = <expr> あるいは 'match' 式に対する例です
let rec visitPattern = function
  | SynPat.Wild(_) -> 
      printfn "  .. アンダースコアパターン"
  | SynPat.Named(pat, name, _, _, _) ->
      visitPattern pat
      printfn "  .. 名前 '%s' のパターン" name.idText
  | SynPat.LongIdent(LongIdentWithDots(ident, _), _, _, _, _, _) ->
      let names = String.concat "." [ for i in ident -> i.idText ]
      printfn "  .. 識別子: %s" names
  | pat -> printfn "  .. その他のパターン: %A" pat
(**
この関数は (`bar という名前の (foo, _)` のような、
ネストされたパターンに対応するために) 再帰関数になっていますが、
以降で定義するいずれの関数も呼び出しません
(パターンはその他の文法的な要素を含むことができないからです)。

次の関数は式全体を走査するものです。
これは処理の大部分が行われる関数で、
20以上のケースをカバーすることになるでしょう
( `SynExpr` と入力するとその他のオプションが確認できます)。
以下のコードでは `if .. then ..` と `let .. = ...` という式を
処理する方法だけを紹介しています:
*)
/// 式を走査する。
/// 式に2つあるいは3つの部分式が含まれていた場合('else'の分岐がない場合は2つ)、
/// let式にはパターンおよび2つの部分式が含まれる
let rec visitExpression = function
  | SynExpr.IfThenElse(cond, trueBranch, falseBranchOpt, _, _, _, _) ->
      // すべての部分式を走査
      printfn "条件部:"
      visitExpression cond
      visitExpression trueBranch
      falseBranchOpt |> Option.iter visitExpression 

  | SynExpr.LetOrUse(_, _, bindings, body, _) ->
      // バインディングを走査
      // ('let .. = .. and .. = .. in ...' に対しては複数回走査されることがある)
      printfn "以下のバインディングを含むLetOrUse:"
      for binding in bindings do
        let (Binding(access, kind, inlin, mutabl, attrs, xmlDoc, 
                     data, pat, retInfo, init, m, sp)) = binding
        visitPattern pat 
        visitExpression init
      // 本体の式を走査
      printfn "本体は以下:"
      visitExpression body
  | expr -> printfn " - サポート対象外の式: %A" expr
(**
`visitExpression` 関数はモジュール内のすべてのトップレベル宣言を走査するような
関数から呼ばれることになります。
今回のチュートリアルでは型やメンバーを無視していますが、
これらを走査する場合も `visitExpression` を呼び出すことになるでしょう。

### 宣言を走査する

既に説明したように、1つのファイルに対するASTには多数のモジュールや
名前空間の宣言が(トップレベルノードとして)含まれ、
モジュール内にも(letバインディングや型の)宣言が、
名前空間にも(こちらは単に型だけの)宣言が含まれます。
以下の関数はそれぞれの宣言を走査します。
ただし今回は型やネストされたモジュール、その他の要素については無視して、
トップレベルの(値および関数に対する) `let` バインディングだけを対象にしています:
*)
/// モジュール内の宣言リストを走査する。
/// モジュール内のトップレベルに記述できるすべての要素
/// (letバインディングやネストされたモジュール、型の宣言など)が対象になる。
let visitDeclarations decls = 
  for declaration in decls do
    match declaration with
    | SynModuleDecl.Let(isRec, bindings, range) ->
        // 宣言としてのletバインディングは
        // (visitExpressionで処理したような)式としてのletバインディングと
        // 似ているが、本体を持たない
        for binding in bindings do
          let (Binding(access, kind, inlin, mutabl, attrs, xmlDoc, 
                       data, pat, retInfo, body, m, sp)) = binding
          visitPattern pat 
          visitExpression body         
    | _ -> printfn " - サポート対象外の宣言: %A" declaration
(**
`visitDeclarations` 関数はモジュールや名前空間の宣言のシーケンスを走査する
関数から呼ばれることになります。
このシーケンスはたとえば複数の `namespace Foo` 宣言を含むようなファイルに対応します:
*)
/// すべてのモジュールや名前空間の宣言を走査する
/// (基本的には 'module Foo =' または 'namespace Foo.Bar' というコード)
/// なおファイル中で明示的に定義されていない場合であっても
/// 暗黙的にモジュールまたは名前空間の宣言が存在することに注意。
let visitModulesAndNamespaces modulesOrNss =
  for moduleOrNs in modulesOrNss do
    let (SynModuleOrNamespace(lid, isRec, isMod, decls, xml, attrs, _, m)) = moduleOrNs
    printfn "名前空間またはモジュール: %A" lid
    visitDeclarations decls
(**
以上でASTの要素を(宣言から始まって式やパターンに至るまで)走査するための
関数がそろったので、サンプル入力からASTを取得した後、
上記の関数を実行することができるようになりました。

すべてを組み合わせる
--------------------

既に説明したように、 `getUntypedTree` 関数では `FSharpChecker` を使って
ASTに対する第1フェーズ(パース)を行ってツリーを返しています。
この関数にはF#のソースコードとともに、ファイルのパスを指定する必要があります。
(単に位置情報として利用されるだけなので)
指定先のパスにファイルが存在している必要はなく、
UnixとWindowsどちらの形式でも指定できます:
*)
// コンパイラサービスへのサンプル入力
let input = """
  let foo() = 
    let msg = "Hello world"
    if true then 
      printfn "%s" msg """
// Unix形式のファイル名
let file = "/home/user/Test.fsx"

// サンプルF#コードに対するASTを取得
let tree = getUntypedTree(file, input) 
(**
このコードをF# Interactiveで実行した場合、コンソールに `tree;;` と入力すると、
データ構造に対する文字列表現が表示されることが確認できます。
ツリーには大量の情報が含まれているため、あまり読みやすいものではありませんが、
木が動作する様子を想像することはできるでしょう。

`tree` の返値はやはり判別共用体で、2つのケースに分かれます。
1つはF#のシグネチャファイル( `*.fsi` )を表す `ParsedInput.SigFile` で、
もう1つは通常のソースコード( `*.fsx` または `*.fs` )を表す
`ParsedInput.ImplFile` です。
上記の手順で作成した関数に渡すことができるモジュールや名前空間のシーケンスは
実装ファイルに含まれています:
*)
// 実装ファイルの詳細をチェックする
match tree with
| ParsedInput.ImplFile(implFile) ->
    // 宣言を展開してそれぞれを走査する
    let (ParsedImplFileInput(fn, script, name, _, _, modules, _)) = implFile
    visitModulesAndNamespaces modules
| _ -> failwith "F# インターフェイスファイル (*.fsi) は未サポートです。"
(**
まとめ
------
このチュートリアルでは型無し抽象構文木に対する基本的な走査方法を紹介しました。
このトピックは包括的なものであるため、1つの記事ですべてを説明することは不可能です。
さらに深く理解するためには、型無しASTを活用するツールのよい例として
[Fantomas project](https://github.com/dungpa/fantomas) を参考にするとよいでしょう。
実際には今回参照したような情報と、次のチュートリアルで説明する
[エディタサービス](editor.html) から得られる情報とを
組み合わせて利用することになるでしょう。
*)
