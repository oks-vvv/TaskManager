TaskManager:

**Assumptions:**
* Task/Process creates a real process in OS. For now, the process executable is configured in appsettings (as it's not mentioned in Add section parameters)
* TaskManager is responsible only for tasks/processes it created (only those processes can be listed & killed)
* When TaskManager stops, all its child processed should be stopped too
* UI is not needed as it's backend position

**Run instructions:**
* Install .NET 5.0 SDK: https://dotnet.microsoft.com/download (sorry, had some time pressure and did not prepare Dockerfile)
* Publish: ```dotnet publish -c Release```
* ```TaskManager/TaskManager.Api/bin/Release/net5.0/publish```
* Run: ```dotnet TaskManager.Api.dll```

**Use instructions:**
* Make sure appsettings.json is properly configured (e.g. ''ProcessSettings_ProcessName'' works for your OS)
* Check swagger page for all the actions documentation: http://localhost:32222/swagger/index.html
* For simple test:
-- POST to http://localhost:32222/task with body:
    {
      "id": 100,
      "priority": "Low"
    }
-- GET to http://localhost:32222/tasks
-- DELETE to http://localhost:32222/task/100