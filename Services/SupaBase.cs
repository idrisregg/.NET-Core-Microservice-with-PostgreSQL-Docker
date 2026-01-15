using Supabase;

namespace Wajeb.API.Services;

public interface ISupabaseService
{
    Task<Client> GetClient();
}

public class SupabaseService : ISupabaseService
{
    private readonly string _url;
    private readonly string _key;
    private Client? _client;

    public SupabaseService(IConfiguration configuration)
    {
        _url = configuration["Supabase:Url"] ?? throw new ArgumentNullException(nameof(configuration), "Supabase:Url is not configured");
        _key = configuration["Supabase:Key"] ?? throw new ArgumentNullException(nameof(configuration), "Supabase:Key is not configured");
    }

    public async Task<Client> GetClient()
    {
        if (_client == null)
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            _client = new Client(_url, _key, options);
            await _client.InitializeAsync();
        }

        return _client;
    }
}