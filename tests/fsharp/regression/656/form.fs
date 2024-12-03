#indent "off"

// command line build:
// fsc --target-winexe -g --standalone misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs -o viewer.exe

open System
open System.IO
open System.Collections.Generic
open System.Windows.Forms
open System.Diagnostics
open System.Drawing
open System.Drawing.Drawing2D
open System.Threading
open System.Xml
open Misc
open MathHelper
open FileHelper
open FormsHelper
open PlayerRecords
open TrackedPlayers
open PlotTrajectory
open Plot
open PlayerRecords

// Initialise level calculation
do SetLevel !k1 !k2 k3
let pins = (Array.init (List.length !skillList) (fun _ -> true))

/// Base Path to search for results in
let basePath = ref @"c:\" 
let argv = System.Environment.GetCommandLineArgs()
do basePath := if (argv |> Array.length > 1) then argv.[1] else @"c:\"
	
let gamesPlayedStart = ref 0s
let gamesPlayedEnd = ref 10000s

let labelK1 = new Label()
let labelK2 = new Label()
	
// Create form	
let mainForm = CreateForm "Select Experiment (v1.05)"
let tip = new ToolTip()
do tip.ShowAlways <- true; tip.ReshowDelay <- 100; tip.AutoPopDelay <- 1500; tip.InitialDelay <- 100

let groupFolder = new GroupBox()
do groupFolder |> BlueFlash
do groupFolder.Text <- "Experiment folder"
do groupFolder.Dock <- DockStyle.Left
do groupFolder.Size <- new Size(mainForm.Size.Width/2,groupFolder.Size.Height)

// Create table layout panel for folders
let folderPanel = new TableLayoutPanel()
do folderPanel.ColumnCount <- 1
do folderPanel.RowCount <- 6
do folderPanel.Dock <- DockStyle.Fill
do groupFolder.Controls.Add(folderPanel)
	
let formDock = FormDockLocation.Create mainForm

/// ComboBox of folder names indicating selected variables
let combos = new List<ComboBox>()
let selectedMeasure = ref "Mu"
let lastFileLock = ref (Guid.NewGuid())
let lastFileName = ref ""
let mutable lastTable = []

/// Get measurement function
let GetMeasureFun measure = 
	match measure with | "TrueSkill" ->	(fun (player:PlayerRecord) -> player.TrueSkill) 
					   | "Level" ->	(fun (player:PlayerRecord) -> player.Level !k1 !k2 k3 ) 
				       | _ -> (fun (player:PlayerRecord) -> player.mu)

let mutable brush = new HatchBrush(HatchStyle.HorizontalBrick, Colour.Blue, Colour.Blue)

let playedEdges =(linspace 0. 500. 101) |>  List.map (fun v -> Math.Round(v,0) )
let playedCounts = Array.init ((List.length playedEdges) + 1) (fun _ -> 0) 	
let playedTree = System.Double.MinValue :: playedEdges |> List.toArray |> CreateTree 0

let ShowGamesPlayed() =
	let edges = System.Double.MinValue :: (List.append playedEdges [System.Double.MaxValue]) |> List.toArray in
	let playedPairs = List.zip (System.Double.MinValue :: playedEdges) (List.append playedEdges [System.Double.MaxValue]) |> List.toArray in				
	let funNo = ref 0 in
	let rangeFun() = 
		let vertRange = 
			match !funNo with 
				| 0 -> {new Range with min=0.0F; and max=max 1.0F (float32 (Array.fold max 0 playedCounts)); and name="Players" }
				| 1 -> {new Range with min=0.0F; and max=max 1.0F (float32 (Array.fold (+) 0 playedCounts)); and name="Cumulative Players" }
				| 2 -> {new Range with min=0.0F; and max=100.0F; and name="Cumulative Players %" }
				| 3 -> {new Range with min=0.0F; and max=100.0F; and name="Cumulative Players %" }
				| _ -> raise (new ApplicationException("error"))
				in
		let horizRange = {new Range with min=0.0F; and max=500.0F; and name="Played" } in 
		(vertRange, horizRange)
		in
	let drawFun gr t = 
		let playedCumulative, total = playedCounts |> Array.fold ( fun (li,cumulative) i -> let v = cumulative+i in (v :: li, v ) ) ([],0) in
		let playedCumulative = playedCumulative |> List.rev in
		match !funNo with
			| 0 -> 
				let ar = playedCounts |> Array.mapi (fun i a -> let x1,x2 = playedPairs.[i] in (float32 x1,float32 x2,a) ) in					
				Array.sub ar 1 (ar.Length-2) |> DrawBarGraph gr t brush 		
			| 1 ->
				let ar = playedCumulative |> List.toArray |> Array.mapi (fun i a -> let x1,x2 = playedPairs.[i] in (float32 x1,float32 x2,a) ) in					
				Array.sub ar 1 (ar.Length-2) |> DrawBarGraph gr t brush; 		
 
				let cumulative = playedCumulative |> List.mapi (fun i v -> (edges.[i+1], float v) ) |> List.toArray in	
				Array.sub cumulative 1 (cumulative.Length-2) |> DrawLineGraph gr t 				
			| 2 -> 
				let playedPc = playedCumulative |> List.map ( fun i -> if 0=i  then 0.0 else let y = float i in let t = float total in  
				((y*100.0)/t) ) in	
				let pc = playedPc |> List.mapi (fun i v -> (edges.[i+1], v) ) |> List.toArray in	
				Array.sub pc 1 (pc.Length-2) |> DrawLineGraph gr t	
			| 3 ->
				let m = max 1.0F (float32 (Array.fold max 0 playedCounts)) in
				let ar = playedCounts |> Array.mapi (fun i a -> let x1,x2 = playedPairs.[i] in (float32 x1,float32 x2, int (float32 a * 100.F/m)) ) in					
				Array.sub ar 1 (ar.Length-2) |> DrawBarGraph gr t brush;

				let playedPc = playedCumulative |> List.map ( fun i -> if 0=i  then 0.0 else let y = float i in let t = float total in  
				((y*100.0)/t) ) in	
				let pc = playedPc |> List.mapi (fun i v -> (edges.[i+1], v) ) |> List.toArray in	
				Array.sub pc 1 (pc.Length-2) |> DrawLineGraph gr t									 
				
			| _ -> raise (new ApplicationException("error"))
		in		
	let form = CreateForm "Games played" in
	form.Width <- 1000; form.Height <- 800;	
	let picture = AddGraph form rangeFun drawFun false in

	let menuFunction = form.Menu.MenuItems.Add("&Function") in 
	let miHist = new MenuItem("Histogram") in
	let miCumulative = new MenuItem("Cumulative") in
	let miPc = new MenuItem("Cumulative %")in
	let miPcHist = new MenuItem("Cumulative % (+ Histogram)")in

	menuFunction.MenuItems.AddRange([|miHist;miCumulative;miPc;miPcHist|]) |> ignore;			
	miHist.Click.Add( fun _ -> funNo := 0; form.Invalidate(true) );
	miCumulative.Click.Add( fun _ -> funNo := 1; form.Invalidate(true) ); 
	miPc.Click.Add( fun _ -> funNo := 2; form.Invalidate(true) ); 
	miPcHist.Click.Add( fun _ -> funNo := 3; form.Invalidate(true) ); 
	form.Menu.MenuItems.Add(menuFunction) |> ignore;

	let _ = form.Show() in
	form


/// Display sample histogram on specified form
let DisplaySample (form:Form) (measure, fileName) =	
	let skillEdges = (linspace -25. 75. 101) |> List.map (fun v -> Math.Round(v,2) ) in	  
	let skillCounts = Array.init ((List.length skillEdges) + 1) (fun _ -> 0) in		
		
	let dataThread = new Thread( fun() ->			
		let f = GetMeasureFun measure in
		let skillTree = System.Double.MinValue :: skillEdges |> List.toArray |> CreateTree 0 in
			form.Invoke( new MethodInvoker( fun _ -> tip.SetToolTip(labelK1, "Mean: calculating" ) )) |> ignore;
			form.Invoke( new MethodInvoker( fun _ -> tip.SetToolTip(labelK2, "Standard Deviation: calculating" ) )) |>ignore;											
			let ShowStatistics count total skills = 					
				let (mean:float) = (total/(float) count ) in
				let _ = try if not form.IsDisposed then form.Invoke( new MethodInvoker( 
					fun _ -> try tip.SetToolTip(labelK1, "Mean: " + mean.ToString() ) with e -> LogWarning e
					)) |> ignore with e -> LogWarning e
				in
				let deltaSquared = skills |> List.fold ( fun acc value -> let delta = (mean-value) in acc + (delta*delta) ) 0.0 in
				let sd = Math.Sqrt(deltaSquared / (float count)) in								
				try if not form.IsDisposed then form.Invoke( new MethodInvoker( 
					fun _ -> try tip.SetToolTip(labelK2, "Standard Deviation: " + sd.ToString() ) with e -> LogWarning e
					)) |>ignore  with e -> LogWarning e |> ignore
			in
			lock lastFileLock ( fun _ ->
			if not (fileName = (!lastFileName)) then 
			(					
				Array.fill playedCounts 0 (Array.length playedCounts) 0;		
				let (table, skills, total, count) = EnumerateLeaderboard fileName |>
					Seq.fold (fun (table, skills, total, count) (p:PlayerRecord) -> 			
						addone playedTree playedCounts (Convert.ToDouble(p.games_played));		
						if (p.games_played>=(!gamesPlayedStart) && p.games_played<(!gamesPlayedEnd)) then							
							let v = f(p) in 
								addone skillTree skillCounts v;	// update histogram
						    let skill = GetSkill p.mu  p.sigma in
							(p :: table, skill :: skills, total + skill, count + 1)
						else						
							(p :: table, skills, total, count) ) ([], [], 0., 0) in			
				lastFileName := fileName; lastTable <- table;				
				ShowStatistics count total skills
			)
			else
			(			
				let skills, total, count = lastTable |> List.fold ( fun (skills,total,count) (p:PlayerRecord) ->
					if (p.games_played>=(!gamesPlayedStart) && p.games_played<(!gamesPlayedEnd)) then
						let v = f(p) in addone skillTree skillCounts v;	// update histogram											
						let skill = GetSkill p.mu p.sigma in
						(skill :: skills, total + skill, count + 1)
					else
						(skills, total, count) ) ([], 0., 0)								
				in
				ShowStatistics count total skills
			)
		)
	) in dataThread.Start();	
	
	let finished = ref false in
	let drawThread = new Thread( fun () -> 
		let f() = try if not (form.IsDisposed) then form.Invalidate(true) with e -> LogWarning e in
		try 
			brush <- new HatchBrush(HatchStyle.DiagonalCross, Colour.AliceBlue, Colour.Blue);			
			while( not form.IsDisposed && not !finished ) do 
				finished := dataThread.Join(100);				
				let _ = if !finished then brush <- new HatchBrush(HatchStyle.HorizontalBrick, Colour.Blue, Color.Blue) in
				if not (form.IsDisposed) then form.Invoke( new MethodInvoker(f) ) |> ignore
				done; 
		with e -> LogWarning e
	) in drawThread.Start();
	
		
		
	let funNo = ref 0 in		
												
	let edgesArray = skillEdges |> List.map float32 |> List.toArray in																							
	/// Show bar graph
	let hlabel = measure in let vlabel = "Players" in
	let rangeFun() = 
		let first = System.Array.FindIndex(skillCounts, ( fun (v:int) -> v>0 ) ) in
		let last = System.Array.FindLastIndex(skillCounts, ( fun(v:int) -> v>0 ) ) in					
		let horizRange = {new Range with min=if -1 = first then 0.F else min 0.F edgesArray.[first]; 
			and max=if -1 = last then 1.F else edgesArray.[last]; and name=hlabel }  in
		let vertRange = match !funNo with
			| 0 -> {new Range with min=0.0F; and max=max 1.0F (float32 (skillCounts |> Array.fold max 1)); and name=vlabel }
			| 1 -> {new Range with min=0.0F; and max=max 1.0F (float32 (skillCounts |> Array.fold (+) 0)); and name=vlabel }
			| 2 -> {new Range with min=0.0F; and max=100.0F; and name="Players %" }
			| _ -> raise (new ApplicationException("error"))
		in
		(vertRange, horizRange) 
	in
	let drawFun gr t = 
		let _, hRange = rangeFun() in
		let first, last = max 1 (System.Array.FindIndex(edgesArray, (fun(v) -> hRange.min = v ) )), 	
			System.Array.FindIndex(edgesArray,(fun(v) -> hRange.max = v) )in			
		let copy = Array.init (last - first) (fun _ -> 0 ) in 
		System.Array.ConstrainedCopy(skillCounts,first,copy,0,last-first);
		
		let ar = match !funNo with
			| 0 -> copy
			| 1 -> let li, t = copy |> Array.fold( fun (li,t) i -> let x = t + i in (x :: li, x)  ) ([],0) in li |> List.rev |> List.toArray
			| 2 -> let li, t = copy |> Array.fold( fun (li,t) i -> let x = t + i in (x :: li, x)  ) ([],0) in 
				li |> List.map ( fun v -> if v = 0 then 0 else (v*100)/t )	// TODO: fix rounding issue
				|> List.rev |> List.toArray
			| _ -> raise (new ApplicationException("error"))
		in
		ar |> Array.mapi (fun i a -> (edgesArray.[i+first-1], edgesArray.[i+first],a) ) |>
		DrawBarGraph gr t brush in
	let picture = AddGraph form rangeFun drawFun false in
	let menuFunction = form.Menu.MenuItems.Add("&Function") in 
	let miHist = new MenuItem("Histogram") in
	let miCumulative = new MenuItem("Cumulative") in
	let miPc = new MenuItem("Cumulative %") in
	menuFunction.MenuItems.AddRange([|miHist;miCumulative;miPc|]) |> ignore;			
	miHist.Click.Add( fun _ -> funNo := 0; form.Invalidate(true) );
	miCumulative.Click.Add( fun _ -> funNo := 1; form.Invalidate(true) ); 
	miPc.Click.Add( fun _ -> funNo := 2; form.Invalidate(true) ); 
	form.Menu.MenuItems.Add(menuFunction) |> ignore


  	   																				
/// Show leaderboard histogram
let LeaderBoardHandler form measure fileName =	
	let ShowSample sample =  			
		let f() = try DisplaySample form sample with e -> LogWarning e in
		if not form.IsDisposed then try form.Invoke( new MethodInvoker(f) ) |> ignore with e -> LogWarning e else ()
	in	
	//AsyncApp (measure,fileName) LoadSample ShowSample;
	DisplaySample form (measure,fileName);
	form
	
/// Handler change to input parameters
let ChangeHandler measure path =
	// Check for tracked players file
	let trackFile = GetLatestFile path "Track-*.csv" in
	let _ = match trackFile with 
		| Some name -> 	let title = "Select Tracked Players (" + path + ")" in
						let form = match formDock.GetChildForm "track" with
								   | Some child -> child.Text <- title; child
								   | None -> let child = CreateForm title in
											 child.Width <- 1000;										
											 let _ = child.Show() in 	
											 formDock.SetChild child "track" 0 mainForm.Height; // Align to bottom left
											 child
						in																					   					  
						let _ = TrackedPlayersHandler form formDock measure (!gamesPlayedStart,!gamesPlayedEnd) (Path.Combine(path,name)) in					   
						()					    
		| None -> ()
	in
	let width = 1000 in
	// Check for leader board of players file
	let leadFile = GetLatestFile path "Lead-*.csv" in
	let _ = match leadFile with 
		| Some name ->	let title =  "Bar graph (" + path + ")" in
						let form = match formDock.GetChildForm "lead" with
								  | Some child -> child.Text<-title;child.Controls.Clear();child.Menu.MenuItems.Clear();child
								  | None -> let child = CreateForm title in child.Show();
											child.Height <- mainForm.Height;
   											child.Width <- width - mainForm.Width;
											formDock.SetChild child "lead" mainForm.Width 0;	// Align to top right
											child
					   in		      			   					   	
					   let _ = LeaderBoardHandler form measure (Path.Combine(path,name)) in  					   				   					     					   
					   ()
		| None -> ()
	in ()		
	
/// Get relative path from combos
let GetRelativePath (combos:List<ComboBox>) =
	let folders = combos.ToArray() |> Array.map ( fun (combo:ComboBox) -> if combo.SelectedItem <> null then combo.SelectedItem.ToString() else "" ) in
	System.String.Join(Path.DirectorySeparatorChar.ToString(), folders)
	
/// Get selected path from combo boxes	
let GetSelectedPath (combos:List<ComboBox>) = 
	let relPath = GetRelativePath combos in
	Path.Combine(!basePath,relPath)

/// ComboBox select event handler
let rec ComboSelectHandler (sender:obj) (e:EventArgs) =
	let index = combos.FindIndex((fun combo -> combo.Equals(sender) ) ) in	
	let relevant = combos.GetRange(0, (index+1)) in 
	let path = GetSelectedPath relevant in
	let folders = GetSubFolderNames path in
	let _ = if (folders.Length > 0 ) then 
		// Remove any combos after this one
		let _ = combos.ToArray() |> Array.iteri (fun i combo -> 
			if( i>index ) then let _ = combos.Remove(combo) in folderPanel.Controls.Remove(combo); ()  ) in
		// Create the new combo
		let combo = CreateCombo folders in
		let _ = combos.Add(combo) in		
		let	_ = combo.SelectionChangeCommitted.AddHandler( new EventHandler(ComboSelectHandler) )  in
		folderPanel.Controls.Add(combo)
	in
	ChangeHandler !selectedMeasure path				
			
/// Sets combos from specified path
let SetCombos (relPath:string) =
	// Remove current combos
	combos.ToArray() |> Array.iter (fun combo -> combos.Remove(combo) |> ignore; folderPanel.Controls.Remove(combo) );	
	// Set combos
	let path = relPath.Split([|Path.DirectorySeparatorChar|]) |> Array.fold ( fun (acc:string) folder ->
		let absPath = Path.Combine( !basePath, acc) in
		let folders = GetSubFolderNames absPath in
		let combo = CreateCombo folders in
		combo.SelectedItem <- folder;
		let _ = combos.Add(combo) in		
		let	_ = combo.SelectionChangeCommitted.AddHandler( new EventHandler(ComboSelectHandler) )  in
		folderPanel.Controls.Add(combo) |> ignore; 
		Path.Combine(acc,folder)
	) "" in	
	ChangeHandler !selectedMeasure (Path.Combine(!basePath,path))
			
			
// Create first ComboBox	
do let folders = GetSubFolderNames !basePath in
	let combo = CreateCombo folders in
	let _ = combos.Add(combo) in
	combo.SelectionChangeCommitted.AddHandler( new EventHandler(ComboSelectHandler) );
	folderPanel.Controls.Add( combo )

// Group box for measurement types
let groupMeasure = new GroupBox()
do groupMeasure |> BlueFlash
do groupMeasure.Dock <- DockStyle.Top
do groupMeasure.Text <- "Measure"

let tableMeasure = new TableLayoutPanel() 
do tableMeasure.ColumnCount <- 3 
do tableMeasure.Dock <- DockStyle.Fill
do tableMeasure |> AddToControl groupMeasure
	
do	let f = (fun (sender:obj) e -> let radio = sender :?> RadioButton in 
		if radio.Checked then let path = GetSelectedPath combos in 
			selectedMeasure := radio.Text;
			ChangeHandler radio.Text path	
	) in	
	let r1 = CreateRadio "Mu" f in r1 |> BlueFlash;
	let r2 = CreateRadio "TrueSkill" f in r2 |> BlueFlash; r2.Checked <- true;
	let r3 = CreateRadio "Level" f in r3 |> BlueFlash;
	[|r1;r2;r3|] |> AddArrayToControl tableMeasure
			
do	tableMeasure |> AddToControl groupMeasure	
do groupMeasure.Size <- new Size(groupMeasure.Size.Width, groupMeasure.PreferredSize.Height + tableMeasure.PreferredSize.Height)

// Create right panel	
let rightPanel = new Panel()
do rightPanel.Dock <- DockStyle.Fill


do mainForm.Controls.Add(rightPanel)
do mainForm.Controls.Add(groupFolder)	
do groupMeasure |> AddToControl mainForm

// --- Range Group
let rangeGroup = new GroupBox()
do rangeGroup |> BlueFlash
do rangeGroup.FlatStyle <- FlatStyle.Popup
do rangeGroup.Height <- rightPanel.Height/2
do rangeGroup.Text <- "Games played range"
do rangeGroup.Click.Add( fun _ -> ShowGamesPlayed() |> ignore ) 
do rangeGroup.Dock <- DockStyle.Fill //Bottom
do rangeGroup |> AddToControl rightPanel

let rangeTable = new TableLayoutPanel()
do rangeTable.ColumnStyles.Add( new ColumnStyle(SizeType.Percent,50.0f) ) |> ignore
do rangeTable.ColumnStyles.Add( new ColumnStyle(SizeType.Percent,50.0f) ) |> ignore
do rangeTable.Dock <- DockStyle.Fill
do rangeTable.ColumnCount <- 2
let labelStart = new Label()
do labelStart.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Right
do labelStart.Text <- "Start"
do labelStart |> BlueFlash
let editStart = new NumericUpDown()
do editStart.Maximum <- Convert.ToDecimal(100000)
do editStart.Value <- Convert.ToDecimal(!gamesPlayedStart)
do editStart.Increment <- Convert.ToDecimal(10)
do editStart.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Left
do editStart.Width <- 64
do editStart.TextChanged.Add( fun _ -> gamesPlayedStart := if editStart.Value<=Convert.ToDecimal(Int16.MaxValue) then Decimal.ToInt16(editStart.Value) else Int16.MaxValue )
let labelEnd = new Label()
do labelEnd.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Right
do labelEnd.Text <- "End"
do labelEnd |> BlueFlash
let editEnd = new NumericUpDown()
do editEnd.Maximum <- Convert.ToDecimal(100000)
do editEnd.Value <- Convert.ToDecimal(!gamesPlayedEnd)
do editEnd.Increment <- Convert.ToDecimal(10)
do editEnd.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Left
do editEnd.Width <- 64
do editEnd.TextChanged.Add( fun _ -> gamesPlayedEnd := if editEnd.Value<=Convert.ToDecimal(Int16.MaxValue) then Decimal.ToInt16(editEnd.Value) else Int16.MaxValue )


do rangeTable.Controls.Add(labelStart)
do rangeTable.Controls.Add(editStart)
do rangeTable.Controls.Add(labelEnd)
do rangeTable.Controls.Add(editEnd)

do rangeGroup.Controls.Add(rangeTable)

let rangeUpdateButton = new Button();
do rangeUpdateButton.Text <- "Update"
do rangeUpdateButton.Dock <- DockStyle.Bottom
do rangeUpdateButton |> BlueFlash
do rangeGroup.Controls.Add(rangeUpdateButton)

do rangeUpdateButton.Click.Add( fun _ -> GetSelectedPath combos |> ChangeHandler !selectedMeasure)
// ---

// --- Level Group -----
let levelGroup = new GroupBox()
do levelGroup |> BlueFlash
do levelGroup.Height <- (rightPanel.Height/2)+4
do levelGroup.Text <- "Level parameters"
do levelGroup.Dock <- DockStyle.Top
do levelGroup |> AddToControl rightPanel


let levelTable = new TableLayoutPanel()
do levelTable.ColumnStyles.Add( new ColumnStyle(SizeType.Percent,50.0f) ) |> ignore
do levelTable.ColumnStyles.Add( new ColumnStyle(SizeType.Percent,50.0f) ) |> ignore
do levelTable.Dock <- DockStyle.Fill
do levelTable.ColumnCount <- 2
do labelK1.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Right
do labelK1.Text <- "K1"
do labelK1 |> BlueFlash
do tip.SetToolTip(labelK1, "Experiment's mean (of mu-n*sigma) will appear on this tip" ) 
let editK1 = CreateNumericBox true
do editK1.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Left
do editK1.Width <- 64
do editK1.Text <- Convert.ToString(!k1)
do editK1.TextChanged.Add( fun _ -> let v = ref 0.0 in if Double.TryParse(editK1.Text, v) then k1 := !v) 
do labelK2.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Right
do labelK2.Text <- "K2"
do labelK2 |> BlueFlash
do tip.SetToolTip(labelK2, "Experiment's standard deviation (from mu-n*sigma) will appear on this tip" ) 
let editK2 = CreateNumericBox false
do editK2.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Left
do editK2.Width <- 64
do editK2.Text <- Convert.ToString(!k2)
do editK2.TextChanged.Add( fun _ -> let v = ref 0.0 in if Double.TryParse(editK2.Text, v) then k2 := !v)

let labelSigma = CreateLabel "Sigma"
do labelSigma.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Right
do labelSigma |> BlueFlash
do tip.SetToolTip(labelSigma, "Experiment's sigma factor used to calculate TrueSkill and Level as mu - n*sigma, default value is 3." ) 
let editSigma = CreateNumericBox true
do editSigma.Anchor <- AnchorStyles.Bottom ||| AnchorStyles.Left
do editSigma.Width <- 64
do editSigma.Text <- Convert.ToString(!SigmaFactor)
do editSigma.TextChanged.Add( fun _ -> let v = ref 0.0 in if Double.TryParse(editSigma.Text, v) then SigmaFactor := !v)

let labelMap = new Label()
do labelMap.Text <- "Skill map"

let buttonEditor = new Button();
do buttonEditor.Text <- "Edit"

do levelTable.Controls.Add(labelK1)
//do levelTable.Controls.Add(editK1)
do levelTable.Controls.Add(labelK2)
//do levelTable.Controls.Add(editK2)
do levelTable.Controls.Add( labelMap )
do levelTable.Controls.Add( buttonEditor )
do levelTable.Controls.Add(labelSigma)
do levelTable.Controls.Add(editSigma)

do levelGroup.Controls.Add(levelTable)

let levelUpdateButton = new Button();
do levelUpdateButton.Text <- "Update"
do levelUpdateButton.Dock <- DockStyle.Bottom
do levelUpdateButton |> BlueFlash
do levelGroup.Controls.Add(levelUpdateButton)

do levelUpdateButton.Click.Add( fun _ -> GetSelectedPath combos |> ChangeHandler !selectedMeasure)

// Test code
let showInverse() =
	let xs = linspace -10. 70. 81 in
	let li = xs |> List.map (fun x -> (x, GetLevelFromSkill x !k1 !k2 k3) ) in
	let _ = ShowLineGraph "Level" "TrueSkill" (List.toArray li) in ()
	
do levelGroup.Click.Add( fun _ -> showInverse() )

let resetItem points (pinned: _ []) item = 
	let mutable low = if item = 0 then 0 else item-1 in
	while not pinned.[low] do low <- low - 1 done;
	let top = (Array.length points)-1 in
	let mutable high = if item = top then top else item+1 in
	while not pinned.[high] do high <- high + 1 done;		
		
	for i = (low+1) to high-1 do 
		let sx, sy = points.[low] in let ex, ey = points.[high] in
		let dy = ey - sy in let dx = ex - sx in
		let	cx, cy = points.[i] in					
		points.[i] <- (cx, sy + ((dy*(cx-sx))/dx))
		done;

// Skill mapping editor
do buttonEditor.Click.Add( fun _ ->
	let lt = ref {new Transformation2d with origin = (new PointF(0.F,0.F)); and scale = (new SizeF(0.F,0.F)) } in
	let rects = ref [||] in

 	let xs = linspace 1.0 49. 49 in
 	//xs |> List.iter ( fun x -> Debug.WriteLine(x) );
 	let on = Array.init (List.length xs) (fun i -> true) in
 	pins |> Array.iteri (fun i x -> on.[i] <- x );
 	
 	let skillArray = !skillList |> List.toArray in
	let data = xs |> List.mapi (fun i x -> (x,  skillArray.[i]) //(norminv (x/k3))*(!k2) + !k1 ) 
		) |> List.toArray in	
	let original = Array.copy data in
		
	
	let GetCrosshairs (points:PointF array) =
		let width = 4.0F in
		points |> Array.map ( fun (point:PointF) ->			 
		new RectangleF( point.X - width, point.Y - width, (2.0F*width)+1.0F, (2.0F*width)+1.0F) ) 		
	in
	
	let DrawLineGraph (gr:Graphics) transform (data:(float * float) array) = 		
		lt := transform;
		
		// Draw original line
		let op = original |> Array.map ( fun (x,y) -> CreatePoint transform (float32 x, float32 y) ) in			
		gr.DrawLines( new Pen(Colour.Green), op);
		
		//let opabove = original |> Array.map ( fun (x,y) -> CreatePoint transform (float32 x, float32 (y + 1.0)) ) in			
		//gr.DrawLines( new Pen(Colour.Honeydew), opabove);

		//let opbelow = original |> Array.map ( fun (x,y) -> CreatePoint transform (float32 x, float32 (y - 1.0)) ) in			
		//gr.DrawLines( new Pen(Colour.Honeydew), opbelow);
												
		let points = data |> Array.map ( fun (x,y) -> CreatePoint transform (float32 x, float32 y) ) in
		rects := GetCrosshairs points; 		
				
		// Draw red line
		let pen = new Pen(Colour.Red) in
		pen.Width <- 2.0F;
		let linePoints = on |> Array.mapi( fun i b -> (b, points.[i]) ) |> Array.filter( fun (b,pt) -> b )
			|> Array.map( fun (b,pt) -> pt ) in		
		gr.DrawLines(pen, linePoints);
		
		// Draw red rectangles
		let red = !rects |>  Array.mapi( fun i x -> (on.[i], x) ) |> Array.filter( fun (b,x) -> b )
			|> Array.map( fun (b,x) -> x ) in
		
		gr.DrawRectangles(pen, red);
		
		// Draw blue (disabled) rectangles
		let blue = !rects |>  Array.mapi( fun i x -> (on.[i], x) ) |> Array.filter( fun (b,x) -> not b )
			|> Array.map( fun (b,x) -> x ) in

		if Array.length blue > 0 then gr.DrawRectangles(new Pen(Colour.Blue), blue)
						
	in
		
	let drawFun gr transform = data |> DrawLineGraph gr transform in
	
	let setItem item ay = 
		let mutable low = if item = 0 then 0 else item-1 in
		while not on.[low] do low <- low - 1 done;
		let top = (Array.length data)-1 in
		let mutable high = if item = top then top else item+1 in
		while not on.[high] do high <- high + 1 done;		
		let hello = 0.1134 in
		let _,miny = if item>0 then data.[low] else (hello, Double.NegativeInfinity) in
		let _,maxy = if item<top then data.[high] else (hello, Double.PositiveInfinity) in
		let ky = float ay in
		let ry = min (max miny ky) maxy in						
		let vx, vy = data.[item] in	
		data.[item] <- (vx, ry);
		// Recalculate points
		for i = (low+1) to item do 
			let sx, sy = data.[low] in let ex, ey = data.[item] in
			let dy = ey - sy in let dx = ex - sx in
			let cx, cy = data.[i] in
			data.[i] <- (cx, sy + ((dy*(cx-sx))/dx))
			done;					
		for i = item+1 to (high-1) do
			let sx, sy = data.[item] in let ex, ey = data.[high] in
			let dy = ey - sy in let dx = ex - sx in
			let cx, cy = data.[i] in
			data.[i] <- (cx, sy + ((dy*(cx-sx))/dx))			
			done;									
		()
		in 
		
	// Create form	
	let rangeFun() =
		let xl, xh, yl, yh = data |> Array.fold ( fun acc (x,y) -> 
		let xl,xh,yl,yh = acc in (min x xl, max x xh, min y yl, max y yh) 
		) (Double.MaxValue,Double.MinValue,Double.MaxValue,Double.MinValue) in
		let yl,yh = if yl=yh then yl - 1.,yh + 1. else yl,yh in
		let horizRange = {new Range with min=min 0.0F (float32 xl); and max=float32 xh; and name="Level" } in	
		let vertRange = { new Range with min=min 0.0F (float32 (yl - 2.0)); and max=float32 (yh + 2.0); and name="TrueSkill" } in	
		(vertRange, horizRange) in
	let form = CreateForm "Skill mapping editor" in
	form.Width <- 1000; form.Height <- 800;	
	let panel, picture = AddGraph form rangeFun drawFun true in
			
		
	let buttonShow = new Button() in buttonShow.Text <- "Show";		
	let buttonUse = new Button() in buttonUse.Text <- "Use";
	let buttonClearAll = new Button() in buttonClearAll.Text <- "Clear All";
	let buttonSetAll = new Button() in buttonSetAll.Text <- "Set All";
	//let buttonInverse = new Button() in buttonInverse.Text <- "Show Inverse";
	let buttonOK = new Button() in buttonOK.Text <- "OK(Commit)"; buttonOK.DialogResult <- DialogResult.OK;
	let buttonCancel = new Button() in buttonCancel.Text <- "Cancel(Exit)"; buttonCancel.DialogResult <- DialogResult.Cancel;

	buttonClearAll.Click.Add( fun _ -> 
		for i=1 to (Array.length on)-2 do on.[i] <- false done;
		resetItem data on ((Array.length on)/2);
		panel.Invalidate() 
	) ;
	
	buttonSetAll.Click.Add( fun _ -> 
		for i=1 to (Array.length on)-2 do on.[i] <- true done; 
		panel.Invalidate() 		
	);	
	
	buttonShow.Click.Add( fun _ ->
		// replot original
		let ar = xs |> List.map (fun x -> (x, (norminv (x/k3))*(!k2) + !k1 ) ) |> List.toArray in
		ar |> Array.iteri (fun i v -> original.[i] <- v );
		panel.Invalidate()
	);
	
	buttonUse.Click.Add( fun _ -> 		
		let ar = xs |> List.map (fun x -> (x, (norminv (x/k3))*(!k2) + !k1 ) ) |> List.toArray in
		ar |> Array.iteri (fun i v -> original.[i] <- v; data.[i] <-v );
		for i=1 to (Array.length on)-2 do on.[i] <- true done; 
		panel.Invalidate()
	); 
	
	//buttonInverse.Click.Add( fun _ -> showInverse() ); // Note: doesn't work until values have been committed!
	
	let labelK1a = new Label() in
	let labelK2a = new Label() in

	labelK1a.Text <- "K1";
	labelK1a.Width <- 32;
	labelK1a |> BlueFlash;		
	let editK1 = CreateNumericBox true in
	editK1.Width <- 64;
	editK1.Text <- Convert.ToString(!k1);
	editK1.TextChanged.Add( fun _ -> let v = ref 0.0 in if Double.TryParse(editK1.Text, v) then k1 := !v);
	labelK2a.Text <- "K2";
	labelK2a.Width <- 32;
	labelK2a |> BlueFlash;	
	let editK2 = CreateNumericBox false in
	editK2.Width <- 64;
	editK2.Text <- Convert.ToString(!k2);
	editK2.TextChanged.Add( fun _ -> let v = ref 0.0 in if Double.TryParse(editK2.Text, v) then k2 := !v);
	
	let panel2 = new TableLayoutPanel() in 
	panel2.ColumnCount <- 12;
	panel2.Controls.Add(labelK1a);
	panel2.Controls.Add(editK1);
	panel2.Controls.Add(labelK2a);
	panel2.Controls.Add(editK2);
		
	panel2.Controls.Add(buttonShow);
	panel2.Controls.Add(buttonUse);
	panel2.Controls.Add(buttonClearAll);
	panel2.Controls.Add(buttonSetAll);
	//panel2.Controls.Add(buttonInverse);
	panel2.Controls.Add( new Label() );
	panel2.Controls.Add( new Label() );
	panel2.Controls.Add(buttonOK);
	panel2.Controls.Add(buttonCancel);
		
	panel2.Dock <- DockStyle.Bottom;
	panel2.Height <- 32;
	form.Controls.Add(panel2);
		
	//form.Load.Add( fun _ -> SendKeys.Send("{F6}") ); 		
		
	let GetRectNo x y = System.Array.FindIndex(!rects, (fun rect -> rect.Contains(x,y) ))
		in
	
	// left mouse click - toggle box - right mouse click drag
	let dragItem = ref (-1) in
    let oldMouseX = ref 0 in
    let oldMouseY = ref 0 in                         
   	picture.MouseDown.Add (fun me -> 
   		oldMouseX := me.X; oldMouseY := me.Y;
		let x,y = float32 me.X, float32 me.Y in
		let index = GetRectNo x y in
		if index <> -1 then if on.[index] then dragItem := index 
			else dragItem := -1
   	);	
   	
   	picture.MouseUp.Add(fun me -> dragItem := -1; panel.Invalidate() );
   						
	picture.MouseMove.Add (fun me ->
		if !dragItem <> -1 then
			let x,y = float32 me.X, float32 me.Y in
			let ax,ay = (!lt).InverseTransform(x,y)
			in
													
			setItem !dragItem ay
	);	
	
	picture.MouseClick.Add( fun e -> 	
			let x,y = float32 e.X, float32 e.Y in
			
			let item = GetRectNo x y in
			let _ = if item <> -1 && item>0 && item<((Array.length !rects)-1) then on.[item] <- (if on.[item] then false else true)
			in ()			
			;
			if item <> -1 then		
				// Recalc points								
				resetItem data on item;
				panel.Invalidate()			
		);	
						
	let result = form.ShowDialog() in
	if result = DialogResult.OK then 
		(
		Debug.WriteLine( "OK" );
		let _ = on |> Array.iteri ( fun i x -> pins.[i] <- x ) in
		let ar = data |> Array.map (fun (x,y) -> y ) in				
		let li = ar |> Array.toList in 
		let _ = SetLevelList li in
		let _ = GetSelectedPath combos |> ChangeHandler !selectedMeasure in
		() 
		)
	)
// ---

// -- Menu Code
let memuMain = mainForm.Menu <- new MainMenu()
let menuFile= mainForm.Menu.MenuItems.Add("&File")
let miFileOpen = new MenuItem("&Open")
let miFileExit = new MenuItem("&Exit")
do menuFile.MenuItems.Add(miFileOpen) |> ignore
do menuFile.MenuItems.Add(miFileExit) |> ignore
do miFileOpen.Click.Add( fun _ ->
	let browser = new FolderBrowserDialog() in
	browser.SelectedPath <- !basePath;
	if DialogResult.OK = browser.ShowDialog() then
		basePath := browser.SelectedPath;
		SetCombos "" )
do miFileExit.Click.Add( fun _ -> mainForm.Close() )		

let menuEdit = mainForm.Menu.MenuItems.Add("&Edit")
let miEditCopy = new MenuItem("&Copy") 
do miEditCopy.Shortcut <- Shortcut.CtrlC; miEditCopy.ShowShortcut <- true
let miEditPaste = new MenuItem("&Paste")
do miEditPaste.Shortcut <- Shortcut.CtrlV; miEditPaste.ShowShortcut <- true
do menuEdit.MenuItems.Add(miEditCopy) |> ignore
do menuEdit.MenuItems.Add(miEditPaste) |> ignore
do miEditCopy.Click.Add( fun _ ->
			let levels = (!skillList) |> List.mapi ( fun i x -> 
				if pins.[i] then ("\t\t<level index=\"" + (i+1).ToString() + "\">" + x.ToString() + "</level>\r\n") else "") in
			let value = "<view>\r\n" + 
				"\t<folder>" + (GetRelativePath combos) + "</folder>\r\n" +
				"\t<k1>" + (!k1).ToString() + "</k1>\r\n" +
				"\t<k2>" + (!k2).ToString() + "</k2>\r\n" +
				"\t<sigma>" + (!SigmaFactor).ToString() + "</sigma>\r\n" +
				"\t<playedStart>" + (!gamesPlayedStart).ToString() + "</playedStart>\r\n" +
				"\t<playedEnd>" + (!gamesPlayedEnd).ToString() + "</playedEnd>\r\n" +
				"\t<levels>\r\n" + System.String.Join("",levels |> List.toArray) + "\t</levels>\r\n" +
				"</view>\r\n" 
			in Clipboard.SetDataObject(value)	// Copy xml to clip board
		)
do miEditPaste.Click.Add( fun _ ->
		// Paste xml from clipboard
		let value = Clipboard.GetDataObject() in
		match value.GetData(typeof<string>) with
			| :? string as xml ->
				let _ = try 
					
					Debug.WriteLine(xml);			
					let doc = new XmlDocument() in
					doc.LoadXml( xml );
					let root = doc.FirstChild in
					let relPath = (root.Item("folder")).InnerText in								
					let k1Text = (root.Item("k1")).InnerText in
					k1 := Double.Parse(k1Text);
					editK1.Text <- Convert.ToString(!k1);
					let k2Text = (root.Item("k2")).InnerText in
					k2 := Double.Parse(k2Text);					
					editK2.Text <- Convert.ToString(!k2);
					let sigmaNode = root.Item("sigma") in
					let _ = if( sigmaNode<>null ) then SigmaFactor := Double.Parse(sigmaNode.InnerText) in
					editSigma.Text <- Convert.ToString(!SigmaFactor);
					let playedStart = root.Item("playedStart") in
					let _ = if playedStart <> null then gamesPlayedStart := XmlConvert.ToInt16(playedStart.InnerText); editStart.Value <- Convert.ToDecimal(!gamesPlayedStart); editStart.Invalidate() in
					let playedEnd = root.Item("playedEnd") in
					let _ = if playedEnd <> null then gamesPlayedEnd := XmlConvert.ToInt16(playedEnd.InnerText); editEnd.Value <- Convert.ToDecimal(!gamesPlayedEnd); editEnd.Invalidate() in					
					SetCombos relPath;													
					// Read levels
					let levelsNode = root.Item("levels") in
					let _ = if(levelsNode<>null ) then 					
						let xs = linspace 1.0 49. 49 in 	 							
						let data = xs |> List.mapi (fun i x -> (x, (norminv (x/k3))*(!k2) + !k1 )) |> List.toArray in														
						pins |> Array.iteri ( fun i _ -> pins.[i] <- false );
						levelsNode.ChildNodes |> Seq.cast |> Seq.iter ( fun (node:XmlNode) ->
							Debug.WriteLine( node.InnerText.ToString() );
							let attribute = node.Attributes.GetNamedItem("index") in 
							Debug.WriteLine( attribute.Value.ToString() );
							let index = System.Int32.Parse( attribute.Value ) - 1
							in pins.[index] <- true;
							let x,_ = data.[index] in
							data.[index] <- (x, Double.Parse(node.InnerText));							
							()
						);
						pins |> Array.iteri ( fun i x -> if not x then resetItem data pins i );
						skillList := data |> Array.toList |> List.map ( fun (x,y) -> y )
						in
					
					()															
				with e -> MessageBox.Show("Paste failed: " + e.Message) |> ignore
				in () 			
			| _ -> ()	
	)

let menuHelp = mainForm.Menu.MenuItems.Add("&Help")
let miHelpAbout = new MenuItem("&About")
do menuHelp.MenuItems.Add(miHelpAbout) |> ignore
do miHelpAbout.Click.Add( fun _ ->
		let dialog = new Form() in dialog.Text <- "About";
		let box = new RichTextBox() in box.Dock <- DockStyle.Fill;
		box.BackColor <- Colour.White;
		box.ForeColor <- Colour.Black;	
		box.SelectionAlignment <- HorizontalAlignment.Center;	
		box.SelectedText <- "Experiment Viewer\r\n\r\n";
		box.SelectionAlignment <- HorizontalAlignment.Left;
		box.SelectedText <- "The starting folder can be specified from the command line or the File Menu.\r\n\r\n" +
							"Once an experiment file has loaded the suggested values for K1 & K2 are viewable by hovering the mouse over the respective labels.\r\n" +
							"The values for K1 & K2 are calculated as the mean and standard deviation of Mu-(n*Sigma).\r\n\r\n" +
							"The setting can be copied and pasted as XML to and from the editable windows using CTRL-C & CTRL-V.\r\n\r\n" +
							"The graphs can be panned by moving the mouse with the left mouse button down, and zoomed with the right mouse button down." +
							"Press F5 to reset the view, and F3 to toggle the grid off and on.";
		box.Enabled <- false;
		dialog.Controls.Add(box);
		dialog.ShowDialog() |> ignore
	) 	
	
	
let testFunc () =
	let form = CreateForm "Test" in
	
	let data = [|(0.0F,1.0F,0);(1.0F,2.0F,1);(2.0F,3.0F,2);(3.0F,4.0F,3)|] in	
	let controls = data |> Array.map( fun _ -> let control = new Label() in control.Visible <- false; control ) in
	controls |> Array.iter (fun control -> form.Controls.Add(control) );
	
	let DrawBarGraph2 (gr:Graphics) (transform:Transformation2d) (brush:#Brush) (values:(float32 * float32 * int) array) =					
		let border = new Pen(Colour.Black) in
		values |> Array.iteri (fun i (x1,x2,y) ->

		let tx, ty = transform.Transform (x1, float32 y) in
		let bx, by = transform.Transform (x2, 0.0F) in		
		gr.FillRectangle(brush, tx, ty, bx - tx, by - ty );		
		gr.DrawRectangle(border, tx, ty, bx - tx, by - ty );
		()
	)
	in
	
	let horizRange = {new Range with min=0.0F; and max=float32 (Array.length data); and name="x" } in
	let vertRange = {new Range with min=0.0F; and max=3.0F; and name="y" } in
	let rangeFunc() = (vertRange, horizRange) in	
	let drawFunc gr t = data |> DrawBarGraph2 gr t (Brushes.Blue) in
	let panel, pbox = AddGraph form rangeFunc drawFunc false in
	let _ = form.Show() in
	()
	
do testFunc()
	
do mainForm.Paint.Add(fun _ -> Application.Exit());;

// Run the main code. The attribute marks the startup application thread as "Single 
// Thread Apartment" mode, which is necessary for GUI applications. 
[<STAThread>]    

// On Vista (don't know about other OS'), if I [a-sazach] am away (e.g. "lock my computer" or if I log out of a TS session)
// then this form just sits there waiting for me to log back in.  
// This hack seems to fix that.
let timer = new System.Windows.Forms.Timer()
do timer.Interval <- 2000 (* ms *)
do timer.add_Tick(new EventHandler(fun _ _ -> mainForm.Close()));;
do timer.Start();;

do Application.Run(mainForm)

let _ = 
   (System.Console.Out.WriteLine "Test Passed"; 
        printf "TEST PASSED OK" ; 
        exit 0)
