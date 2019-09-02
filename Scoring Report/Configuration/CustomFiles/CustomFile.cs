using Scoring_Report.Configuration.Comparisons;
using System.IO;

namespace Scoring_Report.Configuration.CustomFiles
{
    public class CustomFile
    {
        public string Path = "";

        public IComparison[] Comparisons = null;

        public bool Matches = true;

        public string CustomOutput = "";

        public bool IsScored = false;

        public static CustomFile Parse(BinaryReader reader)
        {
            // Create instance
            CustomFile customFile = new CustomFile();

            customFile.Path = reader.ReadString();
            customFile.Matches = reader.ReadBoolean();

            int comparisonCount = reader.ReadInt32();

            customFile.Comparisons = new IComparison[comparisonCount];

            for (int i = 0; i < comparisonCount; i++)
            {
                EComparison type = (EComparison)reader.ReadInt32();
                switch (type)
                {
                    case EComparison.Simple:
                        customFile.Comparisons[i] = new ComparisonSimple();
                        break;
                    case EComparison.Regex:
                        customFile.Comparisons[i] = new ComparisonRegex();
                        break;
                }

                customFile.Comparisons[i].Load(reader);
            }

            // Read scoring values
            customFile.CustomOutput = reader.ReadString();
            customFile.IsScored = reader.ReadBoolean();

            return customFile;
        }
    }
}
