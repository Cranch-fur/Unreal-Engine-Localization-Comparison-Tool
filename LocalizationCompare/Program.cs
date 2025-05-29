using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

class Program
{
    public static string programTitle = "Unreal Engine Localization Comparison Tool";
    private static ConsoleColor baseColor = ConsoleColor.Green;
    private static ConsoleColor comparisonColor = ConsoleColor.Red;




    static string RequestFilePath(string dialogTitle)
    {
        using (var openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Title = dialogTitle;
            openFileDialog.Filter = "JSON File (*.json)|*.json|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }


    static string SafeOperators(string input)
    {
        string safeOperatorsString = input;


        safeOperatorsString = safeOperatorsString.Replace("\n", "\\n");
        safeOperatorsString = safeOperatorsString.Replace("\t", "\\t");


        return safeOperatorsString;
    }


    static void CompareLocalizations(string baseFilePath, string comparisonFilePath)
    {
        JObject baseJson = JObject.Parse(File.ReadAllText(baseFilePath));
        JObject comparisonJson = JObject.Parse(File.ReadAllText(comparisonFilePath));


        foreach (var baseEntry in baseJson)
        {
            string key = baseEntry.Key;
            var baseObject = baseEntry.Value as JObject;
            var comparisonObject = comparisonJson[key] as JObject;

            if (baseObject == null)
            {
                Console.WriteLine();
                Console.ForegroundColor = baseColor;
                Console.WriteLine($"[{Path.GetFileNameWithoutExtension(baseFilePath)}] Localization Object Is Missing: \"{key}\"");
                Console.ResetColor();
                continue;
            }

            if (comparisonObject == null)
            {
                Console.WriteLine();
                Console.ForegroundColor = comparisonColor;
                Console.WriteLine($"[{Path.GetFileNameWithoutExtension(comparisonFilePath)}] Localization Object Is Missing: \"{key}\"");
                Console.ResetColor();
                continue;
            }

            foreach (var property in baseObject)
            {
                string propertyKey = property.Key;
                string baseValue = null;
                string comparisonValue = null;

                if (baseObject.ContainsKey(propertyKey))
                    baseValue = SafeOperators(property.Value.ToString());

                if (comparisonObject.ContainsKey(propertyKey))
                    comparisonValue = SafeOperators(comparisonObject[propertyKey]?.ToString());

                if (comparisonValue == null)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = baseColor;
                    Console.WriteLine($"[===BASE===] {key} → {propertyKey} → {baseValue}");
                    Console.ForegroundColor = comparisonColor;
                    Console.WriteLine($"[COMPARISON] Localization Key Is Missing");
                    Console.ResetColor();
                }
                else if (baseValue != comparisonValue)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = baseColor;
                    Console.WriteLine($"[===BASE===] {key} → {propertyKey} → {baseValue}");
                    Console.ForegroundColor = comparisonColor;
                    Console.WriteLine($"[COMPARISON] {key} → {propertyKey} → {comparisonValue}");
                    Console.ResetColor();
                }
            }
        }
    }




    [STAThread]
    static void Main()
    {
        Console.Title = programTitle;
        Console.WriteLine("");
        Console.WriteLine("         ██╗   ██╗███████╗██╗      ██████╗████████╗");
        Console.WriteLine("         ██║   ██║██╔════╝██║     ██╔════╝╚══██╔══╝");
        Console.WriteLine("         ██║   ██║█████╗  ██║     ██║        ██║   ");
        Console.WriteLine("         ██║   ██║██╔══╝  ██║     ██║        ██║   ");
        Console.WriteLine("         ╚██████╔╝███████╗███████╗╚██████╗   ██║   ");
        Console.WriteLine("          ╚═════╝ ╚══════╝╚══════╝ ╚═════╝   ╚═╝   ");
        Console.WriteLine("        Unreal Engine Localization Comparison Tool");
        Console.WriteLine("");


        string baseFilePath = null;
        bool baseFileSpecified = false;
        while (baseFileSpecified == false)
        {
            baseFilePath = RequestFilePath("Specify Base Localization File");
            if (File.Exists(baseFilePath) == false)
            {
                MessageBox.Show("Specified Base Localization File Wasn't Found!", programTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                continue;
            }

            baseFileSpecified = true;
        }
        Console.ForegroundColor = baseColor;
        Console.WriteLine($"Base Localization File: {baseFilePath}");


        string comparisonFilePath = null;
        bool comparisonFileSpecified = false;
        while (comparisonFileSpecified == false)
        {
            comparisonFilePath = RequestFilePath("Specify Comparison Localization File");
            if (File.Exists(comparisonFilePath) == false)
            {
                MessageBox.Show("Specified Comparison Localization File Wasn't Found!", programTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                continue;
            }

            comparisonFileSpecified = true;
        }
        Console.ForegroundColor = comparisonColor;
        Console.WriteLine($"Comparison Localization File: {baseFilePath}");
        Console.ResetColor();


        CompareLocalizations(baseFilePath, comparisonFilePath);


        Console.Write("");
        while (true)
        {
            // Console.Write("\n\nPress ENTER To Continue...");
            Console.ReadLine();
        }
    }
}