using Unity.NetCode;

[UnityEngine.Scripting.Preserve]
public class AutoConnectBootstrap : ClientServerBootstrap
{
	public override bool Initialize(string defaultWorldName)
	{
		// Disable the automtic client/server connection.
		return false;
	}
}