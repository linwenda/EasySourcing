namespace EasySourcing.Abstraction;

public interface IStoreOptions
{
    int TakeSnapshotVersion { get; set; }
}