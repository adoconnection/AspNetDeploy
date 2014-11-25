namespace AspNetDeploy.Contracts
{
    public enum SatelliteState
    {
        Undefined = 0,
        Alive,
        UnableToEstablishSecureConnection,
        Inactive,
        NotConfigured,
        Timeout
    }
}