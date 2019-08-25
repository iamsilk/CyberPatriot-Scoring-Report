using Scoring_Report.Configuration.Forensics;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionForensics : ISection
    {
        public ESectionType Type => ESectionType.Forensics;

        public string Header => TranslationManager.Translate("SectionForensics");

        public static List<Question> Questions { get; } = new List<Question>();

        public int MaxScore()
        {
            return Questions.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // For each forensic question
            foreach (Question question in Questions)
            {
                // If question/answer file doesn't exist, skip
                if (!File.Exists(question.Path)) continue;

                // Read all lines
                string[] lines = File.ReadAllLines(question.Path);

                string answerLine = null;

                // Find line with answer
                foreach (string line in lines)
                {
                    if (line.StartsWith(question.AnswerPrefix))
                    {
                        answerLine = line;
                        break;
                    }
                }

                // If no answer line is found, skip
                if (answerLine == null) continue;

                // Get string with just answer
                string answer = answerLine.Substring(question.AnswerPrefix.Length);

                // Check all possible answers for match
                foreach (IAnswer possibleAnswer in question.Answers)
                {
                    bool matches = false;

                    switch (possibleAnswer.Type)
                    {
                        case EAnswerType.Text:
                            if (answer == possibleAnswer.Info)
                                matches = true;
                            break;
                        case EAnswerType.Regex:
                            // Placed in try/catch incase format is incorrect
                            try
                            {
                                Regex regex = new Regex(possibleAnswer.Info);
                                matches = regex.IsMatch(answer);
                            }
                            catch { }
                            break;
                    }

                    // If match is found, give score and break loop
                    if (matches)
                    {
                        details.Points++;
                        details.Output.Add(TranslationManager.Translate("AnswerCorrect", question.Title));

                        break;
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear questions
            Questions.Clear();

            // Get number of questions
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get question info
                Question question = Question.Parse(reader);

                // Add to list
                Questions.Add(question);
            }
        }
    }
}
