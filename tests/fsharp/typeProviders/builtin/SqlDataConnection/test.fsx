// #Conformance #TypeProviders #SqlDataConnection
#r "FSharp.Data.TypeProviders.dll"
#r "System.Management.dll"

open Microsoft.FSharp.Core.CompilerServices
open System.IO

[<AutoOpen>]
module Infrastructure = 
    let failures = ref false
    let reportFailure () = stderr.WriteLine " NO"; failures := true
    let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else reportFailure() 
    let check s v1 v2 = stderr.Write(s:string);  if v1 = v2 then stderr.WriteLine " OK" else eprintf "... FAILURE: expected %A, got %A  " v2 v1;  reportFailure() 

    let argv = System.Environment.GetCommandLineArgs() 
    let SetCulture() = 
      if argv.Length > 2 && argv.[1] = "--culture" then  begin
        let cultureString = argv.[2] in 
        let culture = new System.Globalization.CultureInfo(cultureString) in 
        stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
        System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
      end 

    do SetCulture()    

module CheckSqlConnectionTypeProvider = 

    let isSQLExpressInstalled =
        let edition = "Express Edition"
        let instance = "MSSQL$SQLEXPRESS"

        try
            let getSqlExpress = 
                new System.Management.ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement10",
                                                               "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and ServiceName = '" + instance + "' and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')")

            // If nothing is returned, SQL Express isn't installed.
            getSqlExpress.Get().Count <> 0
        with
        | _ -> false


    let checkHostedType (hostedType: System.Type) = 
        //let hostedType = hostedAppliedType1
        test "ceklc09wlkm1a" (hostedType.Assembly <> typeof<Microsoft.FSharp.Data.TypeProviders.DesignTime.DataProviders>.Assembly)
        test "ceklc09wlkm1b" (hostedType.Assembly.FullName.StartsWith "tmp")

        check "ceklc09wlkm2" hostedType.DeclaringType null
        check "ceklc09wlkm3" hostedType.DeclaringMethod null
        check "ceklc09wlkm4" hostedType.FullName "Microsoft.FSharp.Data.TypeProviders.SqlDataConnectionApplied"
        check "ceklc09wlkm5" (hostedType.GetConstructors()) [| |]
        check "ceklc09wlkm6" (hostedType.GetCustomAttributesData().Count) 1
        check "ceklc09wlkm6" (hostedType.GetCustomAttributesData().[0].Constructor.DeclaringType.FullName) typeof<TypeProviderXmlDocAttribute>.FullName
        check "ceklc09wlkm7" (hostedType.GetEvents()) [| |]
        check "ceklc09wlkm8" (hostedType.GetFields()) [| |]
        check "ceklc09wlkm9" [ for m in hostedType.GetMethods() -> m.Name ] [ "GetDataContext" ; "GetDataContext" ]
        let m0 = hostedType.GetMethods().[0]
        let m1 = hostedType.GetMethods().[1]
        check "ceklc09wlkm9b" (m0.GetParameters().Length) 0
        check "ceklc09wlkm9b" (m1.GetParameters().Length) 1
        check "ceklc09wlkm9b" (m0.ReturnType.Name) "Northwnd"
        check "ceklc09wlkm9b" (m0.ReturnType.FullName) "Microsoft.FSharp.Data.TypeProviders.SqlDataConnectionApplied+ServiceTypes+SimpleDataContextTypes+Northwnd"
        check "ceklc09wlkm10" (hostedType.GetProperties()) [| |]
        check "ceklc09wlkm11" (hostedType.GetNestedTypes().Length) 1
        check "ceklc09wlkm12" 
            (set [ for x in hostedType.GetNestedTypes() -> x.Name ]) 
            (set ["ServiceTypes"])

        let hostedServiceTypes = hostedType.GetNestedTypes().[0]
        check "ceklc09wlkm12b" (hostedServiceTypes.GetMethods()) [| |]
        check "ceklc09wlkm12c" (hostedServiceTypes.GetNestedTypes().Length) 38

        let hostedSimpleDataContextTypes = hostedServiceTypes.GetNestedType("SimpleDataContextTypes")
        check "ceklc09wlkm12d" (hostedSimpleDataContextTypes.GetMethods()) [| |]
        check "ceklc09wlkm12e" (hostedSimpleDataContextTypes.GetNestedTypes().Length) 1
        check "ceklc09wlkm12e" [ for x in hostedSimpleDataContextTypes.GetNestedTypes() -> x.Name] ["Northwnd"]

        check "ceklc09wlkm12" 
            (set [ for x in hostedServiceTypes.GetNestedTypes() -> x.Name ]) 
            (set ["Northwnd"; "SimpleDataContextTypes"; "AlphabeticalListOfProduct"; "Category"; "CategorySalesFor1997"; "CurrentProductList"; "CustomerAndSuppliersByCity"; 
                  "CustomerCustomerDemo"; "CustomerDemographic"; "Customer"; "Employee"; "EmployeeTerritory"; 
                  "Invoice"; "OrderDetail"; "OrderDetailsExtended"; "OrderSubtotal"; "Order"; "OrdersQry"; 
                  "ProductSalesFor1997"; "Product"; "ProductsAboveAveragePrice"; "ProductsByCategory"; 
                  "QuarterlyOrder"; "Region"; "SalesByCategory"; "SalesTotalsByAmount"; "Shipper"; 
                  "SummaryOfSalesByQuarter"; "SummaryOfSalesByYear"; "Supplier"; "Territory"; "CustOrderHistResult"; 
                  "CustOrdersDetailResult"; "CustOrdersOrdersResult"; "EmployeeSalesByCountryResult"; "SalesByYearResult"; 
                  "SalesByCategoryResult"; "TenMostExpensiveProductsResult"])

        let customersType = (hostedServiceTypes.GetNestedTypes() |> Seq.find (fun t -> t.Name = "Customer"))
        check "ceklc09wlkm13"  (customersType.GetProperties().Length) 13


    let instantiateTypeProviderAndCheckOneHostedType(connectionStringName, configFile, useDataDirectory, dataDirectory, useLocalSchemaFile: string option, useForceUpdate: bool option, typeFullPath: string[], resolutionFolder:string option) = 
        let assemblyFile = typeof<Microsoft.FSharp.Data.TypeProviders.DesignTime.DataProviders>.Assembly.CodeBase.Replace("file:///","").Replace("/","\\")
        test "cnlkenkewe" (File.Exists assemblyFile) 

        // If/when we care about the "target framework", this mock function will have to be fully implemented
        let systemRuntimeContainsType s = 
            printfn "Call systemRuntimeContainsType(%s) returning dummy value 'true'" s
            true

        let tpConfig = new TypeProviderConfig(systemRuntimeContainsType, ResolutionFolder=__SOURCE_DIRECTORY__, RuntimeAssembly=assemblyFile, ReferencedAssemblies=[| |], TemporaryFolder=Path.GetTempPath(), IsInvalidationSupported=false, IsHostedExecution=true)
        use typeProvider1 = (new Microsoft.FSharp.Data.TypeProviders.DesignTime.DataProviders( tpConfig ) :> ITypeProvider)

        let invalidateEventCount = ref 0

        typeProvider1.Invalidate.Add(fun _ -> incr invalidateEventCount)

        // Load a type provider instance for the type and restart
        let hostedNamespace1 = typeProvider1.GetNamespaces() |> Seq.find (fun t -> t.NamespaceName = "Microsoft.FSharp.Data.TypeProviders")

        check "eenewioinw" (set [ for i in hostedNamespace1.GetTypes() -> i.Name ]) (set ["DbmlFile"; "EdmxFile"; "ODataService"; "SqlDataConnection";"SqlEntityConnection";"WsdlService"])

        let hostedType1 = hostedNamespace1.ResolveTypeName("SqlDataConnection")
        let hostedType1StaticParameters = typeProvider1.GetStaticParameters(hostedType1)
        check "eenewioinw2" 
            (set [ for i in hostedType1StaticParameters -> i.Name ]) 
            (set ["ConnectionString"; "ConnectionStringName"; "DataDirectory"; "ResolutionFolder"; "ConfigFile"; "LocalSchemaFile"; 
                  "ForceUpdate"; "Pluralize"; "Views"; "Functions"; "StoredProcedures"; "Timeout"; 
                  "ContextTypeName"; "Serializable" ])

        let northwind = "NORTHWND.mdf"
        let northwindLog = "NORTHWND_log.ldf"
        let northwindFile = 
            let baseFile = 
                match dataDirectory with 
                | None -> northwind
                | Some dd -> System.IO.Path.Combine(dd,northwind)
            match resolutionFolder with 
            | None -> 
                match dataDirectory with 
                | None -> ()
                | Some dd -> if not(System.IO.Directory.Exists dd) then System.IO.Directory.CreateDirectory dd |> ignore
                baseFile
            | Some rf -> 
                if not(System.IO.Directory.Exists rf) then System.IO.Directory.CreateDirectory rf |> ignore
                match dataDirectory with 
                | None -> ()
                | Some dd -> let dd = System.IO.Path.Combine(rf,dd) in if not(System.IO.Directory.Exists dd) then System.IO.Directory.CreateDirectory dd |> ignore                
                System.IO.Path.Combine(rf,baseFile)


        if not(System.IO.File.Exists(northwindFile)) then 
            System.IO.File.Copy("DB\\northwnd.mdf", northwindFile, false)
            System.IO.File.SetAttributes(northwindFile, System.IO.FileAttributes.Normal)


        let connectionString = 
            if useDataDirectory then
                if isSQLExpressInstalled then
                    @"AttachDBFileName = '|DataDirectory|\" + northwind + "';Server='.\SQLEXPRESS';User Instance=true;Integrated Security=SSPI"
                else
                    "AttachDBFileName = '|DataDirectory|\\" + northwind + "';Server='(localdb)\\MSSQLLocalDB'"
            else
                if isSQLExpressInstalled then
                    @"AttachDBFileName = '" + System.IO.Path.Combine(__SOURCE_DIRECTORY__, northwindFile) + "';Server='.\SQLEXPRESS';User Instance=true;Integrated Security=SSPI"
                else
                    "AttachDBFileName = '" + System.IO.Path.Combine(__SOURCE_DIRECTORY__, northwindFile) + "';Server='(localdb)\\MSSQLLocalDB'"

        match connectionStringName with 
        | None -> ()
        | Some connectionStringName -> 
               let configFileName = match configFile with None -> "app.config" | Some nm -> nm
               let configFileName = 
                   match resolutionFolder with 
                   | None -> configFileName
                   | Some rf -> 
                       System.IO.Path.Combine(rf,configFileName)
               System.IO.File.WriteAllText(configFileName,
                   sprintf """<?xml version="1.0"?>

<configuration>
  <connectionStrings>
    <add name="%s"
         connectionString="%s"
         providerName="System.Data.SqlClient" />
  </connectionStrings>

  <system.webServer>
     <modules runAllManagedModulesForAllRequests="true"/>
  </system.webServer>
</configuration>
"""               
                       connectionStringName
                       connectionString)
        let staticParameterValues = 
            [| for x in hostedType1StaticParameters -> 
                (match x.Name with 
                 | "ConnectionString" when connectionStringName.IsNone  -> box connectionString
                 | "Pluralize" -> box true
                 | "ConnectionStringName" when connectionStringName.IsSome -> box connectionStringName.Value
                 | "ResolutionFolder" when resolutionFolder.IsSome -> box resolutionFolder.Value
                 | "DataDirectory" when dataDirectory.IsSome -> box dataDirectory.Value
                 | "ConfigFile"  when configFile.IsSome -> box configFile.Value
                 | "ContextTypeName" -> box "Northwnd" 
                 | "LocalSchemaFile" when useLocalSchemaFile.IsSome -> box useLocalSchemaFile.Value
                 | "ForceUpdate" when useForceUpdate.IsSome -> box useForceUpdate.Value
                 | "Timeout" -> box 60
                 | _ -> box x.RawDefaultValue) |]
        printfn "instantiating database type... may take a while for db to attach, code generation tool to run and csc.exe to run..."
        
        try
            let hostedAppliedType1 = typeProvider1.ApplyStaticArguments(hostedType1, typeFullPath, staticParameterValues)

            checkHostedType hostedAppliedType1
        with
        | e ->
            printfn "%s" (e.ToString())
            reportFailure()

    instantiateTypeProviderAndCheckOneHostedType(None, None, false, None, None, None, [| "SqlDataConnectionApplied" |], None)

    // Use an implied app.config config file, use the current directory as the DataDirectory
    instantiateTypeProviderAndCheckOneHostedType(Some "ConnectionString1", None, true, None, None, None, [| "SqlDataConnectionApplied" |], None)

    // Use a config file, use an explicit relative DataDirectory
    instantiateTypeProviderAndCheckOneHostedType(Some "ConnectionString2", Some "app.config", true, Some "DataDirectory", None, None, [| "SqlDataConnectionApplied" |], None)

    // Use a config file, use an explicit relative DataDirectory and an explicit ResolutionFolder.
    instantiateTypeProviderAndCheckOneHostedType(Some "ConnectionString2", Some "app.config", true, Some "DataDirectory", None, None, [| "SqlDataConnectionApplied" |], Some "ExampleResolutionFolder")

    
    // Use an absolute config file, use an absolute DataDirectory 
    instantiateTypeProviderAndCheckOneHostedType(Some "ConnectionString3", Some (__SOURCE_DIRECTORY__ + @"\test.config"), true, Some (__SOURCE_DIRECTORY__ + @"\DataDirectory"), None, None, [| "SqlDataConnectionApplied" |], None)

    let schemaFile2 = Path.Combine(__SOURCE_DIRECTORY__, "nwind2.dbml")
    (try File.Delete schemaFile2 with _ -> ())
    instantiateTypeProviderAndCheckOneHostedType(None, None, false, None, Some (Path.Combine(__SOURCE_DIRECTORY__, "nwind2.dbml")), Some true, [| "SqlDataConnectionApplied" |], None)
    // schemaFile2 should now exist
    check "eoinew0c9e" (File.Exists schemaFile2)

    // Reuse the DBML just created
    instantiateTypeProviderAndCheckOneHostedType(None, None, false, None, Some (Path.Combine(__SOURCE_DIRECTORY__, "nwind2.dbml")), Some false, [| "SqlDataConnectionApplied" |], None)
    // schemaFile2 should now still exist
    check "eoinew0c9e" (File.Exists schemaFile2)

    // // A relative path should work....
    // instantiateTypeProviderAndCheckOneHostedType(Some "nwind2.dbml", Some false)
    // // schemaFile2 should now still exist
    // check "eoinew0c9e" (File.Exists schemaFile2)

    let schemaFile3 = Path.Combine(__SOURCE_DIRECTORY__, "nwind3.dbml") 
    (try File.Delete schemaFile3 with _ -> ())
    instantiateTypeProviderAndCheckOneHostedType(None, None, false, None, Some schemaFile3, None, [| "SqlDataConnectionApplied" |], None)
    
    // schemaFile3 should now exist
    check "eoinew0c9e" (File.Exists schemaFile3)

let _ = 
    if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    File.WriteAllText("test.ok","ok"); 
    exit 0)

