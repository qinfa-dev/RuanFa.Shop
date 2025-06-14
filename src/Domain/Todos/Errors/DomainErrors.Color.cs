using ErrorOr;

public static partial class DomainErrors
{
    // Colour Flow
    public static class Colour
    {
        public static class System
        {
            public static Error NotSupported(string colorCode) => Error.Failure(
                code: "Colour.NotSupported",
                description: $"The colour with code {colorCode} is not supported.");

            public static Error EmptyCode => Error.Failure(
                code: "Colour.EmptyCode",
                description: "The colour code cannot be empty.");
        }
    }
}
