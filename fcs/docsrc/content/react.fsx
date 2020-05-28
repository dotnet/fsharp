(*** hide ***)
#I "../../../artifacts/bin/fcs/net461"
(**
Compiler Services: Reacting to Changes
============================================

This tutorial discusses some technical aspects of how to make sure the F# compiler service is 
providing up-to-date results especially when hosted in an IDE. See also [project wide analysis](project.html)
for information on project analysis.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published.

The logical results of all "Check" routines (``ParseAndCheckFileInProject``, ``GetBackgroundCheckResultsForFileInProject``, 
``TryGetRecentTypeCheckResultsForFile``, ``ParseAndCheckProject``) depend on results reported by the file system,
especially the ``IFileSystem`` implementation described in the tutorial on [project wide analysis](project.html).
Logically speaking, these results would be different if file system changes occur.  For example,
referenced DLLs may change on disk, or referenced files may change.

The ``FSharpChecker`` component from FSharp.Compiler.Service does _not_ actively "listen"
to changes in the file system.  However ``FSharpChecker`` _does_ repeatedly ask for
time stamps from the file system which it uses to decide if recomputation is needed.
FCS doesn't listen for changes directly - for example, it creates no ``FileWatcher`` object (and the
``IFileSystem`` API has no ability to create such objects).  This is partly for legacy reasons,
and partly because some hosts forbid the creation of FileWatcher objects.

In most cases the repeated timestamp requests are sufficient. If you don't actively
listen for changes, then ``FSharpChecker`` will still do _approximately_ 
the right thing, because it is asking for time stamps repeatedly.  However, some updates on the file system
(such as a DLL appearing after a build, or the user randomly pasting a file into a folder) 
may not actively be noticed by ``FSharpChecker`` until some operation happens to ask for a timestamp.
By issuing fresh requests, you can ensure that FCS actively reassesses the state of play when
stays up-to-date when changes are observed.

If you want to more actively listen for changes, then you should add watchers for the
files specified in the ``DependencyFiles`` property of ``FSharpCheckFileResults`` and ``FSharpCheckProjectResults``.
Here�s what you need to do: 

* When your client notices an CHANGE event on a DependencyFile, it should schedule a refresh call to perform the ParseAndCheckFileInProject (or other operation) again.
  This will result in fresh FileSystem calls to compute time stamps.

* When your client notices an ADD event on a DependencyFile, it should call ``checker.InvalidateConfiguration`` 
  for all active projects in which the file occurs. This will result in fresh FileSystem calls to compute time 
  stamps, and fresh calls to compute whether files exist.

* Generally clients don�t listen for DELETE events on files.  Although it would be logically more consistent 
  to do so, in practice it�s very irritating for a "project clean" to invalidate all intellisense and 
  cause lots of red squiggles.  Some source control tools also make a change by removing and adding files, which 
  is best noticed as a single change event.



If your host happens to be Visual Studio, then this is one technique you can use:
* Listeners should be associated with a visual source file buffer
* Use fragments like this to watch the DependencyFiles:

        // Get the service
        let vsFileWatch = fls.GetService(typeof<SVsFileChangeEx >) :?> IVsFileChangeEx

        // Watch the Add and Change events
        let fileChangeFlags = 
            uint32 (_VSFILECHANGEFLAGS.VSFILECHG_Add ||| 
                    // _VSFILECHANGEFLAGS.VSFILECHG_Del ||| // don't listen for deletes - if a file (such as a 'Clean'ed project reference) is deleted, just keep using stale info
                    _VSFILECHANGEFLAGS.VSFILECHG_Time)

        // Advise on file changes...
        let cookie = Com.ThrowOnFailure1(vsFileWatch.AdviseFileChange(file, fileChangeFlags, changeEvents))

        ...
        
        // Unadvised file changes...
        Com.ThrowOnFailure0(vsFileWatch.UnadviseFileChange(cookie))


*)
