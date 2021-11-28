# Introduction

This library implements Ext Direct specification for the Microsoft Asp.Net Core platform.

We assume that you are familiar with Sencha ExtJS and have basic knowledge of Ext Direct.

If you are restricted with .NET Framework 4.5-4.8, that library will not be helpful for you. 
Consider using this a little bit outdated but still actual project - https://github.com/elishnevsky/ext-direct-mvc

To learn more about Ext Direct RPC and Polling, please visit https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html

# How to start

Currently, in pre-release, I have no plans to distribute the library as Nuget package, so please read the following instruction to setup the library manually:

1. Clone this repository

2. Create a solution and then add to that solution a new AspNet Core Web Application project, or open your existing AspNet Core Web Application project

3. Add the AspNetCore.ExtDirect project to your Web Application Project as referenced project. Open project properties page and choose proper Target Framework on the Application tab (.NET 5.0, .NET 6.0, and so on)

4. Setup ExtJS client-side assets 

5. Implement some Ext Direct remoting handler. This may be a class which has a method that accepts a few arguments, e.g.:

```csharp

// Class that wraps named arguments
public class MultiplicationArguments
{
    public int Multiplier { get;set; }
    public int Multiplicand { get;set; }
}

// Ext Direct Remoting handler
public class CalculatorService
{
    // Constructor supports dependency injection
    public CalculatorService(ILogger<CalculatorService> logger)
    {
    }

    // Synchronous method, ordered arguments
    public int Add(int a, int b)
    {
        return a + b;
    }

    // Asynchronous method, ordered arguments
    public async Task<int> Subtract(int a, int b)
    {
        return await Task.FromResult(a - b);
    }

    // Synchronous method, named arguments
    [ExtDirectNamedArgs]
    public int Multiple(MultiplicationArguments args)
    {
        return args.Multiplier * args.Multiplicand;
    }
}

``` 

Here we have defined two methods that accept ordered arguments and another one that accepts named arguments.
Please note that the following features are supported:
- Ordered arguments
- Named arguments
- Asynchronous operations
- Dependency injection

6. Implement another class that would act as an Ext Direct polling handler:

```csharp

public class PollingService 
{
    public IEnumerable<PollResponse>() GetEvents() 
    {
       for (var i = 0; i < 100; i++) 
       {
           yield return new RollResponse { Name = "onevent", Data = i };
       }
    }
}

```

7. Edit your Web Application project Startup.cs file to register AspNetCore.ExtDirect controllers and middleware:
 
```csharp


public void ConfigureServices(IServiceCollection services)
{
    ...
    // Configure Ext Direct common options
    services.AddExtDirect(options => 
    {
       ...
    });

    // Register Ext Direct remoting handlers
    services.AddExtDirectRemotingApi(options =>
    {
        options.AddActionHandler<CalculatorService>("Calculator");
    });
    
    // Register Ext Direct polling handlers
    services.AddExtDirectPollingApi(options =>
    {
        options.AddPollingHandler<PollingService>((sender) => sender.GetEvents());
    });
    ...
}

public void Configure(IApplicationBuilder app)
{
    ...
    app.UseExtDirect();
    ...
}

```


8. By default, library middleware registers three special routes to handle Ext Direct requests:
- /ExtDirect.js - this URL instructs client-side Ext Direct providers about available server-side endpoints - Actions, Methods and Event sources. You have to reference this URL on the client-side.
- /ExtDirect - endpoint that handles incoming Ext Direct client-side requests
- /ExtDirectEvents - endpoint for Ext Direct Polling provider


To see AspNetCore.ExtDirect in action, please take a look at the AspNetCore.ExtDirect.Demo web application project. It's a very simple single-page application that demonstrates basic library capabilities.

# How to...

## Changing default AspNetCore.ExtDirect endpoint URLs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    // Configure Ext Direct common options
    services.AddExtDirect(options => 
    {
       // This will register the following routes: 
       //
       // 1. http://localhost/MyExtDirectRemotingApi.js - for client javascript Ext Direct API
       // 2. http://localhost/MyExtDirectRemotingApi/remoting/{remotingProviderName} - for handling Ext Direct remoting requests
       // 3. http://localhost/MyExtDirectRemotingApi/polling/{pollingProviderName} - for handling Ext Direct polling requests

       options.Url = "MyExtDirectRemotingApi";
    });
}
```

## Configuring multiple Ext Direct APIs

**Server (Startup.cs)**

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    // Configure Ext Direct common options
    services.AddExtDirect(options => 
    {
       options.Url = "ExtDirect";
    });

    // Register first Ext Direct remoting handler
    services.AddExtDirectRemotingApi(options =>
    {
        // This will create remoting handler endpoint at /extdirect/remoting/REMOTING_API_1
        options.Name = "REMOTING_API_1";
        options.Namespace = "CalculatorNS1";
        options.AddActionHandler<CalculatorService>("Calculator");
    });

    // Register another Ext Direct remoting handler
    services.AddExtDirectRemotingApi(options =>
    {
        // This will create another remoting handler endpoint at /extdirect/remoting/REMOTING_API_2
        options.Name = "REMOTING_API_2";
        options.Namespace = "CalculatorNS2";
        options.AddActionHandler<CalculatorService>("Calculator");
    });
    ...
}
```

**Client (Index.cshtml)**

```html

<script src="~/ExtDirect.js"></script>

<script>
...
Ext.direct.Manager.addProvider(Ext.REMOTING_API_1);
Ext.direct.Manager.addProvider(Ext.REMOTING_API_2);
...
CalculatorNS1.Calculator.Add(1, 2, function(result) { console.log(result); });
CalculatorNS2.Calculator.Add(3, 4, function(result) { console.log(result); });

</script>

```


## Using Dependency Injection

Your Ext Direct remoting and polling handlers can be implemented as services that reuse other ASP.NET Core application services and receive dependencies in a typical way, via constructor:

```csharp
public class DemoHandler
{
    public DemoHandler(IServiceProvider serviceProvider,
                       ILogger<DemoHandler> logger,
                       MyDbContext dbContext)
    {
        ...
    }
}
```

## Using asynchronous methods

Ext Direct **Remoting** handler methods can be either synchronous or asynchronous:

```csharp
public class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public async Task<int> AddAsync(int a, int b)
    {
        var result = a + b;
        return await Task.FromResult(result);
    }
}
```

As for now, methods of **Polling** event handlers can only be synchronous.

***

## Passing arguments to Polling handlers

The Ext Direct specification allows a developer to pass arguments (in a form of query string) to polling handlers. 
This may be handy if your polling handler produces multiple type of events and you want to filter out some of those.
Lets imagine your query string looks like `?eventName=ondata&skip=100&take=100`.
In ASP.NET Core such an URL can be mapped to a C# class with the following structure:

```csharp
public class PollingEventFilter
{
    public string EventName { get;set; }
    public int? Skip { get;set; }
    public int Take { get;set; }
}
```

The library can automatically convert the query string to an argument that can be passed to your Ext Direct polling handler method. 
Simply use a special overriden configuration method:

```csharp

// PollingService.cs

public class PollingService
{
    public IEnumerable<PollResponse> GetEvents(PollingEventFilter filter)
    {
        if (filter.EventName == "ondata")
        {
            return this
                .dbContext
                .Events
                .Where(row => row.Name == "ondata")
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(row => new PollResponse { Name = row.Name, Data = row.Data });
        }
    }
}

// Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddExtDirect(options =>
    {
        ...
    });

    services.AddExtDirectPollingApi(options =>
    {
        options.AddPollingHandler<PollingService, PollingEventFilter>((sender, args) => sender.GetEvents(args));
    }
}

```

## Ext Direct Namespaces

Namespaces are not required by Ext Direct specification, so the library does not automatically expose class namespaces when you register Remoting and Polling handlers.
If you want to group handlers by namespaces, you can perform that manually configuring the library as follows:

**Server**

```csharp
// Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddExtDirect(options =>
    {
        ...
    });

    services.AddExtDirectRemotingApi(options =>
    {
        options.Namespace = "CalculatorNS";
        options.AddActionHandler<CalculatorService>("Calculator");
    });
}
```

**Client**

```html
<script src="~/ExtDirect.js"></script>

<script>
...
Ext.direct.Manager.addProvider(Ext.REMOTING_API);
...
CalculatorNS.Calculator.Add(1, 2, function(result) { console.log(result); });

</script>

```













