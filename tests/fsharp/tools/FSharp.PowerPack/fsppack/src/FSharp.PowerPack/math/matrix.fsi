namespace Microsoft.FSharp.Math

open Microsoft.FSharp.Math
open System
open System.Collections
open System.Collections.Generic

/// The type of matrices. The arithmetic operations on the element type are determined by inspection on the element type itself.
/// Two representations are supported: sparse and dense. 
[<Sealed>]
type Matrix<'T> = 

    /// Get the number of rows in a matrix
    member NumRows : int

    /// Get the number of columns in a matrix
    member NumCols : int

    /// Get the number of (rows,columns) in a matrix
    member Dimensions : int * int

    /// Get the item at the given position in a matrix
    member Item : int * int -> 'T with get,set

    /// Supports the slicing syntax 'A.[idx1..idx2,idx1..idx2]'
    member GetSlice : start1:int option * finish1:int option * start2:int option * finish2:int option -> Matrix<'T>

    /// Supports the slicing syntax 'A.[idx1..idx2,idx1..idx2] <- B'
    member SetSlice : start1:int option * finish1:int option * start2:int option * finish2:int option * source:Matrix<'T> -> unit
    
    /// Retrieve the dictionary of numeric operations associated with the element
    /// type of this matrix. Accessing the property may raise an NotSupportedException if the element
    /// type doesn't support any numeric operations. The object returned
    /// may support additional numeric operations such as IFractional: 
    /// this can be determined by a dynamic type test against the object
    /// returned.
    member ElementOps : INumeric<'T>

    /// Point-wise addition of two matrices. An InvalidArgument exception will be
    /// raised if the dimensions do not match.
    static member ( +  ) : Matrix<'T> * Matrix<'T> -> Matrix<'T>

    /// Point-wise subtraction of two matrices. An InvalidArgument exception will be
    /// raised if the dimensions do not match.
    static member ( -  ) : Matrix<'T> * Matrix<'T> -> Matrix<'T>

    /// Matrix negation. 
    static member ( ~- ) : Matrix<'T>              -> Matrix<'T>

    /// Prefix '+' operator. A nop.
    static member ( ~+ ) : Matrix<'T>              -> Matrix<'T>

    /// Matrix multiplication. An InvalidArgument exception will be
    /// raised if the dimensions do not match.
    static member ( * ) : Matrix<'T> * Matrix<'T> -> Matrix<'T>

    /// Matrix-vector multiplication. 
    static member ( * ) : Matrix<'T> * Vector<'T> -> Vector<'T>

    /// Multiply each element of a matrix by the given scalar value
    static member ( * ) : Matrix<'T> * 'T          -> Matrix<'T>

    /// Point-wise matrix multiplication. An InvalidArgument exception will be
    /// raised if the dimensions do not match.
    static member ( .* ) : Matrix<'T> * Matrix<'T>  -> Matrix<'T>

    /// Multiply each element of a matrix by a scalar value
    static member ( * ) : 'T          * Matrix<'T> -> Matrix<'T>
    
    /// Get the transpose of a matrix.
    member Transpose : Matrix<'T>
    
    /// Permutes the rows of a matrix.
    member PermuteRows : permutation:(int -> int) -> Matrix<'T>
    
    /// Permutes the columns of a matrix.
    member PermuteColumns : permutation:(int -> int) -> Matrix<'T>


    //interface IMatrix<'T>
    interface IComparable
    interface IStructuralComparable
    interface IStructuralEquatable
    interface IEnumerable<'T>
    override GetHashCode : unit -> int
    override Equals : obj -> bool
  
    /// Return a new array containing the elements of a matrix
    member ToArray2D : unit -> 'T[,]  

    /// Return the non-zero entries of a sparse or dense matrix
    member NonZeroEntries: seq<int * int * 'T> 

    /// Convert a matrix to a row vector
    member ToRowVector : unit -> RowVector<'T>                 

    /// Convert a matrix to a column vector
    member ToVector : unit -> Vector<'T> 

    /// Returns sqrt(sum(norm(x)*(norm(x))) of all the elements of a matrix.
    /// The element type of a matrix must have an associated instance of INormFloat<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
    member Norm : float

    /// Select a column from a matrix
    member Column : index:int -> Vector<'T>

    /// Select a row from a matrix
    member Row :  index:int -> RowVector<'T>

    /// Select a range of columns from a matrix
    member Columns : start:int  * length:int -> Matrix<'T>

    /// Select a range of rows from a matrix
    member Rows : start:int * length:int -> Matrix<'T>

    /// Select a region from a matrix
    member Region : starti:int * startj:int * lengthi:int * lengthj:int -> Matrix<'T>


    /// Return the nth diagonal of a matrix, as a vector. Diagonal 0 is the primary
    /// diagonal, positive diagonals are further to the upper-right of a matrix.
    member GetDiagonal : int -> Vector<'T>

    /// Get the main diagonal of a matrix, as a vector
    member Diagonal : Vector<'T>

    /// Create a new matrix that is a copy of an array
    member Copy : unit -> Matrix<'T>

    /// Get the internal array of values for a dense matrix. This property 
    /// should only be used when interoperating with other matrix libraries.
    member InternalDenseValues : 'T[,]
    /// Get the internal array of values for a sparse matrix. This property 
    /// should only be used when interoperating with other matrix libraries.
    member InternalSparseValues : 'T[]
    /// Get the internal array of row offsets for a sparse matrix. This property 
    /// should only be used when interoperating with other matrix libraries.
    member InternalSparseRowOffsets : int[]
    /// Get the internal array of column values for a sparse matrix. This property 
    /// should only be used when interoperating with other matrix libraries.
    member InternalSparseColumnValues : int[]

    /// Indicates if a matrix uses the sparse representation.
    member IsSparse : bool

    /// Indicates if a matrix uses the dense representation.
    member IsDense : bool

    [<System.Obsolete("This member has been renamed to 'ToArray2D'")>]
    member ToArray2 : unit -> 'T[,]  

/// The type of column vectors. The arithmetic operations on the element type are determined by inspection 
/// on the element type itself
and 

  [<Sealed>]
  Vector<'T> = 


    /// Get the underlying internal array of values for a vector. This property 
    /// should only be used when interoperating with other matrix libraries.
    member InternalValues : 'T[]

    /// Gets the number of entries in a vector
    member Length : int

    /// Gets the number of rows in a vector
    member NumRows : int

    /// Gets an item from a vector
    member Item : int -> 'T with get,set

    /// Gets the element operations for the element type of a vector, if any
    member ElementOps : INumeric<'T>
    
    /// Supports the slicing syntax 'v.[idx1..idx2]'
    member GetSlice      : start:int option * finish:int option -> Vector<'T>

    /// Supports the slicing syntax 'v.[idx1..idx2] <- v2'
    member SetSlice      : start:int option * finish:int option * source:Vector<'T> -> unit

    /// Add two vectors, pointwise
    static member ( +  ) : Vector<'T>    * Vector<'T> -> Vector<'T>

    /// Subtract two vectors, pointwise
    static member ( -  ) : Vector<'T>    * Vector<'T> -> Vector<'T>

    /// Negate a vector
    static member ( ~- ) : Vector<'T>                  -> Vector<'T>

    /// Return the input vector 
    static member ( ~+ ) : Vector<'T>                  -> Vector<'T>

    /// Point-wise multiplication of two vectors.
    static member ( .* ) : Vector<'T>    * Vector<'T> -> Vector<'T>

    /// Multiply each element of a vector by a scalar value.
    static member ( * ) : 'T             * Vector<'T> -> Vector<'T>

    /// Multiply a column vector and a row vector to produce a matrix
    static member ( * )   : Vector<'T>    * RowVector<'T> -> Matrix<'T>

    /// Multiply a vector by a scalar
    static member ( * )   : Vector<'T>    * 'T          -> Vector<'T>

    /// Get the transpose of a vector.
    member Transpose : RowVector<'T>
    
    /// Permute the elements of a vector.
    member Permute : permutation:(int -> int) -> Vector<'T>    
    
    interface IComparable
    interface IStructuralComparable
    interface IStructuralEquatable
    interface IEnumerable<'T>
    override GetHashCode : unit -> int
    override Equals : obj -> bool

    /// Return a new array containing a copy of the elements of a vector
    member ToArray : unit    -> 'T[]

    /// Computes the 2-norm of a vector: sqrt(x.Transpose*x).
    member Norm : float

    /// Create a new matrix that is a copy of a array
    member Copy : unit -> Vector<'T>





/// The type of row vectors. 
and [<Sealed>]
    RowVector<'T> =

    /// Get the underlying internal array of values for a vector. This property 
    /// should only be used when interoperating with other matrix libraries.
    member InternalValues : 'T[]

    // Basic access
    member Length : int
    member NumCols : int
    member Item : int -> 'T with get,set
    member ElementOps : INumeric<'T>

    /// Supports the slicing syntax 'rv.[idx1..idx2]'
    member GetSlice      : start:int option * finish:int option -> RowVector<'T>

    /// Supports the slicing syntax 'rv.[idx1..idx2] <- rv2'
    member SetSlice      : start:int option * finish:int option * source:RowVector<'T> -> unit


    /// Point-wise addition of two row vectors
    static member ( +  )  : RowVector<'T>    * RowVector<'T> -> RowVector<'T>

    /// Point-wise subtraction of two row vectors
    static member ( -  )  : RowVector<'T>    * RowVector<'T> -> RowVector<'T>

    /// Point-wise negation of a row vector
    static member ( ~- )  : RowVector<'T>                  -> RowVector<'T>

    /// Return a row vector, unchanged
    static member ( ~+ )  : RowVector<'T>                  -> RowVector<'T>

    /// Point-wise multiplication of two row vectors
    static member ( .* )  : RowVector<'T> * RowVector<'T>    -> RowVector<'T>
    
    /// Multiply a row vector by a vector
    static member ( * ) : RowVector<'T> * Vector<'T>    -> 'T

    /// Multiply a row vector by a matrix
    static member ( * )   : RowVector<'T> * Matrix<'T>    -> RowVector<'T>

    /// Multiply a row vector by a scalar
    static member ( * )   : RowVector<'T> * 'T            -> RowVector<'T>

    /// Multiply a scalar by a row vector
    static member ( * )  : 'T            * RowVector<'T>    -> RowVector<'T>

    /// Get the transpose of the row vector.
    member Transpose : Vector<'T>
    
    /// Permute the elements of the row vector.
    member Permute : permutation:(int -> int) -> RowVector<'T>  

    interface IComparable
    interface IStructuralComparable
    interface IStructuralEquatable
    interface IEnumerable<'T>
    override GetHashCode : unit -> int
    override Equals : obj -> bool

    /// Return a new array containing a copy of the elements of a vector
    member ToArray : unit    -> 'T[]

    /// Create a new matrix that is a copy of a array
    member Copy : unit -> RowVector<'T>

  
/// The type of floating point matrices
type matrix = Matrix<float>
/// The type of floating point column vectors
type vector = Vector<float>
/// The type of floating point row vectors
type rowvec = RowVector<float>

/// Operations to manipulate floating
/// point matrices. The submodule <c>Matrix.Generic</c> contains a 
/// matching set of operations to manipulate matrix types carrying
/// arbitrary element types.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Matrix =


    /// Get an element of a matrix
    val get : matrix -> int -> int -> float

    /// Set an element of a matrix
    val set : matrix -> int -> int -> float -> unit

    // Creation: general
    val init  : int -> int -> (int -> int -> float) -> matrix

    /// Create a matrix with all entries the given constant
    val create : int -> int -> float -> matrix
    
    /// Create a dense representation matrix with the given entries. 
    val initDense : int -> int -> seq<int * int * float> -> matrix

    /// Create a sparse representation matrix with the given entries. Not all 
    /// operations are available for sparse matrices, and mutation is not permitted.
    /// If an operation on sparse matrices raises a runtime exception then consider 
    /// converting to a dense matrix using to_dense.
    val initSparse : int -> int -> seq<int * int * float> -> matrix

    /// Create a matrix with all entries zero
    val zero     : int -> int          -> matrix
    
    /// Create a square matrix with the constant 1.0 lying on diagonal
    val identity      : int -> matrix

    /// Create a square matrix with a vector lying on diagonal
    val initDiagonal : vector -> matrix
    
    /// Create a matrix from the given data
    val ofList   : float list list     -> matrix

    /// Create a matrix from the given data
    val ofSeq    : seq<#seq<float>>   -> matrix

    /// Create a matrix from the given data
    val ofArray2D : float[,]            -> matrix

    /// Return a new array containing the elements of the given matrix
    val toArray2D : matrix              -> float[,]


    /// Create a 1x1 matrix containing the given value 
    val ofScalar : float               -> matrix
    /// Create a matrix with one row from the given row vector
    val ofRowVector : rowvec              -> matrix
    /// Create a matrix with one column from the given vector
    val ofVector : vector              -> matrix

    /// Return the element at row 0, column 0 of a matrix
    val toScalar : matrix              -> float                
    /// Return the first row of a matrix
    val toRowVector : matrix              -> rowvec                
    /// Return the first column of a matrix
    val toVector : matrix              -> vector
    /// Ensure that a matrix uses dense representation
    val toDense  : matrix              -> matrix

    /// Point-wise maximum element of two matrices
    val cptMax    : matrix -> matrix -> matrix
    
    /// Point-wise minimum element of two matrices
    val cptMin    : matrix -> matrix -> matrix
    
    /// Add two matrices (operator +)
    val add       : matrix -> matrix -> matrix

    /// Dot product
    val dot       : matrix -> matrix -> float

    /// Point-wise exponential of a matrix.
    val cptPow       : matrix -> float -> matrix

    /// Transpose of a matrix. Use also m.Transpose
    val transpose     :           matrix -> matrix

    /// Sum of the diagonal elements of a matrix
    val trace     : matrix -> float
    
    /// Generate a new matrix of the same size as the input with random entries 
    /// drawn from the range 0..aij. Random numbers are generated using a globally 
    /// shared System.Random instance with the initial seed 99.
    val randomize : matrix -> matrix

    /// Sum all the elements of a matrix
    val sum       : matrix -> float

    ///Multiply all the elements of a matrix
    val prod      : matrix -> float

    ///sqrt(sum(x*x)) of all the elements of a matrix
    val norm      : matrix -> float

    /// Check if a predicate holds for all elements of a matrix
    val forall  : (float -> bool) -> matrix -> bool

    /// Check if a predicate holds for at least one element of a matrix
    val exists  : (float -> bool) -> matrix -> bool 

    /// Check if a predicate holds for all elements of a matrix
    val foralli  : (int -> int -> float -> bool) -> matrix -> bool

    /// Check if a predicate holds for at least one element of a matrix
    val existsi  : (int -> int -> float -> bool) -> matrix -> bool 

    /// Fold a function over all elements of a matrix
    val fold      : ('T -> float -> 'T) -> 'T         -> matrix -> 'T

    /// Fold an indexed function over all elements of a matrix
    val foldi      : (int -> int -> 'T -> float -> 'T) -> 'T         -> matrix -> 'T

    /// Fold a function down each column of a matrix
    val foldByCol : ('T -> float -> 'T) -> RowVector<'T> -> matrix -> RowVector<'T>

    /// Fold a function along each row of a matrix
    val foldByRow : ('T -> float -> 'T) -> Vector<'T> -> matrix -> Vector<'T>

    /// Fold a function along a particular column of a matrix
    val foldCol   : ('T -> float -> 'T) -> 'T         -> matrix -> int -> 'T 

    /// Fold a function down a particular row of a matrix
    val foldRow   : ('T -> float -> 'T) -> 'T         -> matrix -> int -> 'T
    
    /// Map a function over each element of a matrix, producing a new matrix
    val map  : (float -> float) -> matrix -> matrix
   
    /// Map the given indexed function over each element of a matrix, producing a new matrix
    val mapi  : (int -> int -> float -> float) -> matrix -> matrix
   
    /// Create a new matrix that is a copy of the given array
    val copy : matrix -> matrix
   
    /// In-place addition, mutating the first matrix argument.
    val inplaceAdd    : matrix -> matrix -> unit

    /// In-place subtraction, mutating the first matrix argument. 
    val inplaceSub    : matrix -> matrix -> unit

    [<Obsolete("Use the '.NonZeroEntries' property instead")>]
    val nonzero_entries : matrix -> seq<int * int * float>         
    [<System.Obsolete("This function has been renamed. Use 'Matrix.inplaceAdd' instead")>]
    val inplace_add    : matrix -> matrix -> unit
    [<System.Obsolete("This function has been renamed. Use 'Matrix.inplaceSub' instead")>]
    val inplace_sub    : matrix -> matrix -> unit
    [<System.Obsolete("This function has been renamed. Use 'Matrix.ofList' instead")>]
    val of_list   : float list list     -> matrix
    [<System.Obsolete("This function has been renamed. Use 'Matrix.ofSeq' instead")>]
    val of_seq    : seq<#seq<float>>   -> matrix
    [<System.Obsolete("This function has been renamed. Use 'Matrix.ofArray2D' instead")>]
    val inline of_array2D : float[,]            -> matrix
    [<System.Obsolete("This function has been renamed. Use 'Matrix.toArray2D' instead")>]
    val inline to_array2D : matrix              -> float[,]
    [<System.Obsolete("This function has been renamed. Use 'Matrix.ofScalar' instead")>]
    val of_scalar : float               -> matrix
    [<System.Obsolete("This function has been renamed. Use 'Matrix.ofRowVector' instead")>]
    val of_rowvec : rowvec              -> matrix
    [<System.Obsolete("This function has been renamed. Use 'Matrix.ofVector' instead")>]
    val of_vector : vector              -> matrix
    [<System.Obsolete("This function has been renamed. Use 'Matrix.toScalar' instead")>]
    val to_scalar : matrix              -> float                
    [<System.Obsolete("This function has been renamed. Use 'Matrix.toRowVector' instead")>]
    val to_rowvec : matrix              -> rowvec                
    [<System.Obsolete("This function has been renamed. Use 'Matrix.toVector' instead")>]
    val to_vector : matrix              -> vector
    [<System.Obsolete("This function has been renamed. Use 'Matrix.toDense' instead")>]
    val to_dense  : matrix              -> matrix
    [<Obsolete("Use the '*' operator instead")>]
    val mul       : matrix -> matrix -> matrix
    [<Obsolete("This function has been renamed to 'initDiagonal'")>]
    val init_diagonal : vector -> matrix
    [<Obsolete("Use the 'Matrix.init' function instead")>]
    val constDiag : int -> float        -> matrix
    [<Obsolete("Use the 'initDiagonal' function instead")>]
    val diag      : vector -> matrix
    [<Obsolete("This function has been renamed to 'initDense'")>]
    val init_dense : int -> int -> seq<int * int * float> -> matrix
    [<Obsolete("This function has been renamed to 'initSparse'")>]
    val init_sparse : int -> int -> seq<int * int * float> -> matrix
    [<Obsolete("Use the '.*' operator instead")>]
    val cptMul    : matrix -> matrix -> matrix
    [<Obsolete("Use the '*' operator instead")>]
    val mulV      : matrix -> vector -> vector
    [<Obsolete("Use the '*' operator instead")>]
    val mulRV     : rowvec -> matrix -> rowvec
    [<Obsolete("Use the '-' operator instead")>]
    val sub       : matrix -> matrix -> matrix
    [<Obsolete("Use the '-' prefix operator instead")>]
    val neg       :           matrix -> matrix
    [<Obsolete("Use the '*' operator instead")>]
    val scale     : float  -> matrix -> matrix
    [<Obsolete("Use the '.Column' method instead")>]
    val getCol  : matrix -> int -> vector
    [<Obsolete("Use the '.Row' method instead")>]
    val getRow  : matrix -> int -> rowvec
    [<Obsolete("Use the '.Columns' method instead")>]
    val getCols : matrix -> start:int -> length:int -> matrix
    [<Obsolete("Use the '.Rows' method instead")>]
    val getRows : matrix -> start:int -> length:int -> matrix
    [<Obsolete("Use the '.Region' method instead")>]
    val getRegion  : matrix -> starti:int -> startj:int -> lengthi:int -> lengthj:int -> matrix
    [<Obsolete("Use the '.Diagonal' method instead")>]
    val getDiagN : matrix -> int    -> vector
    [<Obsolete("Use the '.Diagonal' property instead")>]
    val getDiag  : matrix           -> vector
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
    val inplace_assign       : (int -> int -> float) -> matrix -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
    val inplace_mapi  : (int -> int -> float -> float) -> matrix -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
    val inplace_cptMul : matrix -> matrix -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
    val inplace_scale  : float -> matrix -> unit

    /// Operations to manipulate matrix types carrying
    /// arbitrary element types. The names and types of the operations match those
    /// in the containing module Math.Matrix. 
    ///
    /// The numeric operations on the element type (add, zero etc.) are inferred from the type
    /// argument itself. That is, for some operations 
    /// the element type of a matrix must have an associated instance of INumeric<'T> 
    /// or some more specific numeric association (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
    [<RequireQualifiedAccess>]
    module Generic =

        /// Get an element from a matrix. The indexes are given in row/column order.
        val get   : Matrix<'T> -> int -> int -> 'T
        /// Set an element in a matrix. The indexes are given in row/column order.
        val set   : Matrix<'T> -> int -> int -> 'T -> unit

        /// Create a matrix from the given data  
        val ofList      : 'T list list       -> Matrix<'T>

        /// Create a matrix from the given data  
        val ofSeq       : seq<#seq<'T>>     -> Matrix<'T>

        /// Create a matrix from the given data  
        val ofArray2D    : 'T[,]              -> Matrix<'T>

        /// Return a new array containing the elements of the given matrix
        val toArray2D : Matrix<'T>         -> 'T[,]  

        /// Create a matrix containing the given value at every element.
        val create     : int -> int -> 'T   -> Matrix<'T>

        /// Create a dense matrix from the given sequence of elements
        val initDense : int -> int -> seq<int * int * 'T>  -> Matrix<'T>

        /// Create a sparse matrix from the given sequence of elements
        val initSparse : int -> int -> seq<int * int * 'T>  -> Matrix<'T>

        /// Create a 1x1 matrix containing the given value 
        val ofScalar    : 'T                 -> Matrix<'T>

        /// Create a matrix from a row vector
        val ofRowVector  : RowVector<'T>     -> Matrix<'T>
        /// Create a matrix from a column vector
        val ofVector  : Vector<'T>        -> Matrix<'T>

        /// Get the element at column zero, row zero
        val toScalar : Matrix<'T>         -> 'T                
        /// Extract the first row of a matrix
        val toRowVector : Matrix<'T>         -> RowVector<'T>                 
        /// Extract the first column of a matrix
        val toVector : Matrix<'T>         -> Vector<'T> 


        /// Create a matrix using a function to compute the item at each index.
        val init : int -> int -> (int -> int -> 'T) -> Matrix<'T>

        /// Create a matrix using a function to compute the item at each index.
        /// The element type of a matrix must have an associated instance of INumeric<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
        /// The function is passed the dictionary of associated operations in addition to the index pair.
        val initNumeric  : int -> int -> (INumeric<'T> -> int -> int -> 'T)        -> Matrix<'T>

        /// Create a matrix containing the zero element at each index.
        /// The element type of a matrix must have an associated instance of INumeric<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
        val zero      : int -> int         -> Matrix<'T>

        /// Create a square matrix with the one for the element type lying on diagonal
        /// The element type of a matrix must have an associated instance of INumeric<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
        val identity      : int -> Matrix<'T>

        /// Create a matrix containing the given vector along the diagonal.
        /// The element type of a matrix must have an associated instance of INumeric<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
        val initDiagonal : Vector<'T>        -> Matrix<'T>

        ///Take the pointwise maximum of two matrices
        val cptMax    : Matrix<'T> -> Matrix<'T> -> Matrix<'T> when 'T : comparison

        ///Take the pointwise maximum of two matrices
        val cptMin    : Matrix<'T> -> Matrix<'T> -> Matrix<'T> when 'T : comparison

        /// Sum of the point-wise multiple of the two matrices.
        /// The element type of a matrix must have an associated instance of INumeric<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
        val dot       : Matrix<'T> -> Matrix<'T> -> 'T


        /// Return a new matrix which is the transpose of the input matrix
        val transpose : Matrix<'T> -> Matrix<'T>

        /// Compute the sum of the diagonal of a square matrix
        val trace     : Matrix<'T> -> 'T
        /// Compute the sum of the elements in a matrix
        val sum       : Matrix<'T> -> 'T
        /// Compute the product of the elements in a matrix
        val prod      : Matrix<'T> -> 'T
        /// Returns sqrt(sum(norm(x)*(norm(x))) of all the elements of a matrix.
        /// The element type of a matrix must have an associated instance of INormFloat<'T> (see <c>GlobalAssociations</c>) ((else NotSupportedException)).
        val norm      : Matrix<'T> -> float

        /// Fold a function over all elements of a matrix
        val fold  : folder:('State -> 'T -> 'State) -> 'State -> Matrix<'T> -> 'State

        /// Fold an indexed function over all elements of a matrix
        val foldi  : (int -> int -> 'State -> 'T -> 'State) -> 'State -> Matrix<'T> -> 'State

        /// Return true if a predicate returns true for all values in a matrix
        val forall: ('T -> bool)           -> Matrix<'T> -> bool

        /// Return true if a predicate returns true for some value in a matrix
        val exists: ('T -> bool)           -> Matrix<'T> -> bool 

        /// Return true if an indexed predicate returns true for all values in a matrix
        val foralli: (int -> int -> 'T -> bool)           -> Matrix<'T> -> bool

        /// Return true if an indexed predicate returns true for some value in a matrix
        val existsi: (int -> int -> 'T -> bool)           -> Matrix<'T> -> bool 

        /// Create a new matrix that is a copy of the given array
        val copy : Matrix<'T> -> Matrix<'T>
   
        /// Map a function over a matrix
        val map   : ('T -> 'T) -> Matrix<'T> -> Matrix<'T>

        /// Map the given position-indexed function over a matrix
        val mapi  : (int -> int -> 'T -> 'T) -> Matrix<'T> -> Matrix<'T>

        /// Add the second matrix to the first by, mutating the first
        val inplaceAdd    : Matrix<'T> -> Matrix<'T> -> unit
        /// Subtract the second matrix from the first, by mutating the first
        val inplaceSub    : Matrix<'T> -> Matrix<'T> -> unit

        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.inplaceAdd' instead")>]
        val inplace_add    : Matrix<'T> -> Matrix<'T> -> unit
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.inplaceSub' instead")>]
        val inplace_sub    : Matrix<'T> -> Matrix<'T> -> unit
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.ofList' instead")>]
        val of_list      : 'T list list       -> Matrix<'T>
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.ofSeq' instead")>]
        val of_seq       : seq<#seq<'T>>     -> Matrix<'T>
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.ofArray2D' instead")>]
        val inline of_array2D    : 'T[,]              -> Matrix<'T>
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.toArray2D' instead")>]
        val inline to_array2D : Matrix<'T>         -> 'T[,]  
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.ofScalar' instead")>]
        val of_scalar    : 'T                 -> Matrix<'T>
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.ofRowVector' instead")>]
        val of_rowvec  : RowVector<'T>     -> Matrix<'T>
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.ofVector' instead")>]
        val of_vector  : Vector<'T>        -> Matrix<'T>
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.toScalar' instead")>]
        val to_scalar : Matrix<'T>         -> 'T                
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.toRowVector' instead")>]
        val to_rowvec : Matrix<'T>         -> RowVector<'T>                 
        [<System.Obsolete("This function has been renamed. Use 'Matrix.Generic.toVector' instead")>]
        val to_vector : Matrix<'T>         -> Vector<'T> 
        [<Obsolete("This function has been renamed to 'initDiagonal'")>]
        val init_diagonal : Vector<'T>        -> Matrix<'T>
        [<Obsolete("This function has been renamed to 'initDense'")>]
        val init_dense : int -> int -> seq<int * int * 'T>  -> Matrix<'T>
        [<Obsolete("This function has been renamed to 'initSparse'")>]
        val init_sparse : int -> int -> seq<int * int * 'T>  -> Matrix<'T>
        [<Obsolete("Use the '.NonZeroEntries' property instead")>]
        val nonzero_entries : Matrix<'T> -> seq<int * int * 'T> 
        [<Obsolete("Use the 'Matrix.Generic.init' function instead")>]
        val constDiag : int -> 'T          -> Matrix<'T>
        [<Obsolete("Use the '+' operator instead")>]
        val add       : Matrix<'T> -> Matrix<'T> -> Matrix<'T>
        [<Obsolete("Use the 'initDiagonal' function instead")>]
        val diag      : Vector<'T>        -> Matrix<'T>
        [<Obsolete("Use the '*' operator instead")>]
        val mul       : Matrix<'T> -> Matrix<'T> -> Matrix<'T>
        [<Obsolete("Use the '*' operator instead")>]
        val mulV      : Matrix<'T> -> Vector<'T> -> Vector<'T>
        [<Obsolete("Use the '*' operator instead")>]
        val mulRV     : RowVector<'T> -> Matrix<'T> -> RowVector<'T>
        [<Obsolete("Use the '.*' operator instead")>]
        val cptMul    : Matrix<'T> -> Matrix<'T> -> Matrix<'T>
        [<Obsolete("Use the '-' operator instead")>]
        val sub       : Matrix<'T> -> Matrix<'T> -> Matrix<'T>
        [<Obsolete("Use the '-' prefix operator instead")>]
        val neg       : Matrix<'T> -> Matrix<'T>
        [<Obsolete("Use the '*' operator instead")>]
        val scale     : 'T  -> Matrix<'T> -> Matrix<'T>
        [<Obsolete("Use the '.Column' method instead")>]
        val getCol  : Matrix<'T> -> int -> Vector<'T>
        [<Obsolete("Use the '.Row' method instead")>]
        val getRow  : Matrix<'T> -> int -> RowVector<'T>
        [<Obsolete("Use the '.Columns' method instead")>]
        val getCols : Matrix<'T> -> start:int -> length:int -> Matrix<'T>
        [<Obsolete("Use the '.Rows' method instead")>]
        val getRows : Matrix<'T> -> start:int -> length:int -> Matrix<'T>
        [<Obsolete("Use the '.Region' method instead")>]
        val getRegion  : Matrix<'T> -> starti:int -> startj:int -> lengthi:int -> lengthj:int -> Matrix<'T>
        [<Obsolete("Use the '.Diagonal' method instead")>]
        val getDiagN : Matrix<'T> -> int    -> Vector<'T>
        [<Obsolete("Use the '.Diagonal' property instead")>]
        val getDiag  : Matrix<'T>           -> Vector<'T>
        [<Obsolete("This function will be removed in a future revision of this library. Just use 'compare m1 m2' instead")>]
        val compare  : Matrix<'T> -> Matrix<'T> -> int
        [<Obsolete("This function will be removed in a future revision of this library. Just use 'hash m' instead")>]
        val hash     : Matrix<'T> -> int
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
        val inplace_assign       : (int -> int -> 'T) -> Matrix<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
        val inplace_mapi  : (int -> int -> 'T -> 'T) -> Matrix<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
        val inplace_cptMul : Matrix<'T> -> Matrix<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the matrix directly instead using 'm.[i,j] <- v'")>]
        val inplace_scale  : 'T -> Matrix<'T> -> unit


/// Operations to manipulate floating
/// point column vectors. The submodule VectorOps.Generic contains a 
/// matching set of operations to manipulate column vectors carrying
/// arbitrary element types.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Vector =

    /// Get an element of a column vector
    val get   : vector -> int -> float

    /// Set an element of a column vector
    val set   : vector -> int -> float -> unit

    /// Get the dimensions (number of rows) of a column vector. Identical to <c>nrows</c>
    val length   : vector -> int 

    /// Create a vector of a fixed length using a function to compute the initial element values
    val init  : int ->        (int -> float)        -> vector

    /// Create a vector from a list of numbers
    val ofList   : float list          -> vector
    
    /// Create a vector from a sequence of numbers
    val ofSeq    : seq<float>         -> vector

    /// Create a vector from an array of double precision floats
    val ofArray  : float array         -> vector

    /// Return a new array containing a copy of the elements of the given vector
    val toArray  : vector           -> float array
    
    /// Create a 1-element vector
    val ofScalar  : float              -> vector

    /// Generate a vector of the given length where each entry contains the given value
    val create    : int        -> float -> vector
    
    /// Return a vector of the given length where every entry is zero.
    val zero     : int                 -> vector
    
    /// Create a vector that represents a mesh over the given range
    /// e.g. rangef (-1.0) 0.5 1.0 = vector [ -1.0; -0.5; 0.0; 0.5; 1.0]
    val rangef : float -> float -> float -> vector
    
    /// Create a vector that represents a integral mesh over the given range
    /// e.g. range 1 5 = vector [ 1.;2.;3.;4.;5. ]
    val range  : int -> int              -> vector
      
    ///Dot product
    val dot       : vector -> vector -> float

    ///Point-wise exponential of a vector.
    val cptPow    : vector -> float -> vector

    ///Transpose of a matrix. Use also m.Transpose
    val transpose     :           vector -> rowvec

    ///Sum all the elements of a vector
    val sum       : vector -> float

    ///Multiply all the elements of a matrix
    val prod      : vector -> float

    /// Computes the 2-norm of a vector: sqrt(x.Transpose*x).
    val norm      : vector -> float

    /// Return true if a predicate returns true for all values in a vector
    val forall  : (float -> bool) -> vector -> bool

    /// Return true if a predicate returns true for some value in a vector
    val exists  : (float -> bool) -> vector -> bool 

    /// Return true if an indexed predicate returns true for all values in a vector
    val foralli  : (int ->        float -> bool) -> vector -> bool

    /// Return true if an indexed predicate returns true for some value in a vector
    val existsi  : (int ->        float -> bool) -> vector -> bool 

    /// Fold a function over all elements of a vector
    val fold      : ('T -> float -> 'T) -> 'T         -> vector -> 'T

    /// Fold an indexed function over all elements of a vector
    val foldi      : (int -> 'T -> float -> 'T) -> 'T         -> vector -> 'T

    /// Copy a vector
    val copy : vector -> vector
    
    /// Map a function over each element of a vector
    val map  : (float -> float) -> vector -> vector
   
    /// Map an indexed function over each element of a vector
    val mapi  : (int        -> float -> float) -> vector -> vector
   
    [<System.Obsolete("This function has been renamed. Use 'Vector.ofList' instead")>]
    val of_list   : float list          -> vector
    [<System.Obsolete("This function has been renamed. Use 'Vector.ofSeq' instead")>]
    val of_seq    : seq<float>         -> vector
    [<System.Obsolete("This function has been renamed. Use 'Vector.ofArray' instead")>]
    val of_array  : float array         -> vector
    [<System.Obsolete("This function has been renamed. Use 'Vector.toArray' instead")>]
    val to_array  : vector           -> float array
    [<System.Obsolete("This function has been renamed. Use 'Vector.ofScalar' instead")>]
    val of_scalar  : float              -> vector
    [<Obsolete("Use the '+' operator instead")>]
    val add       : vector -> vector -> vector
    [<Obsolete("Use the '-' operator instead")>]
    val sub       : vector -> vector -> vector
    [<Obsolete("Use the '-' prefix operator instead")>]
    val neg       :           vector -> vector
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
    val inplace_assign       : (int ->        float) -> vector -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
    val inplace_mapi  : (int ->        float -> float) -> vector -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
    val inplace_add    : vector -> vector -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
    val inplace_sub    : vector -> vector -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
    val inplace_cptMul : vector -> vector -> unit
    [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
    val inplace_scale  : float -> vector -> unit
    [<Obsolete("Use the '.*' operator instead")>]
    val cptMul    : vector -> vector -> vector
    [<Obsolete("Use the '*' operator instead")>]
    val scale     : float  -> vector -> vector

    
    /// Operations to manipulate column vectors carrying
    /// arbitrary element types. 
    module Generic =
        // Accessors

        /// Get an element of a column vector
        val get   : Vector<'T> -> int -> 'T

        /// Set an element of a column vector
        val set   : Vector<'T> -> int -> 'T -> unit

        /// Get the dimensions (number of rows) of a column vector. Identical to <c>nrows</c>
        val length   : Vector<'T> -> int 

        /// Creation: general
        val init  : int ->        (int -> 'T)        -> Vector<'T>

        /// Creation: useful when the element type has associated operations.
        val initNumeric  : int ->        (INumeric<'T> -> int -> 'T)        -> Vector<'T>

        /// Create a vector from a list of numbers
        val ofList   : 'T list          -> Vector<'T>
      
        /// Create a vector from a sequence of numbers
        val ofSeq    : seq<'T>         -> Vector<'T>

        /// Create a 1-element vector
        val ofScalar  : 'T              -> Vector<'T>

        /// Create a vector from an array of elements
        val ofArray  : 'T[]         -> Vector<'T>

        /// Return a new array containing a copy of the elements of the given vector
        val toArray  : Vector<'T>    -> 'T[]

        /// Generate a vector of the given length where each entry contains the given value
        val create    : int        -> 'T -> Vector<'T>
      
        /// Return a vector of the given length where every entry is zero.
        val zero     : int                 -> Vector<'T>
      
        ///Take the pointwise maximum of two vectors
        val cptMax    : Vector<'T> -> Vector<'T> -> Vector<'T> when 'T : comparison

        ///Take the pointwise minimum of two vectors
        val cptMin    : Vector<'T> -> Vector<'T> -> Vector<'T> when 'T : comparison

        ///Dot product
        val dot       : Vector<'T> -> Vector<'T> -> 'T

        ///Sum all the elements of a vector
        val sum       : Vector<'T> -> 'T
      
        ///Multiply all the elements of a matrix
        val prod      : Vector<'T> -> 'T

        /// Computes the 2-norm of a vector: sqrt(x.Transpose*x).
        val norm      : Vector<'T> -> float

        /// Return true if a predicate returns true for all values in a vector
        val forall  : predicate:('T -> bool) -> Vector<'T> -> bool

        /// Return true if a predicate returns true for some value in a vector
        val exists  : predicate:('T -> bool) -> Vector<'T> -> bool 

        /// Return true if an indexed predicate returns true for all values in a vector
        val foralli  : (int -> 'T -> bool) -> Vector<'T> -> bool

        /// Return true if an indexed predicate returns true for some value in a vector
        val existsi  : (int -> 'T -> bool) -> Vector<'T> -> bool 

        /// Fold a function over all elements of a vector
        val fold      : folder:('State -> 'T -> 'State) -> 'State -> Vector<'T> -> 'State

        /// Fold an indexed function over all elements of a vector
        val foldi      : (int -> 'State -> 'T -> 'State) -> 'State -> Vector<'T> -> 'State

        /// Copy the vector
        val copy: Vector<'T> -> Vector<'T>
        
        /// Map a function over each element of a vector
        val map  : ('T -> 'T) -> Vector<'T> -> Vector<'T>
     
        /// Map an indexed function over each element of a vector
        val mapi  : (int        -> 'T -> 'T) -> Vector<'T> -> Vector<'T>
     

        [<System.Obsolete("This function has been renamed. Use 'Vector.ofList' instead")>]
        val of_list   : 'T list          -> Vector<'T>
        [<System.Obsolete("This function has been renamed. Use 'Vector.ofSeq' instead")>]
        val of_seq    : seq<'T>         -> Vector<'T>
        [<System.Obsolete("This function has been renamed. Use 'Vector.ofScalar' instead")>]
        val of_scalar  : 'T              -> Vector<'T>
        [<System.Obsolete("This function has been renamed. Use 'Vector.ofArray' instead")>]
        val of_array  : 'T[]         -> Vector<'T>
        [<System.Obsolete("This function has been renamed. Use 'Vector.toArray' instead")>]
        val to_array  : Vector<'T>    -> 'T[]
        [<Obsolete("Use the .* operator instead")>]
        val cptMul    : Vector<'T> -> Vector<'T> -> Vector<'T>      
        [<Obsolete("Use the 'vector.Transpose' property instead")>]
        val transpose : Vector<'T> -> RowVector<'T>
        [<Obsolete("Use the '+' operator instead")>]
        val add       : Vector<'T> -> Vector<'T> -> Vector<'T>
        [<Obsolete("Use the '-' operator instead")>]
        val sub       : Vector<'T> -> Vector<'T> -> Vector<'T>
        [<Obsolete("Use the '-' prefix operator instead")>]
        val neg       : Vector<'T> -> Vector<'T>
        [<Obsolete("Use the '*' operator instead")>]
        val scale     : 'T  -> Vector<'T> -> Vector<'T>
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
        val inplace_assign : (int -> 'T) -> Vector<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
        val inplace_mapi  : (int -> 'T -> 'T) -> Vector<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
        val inplace_add    : Vector<'T> -> Vector<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
        val inplace_sub    : Vector<'T> -> Vector<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
        val inplace_cptMul : Vector<'T> -> Vector<'T> -> unit
        [<Obsolete("This function will be removed in a future revision of this library. Set the elements of the vector directly instead using 'vec.[i] <- x'")>]
        val inplace_scale  : 'T -> Vector<'T> -> unit



/// Operations to manipulate floating
/// point row vectors. These are included for completeness and are
/// nearly always transposed to column vectors.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module RowVector =

    /// Get an element of a column vector
    val get   : rowvec -> int -> float

    /// Set an element of a column rowvec
    val set   : rowvec -> int -> float -> unit

    /// Get the dimensions (number of rows) of a column rowvec. 
    val length   : rowvec -> int 

    /// Create by constant initialization
    val create : int -> float  -> rowvec

    /// Create by comprehension
    val init : int ->  (int -> float) -> rowvec

    /// Return a vector of the given length where every entry is zero.
    val zero     : int -> rowvec

    // Transpose the row vector
    val transpose : rowvec -> vector

    // Copy the row vector
    val copy : rowvec -> rowvec

    /// Create a vector from a list of numbers
    val ofList   : float list          -> rowvec

    /// Create a vector from a sequence of numbers
    val ofSeq    : seq<float>         -> rowvec

    /// Create a vector from an array of double precision floats
    val ofArray  : float array         -> rowvec

    /// Return a new array containing a copy of the elements of the given vector
    val toArray  : rowvec              -> float array
    
    [<System.Obsolete("This function has been renamed. Use 'RowVector.ofList' instead")>]
    val of_list   : float list          -> rowvec
    [<System.Obsolete("This function has been renamed. Use 'RowVector.ofSeq' instead")>]
    val of_seq    : seq<float>         -> rowvec
    [<System.Obsolete("This function has been renamed. Use 'RowVector.ofArray' instead")>]
    val of_array  : float array         -> rowvec
    [<System.Obsolete("This function has been renamed. Use 'RowVector.toArray' instead")>]
    val to_array  : rowvec              -> float array
    
    
    /// Operations to manipulate row vectors types carrying
    /// arbitrary element types. 
    module Generic =
        // Accessors

        /// Get an element from a column vector. 
        val get   : RowVector<'T> -> int -> 'T
        /// Set an element in a column vector. 
        val set   : RowVector<'T> -> int -> 'T -> unit
        /// Get the number of rows in a column vector. 
        val length: RowVector<'T> -> int 
        /// Transpose the row vector
        val transpose        : RowVector<'T>  -> Vector<'T> 
        /// Create by comprehension
        val init       : int ->        (int -> 'T)        -> RowVector<'T> 
        /// Create by constant initialization
        val create       : int ->      'T  -> RowVector<'T> 
        /// Return a vector of the given length where every entry is zero.
        val zero     : int                 -> RowVector<'T>
        /// Create a row vector from a list of elements
        val ofList   : 'T list          -> RowVector<'T> 
        /// Create a row vector from a sequence of elements
        val ofSeq    : seq<'T>         -> RowVector<'T> 

        /// Create a row vector from an array of elements
        val ofArray  : 'T[]         -> RowVector<'T>

        /// Return a new array containing a copy of the elements of the given vector
        val toArray  : RowVector<'T>    -> 'T[]         

        // Copy the row vector
        val copy    :   RowVector<'T> -> RowVector<'T>

        [<System.Obsolete("This function has been renamed. Use 'RowVector.Generic.ofList' instead")>]
        val of_list   : 'T list          -> RowVector<'T> 
        [<System.Obsolete("This function has been renamed. Use 'RowVector.Generic.ofSeq' instead")>]
        val of_seq    : seq<'T>         -> RowVector<'T> 
        [<System.Obsolete("This function has been renamed. Use 'RowVector.Generic.ofArray' instead")>]
        val of_array  : 'T[]         -> RowVector<'T>
        [<System.Obsolete("This function has been renamed. Use 'RowVector.Generic.toArray' instead")>]
        val to_array  : RowVector<'T>    -> 'T[]         

namespace Microsoft.FSharp.Core

    /// The type of floating-point matrices. See Microsoft.FSharp.Math
    type matrix = Microsoft.FSharp.Math.matrix
    /// The type of floating-point vectors. See Microsoft.FSharp.Math
    type vector = Microsoft.FSharp.Math.vector
    /// The type of floating-point row vectors. See Microsoft.FSharp.Math
    type rowvec = Microsoft.FSharp.Math.rowvec


    [<AutoOpen>]
    module MatrixTopLevelOperators = 
        /// Builds a matrix from a sequence of sequence of floats.
        val matrix : seq<#seq<float>> -> matrix
        /// Builds a (column) vector from a sequence of floats.
        val vector : seq<float> -> vector
        /// Builds a (row) vector from a sequence of floats.
        val rowvec : seq<float> -> rowvec

