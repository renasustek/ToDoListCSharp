
echo "--- ToDo Application Launcher ---"

DB_CONTAINER_ID=$(docker-compose ps -q db)

if [ -z "$DB_CONTAINER_ID" ]; then
    echo "MySQL container is not running. Starting it now..."
    docker-compose up -d
    if [ $? -eq 0 ]; then
        echo "MySQL container started successfully."
        echo "Giving MySQL a few seconds to warm up..."
        sleep 10
    else
        echo "Failed to start MySQL container"
        exit 1 
    fi
else
    echo "MySQL container is already running."
fi

echo "Running the C# ToDo application..."
dotnet run

echo "--- Application Finished ---"
