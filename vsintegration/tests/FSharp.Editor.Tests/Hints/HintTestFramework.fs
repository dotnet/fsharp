// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open System
open System.Threading
open Microsoft.IO
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Hints
open Microsoft.CodeAnalysis.Text
open Hints
open FSharp.Compiler.CodeAnalysis
open FSharp.Editor.Tests.Helpers

module HintTestFramework =

    // another representation for extra convenience
    type TestHint =
        { Content: string; Location: int * int }

    // like: C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\7.0.0\ref\net7.0\mscorlib.dll
    let locateMscorlib() =
        let programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        let dotnetPacks = $"{programFiles}\\dotnet\\packs"
        let mscorlibs = Directory.GetFiles(dotnetPacks, "mscorlib.dll", SearchOption.AllDirectories) 
        mscorlibs |> Seq.last

    let private convert hint =
        let content =
            hint.Parts |> Seq.map (fun hintPart -> hintPart.Text) |> String.concat ""

        // that's about different coordinate systems
        // in tests, the most convenient is the one used in editor,
        // hence this conversion
        let location = (hint.Range.StartLine - 1, hint.Range.EndColumn + 1)

        {
            Content = content
            Location = location
        }

    let getFsDocument code =
        use project = SingleFileProject code
        let fileName = fst project.Files.Head
        // I don't know, without this lib some symbols are just not loaded
        let options = { project.Options with OtherOptions = 
                                                [|
                                                    "-o:obj\\Debug\\net7.0\\FSharp.dll"
                                                    "-g"
                                                    "--debug:portable"
                                                    "--noframework"
                                                    "--define:TRACE"
                                                    "--define:DEBUG"
                                                    "--define:NET"
                                                    "--define:NET7_0"
                                                    "--define:NETCOREAPP"
                                                    "--define:NET5_0_OR_GREATER"
                                                    "--define:NET6_0_OR_GREATER"
                                                    "--define:NET7_0_OR_GREATER"
                                                    "--define:NETCOREAPP1_0_OR_GREATER"
                                                    "--define:NETCOREAPP1_1_OR_GREATER"
                                                    "--define:NETCOREAPP2_0_OR_GREATER"
                                                    "--define:NETCOREAPP2_1_OR_GREATER"
                                                    "--define:NETCOREAPP2_2_OR_GREATER"
                                                    "--define:NETCOREAPP3_0_OR_GREATER"
                                                    "--define:NETCOREAPP3_1_OR_GREATER"
                                                    "--optimize-"
                                                    "--tailcalls-"
                                                    "--target:exe"
                                                    "--nowarn:IL2121"
                                                    "--warn:3"
                                                    "--warnaserror:3239"
                                                    "--fullpaths"
                                                    "--flaterrors"
                                                    "--highentropyva+"
                                                    "--targetprofile:netcore"
                                                    "--nocopyfsharpcore"
                                                    "--deterministic+"
                                                    "--simpleresolution"
                                                    "-r:C:\\Users\\psemkin\\.nuget\\packages\\fsharp.core\\7.0.0\\lib\\netstandard2.1\\FSharp.Core.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\Microsoft.CSharp.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\Microsoft.VisualBasic.Core.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\Microsoft.VisualBasic.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\Microsoft.Win32.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\Microsoft.Win32.Registry.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\netstandard.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.AppContext.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Buffers.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Collections.Concurrent.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Collections.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Collections.Immutable.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Collections.NonGeneric.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Collections.Specialized.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ComponentModel.Annotations.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ComponentModel.DataAnnotations.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ComponentModel.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ComponentModel.EventBasedAsync.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ComponentModel.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ComponentModel.TypeConverter.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Configuration.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Console.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Core.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Data.Common.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Data.DataSetExtensions.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Data.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.Contracts.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.Debug.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.DiagnosticSource.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.FileVersionInfo.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.Process.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.StackTrace.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.TextWriterTraceListener.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.Tools.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.TraceSource.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Diagnostics.Tracing.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Drawing.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Drawing.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Dynamic.Runtime.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Formats.Asn1.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Formats.Tar.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Globalization.Calendars.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Globalization.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Globalization.Extensions.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.Compression.Brotli.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.Compression.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.Compression.FileSystem.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.Compression.ZipFile.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.FileSystem.AccessControl.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.FileSystem.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.FileSystem.DriveInfo.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.FileSystem.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.FileSystem.Watcher.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.IsolatedStorage.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.MemoryMappedFiles.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.Pipes.AccessControl.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.Pipes.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.IO.UnmanagedMemoryStream.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Linq.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Linq.Expressions.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Linq.Parallel.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Linq.Queryable.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Memory.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Http.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Http.Json.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.HttpListener.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Mail.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.NameResolution.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.NetworkInformation.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Ping.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Quic.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Requests.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Security.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.ServicePoint.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.Sockets.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.WebClient.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.WebHeaderCollection.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.WebProxy.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.WebSockets.Client.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Net.WebSockets.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Numerics.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Numerics.Vectors.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.ObjectModel.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.DispatchProxy.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.Emit.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.Emit.ILGeneration.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.Emit.Lightweight.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.Extensions.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.Metadata.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Reflection.TypeExtensions.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Resources.Reader.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Resources.ResourceManager.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Resources.Writer.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.CompilerServices.Unsafe.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.CompilerServices.VisualC.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Extensions.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Handles.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.InteropServices.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.InteropServices.JavaScript.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.InteropServices.RuntimeInformation.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Intrinsics.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Loader.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Numerics.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Serialization.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Serialization.Formatters.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Serialization.Json.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Serialization.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Runtime.Serialization.Xml.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.AccessControl.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Claims.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.Algorithms.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.Cng.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.Csp.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.Encoding.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.OpenSsl.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.Primitives.dll"
                                                    "-r:C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.0\\ref\\net7.0\\System.Security.Cryptography.X509Certificates.dll"
                                                    "--ignorelinedirectives"
                                                |] }
        let document, _ = RoslynTestHelpers.CreateSingleDocumentSolution(fileName, code, options)
        document

    let getFsiAndFsDocuments (fsiCode: string) (fsCode: string) =
        RoslynTestHelpers.CreateTwoDocumentSolution("test.fsi", SourceText.From fsiCode, "test.fs", SourceText.From fsCode)

    let getHints document hintKinds =
        async {
            let! hints = HintService.getHintsForDocument document hintKinds "test" CancellationToken.None
            return hints |> Seq.map convert
        }
        |> Async.RunSynchronously

    let getTypeHints document =
        getHints document (Set.empty.Add(HintKind.TypeHint))

    let getParameterNameHints document =
        getHints document (Set.empty.Add(HintKind.ParameterNameHint))

    let getAllHints document =
        let hintKinds = Set.empty.Add(HintKind.TypeHint).Add(HintKind.ParameterNameHint)

        getHints document hintKinds
