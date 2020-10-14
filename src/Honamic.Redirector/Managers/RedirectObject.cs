namespace Honamic.Redirector
{
    public class RedirectObject
    {
        public string Id { get; set; }

        public RedirectType Type { get; set; }

        public string Path { get; set; }

        public string Destination { get; set; }

        public decimal Order { get; set; }

        public int? HttpCode { get; set; }
    }

    public enum RedirectType
    {
        Path,
        Regex
    }
}
