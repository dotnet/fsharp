using System;
using System.Collections.Generic;
using System.Globalization;

namespace TestLib {
    public class T {
        public T() {
        }

        public int ValueTypeOptArg(int x = 100) {
            return x;
        }

        public int? NullableOptArgNullDefault(int? x = null) {
            return x;
        }

        public double? NullableOptArgWithDefault(double? x = 5.7) {
            return x;
        }

        public string NullOptArg(string x = null) {
            return x;
        }

        public string NonNullOptArg(string x = "abc") {
            return x;
        }

        public List<T> GenericOptArg<T>(List<T> x = null) {
            return x;
        }

        public string ComboOptionals<T>(string required, string a = null, string b = "abc", int c = 100, int? d = 200, double? e = null, List<T> f = null) {
            var result = String.Format(CultureInfo.InvariantCulture, "[{0}] [{1}] [{2}] [{3}] [{4}] [{5}]", a, b, c, d, e, f);
            return result;
        }
    }
}