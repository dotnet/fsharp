F# コンパイラのREADME
=============================================================================================

> **注意:** このreadmeファイルはF# コンパイラソースコード
([github.com/fsharp/fsharp](https://github.com/fsharp/fsharp))
に付属していたオリジナルのファイルのコピーです。
F# Compiler Servicesプロジェクトは元々F# Compilerプロジェクトから派生したもので、
いくつかのサービスを公開するために若干の変更が加えられています。
ここに含まれるreadmeファイルは参考程度にとどめてください。

このプロジェクトには(オープンソース版の)F# コンパイラ、コアライブラリ、コアツールが含まれます。
いずれもMITライセンスが適用されます。
`master` ブランチは最新バージョンのF#(現時点ではF# 3.0)に対応します。
なおコンパイラをブートストラップするために、
このプロジェクトの以前のバージョンでビルドされたバイナリが使用されます。

## 必須要件

Mono 2.9以上のバージョンが必要です。Mono 3.0が推奨されます。

OS Xの場合、automake 2.69が必要です。
[homebrew](http://brew.sh/) 経由でインストールする場合は
以下のようにします:

    [lang=text]
    brew install automake

## ビルド

### Linuxおよびその他のUnixシステムの場合

通常の手順は以下の通りです:

    [lang=text]
    ./autogen.sh
    make
    sudo make install

デフォルトでは最適化されたバイナリが生成されます。
デバッグ版をビルドする場合は `make CONFIG=debug` とします。

### MacOS (OSX)の場合

Monoのバージョンを指定するprefixを設定します:

    [lang=text]
    ./autogen.sh --prefix=/Library/Frameworks/Mono.framework/Versions/Current/
    make
    sudo make install

デフォルトでは最適化されたバイナリが生成されます。
デバッグ版をビルドする場合は `make CONFIG=debug` とします。

### Windows上でmsbuildを使用する(つまり.NETがインストールされている)場合

VS2010がインストールされておらず、VS2012しかインストールされていない場合には
[F# 2.0 ランタイム](http://www.microsoft.com/en-us/download/details.aspx?id=13450)
のインストールが必要です:

    [lang=text]
    cd src
    msbuild fsharp-proto-build.proj
    ngen install ..\lib\proto\4.0\fsc-proto.exe (optional)
    msbuild fsharp-library-build.proj /p:Configuration=Release
    msbuild fsharp-compiler-build.proj /p:Configuration=Release

また、.NET 2.0やMono 2.1、MonoTouch、Silverlight 5.0、
Windows Phone 7.1、ポータブルプロファイル47(.NET4.5+Silverlight5+Windows8)、
ポータブルプロファイル88(.NET4+Silverlight4+WindowsPhone7.1+Windows8)、
Xbox 360用XNA 4.0のプロファイルに対応するFSharp.Coreをビルドすることもできます:

    [lang=text]
    msbuild fsharp-library-build.proj /p:TargetFramework=net20 /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=mono21 /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=monotouch /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=portable-net45+sl5+win8 /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=portable-net4+sl4+wp71+win8 /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=sl5 /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=wp7 /p:Configuration=Release
    msbuild fsharp-library-build.proj /p:TargetFramework=net40-xna40-xbox360 /p:Configuration=Release

Silverlight 5.0用にFSharp.CoreとFSharp.Compiler.Silverlight.dll
をビルドすることもできます:

    [lang=text]
    msbuild fsharp-library-build.proj /p:TargetFramework=sl5-compiler  /p:Configuration=Release
    msbuild fsharp-compiler-build.proj /p:TargetFramework=sl5-compiler /p:Configuration=Release

デバッグ版バイナリを出力する場合は ` /p:Configuration=Debug` に変更します。

### Windows上でxbuildを使用する(つまり.NETがインストールされておらず、Mono 3.0だけがインストールされている)場合

    [lang=text]
    cd src
    xbuild fsharp-proto-build.proj
    xbuild fsharp-library-build.proj
    xbuild fsharp-compiler-build.proj

xbuildを使用したビルドはMono準拠の公開用バイナリ生成にはまだ対応していないため
(src/fsharp/targets.make を参照)、個人的な使用にとどめ、
公開用のビルドには使用してはいけません。

## 厳密名

生成されたFSharp.Core.dllには遅延署名だけが行われます
(Monoでは厳密名が必須ではありません)。
厳密名で署名されたFSharp.Core.dllが必要であれば以下を使用してください:

    [lang=text]
    lib\bootstrap\signed\3.0\v4.0\FSharp.Core.dll
  
## 生成されるファイル

ビルドが完了すると、メインのコンパイラバイナリは以下の場所に生成されます:

    [lang=text]
    lib/release/4.0

.NET 2.0やMonoAndroid、MonoTouch(Monoプロファイル2.1)は以下の場所に生成されます:

    [lang=text]
    lib/release/2.0
    lib/release/2.1
    lib/release/2.1monotouch

`make install`　を実行した場合のプレフィックスは以下のようになります:

    [lang=text]
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/2.0/FSharp.Core.dll
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/2.1/FSharp.Core.dll
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.0/fsc.exe
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.0/FSharp.Compiler.dll
    ...
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/fsc.exe
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/FSharp.Compiler.dll
    ...
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/gac/.../FSharp.Compiler.dll
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/gac/.../FSharp.Compiler.dll
    ...

またxbuildをサポートする場合は以下のプレフィックスになります:

    [lang=text]
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/Microsoft\ F#/v4.0/*
    /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/Microsoft\ SDKs/F#/3.0/Framework/*

(これらの名前はVisual Studio由来のプロジェクトファイルで使用されている
Microsoft.FSharp.Targetsファイル内における正式名です)

また、以下のスクリプトが配置されます:

    [lang=text]
    /usr/bin/fsharpc   (F# コンパイラ)
    /usr/bin/fsharpi   (F# Interactive)

## 開発者用メモ

### 継続的インテグレーションビルド

このプロジェクトはJetBrains/Teamcityサーバー上において、
F#コミュニティプロジェクトの一部として継続的インテグレーション
(CI:continuous integration)ビルドが行われています:

[http://teamcity.codebetter.com/project.html?projectId=project61&tab=projectOverview](http://teamcity.codebetter.com/project.html?projectId=project61&tab=projectOverview)

主な管理者は @forki です。
今のところMono用に'make' installと、
Windows用に 'cd src; msbuild fsharp-build.proj' のビルドが行われています。
ビルドされたバイナリは保存されておらず、
単にサニティチェックだけが対象になっています。

### Visual StudioまたはMonoDevelop上でコンパイラを編集する

`all-vs2012.sln` を開いてモードをDebugまたはReleaseに設定します。
コンパイラはコンパイル中でも気を利かせてワークフローに若干介入することがあるため、
実際にコンパイルを実行するには上記にあるようなコマンドライン経由で
コンパイルするとよいでしょう。

MonoDevelopでサポートされているF#ではプロセス内バックグラウンドコンパイラが
使用されます。
Mac上ではこれが原因でガベージコレクションを止めることがあり、
MonoDevelop上でのコンパイラの編集がしづらくなる場合があります。

### .NET 4.x用にF# Coreの単体テストをビルドする(省略可)

このプロジェクトでは、 `FSharp.Core.dll` や `FSharp.Compiler.dll` の一部を
チェックする単体テストをビルドするためにprotoコンパイラを使用しています。
また、 `tests\fsharp` 以下にもいくつかのテストがあります。

    [lang=text]
    msbuild fsharp-library-unittests-build.proj /p:TargetFramework=net40

*注意: 単体テストをビルドする場合、NUnitをインストールしておく必要があります。*

### 検証および使用方法

ビルドされたバイナリを簡単に検証するには、以下のようにして `fsi.exe`
(F# Interactive) を起動してみるとよいでしょう:

    [lang=text]
    lib\debug\4.0\fsi.exe
    1 + 1;;
    \#q;;
    lib\debug\4.0\fsi.exe /help
    lib\debug\4.0\fsc.exe /help
    echo printfn "hello world" > hello.fs
    lib\debug\4.0\fsc.exe hello.fs
    hello.exe


### (Windows上で)コンパイラのテストを実行する

`tests\fsharp\core` 以下には言語機能のテストがあります。
テスト機構は素朴なもので、残念なことにバッチファイルを使用しています。
これらのテストをWindows上で実行するには以下のようにします:

    [lang=text]
    cd ..\tests\fsharp\core
    ..\..\build-and-run-all-installed-ilx-configs.bat results.log

それぞれのテストディレクトリには1つのテスト結果ファイルが生成され、
発生したエラーも記録されます。

    [lang=text]
    tests\fsharp\core
    tests\fsharp\core\queriesCustomQueryOps
    tests\fsharp\core\queriesLeafExpressionConvert
    tests\fsharp\core\queriesNullableOperators
    tests\fsharp\core\queriesOverIEnumerable
    ...

LINQクエリに対するいくつかのテストではSQL Serverのインストールが必要です。
テストが失敗すると、たとえば以下のように出力されます:

    [lang=text]
    ERRORLEVEL=1: in tests\fsharp\core\csfromfs\build.bat

この場合、関連するディレクトリに移動した後、
`build.bat` および `run.bat` を実行します。

## 歴史

Microsoftから公開されたF#コンパイラのソースは
[fsharppowerpack.codeplex.com](http://fsharppowerpack.codeplex.com) にあります。

ブートストラップ用ライブラリ、ツール、F#コンパイラが利用できます。
`lib/bootstrap/X.0` ディレクトリにはMonoビルド用ライブラリやコンパイラ、
ビルドをブートストラップするために使用するツールなどが含まれています。
ブートストラップを独自に指定する場合は `--with-bootstrap` オプションを使用します。
