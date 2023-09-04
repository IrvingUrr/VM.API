namespace VM.Core.Extensions
{
    public static class StringExtensionMethods
    {   
        public static string RemoveStringCharacters(this string str)
        {
            var allowedChars = "01234567890.";
            return new string(str.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
