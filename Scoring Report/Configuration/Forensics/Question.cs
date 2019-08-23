using System.Collections.Generic;
using System.IO;

namespace Scoring_Report.Configuration.Forensics
{
    public class Question
    {
        public string Path = "";

        public string AnswerPrefix = "";

        public string Title = "";

        public List<IAnswer> Answers = new List<IAnswer>();

        public static Question Parse(BinaryReader reader)
        {
            Question question = new Question();

            question.Path = reader.ReadString();
            question.AnswerPrefix = reader.ReadString();
            question.Title = reader.ReadString();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                IAnswer answer = null;

                EAnswerType type = (EAnswerType)reader.ReadInt32();
                string info = reader.ReadString();

                switch (type)
                {
                    case EAnswerType.Text:
                        answer = new AnswerText();
                        break;
                    case EAnswerType.Regex:
                        answer = new AnswerRegex();
                        break;
                }

                if (answer == null) continue;

                answer.Info = info;

                question.Answers.Add(answer);
            }

            return question;
        }
    }
}
