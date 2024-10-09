// #Regression 
type Name = string
type Attribute = string * string
and Content =
   | CElem of Name * Attribute list * Content list
   | CText of string

type CFilter = Content -> Content list

(* Crude: doesn't do encode entities like &apos; *)
(* usage: ignore (List.map (ppContent 0) parse) *)
let rec ppContent depth content =
   let indent = "" in
   let ppA a acc = match a with (n,v) -> System.String.Format(" {0}=\"{1}\"{2}", n, v, acc) in
   match content with
   | CElem (n, a, c) -> (match (a,c) with
                               | ([], []) -> System.Console.Write("<{0}/>\n", n)
                               | _  -> System.Console.Write("<{0}{1}>\n", n, List.foldBack ppA a "");
                                       ignore (List.map (ppContent depth) c);
                                       System.Console.Write("</{0}>\n", n))
   | CText s -> System.Console.Write("{0}{1}\n", indent, s)

exception Bad_xml_structure of string

 
let _ = 
      (System.Console.Out.WriteLine "Test Passed"; 
       printf "TEST PASSED OK"; 
       exit 0)
