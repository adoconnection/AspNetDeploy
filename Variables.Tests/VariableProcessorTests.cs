using System;
using System.Collections.Generic;
using AspNetDeploy.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Variables.Tests
{
    [TestClass]
    public class VariableProcessorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            VariableProcessor variableProcessor = new VariableProcessor(
                new Dictionary<string, string>()
                {
                    { "nick", "shade" },
                    { "path", @"c:\windows\System32" }
                },
                new Dictionary<string, string>()
                {
                    { "version", "1.3" }
                },
                new Dictionary<string, Func<string, string>>()
                {
                    { "tolower", s => s.ToLower() },
                    { "sslsafe", s => s.Replace(".", "-") },
                });

            Assert.AreEqual("subject: shade, 1-3", variableProcessor.ProcessValue("subject: {var:nick}, {env:version:sslSafe}"));
        }
    }
}
