namespace Alpha.SharedLibrary.Realisation
{
    public partial class ErrorCodes
    {
        public const int NoError = -1;
        public const int Common = 0; // Appears on non api errors (no internet connection, parse errors)

        public int ErrorCode { get; private set; }
        public object? ErrorData { get; private set; } = null;

        public ErrorCodes(int errorCode) { ErrorCode = errorCode; }

        public ErrorCodes(int errorCode, object? errorData) : this(errorCode) { ErrorData = errorData; }
    }
}
