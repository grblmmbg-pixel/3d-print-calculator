using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PrintCalc
{
    public class MainWindow : Window
    {
        // Colors & Theme Brushes
        public static readonly SolidColorBrush BgWindow = new SolidColorBrush(Color.FromRgb(15, 17, 23));
        public static readonly SolidColorBrush BgHeader = new SolidColorBrush(Color.FromRgb(22, 25, 34));
        public static readonly SolidColorBrush BgCard = new SolidColorBrush(Color.FromRgb(26, 30, 42));
        public static readonly SolidColorBrush BgCardInner = new SolidColorBrush(Color.FromRgb(19, 22, 31));
        public static readonly SolidColorBrush BgInput = new SolidColorBrush(Color.FromRgb(16, 18, 26));
        public static readonly SolidColorBrush BorderColor = new SolidColorBrush(Color.FromRgb(42, 47, 63));
        public static readonly SolidColorBrush BorderHover = new SolidColorBrush(Color.FromRgb(70, 78, 102));

        public static readonly SolidColorBrush AccentGreen = new SolidColorBrush(Color.FromRgb(16, 185, 129));
        public static readonly SolidColorBrush AccentGreenHover = new SolidColorBrush(Color.FromRgb(5, 150, 105));
        public static readonly SolidColorBrush AccentCyan = new SolidColorBrush(Color.FromRgb(56, 189, 248));
        public static readonly SolidColorBrush AccentAmber = new SolidColorBrush(Color.FromRgb(245, 158, 11));
        public static readonly SolidColorBrush AccentRed = new SolidColorBrush(Color.FromRgb(239, 68, 68));

        public static readonly SolidColorBrush TextPrimary = new SolidColorBrush(Color.FromRgb(243, 244, 246));
        public static readonly SolidColorBrush TextSecondary = new SolidColorBrush(Color.FromRgb(156, 163, 175));
        public static readonly SolidColorBrush TextMuted = new SolidColorBrush(Color.FromRgb(107, 114, 128));

        // State
        private AppSettings _settings;
        private List<Filament> _filaments;
        private Filament _selectedFilament;

        // UI Controls - Tabs
        private Button _btnTabCalc;
        private Button _btnTabFilaments;
        private Grid _viewCalculator;
        private Grid _viewFilaments;

        // UI Controls - Calculator Inputs
        private ComboBox _comboFilaments;
        private TextBlock _lblFilamentDensity;
        private TextBlock _lblFilamentPrice;
        private TextBox _txtWeight;
        private TextBox _txtHours;
        private TextBox _txtMinutes;
        private TextBox _txtHourlyRate;
        private TextBox _txtMarkup;

        // UI Controls - Result Display
        private TextBlock _resFilamentCost;
        private TextBlock _resPrintTimeCost;
        private TextBlock _resBaseCost;
        private TextBlock _resMarkupAmount;
        private TextBlock _resTotalExclVat;
        private TextBlock _resVatAmount;
        private TextBlock _resTotalInclVat;

        // UI Controls - Filaments Management
        private StackPanel _filamentsListPanel;
        private Border _filamentEditorCard;
        private TextBox _editName;
        private TextBox _editType;
        private TextBox _editDensity;
        private TextBox _editPrice;
        private Filament _currentEditingFilament = null;
        private TextBlock _editorTitle;

        // Header controls to update on language switch
        private TextBlock _headerTitle;
        private TextBlock _headerSubtitle;
        private TextBlock _lblSummaryHeader;
        private TextBlock _lblFilamentCostHeader;
        private TextBlock _lblTimeCostHeader;
        private TextBlock _lblBaseCostHeader;
        private TextBlock _lblMarkupHeader;
        private TextBlock _lblTotalExclVatHeader;
        private TextBlock _lblVatHeader;
        private TextBlock _lblTotalInclVatHeader;
        private TextBlock _lblInputFilament;
        private TextBlock _lblInputWeight;
        private TextBlock _lblInputTime;
        private TextBlock _lblInputHourlyRate;
        private TextBlock _lblInputHourlyHint;
        private TextBlock _lblInputMarkup;
        private TextBlock _lblFormulaNote;
        private Button _btnCopyQuote;
        private Button _btnResetInputs;
        private Button _btnAddFilamentTop;
        private Button _btnSaveFilament;
        private Button _btnCancelFilament;
        private TextBlock _lblHours;
        private TextBlock _lblMinutes;
        private TextBlock _lblUnitGrams;
        private Button _btnGoToFilaments;
        private List<Tuple<Button, string>> _weightChipsList = new List<Tuple<Button, string>>();
        private List<Tuple<Button, string>> _timeChipsList = new List<Tuple<Button, string>>();

        // Currency buttons
        private List<Button> _currencyButtons = new List<Button>();
        private List<Button> _langButtons = new List<Button>();

        public MainWindow()
        {
            _settings = DataService.LoadSettings();
            _filaments = DataService.LoadFilaments();
            
            if (_settings.Language == "ru" || string.IsNullOrEmpty(_settings.Language))
                _settings.Language = "uk";
            if (_settings.Currency != "€" && _settings.Currency != "₴")
                _settings.Currency = "€";
            DataService.SaveSettings(_settings);
            
            Loc.CurrentLanguage = _settings.Language;

            InitializeWindow();
            BuildUI();
            ApplyLanguage();
            UpdateCurrencyButtons();
            UpdateLanguageButtons();
            SelectInitialFilament();
            Recalculate();
        }

        private void InitializeWindow()
        {
            this.Title = Loc.T("AppTitle");
            this.Width = 1140;
            this.Height = 700;
            this.MinWidth = 980;
            this.MinHeight = 580;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Background = BgWindow;
            this.Foreground = TextPrimary;
            this.FontFamily = new FontFamily("Segoe UI, Arial, sans-serif");

            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri(iconPath));
                }
            }
            catch { }
        }

        private void BuildUI()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Top Bar
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Navigation Tabs
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content

            // 1. TOP HEADER BAR
            mainGrid.Children.Add(BuildHeaderBar());
            Grid.SetRow((UIElement)mainGrid.Children[0], 0);

            // 2. TABS BAR
            var tabsBar = BuildTabsBar();
            mainGrid.Children.Add(tabsBar);
            Grid.SetRow(tabsBar, 1);

            // 3. MAIN CONTENT CONTAINER
            var contentGrid = new Grid();
            _viewCalculator = BuildCalculatorView();
            _viewFilaments = BuildFilamentsView();

            contentGrid.Children.Add(_viewCalculator);
            contentGrid.Children.Add(_viewFilaments);
            _viewFilaments.Visibility = Visibility.Collapsed;

            mainGrid.Children.Add(contentGrid);
            Grid.SetRow(contentGrid, 2);

            this.Content = mainGrid;
        }

        private Border BuildHeaderBar()
        {
            var border = new Border
            {
                Background = BgHeader,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(24, 16, 24, 16)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Left: Logo & Title
            var titleStack = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            
            // 3D Box Icon badge
            var iconBorder = new Border
            {
                Width = 42,
                Height = 42,
                CornerRadius = new CornerRadius(10),
                Background = new LinearGradientBrush(Color.FromRgb(16, 185, 129), Color.FromRgb(6, 182, 212), 45),
                Margin = new Thickness(0, 0, 14, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            var iconText = new TextBlock
            {
                Text = "3D",
                FontWeight = FontWeights.ExtraBold,
                FontSize = 18,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            iconBorder.Child = iconText;
            titleStack.Children.Add(iconBorder);

            var textStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            _headerTitle = new TextBlock
            {
                Text = Loc.T("AppTitle"),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = TextPrimary
            };
            _headerSubtitle = new TextBlock
            {
                Text = Loc.T("Subtitle"),
                FontSize = 12,
                Foreground = TextSecondary,
                Margin = new Thickness(0, 2, 0, 0)
            };
            textStack.Children.Add(_headerTitle);
            textStack.Children.Add(_headerSubtitle);
            titleStack.Children.Add(textStack);

            grid.Children.Add(titleStack);
            Grid.SetColumn(titleStack, 0);

            // Right: Currency & Language Switchers
            var controlsStack = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            // Currencies: only EUR and UAH
            var currPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 18, 0) };
            string[] currencies = new string[] { "€", "₴" };
            foreach (var c in currencies)
            {
                var cur = c;
                var btn = CreatePillButton(cur, () => {
                    _settings.Currency = cur;
                    DataService.SaveSettings(_settings);
                    UpdateCurrencyButtons();
                    Recalculate();
                });
                _currencyButtons.Add(btn);
                currPanel.Children.Add(btn);
            }
            controlsStack.Children.Add(currPanel);

            // Divider
            var divider = new Border { Width = 1, Background = BorderColor, Height = 28, Margin = new Thickness(0, 0, 18, 0) };
            controlsStack.Children.Add(divider);

            // Languages: UK, EN, BG
            var langPanel = new StackPanel { Orientation = Orientation.Horizontal };
            string[] langs = new string[] { "uk", "en", "bg" };
            foreach (var l in langs)
            {
                var langCode = l;
                var btn = CreatePillButton(langCode.ToUpper(), () => {
                    SetLanguage(langCode);
                });
                _langButtons.Add(btn);
                langPanel.Children.Add(btn);
            }
            controlsStack.Children.Add(langPanel);

            grid.Children.Add(controlsStack);
            Grid.SetColumn(controlsStack, 1);

            border.Child = grid;
            return border;
        }

        private Border BuildTabsBar()
        {
            var border = new Border
            {
                Background = BgWindow,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(24, 10, 24, 0)
            };

            var stack = new StackPanel { Orientation = Orientation.Horizontal };

            _btnTabCalc = CreateTabButton("⚡ " + Loc.T("CalcTab"), true, () => SwitchTab(true));
            _btnTabFilaments = CreateTabButton("🧵 " + Loc.T("FilamentsTab"), false, () => SwitchTab(false));

            stack.Children.Add(_btnTabCalc);
            stack.Children.Add(_btnTabFilaments);

            border.Child = stack;
            return border;
        }

        private void SwitchTab(bool toCalc)
        {
            if (toCalc)
            {
                _viewCalculator.Visibility = Visibility.Visible;
                _viewFilaments.Visibility = Visibility.Collapsed;
                SetTabActive(_btnTabCalc, true);
                SetTabActive(_btnTabFilaments, false);
                ReloadFilamentsComboBox();
                Recalculate();
            }
            else
            {
                _viewCalculator.Visibility = Visibility.Collapsed;
                _viewFilaments.Visibility = Visibility.Visible;
                SetTabActive(_btnTabCalc, false);
                SetTabActive(_btnTabFilaments, true);
                RefreshFilamentsGrid();
            }
        }

        private Grid BuildCalculatorView()
        {
            var grid = new Grid { Margin = new Thickness(20, 12, 20, 14) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.35, GridUnitType.Star) }); // Inputs
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16) }); // Spacing
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) }); // Summary card

            // LEFT COLUMN: INPUTS (Wrapped in auto-scrollviewer only if window is shrunk)
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
            var inputStack = new StackPanel { Margin = new Thickness(0, 0, 6, 0) };

            // 1. FILAMENT SELECTION (Full Width at Top)
            _lblInputFilament = CreateSectionHeader("1. " + Loc.T("FilamentLabel"));
            inputStack.Children.Add(_lblInputFilament);

            var filCard = CreateCard();
            var filCardStack = new StackPanel();

            _comboFilaments = new ComboBox
            {
                Background = BgInput,
                Foreground = Brushes.White,
                BorderBrush = BorderColor,
                FontSize = 14,
                Padding = new Thickness(12, 9, 12, 9),
                Margin = new Thickness(0, 0, 0, 8)
            };
            _comboFilaments.SelectionChanged += (s, e) => {
                if (_comboFilaments.SelectedItem is Filament)
                {
                    _selectedFilament = (Filament)_comboFilaments.SelectedItem;
                    _settings.SelectedFilamentId = _selectedFilament.Id;
                    DataService.SaveSettings(_settings);
                    UpdateFilamentBadges();
                    Recalculate();
                }
            };
            filCardStack.Children.Add(_comboFilaments);

            var badgesRow = new StackPanel { Orientation = Orientation.Horizontal };
            _lblFilamentDensity = CreateBadge("1.24 g/cm³", AccentCyan);
            _lblFilamentPrice = CreateBadge("20.00 / kg", AccentGreen);
            _btnGoToFilaments = CreateLinkButton(Loc.T("ManageFilaments") + " →", () => SwitchTab(false));

            badgesRow.Children.Add(_lblFilamentDensity);
            badgesRow.Children.Add(_lblFilamentPrice);
            badgesRow.Children.Add(_btnGoToFilaments);
            filCardStack.Children.Add(badgesRow);

            filCard.Child = filCardStack;
            inputStack.Children.Add(filCard);

            // 2-COLUMN INPUTS GRID FOR REMAINING 4 SECTIONS
            var inputsGrid = new Grid();
            inputsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            inputsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            inputsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            inputsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            inputsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 2. WEIGHT INPUT (Col 0, Row 0)
            var weightPanel = new StackPanel();
            _lblInputWeight = CreateSectionHeader("2. " + Loc.T("WeightLabel"));
            weightPanel.Children.Add(_lblInputWeight);

            var weightCard = CreateCard();
            var weightStack = new StackPanel();

            var weightRow = new Grid();
            weightRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            weightRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            _txtWeight = CreateStyledTextBox("65");
            _txtWeight.TextChanged += (s, e) => Recalculate();
            weightRow.Children.Add(_txtWeight);
            Grid.SetColumn(_txtWeight, 0);

            _lblUnitGrams = new TextBlock
            {
                Text = Loc.T("GramsUnit"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = TextSecondary,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 4, 0)
            };
            weightRow.Children.Add(_lblUnitGrams);
            Grid.SetColumn(_lblUnitGrams, 1);
            weightStack.Children.Add(weightRow);

            _weightChipsList.Clear();
            var weightChips = new WrapPanel { Margin = new Thickness(0, 6, 0, 0) };
            string[] weights = new string[] { "25", "50", "100", "200", "350", "500" };
            foreach (var w in weights)
            {
                var val = w;
                var btn = CreateChip(val + " " + Loc.T("GramsUnit"), () => {
                    _txtWeight.Text = val;
                });
                _weightChipsList.Add(Tuple.Create(btn, val));
                weightChips.Children.Add(btn);
            }
            weightStack.Children.Add(weightChips);
            weightCard.Child = weightStack;
            weightPanel.Children.Add(weightCard);
            inputsGrid.Children.Add(weightPanel);
            Grid.SetColumn(weightPanel, 0);
            Grid.SetRow(weightPanel, 0);

            // 3. PRINT TIME INPUT (Col 2, Row 0)
            var timePanel = new StackPanel();
            _lblInputTime = CreateSectionHeader("3. " + Loc.T("PrintTimeLabel"));
            timePanel.Children.Add(_lblInputTime);

            var timeCard = CreateCard();
            var timeStack = new StackPanel();

            var timeRow = new Grid();
            timeRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            timeRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            timeRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            timeRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            timeRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            _txtHours = CreateStyledTextBox("3");
            _txtHours.TextChanged += (s, e) => Recalculate();
            timeRow.Children.Add(_txtHours);
            Grid.SetColumn(_txtHours, 0);

            _lblHours = new TextBlock { Text = Loc.T("HoursLabel"), FontSize = 12, Foreground = TextSecondary, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };
            timeRow.Children.Add(_lblHours);
            Grid.SetColumn(_lblHours, 1);

            _txtMinutes = CreateStyledTextBox("45");
            _txtMinutes.TextChanged += (s, e) => Recalculate();
            timeRow.Children.Add(_txtMinutes);
            Grid.SetColumn(_txtMinutes, 3);

            _lblMinutes = new TextBlock { Text = Loc.T("MinutesLabel"), FontSize = 12, Foreground = TextSecondary, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };
            timeRow.Children.Add(_lblMinutes);
            Grid.SetColumn(_lblMinutes, 4);

            timeStack.Children.Add(timeRow);

            _timeChipsList.Clear();
            var timeChips = new WrapPanel { Margin = new Thickness(0, 6, 0, 0) };
            string[] timeValues = new string[] { "1", "2", "4", "6", "10", "24" };
            foreach (var t in timeValues)
            {
                var val = t;
                var btn = CreateChip(val + " " + Loc.T("HoursLabel"), () => {
                    _txtHours.Text = val;
                    _txtMinutes.Text = "0";
                });
                _timeChipsList.Add(Tuple.Create(btn, val));
                timeChips.Children.Add(btn);
            }
            timeStack.Children.Add(timeChips);
            timeCard.Child = timeStack;
            timePanel.Children.Add(timeCard);
            inputsGrid.Children.Add(timePanel);
            Grid.SetColumn(timePanel, 2);
            Grid.SetRow(timePanel, 0);

            // 4. PRINTER HOURLY RATE (Col 0, Row 1)
            var ratePanel = new StackPanel();
            _lblInputHourlyRate = CreateSectionHeader("4. " + Loc.T("HourlyRateLabel"));
            ratePanel.Children.Add(_lblInputHourlyRate);

            var rateCard = CreateCard();
            var rateStack = new StackPanel();

            _txtHourlyRate = CreateStyledTextBox(_settings.HourlyRate.ToString("F2", CultureInfo.InvariantCulture));
            _txtHourlyRate.TextChanged += (s, e) => {
                double r;
                if (double.TryParse(_txtHourlyRate.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out r))
                {
                    _settings.HourlyRate = r;
                    DataService.SaveSettings(_settings);
                }
                Recalculate();
            };
            rateStack.Children.Add(_txtHourlyRate);

            _lblInputHourlyHint = new TextBlock
            {
                Text = Loc.T("HourlyRateHint"),
                FontSize = 10,
                Foreground = TextMuted,
                Margin = new Thickness(2, 3, 0, 4)
            };
            rateStack.Children.Add(_lblInputHourlyHint);

            var rateChips = new WrapPanel();
            string[] rates = new string[] { "1.50", "2.00", "2.50", "3.00", "4.00", "5.00" };
            foreach (var r in rates)
            {
                var val = r;
                rateChips.Children.Add(CreateChip(val, () => {
                    _txtHourlyRate.Text = val;
                }));
            }
            rateStack.Children.Add(rateChips);
            rateCard.Child = rateStack;
            ratePanel.Children.Add(rateCard);
            inputsGrid.Children.Add(ratePanel);
            Grid.SetColumn(ratePanel, 0);
            Grid.SetRow(ratePanel, 1);

            // 5. MARKUP PERCENTAGE (Col 2, Row 1)
            var markupPanel = new StackPanel();
            _lblInputMarkup = CreateSectionHeader("5. " + Loc.T("MarkupLabel"));
            markupPanel.Children.Add(_lblInputMarkup);

            var markupCard = CreateCard();
            var markupStack = new StackPanel();

            _txtMarkup = CreateStyledTextBox(_settings.MarkupPercent.ToString("F0", CultureInfo.InvariantCulture));
            _txtMarkup.TextChanged += (s, e) => {
                double m;
                if (double.TryParse(_txtMarkup.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out m))
                {
                    _settings.MarkupPercent = m;
                    DataService.SaveSettings(_settings);
                }
                Recalculate();
            };
            markupStack.Children.Add(_txtMarkup);

            var markupChips = new WrapPanel { Margin = new Thickness(0, 6, 0, 0) };
            string[] markups = new string[] { "0%", "25%", "40%", "50%", "75%", "100%", "150%" };
            foreach (var m in markups)
            {
                var val = m.Replace("%", "");
                markupChips.Children.Add(CreateChip(m, () => {
                    _txtMarkup.Text = val;
                }));
            }
            markupStack.Children.Add(markupChips);
            markupCard.Child = markupStack;
            markupPanel.Children.Add(markupCard);
            inputsGrid.Children.Add(markupPanel);
            Grid.SetColumn(markupPanel, 2);
            Grid.SetRow(markupPanel, 1);

            inputStack.Children.Add(inputsGrid);

            scroll.Content = inputStack;
            grid.Children.Add(scroll);
            Grid.SetColumn(scroll, 0);

            // RIGHT COLUMN: LIVE SUMMARY & QUOTE CARD
            var summaryCard = BuildSummaryCard();
            grid.Children.Add(summaryCard);
            Grid.SetColumn(summaryCard, 2);

            return grid;
        }

        private Border BuildSummaryCard()
        {
            var card = new Border
            {
                Background = BgCard,
                BorderBrush = new SolidColorBrush(Color.FromArgb(160, 16, 185, 129)), // Emerald glow border
                BorderThickness = new Thickness(1.5),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(18, 14, 18, 14),
                VerticalAlignment = VerticalAlignment.Stretch
            };
            card.Effect = new DropShadowEffect
            {
                Color = Color.FromRgb(16, 185, 129),
                Opacity = 0.15,
                BlurRadius = 24,
                ShadowDepth = 0
            };

            var stack = new StackPanel();

            // Header
            _lblSummaryHeader = new TextBlock
            {
                Text = Loc.T("SummaryHeader"),
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = TextPrimary,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stack.Children.Add(_lblSummaryHeader);

            // Subitems Grid
            var listStack = new StackPanel();

            _lblFilamentCostHeader = new TextBlock { Text = Loc.T("FilamentCost") };
            _resFilamentCost = new TextBlock { Text = "0.00", FontWeight = FontWeights.SemiBold };
            listStack.Children.Add(CreateSummaryRow(_lblFilamentCostHeader, _resFilamentCost, TextSecondary, TextPrimary));

            _lblTimeCostHeader = new TextBlock { Text = Loc.T("TimeCost") };
            _resPrintTimeCost = new TextBlock { Text = "0.00", FontWeight = FontWeights.SemiBold };
            listStack.Children.Add(CreateSummaryRow(_lblTimeCostHeader, _resPrintTimeCost, TextSecondary, TextPrimary));

            // Divider 1
            listStack.Children.Add(CreateCardDivider());

            _lblBaseCostHeader = new TextBlock { Text = Loc.T("BaseCost") };
            _resBaseCost = new TextBlock { Text = "0.00", FontWeight = FontWeights.SemiBold };
            listStack.Children.Add(CreateSummaryRow(_lblBaseCostHeader, _resBaseCost, TextSecondary, AccentCyan));

            _lblMarkupHeader = new TextBlock { Text = Loc.T("MarkupAmount") };
            _resMarkupAmount = new TextBlock { Text = "0.00", FontWeight = FontWeights.SemiBold };
            listStack.Children.Add(CreateSummaryRow(_lblMarkupHeader, _resMarkupAmount, TextSecondary, AccentAmber));

            // Divider 2
            listStack.Children.Add(CreateCardDivider());

            // Total Excl. VAT
            _lblTotalExclVatHeader = new TextBlock { Text = Loc.T("TotalExclVat"), FontWeight = FontWeights.Bold, FontSize = 13 };
            _resTotalExclVat = new TextBlock { Text = "0.00", FontWeight = FontWeights.Bold, FontSize = 17, Foreground = AccentCyan };
            listStack.Children.Add(CreateSummaryRow(_lblTotalExclVatHeader, _resTotalExclVat, TextPrimary, AccentCyan));

            // VAT 20%
            _lblVatHeader = new TextBlock { Text = Loc.T("VatLabel") };
            _resVatAmount = new TextBlock { Text = "0.00", FontWeight = FontWeights.SemiBold };
            listStack.Children.Add(CreateSummaryRow(_lblVatHeader, _resVatAmount, TextSecondary, TextSecondary));

            stack.Children.Add(listStack);

            // STANDOUT BOX: TOTAL WITH VAT 20%
            var totalWithVatBox = new Border
            {
                Background = new LinearGradientBrush(Color.FromArgb(40, 16, 185, 129), Color.FromArgb(20, 6, 182, 212), 45),
                BorderBrush = AccentGreen,
                BorderThickness = new Thickness(1.5),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(0, 10, 0, 8)
            };
            var totalStack = new StackPanel();
            _lblTotalInclVatHeader = new TextBlock
            {
                Text = Loc.T("TotalInclVat"),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = AccentGreen,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            _resTotalInclVat = new TextBlock
            {
                Text = "0.00 €",
                FontSize = 26,
                FontWeight = FontWeights.ExtraBold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 2, 0, 0)
            };
            totalStack.Children.Add(_lblTotalInclVatHeader);
            totalStack.Children.Add(_resTotalInclVat);
            totalWithVatBox.Child = totalStack;
            stack.Children.Add(totalWithVatBox);

            // Formula Note
            _lblFormulaNote = new TextBlock
            {
                Text = Loc.T("FormulaNote"),
                FontSize = 10,
                Foreground = TextMuted,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };
            stack.Children.Add(_lblFormulaNote);

            // Action Buttons
            _btnCopyQuote = CreatePrimaryButton("📋 " + Loc.T("BtnCopy"), CopyQuoteToClipboard);
            _btnCopyQuote.Height = 38;
            _btnCopyQuote.Margin = new Thickness(0, 0, 0, 6);
            stack.Children.Add(_btnCopyQuote);

            _btnResetInputs = CreateSecondaryButton("↺ " + Loc.T("BtnReset"), ResetInputs);
            _btnResetInputs.Height = 32;
            stack.Children.Add(_btnResetInputs);

            card.Child = stack;
            return card;
        }

        private Grid BuildFilamentsView()
        {
            var grid = new Grid { Margin = new Thickness(20, 14, 20, 14) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });

            // LEFT: Filaments List
            var leftStack = new StackPanel();

            var topRow = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            topRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var listTitle = new TextBlock
            {
                Text = Loc.T("FilamentsTab"),
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = TextPrimary,
                VerticalAlignment = VerticalAlignment.Center
            };
            topRow.Children.Add(listTitle);
            Grid.SetColumn(listTitle, 0);

            _btnAddFilamentTop = CreatePrimaryButton(Loc.T("BtnAddFilament"), () => StartEditFilament(null));
            _btnAddFilamentTop.Padding = new Thickness(14, 8, 14, 8);
            topRow.Children.Add(_btnAddFilamentTop);
            Grid.SetColumn(_btnAddFilamentTop, 1);

            leftStack.Children.Add(topRow);

            var scroll = new ScrollViewer { MaxHeight = 490, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            _filamentsListPanel = new StackPanel();
            scroll.Content = _filamentsListPanel;
            leftStack.Children.Add(scroll);

            grid.Children.Add(leftStack);
            Grid.SetColumn(leftStack, 0);

            // RIGHT: Edit / Add Form Card
            _filamentEditorCard = BuildFilamentEditorCard();
            grid.Children.Add(_filamentEditorCard);
            Grid.SetColumn(_filamentEditorCard, 2);

            RefreshFilamentsGrid();
            return grid;
        }

        private Border BuildFilamentEditorCard()
        {
            var card = CreateCard();
            card.VerticalAlignment = VerticalAlignment.Top;
            card.Padding = new Thickness(20);

            var stack = new StackPanel();

            _editorTitle = new TextBlock
            {
                Text = Loc.T("EditorTitleNew"),
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = TextPrimary,
                Margin = new Thickness(0, 0, 0, 16)
            };
            stack.Children.Add(_editorTitle);

            // Name
            stack.Children.Add(CreateFormLabel(Loc.T("FieldName")));
            _editName = CreateStyledTextBox("");
            stack.Children.Add(_editName);

            // Type
            stack.Children.Add(CreateFormLabel(Loc.T("FieldType")));
            _editType = CreateStyledTextBox("");
            stack.Children.Add(_editType);

            // Density
            stack.Children.Add(CreateFormLabel(Loc.T("FieldDensity")));
            _editDensity = CreateStyledTextBox("1.24");
            stack.Children.Add(_editDensity);

            // Price per kg
            stack.Children.Add(CreateFormLabel(Loc.T("FieldPrice")));
            _editPrice = CreateStyledTextBox("20.00");
            stack.Children.Add(_editPrice);

            // Buttons
            var btnRow = new Grid { Margin = new Thickness(0, 20, 0, 0) };
            btnRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            btnRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            btnRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            _btnSaveFilament = CreatePrimaryButton("✓ " + Loc.T("BtnSave"), SaveCurrentFilament);
            btnRow.Children.Add(_btnSaveFilament);
            Grid.SetColumn(_btnSaveFilament, 0);

            _btnCancelFilament = CreateSecondaryButton(Loc.T("BtnCancel"), () => StartEditFilament(null));
            btnRow.Children.Add(_btnCancelFilament);
            Grid.SetColumn(_btnCancelFilament, 2);

            stack.Children.Add(btnRow);

            card.Child = stack;
            return card;
        }

        private void RefreshFilamentsGrid()
        {
            if (_filamentsListPanel == null) return;
            _filamentsListPanel.Children.Clear();

            foreach (var item in _filaments)
            {
                var fil = item;
                var rowBorder = new Border
                {
                    Background = BgCard,
                    BorderBrush = BorderColor,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(14, 12, 14, 12),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var rowGrid = new Grid();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var infoStack = new StackPanel();
                var nameText = new TextBlock
                {
                    Text = fil.Name,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    Foreground = TextPrimary
                };
                var detailsText = new TextBlock
                {
                    Text = string.Format("{0}  •  {1}: {2:F2} g/cm³  •  {3}: {4:F2} {5}/kg",
                        fil.Type, Loc.T("Density"), fil.Density, Loc.T("CostPerKg"), fil.CostPerKg, _settings.Currency),
                    FontSize = 12,
                    Foreground = TextSecondary,
                    Margin = new Thickness(0, 3, 0, 0)
                };
                infoStack.Children.Add(nameText);
                infoStack.Children.Add(detailsText);
                rowGrid.Children.Add(infoStack);
                Grid.SetColumn(infoStack, 0);

                var actionsStack = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                var btnEdit = CreateMiniButton("✏", () => StartEditFilament(fil), BgCardInner, AccentCyan);
                var btnDel = CreateMiniButton("✕", () => DeleteFilament(fil), BgCardInner, AccentRed);

                actionsStack.Children.Add(btnEdit);
                actionsStack.Children.Add(btnDel);

                rowGrid.Children.Add(actionsStack);
                Grid.SetColumn(actionsStack, 1);

                rowBorder.Child = rowGrid;
                _filamentsListPanel.Children.Add(rowBorder);
            }
        }

        private void StartEditFilament(Filament fil)
        {
            _currentEditingFilament = fil;
            if (fil == null)
            {
                _editorTitle.Text = Loc.T("EditorTitleNew");
                _editName.Text = "";
                _editType.Text = "PLA";
                _editDensity.Text = "1.24";
                _editPrice.Text = "20.00";
            }
            else
            {
                _editorTitle.Text = Loc.T("EditorTitleEdit");
                _editName.Text = fil.Name;
                _editType.Text = fil.Type;
                _editDensity.Text = fil.Density.ToString("F2", CultureInfo.InvariantCulture);
                _editPrice.Text = fil.CostPerKg.ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        private void SaveCurrentFilament()
        {
            string name = _editName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter filament name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double density, price;
            if (!double.TryParse(_editDensity.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out density))
                density = 1.24;

            if (!double.TryParse(_editPrice.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                price = 20.0;

            if (_currentEditingFilament == null)
            {
                var newFil = new Filament
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Type = _editType.Text.Trim(),
                    Density = density,
                    CostPerKg = price
                };
                _filaments.Add(newFil);
            }
            else
            {
                _currentEditingFilament.Name = name;
                _currentEditingFilament.Type = _editType.Text.Trim();
                _currentEditingFilament.Density = density;
                _currentEditingFilament.CostPerKg = price;
            }

            DataService.SaveFilaments(_filaments);
            RefreshFilamentsGrid();
            ReloadFilamentsComboBox();
            StartEditFilament(null);
        }

        private void DeleteFilament(Filament fil)
        {
            var res = MessageBox.Show(Loc.T("ConfirmDelete") + "\n\n" + fil.Name, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                _filaments.Remove(fil);
                DataService.SaveFilaments(_filaments);
                RefreshFilamentsGrid();
                ReloadFilamentsComboBox();
                if (_selectedFilament != null && _selectedFilament.Id == fil.Id)
                {
                    SelectInitialFilament();
                    Recalculate();
                }
            }
        }

        private void ReloadFilamentsComboBox()
        {
            if (_comboFilaments == null) return;
            _comboFilaments.ItemsSource = null;

            // Create explicit white text DataTemplate for ComboBox items and selection box
            var template = new DataTemplate(typeof(Filament));
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("DisplayName"));
            factory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            factory.SetValue(TextBlock.FontSizeProperty, 14.0);
            factory.SetValue(TextBlock.FontWeightProperty, FontWeights.Medium);
            factory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            template.VisualTree = factory;
            _comboFilaments.ItemTemplate = template;

            _comboFilaments.ItemsSource = _filaments;

            if (_selectedFilament != null)
            {
                var match = _filaments.Find(f => f.Id == _selectedFilament.Id);
                if (match != null)
                    _comboFilaments.SelectedItem = match;
                else if (_filaments.Count > 0)
                    _comboFilaments.SelectedIndex = 0;
            }
            else if (_filaments.Count > 0)
            {
                _comboFilaments.SelectedIndex = 0;
            }
        }

        private void SelectInitialFilament()
        {
            ReloadFilamentsComboBox();
            if (!string.IsNullOrEmpty(_settings.SelectedFilamentId))
            {
                var match = _filaments.Find(f => f.Id == _settings.SelectedFilamentId);
                if (match != null)
                {
                    _comboFilaments.SelectedItem = match;
                    _selectedFilament = match;
                    UpdateFilamentBadges();
                    return;
                }
            }

            if (_filaments.Count > 0)
            {
                _comboFilaments.SelectedIndex = 0;
                _selectedFilament = _filaments[0];
                UpdateFilamentBadges();
            }
        }

        private void UpdateFilamentBadges()
        {
            if (_selectedFilament != null && _lblFilamentDensity != null && _lblFilamentPrice != null)
            {
                _lblFilamentDensity.Text = string.Format("{0:F2} g/cm³", _selectedFilament.Density);
                _lblFilamentPrice.Text = string.Format("{0:F2} {1}/kg", _selectedFilament.CostPerKg, _settings.Currency);
            }
        }

        // CALCULATION LOGIC
        private CalculationResult CurrentResult = new CalculationResult();

        private void Recalculate()
        {
            if (_txtWeight == null || _txtHours == null || _txtMinutes == null || _txtHourlyRate == null || _txtMarkup == null)
                return;

            double weightGrams = 0;
            double.TryParse(_txtWeight.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out weightGrams);

            int hours = 0;
            int.TryParse(_txtHours.Text, out hours);

            int minutes = 0;
            int.TryParse(_txtMinutes.Text, out minutes);

            double hourlyRate = 0;
            double.TryParse(_txtHourlyRate.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out hourlyRate);

            double markupPercent = 0;
            double.TryParse(_txtMarkup.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out markupPercent);

            double filamentCostPerKg = (_selectedFilament != null) ? _selectedFilament.CostPerKg : 20.0;

            // Formulations specified by User:
            // 1. Cost of required filament: (grams / 1000) * cost_per_kg
            double filamentCost = (weightGrams / 1000.0) * filamentCostPerKg;

            // 2. Cost by print time: total hours * hourly rate
            double totalHours = hours + (minutes / 60.0);
            double printTimeCost = totalHours * hourlyRate;

            // 3. Base production cost: filament + time
            double baseCost = filamentCost + printTimeCost;

            // 4. Markup: total price = (filament cost + print time cost) * (1 + markup% / 100)
            double profitAmount = baseCost * (markupPercent / 100.0);
            double totalExclVat = baseCost + profitAmount;

            // 5. VAT 20%
            double vatRate = 20.0;
            double vatAmount = totalExclVat * (vatRate / 100.0);
            double totalInclVat = totalExclVat + vatAmount;

            // Store in Result
            CurrentResult.WeightGrams = weightGrams;
            CurrentResult.FilamentCostPerKg = filamentCostPerKg;
            CurrentResult.FilamentCost = filamentCost;
            CurrentResult.PrintHours = hours;
            CurrentResult.PrintMinutes = minutes;
            CurrentResult.TotalHours = totalHours;
            CurrentResult.HourlyRate = hourlyRate;
            CurrentResult.PrintTimeCost = printTimeCost;
            CurrentResult.BaseCost = baseCost;
            CurrentResult.MarkupPercent = markupPercent;
            CurrentResult.ProfitAmount = profitAmount;
            CurrentResult.TotalExclVat = totalExclVat;
            CurrentResult.VatPercent = vatRate;
            CurrentResult.VatAmount = vatAmount;
            CurrentResult.TotalInclVat = totalInclVat;
            CurrentResult.Currency = _settings.Currency;

            // Update UI elements
            string cur = _settings.Currency;
            _resFilamentCost.Text = string.Format("{0:N2} {1}", filamentCost, cur);
            _resPrintTimeCost.Text = string.Format("{0:N2} {1}", printTimeCost, cur);
            _resBaseCost.Text = string.Format("{0:N2} {1}", baseCost, cur);
            _resMarkupAmount.Text = string.Format("+{0:N2} {1} ({2:F0}%)", profitAmount, cur, markupPercent);
            _resTotalExclVat.Text = string.Format("{0:N2} {1}", totalExclVat, cur);
            _resVatAmount.Text = string.Format("{0:N2} {1}", vatAmount, cur);
            _resTotalInclVat.Text = string.Format("{0:N2} {1}", totalInclVat, cur);
        }

        private void CopyQuoteToClipboard()
        {
            string cur = _settings.Currency;
            string filName = (_selectedFilament != null) ? _selectedFilament.Name : "Standard 3D Filament";
            string filType = (_selectedFilament != null) ? _selectedFilament.Type : "PLA";

            string quote = string.Format(
                "=========================================\n" +
                "  {0}\n" +
                "=========================================\n" +
                "{1}: {2} ({3})\n" +
                "{4}: {5:F1} {6}\n" +
                "{7}: {8} {9} {10} {11} ({12:F2} {13})\n" +
                "-----------------------------------------\n" +
                "{14}: {15:N2} {16}\n" +
                "{17}: {18:N2} {19}\n" +
                "{20}: {21:N2} {22}\n" +
                "{23} ({24:F0}%): +{25:N2} {26}\n" +
                "-----------------------------------------\n" +
                "{27}: {28:N2} {29}\n" +
                "{30}: {31:N2} {32}\n" +
                "=========================================\n" +
                "  {33}: {34:N2} {35}\n" +
                "=========================================\n",
                Loc.T("QuoteTitle"),
                Loc.T("QuoteMaterial"), filName, filType,
                Loc.T("QuoteWeight"), CurrentResult.WeightGrams, Loc.T("GramsUnit"),
                Loc.T("QuoteTime"), CurrentResult.PrintHours, Loc.T("HoursLabel"), CurrentResult.PrintMinutes, Loc.T("MinutesLabel"), CurrentResult.TotalHours, Loc.T("HoursLabel"),
                Loc.T("QuoteFilamentCost"), CurrentResult.FilamentCost, cur,
                Loc.T("QuoteTimeCost"), CurrentResult.PrintTimeCost, cur,
                Loc.T("QuoteBase"), CurrentResult.BaseCost, cur,
                Loc.T("QuoteMarkup"), CurrentResult.MarkupPercent, CurrentResult.ProfitAmount, cur,
                Loc.T("QuoteSubtotal"), CurrentResult.TotalExclVat, cur,
                Loc.T("QuoteVat"), CurrentResult.VatAmount, cur,
                Loc.T("QuoteTotalWithVat"), CurrentResult.TotalInclVat, cur
            );

            try
            {
                Clipboard.SetText(quote);
                MessageBox.Show(Loc.T("CopiedSuccess"), "3D Print Calculator", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy to clipboard: " + ex.Message, "Notice", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetInputs()
        {
            _txtWeight.Text = "50";
            _txtHours.Text = "2";
            _txtMinutes.Text = "30";
            _txtHourlyRate.Text = "2.50";
            _txtMarkup.Text = "40";
            Recalculate();
        }

        private void SetLanguage(string lang)
        {
            Loc.SetLanguage(lang);
            _settings.Language = lang;
            DataService.SaveSettings(_settings);
            ApplyLanguage();
            UpdateLanguageButtons();
            RefreshFilamentsGrid();
            ReloadFilamentsComboBox();
            Recalculate();
        }

        private void ApplyLanguage()
        {
            this.Title = Loc.T("AppTitle");
            if (_headerTitle != null) _headerTitle.Text = Loc.T("AppTitle");
            if (_headerSubtitle != null) _headerSubtitle.Text = Loc.T("Subtitle");
            if (_btnTabCalc != null) _btnTabCalc.Content = "⚡ " + Loc.T("CalcTab");
            if (_btnTabFilaments != null) _btnTabFilaments.Content = "🧵 " + Loc.T("FilamentsTab");

            if (_lblInputFilament != null) _lblInputFilament.Text = "1. " + Loc.T("FilamentLabel");
            if (_lblInputWeight != null) _lblInputWeight.Text = "2. " + Loc.T("WeightLabel");
            if (_lblInputTime != null) _lblInputTime.Text = "3. " + Loc.T("PrintTimeLabel");
            if (_lblInputHourlyRate != null) _lblInputHourlyRate.Text = "4. " + Loc.T("HourlyRateLabel");
            if (_lblInputHourlyHint != null) _lblInputHourlyHint.Text = Loc.T("HourlyRateHint");
            if (_lblInputMarkup != null) _lblInputMarkup.Text = "5. " + Loc.T("MarkupLabel");

            if (_lblHours != null) _lblHours.Text = Loc.T("HoursLabel");
            if (_lblMinutes != null) _lblMinutes.Text = Loc.T("MinutesLabel");
            if (_lblUnitGrams != null) _lblUnitGrams.Text = Loc.T("GramsUnit");
            if (_btnGoToFilaments != null) _btnGoToFilaments.Content = Loc.T("ManageFilaments") + " →";

            if (_timeChipsList != null)
            {
                foreach (var item in _timeChipsList)
                {
                    item.Item1.Content = string.Format("{0} {1}", item.Item2, Loc.T("HoursLabel"));
                }
            }

            if (_weightChipsList != null)
            {
                foreach (var item in _weightChipsList)
                {
                    item.Item1.Content = string.Format("{0} {1}", item.Item2, Loc.T("GramsUnit"));
                }
            }

            if (_lblSummaryHeader != null) _lblSummaryHeader.Text = Loc.T("SummaryHeader");
            if (_lblFilamentCostHeader != null) _lblFilamentCostHeader.Text = Loc.T("FilamentCost");
            if (_lblTimeCostHeader != null) _lblTimeCostHeader.Text = Loc.T("TimeCost");
            if (_lblBaseCostHeader != null) _lblBaseCostHeader.Text = Loc.T("BaseCost");
            if (_lblMarkupHeader != null) _lblMarkupHeader.Text = Loc.T("MarkupAmount");
            if (_lblTotalExclVatHeader != null) _lblTotalExclVatHeader.Text = Loc.T("TotalExclVat");
            if (_lblVatHeader != null) _lblVatHeader.Text = Loc.T("VatLabel");
            if (_lblTotalInclVatHeader != null) _lblTotalInclVatHeader.Text = Loc.T("TotalInclVat");
            if (_lblFormulaNote != null) _lblFormulaNote.Text = Loc.T("FormulaNote");

            if (_btnCopyQuote != null) _btnCopyQuote.Content = "📋 " + Loc.T("BtnCopy");
            if (_btnResetInputs != null) _btnResetInputs.Content = "↺ " + Loc.T("BtnReset");
            if (_btnAddFilamentTop != null) _btnAddFilamentTop.Content = Loc.T("BtnAddFilament");
            if (_btnSaveFilament != null) _btnSaveFilament.Content = "✓ " + Loc.T("BtnSave");
            if (_btnCancelFilament != null) _btnCancelFilament.Content = Loc.T("BtnCancel");
            if (_editorTitle != null && _currentEditingFilament == null) _editorTitle.Text = Loc.T("EditorTitleNew");
        }

        private void UpdateCurrencyButtons()
        {
            foreach (var b in _currencyButtons)
            {
                bool active = (b.Content.ToString() == _settings.Currency);
                b.Background = active ? AccentGreen : BgCardInner;
                b.Foreground = active ? Brushes.White : TextSecondary;
                b.BorderBrush = active ? AccentGreen : BorderColor;
            }
            UpdateFilamentBadges();
        }

        private void UpdateLanguageButtons()
        {
            foreach (var b in _langButtons)
            {
                bool active = (b.Content.ToString().ToLower() == Loc.CurrentLanguage);
                b.Background = active ? AccentCyan : BgCardInner;
                b.Foreground = active ? Brushes.Black : TextSecondary;
                b.FontWeight = active ? FontWeights.Bold : FontWeights.Normal;
                b.BorderBrush = active ? AccentCyan : BorderColor;
            }
        }

        // UI HELPER CREATION METHODS
        private Border CreateCard()
        {
            return new Border
            {
                Background = BgCard,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 8)
            };
        }

        private TextBlock CreateSectionHeader(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = TextPrimary,
                Margin = new Thickness(0, 2, 0, 5)
            };
        }

        private TextBlock CreateFormLabel(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = TextSecondary,
                Margin = new Thickness(0, 10, 0, 4)
            };
        }

        private TextBox CreateStyledTextBox(string initial)
        {
            return new TextBox
            {
                Text = initial,
                Background = BgInput,
                Foreground = TextPrimary,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(1),
                FontSize = 15,
                FontWeight = FontWeights.Medium,
                Padding = new Thickness(10, 8, 10, 8),
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }

        private Button CreatePillButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Background = BgCardInner,
                Foreground = TextSecondary,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10, 6, 10, 6),
                Margin = new Thickness(3, 0, 3, 0),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private Button CreateChip(string text, Action onClick)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 11,
                Background = BgCardInner,
                Foreground = TextSecondary,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 6, 0),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private TextBlock CreateBadge(string text, SolidColorBrush fg)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = fg,
                Background = BgCardInner,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 8, 0)
            };
        }

        private Button CreateLinkButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 12,
                Foreground = AccentCyan,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private Button CreateMiniButton(string text, Action onClick, SolidColorBrush bg, SolidColorBrush fg)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 13,
                Width = 32,
                Height = 32,
                Background = bg,
                Foreground = fg,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(4, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private Button CreatePrimaryButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = AccentGreen,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(16, 10, 16, 10),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private Button CreateSecondaryButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Background = BgCardInner,
                Foreground = TextSecondary,
                BorderBrush = BorderColor,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(14, 8, 14, 8),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private Button CreateTabButton(string text, bool isActive, Action onClick)
        {
            var btn = new Button
            {
                Content = text,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Background = Brushes.Transparent,
                Foreground = isActive ? AccentCyan : TextSecondary,
                BorderThickness = new Thickness(0, 0, 0, 2),
                BorderBrush = isActive ? AccentCyan : Brushes.Transparent,
                Padding = new Thickness(16, 10, 16, 10),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void SetTabActive(Button btn, bool active)
        {
            btn.Foreground = active ? AccentCyan : TextSecondary;
            btn.BorderBrush = active ? AccentCyan : Brushes.Transparent;
        }

        private Grid CreateSummaryRow(TextBlock titleBlock, TextBlock valBlock, SolidColorBrush titleFg, SolidColorBrush valFg)
        {
            var g = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            titleBlock.Foreground = titleFg;
            titleBlock.FontSize = 13;
            g.Children.Add(titleBlock);
            Grid.SetColumn(titleBlock, 0);

            valBlock.Foreground = valFg;
            g.Children.Add(valBlock);
            Grid.SetColumn(valBlock, 1);

            return g;
        }

        private Border CreateCardDivider()
        {
            return new Border
            {
                Height = 1,
                Background = BorderColor,
                Margin = new Thickness(0, 5, 0, 5)
            };
        }
    }
}
