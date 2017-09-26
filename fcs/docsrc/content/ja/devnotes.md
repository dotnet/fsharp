開発者用メモ
============

F#コンパイラの修正版クローンではクライアントの編集機能やF#コンパイラの埋め込み、
F# Interactiveをサービスとして動作させるための機能が追加されています。

## コンポーネント

まず `FSharp.Compiler.Service.dll` というコンポーネントがあります。
このコンポーネントにはリファクタリングやその他の編集ツールが完全なF# ASTやパーサ機能を利用できるように
可視性を変更するというマイナーな変更だけが加えられています。
主な狙いとしては、メインコンパイラの安定版かつドキュメントが備えられたフォークを用意することにより、
このコンポーネントにある共通コードを様々なツールで共有できるようにすることです。

2つ目のコンポーネントはF# Interactiveをサービスとして組み込めるようにするためのもので、
`fsi.exe` のソースコードに多数の変更が加えられており、
`EvalExpression` や `EvalInteraction` といった関数が追加されています。

このレポジトリは以下の点を除けば 'fsharp' と **同一** です:

  - `FSharp.Compiler.Service.dll` のビルド、特に以下の点に関する変更:
    - アセンブリ名の変更
    - `FSharp.Compiler.Service.dll` のみビルドされる
    - ブートストラッパーやプロトコンパイラを使用しない。
      F#コンパイラがインストール済みであることを想定。

  - FAKEを使用するビルドスクリプト。
    すべてのコードのビルドとNuGetパッケージ、ドキュメントの生成、
    NuGetパッケージの配布に必要なファイルの生成などがFAKEによって行われる。
    ([F# プロジェクト スキャフォールド](https://github.com/fsprojects/FSharp.ProjectScaffold) に準拠)

  - 新機能追加のためにコンパイラのソースコードを変更。
    また、評価用関数を実装するためにF# Interactiveサービスに対する変更を追加。

  - F#編集用クライアントで使用されるAPIを改善するためにコンパイラのソースコードを変更。

  - コンパイラサービスAPIに新機能を追加するためにコンパイラのソースコードを変更。

`fsharp/fsharp` のレポジトリに言語あるいはコンパイラが追加コミットされた場合、
それらはこのレポジトリにもマージされるべきで、同時に新しいNuGetパッケージもリリースする必要があります。

## ビルドとNuGet

ビルドの手順は [F# プロジェクト スキャフォールド](https://github.com/fsprojects/FSharp.ProjectScaffold)
で推奨されているものに準じます。
プロジェクトを独自にビルドする場合、以下の手順に従ってください:

    [lang=text]
    git clone https://github.com/fsharp/FSharp.Compiler.Service
    cd FSharp.Compiler.Service

次に、(Windowsであれば) `build.cmd` または(LinuxやMac OSであれば) `build.sh` を実行してすべてをビルドします。
ファイルは `bin` ディレクトリ内に出力されます。
ドキュメントやNuGetパッケージもビルドしたい場合には `build Release` を実行します
(このコマンドはGitHub上のドキュメントを更新しようとしますが、GitHubのレポジトリに適切な権限を持っている場合にのみ有効です)。

## クライアント

このコンポーネントは以下のようなツールで使用されています:

 * [Fantomas](https://github.com/dungpa/fantomas) - F# コードフォーマットツール
 * [Fsharp-Refactor](https://github.com/Lewix/fsharp-refactor) - F#用リファクタリングツール
 * [FSharpbinding](https://github.com/fsharp/fsharpbinding) - Xamarin Studio バインディング
 * [F# Snippets web site](http://fssnip.net/) - F# 版のpastebin
 * [F# ACE Code Editor](https://github.com/BayardRock/FSharpWebIntellisense/) - Web上のF#編集ツール
