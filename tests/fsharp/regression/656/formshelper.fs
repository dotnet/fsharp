#indent "off"

module FormsHelper

open System
open System.Collections.Generic
open System.Windows.Forms
open System.Drawing

/// Add a control to a control
let AddToControl (parent:#Control) (child:#Control) = parent.Controls.Add(child)

/// Add controls to a control
let AddArrayToControl (parent:#Control) (children:#Control array) = 
	parent.Controls.AddRange(children |> Array.map (fun (i:#Control) -> (i :> Control)))

/// Add a hint of blue when mouse enters control
let BlueFlash (control:#Control) =
	let AddBlue n (c:Color) = 
		Color.FromArgb(int c.R,int c.G, min 255 (int c.B + n))		
	in
	control.MouseEnter.Add( fun _ -> 
		control.ForeColor <- Color.DarkBlue;
		control.BackColor <- AddBlue 5 Control.DefaultBackColor
	);
	control.MouseLeave.Add( fun _ -> 
		control.ForeColor <- Control.DefaultForeColor; 
		control.BackColor <- Control.DefaultBackColor )

/// Creates a numeric text box
let CreateNumericBox allowNeg = 
	let edit = new TextBox() in
	edit.KeyDown.Add( fun(e:KeyEventArgs) -> if( Keys.D0 <= e.KeyCode && Keys.D9 >= e.KeyCode ) then ()
					else if (Keys.NumPad0 <= e.KeyCode && Keys.NumPad9 >= e.KeyCode ) then ()
					else if (Keys.Back = e.KeyCode) then ()
					else if (Keys.Left = e.KeyCode || Keys.Right = e.KeyCode) then ()
					else if (Keys.OemPeriod = e.KeyCode) then ()
					else if (allowNeg && 0 = edit.SelectionStart && Keys.Subtract = e.KeyCode) then ()
					else if (allowNeg && 0 = edit.SelectionStart && Keys.OemMinus = e.KeyCode) then ()					
					else e.SuppressKeyPress <- true );
	edit

/// Creates a form with a specified name
let CreateForm name =
	let form = new Form() in form.Text <- name; form

/// Creates a label with the specified text
let CreateLabel text =
	let label = new Label() in
	label.Text <- text;
	label	

/// Creates a radio button	
let CreateRadio label changeHandler =
	let radio = new RadioButton() in 
	radio.Text <- label; 
	radio.CheckedChanged.AddHandler(new EventHandler(changeHandler) );
	radio	

/// Adds tool tip specified text to control
let AddToolTip (control:#Control) (text:string) =
	let tip = new ToolTip() in tip.SetToolTip(control, text)

/// Creates a track bar	
let CreateTrackBar label extent tickFreq value changedHandler =
	let slider = new TrackBar() in 
	let tip = new ToolTip() in tip.SetToolTip(slider, label);
	slider.Text <- label; 
	slider.Maximum <- extent; 
	slider.TickFrequency <- tickFreq;
	slider.Value <- value;
	slider.ValueChanged.AddHandler(new EventHandler(changedHandler)); 
	slider	
	
/// Create a combo box with the specified names
let CreateCombo names =
	let combo = new ComboBox() in
	Array.iter ( fun name -> let _ = combo.Items.Add(name) in () ) names;
	combo	
	
type OpaqueCheckBox =
	class 
		inherit CheckBox
		new () as x = { inherit CheckBox();  } (* then 			
			x.SetStyle(ControlStyles.UserPaint,true);								
			x.UseVisualStyleBackColor <- true;
			x.FlatStyle <- FlatStyle.Flat
			override x.OnPaint (p:PaintEventArgs) = ControlPaint.DrawCheckBox(p.Graphics, p.ClipRectangle, if x.Checked then ButtonState.Checked else ButtonState.Normal);
				p.Graphics.DrawString( x.Text, new Font(FontFamily.GenericMonospace, 9.0F), Brushes.Black, float32 p.ClipRectangle.Left, float32 p.ClipRectangle.Top) *)
	end	
		
/// Create a check box with given name and handler	
let CreateCheckBox name tag eventHandler =
	let box = new OpaqueCheckBox() in 		
	box.CheckStateChanged.AddHandler(new EventHandler(eventHandler));		
	box.Tag <- tag;	box.Text <- name;
	box
	
/// Create a table layout panel	
let CreateTableLayoutPanel columns = 	
	let panel = new TableLayoutPanel() in 
	panel.GrowStyle <- TableLayoutPanelGrowStyle.AddRows;
	panel.ColumnCount <- columns;
	panel.Dock <- DockStyle.Fill; 
	panel

/// Creates a PictureBox with the specified Bitmap	
let CreatePictureBox (bm:Bitmap) =
	let pict = new PictureBox() in	
	pict.Size <- bm.Size;	pict.Image <- bm;
	pict.Dock <- DockStyle.Fill;
	pict
				
/// Creates a form from a control
let CreateFormWithControl (title:string) (control:#Control) =
	// Create form to contain picture box 
	let form = CreateForm(title) in 
	let header = 32 in
	form.Size <- new Size(control.Size.Width, control.Size.Height + header); 
	control.Dock <- DockStyle.Fill;
	form.Controls.Add(control); 
	form
	
/// Docks desktop location of parent form with a set of named child forms
type FormDockLocation = 
	class
		val parent : Form		// Parent form
		val mutable childForms : ((Form * string * int * int) list)	// List of child forms
				
		/// Docks child locations with parent
		member instance.DockChildren () = 
			instance.childForms |> List.iter ( 
				fun ((child:Form),label,x,y) -> 					
					let point = new Point(instance.parent.DesktopLocation.X + x, instance.parent.DesktopLocation.Y + y ) in
					child.DesktopLocation <- point )
		
		/// Constructor (would ideally be private)
		new (form:Form) = { parent = form; childForms = [] }
		
		///	Create instance factory method
		static member Create (form:Form) =
			let instance = new FormDockLocation(form) in			
			form.Move.Add ( fun _ -> instance.DockChildren() );
			form.HandleDestroyed.AddHandler ( fun sender e -> 
				// Close child windows
				instance.childForms |> List.iter (fun (child,_,_,_) -> let _ = child.Close() in ()  ) );				
			instance
			
		/// Get child form instance with specified name
		member instance.GetChildForm name =
			match instance.childForms |> List.tryFind (fun (_,label,_,_) -> (label = name) ) with
				| Some (form,_,_,_) -> Some(form)
				| None -> None
					
		/// Set child - there can be only one with a specified name
		member instance.SetChild (child:Form) name x y = 		
			child.DesktopLocation <- new Point(instance.parent.DesktopLocation.X + x, instance.parent.DesktopLocation.Y + y);		
			// Remove any forms with same label
			instance.childForms <- instance.childForms |> 
				List.filter (fun ((form:Form),label,x,y) -> if (label = name) then let _ = form.Close() in false else true );
			// Add child form
			instance.childForms <- (child,name,x,y) :: instance.childForms;
			// Handle window closed/destroyed
			child.HandleDestroyed.AddHandler ( fun sender e -> 
				let control = sender :?> Form in
				instance.childForms <- instance.childForms |> List.filter ( fun (form,label,x,y) -> form <> control ) 
			)
	end	
	
