(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラサービス: シンボルの処理
==================================

このチュートリアルでは、F#コンパイラによって提供される
シンボルの扱い方についてのデモを紹介します。
シンボルの参照に関する情報については [プロジェクト全体の分析](project.html)
も参考にしてください。

> **注意:** 以下で使用しているAPIは試験的なもので、
  最新のnugetパッケージの公開に伴って変更されることがあります。

これまでと同じく、 `FSharp.Compiler.Service.dll` への参照を追加した後、
適切な名前空間をオープンし、 `FSharpChecker` のインスタンスを作成します:

*)
// F#コンパイラAPIへの参照
#r "FSharp.Compiler.Service.dll"

open System
open System.IO
open Microsoft.FSharp.Compiler.SourceCodeServices

// インタラクティブチェッカーのインスタンスを作成
let checker = FSharpChecker.Create()

(**

そして特定の入力値に対して型チェックを行います:

*)

let parseAndTypeCheckSingleFile (file, input) = 
    // スタンドアロンの(スクリプト)ファイルを表すコンテキストを取得
    let projOptions = 
        checker.GetProjectOptionsFromScript(file, input)
        |> Async.RunSynchronously

    let parseFileResults, checkFileResults = 
        checker.ParseAndCheckFileInProject(file, 0, input, projOptions) 
        |> Async.RunSynchronously

    // 型チェックが成功(あるいは100%に到達)するまで待機
    match checkFileResults with
    | FSharpCheckFileAnswer.Succeeded(res) -> parseFileResults, res
    | res -> failwithf "Parsing did not finish... (%A)" res

let file = "/home/user/Test.fsx"

(**
## ファイルに対する解決済みのシグネチャ情報を取得する

ファイルに対する型チェックが完了すると、
`TypeCheckResults` の `PartialAssemblySignature` プロパティを参照することにより、
チェック中の特定のファイルを含む、推論されたプロジェクトのシグネチャに
アクセスすることができます。

モジュールや型、属性、メンバ、値、関数、共用体、レコード型、測定単位、
およびその他のF#言語要素に対する完全なシグネチャ情報が参照できます。

ただし型付き式ツリーに対する情報は(今のところ)この方法では利用できません。

*)

let input2 = 
      """
[<System.CLSCompliant(true)>]
let foo(x, y) = 
    let msg = String.Concat("Hello"," ","world")
    if true then 
        printfn "x = %d, y = %d" x y 
        printfn "%s" msg

type C() = 
    member x.P = 1
      """
let parseFileResults, checkFileResults = 
    parseAndTypeCheckSingleFile(file, input2)

(**
これでコードに対する部分的なアセンブリのシグネチャが取得できるようになります:
*)
let partialAssemblySignature = checkFileResults.PartialAssemblySignature

partialAssemblySignature.Entities.Count = 1  // エンティティは1つ

(**
そしてコードを含むモジュールに関連したエンティティを取得します:
*)
let moduleEntity = partialAssemblySignature.Entities.[0]

moduleEntity.DisplayName = "Test"

(**
そしてコード内の型定義に関連したエンティティを取得します:
*)
let classEntity = moduleEntity.NestedEntities.[0]

(**
そしてコード内で定義された関数に関連した値を取得します:
*)
let fnVal = moduleEntity.MembersFunctionsAndValues.[0]

(**
関数値に関するプロパティの値を確認してみましょう。
*)
fnVal.Attributes.Count // 1
fnVal.CurriedParameterGroups.Count // 1
fnVal.CurriedParameterGroups.[0].Count // 2
fnVal.CurriedParameterGroups.[0].[0].Name // "x"
fnVal.CurriedParameterGroups.[0].[1].Name // "y"
fnVal.DeclarationLocation.StartLine // 3
fnVal.DisplayName // "foo"
fnVal.DeclaringEntity.DisplayName // "Test"
fnVal.DeclaringEntity.DeclarationLocation.StartLine // 1
fnVal.GenericParameters.Count // 0
fnVal.InlineAnnotation // FSharpInlineAnnotation.OptionalInline
fnVal.IsActivePattern // false
fnVal.IsCompilerGenerated // false
fnVal.IsDispatchSlot // false
fnVal.IsExtensionMember // false
fnVal.IsPropertyGetterMethod // false
fnVal.IsImplicitConstructor // false
fnVal.IsInstanceMember // false
fnVal.IsMember // false
fnVal.IsModuleValueOrMember // true
fnVal.IsMutable // false
fnVal.IsPropertySetterMethod // false
fnVal.IsTypeFunction // false

(**
次に、この関数の型がファーストクラスの値として使用されているかどうかチェックします。
(ちなみに `CurriedParameterGroups` プロパティには引数の名前など、
より多くの情報も含まれています)
*)
fnVal.FullType // int * int -> unit
fnVal.FullType.IsFunctionType // true
fnVal.FullType.GenericArguments.[0] // int * int 
fnVal.FullType.GenericArguments.[0].IsTupleType // true
let argTy1 = fnVal.FullType.GenericArguments.[0].GenericArguments.[0]

argTy1.TypeDefinition.DisplayName // int

(**
というわけで `int * int -> unit` という型を表現するオブジェクトが取得できて、
その1つめの 'int' を確認できたわけです。
また、以下のようにすると 'int' 型についてのより詳細な情報が取得でき、
それが名前付きの型であり、F#の型省略形 `type int = int32` であることがわかります:
*)

argTy1.HasTypeDefinition // true
argTy1.TypeDefinition.IsFSharpAbbreviation // true

(**
型省略形の右辺、つまり `int32` についてもチェックしてみましょう:
*)

let argTy1b = argTy1.TypeDefinition.AbbreviatedType
argTy1b.TypeDefinition.Namespace // Some "Microsoft.FSharp.Core" 
argTy1b.TypeDefinition.CompiledName // "int32" 

(**
そして再び型省略形 `type int32 = System.Int32` から型に関する完全な情報が取得できます:
*)
let argTy1c = argTy1b.TypeDefinition.AbbreviatedType
argTy1c.TypeDefinition.Namespace // Some "System" 
argTy1c.TypeDefinition.CompiledName // "Int32" 

(**
ファイルに対する型チェックの結果には、
コンパイル時に使用されたプロジェクト(あるいはスクリプト)のオプションに関する
`ProjectContext` と呼ばれる情報も含まれています:
*)
let projectContext = checkFileResults.ProjectContext

for ass in projectContext.GetReferencedAssemblies() do
    match ass.FileName with 
    | None -> printfn "コンパイル時にファイルの存在しないアセンブリを参照しました"
    | Some s -> printfn "コンパイル時にアセンブリ '%s' を参照しました" s

(**
**注意:**

  - 不完全なコードが存在する場合、一部あるいはすべての属性が意図したとおりには
    並ばないことがあります。
  - (実際には非常によくあることですが)一部のアセンブリが見つからない場合、
    外部アセンブリに関連する値やメンバ、エンティティにおける 'IsUnresolved' が
    trueになることがあります。
    IsUnresolvedによる例外に対処できるよう、堅牢なコードにしておくべきです。

*)

(**

## プロジェクト全体に対するシンボル情報を取得する

プロジェクト全体をチェックする場合、チェッカーを作成した後に `parseAndCheckScript`
を呼び出します。
今回の場合は単に1つのスクリプトだけが含まれたプロジェクトをチェックします。
異なる "projOptions" を指定すると、巨大なプロジェクトに対する設定を
構成することもできます。
*)
let parseAndCheckScript (file, input) = 
    let projOptions = 
        checker.GetProjectOptionsFromScript(file, input)
        |> Async.RunSynchronously

    let projResults = 
        checker.ParseAndCheckProject(projOptions) 
        |> Async.RunSynchronously

    projResults

(**
そして特定の入力に対してこの関数を呼び出します:
*)

let tmpFile = Path.ChangeExtension(System.IO.Path.GetTempFileName() , "fs")
File.WriteAllText(tmpFile, input2)

let projectResults = parseAndCheckScript(tmpFile, input2)


(**
結果は以下の通りです:
*)

let assemblySig = projectResults.AssemblySignature

assemblySig.Entities.Count = 1  // エンティティは1つ
assemblySig.Entities.[0].Namespace  // null
assemblySig.Entities.[0].DisplayName // "Tmp28D0"
assemblySig.Entities.[0].MembersFunctionsAndValues.Count // 1 
assemblySig.Entities.[0].MembersFunctionsAndValues.[0].DisplayName // "foo" 
