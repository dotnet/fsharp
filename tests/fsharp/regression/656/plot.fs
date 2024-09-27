#indent "off"

module Plot

open System
open System.Collections.Generic
open System.Windows.Forms
open System.Drawing
open System.Diagnostics

/// Encapsulates a named range of values 
type Range = { min:float32; max:float32; name:string }
		with 
			member instance.distance = instance.max - instance.min
		end
								
/// 2d Transformation type
type Transformation2d = { origin:PointF; scale:SizeF }
	with 
		/// Transform coordinate pair by scale around origin
		member instance.Transform (x,y) =
			let tx = (instance.origin.X + (instance.scale.Width * x)) in 
			let ty = (instance.origin.Y - (instance.scale.Height * y)) in
			(tx,ty)
		
		/// Inverse transform	
		member instance.InverseTransform (x,y) = 
			let tx = (x - instance.origin.X)/instance.scale.Width in
			let ty = (instance.origin.Y - y)/instance.scale.Height in
			(tx,ty)				

		/// Create new 2d transformation from given	variables
		static member Create (source:RectangleF) (dest:RectangleF) (zoom:float32)  = 
			Debug.Assert(dest.Width <> 0.0F ); Debug.Assert(dest.Height <> 0.0F );
			Debug.Assert(source.Width <> 0.0F); Debug.Assert(source.Height <> 0.0F);			
			let scale = new SizeF(dest.Width/source.Width, dest.Height/source.Height) in
			let origin = new PointF(dest.X - (scale.Width  * source.X),	
									dest.Y + (scale.Height * source.Y)) in		
			{ new Transformation2d with origin = origin; 
				and scale = new SizeF(scale.Width*zoom,scale.Height*zoom) }
	end
			
/// Creates a generic font
let CreateFont = new Font(FontFamily.GenericMonospace, 9.0F)

/// Draw string to graphics
let DrawString (gr:Graphics) (pen:Pen) (point:PointF) (label:string) =
	let font = CreateFont in	
	let brush = new SolidBrush(pen.Color) in
	gr.DrawString(label, font, brush, point)
	
/// Measure string
let MeasureString (gr:Graphics) (label:string) =
	gr.MeasureString(label, CreateFont)		
	
/// Draws string to left of specified point					
let DrawStringOnScreen (gr:Graphics) (pen:Pen) (point:PointF) (label:string) =	
	let textSize = MeasureString gr label in
	let x = max point.X 0.0F in let x = min x (gr.VisibleClipBounds.Right - textSize.Width) in
	let y = max point.Y 0.0F in let y = min y (gr.VisibleClipBounds.Bottom - textSize.Height) in	
	let point = new PointF(x,y) in
	DrawString gr pen point label
			
/// Negate points
let Rotate180 points =
	points |> Array.map (fun (x,y) -> (-x,-y))		
	
/// Map relative points to origin	
let MapPointsTo (originx,originy) relPoints  =
	relPoints |> Array.map (fun (x,y) -> new PointF(originx+x, originy+y) )

/// Polygon fill function	
let FillPolygon (gr:Graphics) (brush:Brush) (points:PointF array) = 
	gr.FillPolygon(brush,points)
			
/// Draw X & Y Axis	
let DrawAxis (gr:Graphics) (pen:Pen) (transform:Transformation2d) (horiz:Range) (vert:Range) =
	let x, y = transform.Transform (0.0F, 0.0F) in // Note: axis origin assumed to be at (0,0)
	
	let K = 3.0F in	// Draw lines beyond extents
	let xmin, ymin = transform.Transform (horiz.min, vert.min) in
	let xmax, ymax = transform.Transform (horiz.max, vert.max) in	
	let xmin, ymin, xmax, ymax = (xmin-K, ymin+K, xmax+K, ymax-K) in
		
	// Draw x axis
	let _ = gr.DrawLine(pen, (new PointF(xmin, y)), (new PointF(xmax,y)) ) in
	let rarrow = [|(0.0F, -3.0F);(0.0F,3.0F);(7.0F,0.0F)|] in
	let _ = rarrow |> MapPointsTo (xmax,y) |> FillPolygon gr Brushes.Black in
	let _ = DrawStringOnScreen gr pen (new PointF(xmax + 12.0F, y + 12.0F)) horiz.name in
	let _ = if (horiz.min < 0.0F ) then let _ = Rotate180 rarrow |> MapPointsTo (xmin, y) |> FillPolygon gr Brushes.Black in () in

	// Draw y axis
	gr.DrawLine(pen, (new PointF(x, ymin)), (new PointF(x,ymax)) );
	let uarrow = [|(-3.0F,0.0F);(3.0F,0.0F);(0.0F,-7.0F)|] in
	uarrow |> MapPointsTo (x,ymax) |> FillPolygon gr Brushes.Black;
	if (vert.min < 0.0F ) then 	Rotate180 uarrow |> MapPointsTo (x, ymin) |> FillPolygon gr Brushes.Black;
	let vtsize = (MeasureString gr vert.name) in
	DrawString gr pen (new PointF(x - (vtsize.Width/2.0F), ymax - 8.0F - vtsize.Height)) vert.name

/// Draw X Axis units		
let DrawXAxisUnits (gr:Graphics) (pen:Pen) (transform:Transformation2d) (horiz:Range) (vert:Range) hunit (grid:bool) =
	// Note: axis origin assumed to be at (0,0)
	let _, ymin = transform.Transform (horiz.min, vert.min) in
	let _, ymax = transform.Transform (horiz.max, vert.max) in	
	// Draw horizontal point function
	let DrawHPoint (p:float32) =
		let x, y = transform.Transform (p, 0.0F) in		
		let ystart, yend = if (grid) then (ymin,ymax) else (y - 2.0F, y + 2.0F) in
		gr.DrawLine( pen, new PointF(x, ystart), new PointF(x, yend) );
		let legend = if (hunit >= 1.0F) then sprintf "%.0f" (float p) else sprintf "%.2f" (float p) in
		if( x <> 0.0F ) then
			let textSize = MeasureString gr legend in
			DrawString gr pen (new PointF(x - (textSize.Width / 2.0F), y + 4.0F)) legend
	in	
	// Draw horizontal scale	
	if hunit>0.F then
		let i = ref 0.0F in
		while (!i <= horiz.max) do DrawHPoint !i; i := !i + hunit done;
		i := - hunit;
		while (!i >= horiz.min) do DrawHPoint !i; i := !i - hunit done
	
/// Draw y axis units
let DrawYAxisUnits (gr:Graphics) (pen:Pen) (transform:Transformation2d) (horiz:Range) (vert:Range) vunit (grid:bool) =
	// Note: axis origin assumed to be at (0,0)
	let xmin, _ = transform.Transform (horiz.min, vert.min) in
	let xmax, _ = transform.Transform (horiz.max, vert.max) in	
	// Draw vertical point function
	let DrawVPoint (p:float32) =
		let x, y = transform.Transform (0.0F, p) in
		let xstart, xend = if (grid) then (xmin,xmax) else (x - 2.0F, x + 2.0F) in
		gr.DrawLine( pen, new PointF(xstart, y), new PointF(xend, y) );
		if (y <> 0.0F ) then					
			let legend = if (vunit>=1.0F) then sprintf "%.0f" (float p) else sprintf "%.2f" (float p) in
			let textSize = MeasureString gr legend in
			DrawString gr pen (new PointF(x - textSize.Width - 4.0F,y - 6.0F)) legend		
	in	
	// Draw vertical scale
	if vunit>0.F then
		let i = ref 0.0F in
		while (!i <= vert.max) do DrawVPoint !i; i := !i + vunit done;
		i := - vunit;
		while (!i >= vert.min) do DrawVPoint !i; i := !i - vunit done	
					
/// Creates a transformed point from the coordinate pair
let CreatePoint (transform:Transformation2d)  (x,y) =
	let pointx, pointy = transform.Transform (x,y) in
	new PointF(pointx,pointy)					
				
/// Creates a point array from specified values within specified range
let CreatePoints transform (values:float array)  =
	let pointList = new List<PointF>() in	
	values |>  Array.iteri ( fun (index:int) (value:float) ->	
		pointList.Add( CreatePoint transform (float32 index, float32 value) )		
	);
	pointList.ToArray()
	
/// Draw crosshairs (markers) over points
let DrawCrosshairs (gr:Graphics) (pen:Pen) (points:PointF array) =
	let rects = points |> Array.map ( fun (point:PointF) -> 
		new RectangleF( point.X - (2.0F*pen.Width), point.Y - (2.0F*pen.Width), (4.0F*pen.Width)+1.0F, (4.0F*pen.Width)+1.0F) ) in
	gr.DrawRectangles(pen, rects)
										
/// Plot values on graphics surface with pen
let PlotValues (gr:Graphics) (pen:Pen) transform  (label:string) (values:float array) =	 		
	if Array.length values = 0 then () else
	// Create list of points
	let points = CreatePoints transform values in
	// Draw points as lines
	gr.DrawLines(pen, points );
	// If possible draw crosshairs to highlight points
	let _ = if (transform.scale.Width >= (10.0F*pen.Width)) then DrawCrosshairs gr pen points in
	// Draw label
	let lastPoint = points.[points.Length-1] in		
	DrawStringOnScreen gr pen lastPoint label
