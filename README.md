# Introduction

This library implements [Ext Direct](https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html) specification for the Microsoft Asp.Net Core platform.

The author assumes that you are familiar with Visual Studio, ASP.NET Core, and Sencha ExtJS.
Author also hopes that you have basic knowledge of Ext Direct. 
To learn more about Ext Direct RPC and Polling, please visit [Ext Direct Specification official page](https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html)

Currently the library supports the following features of Ext Direct:
  * Remote function calls
  * Ordered and named arguments
  * Polling
  * Form POSTS and file uploads

If you are restricted with .NET Framework 4.5-4.8, this library will not be helpful for you. 
Consider using this a little bit outdated but still actual project [Ext.Direct for ASP.NET MVC](https://github.com/elishnevsky/ext-direct-mvc)


# How to start

In pre-release, the author has no plans to distribute the library as Nuget package, so please read the following instruction to setup the library manually:

1. Clone this repository

2. Create a solution and then add to that solution a new AspNet Core Web Application project, or add your existing AspNet Core Web Application project

3. Add the AspNetCore.ExtDirect to your Web Application Project as referenced project. Open project properties page and choose proper Target Framework on the Application tab (.NET 5.0, .NET 6.0, and so on)

4. Upgrade all the Nuget dependencies using Visual Studio built-in package manager

5. Setup ExtJS client-side assets and create an ExtJS client application

6. Implement some Ext Direct remoting handler. This may be a class which has a method that accepts a few arguments, e.g.:

```csharp

// A class that wraps named arguments
public class MultiplicationArguments
{
    public int Multiplier { get;set; }

    public int Multiplicand { get;set; }
}

// Ext Direct Remoting handler
public class CalculatorService
{
    // Your Ext Direct remoting handler constructor can support dependency injection
    public CalculatorService(ILogger<CalculatorService> logger)
    {
    }

    // A synchronous method that accepts ordered arguments
    public int Add(int a, int b)
    {
        return a + b;
    }

    // An asynchronous method that accepts ordered arguments
    public async Task<int> Subtract(int a, int b)
    {
        return await Task.FromResult(a - b);
    }

    // A public, but hidden method
    [ExtDirectIgnore]
    public string HiddenMethod()
    {
        return "This should never be called";
    }

    // A synchronous method that accepts named arguments
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

Please note that the library exposes to client all the public methods of your Ext Direct remoting handler. 
If you want to hide a method, just make it private, protected or internal, or, alternatively, mark such a method with the `[ExtDirectIgnore]` attribute.

7. Add another class that would act as an Ext Direct polling handler:

```csharp

public class PollingService 
{
    // Return whatever that implements the IEnumerable interface
    public IEnumerable GetEvents() 
    {
       for (var i = 0; i < 100; i++) 
       {
           yield return i;
       }
    }
}

```

8. Edit your Web Application project Startup.cs file to register AspNetCore.ExtDirect services and middleware:

**Startup.cs** 

```csharp
...
using AspNetCore.ExtDirect;

public class Startup
{
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
            options.AddHandler<CalculatorService>("Calculator");
        });
    
        // Register Ext Direct polling handlers
        services.AddExtDirectPollingApi(options =>
        {
            options.AddHandler<PollingService>((sender) => sender.GetEvents(), "ondata");
        });
        ...
    }

    public void Configure(IApplicationBuilder app)
    {
        ...
        app.UseExtDirect();
        ...
    }
}

```


9. By default, the library middleware registers three special routes to handle Ext Direct requests:
- `/ExtDirect.js` - this URL instructs client-side Ext Direct providers about available server-side endpoints - Actions, Methods and Event sources. You have to reference this URL on the client-side.
- `/ExtDirect/{providerId}` - endpoint that handles incoming Ext Direct client-side requests
- `/ExtDirectEvents/{providerId}` - endpoint for Ext Direct Polling provider

10. Register remoting and polling providers on client side:

**Index.cshtml**

```html

<script src="~/ExtDirect.js">/* Required */</script>

<script>
Ext.direct.Manager.addProvider(Ext.REMOTING_API);
Ext.direct.Manager.addProvider(Ext.POLLING_API);
</script>
```

11. Call some remote function from your ExtJS application:

```javascript

Calculator.add(3, 4, function(response) {
    alert(response);
});

```

To see AspNetCore.ExtDirect in action, please take a look at the AspNetCore.ExtDirect.Demo web application project. 
It's a very simple single-page application that demonstrates basic library capabilities.

# How to...

## Change default AspNetCore.ExtDirect endpoint URLs

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
       // 2. http://localhost/MyExtDirectRemotingApi/remoting/{remotingProviderId} - for handling Ext Direct remoting requests
       // 3. http://localhost/MyExtDirectRemotingApi/polling/{pollingProviderId} - for handling Ext Direct polling requests

       options.Url = "MyExtDirectRemotingApi";
    });
}
```

## Configure multiple Ext Direct APIs

The library allows a developer to register multiple remoting and polling ExtDirect APIs. 
Please not that each API should have an unique ID and unique name.


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
        options.Name = options.Id = "REMOTING_API_1";
        options.Namespace = "CalculatorNS1";
        options.AddHandler<CalculatorService>("Calculator");
    });

    // Register another Ext Direct remoting handler
    services.AddExtDirectRemotingApi(options =>
    {
        // This will create another remoting handler endpoint at /extdirect/remoting/REMOTING_API_2
        options.Name = options.Id = "REMOTING_API_2";
        options.Namespace = "CalculatorNS2";
        options.AddHandler<CalculatorService>("Calculator");
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


## Dependency Injection

Ext Direct remoting and polling handlers can be implemented as services that reuse other ASP.NET Core web application services and receive dependencies in a typical way, via constructor:

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

These services can be pretty useful to perform routine operations (logging, authorization, audit and so on).

## Using asynchronous methods

Ext Direct **Remoting** and **Polling** handler methods can be either synchronous or asynchronous:

```csharp

// Remoting handler

public class CalculatorService
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

// Polling handler

public class PollingService
{
    public async Task<IEnumerable> GetEvents(Filter filter)
    {
        var list = new List<string>();
        // Populate the list
        return await Task.FromResult(list);
    }
}

// Async handlers registration (Startup.cs)

public void ConfigureServices(IServiceCollection services)
{
    services.AddExtDirectRemotingApi(options =>
    {
        options.AddHandler<CalculatorService>("Calculator");
    });

    services.AddExtDirectPollingApi(options =>
    {
        options.AddHandler<PollingService, Filter>((sender, filter) => sender.GetEvents(filter), "onevent");
    });
}

```


## Passing arguments to Polling handlers

The Ext Direct specification allows a developer to pass arguments (in a form of query string) to polling handlers. 
This may be handy if your polling handler produces multiple type of events and you want to filter out some of those on server-side.
Lets imagine that your filter query string looks like `?eventName=ondata&skip=100&take=100`.
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
    public IEnumerable GetEvents(PollingEventFilter filter)
    {
        return this
            .dbContext
            .Events
            .Where(row => row.Name == filter?.EventName)
            .Skip(filter.Skip)
            .Take(filter.Take)
            .Select(row => row.Data);
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
        options.AddHandler<PollingService, PollingEventFilter>((sender, args) => sender.GetEvents(args), "ondata");
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
        options.AddHandler<CalculatorService>("Calculator");
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

## Handling exceptions on client-side

If you call some remote method and if an exception occurs on server-side, the library does not return HTTP error code,
but returns an exception object that can be handled by Ext Direct infrastructure on client-side:

```javascript
Calculator.add(2, 2, function (response, e) {
    if (e.type === "exception") {
        Ext.Msg.show(
            {
                title: "Error",
                message: e.message,
                icon: Ext.MessageBox.ERROR,
                buttons: Ext.MessageBox.OK
            }
        );
    } else {
        Ext.Msg.alert("Success", response);
    }
});

```










