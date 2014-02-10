module FSharp.Data.Experimental.TypeProviderTest

open System
open System.Data
open Xunit

[<Literal>]
let connectionString = @"Data Source=(LocalDb)\v11.0;Initial Catalog=AdventureWorks2012;Integrated Security=True"

type QueryWithTinyInt = SqlCommand<"SELECT CAST(10 AS TINYINT) AS Value", connectionString, ResultRows = ExpectedRows.ExactlyOne>

[<Fact>]
let TinyIntConversion() = 
    let cmd = QueryWithTinyInt()
    Assert.Equal(Some 10uy, cmd.Execute())    

type GetServerTime = SqlCommand<"IF @Bit = 1 SELECT 'TRUE' ELSE SELECT 'FALSE'", connectionString, ResultRows = ExpectedRows.ExactlyOne>

[<Fact>]
let SqlCommandClone() = 
    let cmd = new GetServerTime()
    Assert.Equal<string>("TRUE", cmd.Execute(Bit = 1))    
    let cmdClone = cmd.AsSqlCommand()
    cmdClone.Connection.Open()
    Assert.Throws<SqlClient.SqlException>(cmdClone.ExecuteScalar) |> ignore
    cmdClone.Parameters.["@Bit"].Value <- 1
    Assert.Equal(box "TRUE", cmdClone.ExecuteScalar())    
    Assert.Equal(cmdClone.ExecuteScalar(), cmd.Execute(Bit = 1))    
    Assert.Equal<string>("FALSE", cmd.Execute(Bit = 0))    
    Assert.Equal(box "TRUE", cmdClone.ExecuteScalar())    
    cmdClone.CommandText <- "SELECT 0"
    Assert.Equal<string>("TRUE", cmd.Execute(Bit = 1))    

type ConditionalQuery = SqlCommand<"IF @flag = 0 SELECT 1, 'monkey' ELSE SELECT 2, 'donkey'", connectionString, ResultRows = ExpectedRows.ExactlyOne>

[<Fact>]
let ConditionalQuery() = 
    let cmd = ConditionalQuery()
    Assert.Equal((1, "monkey"), cmd.Execute(flag = 0))    
    Assert.Equal((2, "donkey"), cmd.Execute(flag = 1))    

type ColumnsShouldNotBeNull2 = 
    SqlCommand<"SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = 'DatabaseLog' and numeric_precision is null
            ORDER BY ORDINAL_POSITION", connectionString, ResultRows = ExpectedRows.ExactlyOne>

[<Fact>]
let columnsShouldNotBeNull2() = 
    let cmd = new ColumnsShouldNotBeNull2()
    let _,_,_,_,precision = cmd.Execute()
    Assert.Equal(None, precision)    



