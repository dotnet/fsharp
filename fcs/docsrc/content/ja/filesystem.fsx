(*** hide ***)
#I "../../../../debug/bin/net45/"
(**
コンパイラサービス: ファイルシステム仮想化
==========================================

`FSharp.Compiler.Service` にはファイルシステムを表すグローバル変数があります。
この変数を設定するこにより、ファイルシステムが利用できない状況でも
コンパイラをホストすることができるようになります。
  
> **注意:** 以下で使用しているAPIは実験的なもので、
  新しいnugetパッケージの公開に伴って変更される可能性があります。

FileSystemの設定
----------------

以下の例ではディスクからの読み取りを行うような実装をファイルシステムに設定しています:
*)
#r "FSharp.Compiler.Service.dll"
open System
open System.IO
open System.Collections.Generic
open System.Text
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

let defaultFileSystem = Shim.FileSystem

let fileName1 = @"c:\mycode\test1.fs" // 注意: 実際には存在しないファイルのパス
let fileName2 = @"c:\mycode\test2.fs" // 注意: 実際には存在しないファイルのパス

type MyFileSystem() = 
    let file1 = """
module File1

let A = 1"""
    let file2 = """
module File2
let B = File1.A + File1.A"""
    let files = dict [(fileName1, file1); (fileName2, file2)]

    interface IFileSystem with
        // 読み取りおよび書き込み用にファイルをオープンする機能を実装
        member __.FileStreamReadShim(fileName) = 
            match files.TryGetValue(fileName) with
            | true, text -> new MemoryStream(Encoding.UTF8.GetBytes(text)) :> Stream
            | _ -> defaultFileSystem.FileStreamReadShim(fileName)

        member __.FileStreamCreateShim(fileName) = 
            defaultFileSystem.FileStreamCreateShim(fileName)

        member __.FileStreamWriteExistingShim(fileName) = 
            defaultFileSystem.FileStreamWriteExistingShim(fileName)

        member __.ReadAllBytesShim(fileName) = 
            match files.TryGetValue(fileName) with
            | true, text -> Encoding.UTF8.GetBytes(text)
            | _ -> defaultFileSystem.ReadAllBytesShim(fileName)

        // 一時パスおよびファイルのタイムスタンプに関連する機能を実装
        member __.GetTempPathShim() = 
            defaultFileSystem.GetTempPathShim()
        member __.GetLastWriteTimeShim(fileName) = 
            defaultFileSystem.GetLastWriteTimeShim(fileName)
        member __.GetFullPathShim(fileName) = 
            defaultFileSystem.GetFullPathShim(fileName)
        member __.IsInvalidPathShim(fileName) = 
            defaultFileSystem.IsInvalidPathShim(fileName)
        member __.IsPathRootedShim(fileName) = 
            defaultFileSystem.IsPathRootedShim(fileName)

        // ファイルの存在確認および削除に関連する機能を実装
        member __.SafeExists(fileName) = 
            files.ContainsKey(fileName) || defaultFileSystem.SafeExists(fileName)
        member __.FileDelete(fileName) = 
            defaultFileSystem.FileDelete(fileName)

        // アセンブリのロードに関連する機能を実装。
        // 型プロバイダやF# Interactiveで使用される。
        member __.AssemblyLoadFrom(fileName) = 
            defaultFileSystem.AssemblyLoadFrom fileName
        member __.AssemblyLoad(assemblyName) = 
            defaultFileSystem.AssemblyLoad assemblyName 

let myFileSystem = MyFileSystem()
Shim.FileSystem <- MyFileSystem() 

(**

FileSystemによるコンパイルの実行
--------------------------------

*)
open Microsoft.FSharp.Compiler.SourceCodeServices

let checker = FSharpChecker.Create()
let projectOptions = 
    let allFlags = 
        [| yield "--simpleresolution"; 
           yield "--noframework"; 
           yield "--debug:full"; 
           yield "--define:DEBUG"; 
           yield "--optimize-"; 
           yield "--doc:test.xml"; 
           yield "--warn:3"; 
           yield "--fullpaths"; 
           yield "--flaterrors"; 
           yield "--target:library"; 
           let references =
             [ @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\mscorlib.dll"; 
               @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll"; 
               @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Core.dll"; 
               @"C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\FSharp.Core.dll"]
           for r in references do 
                 yield "-r:" + r |]
 
    { ProjectFileName = @"c:\mycode\compilation.fsproj" // 現在のディレクトリで一意な名前を指定
      ProjectFileNames = [| fileName1; fileName2 |]
      OtherOptions = allFlags 
      ReferencedProjects=[| |]
      IsIncompleteTypeCheckEnvironment = false
      UseScriptResolutionRules = true 
      LoadTime = System.DateTime.Now // 'Now' を指定して強制的に再読込させている点に注意
      UnresolvedReferences = None }

let results = checker.ParseAndCheckProject(projectOptions) |> Async.RunSynchronously

results.Errors
results.AssemblySignature.Entities.Count //2
results.AssemblySignature.Entities.[0].MembersFunctionsAndValues.Count //1
results.AssemblySignature.Entities.[0].MembersFunctionsAndValues.[0].DisplayName // "B"

(**
まとめ
------
このチュートリアルでは FSharp.Compiler.Service コンポーネントで使用される
ファイルシステムに注目して、グローバルな設定を変更する方法について紹介しました。

このチュートリアルの執筆時点では、以下に列挙したSystem.IOの操作に対しては
仮想化されたファイルシステムAPIが用意されない予定になっています。
将来のバージョンのコンパイラサービスではこれらのAPIが追加されるかもしれません。

  - Path.Combine
  - Path.DirectorySeparatorChar
  - Path.GetDirectoryName
  - Path.GetFileName
  - Path.GetFileNameWithoutExtension
  - Path.HasExtension
  - Path.GetRandomFileName (アセンブリ内にコンパイル済みwin32リソースを生成する場合にのみ使用される)

**注意:** `SourceCodeServices` API内の一部の操作では、
引数にファイルの内容だけでなくファイル名を指定する必要があります。
これらのAPIにおいて、ファイル名はエラーの報告のためだけに使用されます。
  
**注意:** 型プロバイダーコンポーネントは仮想化されたファイルシステムを使用しません。

**注意:** コンパイラサービスは `--simpleresolution` が指定されていない場合、
MSBuildを使ってアセンブリの解決を試みることがあります。
`FileSystem` APIを使用する場合、通常はコンパイラへのフラグとして
`--simpleresolution` を指定することになります。
それと同時に `--noframework` を指定します。
.NETアセンブリに対するすべての参照を明示的に指定する必要があるでしょう。
*)
