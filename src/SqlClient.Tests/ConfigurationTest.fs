﻿module FSharp.Data.SqlClient.ConfigurationTests

open Xunit
open FsUnit.Xunit
open System.Configuration
open System.IO
open FSharp.Data

[<Fact>]
let ``Wrong config file name`` () = 
    should throw typeof<FileNotFoundException> <| fun() ->
        Configuration.ReadConnectionStringFromConfigFileByName ( name = "", resolutionFolder = "", fileName = "non_existent") |> ignore

[<Fact>]
let ``From config file`` () = 
    Configuration.ReadConnectionStringFromConfigFileByName(
        name = "AdventureWorks2012", 
        resolutionFolder = __SOURCE_DIRECTORY__,
        fileName = "app.config"
    ) 
    |> should equal ConfigurationManager.ConnectionStrings.["AdventureWorks2012"].ConnectionString

[<Fact>]
let RuntimeConfig () = 
    let name = "AdventureWorks2012"
    Configuration.GetConnectionStringAtRunTime name
    |> should equal ConfigurationManager.ConnectionStrings.[name].ConnectionString

type Get42RelativePath = SqlCommandProvider<"MySqlFolder/sampleCommand.sql", "name=AdventureWorks2012">
