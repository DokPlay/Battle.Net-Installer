namespace BNetInstaller.Endpoints.Install;

internal sealed class InstallEndpoint(AgentClient client) : BaseProductEndpoint<InstallModel>("install", client)
{
    protected override void ValidateResponse(JsonNode response, string content)
    {
        var agentError = GetErrorCode(response["error"]);

        if (agentError <= 0)
            return;

        // try to identify the erroneous section
        foreach (var section in new[] { "authentication", "game_dir", "min_spec" })
        {
            var node = response["form"]?[section];
            var errorCode = GetErrorCode(node?["error"]);

            if (errorCode > 0)
                throw new AgentException(agentError, $"Agent Error: Unable to install - {errorCode} ({section}).", content, new Exception(content));
        }

        // fallback to throwing a global error
        throw new AgentException(agentError, $"Agent Error: {agentError} - {AgentException.Describe(agentError)}", content, new Exception(content));
    }
}
