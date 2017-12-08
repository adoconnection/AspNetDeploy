namespace AspNetDeploy.Contracts
{
    public class TestResult
    {
        public string TestClassName { get; set; }
        public string TestName { get; set; }
        public bool IsPass { get; set; }
        public string Message { get; set; }
    }
}