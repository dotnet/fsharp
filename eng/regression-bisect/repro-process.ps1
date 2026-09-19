param([string] $ErrorMarker)
$ErrorActionPreference = 'Stop'
try {
    [Console]::InputEncoding = New-Object System.Text.UTF8Encoding($false)
    $request = [Console]::ReadLine() | ConvertFrom-Json

    # Emit only P/Invoke declarations: Add-Type would spawn an uncontained compiler before the job exists.
    $assembly = [AppDomain]::CurrentDomain.DefineDynamicAssembly(
        (New-Object Reflection.AssemblyName('ReproJob')), [Reflection.Emit.AssemblyBuilderAccess]::Run)
    $native = $assembly.DefineDynamicModule('ReproJob').DefineType('ReproJob', 'Public, Abstract, Sealed')
    foreach ($signature in @(
        @('CreateJobObjectW', [IntPtr], @([IntPtr], [string])),
        @('SetInformationJobObject', [bool], @([IntPtr], [int], [IntPtr], [int])),
        @('AssignProcessToJobObject', [bool], @([IntPtr], [IntPtr])),
        @('TerminateProcess', [bool], @([IntPtr], [int]))
    )) {
        $method = $native.DefinePInvokeMethod($signature[0], 'kernel32.dll', 'Public, Static, PinvokeImpl',
            [Reflection.CallingConventions]::Standard, $signature[1], [Type[]] $signature[2],
            [Runtime.InteropServices.CallingConvention]::Winapi, [Runtime.InteropServices.CharSet]::Unicode)
        $method.SetImplementationFlags([Reflection.MethodImplAttributes]::PreserveSig)
    }
    $native.CreateType() | Out-Null
    $job = [ReproJob]::CreateJobObjectW([IntPtr]::Zero, $null)
    if ($job -eq [IntPtr]::Zero) { throw 'CreateJobObject failed' }

    # JOBOBJECT_EXTENDED_LIMIT_INFORMATION: LimitFlags at byte 16; size is 144 (64-bit) or 112 (32-bit).
    $size = if ([IntPtr]::Size -eq 8) { 144 } else { 112 }
    $buffer = [Runtime.InteropServices.Marshal]::AllocHGlobal($size)
    try {
        [Runtime.InteropServices.Marshal]::Copy((New-Object byte[] $size), 0, $buffer, $size)
        [Runtime.InteropServices.Marshal]::WriteInt32($buffer, 16, 0x2000) # KILL_ON_JOB_CLOSE
        if (-not [ReproJob]::SetInformationJobObject($job, 9, $buffer, $size)) { throw 'SetInformationJobObject failed' }
        $self = [Diagnostics.Process]::GetCurrentProcess()
        try {
            if (-not [ReproJob]::AssignProcessToJobObject($job, $self.Handle)) { throw 'AssignProcessToJobObject failed' }
        } finally { $self.Dispose() }

        # The supervisor joins before spawning. Its non-inheritable job handle kills all descendants on exit.
        $arguments = foreach ($argument in $request.args) {
            '"' + [regex]::Replace([regex]::Replace($argument, '(\\*)"', '$1$1\"'), '(\\+)$', '$1$1') + '"'
        }
        $start = New-Object Diagnostics.ProcessStartInfo($request.executable, ($arguments -join ' '))
        $start.UseShellExecute = $false
        $start.WindowStyle = [Diagnostics.ProcessWindowStyle]::Hidden
        $start.RedirectStandardOutput = $true
        $start.RedirectStandardError = $true
        $start.EnvironmentVariables.Clear()
        foreach ($entry in $request.environment) {
            $separator = $entry.IndexOf('=')
            $start.EnvironmentVariables.Add($entry.Substring(0, $separator), $entry.Substring($separator + 1))
        }
        $child = [Diagnostics.Process]::Start($start)
        try {
            $stdout = $child.StandardOutput.BaseStream.CopyToAsync([Console]::OpenStandardOutput())
            $stderr = $child.StandardError.BaseStream.CopyToAsync([Console]::OpenStandardError())
            $child.WaitForExit()
            [Threading.Tasks.Task]::WaitAll([Threading.Tasks.Task[]] @($stdout, $stderr))
            # Native exit avoids CLR shutdown waits and kills any servers that have closed their output pipes.
            $self = [Diagnostics.Process]::GetCurrentProcess()
            if (-not [ReproJob]::TerminateProcess($self.Handle, $child.ExitCode)) { throw 'TerminateProcess failed' }
        } finally { $child.Dispose() }
    } finally { [Runtime.InteropServices.Marshal]::FreeHGlobal($buffer) }
}
catch {
    $message = $_.Exception.GetBaseException().Message
    $bytes = [Text.Encoding]::UTF8.GetBytes($message.Substring(0, [Math]::Min(1024, $message.Length)))
    [Console]::Error.WriteLine($ErrorMarker + [Convert]::ToBase64String($bytes))
    [Environment]::Exit(1)
}
