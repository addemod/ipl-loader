using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Controllers;
using Addemod.IPLLoader.Shared;

namespace Addemod.IPLLoader.Server
{
	[PublicAPI]
	public class IPLLoaderController : ConfigurableController<Configuration>
	{
		public IPLLoaderController(ILogger logger, Configuration configuration, ICommunicationManager comms) : base(logger, configuration)
		{
			// Send configuration when requested
			comms.Event(IPLLoaderEvents.Configuration).FromClients().OnRequest(e => e.Reply(this.Configuration));
		}
	}
}
