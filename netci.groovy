import jobs.generation.Utilities;
import jobs.generation.JobReport;

def project = GithubProject
def branch = GithubBranchName

def osList = ['Windows_NT', 'Ubuntu14.04']  //, 'OSX'], 'CentOS7.1'

def static getBuildJobName(def configuration, def os) {
    return configuration.toLowerCase() + '_' + os.toLowerCase()
}

[true, false].each { isPullRequest ->
    osList.each { os ->
        def configurations = ['Debug', 'Release_ci_part1', 'Release_ci_part2', 'Release_ci_part3', 'Release_ci_part4', 'Release_net40_no_vs' ];
        if (os != 'Windows_NT') {
            // Only build one configuration on Linux/... so far
            configurations = ['Release'];
        }
        configurations.each { configuration ->

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
                else if (configuration == "Release_ci_part2") {
                    build_args = "ci_part2"
                }
                else if (configuration == "Release_ci_part3") {
                    build_args = "ci_part3"
                }
                else if (configuration == "Release_ci_part4") {
                    build_args = "ci_part4"
                }
                else if (configuration == "Release_net40_no_vs") {
                    build_args = "net40"
                }
                else {
                    build_args = "ci"
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
                        batchFile("""
echo *** Build Visual F# Tools ***

.\\build.cmd ${buildFlavor} ${build_args}""")
                    }
                    else {
                        // Shell
                        shell(buildCommand)
                    }
                }
            }

            // TODO: set to false after tests are fully enabled
            def skipIfNoTestFiles = true

            def affinity = configuration == 'Release_net40_no_vs' ? 'latest-or-auto' : (os == 'Windows_NT' ? 'latest-or-auto-dev15-0' : 'latest-or-auto')
            Utilities.setMachineAffinity(newJob, os, affinity)
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
