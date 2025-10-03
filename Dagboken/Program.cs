using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DagboksApp
{
    public class DiaryEntry
    {   //Lade till domänklass DiaryEntry med Date och Text
        public DateTime Date { get; set; } //Lade till funktion för att söka anteckningar baserat på datum
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd HH:mm} - {Text}";
        }
    }

    internal class Program
    {   //Införde Dictionary för snabbare uppslag av anteckningar efter datum
        static List<DiaryEntry> diaryEntries = new List<DiaryEntry>();
        static Dictionary<DateTime, DiaryEntry> diaryLookup = new Dictionary<DateTime, DiaryEntry>();
        const string filePath = "diary.json";

        
        static void ShowMenu()
        {   // meny funktion för att visa val
            Console.WriteLine("Välkommen till Dagboksappen");
            Console.WriteLine("1. Skriv en ny dagboksanteckning");
            Console.WriteLine("2. Visa alla dagboksanteckningar");
            Console.WriteLine("3. Sök anteckning på datum");
            Console.WriteLine("4. Spara till fil");
            Console.WriteLine("5. Läs från fil");
            Console.WriteLine("6. Avsluta");
            Console.Write("Val: ");
        }

        static void Main(string[] args)
        {
            LoadFromFile();
            //Skapade menystruktur med while-loop och switch-case
            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEntry();
                        break;
                    case "2":
                        ShowEntries();
                        break;
                    case "3":
                        SearchEntry();
                        break;
                    case "4":
                        SaveToFile();
                        break;
                    case "5":
                        LoadFromFile();
                        break;
                    case "6":
                        Console.WriteLine("Avslutar programmet.");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Fel val, försök igen.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void AddEntry() //Lagt till metod AddEntry() som tar in användarens text
        {
            Console.WriteLine("Skriv din dagboksanteckning");
            string text = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Anteckningen kan inte vara tom");
                return;
            }

            DiaryEntry entry = new DiaryEntry
            {
                Date = DateTime.Now,
                Text = text
            };

            diaryEntries.Add(entry);
            diaryLookup[entry.Date] = entry;

            Console.WriteLine("Anteckningen sparades");
        }

        static void ShowEntries()
        {
            if (diaryEntries.Count == 0)
            {
                Console.WriteLine("Det finns inga anteckningar ännu");
                return;
            }

            Console.WriteLine("Dina dagboksanteckningar");
            foreach (var entry in diaryEntries)
            {
                Console.WriteLine(entry);
            }
        }

        static void SearchEntry()
        {
            Console.Write("Ange datum (yyyy-MM-dd): ");
            string input = Console.ReadLine();

            if (!DateTime.TryParse(input, out DateTime date))
            {
                Console.WriteLine("fel datum");
                return;
            }

            bool found = false;
            foreach (var entry in diaryEntries)
            {
                if (entry.Date.Date == date.Date)
                {
                    Console.WriteLine(entry);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("Ingen anteckning hittades för det datumet.");
            }
        }

        static void SaveToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(diaryEntries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                Console.WriteLine("Anteckningarna sparades till fil.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande: {ex.Message}");
            }
        }

        static void LoadFromFile()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Ingen fil hittades. Börjar med en tom dagbok.");
                    return;
                }

                string json = File.ReadAllText(filePath);
                diaryEntries = JsonSerializer.Deserialize<List<DiaryEntry>>(json) ?? new List<DiaryEntry>();

                diaryLookup.Clear();
                foreach (var entry in diaryEntries)
                {
                    diaryLookup[entry.Date] = entry;
                }

                Console.WriteLine("Anteckningarna laddades från fil.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid inläsning: {ex.Message}");
                diaryEntries = new List<DiaryEntry>();
                diaryLookup = new Dictionary<DateTime, DiaryEntry>();
            }
        }
    }
}
