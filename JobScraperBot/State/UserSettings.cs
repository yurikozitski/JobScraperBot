namespace JobScraperBot.State
{
    public class UserSettings
    {
        public string Stack { get; set; } = default!;

        public string Grade { get; set; } = default!;

        public string? Type { get; set; }

        public void Reset()
        {
            this.Stack = default!;
            this.Grade = default!;
            this.Type = default!;
        }

        public override string ToString() =>
            $"{this.Stack}, {this.Grade}, {this.Type}";
    }
}
