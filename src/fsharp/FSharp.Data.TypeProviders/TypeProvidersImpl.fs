// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Data.TypeProviders.DesignTime

open System
open System.IO
open System.Reflection
open System.Collections.Generic
open System.Linq
open System.Xml.Linq
open System.Net
open System.Configuration
open Microsoft.FSharp.Data.TypeProviders.Utility

[<AutoOpen>]
module internal Utilities = 

    let validateSchemaFileExtension localSchemaFile ext = 
        // treat extensions that differ only in case as equal
        if not (Path.GetExtension(localSchemaFile).Equals("."+ext, StringComparison.OrdinalIgnoreCase)) then 
            failwith (FSData.SR.errorInvalidExtensionSchema(Path.GetFileName(localSchemaFile),ext))

    /// Use a local schema file if the 'localSchemaFile' parameter is present (IsNullOrWhiteSpace). Otherwise,
    /// use a temporary file to hold the metadata description of the service. Always force the update
    /// of the file if 'forceUpdate' is true and the downloaded contents have changed. 
    let tryUseLocalSchemaFile (localSchemaFile, forceUpdate, ext) loader =
        let useTempFile = String.IsNullOrWhiteSpace localSchemaFile
        let schemaFile = if useTempFile then Util.TemporaryFile ext else Util.ExistingFile localSchemaFile

        // Check if the extension of LocalSchema matches the given "ext"
        if not useTempFile then validateSchemaFileExtension localSchemaFile ext

        if useTempFile || forceUpdate || not (File.Exists schemaFile.Path) then 
            let metadataText = Util.TemporaryFile ext |> loader

            // Only write if it's actually changed
            if useTempFile || (try File.ReadAllText schemaFile.Path with _ -> null) <> metadataText then 
                try 
                   File.WriteAllText(schemaFile.Path, metadataText)
                with :? System.IO.IOException as e -> failwith (FSData.SR.errorWritingLocalSchemaFile(e.Message))

        schemaFile

    // tricky moment that error message can spread through multiple lines
    // so we:
    // - walk through lines until we met first line with error code, save this line        
    // - traverse lines till the next error code (or EOF). append every line to the buffer
    // - flush buffer on every line with error code (or at the end)
    let private selectLines hasErrorCode (arr : string[]) = 

        // sqlmetal\edmgen pads inner exceptions with tabs so if line starts with tab - this is not beginning of the message
        // svcutil pads inner exceptions with '    ' so this is not message start as well
        let (|MessageStart|_|) line = 
            if not (System.String.IsNullOrEmpty line) && line.[0] <> '\t' && not (line.StartsWith "    ") && hasErrorCode line then Some () else None
        
        let (|EmptyLine|_|) line = 
            if String.IsNullOrEmpty line then Some () else None
        
        [|
            let buf = System.Text.StringBuilder()
            for line in arr do
                match line with
                | MessageStart -> 
                    // return accumulated result with the prevous message
                    if buf.Length <> 0 then
                        yield buf.ToString()
                        buf.Length <- 0
                    // start new message
                    buf.Append(line.Trim()) |> ignore
                | EmptyLine ->
                    // // we assume that messages cannot contain embedded empty lines so empty line terminates current message
                    if buf.Length <> 0 then
                        yield buf.ToString()
                        buf.Length <- 0
                | l ->
                    // if we already started writing the message - then interleave lines in buffer with spaces
                    // if buffer.Length = 0 - this means that we previous error has already ended but MessageStart for the new one wasn't yet met.
                    // in this case we discard this line - it is probably header or footer of output
                    if buf.Length <> 0 then
                        buf
                            .Append(" ")
                            .Append(l.Trim()) |> ignore

            // flush remaining content
            if buf.Length <> 0 then
                yield buf.ToString()                       
        |]

    // Defines the main pattern for matching messages.
    // taken from vsproject\xmake\shared\canonicalerror.cs
    let private canonicalMessageRegex = 
        new System.Text.RegularExpressions.Regex            
                  (
                     // Beginning of line and any amount of whitespace.
                     @"^\s*" +
                     // Match a [optional project number prefix 'ddd>'], single letter + colon + remaining filename, or
                     // string with no colon followed by a colon.
                     @"(((?<ORIGIN>(((\d+>)?[a-zA-Z]?:[^:]*)|([^:]*))):)" +
                     // Origin may also be empty. In this case there's no trailing colon.
                     "|())" +
                     // Match the empty string or a string without a colon that ends with a space
                     "(?<SUBCATEGORY>(()|([^:]*? )))" +
                     // Match 'error' or 'warning'.
                     @"(?<CATEGORY>(error|warning))" +
                     // Match anything starting with a space that's not a colon/space, followed by a colon. 
                     // Error code is optional in which case "error"/"warning" can be followed immediately by a colon.
                     @"( \s*(?<CODE>[^: ]*))?\s*:" +
                     // Whatever's left on this line, including colons.
                     "(?<TEXT>.*)$",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase ||| System.Text.RegularExpressions.RegexOptions.Compiled
                  )

    // Defines the additional pattern for matching messages which may contain localized strings.
    // Note that the string "error|warning" from canonicalMessageRegex is simply replaced by "\w+"
    // to match both any localized strings and PLOC'd strings.
    // It has some group misalignment for PLOC'd strings, but as a whole, it works as expected.
    let private structuralMessageRegex = 
        new System.Text.RegularExpressions.Regex            
                  (
                     // Beginning of line and any amount of whitespace.
                     @"^\s*" +
                     // Match a [optional project number prefix 'ddd>'], single letter + colon + remaining filename, or
                     // string with no colon followed by a colon.
                     @"(((?<ORIGIN>(((\d+>)?[a-zA-Z]?:[^:]*)|([^:]*))):)" +
                     // Origin may also be empty. In this case there's no trailing colon.
                     "|())" +
                     // Match the empty string or a string without a colon that ends with a space
                     "(?<SUBCATEGORY>(()|([^:]*? )))" +
                     // Match localized 'error' or 'warning'.
                     @"(?<CATEGORY>(\w+))" +
                     // Match anything starting with a space that's not a colon/space, followed by a colon. 
                     // Error code is optional in which case "error"/"warning" can be followed immediately by a colon.
                     // constraints for CODE here is more restrictive than in cannonical case - here we match only alphanumeric chars
                     @"( \s*(?<CODE>\w*))?\s*:" +
                     // Whatever's left on this line, including colons.
                     "(?<TEXT>.*)$",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase ||| System.Text.RegularExpressions.RegexOptions.Compiled
                  )

    let formatErr (stdout: string[]) (stderr: string[])  : unit  =
        let getErrors s = 
            Array.ofSeq s
            |> selectLines (fun s -> 
                if canonicalMessageRegex.IsMatch s then true
                else 
                    // structural regex can produce false positives when line contain ':' chars
                    // to reduce amount of false positives we'll mangle the most frequent scenario when ':' can be met - urls.
                    // In addition, "!" which wraps PLOC'd strings will be also replaced to "_" to make the matching easier. 
                    let escapedLine = s.Replace("://", "_").Replace("!","_")
                    structuralMessageRegex.IsMatch escapedLine
                )
            |> Array.map Failure
        let exns = 
            Array.concat 
                [
                    getErrors stdout
                    getErrors stderr
                ] 
        assert (exns.Length <> 0)
        raise (AggregateException exns)
            
    /// Run SQLMetal and strip out a decent error message
    let sqlmetal args = Util.shell (Path.GetTempPath(), Util.sdkUtil "SqlMetal.exe", args, Some formatErr)

    let inline handleReadingSchemaError (e : exn) = 
        match e with
        | :? AggregateException as ae -> 
            let inners = 
                ae.InnerExceptions
                |> Seq.map (fun e -> Failure(FSData.SR.errorReadingSchema(e.Message)))
                |> Seq.toArray

            raise(AggregateException(inners))
        | e -> failwith (FSData.SR.errorReadingSchema(e.Message))

    // will exists forever - but I guess we can live with it as the leak is minor
    // CSharpCodeProvider is IDisposable - 
    // Dispose is inherited from Component: removes component from the ISite and unsubscribes from events - we don't use any of this features    
    let private csCodeProvider = new Microsoft.CSharp.CSharpCodeProvider()
    
    let validateDataContextClassName name = 
        if not(String.IsNullOrWhiteSpace name) && not (csCodeProvider.IsValidIdentifier name) then
            failwith (FSData.SR.invalidDataContextClassName(name))

    type UC = System.Globalization.UnicodeCategory

    let sanitizeDataContextFileName (name : string) = 
        if String.IsNullOrEmpty name then failwith (FSData.SR.invalidDataContextClassName(name))
        
        /// tests if specified character is valid start character for C# identifier
        let isValidStartChar c = Char.IsLetter c || c = '_'

        /// tests if specified character is valid non-start character for C# identifier
        let isValidNonStartChar c = 
            if Char.IsLetterOrDigit c || c = '_' then true
            else
            match Char.GetUnicodeCategory(c) with
            | UC.NonSpacingMark | UC.ConnectorPunctuation | UC.SpacingCombiningMark -> true
            | _ -> false

        let buf = System.Text.StringBuilder()
    
        let name = 
            // convert given identifier to the uniform representation
            // 1. make it all lower-cased
            // 2. convert all word starting letters to upper-case + remove whitespaces
            let name = name.ToLower()
            let buf = System.Text.StringBuilder()        
            let rec capitalizeFirstLetters i convertNextLetterToUpper = 
                if i >= name.Length then buf.ToString()
                else
                let c = name.[i]
                if Char.IsWhiteSpace c then 
                    // whitespace - set convertNextLetterToUpper to true and proceed to the next character (thus removing whitespace from the result buffer)
                    capitalizeFirstLetters (i + 1) true
                else
                    // check if we see whitespace before.
                    // if yes and current character is letter - it should be converted it to upper case before appending character to the result buffer
                    let c = if Char.IsLetter c && convertNextLetterToUpper then Char.ToUpper c else c
                    buf.Append(c) |> ignore
                    capitalizeFirstLetters (i + 1) false

            capitalizeFirstLetters 0 true

        let startIndex = 
            let first = name.[0]
            if isValidStartChar first then 
                // if first character is valid start character for C# identifier => append it to the result buffer
                buf.Append(first) |> ignore
                1
            elif isValidNonStartChar first then
                // first character in name is not valid start character but valid as non-start. => it should be prepended with '_'
                buf.Append('_').Append(first) |> ignore
                1
            else 
                // first character is invalid for usage in C# identifier => let loop below do all job (this char will be replaced with '_')
                0
        
        // check char by char if they can be used in C# identifier. invalid characters are replaced with '_'
        for i=startIndex to (name.Length - 1) do
            let c = name.[i]
            let c = if isValidNonStartChar c then c else '_'
            buf.Append(c) |> ignore

        // pad result with underscores if it occasionaly matches some C# keyword or reserved word
        csCodeProvider.CreateValidIdentifier(buf.ToString())   

module internal ConfigFiles =
    
    type ConfigFileSearchResult = 
        | StandardFound of string
        | CustomFound of string
        | StandardNotFound
        | CustomNotFound of string

    /// searches for the configuration file that will be used by TP
    /// designTimeDirectory- root folder
    /// configFileName - full path to the custom configuration file
    let findConfigFile (designTimeDirectory, configFileName) = 
        if String.IsNullOrWhiteSpace configFileName then 
            let appConfig = System.IO.Path.Combine(designTimeDirectory,"app.config") 
            let webConfig = System.IO.Path.Combine(designTimeDirectory,"web.config") 
            if System.IO.File.Exists appConfig then StandardFound appConfig
            elif System.IO.File.Exists webConfig then StandardFound webConfig
            else StandardNotFound
        else 
            if System.IO.File.Exists configFileName then 
                CustomFound configFileName
            else 
                CustomNotFound configFileName

    [<RequireQualifiedAccess>]
    type ConnectionStringReadResult = 
        | Ok of ConnectionStringSettings
        | NotFound
        | Error of exn

    /// reads ConnectionStringSettings with name <connectionStringName> from the specified file
    /// returns either ConnectionStringReadResult.Ok (cs : ConnectionStringSettings) (cs is not null)
    /// or ConnectionStringReadResult.NotFound if configuration file doesn't contain ConnectionStringSettings with specified name
    /// or ConnectionStringReadResult.Error if reading of the file ends with error
    let tryReadConnectionString (configFileName : string, connectionStringName : string) = 
        try 
            // copy original file to avoid its locking
            let text = System.IO.File.ReadAllText configFileName
            use tmpConfigFile = Util.TemporaryFile  "config"
            do System.IO.File.WriteAllText(tmpConfigFile.Path,text)
            let map = ExeConfigurationFileMap(ExeConfigFilename = tmpConfigFile.Path)
            let config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            let connString = config.ConnectionStrings.ConnectionStrings.[connectionStringName]
            if connString <> null then ConnectionStringReadResult.Ok connString
            else ConnectionStringReadResult.NotFound
        with exn -> 
            ConnectionStringReadResult.Error exn


    let computeStaticConnectionString (name,checkForSqlClientProvider, connectionString, connectionStringName, configFileName, absoluteDataDirectoryParam, designTimeDirectory) = 
        let connectionString, usingConfigFileInfo = 
            if String.IsNullOrWhiteSpace connectionString then 

                let configFileName, configFileBaseNameOpt = 
                    match findConfigFile (designTimeDirectory, configFileName) with
                    | StandardFound path ->
                        let name = Path.GetFileName path
                        path, Some name
                    | CustomFound path -> path, None
                    | StandardNotFound -> failwith (FSData.SR.noConfigFileFound1())
                    | CustomNotFound name -> failwith (FSData.SR.noConfigFileFound2(name))

                if String.IsNullOrWhiteSpace connectionStringName then 
                    failwith (FSData.SR.noConnectionStringOrConnectionStringName(name))
                
                let mkReadConnectionStringError msg = 
                    FSData.SR.errorWhileReadingConnectionStringInConfigFile (connectionStringName, configFileName, msg)
                    |> Choice2Of2

                let result = 
                    match tryReadConnectionString(configFileName, connectionStringName) with
                    | ConnectionStringReadResult.Ok connString ->                        
                        if checkForSqlClientProvider && connString.ProviderName <> "System.Data.SqlClient" then 
                            Choice2Of2 (FSData.SR.invalidProviderInConfigFile (connString.ProviderName, connectionStringName, configFileName))

                        else
                            let result = connString.ConnectionString
                            if String.IsNullOrWhiteSpace result then 
                                Choice2Of2 (FSData.SR.invalidConnectionStringInConfigFile (result, connectionStringName, configFileName))
                            else
                                Choice1Of2  result
                    | ConnectionStringReadResult.NotFound -> 
                        FSData.SR.connectionStringNotFound(connectionStringName)
                        |> mkReadConnectionStringError

                    | ConnectionStringReadResult.Error exn -> 
                        mkReadConnectionStringError exn.Message

                match result with 
                | Choice1Of2 r -> r, Some (connectionStringName, configFileBaseNameOpt)
                | Choice2Of2 msg -> failwith msg
            else
                if not (String.IsNullOrWhiteSpace connectionStringName) then 
                    failwith (FSData.SR.notBothConnectionStringOrConnectionStringName())
                connectionString, None

        // Always do |DataDirectory| substitution
        let connectionString = 
            let absoluteDataDirectory = 
                if String.IsNullOrWhiteSpace absoluteDataDirectoryParam then 
                    designTimeDirectory 
                else 
                    if not (System.IO.Directory.Exists absoluteDataDirectoryParam) then 
                        failwith (FSData.SR.dataDirectoryNotFound(absoluteDataDirectoryParam))
                    absoluteDataDirectoryParam            
            connectionString.Replace("|DataDirectory|", absoluteDataDirectory)
        
        connectionString, usingConfigFileInfo

module internal Dbml =

    let buildType (dbmlFileContent: string, contextTypeName:string, serializable:bool)  =
        
        validateDataContextClassName contextTypeName

        // Remove the Database/Connection element if exist, which can be created by the O/R Designer
        // Its attributes, "SettingsObjectName" and "SettingsPropertyName" can cause sqlmetal to generate a problematic csharp file 
        let dbmlFileContent =
            if dbmlFileContent.Contains("Connection") then
                let xdoc = XDocument.Parse dbmlFileContent
                xdoc.Elements()
                |> Seq.filter (fun xnode -> xnode.Name.LocalName = "Database")
                |> Seq.choose (fun xnode -> 
                    match xnode.Elements(xnode.Name.Namespace + "Connection") with 
                    | null -> None 
                    | s -> Some (s))
                |> Seq.iter (fun xchild -> xchild.Remove())
                xdoc.ToString()
            else dbmlFileContent
                    
        use csFile = Util.TemporaryFile "cs"
        use dbmlFile = Util.TemporaryFile "dbml"
        File.WriteAllText (dbmlFile.Path, dbmlFileContent)

        let sqlMetalArgs = 
            let command = sprintf "/code:\"%s\" /language:C#  \"%s\"" csFile.Path dbmlFile.Path
            let command = if not (String.IsNullOrWhiteSpace contextTypeName) then sprintf "%s /context:\"%s\" " command contextTypeName else command
            let command = if serializable then sprintf "%s /serialization:Unidirectional" command else command
            command

        sqlmetal sqlMetalArgs

        let assemblyFile = Util.TemporaryFile "dll" // TODO: load in-memory and clean up this file

        let cscCommand = sprintf "/nologo /target:library /out:\"%s\" \"%s\"" assemblyFile.Path csFile.Path
        Util.shell(Path.GetTempPath(), Util.cscExe(), cscCommand, None) 
            
        assemblyFile

module internal SqlConnection =

    let getDbml (dbmlFile: Util.FileResource, connString, flagPluralize, flagViews, flagFunctions, flagStoredProcs, flagTimeout, contextTypeName) = 
        
        validateDataContextClassName contextTypeName
        
        let sqlMetalArgs = 
            let command = sprintf "/conn:\"%s\" /dbml:\"%s\"" connString dbmlFile.Path
            let command = if flagPluralize   then sprintf "%s /pluralize" command else command
            let command = if flagViews       then sprintf "%s /views"     command else command
            let command = if flagFunctions   then sprintf "%s /functions" command else command
            let command = if flagStoredProcs then sprintf "%s /sprocs"    command else command
            let command = if not (String.IsNullOrWhiteSpace contextTypeName)    then sprintf "%s /context:\"%s\" " command contextTypeName else command
            let command = sprintf "%s /timeout:%d" command flagTimeout 
            command
        sqlmetal sqlMetalArgs
        File.ReadAllText dbmlFile.Path


    let buildType (connectionString, connectionStringName:string, configFileName:string, absoluteDataDirectory:string, localSchemaFile:string, forceUpdate:bool, flagPluralize, flagViews, flagFunctions, flagStoredProcs, flagTimeout, contextTypeName, serializable, designTimeDirectory)  =
        let connectionString, usingConfigFileInfo = ConfigFiles.computeStaticConnectionString ("SqlDataConnection", true, connectionString, connectionStringName, configFileName, absoluteDataDirectory, designTimeDirectory)
        
        // parses connection string on components, validates only syntax
        let connStringBuilder = System.Data.Common.DbConnectionStringBuilder()
        connStringBuilder.ConnectionString <- connectionString
        let contextTypeName = 
            // if context is not explicitly set - try to infer context name from database filename
            if String.IsNullOrWhiteSpace(contextTypeName) then
                let dbFileNameOpt = 
                    // SqlCE allows usage of both 'Data Source' and 'DataSource' names and treats them as equivalent
                    // however SqlMetal doesn't recognize 'DataSource' and always looks for 'Data Source'
                    // so to figure out if given connection string is SqlCE - we'll get value of 'Data Source' and check if it ends with .sdf
                    // NOTE: we cannot just use SqlConnectionStringBuilder because it rejects some SqlCE specific parameter names
                    match connStringBuilder.TryGetValue ("Data Source") with
                    | true, (:? string as ds) when ds.EndsWith(".sdf", System.StringComparison.OrdinalIgnoreCase) -> Some ds // SqlCE
                    | _ -> 
                        // SqlServer
                        let sqlConnString = System.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
                        if not (String.IsNullOrEmpty sqlConnString.AttachDBFilename) then
                            Some (sqlConnString.AttachDBFilename)
                        else 
                            None

                match dbFileNameOpt with
                | Some f -> Path.GetFileNameWithoutExtension f |> sanitizeDataContextFileName
                | None -> contextTypeName
            else
                contextTypeName

        use dbmlFile = 
            tryUseLocalSchemaFile 
                (localSchemaFile, forceUpdate, "dbml") 
                (fun dbmlFile -> 
                  try 
                    getDbml (dbmlFile,connectionString, flagPluralize, flagViews, flagFunctions, flagStoredProcs, flagTimeout, contextTypeName)
                  with 
                    e -> handleReadingSchemaError e)

        use csFile = Util.TemporaryFile "cs"
        let sqlMetalArgs = 
            let command = sprintf "/code:\"%s\" /language:C#  \"%s\"" csFile.Path dbmlFile.Path 
            let command = if not (String.IsNullOrWhiteSpace contextTypeName)    then sprintf "%s /context:\"%s\" "    command contextTypeName else command
            let command = if serializable then sprintf "%s /serialization:Unidirectional" command else command
            command
        sqlmetal sqlMetalArgs
        
        let assemblyFile = Util.TemporaryFile "dll" // TODO: load in-memory and clean up this file

        let cscCommand = sprintf "/nologo /target:library /out:\"%s\" \"%s\"" assemblyFile.Path csFile.Path
        Util.shell(Path.GetTempPath(), Util.cscExe(), cscCommand, None) 
            
        assemblyFile, usingConfigFileInfo




module internal DataSvcUtil =

    type MetadataError = FixedQueryNotSupported | DataQualityServiceNotSupported
    module Metadata = 
    
        let private edmx = XNamespace.Get "http://schemas.microsoft.com/ado/2007/06/edmx"
        let private dr = XNamespace.Get "http://schemas.microsoft.com/dallas/2010/04"
        let private edm08 = XNamespace.Get "http://schemas.microsoft.com/ado/2009/08/edm"
        let private edm05 = XNamespace.Get "http://schemas.microsoft.com/ado/2007/05/edm"

        let private get (name : XName) (c : XContainer) = 
            match c.Element name with
            | null -> None
            | el -> Some (el :> XContainer)
        
        /// Analyzes given metadata file and tries to find evidence of specific cases that may cause datasvcutil to fail
        let TryPredictError (path : string) = 
            try
                let xdoc = XDocument.Load(path)
                let error =                     
                    defaultArg (get (edmx + "Edmx") xdoc) (xdoc :> XContainer)
                    |> get (edmx + "DataServices")
                    |> Option.bind (fun ds -> 
                        match get (edm08 + "Schema") ds with
                        | Some el -> Some (el, edm08)
                        | None -> 

                        match get (edm05 + "Schema") ds with
                        | Some el -> Some (el, edm05)
                        | None -> None
                    )
                    |> Option.bind (fun (schema, ns) ->
                        // check if metadata contains node
                        // <Using Namespace="Microsoft.DQS">
                        // this indicates that target service implements DataQuality API - datasvcutil cannot consume it correctly
                        let isDQS = 
                            match get (ns + "Using") schema with
                            | Some (:? XElement as e) -> 
                                match e.Attribute(XName.Get "Namespace") with
                                | null -> false
                                | attr -> attr.Value = "Microsoft.DQS"
                            | _ -> false
                        
                        if isDQS then Some DataQualityServiceNotSupported
                        else

                        // check all EntityType nodes
                        // if all of them has <Property Queryable='false'> - then it may indicate that service support fixed query
                        let entityTypes = schema.Elements(ns + "EntityType")
                        let isFixed = 
                            entityTypes.Elements (ns + "Property")
                            |> Seq.forall (fun el ->
                                let attr = el.Attribute(dr + "Queryable")
                                attr <> null && attr.Value = "false"
                                )

                        if isFixed then Some FixedQueryNotSupported 
                        else None
                    )
                error
            with _ -> None // swallow this error (if datasvcutil will fail - we'll report errors from the output) 

    /// Add the default service URI to the generated CS file by inserting it as the argument
    /// to the default data context constructor, also drop the namespace declaration.
    let addDefaultUriToDataSvcUtilCSharpFile uri csFile = 
        let csText = File.ReadAllLines csFile

        let (|DataContextClass|_|) (line:string) =
            let trimmed = line.Trim('{',' ','\t') 
            let starting = "public partial class "
            let ending = "System.Data.Services.Client.DataServiceContext"
            if trimmed.StartsWith starting && trimmed.Contains ":" && trimmed.EndsWith ending then
                let s = trimmed.Substring starting.Length
                Some (s.Substring(0, s.IndexOf ' '), line.Contains "{")
            else None

        let doctoredText =
            [|  // Was the last line a namespace?
                let prevNS = ref false
                let foundDataContextClass = ref None
                for line in csText do
                    match line with
                    |   _ when prevNS.Value -> prevNS := false // note, the line after the namespace is dropped
                    |   _ when line.StartsWith "namespace" -> prevNS := true // note, the namespace declaration is dropped

                    |   DataContextClass (className, endsWithBrace) ->
                            foundDataContextClass := Some (className, endsWithBrace)
                            yield line
                    |   _ -> 
                         match line.Trim(), foundDataContextClass.Value with 
                         | "{", Some(className, false) 
                         | "", Some(className, true) ->
                            yield line
                            yield sprintf "public %s() : this(new global::System.Uri(\"%s\")) {}" className uri
                            foundDataContextClass := None

                         |   _ when line <> "}" -> yield line
                         |   _ -> ()
            |] 
        File.WriteAllLines(csFile, doctoredText)

    let buildTypeFromMetadataUri (uri : string, localSchemaFile:string, forceUpdate:bool, dataServiceCollection) =

        use csdlFile = 
         tryUseLocalSchemaFile (localSchemaFile, forceUpdate, "csdl") (fun _ -> 
            try 
                let metadataUri =
                    if uri.EndsWith "/" then uri + "$metadata"
                    else uri + "/$metadata"
                let metadataText = 
                    use c = new WebClient(UseDefaultCredentials=true) // TODO: consider adding schema credentials here 
                    c.DownloadString metadataUri
                metadataText
            with e -> handleReadingSchemaError e)
        
        let expectedError = Metadata.TryPredictError csdlFile.Path

        use csFile = Util.TemporaryFile "cs"
        let dataSvcUtilArgs = 
            let args = sprintf "/nologo /in:\"%s\" /out:\"%s\" /language:CSharp /version:2.0" csdlFile.Path csFile.Path
            let args = if dataServiceCollection then sprintf "%s /DataServiceCollection" args else args
            args
        try
            Util.shell(Path.GetTempPath(), Util.dataSvcUtilExe(), dataSvcUtilArgs, Some formatErr)
        with
            _ ->
                // check if we expect errors in this file
                match expectedError with
                | Some FixedQueryNotSupported -> failwith (FSData.SR.fixedQueriesNotSupported())
                | Some DataQualityServiceNotSupported -> failwith (FSData.SR.dqsServicesNotSupported())
                | None -> reraise()

        addDefaultUriToDataSvcUtilCSharpFile uri csFile.Path
        
        let assemblyFile = Util.TemporaryFile "dll" // TODO: load in-memory and clean up this file
        
        let cscCommand = sprintf "/nologo /target:library /r:System.Data.Services.Client.dll /out:\"%s\" \"%s\"" assemblyFile.Path csFile.Path
        Util.shell(Path.GetTempPath(), Util.cscExe(), cscCommand, None)             
        assemblyFile


module internal SvcUtil =
    
    let serviceMetadataFilesXName = XName.Get "ServiceMetadataFiles"
    let serviceMetadataFileXName = XName.Get "ServiceMetadataFile"
    let fileNameXName = XName.Get "name"

    /// assumes that destFolder is already created
    let private unpackFolder (sourceFile : string) (destFolder : string) =
        let xml = XDocument.Load sourceFile
        for fileNode in xml.Root.Elements(serviceMetadataFileXName) do
            let name = fileNode.Attribute(fileNameXName)
            let resultPath = Path.Combine(destFolder, name.Value)
            let root = 
                fileNode.Elements() 
                |> Seq.tryFind(fun _ -> true) // pick first element
            match root with
            | Some root -> root.Save(resultPath)
            | None -> failwith (FSData.SR.serviceMetadataFileElementIsEmpty())

    let private packFolder sourceFolder (destFile : string) patterns =
        let root =XElement(serviceMetadataFilesXName)
        let doc = XDocument(root)

        let append pat = 
            for f in Directory.GetFiles(sourceFolder, pat) do
                let xml = XDocument.Load(f)
                let node = XElement(serviceMetadataFileXName)
                let fileName = Path.GetFileName f
                // add attribute with the name of source file
                node.Add(XAttribute(fileNameXName, fileName))
                // append file content
                node.Add(xml.Root)

                root.Add(node)

        for pat in patterns do
            append pat

        doc.Save(destFile)

    let validateWsdlUri uri = 
        if String.IsNullOrWhiteSpace uri then
            failwith (FSData.SR.invalidWsdlUri())

    let buildTypeFromWsdlUri (namespaceName, wsdlUri : string, localSchemaFile:string, forceUpdate:bool, messageContract, enableDataBinding, serializable, async, collectionType) =
        
        validateWsdlUri wsdlUri

        use metadataDir = 
            let tempFolder = Util.TemporaryDirectory()
            
            let schemaFileSet = not (String.IsNullOrWhiteSpace localSchemaFile)
            let schemaFileExists = schemaFileSet && ( try File.Exists(localSchemaFile) with _ -> false )

            if schemaFileSet then validateSchemaFileExtension localSchemaFile "wsdlschema"

            // downloads metadata to the specified temp folder
            // if localSchemaFile is set - pack downloaded content as localSchemaFile
            let downloadMetadata () = 
                if (try System.IO.Directory.Exists tempFolder.Path with _ -> false) then 
                    (try System.IO.Directory.Delete (tempFolder.Path, true) with _ -> ())
                if not (System.IO.Directory.Exists tempFolder.Path) then 
                    System.IO.Directory.CreateDirectory tempFolder.Path |> ignore
                let svcUtilArgs = 
                    let args = sprintf "/nologo /t:metadata \"%s\"" wsdlUri 
                    args
        
                Util.shell(tempFolder.Path, Util.svcUtilExe(), svcUtilArgs, Some formatErr)

                if schemaFileSet then
                    packFolder tempFolder.Path localSchemaFile ["*.wsdl"; "*.xsd"]
            
            if not (schemaFileExists) || forceUpdate then
                downloadMetadata()
            else
                // reraise exceptions if local schema file is corrupted
                try
                    unpackFolder localSchemaFile tempFolder.Path 
                with
                    e -> failwith (FSData.SR.errorReadingSchema(e.Message))


            tempFolder

        use configFile = Util.TemporaryFile "config" //Util.ExistingFile (System.IO.Path.Combine(metadataDir.Path, configFileName))
        use csFile = Util.TemporaryFile "cs"

        let svcUtilArgs = 
            let args = sprintf "/nologo  /out:\"%s\" /config:\"%s\" /language:CSharp /serializer:Auto /fault" csFile.Path  configFile.Path
            let args = if System.IO.Directory.EnumerateFiles(metadataDir.Path, "*.wsdl") |> Seq.isEmpty then args else sprintf "%s \"%s\"" args (Path.Combine(metadataDir.Path,"*.wsdl"))
            let args = if System.IO.Directory.EnumerateFiles(metadataDir.Path, "*.xsd") |> Seq.isEmpty then args else sprintf "%s \"%s\"" args (Path.Combine(metadataDir.Path,"*.xsd"))
            let args = if messageContract                           then sprintf "%s /messageContract"   args else args
            let args = if enableDataBinding                         then sprintf "%s /enableDataBinding" args else args
            let args = if serializable                              then sprintf "%s /serializable"      args else args
            let args = if async                                     then sprintf "%s /async"             args else args
            let args = if not (String.IsNullOrWhiteSpace collectionType) then sprintf "%s /collectionType:%s" args collectionType else args
            args
        
        Util.shell(Path.GetTempPath(), Util.svcUtilExe(), svcUtilArgs, Some formatErr)
        
        let assemblyFile = Util.TemporaryFile "dll" // TODO: load in-memory and clean up this file
        
        let cscCommand = sprintf "/nologo /target:library /resource:\"%s\",%s.config,public /out:\"%s\" \"%s\"" configFile.Path namespaceName assemblyFile.Path csFile.Path
        Util.shell(Path.GetTempPath(), Util.cscExe(), cscCommand, None)             

        let endPointNames = 
            try 
                let config = ConfigurationManager.OpenMappedExeConfiguration(ExeConfigurationFileMap(ExeConfigFilename = configFile.Path), ConfigurationUserLevel.None);
                [ for ep in ((config.GetSectionGroup "system.serviceModel").Sections.["client"] :?> System.ServiceModel.Configuration.ClientSection).Endpoints do 
                    yield ep.Name ]
            with _ -> 
                []

        assemblyFile, endPointNames

module internal Edmx =
    type private EdmxNamespaceGroup = { Edmx : XNamespace; Edm : XNamespace; Ssdl :XNamespace; Msl : XNamespace; RequireDotNet45 : bool }
    let private makeGroup (edmx : string, edm : string, ssdl : string, msl : string, requireDotNet45)= 
        { Edmx = XNamespace.Get edmx
          Edm = XNamespace.Get edm
          Ssdl = XNamespace.Get ssdl
          Msl = XNamespace.Get msl 
          RequireDotNet45 = requireDotNet45 }

    let private ns45 = 
        makeGroup 
            (
                "http://schemas.microsoft.com/ado/2009/11/edmx",
                "http://schemas.microsoft.com/ado/2009/11/edm",
                "http://schemas.microsoft.com/ado/2009/11/edm/ssdl",
                "http://schemas.microsoft.com/ado/2009/11/mapping/cs",
                true
             )

    let private ns40 =
        makeGroup 
            (
                "http://schemas.microsoft.com/ado/2008/10/edmx",
                "http://schemas.microsoft.com/ado/2008/09/edm",
                "http://schemas.microsoft.com/ado/2009/02/edm/ssdl",
                "http://schemas.microsoft.com/ado/2008/09/mapping/cs",
                false
             )

    let private ns20 =
        makeGroup 
            (
                "http://schemas.microsoft.com/ado/2007/06/edmx",
                "http://schemas.microsoft.com/ado/2006/04/edm",
                "http://schemas.microsoft.com/ado/2006/04/edm/ssdl",
                "urn:schemas-microsoft-com:windows:storage:mapping:CS",
                false
             )

    let loadEdmxFile path = 
        let getElement (c : XContainer) (name : string) (ns : XNamespace) = 
            match c.Element (ns + name) with
            | null -> failwith (FSData.SR.fileDoesNotContainXMLElement(path, name))
            | el -> el

        let xdoc = 
            match XDocument.Load path with
            | null -> failwith (FSData.SR.failedToLoadFileAsXML(path))
            | xdoc -> xdoc
        let extractEdmxComponents (nsGroup : EdmxNamespaceGroup) = 
            let edmxNode = getElement xdoc "Edmx" nsGroup.Edmx
            let runtimeNode = getElement edmxNode "Runtime" nsGroup.Edmx
            let csdlNode = 
                let conceptualModelNode = getElement runtimeNode "ConceptualModels" nsGroup.Edmx
                getElement conceptualModelNode "Schema" nsGroup.Edm
            let ssdlNode = 
                let storageModelNode = getElement runtimeNode "StorageModels" nsGroup.Edmx
                getElement storageModelNode "Schema" nsGroup.Ssdl
            let mslNode = 
                let mappingsNode = getElement runtimeNode "Mappings" nsGroup.Edmx
                getElement mappingsNode "Mapping" nsGroup.Msl
            (csdlNode.ToString()), (ssdlNode.ToString()), (mslNode.ToString()), nsGroup.RequireDotNet45

        // tries to process given edmx using supplied set of namespace groups
        let rec load = function
            | [] -> assert false; failwith "impossible"
            | x::xs -> 
                try extractEdmxComponents x 
                with e ->
                    match xs with
                    | [] -> reraise() // we were not able to load file - raise exception
                    | _ -> load xs
        load [ns45; ns40; ns20]

    let buildTypeFromEdmxFile (edmxFile, isTargetingDotNet45) =

        use csDir = Util.TemporaryDirectory()
        let baseContainerName = Path.GetFileNameWithoutExtension edmxFile
        let csFile = Path.Combine(csDir.Path, baseContainerName + ".cs")

        let csdlFileName = baseContainerName + ".csdl"
        let mslFileName = baseContainerName + ".msl"
        let ssdlFileName = baseContainerName + ".ssdl"

        let csdlFile = Path.Combine(csDir.Path, csdlFileName)
        let mslFile = Path.Combine(csDir.Path, mslFileName)
        let ssdlFile = Path.Combine(csDir.Path, ssdlFileName)
        let csdlContent, ssdlContent, mslContent, requireDotNet45 = loadEdmxFile edmxFile

        if requireDotNet45 && not isTargetingDotNet45 then
            failwith (FSData.SR.edmxFileRequiresDotNet45(edmxFile))

        File.WriteAllText(csdlFile, csdlContent)
        File.WriteAllText(mslFile, mslContent)
        File.WriteAllText(ssdlFile, ssdlContent)

        let edmGenArgs = 
            let args = sprintf "/mode:EntityClassGeneration /incsdl:\"%s\" /outobjectlayer:\"%s\" /nologo" csdlFile csFile
            let args = if isTargetingDotNet45 then sprintf "%s /targetversion:4.5" args else args
            args
        
        Util.shell(csDir.Path, Util.edmGenExe(), edmGenArgs, Some formatErr)
        let assemblyFile = Util.TemporaryFile "dll" // TODO: load in-memory and clean up this file

        let cscCommand = sprintf "/nologo /resource:\"%s\",\"%s\"  /resource:\"%s\",\"%s\" /resource:\"%s\",\"%s\" /target:library /r:System.Data.Entity.dll /out:\"%s\" \"%s\"" csdlFile csdlFileName  mslFile mslFileName ssdlFile ssdlFileName assemblyFile.Path csFile

        Util.shell(csDir.Path, Util.cscExe(), cscCommand, None)
        assemblyFile

module internal SqlEntityConnection =

    let getSsdl (namespaceName, ssdlFile: Util.FileResource, provider, connString, flagPluralize, flagSuppressForeignKeyProperties, isTargetingDotNet45) = 
        let edmGenArgs = 
            let args = sprintf "/namespace:\"%s\" /project:\"%s\" /outssdl:\"%s\" /mode:FullGeneration  /connectionstring:\"%s\" " namespaceName (Path.GetFileNameWithoutExtension ssdlFile.Path) ssdlFile.Path connString 
            let args = if not (String.IsNullOrWhiteSpace provider)      then sprintf "%s /provider:%s" args provider else args
            let args = if flagPluralize                                 then sprintf "%s /pluralize" args else args
            let args = if flagSuppressForeignKeyProperties              then sprintf "%s /SuppressForeignKeyProperties" args else args
            let args = if isTargetingDotNet45                           then sprintf "%s /targetversion:4.5" args else args
            args
        Util.shell(System.IO.Path.GetDirectoryName ssdlFile.Path, Util.edmGenExe(), edmGenArgs, Some formatErr)
        File.ReadAllText ssdlFile.Path


    let computeEntityContainerName (entityContainer:string) =
        if String.IsNullOrWhiteSpace entityContainer then 
            "EntityContainer" 
        else 
            entityContainer 

    let buildType (namespaceName, connectionString, connectionStringName, configFileName, absoluteDataDirectory, localSchemaFile:string, provider:string, entityContainer:string, forceUpdate:bool, flagPluralize, flagSuppressForeignKeyProperties, designTimeDirectory, isTargetingDotNet45)  =
        let connectionString, usingConfigFileInfo = ConfigFiles.computeStaticConnectionString ("SqlEntityConnection", false, connectionString, connectionStringName, configFileName, absoluteDataDirectory, designTimeDirectory)
        let entityContainer = computeEntityContainerName entityContainer
        use ssdlFile = 
            tryUseLocalSchemaFile 
                (localSchemaFile, forceUpdate, "ssdl") 
                (fun ssdlFile -> 
                    try 
                        getSsdl (namespaceName, ssdlFile, provider, connectionString, flagPluralize, flagSuppressForeignKeyProperties, isTargetingDotNet45)
                    with 
                        e -> handleReadingSchemaError e)

        use csFile = Util.TemporaryFile "cs"
        use csdlFile = Util.TemporaryFile "csdl"
        use mslFile = Util.TemporaryFile "msl"
        use viewsFile = Util.TemporaryFile "Views.cs"

        let edmGenArgs = 
            let args = sprintf "/mode:FromSsdlGeneration /entitycontainer:\"%s\" /namespace:\"%s\"  /project:\"%s\" /inssdl:\"%s\" /outobjectlayer:\"%s\" /outcsdl:\"%s\" /outmsl:\"%s\" /outviews:\"%s\" /language:CSharp" entityContainer namespaceName (Path.GetFileNameWithoutExtension ssdlFile.Path) ssdlFile.Path  csFile.Path csdlFile.Path mslFile.Path viewsFile.Path
            let args = if not (String.IsNullOrWhiteSpace provider)      then sprintf "%s /provider:%s" args provider else args
            let args = if flagPluralize                                 then sprintf "%s /pluralize" args else args
            let args = if flagSuppressForeignKeyProperties              then sprintf "%s /SuppressForeignKeyProperties" args else args
            let args = if isTargetingDotNet45                           then sprintf "%s /targetversion:4.5" args else args
            args
        Util.shell(System.IO.Path.GetDirectoryName ssdlFile.Path, Util.edmGenExe(), edmGenArgs, Some formatErr)
        
        let assemblyFile = Util.TemporaryFile "dll" 

        let csdlFileName = namespaceName + ".csdl"
        let mslFileName = namespaceName + ".msl"
        let ssdlFileName = namespaceName + ".ssdl"

        let cscCommand = sprintf "/r:System.Data.Entity.dll /nologo /resource:\"%s\",\"%s\"  /resource:\"%s\",\"%s\" /resource:\"%s\",\"%s\" /nologo /target:library /out:\"%s\" \"%s\"" csdlFile.Path csdlFileName  mslFile.Path mslFileName ssdlFile.Path ssdlFileName assemblyFile.Path csFile.Path
        Util.shell(Path.GetTempPath(), Util.cscExe(), cscCommand, None) 
            
        assemblyFile, usingConfigFileInfo


