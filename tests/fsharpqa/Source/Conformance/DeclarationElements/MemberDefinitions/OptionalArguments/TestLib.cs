namespace TestLib {
    public class T {
        public T() {
        }

        public string ValueTypeOptArg(int x = 0) {
            return "1";
        }

        public string NullableOptArg(int? x = null) {
            return "1";
        }
    
        public int NullOptArg(string x = null) {
            return 1;
        }

        public int NonNullOptArg(string x = "") {
            return 1;
        }
    }
}