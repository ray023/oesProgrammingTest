using Microsoft.ProjectOxford.Common.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oesProgrammingTest.Models
{
    public class ClassMate
    {
        Dictionary<string, float> emotions = new Dictionary<string, float>();
        public ClassMate(string fileName, EmotionScores emotionScores)
        {
            FileName = fileName;
            EmotionScores = emotionScores;
            SetEmotionList(emotionScores);
        }

        private void SetEmotionList(EmotionScores emotionScores)
        {
            emotions.Add("Happy", emotionScores.Happiness);
            emotions.Add("Anger", emotionScores.Anger);
            emotions.Add("Contempt", emotionScores.Contempt);
            emotions.Add("Disgust", emotionScores.Disgust);
            emotions.Add("Fear", emotionScores.Fear);
            emotions.Add("Neutral", emotionScores.Neutral);
            emotions.Add("Sadness", emotionScores.Sadness);
            emotions.Add("Surprise", emotionScores.Surprise);
        }

        public string FileName { get; set; }
        public EmotionScores EmotionScores { get; set; }
        public BiggestEmotion GetBiggestEmotion()
        {
            var item = emotions
                        .OrderByDescending(x => x.Value)
                        .FirstOrDefault();
            return new BiggestEmotion()
            {
                Emotion = item.Key,
                Confidence = item.Value
            };

        }
        public override string ToString()
        {
            return $"{FileName} - {emotions.OrderByDescending(x => x.Value).FirstOrDefault().Key}";
        }
    }
}
