// Regression test for DevDiv:257720 - Improve diagnostics for TPs
// TODO: Remove hardcoded path

open Microsoft.FSharp.Data.TypeProviders

// The type provider 'Microsoft.FSharp.Data.TypeProviders.DesignTime.DataProviders' reported an error: Error: No valid input files specified. Specify either metadata documents or assembly files  Microsoft (R) Service Model Metadata Tool [Microsoft (R) Windows (R) Communication Foundation, Version 4.0.30319.17360] Copyright (c) Microsoft Corporation.  All rights reserved.  If you would like more help, type "svcutil /?"
type T1 = ODataService< ServiceUri = @"\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll" >

//<Expects status="error" span="(7,11)" id="FS3033">The type provider 'Microsoft\.FSharp\.Data\.TypeProviders\.DesignTime\.DataProviders' reported an error: Error reading schema\. Could not find a part of the path '[a-zA-Z]:\\Windows\\Microsoft\.NET\\Framework\\v4\.0\.30319\\mscorlib\.dll\\\$metadata'\.$</Expects>