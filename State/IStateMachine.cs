namespace JobScraperBot.State
{
    internal interface IStateMachine<out T>
    {
        T State { get; }

        bool MoveNext();
    }
}
