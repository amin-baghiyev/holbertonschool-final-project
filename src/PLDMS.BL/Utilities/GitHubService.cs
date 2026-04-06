using Octokit;
using PLDMS.BL.Common;

namespace PLDMS.BL.Utilities;

public class GitHubService
{
    private readonly GitHubClient _client;

    public GitHubService(GitHubClient client)
    {
        _client = client;
    }

    public async Task<string> CreateRepoAsync(string repoName)
    {
        var newRepo = new NewRepository(repoName)
        {
            Private = false,
            AutoInit = true
        };

        var repo = await _client.Repository.Create(newRepo);
        return repo.HtmlUrl;
    }

    public async Task DeleteRepoAsync(string repoName)
    {
        try
        {
            var user = await _client.User.Current();
            await _client.Repository.Delete(user.Login, repoName);
        }
        catch (Exception)
        {
            throw new BaseException("Failed to delete GitHub repository");
        }
    }

    public async Task<string> CommitCodeAsync(string repoUrl, string branchName, string filePath, string code, string commitMessage)
    {
        if (string.IsNullOrEmpty(repoUrl)) throw new ArgumentNullException(nameof(repoUrl));
        if (string.IsNullOrEmpty(branchName)) throw new ArgumentNullException(nameof(branchName));
        if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

        var parts = repoUrl.TrimEnd('/').Split('/');
        var repoName = parts[^1];
        var owner = parts[^2];

        try
        {
            var repo = await _client.Repository.Get(owner, repoName);
            var defaultBranch = repo.DefaultBranch;

            try
            {
                await _client.Repository.Branch.Get(owner, repoName, branchName);
            }
            catch (NotFoundException)
            {
                var defaultRef = await _client.Git.Reference.Get(owner, repoName, $"heads/{defaultBranch}");
                await _client.Git.Reference.Create(owner, repoName, new NewReference($"refs/heads/{branchName}", defaultRef.Object.Sha));
            }

            // Get file SHA if it exists for update
            string? sha = null;
            try
            {
                var contents = await _client.Repository.Content.GetAllContentsByRef(owner, repoName, filePath, branchName);
                if (contents != null && contents.Count > 0)
                    sha = contents[0].Sha;
            }
            catch (NotFoundException)
            {
            }

            if (sha == null)
            {
                var request = new CreateFileRequest(commitMessage, code, branchName);
                var result = await _client.Repository.Content.CreateFile(owner, repoName, filePath, request);
                return result.Commit.Sha;
            }
            else
            {
                var request = new UpdateFileRequest(commitMessage, code, sha, branchName);
                var result = await _client.Repository.Content.UpdateFile(owner, repoName, filePath, request);
                return result.Commit.Sha;
            }
        }
        catch (Exception ex)
        {
            throw new BaseException($"Failed to commit to GitHub ({owner}/{repoName}): {ex.Message}");
        }
    }

    public async Task<string?> GetFileContentAsync(string repoUrl, string branchName, string filePath, string commitSha)
    {
        var parts = repoUrl.TrimEnd('/').Split('/');
        var repoName = parts[^1];
        var owner = parts[^2];

        try
        {
            var contents = await _client.Repository.Content.GetAllContentsByRef(owner, repoName, filePath, commitSha);
            if (contents != null && contents.Count > 0)
                return contents[0].Content;

            return null;
        }
        catch (NotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw new BaseException($"Failed to fetch code from GitHub: {ex.Message}");
        }
    }
}