# ============================================================
#  WMS - Start Backend API + Frontend Web
# ============================================================
#  Backend API  : http://localhost:5000  (Swagger UI)
#  Frontend Web  : http://localhost:5001  (Login: admin / Admin@123)
# ============================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   WMS - Warehouse Management System    " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host " Starting Backend API  -> http://localhost:5000" -ForegroundColor Green
Write-Host " Starting Frontend Web -> http://localhost:5001" -ForegroundColor Yellow
Write-Host ""
Write-Host " Login : admin / Admin@123" -ForegroundColor White
Write-Host " Press Ctrl+C in each window to stop"  -ForegroundColor Gray
Write-Host ""

# Start Backend API in new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", `
    "Write-Host '[API] Backend running at http://localhost:5000' -ForegroundColor Green; dotnet run --project src/WMS.API"

Start-Sleep -Seconds 3

# Start Frontend Web in new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", `
    "Write-Host '[WEB] Frontend running at http://localhost:5001' -ForegroundColor Yellow; dotnet run --project src/WMS.Web"

Start-Sleep -Seconds 5

# Open browser
Start-Process "http://localhost:5001"

Write-Host " Both services started. Browser opened." -ForegroundColor Cyan
Write-Host ""
