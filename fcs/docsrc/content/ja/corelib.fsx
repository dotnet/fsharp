(*** hide ***)
#I "../../../../artifacts/bin/fcs/net461"
(**
コンパイラサービス: FSharp.Core.dll についてのメモ
==================================================

あなたのアプリケーションとともに FSharp.Core を配布する
-------------------------------------------------------

FSharp.Compiler.Service.dll を利用するアプリケーションまたはプラグイン・コンポーネントをビルドする際、普通はアプリの一部として FSharp.Core.dll のコピーも含めることになるでしょう。

例えば、 ``HostedCompiler.exe`` をビルドする場合、普通はあなたの ``HostedCompiler.exe`` と同じフォルダに FSharp.Core.dll (例えば 4.3.1.0)を配置します。

動的コンパイルや動的実行を行う場合、FSharp.Core.optdata と FSharp.Core.sigdata も含める必要があるかもしれませんが、これらについては下記の指針をご覧ください。

あなたのアプリケーションにリダイレクトをバインドする
----------------------------------------------------

FSharp.Compiler.Service.dll コンポーネントは FSharp.Core 4.3.0.0 に依存しています。通例、あなたのアプリケーションはこれより後のバージョンの FSharp.Core をターゲットにしており、FSharp.Core 4.3.0.0 をあなたのアプリケーションで用いる FSharp.Core.dll の最終バージョンにちゃんと転送させるように[バインド リダイレクト](https://msdn.microsoft.com/ja-jp/library/7wd6ex19(v=vs.110).aspx)が必要になるでしょう。バインド リダイレクト ファイルは通常ビルドツールによって自動的に生成されます。そうでない場合、下記のようなファイル(あなたのツールが ``HostedCompiler.exe`` という名前で、バインド リダイレクト ファイルが ``HostedCompiler.exe.config`` という名前の場合)を使うことが出来ます。

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
        <runtime>
          <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
            <dependentAssembly>
              <assemblyIdentity name="FSharp.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
              <bindingRedirect oldVersion="2.0.0.0-4.3.0.0" newVersion="4.3.1.0"/>
            </dependentAssembly>
            <dependentAssembly>
              <assemblyIdentity name="System.Collections.Immutable" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
              <bindingRedirect oldVersion="1.0.0.0-1.2.0.0" newVersion="1.2.1.0" />
            </dependentAssembly>
          </assemblyBinding>
        </runtime>
    </configuration>

どの FSharp.Core と .NET フレームワークがコンパイル時に参照される？
--------------------------------------

FSharp.Combiler.Service コンポーネントは多かれ少なかれ、F#コードを コンパイルするために使われるに過ぎません。特に、コマンドライン引数(あなたのツールを実行するために使われる FSharp.Core や .NET フレームワークとは違います)に明示的に FSharp.Core および/またはフレームワークのアセンブリを参照することが出来ます。

特定の FSharp.Core および .NET フレームワーク アセンブリ、またはそのいずれかをターゲットにする場合、 ``--noframework`` 引数と適切なコマンドライン引数を使います:

    [<Literal>]
    let fsharpCorePath =
        @"C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.1.0\FSharp.Core.dll"
    let errors2, exitCode2 =
      scs.Compile(
        [| "fsc.exe"; "--noframework";
           "-r"; fsharpCorePath;
           "-r"; @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll";
           "-o"; fn3;
           "-a"; fn2 |])

これらのアセンブリが配置されている場所を指定する必要があります。クロスプラットフォームに対応した方法でDLL を配置して、それらをコマンドライン引数に変換する最も簡単な方法は、[F# プロジェクトファイルをクラックする](https://fsharp.github.io/FSharp.Compiler.Service/ja/project.html)ことです。
自分で SDK のパスを処理する代わりに、[FSharp.Compiler.Service.dll 用のテスト](https://github.com/fsharp/FSharp.Compiler.Service/blob/8a943dd3b545648690cb3bed652a469bdb6dd869/tests/service/Common.fs#L54)で使用しているようなヘルパー関数も用意されています。


スクリプトを処理しているか ``GetCheckOptionsFromScriptRoot`` を使っている場合
-------------------------------------------------------------------------

もし SDK 配置先にある FSharp.Core.dll を明示的に参照 *していない* 場合、または ``FsiEvaluationSession`` や ``GetCheckOptionsFromScriptRoot`` を使用してスクリプトを処理している場合、以下のいずれかの方法により、暗黙的にFSharp.Core が参照されます:

1. ``System.Reflection.Assembly.GetEntryAssembly()`` によって返されるホストアセンブリから静的に参照されたFSharp.Core.dll のバージョン

2. ホストアセンブリに FSharp.Core への静的な参照がない場合、

   - FSharp.Compiler.Service 0.x シリーズでは、FSharp.Core バージョン 4.3.0.0 への参照が付与されます

   - FSharp.Compiler.Service 1.3.1.x (F# 3.1 シリーズ)では、FSharp.Core バージョン 4.3.1.0 への参照が付与されます

   - FSharp.Compiler.Service 1.4.0.x (F# 4.0 シリーズ)では、FSharp.Core バージョン 4.4.0.0 への参照が付与されます

FSharp.Core.optdata と FSharp.Core.sigdata を含める必要はありますか？
--------------------------------------

もしあなたのコンパイル引数が SDK 配置先にある FSharp.Core.dll を明示的に参照している場合、FSharp.Core.sigdata と FSharp.Core.optdata はその DLL と同じフォルダになければいけません(これらのファイルがインストールされていない場合、F# SDKの インストールに問題があります)。もしコンパイル引数で常に明示的に参照していたなら、FSharp.Core.optdata と FSharp.Core.sigdata はあなたのアプリケーションの一部として含める必要は *ありません* 。

もしあなたが暗黙的な参照(例えば、上記のスクリプト処理など)に頼っているのなら、これはあなたのツールがアプリケーションの一部として FSharp.Core.dll を参照しているかもしれない、ということです。この場合、FSharp.Core.optdata および FSharp.Core.sigdata が FSharp.Core.dll と同じフォルダに見つからないというエラーが発生するかもしれません。 **もしあなたがアプリケーションに含めている FSharp.Core.dll を暗黙的に参照したいのであれば、FSharp.Core.sigdata と FSharp.Core.optdata もアプリケーションに追加する2つのファイルとして追加しましょう。** ``CombileToDynamicAssembly`` を使用する場合、この問題によって[アセンブリ解決中のスタックオーバーフロー](https://github.com/fsharp/FSharp.Compiler.Service/issues/258)も引き起こされるでしょう。

動的コンパイルと動的コード実行を行うツール(例: ``HostedExecution.exe``)はしばしば FSharp.Core.dll を暗黙的に参照するようになっています。
これはつまり通常 FSharp.Core.optdata と FSharp.Core.sigdata を含んでいるということです。

要約
-------

このデザインノートでは3つのポイントを検討しました:

- どの FSharp.Core.dll があなたのコンパイルツールを実行するのに使われるか
- あなたのコンパイルツールを実行するのに使われる FSharp.Core.dll へのバインド リダイレクトを設定する方法
- あなたのツールによって実行されるチェック時およびコンパイル時にどの FSharp.Core.dll および/またはフレームワークのアセンブリが参照されるか

*)
