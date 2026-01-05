using Microsoft.ML;
using Microsoft.ML.Data;
namespace ResumeMatcherAPI.Services;

public class TextSimilarityService
{
    private readonly MLContext _mlContext = new();

    public float CalculateSimilarity(string resume, string jobDesc)
    {
        var data = new[]
        {
            new TextData { Text = resume },
            new TextData { Text = jobDesc }
        };

        var dataView = _mlContext.Data.LoadFromEnumerable(data);

        var pipeline = _mlContext.Transforms.Text
            .FeaturizeText("Features", nameof(TextData.Text));

        var model = pipeline.Fit(dataView);
        var transformed = model.Transform(dataView);

        var features = _mlContext.Data
            .CreateEnumerable<FeatureVector>(transformed, false)
            .ToList();

        return CosineSimilarity(features[0].Features, features[1].Features) * 100;
    }

    private float CosineSimilarity(float[] v1, float[] v2)
    {
        float dot = 0, mag1 = 0, mag2 = 0;

        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            mag1 += v1[i] * v1[i];
            mag2 += v2[i] * v2[i];
        }

        return dot / (MathF.Sqrt(mag1) * MathF.Sqrt(mag2));
    }

    private class TextData
    {
        public string Text { get; set; }
    }

    private class FeatureVector
    {
        [VectorType]
        public float[] Features { get; set; }
    }
}
