using Microsoft.Extensions.Configuration;
using Octokit;
using PLDMS.BL.Common;

namespace PLDMS.BL.Utilities;

public class GitHubService
{
    private readonly GitHubClient _client;
    private readonly string _org;

    public GitHubService(GitHubClient client, IConfiguration config)
    {
        _client = client;
        _org = config["GitHub:Organization"]!;
    }

    public async Task<string> CreateRepoAsync(string repoName)
    {
        var newRepo = new NewRepository(repoName)
        {
            Private = true
        };

        var repo = await _client.Repository.Create(_org, newRepo);
        return repo.HtmlUrl;
    }

    public async Task DeleteRepoAsync(string repoName)
    {
        try
        {
            await _client.Repository.Delete(_org, repoName);
        }
        catch (Exception)
        {
            throw new BaseException("Failed to delete GitHub repository");
        }
    }
}