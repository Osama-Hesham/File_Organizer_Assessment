using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class FileOrganizer
{
    static readonly Dictionary<string, string[]> Categories = new Dictionary<string, string[]>
    {
        {"Images", new []{".jpg", ".jpeg", ".png", ".bmp"}},
        {"Documents", new []{".txt", ".pdf", ".docx", ".doc",".xlsx", ".pptx", ".csv"}},
        {"Videos", new []{".mp4", ".mkv", ".wmv"}},
        {"Music", new []{".mp3", ".wav"}}
    };

    static void OrganizeFiles(string folderPath, bool simulate)
    {
        var categories = Categories.Keys.ToList();
        categories.Add("Others");

        foreach (var category in categories)
        {
            string categoryFile = Path.Combine(folderPath, category);

            if (File.Exists(categoryFile) && string.IsNullOrEmpty(Path.GetExtension(categoryFile)))
            {
                string newFileName = $"{category}_{Guid.NewGuid()}";
                string newFilePath = Path.Combine(folderPath, newFileName);
                File.Move(categoryFile, newFilePath);
            }
        }

        if (!simulate)
        {
            foreach (var category in categories)
            {
                string subfolder = Path.Combine(folderPath, category);
                if (!Directory.Exists(subfolder))
                {
                    Directory.CreateDirectory(subfolder);
                }
            }
        }

        var files = Directory.GetFiles(folderPath);
        var summary = new Dictionary<string, int>();

        foreach (var category in categories)
        {
            summary[category] = 0;
        }

        foreach (var filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            string extension = Path.GetExtension(fileName).ToLower();
            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string category = "Others";

            foreach (var pair in Categories)
            {
                if (pair.Value.Contains(extension))
                {
                    category = pair.Key;
                    break;
                }
            }

            string targetPath = Path.Combine(folderPath, category, fileName);

            if (File.Exists(targetPath))
            {
                string uniqueName = $"{nameWithoutExt}_{Guid.NewGuid()}{extension}";
                targetPath = Path.Combine(folderPath, category, uniqueName);
            }

            if (simulate)
            {
                Console.WriteLine($"[SIMULATE] Moved: {fileName} -> {category}/");
            }
            else
            {
                if (!filePath.Equals(targetPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Move(filePath, targetPath);
                    Console.WriteLine($"Moved: {fileName} -> {category}/");
                }
            }
            summary[category]++;
        }

        Console.WriteLine("\nSummary:");
        foreach (var cat in categories)
        {
            Console.WriteLine($"{cat}: {summary[cat]} file(s)");
        }
    }

    static void Main()
    {
        while (true)
        {
            Console.Write("Insert a folder path to organize: ");
            string folderPath = Console.ReadLine().Trim();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The provided path does not exist!\n");
                continue;
            }

            Console.Write("Simulate? (y/n): ");
            string simulateInput = Console.ReadLine().Trim().ToLower();
            bool simulate = false;

            if (simulateInput != "y" && simulateInput != "n")
            {
                Console.WriteLine("Please insert 'n' or 'y' only!\n");
                continue;
            }
            else if (simulateInput == "y")
            {
                simulate = true;
            }

            OrganizeFiles(folderPath, simulate);
            break;
        }
    }
}