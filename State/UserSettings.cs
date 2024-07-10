namespace JobScraperBot.State
{
    internal class UserSettings
    {
        public string Stack { get; set; } = default!;

        public string Grade { get; set; } = default!;

        public override string ToString() =>
            $"{this.Stack}, {this.Grade}";
    }
}
