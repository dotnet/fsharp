
# Collection of Windows-only PowerShell build utility functions used by our CI scripts.
# Ported from dotnet/roslyn eng\build-utils-win.ps1 to support running the VS integration
# tests on an interactive desktop session (screenshot probe + tscon reconnect).

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName 'System.Drawing'
Add-Type -AssemblyName 'System.Windows.Forms'
function Capture-Screenshot($path) {
  $width = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Width
  $height = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Height

  $bitmap = New-Object System.Drawing.Bitmap $width, $height
  try {
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
      $graphics.CopyFromScreen( `
        [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.X, `
        [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Y, `
        0, `
        0, `
        $bitmap.Size, `
        [System.Drawing.CopyPixelOperation]::SourceCopy)
    } finally {
      $graphics.Dispose()
    }

    $bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
  } finally {
    $bitmap.Dispose()
  }
}
