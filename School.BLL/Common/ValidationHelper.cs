namespace School.BLL.Common
{
    public static class ValidationHelper
    {
        public static void ValidateId(int id, string message = "ID is invalid.")
        {
            if (id <= 0)
                throw new ArgumentException(message, nameof(id));
        }

        public static string ValidateString(string value, string paramName, int minLength = 1, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} is required.", paramName);

            value = value.Trim();

            if (value.Length < minLength || value.Length > maxLength)
                throw new ArgumentException($"{paramName} must be between {minLength} and {maxLength} characters.", paramName);

            return value;
        }
    }
}