namespace XamlDiff.Library.Models;

public sealed class Error(string message) {

    public string Message { get; } = message;
}
