// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.WebJobs.Host;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

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
        [NoColumnAttribute]
        public List<object> Labels { get; set; }

        [DataMember(Name = "number")]
        [NoColumnAttribute]
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

        private static readonly LocalEnvironment s_environment = new LocalEnvironment();
        private static readonly Lazy<ITransformer> s_loadedModel = new Lazy<ITransformer>(LoadModel);

        private static ITransformer LoadModel()
        {
            using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return TransformerChain.LoadFrom(s_environment, stream);
            }
        }

        public static string Predict(GitHubIssue issue, TraceWriter log)
        {
            // Create prediction engine and test predictions.
            var engine = s_loadedModel.Value.MakePredictionFunction<GitHubIssue, GitHubIssuePrediction>(s_environment);

            GitHubIssuePrediction prediction = engine.Predict(issue);

            float[] probabilities = prediction.Probabilities;
            float maxProbability = probabilities.Max();
            log.Info($"Label {prediction.Area} for {issue.ID} is predicted with confidence {maxProbability.ToString()}");

            return prediction.Area;
        }
    }
}