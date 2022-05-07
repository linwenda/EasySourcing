namespace EasySourcing.Abstraction;

public interface IMementoOriginator
{
    IMemento GetMemento();
    void SetMemento(IMemento memento);
}