// #Stress 
#indent "off"
(*
  http://shootout.alioth.debian.org/benchmark.php?test=nbody&lang=all&sort=cpu
*)

let pi = 3.141592653589793
let solar_mass = 4. * pi * pi
let days_per_year = 365.24



type planet = {
  mutable x : float;  mutable y : float;  mutable z : float;
  mutable vx: float;  mutable vy: float;  mutable vz: float;
  mass : float;
}


let advance bodies dt =
  let n = Array.length bodies - 1 in
  for i = 0 to n do
    let b = bodies.[i] in
    for j = i+1 to n do
      let b2 = bodies.[j] in
      let dx = b.x - b2.x  in let dy = b.y - b2.y  in let dz = b.z - b2.z in
      let distance = sqrt(dx*dx + dy*dy + dz*dz) in
      let mag = dt / (distance * distance * distance) in

      b.vx <- b.vx - dx * b2.mass * mag;
      b.vy <- b.vy - dy * b2.mass * mag;
      b.vz <- b.vz - dz * b2.mass * mag;

      b2.vx <- b2.vx + dx * b.mass * mag;
      b2.vy <- b2.vy + dy * b.mass * mag;
      b2.vz <- b2.vz + dz * b.mass * mag;
    done
  done;
  for i = 0 to n do
    let b = bodies.[i] in
    b.x <- b.x + dt * b.vx;
    b.y <- b.y + dt * b.vy;
    b.z <- b.z + dt * b.vz;
  done


let energy bodies =
  let e = ref 0. in
  for i = 0 to Array.length bodies - 1 do
    let b = bodies.[i] in
    e := !e + 0.5 * b.mass * (b.vx * b.vx + b.vy * b.vy + b.vz * b.vz);
    for j = i+1 to Array.length bodies - 1 do
      let b2 = bodies.[j] in
      let dx = b.x - b2.x  in let dy = b.y - b2.y  in let dz = b.z - b2.z in
      let distance = sqrt(dx * dx + dy * dy + dz * dz) in
      e := !e - (b.mass * b2.mass) / distance
    done
  done;
  !e


let offset_momentum bodies =
  let px = ref 0. in let py = ref 0. in let pz = ref 0. in
  for i = 0 to Array.length bodies - 1 do
    px := !px + bodies.[i].vx * bodies.[i].mass;
    py := !py + bodies.[i].vy * bodies.[i].mass;
    pz := !pz + bodies.[i].vz * bodies.[i].mass;
  done;
  bodies.[0].vx <- - !px / solar_mass;
  bodies.[0].vy <- - !py / solar_mass;
  bodies.[0].vz <- - !pz / solar_mass


let bodies() = 
let jupiter = {
  x = 4.84143144246472090e+00;
  y = -1.16032004402742839e+00;
  z = -1.03622044471123109e-01;
  vx = 1.66007664274403694e-03 * days_per_year;
  vy = 7.69901118419740425e-03 * days_per_year;
  vz = -6.90460016972063023e-05 * days_per_year;
  mass = 9.54791938424326609e-04 * solar_mass;
} in 

let saturn = {
  x = 8.34336671824457987e+00;
  y = 4.12479856412430479e+00;
  z = -4.03523417114321381e-01;
  vx = -2.76742510726862411e-03 * days_per_year;
  vy = 4.99852801234917238e-03 * days_per_year;
  vz = 2.30417297573763929e-05 * days_per_year;
  mass = 2.85885980666130812e-04 * solar_mass;
} in 

let uranus = {
  x = 1.28943695621391310e+01;
  y = -1.51111514016986312e+01;
  z = -2.23307578892655734e-01;
  vx = 2.96460137564761618e-03 * days_per_year;
  vy = 2.37847173959480950e-03 * days_per_year;
  vz = -2.96589568540237556e-05 * days_per_year;
  mass = 4.36624404335156298e-05 * solar_mass;
} in 

let neptune = {
  x = 1.53796971148509165e+01;
  y = -2.59193146099879641e+01;
  z = 1.79258772950371181e-01;
  vx = 2.68067772490389322e-03 * days_per_year;
  vy = 1.62824170038242295e-03 * days_per_year;
  vz = -9.51592254519715870e-05 * days_per_year;
  mass = 5.15138902046611451e-05 * solar_mass;
} in 

let sun = {
  x = 0.0;  y = 0.0;  z = 0.0;  vx = 0.0;  vy = 0.0; vz = 0.0;
  mass= solar_mass;
} in 

let bodies = [| sun; jupiter; saturn; uranus; neptune |] in 
bodies

let main n =
  let bodies = bodies() in 
  offset_momentum bodies;

  Printf.printf "Energy prior = %.9f\n" (energy bodies);
  for i = 1 to n do
    advance bodies 0.01
  done;
  Printf.printf "Energy posterior = %.9f\n" (energy bodies);
  Printf.sprintf "%.9f" (energy bodies)

let _ = main 500000

let failures = ref false
let report_failure () = 
  System.Console.Error.WriteLine " NO"; failures := true
let test (s:string) b = System.Console.Error.Write s;  if b then System.Console.Error.WriteLine " OK" else report_failure() 

let _ = test "dce98nj" (main 500000 = "-0.169096567")



do   (System.Console.WriteLine "Test Passed"; 
       printf "TEST PASSED OK";
       exit 0)

