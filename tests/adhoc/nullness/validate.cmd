dotnet build src\fsc\fscProject -f net472 -c Proto -p ProtoDebug=true

fsc.exe tests\adhoc\nullness\micro.fs -i
fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i

artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\micro.fs -i
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i

artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\micro.fs -i --langversion:preview
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i --langversion:preview

artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\micro.fs -i --langversion:preview --checknulls
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\micro.fsi tests\adhoc\nullness\micro.fs -i --langversion:preview --checknulls

REM -------------


fsc.exe tests\adhoc\nullness\existing\positive.fs 2> tests\adhoc\nullness\existing\positive.previous.bsl & type tests\adhoc\nullness\existing\positive.previous.bsl 
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\existing\positive.fs 2>  tests\adhoc\nullness\existing\positive.next.bsl & type tests\adhoc\nullness\existing\positive.next.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\existing\positive.fs --langversion:preview 2>  tests\adhoc\nullness\existing\positive.next.enabled.bsl & type tests\adhoc\nullness\existing\positive.next.enabled.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\existing\positive.fs --langversion:preview --checknulls 2>  tests\adhoc\nullness\existing\positive.next.enabled.checknulls.bsl & type tests\adhoc\nullness\existing\positive.next.enabled.checknulls.bsl

fsc.exe tests\adhoc\nullness\existing\negative.fs 2> tests\adhoc\nullness\existing\negative.previous.bsl & type tests\adhoc\nullness\existing\negative.previous.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\existing\negative.fs 2>  tests\adhoc\nullness\existing\negative.next.bsl & type tests\adhoc\nullness\existing\negative.next.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\existing\negative.fs --langversion:preview 2>  tests\adhoc\nullness\existing\negative.next.enabled.bsl & type tests\adhoc\nullness\existing\negative.next.enabled.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\existing\negative.fs --langversion:preview --checknulls 2>  tests\adhoc\nullness\existing\negative.next.enabled.checknulls.bsl & type tests\adhoc\nullness\existing\negative.next.enabled.checknulls.bsl

artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\enabled\positive.fs 2>  tests\adhoc\nullness\enabled\positive.next.bsl & type tests\adhoc\nullness\enabled\positive.next.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\enabled\positive.fs --langversion:preview 2>  tests\adhoc\nullness\enabled\positive.next.enabled.bsl & type tests\adhoc\nullness\enabled\positive.next.enabled.bsl
artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\enabled\positive.fs --langversion:preview --checknulls 2>  tests\adhoc\nullness\enabled\positive.next.enabled.checknulls.bsl & type tests\adhoc\nullness\enabled\positive.next.enabled.checknulls.bsl


REM -------------

artifacts\bin\fsc\Proto\net472\fsc.exe tests\adhoc\nullness\test.fsx -i --langversion:preview
