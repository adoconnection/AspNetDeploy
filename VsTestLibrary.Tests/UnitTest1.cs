using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VsTestLibrary.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            VsTestParser parser = new VsTestParser();
            IList<VsTestClassInfo> testClasses = parser.Parse(@"H:\AspNetDeploy\Variables.Tests\bin\Debug\Variables.Tests.dll");

            foreach (VsTestClassInfo testClass in testClasses)
            {
                VsTestRunner runner = new VsTestRunner();

                Console.WriteLine(testClass.Type.FullName);

                foreach (string testMethod in testClass.TestMethods)
                {
                    VsTestRunResult vsTestRunResult = runner.Run(testClass.Type, testClass.InitializeMethod, testMethod, testClass.CleanupMethod);
                    Console.WriteLine((vsTestRunResult.IsSuccess ? "PASS" : "FAIL") + " - " + testMethod + " - " + vsTestRunResult.Exception?.Message);
                }
            }

        }
    }
}
