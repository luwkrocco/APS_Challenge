:: Steps to run the application ::

Application is divided into 2 projects.
 - System Alerts Monitor Dashboard [.NET MVC Project]
 - System Alerts Monitor Provider [GRPC Service]

---------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------
	System Alerts Monitor Provider - System.Alerts.Monitor.Provider.GrpcService
---------------------------------------------------------------------------------------

To run the GRPC Service, you need to take the following steps beforehand:

 - Copy the files found within the Bin folder to the target directory you wish to run the Service From.
 - Make sure you have Admin Rights to execute the GRPC Service.
 - Review the Appsettings.JSON and make sure that the following paths are updated:

		"DataStore": {
			"SQLITE": "E:\\APS Task\\Work In Progress\\System Alerts Monitor\\Files\\Sample_Monitoring_Alerts.sqlite",
			"CSV": "E:\\APS Task\\Work In Progress\\System Alerts Monitor\\Files\\Sample_Monitoring_Alerts.csv"
		},
		"Log": {
			Path": "E:\\APS Task\\Work In Progress\\System Alerts Monitor\\Files\\Logs\\BE\\"
		}

 - Execute the "SAM.Provider.GrpcService.exe" and make sure that Admin rights are available.

---------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------
	System Alerts Monitor Dashboard - System.Alerts.Monitor.Dashboard.UI
---------------------------------------------------------------------------------------
 - Deploy the .Net MVC Web Application in a suitable environment
 - Review the Appsettings.JSON and make sure that the following paths are updated:

		"SystemAlertService": { "URI": "http://localhost:5019" },
		"Log": {
			"Path": "E:\\APS Task\\Work In Progress\\System Alerts Monitor\\Files\\Logs\\UI\\"
		}	
 - Make sure that the GRPC Service is running before running the Dashboard UI.


---------------------------------------------------------------------------------------
:: Assumptions Made / Enhancements Implemented ::
---------------------------------------------------------------------------------------

Please review the document "System Alerts Monitor - Project Specifications Document.pdf" for more details concerning Assumptions Made, and Extra Features Implemented.


---------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------
	- LAST COMMENT -
---------------------------------------------------------------------------------------

	If during deployment issues crop up with custom deployment, the solution can be quickly run from Visual Studio directly.

	Make sure you have Visual Studio running in Admin Mode, and boot up the solution. 

	Review the above steps concerning Appsettings, and then make sure that the Solution will run Multiple Projects at once. ( System.Alerts.Monitor.Provider.GrpcService and System.Alerts.Monitor.Dashboard.UI )

	Do not hesitate to contact me on Email (luwk.rocco@gmail.com) or Mobile (+356 79291663) should you need any clarifications.
