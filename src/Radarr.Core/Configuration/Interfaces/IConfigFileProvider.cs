using System.Collections.Generic;
using Radarr.Core.Authentication;
using Radarr.Core.Configuration.Commands;
using Radarr.Core.Lifecycle.Events;
using Radarr.Core.Messaging.Commands;
using Radarr.Core.Messaging.Events.Interfaces;
using Radarr.Core.Update;

namespace Radarr.Core.Configuration.Interfaces
{
    public interface IConfigFileProvider : 
        IHandleAsync<ApplicationStartedEvent>,
        IExecute<ResetApiKeyCommand>
    {
        Dictionary<string, object> GetConfigDictionary();

        void SaveConfigDictionary(Dictionary<string, object> configValues);

        string BindAddress { get; }

        int Port { get; }

        int SslPort { get; }

        bool EnableSsl { get; }

        bool LaunchBrowser { get; }

        AuthenticationType AuthenticationMethod { get; }

        bool AnalyticsEnabled { get; }

        string LogLevel { get; }

        string Branch { get; }

        string ApiKey { get; }

        string SslCertHash { get; }

        string UrlBase { get; }

        string UiFolder { get; }

        bool UpdateAutomatically { get; }

        UpdateMechanism UpdateMechanism { get; }

        string UpdateScriptPath { get; }
    }
}
