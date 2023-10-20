#indent "off"

module FileHelper

open System
open System.IO
open System.Windows.Forms

/// Gets subfolder names from specified path
let GetSubFolderNames path = 
	let dir = new DirectoryInfo(path) in
	dir.GetDirectories() |> Array.map ( fun (info:DirectoryInfo) -> info.Name )

/// Gets latest file in specified path matching specified search pattern
let GetLatestFile path searchPattern	=
	let dir = new DirectoryInfo(path) in
	let fileInfos = dir.GetFileSystemInfos(searchPattern) in
	if (fileInfos.Length>0 ) then
		let latest = Array.fold 
		( fun (acc:FileSystemInfo) (info:FileSystemInfo) -> if (info.CreationTime>acc.CreationTime) then info else acc  ) 
			fileInfos.[0] fileInfos in
		Some latest.Name
	else
		None
		
/// Create an enumerable stream from specified file
let CreateEnumerableStream (fileName:string) =
    let reader = new StreamReader (fileName) in
    reader |> Seq.unfold (fun (reader:StreamReader) -> if (reader.EndOfStream) then (reader.Close(); None) else Some(reader.ReadLine(),reader))

/// Create an enumerable csv stream from specified file 
let CreateEnumerableCSVStream fileName = 
	CreateEnumerableStream fileName |> Seq.map (fun (s:string) -> s.Split([|','|]))
			
			
	
