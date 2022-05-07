namespace EasySourcing.DependencyInjection;

public class EventSourcingOptions
{
    public int TakeEachSnapshotVersion { get; set; } = 5;
}