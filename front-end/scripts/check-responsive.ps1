$ErrorActionPreference = "Stop"

$chrome = "C:\Program Files\Google\Chrome\Application\chrome.exe"
$port = 9234
$userData = Join-Path $env:TEMP ("chrome-responsive-" + [guid]::NewGuid().ToString())
New-Item -ItemType Directory -Path $userData | Out-Null
$chromeProcess = $null
$ws = $null

function Send-Cdp($ws, [int]$id, [string]$method, $params = $null) {
  $payload = if ($null -eq $params) { @{ id = $id; method = $method } } else { @{ id = $id; method = $method; params = $params } }
  $json = ($payload | ConvertTo-Json -Depth 20 -Compress)
  $bytes = [System.Text.Encoding]::UTF8.GetBytes($json)
  $segment = [ArraySegment[byte]]::new($bytes)
  $ws.SendAsync($segment, [System.Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).GetAwaiter().GetResult()
}

function Receive-Cdp($ws) {
  $buffer = New-Object byte[] 65536
  $builder = New-Object System.Text.StringBuilder
  do {
    $segment = [ArraySegment[byte]]::new($buffer)
    $result = $ws.ReceiveAsync($segment, [Threading.CancellationToken]::None).GetAwaiter().GetResult()
    if ($result.Count -gt 0) {
      [void]$builder.Append([System.Text.Encoding]::UTF8.GetString($buffer, 0, $result.Count))
    }
  } while (-not $result.EndOfMessage)
  $builder.ToString() | ConvertFrom-Json
}

function Invoke-Cdp($ws, [ref]$idRef, [string]$method, $params = $null) {
  $idRef.Value++
  $id = $idRef.Value
  Send-Cdp $ws $id $method $params
  do {
    $message = Receive-Cdp $ws
  } while ($message.id -ne $id)
  $message
}

try {
  $chromeProcess = Start-Process -FilePath $chrome -ArgumentList @("--headless=new", "--disable-gpu", "--remote-debugging-port=$port", "--remote-allow-origins=*", "--user-data-dir=$userData", "about:blank") -PassThru
  Start-Sleep -Seconds 2

  $target = Invoke-RestMethod -Method Put "http://localhost:$port/json/new?about:blank"
  $ws = [System.Net.WebSockets.ClientWebSocket]::new()
  $ws.ConnectAsync([Uri]$target.webSocketDebuggerUrl, [Threading.CancellationToken]::None).GetAwaiter().GetResult()

  $id = 0
  $idRef = [ref]$id
  Invoke-Cdp $ws $idRef "Page.enable" | Out-Null
  Invoke-Cdp $ws $idRef "Runtime.enable" | Out-Null

  $routes = @("home", "diario", "perfil", "onboarding", "cadastro")
  $widths = @(375, 768, 1440)
  $results = @()

  foreach ($route in $routes) {
    foreach ($width in $widths) {
      Invoke-Cdp $ws $idRef "Emulation.setDeviceMetricsOverride" @{ width = $width; height = 1000; deviceScaleFactor = 1; mobile = ($width -lt 768) } | Out-Null
      Invoke-Cdp $ws $idRef "Page.navigate" @{ url = "http://localhost:5173/#/$route" } | Out-Null
      Start-Sleep -Milliseconds 1200

      $expr = @"
(() => ({
  innerWidth: window.innerWidth,
  docScrollWidth: document.documentElement.scrollWidth,
  bodyScrollWidth: document.body.scrollWidth,
  overflow: Math.max(document.documentElement.scrollWidth, document.body.scrollWidth) > window.innerWidth + 1,
  hasContent: document.body.innerText.trim().length > 0,
  title: document.querySelector('h1,h2')?.innerText || ''
}))()
"@

      $response = Invoke-Cdp $ws $idRef "Runtime.evaluate" @{ expression = $expr; returnByValue = $true }
      $value = $response.result.result.value
      $results += [pscustomobject]@{
        Route = $route
        Width = $width
        InnerWidth = $value.innerWidth
        ScrollWidth = [Math]::Max($value.docScrollWidth, $value.bodyScrollWidth)
        Overflow = $value.overflow
        HasContent = $value.hasContent
        Title = $value.title
      }
    }
  }

  $results | Format-Table -AutoSize
}
finally {
  if ($ws) {
    $ws.Dispose()
  }
  if ($chromeProcess -and -not $chromeProcess.HasExited) {
    Stop-Process -Id $chromeProcess.Id -Force
  }
  Start-Sleep -Milliseconds 300
  if (Test-Path $userData) {
    Remove-Item -Recurse -Force -LiteralPath $userData -ErrorAction SilentlyContinue
  }
}
