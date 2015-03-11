using System;

namespace SatelliteService.Contracts
{
    public interface IBackupRepository
    {
        Guid StoreObject(object obj);

        Guid StoreFile(string file);

        Guid StoreDirectory(string path);

        void RestoreFile(Guid guid, string file = null);

        void RestoreDirectory(Guid guid, string path = null);
        //void Cleanup();

        T RestoreObject<T>(Guid guid);
    }
}