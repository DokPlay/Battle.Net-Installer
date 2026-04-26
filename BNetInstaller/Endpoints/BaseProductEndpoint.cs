namespace BNetInstaller.Endpoints;

internal abstract class BaseProductEndpoint<T>(string endpoint, AgentClient client) : BaseEndpoint<T>(endpoint, client) where T : class, IModel, new()
{
    public ProductEndpoint Product { get; private set; }

    public override async Task<JsonNode> Post()
    {
        var content = await base.Post();
        Product = ProductEndpoint.CreateFromResponse(content, Client);

        if (Product == null)
            throw new AgentException(0, "Agent Error: response_uri was missing from the Agent response.", content?.ToJsonString());

        return content;
    }
}
