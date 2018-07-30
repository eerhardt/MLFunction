// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.WebJobs.Host;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MLFunction
{
    public class GitHubIssue
    {
        [Column(ordinal: "0")]
        public string ID;

        [JsonIgnore]
        [Column(ordinal: "1")]
        public string Area;

        [Column(ordinal: "2")]
        public string Title;

        [DataMember(Name = "body")]
        [Column(ordinal: "3")]
        public string Description;

        [DataMember(Name = "labels")]
        public List<object> Labels { get; set; }

        [DataMember(Name = "number")]
        public int Number { get; set; }
    }

    public class GitHubIssuePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Area;

        [ColumnName("Score")]
        public float[] Probabilities;
    }

    internal static class Predictor
    {
        private static string ModelPath => Path.Combine(
            Path.GetDirectoryName(typeof(Predictor).Assembly.Location),
            "..",
            "model",
            "GitHubIssueLabelerModel.zip");

        public static async Task<string> PredictAsync(GitHubIssue issue, TraceWriter log)
        {
            PredictionModel<GitHubIssue, GitHubIssuePrediction> model = await PredictionModel.ReadAsync<GitHubIssue, GitHubIssuePrediction>(ModelPath);
            GitHubIssuePrediction prediction = model.Predict(issue);

            float[] probabilities = prediction.Probabilities;
            float maxProbability = probabilities.Max();
            log.Info($"Label {prediction.Area} for {issue.ID} is predicted with confidence {maxProbability.ToString()}");

            return maxProbability > 0.8 ? prediction.Area : null;
        }
    }
}