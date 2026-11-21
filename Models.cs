using System;
using System.ComponentModel;

namespace PrintCalc
{
    public class Filament : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _type;
        private double _density;
        private double _costPerKg;

        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); OnPropertyChanged("DisplayName"); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged("Type"); OnPropertyChanged("DisplayName"); }
        }

        public double Density
        {
            get { return _density; }
            set { _density = value; OnPropertyChanged("Density"); OnPropertyChanged("DisplayName"); }
        }

        public double CostPerKg
        {
            get { return _costPerKg; }
            set { _costPerKg = value; OnPropertyChanged("CostPerKg"); OnPropertyChanged("DisplayName"); }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} ({1}) - {2:F2} / kg [{3:F2} g/cm³]", _name, _type, _costPerKg, _density);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public Filament Clone()
        {
            return new Filament
            {
                Id = this.Id,
                Name = this.Name,
                Type = this.Type,
                Density = this.Density,
                CostPerKg = this.CostPerKg
            };
        }
    }

    public class AppSettings
    {
        public string Language { get; set; }
        public string Currency { get; set; }
        public double HourlyRate { get; set; }
        public double MarkupPercent { get; set; }
        public double VatPercent { get; set; }
        public string SelectedFilamentId { get; set; }

        public AppSettings()
        {
            Language = "uk";
            Currency = "€";
            HourlyRate = 2.50;
            MarkupPercent = 40.0;
            VatPercent = 20.0;
            SelectedFilamentId = "";
        }
    }

    public class CalculationResult
    {
        public double WeightGrams { get; set; }
        public double FilamentCostPerKg { get; set; }
        public double FilamentCost { get; set; }

        public int PrintHours { get; set; }
        public int PrintMinutes { get; set; }
        public double TotalHours { get; set; }
        public double HourlyRate { get; set; }
        public double PrintTimeCost { get; set; }

        public double BaseCost { get; set; }
        public double MarkupPercent { get; set; }
        public double ProfitAmount { get; set; }

        public double TotalExclVat { get; set; }
        public double VatPercent { get; set; }
        public double VatAmount { get; set; }
        public double TotalInclVat { get; set; }

        public string Currency { get; set; }
    }
}
