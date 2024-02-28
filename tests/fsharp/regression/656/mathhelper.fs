#indent "off"

module MathHelper

open System
open System.Diagnostics

/// Tree node
type Tree = 
    | LeafNode of int
    | InnerNode of float * Tree * Tree

/// Creates a specialized index tree from an array (Note: nodes are repeated for the search)
let rec CreateTree index (edges:float array) =
    let length = Array.length edges in
    match length with
    | 0 -> LeafNode (index)
    | 1 -> LeafNode (index)
    | n -> 
        let mid = ((Array.length edges)/2) in                         
        let lhs = Array.sub edges 0 mid in
        let rhs = Array.sub edges (mid) (length-mid) in
        
        InnerNode (edges.[mid], CreateTree index lhs, CreateTree (index+mid) rhs)

/// Finds node in tree                   
let rec TreeFind (node:Tree) value  =
    match (node) with
    | LeafNode(index) -> index
    | InnerNode(x,lhs,rhs) -> if (value < x) then TreeFind lhs value else TreeFind rhs value
 
 /// Add one to count
let addone tree (counts: _[]) v =
	let i = TreeFind tree v in
		counts.[i] <- counts.[i] + 1
 
/// Computes the counts for a sample
let histcex (sample:float list) edges counts =
	//let timer = Stopwatch.StartNew() in
    //let edgePairs = List.zip (System.Double.MinValue :: edges) (List.append edges [System.Double.MaxValue]) |> List.toArray in 
    let tree = System.Double.MinValue :: edges |> List.toArray |> CreateTree 0 in
    List.iter (fun x -> 
		//let i2 = Array.FindIndex(edgePairs, (fun (l,u) -> (l <= x && x < u) )) in counts.(i2) <- counts.(i2) + 1
		addone tree counts x		
		 ) sample
	//timer.Stop(); Debug.WriteLine( "Elapsed " + timer.ElapsedMilliseconds.ToString() );		

let histc (sample:float list ) edges =
     let N = List.length edges in    
     let counts = Array.init (N + 1) (fun _ -> 0) in
     histcex sample edges counts;
     counts

/// Computes an equal spacing between a and b of N  numbers
let linspace a b N = 
    let M = N - 1 in 
    List.init N (fun i -> a + (float i) / (float M) * (b - a))

/// Calculate mean of sample
let Mean (sample:float array) = 
	let total = sample |> Array.fold( fun acc value -> acc + value ) 0.0 in
	let n = (Array.length sample) in
	total / (float n)

/// Calculate standard deviation from mean
let StandardDeviationFromMean mean (sample:float array) = 
	let deltaSquared = sample |> Array.fold (fun acc value -> let delta = (mean-value) in acc + (delta*delta) ) 0.0 in
	let n = (Array.length sample) in
	Math.Sqrt(deltaSquared / (float n))
	
/// Calculate standard deviation of sample
let StandardDeviation (sample:float array) = 
	Mean sample |> StandardDeviationFromMean 
	
/// Computes the complementary error function. This function is defined by 2/sqrt(pi) * integral from x to infinity of exp (-t^2) dt.
let erfc x =
    if (System.Double.IsNegativeInfinity (x)) then
        2.0
    else
        if (System.Double.IsPositiveInfinity (x)) then
            0.0
        else
            let z = abs (x) in
            let t = 1.0 / (1.0 + 0.5 * z) in
            let res = t * exp (-z * z - 
                1.26551223 + 
                        t * (1.00002368 + 
                        t * (0.37409196 + 
                        t * (0.09678418 + 
                        t * (-0.18628806 + 
                        t * (0.27886807 + 
                        t * (-1.13520398 +
                        t * (1.48851587 + 
                        t * (-0.82215223 + 
                        t * 0.17087277))))))))) in
                if (x >= 0.0) then
                    res
                else
                    2.0 - res

/// Computes the inverse of the complementary error function. 
let erfcinv y = 
	if (y < 0.0 || y > 2.0) then
		failwith "Inverse complementary function not defined outside [0,2]."
	else if (y = 0.0) then
		System.Double.PositiveInfinity
	else if (y = 2.0) then
		System.Double.NegativeInfinity
	else 
		let x = 
			if (y >= 0.0485 && y <= 1.9515) then
				let q = y - 1.0 in
				let r = q * q in
				(((((0.01370600482778535*r - 0.3051415712357203)*r + 1.524304069216834)*r - 3.057303267970988)*r + 2.710410832036097)*r - 0.8862269264526915) * q /
				(((((-0.05319931523264068*r + 0.6311946752267222)*r - 2.432796560310728)*r + 4.175081992982483)*r - 3.320170388221430)*r + 1.0)
			else if (y < 0.0485) then
				let q = sqrt (-2.0 * log (y / 2.0)) in
				(((((0.005504751339936943*q + 0.2279687217114118)*q + 1.697592457770869)*q + 1.802933168781950)*q + -3.093354679843504)*q - 2.077595676404383) / 
				((((0.007784695709041462*q + 0.3224671290700398)*q + 2.445134137142996)*q + 3.754408661907416)*q + 1.0)
			else if (y > 1.9515) then
				let q = sqrt (-2.0 * log (1.0 - y / 2.0)) in
				-(((((0.005504751339936943*q + 0.2279687217114118)*q + 1.697592457770869)*q + 1.802933168781950)*q + -3.093354679843504)*q - 2.077595676404383) / 
				 ((((0.007784695709041462*q + 0.3224671290700398)*q + 2.445134137142996)*q + 3.754408661907416)*q + 1.0) 
			else 0.0
		in 
		let u = (erfc (x) - y) / (-2.0 / sqrt (Math.PI) * exp (-x * x)) in
		x - u / (1.0 + x * u)

/// Computes the cumulative Gaussian distribution at a specified point of interest.
let normcdf t = 
    let sqrt2 = 1.4142135623730951 in (erfc (-t / sqrt2)) / 2.0
			
/// Computes the inverse of the cumulative Gaussian distribution (quantile function) at a specified point of interest.
let norminv p = 
	let sqrt2 = 1.4142135623730951 in -sqrt2 * erfcinv (2.0 * p)
	
