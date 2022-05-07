# EasySourcing
A lightweight event sourcing implementation based on .NET 6

## Installation
You should install [EasySourcing with NuGet](https://www.nuget.org/packages/EasySourcingX):
```
Install-Package EasySourcing
```

## Quick start:
1. Create an EventSourced class
```
public class Post : EventSourced
{
    private Guid _authorId;  
    private string _title;  
    private string _content;
    
	private Post(Guid id) : base(id)  
	{  
	}
}
```
2. Pending