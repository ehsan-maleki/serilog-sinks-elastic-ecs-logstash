# ELK Log Provider `v1.0`
Provides logging methods to send multiple types of logs with different **severity** and
different **data** (as payload)

This provider uses ELK sink of *serilog* to send log to Elasticsearch 
**logstash** over http protocol.

## What is ECS?
[Elastic](https://www.elastic.co) introduced 
[Elastic Common Schema (ecs)](https://www.elastic.co/blog/introducing-the-elastic-common-schema) 
as a common signature for all sorts of logs gathering from all types of sources 
such as file, http, health and ...

Converting from LogEvent to ECS is automated. Data of _Request_ and _Response_ gathers 
and attaches to ECS before is send to logstash. 

## Configuration of Provider
Only this is needed by provider is url of _Logstash Http Plugin_.
Default value for this config is `http://localhost:8080`.
You can change this default url by setting a value for `LoggerLogstashUrl` key 
in web.config/app.config of your application. 

### Defining Server and Application
If you have multiple applications using this provider, so you need to define witch application 
is sending events to log server. There are two ways to achieve this.

#### Using AppSettings
The source application of events can be defined by following `keys` in `.config` file of 
your application. Let's say we have an applications with name `Emzam Audit Log Test API` 
and version is 1.0. 
Following setting keys should be set in `web.config`: 
* **ApplicationId :** Unique string key of your application.
  * `<add key="ApplicationId" value="EALTA" />`
* **ApplicationName :** name of your application.
  * `<add key="ApplicationName" value="Emzam Audit Log Test API" />`
* **ApplicationType :** Type of application is one of : _Api_, _Website_, _WindowsService_, 
_WindowsFormApplication_, _WindowsConsoleApplication_.
  * `<add key="ApplicationType" value="Api" />`
  * For invalid values, _Unknown_ will be used.
* **ApplicationVersion :** Application version in xx.xx.xx  format.
  * `<add key="ApplicationVersion" value="1.0.0" />`
 
#### Using `ChangeApplication` Method
Using `ChangeApplication` method is easier than setting application settings. 
Only thing you need to do is to call the method as following:

`var _logProvider = new ElkLogProvider();`
```
_logProvider.ChangeApplication(new LogApplicationModel
{
    Id = "EALTA",
    Name = "Emzam Audit Log Test API",
    Type = ApplicationTypes.Api,
    Version = "1.0.0"
});
``` 
**Attention :** All events after calling of this method, will be send to server with new application.

## Using Log Methods
After finishing of configuration, your can start your logging by just calling log methods.

**Important :** As mentioned before, during automated cast of LogEvent to Elastic ECS standard,
some extra information will be gathered and attached to ECS model.
This information are:
- Full information of `Http Reuquest` sent by client.
- Global information of `Http Response` sending to client.
- `Current User` information + `IP Address`
- `Current Server` information, computer name, IP and ....
- `Current Running Proccess` information, PID and ...    

Before you start to log, there are some definitions you should know about:
* **Log Category :** is used to categorize some sets of logs together. 
For example, when you use provider to log _Authentication_ actions of user 
such as : login, forget password, reset password and ..., best value for 
`category` is `user authentication`.
  * `Category` has default value of `Default Logs`
* **Log Name :** Name of action that created the log.
  * Name is required.
* **Payload :** There is possibility to send extra information attached to log event.
  * Payload is a list of key/value pair `List<KeyValuePair<string, string>>`. 

### Information Log
Log information about event takes place in your application.
  * Severity level: _Low_.
```
    _logProvider.LogInformation(
        "Category: Authentication",
        "Name: User logged in successfully", 
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Username", "EHSAN")
        }        
    );
```

### Debug Log
Debug is used for developers to log information they need to make sure of action they 
have in code.
  * Severity level: _Normal_.
```
    _logProvider.LogDebug(
        "Name: User logged in from external oAuth service", 
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Username", "EHSAN")
        }, 
        "Category: Authentication"
    );
```

### Warning Log
Warnings have higher severity that can be used to send notifications.
  * Severity level: _High_.
```
    _logProvider.LogWarning(
        "Name: User login fiailed!", 
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Username", "EHSAN")
        }, 
        "Category: Authentication"
    );
```

### Audit Log
Audit log has normal (not low) severity during to data it's logging. 
System assuming audit data are used to track user activities.
  * Severity level: _Normal_.
```
    _logProvider.LogAudit(
        "Name: User password changed by admin", 
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("AdminUsername", "EHSAN")
            new KeyValuePair<string, string>("UserUsername", "REZA")
        }, 
        "Category: Authentication"
    );
```

### Critical Log
Critical logs should be used when you want to send notification based on some events are so important.
Such as `Too many invalid login`, `Too many search by one user` and ...
  * Severity level: _High_.
```
    _logProvider.LogCritical(
        "Name: User login attemt failed 5 times!", 
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Username", "EHSAN")
        }, 
        "Category: Authentication"
    );
```

### Fetal Log
In some situations you decide to not to continue application run and raise events.
Fetal logs designed for  such situations. 
 Example of fetal are `Bank account balance is low`, `SMS serivce provider went down` and ...
  * Severity level: _High_.
```
    _logProvider.LogFetal(
        "Name: Bank account balance is low!", 
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Username", "EHSAN")
        }, 
        "Category: Suppliement"
    );
```

### Error Log
This method designed to log full details of exception occured in your application. 
 Only this you need to do, is to provide a name and specify category for your exception.
  * Severity level by default is _Normal_ and can be defined by developer.
```
    _logProvider.LogError(
        "Name: User login failed due to exception!",
        exception,
        /* Payload Data */ 
        new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Username", "EHSAN")
        }, 
        "Category: Authentication"
    );
```

## Support
This package has no any supports.

Please feel free to send emails to [Ehsan Maleki Zoerma](mailto:ehsan dot maleki at gmail dot com) and tell about your issues and give some opinions on project. Nothing more than hearing from users, would make me happy.



