#indent "off"

module PlayerRecords

open System
open FileHelper
open MathHelper

let SigmaFactor = ref 3.0

let GetSkill mu sigma = mu - ((!SigmaFactor)*sigma)		

let skillList = ref []
let skillTree = ref (LeafNode 0)

let SetLevelList li =
	skillList := li;
	skillTree := System.Double.MinValue :: li |> List.toArray |> CreateTree 0 

let SetLevel k1 k2 k3 = 
	let xs = linspace 1.0 49. 49 in 	 	 	
	let li = xs |> List.map (fun x -> (norminv (x/k3))*(k2) + k1 ) in
	SetLevelList li

let GetLevelFromSkill skill k1 k2 k3 = 
	(float (TreeFind !skillTree skill)) + 1.0

let GetLevel mu sigma k1 k2 k3 = 
	GetLevelFromSkill (GetSkill mu sigma) k1 k2 k3
		
	(*let a = (((GetSkill mu sigma) - k1) / k2) in
	let value = (normcdf a) * k3 in
	Math.Floor(value)*)
			
/// Player record
type PlayerRecord = { gamertag:string; player_id:int64; mu:float; sigma:float; games_played:int16; }
		with 
			member instance.TrueSkill = let skill = (GetSkill instance.mu instance.sigma)
				in Math.Floor(skill) 			
				
			member instance.Level k1 k2 k3 = 
				GetLevel instance.mu instance.sigma k1 k2 k3				
		end

/// Named PlayerRecord array type		
type NamedPlayerRecordArray = { mutable name:(string); mutable value:(PlayerRecord array) }
	with 
		member instance.LoadWith (fileName:string) f = 
			if not (instance.name = fileName ) then				
				let table = f fileName in
				instance.value <- table;
				instance.name <- fileName;
				table
			else
				instance.value			
	end
	
/// Get player table from specified file	
let GetPlayerTable fileName =
	CreateEnumerableCSVStream fileName |> Seq.map (fun s -> 
		{ new PlayerRecord with gamertag = s.[0]; 
						    and player_id = Convert.ToInt64(s.[1]); 
						    and mu = Convert.ToDouble(s.[2]); 
						    and sigma = Convert.ToDouble(s.[3]);
						    and games_played = Convert.ToInt16(s.[4]) } ) 							    						  

let LastPlayerTable = { new NamedPlayerRecordArray with name = ""; and value = [||] }

let LoadPlayerTable fileName =
	let f fileName = GetPlayerTable fileName |> Seq.toArray in
	LastPlayerTable.LoadWith fileName f

/// Enumerates leader board table from specified file						  
let EnumerateLeaderboard fileName = 
	  	CreateEnumerableCSVStream fileName |> Seq.map (fun s -> 
		{ new PlayerRecord with gamertag = System.String.Empty 
						    and player_id = Convert.ToInt64(s.[0]); 
						    and mu = Convert.ToDouble(s.[1]); 
						    and sigma = Convert.ToDouble(s.[2]);
						    and games_played = Convert.ToInt16(s.[3]) } ) 
						    

/// Cache of last leaderboard to load
let LastLeaderboard = { new NamedPlayerRecordArray with name = ""; and value = [||] }

/// Loads leaderboard
let LoadLeaderboard fileName =
	let f fileName = EnumerateLeaderboard fileName |> Seq.toArray in
	LastLeaderboard.LoadWith fileName f
