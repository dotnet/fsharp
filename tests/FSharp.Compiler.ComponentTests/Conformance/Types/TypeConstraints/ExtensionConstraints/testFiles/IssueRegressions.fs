// RFC FS-1043: Regression tests for linked GitHub issues.
// These test the weak resolution changes from the RFC, not extension members directly.
// The issues were fixed by the constraint solver changes that accompany FS-1043.

module IssueRegressions

// ---- dotnet/fsharp#9382: Matrix SRTP stress test ----
// Many statically resolved type parameters, inline record operators.
// Previously produced FS0073 (internal error). Now compiles and runs cleanly.

type Matrix<'a> =
    { m11: 'a; m12: 'a; m13: 'a
      m21: 'a; m22: 'a; m23: 'a
      m31: 'a; m32: 'a; m33: 'a }
    static member inline (/) (m, s) =
        { m11 = m.m11 / s; m12 = m.m12 / s; m13 = m.m13 / s
          m21 = m.m21 / s; m22 = m.m22 / s; m23 = m.m23 / s
          m31 = m.m31 / s; m32 = m.m32 / s; m33 = m.m33 / s }

let inline determinant m =
    m.m11 * m.m22 * m.m33
    + m.m12 * m.m23 * m.m31
    + m.m13 * m.m21 * m.m32
    - m.m31 * m.m22 * m.m13
    - m.m32 * m.m23 * m.m11
    - m.m33 * m.m21 * m.m12

let inline inverse m =
    { m11 = m.m22 * m.m33 - m.m32 * m.m23; m12 = m.m13 * m.m32 - m.m33 * m.m12; m13 = m.m12 * m.m23 - m.m22 * m.m13
      m21 = m.m23 * m.m31 - m.m33 * m.m21; m22 = m.m11 * m.m33 - m.m31 * m.m13; m23 = m.m13 * m.m21 - m.m23 * m.m11
      m31 = m.m21 * m.m32 - m.m31 * m.m22; m32 = m.m12 * m.m31 - m.m32 * m.m11; m33 = m.m11 * m.m22 - m.m21 * m.m12 }
    / (determinant m)

let identity: Matrix<float> =
    { m11 = 1.0; m12 = 0.0; m13 = 0.0
      m21 = 0.0; m22 = 1.0; m23 = 0.0
      m31 = 0.0; m32 = 0.0; m33 = 1.0 }

let inv = inverse identity
if inv.m11 <> 1.0 then failwith $"Expected identity inverse m11=1.0, got {inv.m11}"

// M2: Prove inverse actually generalizes — call at decimal, not just float
let identityDecimal: Matrix<decimal> =
    { m11 = 1.0m; m12 = 0.0m; m13 = 0.0m
      m21 = 0.0m; m22 = 1.0m; m23 = 0.0m
      m31 = 0.0m; m32 = 0.0m; m33 = 1.0m }

let invDecimal = inverse identityDecimal
if invDecimal.m11 <> 1.0m then failwith $"Expected decimal identity inverse m11=1.0m, got {invDecimal.m11}"

// ---- dotnet/fsharp#9416: Records with generic type variables and overloaded operators ----
// Previously produced FS0073 (internal error). Now compiles and runs cleanly.

type Vector<'T> =
    { X: 'T; Y: 'T }
    static member inline (+) (u, a) =
        { X = u.X + a; Y = u.Y + a }

module Vector =
    let inline add (vector: Vector<'T>) amount = vector + amount

let v = Vector.add { X = 1; Y = 2 } 10
if v.X <> 11 then failwith $"Expected 11, got {v.X}"
