namespace JobScraperBot.State
{
    public interface IStateMachine<out T>
    {
        T State { get; }

        bool MoveNext();
    }
}
