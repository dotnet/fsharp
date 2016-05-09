import jobs.generation.Utilities;
import jobs.generation.JobReport;

def project = GithubProject
def branch = GithubBranchName

def osList = ['Windows_NT'] //'Ubuntu', 'OSX', 'CentOS7.1'

def static getBuildJobName(def configuration, def os) {
    return configuration.toLowerCase() + '_' + os.toLowerCase()
}

[true, false].each { isPullRequest ->
    ['Debug', 'Release_ci_part1', 'Release_ci_part2'].each { configuration ->
        osList.each { os ->

            def lowerConfiguration = configuration.toLowerCase()

            // Calculate job name
            def jobName = getBuildJobName(configuration, os)

            def buildCommand = '';

            def buildFlavor= '';
            if (configuration == "Debug") {
                buildFlavor = "debug"
                build_args = ""
            }
            else {
                buildFlavor = "release"
                if (configuration == "Release_ci_part1") {
                    build_args = "ci_part1"
                }
                else {
                    build_args = "ci_part2"
                }
            }

            if (os == 'Windows_NT') {
                buildCommand = ".\\build.cmd ${buildFlavor} ${build_args}"
            }
            else {
                buildCommand = "./build.sh ${buildFlavor} ${build_args}"
            }

            def newJobName = Utilities.getFullJobName(project, jobName, isPullRequest)
            def newJob = job(newJobName) {
                steps {
                    if (os == 'Windows_NT') {
                        // Batch
                        batchFile(buildCommand)
                    }
                    else {
                        // Shell
                        shell(buildCommand)
                    }
                }
            }

            // TODO: set to false after tests are fully enabled
            def skipIfNoTestFiles = true

            Utilities.setMachineAffinity(newJob, os, os == 'Windows_NT' ? 'latest-dev15' : 'latest-or-auto')
            Utilities.standardJobSetup(newJob, project, isPullRequest, "*/${branch}")
            Utilities.addArchival(newJob, "tests/TestResults/*.*", "", skipIfNoTestFiles, false)
            Utilities.addArchival(newJob, "${buildFlavor}/**")

            if (isPullRequest) {
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "${os} ${configuration} Build")
            }
            else {
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}

JobReport.Report.generateJobReport(out)
