using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public struct CreateAccountRequestRpc : IRpcCommand
{
	/// <summary>The new account's username.</summary>
	public FixedString32Bytes username;

	/// <summary>The new account's password.</summary>
	public FixedString32Bytes password;
}

public struct CreateAccountResponseRpc : IRpcCommand
{
	/// <summary>Whether or not the new account's creation was successful.</summary>
	public bool accepted;

	/// <summary>If account creation was unsuccessful, the reason why.</summary>
	public FixedString128Bytes reason;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerCreateAccountSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if create account requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<CreateAccountRequestRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		// Get all unprocessed create account requests and iterate through them all.
		foreach((RefRO<CreateAccountRequestRpc> createAccount, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<CreateAccountRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			// Convert the request's values into regular strings.
			string username = createAccount.ValueRO.username.ToString();
			string password = createAccount.ValueRO.password.ToString();

			// Verify the username.
			FixedString128Bytes reason = Account.VerifyUsername(username);

			// Verify the password.
			if(reason.Length == 0)
			{
				reason = Account.VerifyPassword(password);
			}

			// If both the username and password are valid, attempt to create the account.
			if(reason.Length == 0)
			{
				if(Account.HasFile(username))
				{
					reason = "An account with that name already exists.";
				}
				else
				{
					Account account = new Account(username, password);
					account.SaveToFile();
				}
			}

			// Send a response message to the client saying whether or not account creation was successful.
			Entity response = commandBuffer.CreateEntity();

			commandBuffer.AddComponent(response, new CreateAccountResponseRpc{accepted = reason.Length == 0, reason = reason});
			commandBuffer.AddComponent(response, new SendRpcCommandRequest{TargetConnection = request.ValueRO.SourceConnection});

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}