using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using VsTestLibrary;

namespace TestRunners.VsTests
{
    public class VisualStudioTestRunner : IProjectTestRunner
    {
        private readonly IVariableProcessor variableProcessor;
        private readonly IPathServices pathServices;

        public VisualStudioTestRunner(IVariableProcessor variableProcessor, IPathServices pathServices)
        {
            this.variableProcessor = variableProcessor;
            this.pathServices = pathServices;
        }

        public IList<TestResult> Run(ProjectVersion projectVersion)
        {
            string sourcesFolder = this.pathServices.GetSourceControlVersionPath(projectVersion.SourceControlVersion.SourceControlId, projectVersion.SourceControlVersion.Id);
            string projectPath = Path.Combine(sourcesFolder, projectVersion.ProjectFile);

            XDocument xDocument = XDocument.Load(projectPath);

            string outputPath = this.GetProperty(xDocument, "OutputPath");
            string assemblyName = this.GetProperty(xDocument, "AssemblyName");

            VsTestParser parser = new VsTestParser();
            IList<VsTestClassInfo> testClasses = parser.Parse(Path.Combine(Path.GetDirectoryName(projectPath), outputPath, assemblyName + ".dll"));

            IList<TestResult> result = new List<TestResult>();

            foreach (VsTestClassInfo testClass in testClasses)
            {
                VsTestRunner runner = new VsTestRunner();

                Console.WriteLine(testClass.Type.FullName);

                foreach (string testMethod in testClass.TestMethods)
                {
                    VsTestRunResult vsTestRunResult = runner.Run(testClass.Type, testClass.InitializeMethod, testMethod, testClass.CleanupMethod, variableProcessor.ProcessValue);

                    result.Add(new TestResult()
                    {
                        TestClassName = testClass.Type.FullName,
                        TestName = testMethod,
                        IsPass = vsTestRunResult.IsSuccess,
                        Message = vsTestRunResult.Exception?.Message + ". " + vsTestRunResult.Exception?.InnerException?.Message
                    });
                }
            }

            return result;
        }

        private string GetProperty(XDocument xDocument, string key)
        {
            XNamespace vsNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

            XElement outputPathRelease = xDocument.Descendants(vsNamespace + "PropertyGroup")
                    .Where(e => e.Attribute("Condition") != null)
                    .Where(e => e.Attribute("Condition").Value.Contains("$(Configuration)|$(Platform)' == 'Release|AnyCPU'"))
                    .Descendants(vsNamespace + key)
                    .FirstOrDefault();

            XElement outputPathDefault = xDocument.Descendants(vsNamespace + "PropertyGroup")
                    .Where(e => e.Attribute("Condition") == null)
                    .Descendants(vsNamespace + key)
                    .FirstOrDefault();

            return outputPathRelease != null ? outputPathRelease.Value : outputPathDefault?.Value;
        }
    }
}