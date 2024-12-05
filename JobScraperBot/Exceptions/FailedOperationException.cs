namespace JobScraperBot.Exceptions
{
    public class FailedOperationException : Exception
    {
        public long ChatId { get; }

        public FailedOperationException(long chatId, string message, Exception innerEx)
            : base(message, innerEx)
        {
            this.ChatId = chatId;
        }
    }
}
