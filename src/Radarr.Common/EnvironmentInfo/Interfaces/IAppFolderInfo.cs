namespace Radarr.Common.EnvironmentInfo.Interfaces
{
    public interface IAppFolderInfo
    {
        string AppDataFolder { get; }

        string TempFolder { get; }

        string StartUpFolder { get; }
    }
}
