for ($i = 1; $i -le 10; $i++) {
    Write-Host "=== Test Suite Run $i ==="
    & "D:\NUnit.Console\bin\net8.0\nunit3-console.exe" "C:\Users\khine\Desktop\FlakyAnalyzer.NET\_2TestSuiteLogger\bin\Debug\net8.0\_2TestSuiteLogger.dll"

    Write-Host "Waiting 10 seconds before next run..."
    Start-Sleep -Seconds 10
}