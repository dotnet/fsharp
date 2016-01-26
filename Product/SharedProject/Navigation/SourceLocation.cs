/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Globalization;

namespace Microsoft.VisualStudioTools.Parsing {
    /// <summary>
    /// Represents a location in source code.
    /// </summary>
    [Serializable]
    public struct SourceLocation {
        // TODO: remove index
        private readonly int _index;

        private readonly int _line;
        private readonly int _column;

        /// <summary>
        /// Creates a new source location.
        /// </summary>
        /// <param name="index">The index in the source stream the location represents (0-based).</param>
        /// <param name="line">The line in the source stream the location represents (1-based).</param>
        /// <param name="column">The column in the source stream the location represents (1-based).</param>
        public SourceLocation(int index, int line, int column) {
            ValidateLocation(index, line, column);

            _index = index;
            _line = line;
            _column = column;
        }

        private static void ValidateLocation(int index, int line, int column) {
            if (index < 0) {
                throw ErrorOutOfRange("index", 0);
            }
            if (line < 1) {
                throw ErrorOutOfRange("line", 1);
            }
            if (column < 1) {
                throw ErrorOutOfRange("column", 1);
            }
        }

        private static Exception ErrorOutOfRange(object p0, object p1) {
            return new ArgumentOutOfRangeException(string.Format("{0} must be greater than or equal to {1}", p0, p1));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        private SourceLocation(int index, int line, int column, bool noChecks) {
            _index = index;
            _line = line;
            _column = column;
        }

        /// <summary>
        /// The index in the source stream the location represents (0-based).
        /// </summary>
        public int Index {
            get { return _index; }
        }

        /// <summary>
        /// The line in the source stream the location represents (1-based).
        /// </summary>
        public int Line {
            get { return _line; }
        }

        /// <summary>
        /// The column in the source stream the location represents (1-based).
        /// </summary>
        public int Column {
            get { return _column; }
        }

        /// <summary>
        /// Compares two specified location values to see if they are equal.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>True if the locations are the same, False otherwise.</returns>
        public static bool operator ==(SourceLocation left, SourceLocation right) {
            return left._index == right._index && left._line == right._line && left._column == right._column;
        }

        /// <summary>
        /// Compares two specified location values to see if they are not equal.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>True if the locations are not the same, False otherwise.</returns>
        public static bool operator !=(SourceLocation left, SourceLocation right) {
            return left._index != right._index || left._line != right._line || left._column != right._column;
        }

        /// <summary>
        /// Compares two specified location values to see if one is before the other.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>True if the first location is before the other location, False otherwise.</returns>
        public static bool operator <(SourceLocation left, SourceLocation right) {
            return left._index < right._index;
        }

        /// <summary>
        /// Compares two specified location values to see if one is after the other.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>True if the first location is after the other location, False otherwise.</returns>
        public static bool operator >(SourceLocation left, SourceLocation right) {
            return left._index > right._index;
        }

        /// <summary>
        /// Compares two specified location values to see if one is before or the same as the other.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>True if the first location is before or the same as the other location, False otherwise.</returns>
        public static bool operator <=(SourceLocation left, SourceLocation right) {
            return left._index <= right._index;
        }

        /// <summary>
        /// Compares two specified location values to see if one is after or the same as the other.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>True if the first location is after or the same as the other location, False otherwise.</returns>
        public static bool operator >=(SourceLocation left, SourceLocation right) {
            return left._index >= right._index;
        }

        /// <summary>
        /// Compares two specified location values.
        /// </summary>
        /// <param name="left">One location to compare.</param>
        /// <param name="right">The other location to compare.</param>
        /// <returns>0 if the locations are equal, -1 if the left one is less than the right one, 1 otherwise.</returns>
        public static int Compare(SourceLocation left, SourceLocation right) {
            if (left < right)
                return -1;
            if (right > left)
                return 1;

            return 0;
        }

        /// <summary>
        /// A location that is valid but represents no location at all.
        /// </summary>
        public static readonly SourceLocation None = new SourceLocation(0, 0xfeefee, 0, true);

        /// <summary>
        /// An invalid location.
        /// </summary>
        public static readonly SourceLocation Invalid = new SourceLocation(0, 0, 0, true);

        /// <summary>
        /// A minimal valid location.
        /// </summary>
        public static readonly SourceLocation MinValue = new SourceLocation(0, 1, 1);

        /// <summary>
        /// Whether the location is a valid location.
        /// </summary>
        /// <returns>True if the location is valid, False otherwise.</returns>
        public bool IsValid {
            get {
                return this._line != 0 && this._column != 0;
            }
        }

        public override bool Equals(object obj) {
            if (!(obj is SourceLocation))
                return false;

            SourceLocation other = (SourceLocation)obj;
            return other._index == _index && other._line == _line && other._column == _column;
        }

        public override int GetHashCode() {
            return (_line << 16) ^ _column;
        }

        public override string ToString() {
            return "(" + _line + "," + _column + ")";
        }

        internal string ToDebugString() {
            return String.Format(CultureInfo.CurrentCulture, "({0},{1},{2})", _index, _line, _column);
        }
    }
}