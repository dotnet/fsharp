namespace CsLib
{
    public ref struct RefField<T>
    {
        private readonly ref T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public RefField(ref T value)
        {
            this._value = ref value;
        }

        public ref T GetPinnableReference() => ref _value;
    }
}