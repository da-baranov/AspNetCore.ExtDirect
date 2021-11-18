# Introduction

This library implements Ext Direct specification for the Microsoft Asp.Net Core platform.

To learn more about Ext Direct RPC and Polling, please visit https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html

# Installation

Currently, on pre-release stage, I do not have plans to distribute the library as Nuget package, so please read the following instruction to setup the library manually:

1. Clone this repository
2. Create a solution and then add to that solution a new AspNet Core Web Application project, or open your existing AspNet Core Web Application project
3. Add the AspNetCore.ExtDirect project to your Web Application Project as referenced project. Open project properties page and choose proper Target Framework on the Application tab.
4. Setup ExtJS client-side assets 
5. Edit Web Application project Startup.cs file
```
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

public void ConfigureServices(IServiceCollection services)
{
    ...
    // This enables Ext Direct services
    services.AddExtDirect(options =>
    {
        // TODO: modify options here
    });

    // Add some action handlers
    services.AddExtDirectRemotingHandler<DemoActionHandler>("Demo");

    // Add some polling event sources
    services.AddExtDirectPollingHandler<DemoPollingHandler>();
    ...
}

public void Configure(IApplicationBuilder app)
{
    ...
    app.UseExtDirect();
    ...
}

```
6. By default, the library registers three special routes to handle Ext Direct requests:
- /ExtDirect.js - this URL instructs client-side Ext Direct providers about available server-side endpoints - Actions, Methods and Event sources.
- /ExtDirect - endpoint that handles incoming Ext Direct client-side requests
- /ExtDirectEvents - endpoint for Ext Direct Polling provider

To see AspNetCore.ExtDirect in action, please take a look at the AspNetCore.ExtDirect.Demo web application project. It's a very simple single-page application that demonstrates basic library capabilities.