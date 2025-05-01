#indent "off"

module TrackedPlayers

open System
open System.IO
open System.Text
open System.Xml
open System.Collections.Generic
open System.Windows.Forms
open System.Diagnostics
open System.Drawing
open Misc
open FormsHelper
open Plot
open PlotTrajectory
open PlayerRecords

let k1 = ref 25.0 
let k2 = ref 8.333
let k3 = 50.0

let AutoScale = ref true

/// Read table
let ReadTable fileName =
	let _ = Cursor.Current <- Cursors.WaitCursor in
	let table = LoadPlayerTable fileName in
	let _ = Cursor.Current <- Cursors.Default in	
	table

/// Get array of players last record
let GetPlayersLastRecord (table:PlayerRecord array) = 
	let dict = new Dictionary<string,int>() in
	// Get last array index for each player
	table |> Array.iteri ( fun i (player:PlayerRecord) ->
		let gamertag = player.gamertag in
		if( dict.ContainsKey(gamertag) ) then dict.[gamertag] <- i
		else dict.Add(gamertag, i) );	
	let list = new List<PlayerRecord>() in
	dict |> Seq.iter (fun (pair:KeyValuePair<string,int>) -> list.Add( table.[pair.Value])  );
	list.ToArray()		

/// Gets records associated with a specified player
let GetPlayerRecords (gamertag:string) (table:PlayerRecord array) =
	table |> Array.filter ( fun player -> gamertag = player.gamertag )

/// Get Mu trajectory from specified table
let GetMuTrajectory (table:PlayerRecord array) =
	table |> Array.map ( fun player -> (player.mu) )
	
/// Get TrueSkill trajectory from specified table
let GetTrueSkillTrajectory 	(table:PlayerRecord array) =
	table |> Array.map ( fun player -> player.TrueSkill )
	
/// Get (Bungie) Level trajectory from specified table
let GetLevelTrajectory (table:PlayerRecord array) = 
	table |> Array.map ( fun player -> player.Level !k1 !k2 k3  )

/// Create trajectories for players
let CreateTrajectories (gamertags:string array) table f =
	gamertags |> Array.map ( fun (gamertag:string) -> 
		let values = GetPlayerRecords gamertag table |> f in
		(gamertag, values) )
													
/// Create player check box					
let CreatePlayerCheckBox (toolTip:ToolTip) (player:PlayerRecord) eventHandler =
	let box = CreateCheckBox player.gamertag player eventHandler in
	let tipText = "Mu(" + player.mu.ToString() + ")" 
				+ ", TrueSkill(" + player.TrueSkill.ToString() + ")" 
				+ ", Level(" + (player.Level !k1 !k2 k3).ToString() + ")"
				+ ", Played(" + player.games_played.ToString() + ")" in
	toolTip.SetToolTip(box, tipText); 
	box

// Create eventHandler here
let CheckBoxEventHandler (selected:List<string>) f =
		let eventHandler (sender:obj) (e:EventArgs) = 
		let checkBox = sender :?> CheckBox in 
		let player = checkBox.Tag :?> PlayerRecord in
		let _ = if( true = checkBox.Checked ) then 
			if (false = selected.Contains(player.gamertag)) then selected.Add(player.gamertag) else ()
		else 
			if (true  = selected.Contains(player.gamertag)) then let _ = selected.Remove(player.gamertag) in () else ()
		in 
		f ()
		in eventHandler
					
/// Create CheckBox panel from specified players
let CreateCheckBoxPanel tooltip (players:PlayerRecord array) (selected:List<string>) f =
	// Try to make the panel look square like
	let count = Array.length players in
	let columns = max 10 (int (Math.Sqrt(float count))) in
	// Create panel
	let panel = CreateTableLayoutPanel columns in
	panel.SuspendLayout();
	// Checkbox click handler local definition
	let eventHandler = CheckBoxEventHandler selected f in			
	// Add CheckBoxes
	players |> Array.iter ( fun (player:PlayerRecord) -> 			
			let box = CreatePlayerCheckBox tooltip player eventHandler in	
			let _ = if (true = selected.Contains(player.gamertag)) then box.Checked <- true in				
			panel.Controls.Add(box) 
		);
	// Pad out last row with empty labels	
	let remainder = columns - (count%columns) in
	for i=0 to (remainder+columns) do panel.Controls.Add(new Label()) done;
	panel.ResumeLayout();
	// Return panel
	panel				

/// Line width for plot
let lineWidth = ref 2.0F

/// Draw trajectories to graphics surface
let DrawTrajectories gr transform  trajectories =
	// Colours to cycle through		
	let colours = [|Colour.Red;Colour.Green;Colour.Blue;
					Colour.Yellow;Colour.Orange;Colour.Violet;
					Colour.Brown;Colour.Black;Colour.White|] in
	trajectories |> Array.iteri ( fun index ((name:string),values) ->
		let pen = new Pen(colours.[index%colours.Length]) in		
		pen.Width <- !lineWidth; 		
		PlotValues gr pen transform name values  )

/// Show trajectories	
let ShowTrajectories form vlabel hlabel trajectories =
	// Get maximum length of values array
	let maxLength = trajectories |> Array.fold ( fun acc (_,values) -> Array.length values |> max acc ) 0 in					
	// Define horizontal and vertical ranges
	let horizRange = {new Range with min=0.F; and max=max 1.0F (float32 (maxLength-1)); and name=hlabel } in
	let vertRange = if !AutoScale then 
		let vmin, vmax = trajectories |> Array.fold ( fun ((cmin:float),(cmax:float)) (_,values) -> 
		(Array.fold min cmin values, Array.fold max cmax values) ) (0.0,1.0)
		in { new Range with min=float32 vmin; and max=float32 vmax; and name=vlabel }
	else 
		{new Range with min= -20.0F; and max=70.0F; and name=vlabel }
		
	in
	
	let rangeFun() = (vertRange, horizRange) in
	let drawFun gr transform = trajectories |> DrawTrajectories gr transform in
	AddGraph form rangeFun drawFun false	

/// Get all the child controls of the specified controls
let rec GetAllControls (controls:IEnumerable<Control>) =		
	controls |> Seq.fold (fun acc (c:Control) -> Seq.append acc (GetAllControls (Seq.cast c.Controls)) ) controls

/// Gets collection of selected CheckBoxes in a collection of controls
let GetSelectedCheckBoxes controls =	
	controls |> Seq.filter
	(fun (control:Control) -> match control with
								| :? CheckBox as checkBox -> checkBox.CheckState = CheckState.Checked
								| _ -> false ) 
					
										
/// Populate form
let PopulateForm (form:Form) (formDock:FormDockLocation) measure (playedStart,playedEnd) table =
	let toolTip = new ToolTip() in
	toolTip.ShowAlways <- true; toolTip.ReshowDelay <- 100; toolTip.AutoPopDelay <- 1500; toolTip.InitialDelay <- 100;

	let update = ref true in
	
	// Get list of distinct gamer tags
	let players = GetPlayersLastRecord table |> Array.filter (fun (p:PlayerRecord) -> p.games_played>= playedStart && p.games_played < playedEnd)  in

	let selected = new List<string>() in	
	// Get any existing selections
	let controls = (GetAllControls (Seq.cast form.Controls)) in

	GetSelectedCheckBoxes controls |> Seq.iter ( fun (control:Control) -> 
		let checkBox = control :?> CheckBox in
		let player = checkBox.Tag :?> PlayerRecord in
		if table |> Array.exists (fun p -> p.gamertag = player.gamertag ) then selected.Add(player.gamertag)
	 );
			
	form.Controls.Clear();
		
	let rec SetTrajectoriesWindow () =
		if !update then 
			let childForm = match formDock.GetChildForm "Child" with
				| Some child -> child.Controls.Clear(); child.Menu.MenuItems.Clear(); child
				| None -> let child = CreateForm "Trajectories" in
						  child.Width <- form.Width; child.Height <- 400;	
						  child.Show();
						  formDock.SetChild child "Child" 0 (form.Height+formDock.parent.Height);
						  child
			in															  
			let f = match measure with 
				| "Mu" -> GetMuTrajectory
				| "TrueSkill" -> GetTrueSkillTrajectory
				| "Level" -> GetLevelTrajectory		
				| _ -> GetMuTrajectory 
			in 
			let _ = f |> CreateTrajectories (selected.ToArray()) table
			|> ShowTrajectories childForm measure "Played" in ();
			let menuLine = childForm.Menu.MenuItems.Add("&Line Width") in 
			let miLine1 = new MenuItem("1") in
			let miLine2 = new MenuItem("2") in
			let miLine3 = new MenuItem("3")in
			menuLine.MenuItems.AddRange([|miLine1;miLine2;miLine3|]) |> ignore;			
			miLine1.Click.Add( fun _ -> lineWidth := 1.0F; childForm.Invalidate(true) );
			miLine2.Click.Add( fun _ -> lineWidth := 2.0F; childForm.Invalidate(true) ); 
			miLine3.Click.Add( fun _ -> lineWidth := 3.0F; childForm.Invalidate(true) ); 
			childForm.Menu.MenuItems.Add(menuLine) |> ignore;
			let menuRange = childForm.Menu.MenuItems.Add("&Range") in
			let miRangeAuto = new MenuItem("Auto") in miRangeAuto.Checked <- !AutoScale;
			let _ = menuRange.MenuItems.Add(miRangeAuto) in
			let _ = miRangeAuto.Click.Add( fun _ -> AutoScale := if !AutoScale then false else true; miRangeAuto.Checked <- !AutoScale; SetTrajectoriesWindow()  ) in	
			childForm.Menu.MenuItems.Add(menuRange) |> ignore
	in
		
			
	// Create a tracked player panel
	update := false;
	let panel = ref (CreateCheckBoxPanel toolTip players selected SetTrajectoriesWindow) in
	form.Size <- new Size( form.Size.Width, (!panel).PreferredSize.Height); 
	form.Controls.Add(!panel);
	update := true;
	
	SetTrajectoriesWindow();

	// Create menus	
	let memuMain = form.Menu <- new MainMenu() in
	// Create edit menu
	let menuEdit = form.Menu.MenuItems.Add("&Edit") in
	let miEditCopy = new MenuItem("Copy") in
	miEditCopy.Shortcut <- Shortcut.CtrlC; miEditCopy.ShowShortcut <- true;
	let miEditPaste = new MenuItem("Paste") in
	miEditPaste.Shortcut <- Shortcut.CtrlV; miEditPaste.ShowShortcut <- true;
	let miEditSelAll = new MenuItem("Select All") in
	miEditSelAll.Shortcut <- Shortcut.CtrlA; miEditSelAll.ShowShortcut <- true;
	let miEditClrAll = new MenuItem("Reset All") in
	miEditClrAll.Shortcut <- Shortcut.CtrlR; miEditClrAll.ShowShortcut <- true;
	let _ = menuEdit.MenuItems.Add(miEditCopy) in
	let _ = menuEdit.MenuItems.Add(miEditPaste) in
	let _ = menuEdit.MenuItems.Add(miEditSelAll) in
	let _ = menuEdit.MenuItems.Add(miEditClrAll) in
	
	let _ = miEditCopy.Click.Add ( fun _ -> 
		let builder = new StringBuilder() in
		let writer = new XmlTextWriter( new StringWriter(builder) ) in
		writer.WriteStartElement("players");
		selected |> List.ofSeq |> List.iter ( fun (s:string) -> writer.WriteElementString("gamertag", s) );
		writer.WriteEndElement();
		writer.Close();
		Clipboard.SetDataObject(builder.ToString())
	) in
	let _ = miEditPaste.Click.Add ( fun _ -> 
		let value = Clipboard.GetDataObject() in
		match value.GetData(typeof<string>) with
			| :? string as xml ->
				let _ = try 
					Debug.WriteLine(xml);			
					let doc = new XmlDocument() in
					doc.LoadXml( xml );
					let root = doc.FirstChild in																	
					root.ChildNodes |> Seq.cast |> 
					Seq.filter( fun (node:XmlNode) -> node.Name = "gamertag" ) |>
					Seq.iter( fun (node:XmlNode) -> 
						(!panel).Controls |> Seq.cast |> Seq.iter (fun (control:Control) ->  
						match control with
							| :? CheckBox as checkBox -> 
								let player = checkBox.Tag :?> PlayerRecord in
								if player.gamertag = node.InnerText then checkBox.CheckState <- CheckState.Checked
							| _ ->  () );
					);										
					()															
				with e -> MessageBox.Show("Paste failed: " + e.Message) |> ignore
				in () 			
			| _ -> ()		
	 ) in	
	// Set box state function
	let setBoxState state =
		update := false;
		(!panel).Controls |> Seq.cast |> Seq.iter (fun (control:Control) ->  
			match control with
				| :? CheckBox as checkBox -> checkBox.CheckState <- state
				| _ ->  () );
		update := true; SetTrajectoriesWindow()		
	in						
	let _ = miEditSelAll.Click.Add( fun _ -> CheckState.Checked |> setBoxState ) in
	let _ = miEditClrAll.Click.Add( fun _ -> CheckState.Unchecked |> setBoxState ) in

	// Create sort menu
	let menuSort = form.Menu.MenuItems.Add("&Sort") in
	let miSortName = new MenuItem("Name") in
	let miSortMu = new MenuItem("Mu") in
	let miSortTrueSkill = new MenuItem("TrueSkill") in
	let miSortLevel = new MenuItem("Level") in
	let miSortPlayed = new MenuItem("Played") in
	let miSortSpecial = new MenuItem("Special") in
	let _ = menuSort.MenuItems.Add(miSortName) in
	let _ = menuSort.MenuItems.Add(miSortMu) in
	let _ = menuSort.MenuItems.Add(miSortTrueSkill) in
	let _ = menuSort.MenuItems.Add(miSortLevel) in
	let _ = menuSort.MenuItems.Add(miSortPlayed) in
	let _ = menuSort.MenuItems.Add(miSortSpecial) in
	// Sort boxes function
	let sortBoxes f =
		let toolTip = new ToolTip() in
		toolTip.ShowAlways <- true; toolTip.ReshowDelay <- 100; toolTip.AutoPopDelay <- 1500; toolTip.InitialDelay <- 100;

		update := false; 	
		form.Controls.Clear();
		players |> Array.sortInPlaceWith f ;
		panel := CreateCheckBoxPanel toolTip players selected SetTrajectoriesWindow;
		form.Controls.Add(!panel);
		update := true
	in	
	let _ = miSortName.Click.Add( fun _ -> (fun (p1:PlayerRecord) (p2:PlayerRecord) -> p1.gamertag.CompareTo(p2.gamertag) )
		|> sortBoxes ) in	
	let _ = miSortMu.Click.Add( fun _ -> (fun (p1:PlayerRecord) (p2:PlayerRecord) -> Math.Sign(p1.mu - p2.mu) )  
		|> sortBoxes ) in
	let _ = miSortTrueSkill.Click.Add( fun _ -> (fun (p1:PlayerRecord) (p2:PlayerRecord) -> Math.Sign(p1.TrueSkill - p2.TrueSkill) )  
		|> sortBoxes ) in		
	let _ = miSortLevel.Click.Add( fun _ -> (fun (p1:PlayerRecord) (p2:PlayerRecord) -> Math.Sign( (p1.Level !k1 !k2 k3) - (p2.Level !k1 !k2 k3) ) )  
		|> sortBoxes ) in				
	let _ = miSortPlayed.Click.Add( fun _ -> (fun (p1:PlayerRecord) (p2:PlayerRecord) -> Convert.ToInt32(p1.games_played - p2.games_played) )
		|> sortBoxes ) in
	let _ = miSortSpecial.Click.Add( fun _ ->		
	
		let f = match measure with | "mu" -> ( fun player -> (player.mu) )
			| "TrueSkill" -> ( fun player -> (player.TrueSkill) )
			| "Level" -> ( fun player -> (player.Level !k1 !k2 k3) ) 
			| _ -> ( fun player -> (player.mu) )
		in
	
		let maxF = table |> Array.fold ( fun acc (p:PlayerRecord) ->f(p) |> max acc ) 0.0 in
		let minF = table |> Array.fold ( fun acc (p:PlayerRecord) -> f(p) |> min acc ) 0.0 in
		let maxPlayed = table |> Array.fold ( fun acc (p:PlayerRecord) -> Convert.ToInt32(p.games_played) |> max acc ) 0 in
		let minPlayed = table |> Array.fold ( fun acc (p:PlayerRecord) -> Convert.ToInt32(p.games_played) |> min acc ) 0 in
				
		form.Controls.Clear();				
		
		let width = form.Width in let height = form.Height in
		let eventHandler = CheckBoxEventHandler selected SetTrajectoriesWindow in			
						
	    let OnResize() =	    
    		let toolTip = new ToolTip() in
			toolTip.ShowAlways <- true; toolTip.ReshowDelay <- 100; toolTip.AutoPopDelay <- 1500; toolTip.InitialDelay <- 100;

			update := false;	     			
			form.Controls.Clear();
			players |> Array.iter (fun player -> let box = CreatePlayerCheckBox toolTip player eventHandler in 								
			let v = f(player) in
			box.Top <- (form.Height - 64) - (int ((v - minF)*(float form.Height - 64.0) / (maxF - minF)));
			box.Left <-  (((Convert.ToInt32(player.games_played) - minPlayed)*(form.Width - 64)) / (maxPlayed - minPlayed));
			let _ = if (true = selected.Contains(player.gamertag)) then box.Checked <- true in							
			form.Controls.Add(box)
			);
			update := true;
			form.Invalidate(); () 
		in   
		OnResize();		    
	    form.Resize.Add (fun _ -> OnResize ());    
			
			
		// TODO: handle resize of form
		//)		 		
	) in
	()
																	
/// Show tracker file
let TrackedPlayersHandler form formDock (measure:string) (playedStart,playedEnd) (fileName:string) =
	// Load table async and populate form on completion
	
	let ShowTable table =
		let f() = try PopulateForm form formDock measure (playedStart,playedEnd) table with e -> LogWarning e in
		if not form.IsDisposed then try form.Invoke( new MethodInvoker(f) ) |> ignore with e -> LogWarning e else () 
	in		
	AsyncApp fileName ReadTable ShowTable;
	form
	
	
