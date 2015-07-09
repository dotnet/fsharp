namespace Library2

open System
open System.Data

module M = 
    let dataset = new DataSet("test add reference")
    Console.WriteLine(dataset.DataSetName)
    Console.ReadKey(true)