(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラの組み込み
====================

このチュートリアルではF#コンパイラをホストする方法を紹介します。

> **注意:** 以下で使用しているAPIは実験的なもので、
  新しいnugetパッケージの公開に伴って変更される可能性があります。

> **注意:** F#コンパイラをホストする方法はいくつかあります。
  一番簡単な方法は `fsc.exe` のプロセスを使って引数を渡す方法です。

---------------------------

まず、F# Interactiveサービスを含むライブラリへの参照を追加します:
*)

#r "FSharp.Compiler.Service.dll"
open Microsoft.FSharp.Compiler.SimpleSourceCodeServices
open System.IO

let scs = SimpleSourceCodeServices()

(**
次に、一時ファイルへコンテンツを書き込みます:

*)
let fn = Path.GetTempFileName()
let fn2 = Path.ChangeExtension(fn, ".fs")
let fn3 = Path.ChangeExtension(fn, ".dll")

File.WriteAllText(fn2, """
module M

type C() = 
   member x.P = 1

let x = 3 + 4
""")

(**
そしてコンパイラを呼び出します:
*)

let errors1, exitCode1 = scs.Compile([| "fsc.exe"; "-o"; fn3; "-a"; fn2 |])

(** 

エラーが発生した場合は「終了コード」とエラーの配列から原因を特定できます:

*)
File.WriteAllText(fn2, """
module M

let x = 1.0 + "" // a type error
""")

let errors1b, exitCode1b = scs.Compile([| "fsc.exe"; "-o"; fn3; "-a"; fn2 |])

if exitCode1b <> 0 then
    errors1b
    |> Array.iter (printfn "%A")

(**

動的アセンブリへのコンパイル
============================

コードを動的アセンブリとしてコンパイルすることもできます。
動的アセンブリはF# Interactiveコードジェネレータでも使用されています。

この機能はたとえばファイルシステムが必ずしも利用できないような状況で役に立ちます。

出力ファイルの名前を指定する "-o" オプションを指定することは可能ですが、
実際には出力ファイルがディスク上に書き込まれることはありません。

'execute' 引数に 'None' を指定するとアセンブリ用の初期化コードが実行されません。
*)
let errors2, exitCode2, dynAssembly2 = 
    scs.CompileToDynamicAssembly([| "-o"; fn3; "-a"; fn2 |], execute=None)

(**
'Some' を指定するとアセンブリ用の初期化コードが実行されます。
*)
let errors3, exitCode3, dynAssembly3 = 
    scs.CompileToDynamicAssembly([| "-o"; fn3; "-a"; fn2 |], Some(stdout,stderr))

