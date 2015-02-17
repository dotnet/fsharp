namespace UnitTests.Tests.ProjectSystem

open System
open NUnit.Framework
open UnitTests.TestLib.Utils

[<TestFixture>]
[<Category("wip")>]
module PathUtilsTests =
    open Microsoft.VisualStudio.FSharp.ProjectSystem.PathUtils

    [<Test>]
    let displayPathFrom () =
        let root = Uri(@"c:\project\dir\", UriKind.Absolute)

        //subdir
        displayPathFrom root (Uri("a.txt", UriKind.Relative))
        |> Asserts.AssertEqual (Uri("a.txt", UriKind.Relative))

        displayPathFrom root (Uri(@"a\b\file1.fs", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"a/b/file1.fs", UriKind.Relative))

        displayPathFrom root (Uri(@"a\b\..\.\file2.fs", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"a/file2.fs", UriKind.Relative))

        displayPathFrom root (Uri(@"a\b\..\.\c", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"a/c", UriKind.Relative))

        //out of directory and back
        displayPathFrom root (Uri(@"..\dir\a\b\file3.fs", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"a/b/file3.fs", UriKind.Relative))

        //absolute
        displayPathFrom root (Uri(@"c:\project\dir\src\file4.fs", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"src/file4.fs", UriKind.Relative))

        //out of dir show the filename
        displayPathFrom root (Uri(@"..\b\file5.fs", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"file5.fs", UriKind.Relative))

        displayPathFrom root (Uri(@"c:\another\dir\file6.fs", UriKind.Relative))
        |> Asserts.AssertEqual (Uri(@"file6.fs", UriKind.Relative))

    [<Test>]
    let ensureTrailingDirectorySeparator () =
        
        //absolute
        ensureTrailingDirectorySeparator @"c:\some path\to\dir"
        |> Asserts.AssertEqual @"c:\some path\to\dir\"
        
        ensureTrailingDirectorySeparator @"c:\some path\..\to\.\dir\."
        |> Asserts.AssertEqual @"c:\some path\..\to\.\dir\.\"
        
        //relative
        ensureTrailingDirectorySeparator @"src\Core\Module1"
        |> Asserts.AssertEqual @"src\Core\Module1\"

        ensureTrailingDirectorySeparator @"src\.\Core\..\Module1"
        |> Asserts.AssertEqual @"src\.\Core\..\Module1\"
        
        //mixed dir separator
        ensureTrailingDirectorySeparator @"src/Core\\Module1"
        |> Asserts.AssertEqual @"src/Core\\Module1\"
