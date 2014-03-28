namespace Microsoft.FSharp.Math


// System Internationale. See http://www.bipm.org/en/si/si_brochure/general.html
/// The International System of Units (SI)
module SI =

  [<Measure>] 
  /// metre (or meter), SI unit of length
  type m               

  [<Measure>] 
  /// kilogram, SI unit of mass
  type kg

  [<Measure>] 
  /// second, SI unit of time
  type s

  [<Measure>] 
  /// ampere, SI unit of electric current
  type A             

  [<Measure>] 
  /// kelvin, SI unit of thermodynamic temperature
  type K              

  [<Measure>] 
  /// mole, SI unit of amount of substance
  type mol             

  [<Measure>] 
  /// candela, SI unit of luminous intensity
  type cd              

  [<Measure>] 
  /// hertz, SI unit of frequency
  type Hz = s^-1

  [<Measure>] 
  /// newton, SI unit of force
  type N = kg m / s^2 

  [<Measure>] 
  /// pascal, SI unit of pressure, stress
  type Pa = N / m^2

  [<Measure>] 
  /// joule, SI unit of energy, work, amount of heat
  type J = N m

  [<Measure>] 
  /// watt, SI unit of power, radiant flux
  type W = J / s       

  [<Measure>] 
  /// coulomb, SI unit of electric charge, amount of electricity
  type C = s A 

  [<Measure>] 
  /// volt, SI unit of electric potential difference, electromotive force
  type V = W/A        

  [<Measure>] 
  /// farad, SI unit of capacitance
  type F = C/V

  [<Measure>] 
  /// ohm, SI unit of electric resistance
  type ohm = V/A       

  [<Measure>] 
  /// siemens, SI unit of electric conductance
  type S = A/V         

  [<Measure>] 
  /// weber, SI unit of magnetic flux
  type Wb = V s        

  [<Measure>] 
  /// tesla, SI unit of magnetic flux density
  type T = Wb/m^2      

  [<Measure>] 
  /// henry, SI unit of inductance
  type H = Wb/A        

  [<Measure>] 
  /// lumen, SI unit of luminous flux
  type lm = cd        

  [<Measure>] 
  /// lux, SI unit of illuminance
  type lx = lm/m^2 

  [<Measure>] 
  /// becquerel, SI unit of activity referred to a radionuclide
  type Bq = s^-1       

  [<Measure>] 
  /// gray, SI unit of absorbed dose
  type Gy = J/kg       

  [<Measure>] 
  /// sievert, SI unit of does equivalent
  type Sv = J/kg       

  [<Measure>] 
  /// katal, SI unit of catalytic activity
  type kat = mol/s 
