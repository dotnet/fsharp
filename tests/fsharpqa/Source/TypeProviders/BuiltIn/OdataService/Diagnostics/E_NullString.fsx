// Regression test for DevDiv:257720 - Improve diagnostics for TPs

#if INTERACTIVE
#r "System.Runtime.Serialization"
#r "System.ServiceModel"
#r "FSharp.Data.TypeProviders"
#endif

open System.Runtime.Serialization
open System.ServiceModel
open Microsoft.FSharp.Data.TypeProviders

type T1 = ODataService< ServiceUri = null > 

//<Expects status="error" span="(13,38)" id="FS3045">Invalid static argument to provided type\. Expected an argument of kind 'string'\.$</Expects>
