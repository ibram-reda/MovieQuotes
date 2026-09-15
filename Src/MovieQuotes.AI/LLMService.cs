namespace MovieQuotes.AI;

using System.Text.Json;
using Microsoft.Extensions.AI;
using OllamaSharp;



public class LLMService
{
    IChatClient chatClient ;
    StudyMaterialAiGenerator generator;
    readonly string cacheDirectory;
    public LLMService(string OllamaApiUrl, string modelName)
    {
        chatClient = new OllamaApiClient(OllamaApiUrl, modelName);
        generator = new StudyMaterialAiGenerator(chatClient);
        cacheDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MovieQuotes",
                "cache");
    } 

    public async Task<StudyMaterialAiResponse?> GenerateAISuggestionForStudyMaterial(int id, string phrase, string content, CancellationToken cancellationToken = default)
    {
        var cachedResponse = await LoadResponseFromCacheAsync(id);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        var aiResponse = await generator.GenerateAsync(phrase, content, cancellationToken);
        if (aiResponse.Value != null)
        {
            await SaveResponseToCacheAsync(id, aiResponse.Value);
        }
        return aiResponse.Value;
    }

    // Save the generated response to a file
    public async Task SaveResponseToCacheAsync(int id, StudyMaterialAiResponse response)
    {
        var filePath = Path.Combine(cacheDirectory, $"response_{id}.json");
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    // Load the response from a file
    public async Task<StudyMaterialAiResponse?> LoadResponseFromCacheAsync(int id)
    {
        var filePath = Path.Combine(cacheDirectory, $"response_{id}.json");
        if (!File.Exists(filePath))
        {
            return null;
        }
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<StudyMaterialAiResponse>(json);
    }
}