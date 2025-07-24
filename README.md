ToDo Application
This is a simple console-based ToDo application. It allows you to manage your tasks by adding, listing, removing, and altering them. Task data is stored in a MySQL database.

How to Run
The run_app.sh script is provided to simplify launching the application.

run_app.sh
This script automates the setup and execution:

Checks if the docker sql container is running.

Starts Container if needed.

Launches Application: Once the database is confirmed to be running, the script executes your C# ToDo application using dotnet run.

Run the application: ./run_app.sh
