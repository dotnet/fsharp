#indent "off"

module PlotTrajectory

open System
open System.Collections.Generic
open System.Windows.Forms
open System.Drawing
open FormsHelper
open Plot

/// Map US to English(UK) spelling
type Colour = Color	

/// Infinite collection of units
let WholeUnits = 
	Seq.initInfinite ( fun  (i:int) ->
		let sequence = [|1.0;2.0;5.0|] in
		let x = i / (Array.length sequence) in
		let y = i % (Array.length sequence) in				
		let value = (Math.Pow(10., float x) * sequence.[y]) in
		value
		)
		
// Infinite collection of fractional units
let FractionalUnits =
	let ItemAt i = 
		let sequence = [|1.0;0.5;0.2|] in
		let x = i / (Array.length sequence) in
		let y = i % (Array.length sequence) in				
		(Math.Pow(10., float (-x) ) * sequence.[y]) 
	in
	Seq.initInfinite ( fun  (i:int) -> (ItemAt i, ItemAt (i+1)) )		

/// Gets a unit from the specified value				
let GetUnit (value:float) =
	let sign = float(Math.Sign(value)) in 
	let value2 = value * sign in
	if ( value > 1.0 ) then let unit = WholeUnits |> Seq.find ( fun unit -> unit >= value2 ) in unit * sign
	else 
		let previous, unit = FractionalUnits |> Seq.find ( fun (previous,unit) -> unit <= value2 ) in previous * sign
		
/// Draw scale
let DrawScaleToGraphics (gr:Graphics) transform horiz vert grid =
	// Draw scale
	let pen = new Pen(Color.Gray) in
	let v = (16.0F/transform.scale.Height) in let vunit = GetUnit (float v) in 
	DrawYAxisUnits gr pen transform horiz vert (float32 vunit) grid;
	let h = (32.0F/transform.scale.Width) in let hunit = GetUnit (float h) in
	DrawXAxisUnits gr pen transform horiz vert (float32 hunit) grid;		
	let pen = new Pen(Color.Black) in
	DrawAxis gr pen transform horiz vert
	
/// Show graph	
let AddGraph (form:Form) rangeFun drawFun pinUp =
	// Variables
	let margin = new SizeF(64.0F, 32.0F) in
	let header = 32 in
	let width = form.Width in let height = form.Height-32 in 
	let pinned = ref false in
	let panX = ref 0.0F in
    let panY = ref 0.0F in
    let zoom = ref 1.0F in
    let drawGrid = ref true in
    let transform = ref {new Transformation2d with origin = (new PointF(0.F,0.F)); and scale = (new SizeF(0.F,0.F)) } in
    let hlabel = ref "" in let vlabel = ref "" in

	// Create bitmap (double buffer)
	let buffers = [|new Bitmap(width, height); new Bitmap(width,height)|] in let buffer = ref 0 in
	let bm = buffers.[!buffer] in
	// Create PictureBox and add to panel
	let picture = CreatePictureBox bm in
	let panel = new Panel() in panel.Controls.Add(picture);
	panel.Dock <- DockStyle.Fill;
	panel.Size <- bm.Size;
	
	form.Controls.Add(panel); 
	
	let statusBar = new StatusBar() in
		    
    let OnPaint() = 
    	let vertRange, horizRange = rangeFun() in
    	vlabel := vertRange.name; hlabel := horizRange.name;
		// Create transformation
		let width = (float32 (panel.Size.Width)) in let height = (float32 panel.Size.Height) in
		let source = new RectangleF(horizRange.min, vertRange.min, horizRange.distance, vertRange.distance) in
		let dest = new RectangleF(margin.Width - (!panX * width), height - margin.Height - (!panY*height), 
			width - (margin.Width * 2.0F), height - (margin.Height * 2.0F)) in
		transform := Transformation2d.Create source dest !zoom;
		// Draw to non-visible buffer
		buffer := (!buffer + 1)%2;
		let bm =  if( (buffers.[!buffer]).Size.Equals(panel.Size) ) then buffers.[!buffer] else
			let _ = (buffers.[!buffer]).Dispose() in buffers.[!buffer] <- new Bitmap(panel.Width,panel.Height); 
			buffers.[!buffer]
		in
		let gr = Graphics.FromImage(bm) in
		gr.Clear(panel.BackColor);
		// Draw scale
		DrawScaleToGraphics gr !transform horizRange vertRange !drawGrid;
		// Draw function
		drawFun gr !transform;
		// Now set picture with updated bitmap -> avoids flicker
		picture.Image <- bm;    
		()         
    in   		    
    panel.Paint.Add (fun _ -> OnPaint ());    
            
    let oldMouseX = ref 0 in
    let oldMouseY = ref 0 in             
            
   	picture.MouseDown.Add (fun me -> oldMouseX := me.X; oldMouseY := me.Y );	
	
	picture.MouseMove.Add (fun me ->
		if not !pinned then 
		let x,y = (!transform).InverseTransform (float32 me.X, float32 me.Y) in 
		let loc = sprintf "(%s,%s) (%.2f,%.2f)" !hlabel !vlabel (float x) (float y)  in
		statusBar.Text <- loc;
        let _ = match me.Button with
        | MouseButtons.Left ->  let dx = float32 (me.X - !oldMouseX) in let width = (float32 ) panel.Size.Width in  
								panX := !panX - (dx/width);
                                let dy = float32 (me.Y - !oldMouseY) in let height = (float32 ) panel.Size.Height in  
                                panY := !panY - (dy/height);
                                oldMouseX := me.X; oldMouseY := me.Y;
                                panel.Invalidate ()
        | MouseButtons.Right -> zoom := max 0.1F (!zoom - float32 (me.Y - !oldMouseY) / 100.0F);
                                oldMouseX := me.X; oldMouseY := me.Y;
                                panel.Invalidate ()
        | _ -> ()
        in ()
        
        );
        
    picture.MouseLeave.Add(fun _ -> statusBar.Text <- System.String.Empty );
        
    panel.Resize.Add (fun _ -> panel.Invalidate ());    
        		
	form.Controls.Add( statusBar );
		
	let memuMain = form.Menu <- new MainMenu() in
	let menuFile = form.Menu.MenuItems.Add("&File") in
	let miFileSaveAs = new MenuItem("&Save As") in
	miFileSaveAs.Shortcut <- Shortcut.CtrlS; miFileSaveAs.ShowShortcut <- true;
	let _ = menuFile.MenuItems.Add(miFileSaveAs) in
	let _ = miFileSaveAs.Click.Add( fun _ ->
			let bm = buffers.[!buffer] in
			let dialog = new SaveFileDialog() in			
			dialog.Filter <- "Bitmap|*.bmp|GIF|*.gif|JPEG|*.jpg";
			let _ = if ( DialogResult.OK = dialog.ShowDialog() ) then
				let imageFormat = match dialog.FilterIndex with 
					| 2 -> Imaging.ImageFormat.Gif
					| 3 -> Imaging.ImageFormat.Jpeg
					| _ -> Imaging.ImageFormat.Bmp in
				bm.Save( dialog.FileName, imageFormat)
			in ()
		) in
	let menuEdit = form.Menu.MenuItems.Add("&Edit") in
	let miEditCopy = new MenuItem("&Copy") in
	miEditCopy.Shortcut <- Shortcut.CtrlC; miEditCopy.ShowShortcut <- true;
	let _ = menuEdit.MenuItems.Add(miEditCopy) in
	let _ = miEditCopy.Click.Add( fun _ ->
			let bm = buffers.[!buffer] in Clipboard.SetDataObject(bm)	// Copy image to clip board
		) in
	let menuView = form.Menu.MenuItems.Add("&View") in
	let miViewGrid = new MenuItem("&Grid") in miViewGrid.Checked <- !drawGrid;
	miViewGrid.Shortcut <- Shortcut.F3; miViewGrid.ShowShortcut <- true;
	let miViewReset = new MenuItem("&Reset") in
	miViewReset.Shortcut <- Shortcut.F5; miViewReset.ShowShortcut <- true;
	let _ = menuView.MenuItems.AddRange([|miViewGrid;miViewReset|]) in
	if  pinUp then 
		(
			pinned := true;
			let miViewPinned = new MenuItem("&Pin") in
			miViewPinned.Shortcut <- Shortcut.F6; miViewPinned.ShowShortcut <- true;
			let _ = miViewPinned.Click.Add( fun _ -> pinned := if !pinned then false else true; miViewPinned.Checked <- !pinned ) in
			let _ = menuView.MenuItems.Add(miViewPinned) in ()
		);
	let _ = miViewGrid.Click.Add( fun _ ->
		if (!drawGrid) then drawGrid := false else drawGrid := true;
		miViewGrid.Checked <- !drawGrid; panel.Invalidate() ) 
	in	
	let _ = miViewReset.Click.Add( fun _ -> panX := 0.0F; panY := 0.0F; zoom := 1.0F; panel.Invalidate() ) in
	(panel,picture)
	
/// Creates new form and shows graph	
let ShowGraph title vertRange horizRange drawFun =
	let rangeFun() = (vertRange, horizRange) in
	let form = CreateForm title in
	form.Width <- 1000; form.Height <- 800;	
	let picture = AddGraph form rangeFun drawFun false in
	let _ = form.Show() in
	(form,picture)
	
/// Draw line graph to graphics surface 
let DrawLineGraph (gr:Graphics) transform (data:(float * float) array) = 
	let pen = new Pen(Colour.Red) in
	pen.Width <- 2.0F;
	let points = data |> Array.map ( fun (x,y) -> CreatePoint transform (float32 x, float32 y) ) in
	gr.DrawLines(pen, points);
	if (transform.scale.Width >= (8.0F*pen.Width)) then DrawCrosshairs gr pen points 	 	
		
/// Draw scattergram to graphics surface		
let DrawScattergram (gr:Graphics) (transform:Transformation2d) (pairs:(float * float) array) =
	let pen = new Pen(Colour.Black) in		 	
	pairs |> Array.iter ( fun (x,y) ->
		let tx, ty  = transform.Transform (float32 x, float32 y) in
		let point = new PointF(tx,ty) in
		gr.DrawLine(pen,point, new PointF(point.X+0.5F, point.Y+0.5F) ) 
		)

/// Draw bar graph to graphics surface 
let DrawBarGraph (gr:Graphics) (transform:Transformation2d) (brush:#Brush) (values:(float32 * float32 * int) array) =
	let border = new Pen(Colour.Black) in		 	
	values |> Array.iter (fun (x1,x2,y) ->
		let tx, ty = transform.Transform (x1, float32 y) in
		let bx, by = transform.Transform (x2, 0.0F) in		
		gr.FillRectangle(brush, tx, ty, bx - tx, by - ty );
		gr.DrawRectangle(border, tx, ty, bx - tx, by - ty );
	)

/// Draw bar graph to graphics surface
let DrawLabelledBarGraph (gr:Graphics) (transform:Transformation2d) (pairs:(string * float) array) =
	let blue = new SolidBrush(Colour.Blue) in
	let black = new Pen(Colour.Black) in		 	
	pairs |> Array.iteri (fun x (label,y) ->
		let tx, ty = transform.Transform (float32 x, float32 y) in
		let bx, by = transform.Transform (float32 (x+1), 0.0F) in		
		gr.FillRectangle(blue, tx, ty, bx - tx, by - ty );
		gr.DrawRectangle(black, tx, ty, bx - tx, by - ty );		
	)	
	
/// Show line graph	
let ShowLineGraph vlabel hlabel (data:(float * float) array) =
	let xl, xh, yl, yh = data |> Array.fold ( fun acc (x,y) -> 
		let xl,xh,yl,yh = acc in (min x xl, max x xh, min y yl, max y yh) 
	) (Double.MaxValue,Double.MinValue,Double.MaxValue,Double.MinValue) in
     let yl,yh = if yl=yh then yl - 1.,yh + 1. else yl,yh in
	let horizRange = {new Range with min=min 0.0F (float32 xl); and max=float32 xh; and name=hlabel } in	
	let vertRange = { new Range with min=min 0.0F (float32 yl); and max=float32 yh; and name=vlabel } in
	let drawFun gr transform = data |> DrawLineGraph gr transform in
	ShowGraph "Line graph" vertRange horizRange drawFun
	
/// Show scatter graph	
let ShowScattergram vlabel hlabel data =
	let horizRange = {new Range with min=0.F; and max=1000.0F; and name=hlabel } in
	let vertRange = {new Range with min= -20.0F; and max=70.0F; and name=vlabel } in
	let drawFun gr transform = data |> DrawScattergram gr transform in
	ShowGraph "Scattergram" vertRange horizRange drawFun

/// Show bar graph
let ShowLabelledBarGraph vlabel hlabel (data:(string * float) array) = 	
	let low, high = data |> Array.fold ( fun acc (_,i) -> let l,h = acc in (min i l, max i h) ) (Double.MaxValue , Double.MinValue) in
	let horizRange = {new Range with min=0.0F; and max=float32 (Array.length data); and name=hlabel } in
	let vertRange = {new Range with min=min (float32 low) 0.0F; and max=float32 high; and name=vlabel } in
	let drawFun gr transform = data |> DrawLabelledBarGraph gr transform in
	ShowGraph "Bar graph" vertRange horizRange drawFun
