# Start-All Services for Horizon Solution

Write-Host "Starting Horizon GatewayAPI..." -ForegroundColor Green
Start-Process dotnet "run --project gateway/GatewayAPI/GatewayAPI.csproj --launch-profile https"

Write-Host "Starting Identity Service..." -ForegroundColor Green
Start-Process dotnet "run --project services/identity-service/LearningPlatformAuth.csproj --launch-profile https"

Write-Host "Starting Teacher Service..." -ForegroundColor Green
Start-Process dotnet "run --project services/teacher-service/teacherAPI.csproj --launch-profile https"

Write-Host "Starting Course Service..." -ForegroundColor Green
Start-Process dotnet "run --project services/course-service/Course.csproj --launch-profile https"

Write-Host "Starting Chat Service..." -ForegroundColor Green
Start-Process dotnet "run --project services/chat-service/WebApplication1.csproj --launch-profile WebApplication1"

Write-Host "Starting Student Service..." -ForegroundColor Green
Start-Process dotnet "run --project services/student-service/LearningPlatform.StudentService.csproj --launch-profile https"

Write-Host "Starting Horizon.MVC Frontend..." -ForegroundColor Green
Start-Process dotnet "run --project Horizon.MVC/Horizon.MVC.csproj --launch-profile https"

Write-Host "All services started! Check the newly opened command windows for logs." -ForegroundColor Cyan
