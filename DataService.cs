using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace PrintCalc
{
    public static class DataService
    {
        private static readonly string AppDataDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string FilamentsPath = Path.Combine(AppDataDir, "filaments.json");
        private static readonly string SettingsPath = Path.Combine(AppDataDir, "settings.json");

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public static List<Filament> GetDefaultFilaments()
        {
            return new List<Filament>
            {
                new Filament { Id = Guid.NewGuid().ToString(), Name = "PLA Standard (Bambu / eSUN)", Type = "PLA", Density = 1.24, CostPerKg = 20.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "PLA+ / Tough (eSUN / Sunlu)", Type = "PLA+", Density = 1.24, CostPerKg = 24.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "PETG Rapid (eSUN / Elegoo)", Type = "PETG", Density = 1.27, CostPerKg = 22.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "ABS Pro (Sunlu)", Type = "ABS", Density = 1.04, CostPerKg = 19.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "ASA UV-Resistant (Polymaker)", Type = "ASA", Density = 1.07, CostPerKg = 28.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "TPU 95A Flexible (SainSmart)", Type = "TPU", Density = 1.21, CostPerKg = 32.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "PC High Impact (Prusament)", Type = "PC", Density = 1.20, CostPerKg = 42.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "PA-CF Nylon Carbon (Bambu Lab)", Type = "Nylon/CF", Density = 1.18, CostPerKg = 55.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "PETG-CF Carbon Fiber", Type = "PETG/CF", Density = 1.30, CostPerKg = 38.00 },
                new Filament { Id = Guid.NewGuid().ToString(), Name = "Silk PLA Multi-Color / Dual", Type = "PLA Silk", Density = 1.24, CostPerKg = 26.00 }
            };
        }

        public static List<Filament> LoadFilaments()
        {
            try
            {
                if (File.Exists(FilamentsPath))
                {
                    string json = File.ReadAllText(FilamentsPath, Encoding.UTF8);
                    var list = Serializer.Deserialize<List<Filament>>(json);
                    if (list != null && list.Count > 0)
                        return list;
                }
            }
            catch
            {
                // Fallback to defaults
            }

            var defaults = GetDefaultFilaments();
            SaveFilaments(defaults);
            return defaults;
        }

        public static void SaveFilaments(List<Filament> filaments)
        {
            try
            {
                string json = Serializer.Serialize(filaments);
                File.WriteAllText(FilamentsPath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to save filaments: " + ex.Message);
            }
        }

        public static AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath, Encoding.UTF8);
                    var settings = Serializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                        return settings;
                }
            }
            catch
            {
                // Fallback to defaults
            }

            var defaultSettings = new AppSettings();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                string json = Serializer.Serialize(settings);
                File.WriteAllText(SettingsPath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to save settings: " + ex.Message);
            }
        }
    }
}
