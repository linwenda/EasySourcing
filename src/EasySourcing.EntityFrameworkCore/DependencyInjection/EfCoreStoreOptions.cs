using EasySourcing.Abstraction;

namespace EasySourcing.EntityFrameworkCore.DependencyInjection;

public class EfCoreStoreOptions : StoreOptions
{
}

public class StoreOptions
{
    public int TakeSnapshotVersion { get; set; }
}