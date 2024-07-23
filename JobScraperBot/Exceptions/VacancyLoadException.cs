namespace JobScraperBot.Exceptions
{
    public class VacancyLoadException : Exception
    {
        public override string Message => base.Message + $" ,Request String: {this.RequestString}";

        public string RequestString { get; }

        public VacancyLoadException(string message, string requestString)
            : base(message)
        {
            this.RequestString = requestString;
        }
    }
}
