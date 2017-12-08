using System;
using System.Collections.Generic;
using AspNetDeploy.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Variables.Tests
{
    [TestClass]
    public class VariableProcessorTests
    {
        private readonly Func<string, object> configurationProvider;

        public VariableProcessorTests()
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {"nick", "shade"}
            };

            this.configurationProvider = key => dictionary[key];
        }

        public VariableProcessorTests(Func<string, object> configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        [TestMethod]
        public void TestMethod1()
        {
            VariableProcessor variableProcessor = new VariableProcessor(
                new Dictionary<string, string>()
                {
                    { "nick", (string)this.configurationProvider("nick") },
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
