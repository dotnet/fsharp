// #Conformance 
#indent "off"


open System.Resources

type Resources = A

let foreachE (e : System.Collections.IEnumerator) (f : 'a -> unit) = 
  while (e.MoveNext()) do
    f (unbox e.Current);
  done


let main() = 
  let assembly = (typeof<Resources>).Assembly in 
  Printf.printf "assembly = %s\n" (assembly.ToString());
  let args = System.Environment.GetCommandLineArgs() in
  let rname = if Array.length args > 1 then args.[1] else "Resources" in 
  let resourceMan = new System.Resources.ResourceManager(rname, (typeof<Resources>).Assembly) in 
  let resourceCulture = (null : System.Globalization.CultureInfo) in 
  let image1 : System.Drawing.Bitmap = resourceMan.GetObject("Image1", resourceCulture) :?> System.Drawing.Bitmap in 
  let chimes : System.IO.UnmanagedMemoryStream = resourceMan.GetStream("chimes", resourceCulture) in 
  let icon1 : System.Drawing.Icon = resourceMan.GetObject("Icon1", resourceCulture) :?> System.Drawing.Icon in
  Printf.printf "chimes = %s\n" (chimes.ToString());
  Printf.printf "icon1 = %s\n" (icon1.ToString());
  Printf.printf "image1 = %s\n" (image1.ToString())



do main()

