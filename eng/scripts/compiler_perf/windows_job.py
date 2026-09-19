"""Windows process-tree ownership, established before the child can execute."""

import ctypes
from ctypes import wintypes as w


class BasicLimits(ctypes.Structure):
    _fields_ = [("process_time", ctypes.c_longlong), ("job_time", ctypes.c_longlong),
                ("flags", w.DWORD), ("minimum_working_set", ctypes.c_size_t),
                ("maximum_working_set", ctypes.c_size_t), ("active_processes", w.DWORD),
                ("affinity", ctypes.c_size_t), ("priority", w.DWORD), ("scheduling", w.DWORD)]


class ExtendedLimits(ctypes.Structure):
    _fields_ = [("basic", BasicLimits), ("io_counters", ctypes.c_ulonglong * 6),
                ("process_memory", ctypes.c_size_t), ("job_memory", ctypes.c_size_t),
                ("peak_process_memory", ctypes.c_size_t), ("peak_job_memory", ctypes.c_size_t)]


class ThreadEntry(ctypes.Structure):
    _fields_ = [("size", w.DWORD), ("usage", w.DWORD), ("thread_id", w.DWORD),
                ("process_id", w.DWORD), ("base_priority", w.LONG),
                ("delta_priority", w.LONG), ("flags", w.DWORD)]


class WindowsJob:
    def __init__(self):
        self.kernel = ctypes.WinDLL("kernel32", use_last_error=True)
        for name, result, args in (
            ("CreateJobObjectW", w.HANDLE, [ctypes.c_void_p, w.LPCWSTR]),
            ("SetInformationJobObject", w.BOOL, [w.HANDLE, ctypes.c_int, ctypes.c_void_p, w.DWORD]),
            ("AssignProcessToJobObject", w.BOOL, [w.HANDLE, w.HANDLE]),
            ("TerminateJobObject", w.BOOL, [w.HANDLE, w.UINT]),
            ("CloseHandle", w.BOOL, [w.HANDLE]),
            ("CreateToolhelp32Snapshot", w.HANDLE, [w.DWORD, w.DWORD]),
            ("Thread32First", w.BOOL, [w.HANDLE, ctypes.POINTER(ThreadEntry)]),
            ("Thread32Next", w.BOOL, [w.HANDLE, ctypes.POINTER(ThreadEntry)]),
            ("OpenThread", w.HANDLE, [w.DWORD, w.BOOL, w.DWORD]),
            ("ResumeThread", w.DWORD, [w.HANDLE]),
        ):
            function = getattr(self.kernel, name)
            function.restype, function.argtypes = result, args
        self.handle = self.kernel.CreateJobObjectW(None, None)
        if not self.handle:
            raise ctypes.WinError(ctypes.get_last_error())
        limits = ExtendedLimits()
        limits.basic.flags = 0x2000  # JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
        if not self.kernel.SetInformationJobObject(self.handle, 9, ctypes.byref(limits), ctypes.sizeof(limits)):
            error = ctypes.WinError(ctypes.get_last_error())
            self.kernel.CloseHandle(self.handle)
            raise error

    def assign_and_resume(self, process):
        if not self.kernel.AssignProcessToJobObject(self.handle, int(process._handle)):
            raise ctypes.WinError(ctypes.get_last_error())
        snapshot = self.kernel.CreateToolhelp32Snapshot(0x4, 0)  # TH32CS_SNAPTHREAD
        if snapshot == ctypes.c_void_p(-1).value:
            raise ctypes.WinError(ctypes.get_last_error())
        try:
            entry = ThreadEntry()
            entry.size = ctypes.sizeof(entry)
            found = self.kernel.Thread32First(snapshot, ctypes.byref(entry))
            while found:
                if entry.process_id == process.pid:
                    thread = self.kernel.OpenThread(0x2, False, entry.thread_id)  # THREAD_SUSPEND_RESUME
                    if not thread:
                        raise ctypes.WinError(ctypes.get_last_error())
                    try:
                        if self.kernel.ResumeThread(thread) == 0xFFFFFFFF:
                            raise ctypes.WinError(ctypes.get_last_error())
                    finally:
                        self.kernel.CloseHandle(thread)
                    return
                found = self.kernel.Thread32Next(snapshot, ctypes.byref(entry))
            raise OSError("Cannot find the suspended child's primary thread")
        finally:
            self.kernel.CloseHandle(snapshot)

    def close(self):
        if self.handle is not None:
            if not self.kernel.TerminateJobObject(self.handle, 1):
                raise ctypes.WinError(ctypes.get_last_error())
            if not self.kernel.CloseHandle(self.handle):
                raise ctypes.WinError(ctypes.get_last_error())
            self.handle = None
