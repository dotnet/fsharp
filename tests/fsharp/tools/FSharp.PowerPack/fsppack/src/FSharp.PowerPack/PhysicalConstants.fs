namespace Microsoft.FSharp.Math

open Microsoft.FSharp.Math.SI

/// Fundamental physical constants, with units-of-measure
// Selected from frequently used constants at http://physics.nist.gov/cuu/Constants/index.html
module PhysicalConstants =

  /// speed of light in vacuum
  [<Literal>]
  let c = 299792458.0<m/s>

  /// magnetic constant
  [<Literal>]
  let mu0 = 12.566370614e-7<N A^-2>

  /// electric constant = 1/(mu0 c^2)
  [<Literal>]
  let epsilon0 = 8.854187817e-12<F m^-1>

  /// Newtonian constant of gravitation
  [<Literal>]
  let G = 6.6742867e-11<m^3 kg^-1 s^-2>
  
  /// Planck constant
  [<Literal>]
  let h = 6.6260689633e-34<J s>

  /// Dirac constant, also known as the reduced Planck constant = h/2pi
  [<Literal>]
  let hbar = 1.05457162853e-34<J s>

  /// Elementary charge
  [<Literal>]
  let e = 1.60217648740e-19<C>

  /// Magnetic flux quantum h/2e
  [<Literal>]
  let Phi0 = 2.06783366752e-15<Wb>

  /// Conductance quantum 2e^2/h
  [<Literal>]
  let G0 = 7.748091700453e-5<S>

  /// Electron mass
  [<Literal>]
  let m_e = 9.1093821545e-31<kg>

  /// Proton mass
  [<Literal>]
  let m_p = 1.67262163783e-27<kg>

  /// Fine-structure constant
  [<Literal>]
  let alpha = 7.297352537650e-3

  /// Rydberg constant
  [<Literal>]
  let R_inf = 10973731.56852773<m^-1>

  /// Avogadro constant
  [<Literal>]
  let N_A = 6.0221417930e23<mol^-1>

  /// Faraday constant
  [<Literal>]
  let F = 96485.339924<C/mol>

  /// Molar gas constant
  [<Literal>]
  let R = 8.31447215<J mol^-1 K^-1> 
 
  /// Boltzmann constant R/N_A
  [<Literal>]
  let k = 1.380650424e-23<J/K>

  /// Stefan-Boltzmann constant
  [<Literal>]
  let sigma = 5.67040040e-8<W m^-2 K^-4>
 
  /// Electron volt
  [<Literal>]
  let eV = 1.60217648740e-19<J>

  /// Unified atomic mass unit
  [<Literal>]
  let u = 1.66053878283e-27<kg>


