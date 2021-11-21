namespace Alpha.SharedLibrary.Realisation
{
    using Alpha.SharedLibrary.Interfaces;

    public class SafeReturn<Value> : IBase
    {
        public static bool operator !(SafeReturn<Value> @this) { return @this == null || !@this.IsSuccess(); }

        public Value? Result { get; private set; }
        public ErrorCodes Error { get; private set; } = new ErrorCodes(ErrorCodes.NoError);

        SafeReturn() { }
        SafeReturn(Value DefaultValue) { Result = DefaultValue; }

        public SafeReturn(Value val, int error, object? errorData) : this(val) { Error = new ErrorCodes(error, errorData); }

        public SafeReturn(Value val, int error) : this(val, error, null) { }

        public static SafeReturn<Value> AsEmpty() => new SafeReturn<Value>();
        
        public static SafeReturn<Value> AsEmpty(Value default_value) => new SafeReturn<Value>(default_value);

        public bool IsSuccess() => !EqualityComparer<Value>.Default.Equals(Result, default(Value)) && Error.ErrorCode == ErrorCodes.NoError;
    }
}
