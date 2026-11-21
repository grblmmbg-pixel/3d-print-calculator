using System;
using System.Collections.Generic;

namespace PrintCalc
{
    public static class Loc
    {
        public static string CurrentLanguage = "uk";
        public static event Action LanguageChanged;

        private static readonly Dictionary<string, Dictionary<string, string>> Dictionaries =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        static Loc()
        {
            // UKRAINIAN (uk) - DEFAULT
            var uk = new Dictionary<string, string>
            {
                { "AppTitle", "Калькулятор 3D-друку" },
                { "Subtitle", "Точний розрахунок собівартості та комерційної ціни 3D-друку" },
                { "CalcTab", "Калькулятор" },
                { "FilamentsTab", "База філаментів" },
                { "FilamentLabel", "Філамент:" },
                { "ManageFilaments", "Управління базою" },
                { "WeightLabel", "Витрата філаменту на виріб:" },
                { "GramsUnit", "г" },
                { "PrintTimeLabel", "Час 3D-друку:" },
                { "HoursLabel", "годин" },
                { "MinutesLabel", "хвилин" },
                { "HourlyRateLabel", "Вартість 1 години роботи принтера:" },
                { "HourlyRateHint", "Електроенергія, амортизація, обслуговування" },
                { "MarkupLabel", "Відсоток націнки (маржа):" },
                { "SummaryHeader", "Розрахунок вартості" },
                { "FilamentCost", "Вартість філаменту:" },
                { "TimeCost", "Вартість машинного часу:" },
                { "BaseCost", "Собівартість виробу:" },
                { "MarkupAmount", "Сума націнки (прибуток):" },
                { "TotalExclVat", "ЗАГАЛЬНА ВАРТІСТЬ (БЕЗ ПДВ):" },
                { "VatLabel", "ПДВ (20%):" },
                { "TotalInclVat", "РАЗОМ ДО СПЛАТИ З ПДВ (20%):" },
                { "BtnCopy", "Скопіювати кошторис" },
                { "BtnReset", "Скинути поля" },
                { "BtnAddFilament", "+ Додати філамент" },
                { "BtnEdit", "Редагувати" },
                { "BtnDelete", "Видалити" },
                { "BtnSave", "Зберегти" },
                { "BtnCancel", "Скасувати" },
                { "ColName", "Назва / Бренд" },
                { "ColType", "Тип пластику" },
                { "ColDensity", "Щільність (г/см³)" },
                { "ColPrice", "Ціна за 1 кг" },
                { "Density", "Щільність:" },
                { "CostPerKg", "Ціна/кг:" },
                { "EditorTitleNew", "Новий філамент" },
                { "EditorTitleEdit", "Редагування філаменту" },
                { "FieldName", "Назва:" },
                { "FieldType", "Тип (PLA, PETG, ABS...):" },
                { "FieldDensity", "Щільність (г/см³):" },
                { "FieldPrice", "Ціна за котушку 1 кг:" },
                { "CopiedSuccess", "Кошторис успішно скопійовано в буфер обміну!" },
                { "ConfirmDelete", "Ви дійсно бажаєте видалити цей філамент?" },
                { "QuickSelect", "Швидкий вибір:" },
                { "CurrencyLabel", "Валюта:" },
                { "LanguageLabel", "Мова:" },
                { "FormulaNote", "Формула: (Філамент + Машино-години) × (1 + Націнка%)" },
                { "QuoteTitle", "РОЗРАХУНОК ВАРТОСТІ 3D-ДРУКУ" },
                { "QuoteMaterial", "Матеріал" },
                { "QuoteWeight", "Витрата пластику" },
                { "QuoteTime", "Час друку" },
                { "QuoteRate", "Тариф години" },
                { "QuoteFilamentCost", "Вартість матеріалу" },
                { "QuoteTimeCost", "Вартість часу роботи" },
                { "QuoteBase", "Собівартість" },
                { "QuoteMarkup", "Націнка" },
                { "QuoteSubtotal", "Разом (без ПДВ)" },
                { "QuoteVat", "ПДВ (20%)" },
                { "QuoteTotalWithVat", "РАЗОМ ДО СПЛАТИ З ПДВ" }
            };

            // ENGLISH (en)
            var en = new Dictionary<string, string>
            {
                { "AppTitle", "3D Printing Cost Calculator" },
                { "Subtitle", "Accurate production cost & commercial price estimation" },
                { "CalcTab", "Calculator" },
                { "FilamentsTab", "Filament Library" },
                { "FilamentLabel", "Filament Material:" },
                { "ManageFilaments", "Manage Library" },
                { "WeightLabel", "Required Filament Weight:" },
                { "GramsUnit", "g" },
                { "PrintTimeLabel", "Print Duration:" },
                { "HoursLabel", "hours" },
                { "MinutesLabel", "mins" },
                { "HourlyRateLabel", "Printer Machine Hourly Rate:" },
                { "HourlyRateHint", "Electricity, wear, depreciation, maintenance" },
                { "MarkupLabel", "Markup / Profit Margin (%):" },
                { "SummaryHeader", "Cost Breakdown" },
                { "FilamentCost", "Filament Cost:" },
                { "TimeCost", "Machine Time Cost:" },
                { "BaseCost", "Base Production Cost:" },
                { "MarkupAmount", "Markup / Profit Amount:" },
                { "TotalExclVat", "TOTAL PRICE (EXCL. VAT):" },
                { "VatLabel", "VAT (20%):" },
                { "TotalInclVat", "TOTAL PAYABLE (INCL. 20% VAT):" },
                { "BtnCopy", "Copy Full Quote" },
                { "BtnReset", "Reset All" },
                { "BtnAddFilament", "+ Add Filament" },
                { "BtnEdit", "Edit" },
                { "BtnDelete", "Delete" },
                { "BtnSave", "Save" },
                { "BtnCancel", "Cancel" },
                { "ColName", "Name / Brand" },
                { "ColType", "Polymer Type" },
                { "ColDensity", "Density (g/cm³)" },
                { "ColPrice", "Price per 1 kg" },
                { "Density", "Density:" },
                { "CostPerKg", "Price/kg:" },
                { "EditorTitleNew", "New Filament" },
                { "EditorTitleEdit", "Edit Filament" },
                { "FieldName", "Name:" },
                { "FieldType", "Type (PLA, PETG, ABS...):" },
                { "FieldDensity", "Density (g/cm³):" },
                { "FieldPrice", "Price per 1 kg spool:" },
                { "CopiedSuccess", "Quote copied to clipboard!" },
                { "ConfirmDelete", "Are you sure you want to delete this filament?" },
                { "QuickSelect", "Quick select:" },
                { "CurrencyLabel", "Currency:" },
                { "LanguageLabel", "Language:" },
                { "FormulaNote", "Formula: (Filament + Print Time) × (1 + Markup%)" },
                { "QuoteTitle", "3D PRINTING PRICE ESTIMATE" },
                { "QuoteMaterial", "Material" },
                { "QuoteWeight", "Filament Weight" },
                { "QuoteTime", "Print Time" },
                { "QuoteRate", "Hourly Rate" },
                { "QuoteFilamentCost", "Material Cost" },
                { "QuoteTimeCost", "Machine Time Cost" },
                { "QuoteBase", "Production Cost" },
                { "QuoteMarkup", "Markup" },
                { "QuoteSubtotal", "Subtotal (excl. VAT)" },
                { "QuoteVat", "VAT (20%)" },
                { "QuoteTotalWithVat", "TOTAL PAYABLE (INCL. VAT)" }
            };

            // BULGARIAN (bg)
            var bg = new Dictionary<string, string>
            {
                { "AppTitle", "Калкулатор за 3D принтиране" },
                { "Subtitle", "Точно изчисляване на себестойност и крайна цена за 3D печат" },
                { "CalcTab", "Калкулатор" },
                { "FilamentsTab", "База филаменти" },
                { "FilamentLabel", "Филамент:" },
                { "ManageFilaments", "Управление на базата" },
                { "WeightLabel", "Количество филамент за детайла:" },
                { "GramsUnit", "гр" },
                { "PrintTimeLabel", "Време за 3D печат:" },
                { "HoursLabel", "часа" },
                { "MinutesLabel", "минути" },
                { "HourlyRateLabel", "Цена за 1 час работа на принтера:" },
                { "HourlyRateHint", "Електроенергия, амортизация, поддръжка" },
                { "MarkupLabel", "Процент надценка (печалба):" },
                { "SummaryHeader", "Детайли на калкулацията" },
                { "FilamentCost", "Стойност на филамента:" },
                { "TimeCost", "Стойност на машинното време:" },
                { "BaseCost", "Себестойност на детайла:" },
                { "MarkupAmount", "Надценка (печалба):" },
                { "TotalExclVat", "ОБЩА СТОЙНОСТ (БЕЗ ДДС):" },
                { "VatLabel", "ДДС (20%):" },
                { "TotalInclVat", "ОБЩО ЗА ПЛАЩАНЕ С ДДС (20%):" },
                { "BtnCopy", "Копирай офертата" },
                { "BtnReset", "Изчисти полетата" },
                { "BtnAddFilament", "+ Добави филамент" },
                { "BtnEdit", "Редактирай" },
                { "BtnDelete", "Изтрий" },
                { "BtnSave", "Запази" },
                { "BtnCancel", "Отказ" },
                { "ColName", "Име / Марка" },
                { "ColType", "Вид полимер" },
                { "ColDensity", "Плътност (г/см³)" },
                { "ColPrice", "Цена за 1 кг" },
                { "Density", "Плътност:" },
                { "CostPerKg", "Цена/кг:" },
                { "EditorTitleNew", "Нов филамент" },
                { "EditorTitleEdit", "Редактиране на филамент" },
                { "FieldName", "Име:" },
                { "FieldType", "Вид (PLA, PETG, ABS...):" },
                { "FieldDensity", "Плътност (г/см³):" },
                { "FieldPrice", "Цена за 1 кг ролка:" },
                { "CopiedSuccess", "Офертата е копирана успешно в клипборда!" },
                { "ConfirmDelete", "Сигурни ли сте, че искате да изтриете този филамент?" },
                { "QuickSelect", "Бърз избор:" },
                { "CurrencyLabel", "Валута:" },
                { "LanguageLabel", "Език:" },
                { "FormulaNote", "Формула: (Филамент + Време за печат) × (1 + Надценка%)" },
                { "QuoteTitle", "ОФЕРТА ЗА 3D ПРИНТИРАНЕ" },
                { "QuoteMaterial", "Материал" },
                { "QuoteWeight", "Разход на филамент" },
                { "QuoteTime", "Време за печат" },
                { "QuoteRate", "Часова тарифа" },
                { "QuoteFilamentCost", "Стойност на материала" },
                { "QuoteTimeCost", "Стойност на времето" },
                { "QuoteBase", "Себестойност" },
                { "QuoteMarkup", "Надценка" },
                { "QuoteSubtotal", "Общо (без ДДС)" },
                { "QuoteVat", "ДДС (20%)" },
                { "QuoteTotalWithVat", "ОБЩО ЗА ПЛАЩАНЕ С ДДС" }
            };

            Dictionaries["uk"] = uk;
            Dictionaries["en"] = en;
            Dictionaries["bg"] = bg;
        }

        public static void SetLanguage(string lang)
        {
            if (Dictionaries.ContainsKey(lang))
            {
                CurrentLanguage = lang.ToLower();
                if (LanguageChanged != null)
                    LanguageChanged();
            }
        }

        public static string T(string key)
        {
            Dictionary<string, string> dict;
            if (Dictionaries.TryGetValue(CurrentLanguage, out dict))
            {
                string val;
                if (dict.TryGetValue(key, out val))
                    return val;
            }

            // Fallback to Ukrainian or English
            if (Dictionaries["uk"].ContainsKey(key))
                return Dictionaries["uk"][key];
            if (Dictionaries["en"].ContainsKey(key))
                return Dictionaries["en"][key];

            return key;
        }
    }
}
