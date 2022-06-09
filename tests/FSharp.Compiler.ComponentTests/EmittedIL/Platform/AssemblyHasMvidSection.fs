open System
open System.IO
open System.Reflection
open Testing

let pathToMe = Assembly.GetExecutingAssembly().Location

let stream = File.Open(pathToMe, FileMode.Open)
