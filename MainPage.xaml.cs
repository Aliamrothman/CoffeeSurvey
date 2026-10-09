using System.Text;

namespace CoffeeSurvey;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // IsEnabled: комментарий доступен только после ввода имени
    private void OnNameTextChanged(object? sender, TextChangedEventArgs e)
    {
        bool hasName = !string.IsNullOrWhiteSpace(_entryName_.Text);
        _editorComment_.IsEnabled = hasName;
        _editorComment_.Placeholder = hasName
            ? "Расскажите о своём любимом кофе"
            : "Доступно после ввода имени";
    }

    // Switch: при выключении очищаем e-mail (видимость блока задаёт привязка в XAML)
    private void OnNewsToggled(object? sender, ToggledEventArgs e)
    {
        if (!e.Value)
        {
            _entryEmail_.Text = string.Empty;
        }
    }

    // Slider: оценка только целыми числами
    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        double rounded = Math.Round(e.NewValue);
        if (Math.Abs(rounded - e.NewValue) > 0.001)
        {
            _sliderRating_.Value = rounded;
        }
    }

    private string? GetSelectedTime()
    {
        if (_radioMorning_.IsChecked) return _radioMorning_.Value?.ToString();
        if (_radioDay_.IsChecked) return _radioDay_.Value?.ToString();
        if (_radioEvening_.IsChecked) return _radioEvening_.Value?.ToString();
        return null;
    }

    private void OnShowClicked(object? sender, EventArgs e)
    {
        // Проверка обязательных полей: имя, напиток, время
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_entryName_.Text))
            errors.Add("• введите имя");

        if (_pickerDrink_.SelectedIndex < 0)
            errors.Add("• выберите напиток");

        string? time = GetSelectedTime();
        if (time is null)
            errors.Add("• выберите, когда вы пьёте кофе");

        if (_switchNews_.IsToggled)
        {
            string email = _entryEmail_.Text?.Trim() ?? string.Empty;
            if (email.Length == 0 || !email.Contains('@'))
                errors.Add("• введите корректный e-mail (или выключите подписку)");
        }

        _borderResult_.IsVisible = true;

        if (errors.Count > 0)
        {
            _borderResult_.Stroke = Colors.Red;
            _labelResult_.Text = "Заполните обязательные поля:\n" + string.Join("\n", errors);
            return;
        }

        _borderResult_.Stroke = Color.FromArgb("#6F4E37");

        var additives = new List<string>();
        if (_checkMilk_.IsChecked) additives.Add("молоко");
        if (_checkSugar_.IsChecked) additives.Add("сахар");

        var sb = new StringBuilder();
        sb.AppendLine("Сводка ответов");
        sb.AppendLine($"Имя: {_entryName_.Text!.Trim()}");
        sb.AppendLine($"Напиток: {_pickerDrink_.SelectedItem}");
        sb.AppendLine($"Время: {time}");
        sb.AppendLine($"Добавки: {(additives.Count > 0 ? string.Join(", ", additives) : "без добавок")}");
        sb.AppendLine($"Дата рождения: {_datePickerBirth_.Date:dd.MM.yyyy}");
        sb.AppendLine($"Вкус кофе дома: {_sliderRating_.Value:F0} из 10");
        sb.AppendLine($"Чашек в день: {_stepperCups_.Value:F0}");
        sb.AppendLine(_switchNews_.IsToggled
            ? $"Рассылка: да ({_entryEmail_.Text!.Trim()})"
            : "Рассылка: нет");
        sb.Append("Комментарий: ");
        sb.Append(string.IsNullOrWhiteSpace(_editorComment_.Text) ? "—" : _editorComment_.Text.Trim());

        _labelResult_.Text = sb.ToString();
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        _entryName_.Text = string.Empty;
        _entryEmail_.Text = string.Empty;
        _editorComment_.Text = string.Empty;

        _switchNews_.IsToggled = false;
        _pickerDrink_.SelectedIndex = -1;

        _radioMorning_.IsChecked = false;
        _radioDay_.IsChecked = false;
        _radioEvening_.IsChecked = false;

        _checkMilk_.IsChecked = false;
        _checkSugar_.IsChecked = false;

        _datePickerBirth_.Date = new DateTime(2000, 1, 1);
        _sliderRating_.Value = 5;
        _stepperCups_.Value = 2;

        _labelResult_.Text = string.Empty;
        _borderResult_.IsVisible = false;
        _borderResult_.Stroke = Color.FromArgb("#6F4E37");
    }
}
