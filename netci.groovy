// TODO: the behaviour belyw matches the same appveyor workflow we have now.
// The build scripts will need to be updated later to improve the process.

// Import the utility functionality.
import jobs.generation.Utilities;

// Defines a the new of the repo, used elsewhere in the file
def project = GithubProject

// Generate the builds for different test runs
[
	'net40,portable7,portable47,portable78,portable259,vs',
	'cambridge_suite',
	'qa_suite'
]
.each { configuration ->

	// Generate the builds for debug and release, commit and PRJob
	[true, false].each { isPR -> // Defines a closure over true and false, value assigned to isPR
        
        // Determine the name for the new job.  The first parameter is the project,
        // the second parameter is the base name for the job, and the last parameter
        // is a boolean indicating whether the job will be a PR job.  If true, the
        // suffix _prtest will be appended.
        def newJobName = Utilities.getFullJobName(project, configuration, isPR)
        
        // Define build string
		def buildString = """call appveyor-build.cmd ${configuration}"""

        // Create a new job with the specified name.  The brace opens a new closure
        // and calls made within that closure apply to the newly created job.
        def newJob = job(newJobName) {
            // Indicate this job runs on windows machines.
            // The valid labels right now are:
            // windows (server 2012 R2)
            // ubuntu (v. 14.04)
            // centos-71
            // openSuSE-132
            // mac
            // freebsd
            label('windows')
            
            // This opens the set of build steps that will be run.
            steps {
                // Indicates that a batch script should be run with the build string (see above)
                // Also available is:
                // shell (for unix scripting)
                batchFile(buildString)
            }
        }
        
        // This call performs remaining common job setup on the newly created job.
        // This is used most commonly for simple inner loop testing.
        // It does the following:
        //   1. Sets up source control for the project.
        //   2. Adds a push trigger if the job is a PR job
        //   3. Adds a github PR trigger if the job is a PR job.
        //      The optional context (label that you see on github in the PR checks) is added.
        //      If not provided the context defaults to the job name.
        //   4. Adds standard options for build retention and timeouts
        //   5. Adds standard parameters for PR and push jobs.
        //      These allow PR jobs to be used for simple private testing, for instance.
        // See the documentation for this function to see additional optional parameters.
        Utilities.simpleInnerLoopJobSetup(newJob, project, isPR, "Windows ${configuration}")
    }
}