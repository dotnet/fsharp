REM devenv FSharp.sln /ProjectConfig Proto
set "fsiLocation="C:\Program Files\dotnet\sdk\8.0.300\FSharp\fsc.dll""
REM dotnet build src\fsc\fscProject -f net472 -c Proto -p ProtoDebug=true

dotnet %fsiLocation% tests\adhoc\nullness\micro.fs -i
dotnet %fsiLocation% tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i

artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\micro.fs -i
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i

artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\micro.fs -i --langversion:preview
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i --langversion:preview

artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\micro.fs -i --langversion:preview --checknulls
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i --langversion:preview --checknulls

REM -------------


dotnet %fsiLocation% tests\adhoc\nullness\existing\positive.fs 2> tests\adhoc\nullness\existing\positive.previous.bsl & type tests\adhoc\nullness\existing\positive.previous.bsl 
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\existing\positive.fs 2>  tests\adhoc\nullness\existing\positive.next.bsl & type tests\adhoc\nullness\existing\positive.next.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\existing\positive.fs --langversion:preview 2>  tests\adhoc\nullness\existing\positive.next.enabled.bsl & type tests\adhoc\nullness\existing\positive.next.enabled.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\existing\positive.fs --langversion:preview --checknulls 2>  tests\adhoc\nullness\existing\positive.next.enabled.checknulls.bsl & type tests\adhoc\nullness\existing\positive.next.enabled.checknulls.bsl

dotnet %fsiLocation% tests\adhoc\nullness\existing\negative.fs 2> tests\adhoc\nullness\existing\negative.previous.bsl & type tests\adhoc\nullness\existing\negative.previous.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\existing\negative.fs 2>  tests\adhoc\nullness\existing\negative.next.bsl & type tests\adhoc\nullness\existing\negative.next.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\existing\negative.fs --langversion:preview 2>  tests\adhoc\nullness\existing\negative.next.enabled.bsl & type tests\adhoc\nullness\existing\negative.next.enabled.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\existing\negative.fs --langversion:preview --checknulls 2>  tests\adhoc\nullness\existing\negative.next.enabled.checknulls.bsl & type tests\adhoc\nullness\existing\negative.next.enabled.checknulls.bsl

artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\enabled\positive.fs 2>  tests\adhoc\nullness\enabled\positive.next.bsl & type tests\adhoc\nullness\enabled\positive.next.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\enabled\positive.fs --langversion:preview 2>  tests\adhoc\nullness\enabled\positive.next.enabled.bsl & type tests\adhoc\nullness\enabled\positive.next.enabled.bsl
artifacts\bin\fsc\Release\net8.0\fsc.exe tests\adhoc\nullness\enabled\positive.fs --langversion:preview --checknulls 2>  tests\adhoc\nullness\enabled\positive.next.enabled.checknulls.bsl & type tests\adhoc\nullness\enabled\positive.next.enabled.checknulls.bsl


REM -------------

