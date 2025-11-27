using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Globalization;
using lista_zakupow.Models;

namespace lista_zakupow.ViewModels;

public class ShoppingListViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ShoppingItem> Items { get; } = new();

    private string _newItemName = string.Empty;
    public string NewItemName
    {
        get => _newItemName;
        set { _newItemName = value; OnPropertyChanged(); AddCommandCanExecuteChanged(); }
    }

    private string _newItemPrice = string.Empty; // keep as string for validation
    public string NewItemPrice
    {
        get => _newItemPrice;
        set { _newItemPrice = value; OnPropertyChanged(); AddCommandCanExecuteChanged(); }
    }

    public decimal Total => Items.Sum(i => i.Price);

    public ICommand AddItemCommand { get; }
    public ICommand DeleteItemCommand { get; }

    public ShoppingListViewModel()
    {
        AddItemCommand = new Command(AddItem, CanAddItem);
        DeleteItemCommand = new Command<ShoppingItem>(DeleteItem);
    }

    private bool CanAddItem()
    {
        return !string.IsNullOrWhiteSpace(NewItemName) && TryParsePrice(NewItemPrice, out var price) && price >= 0m;
    }

    private bool TryParsePrice(string input, out decimal price)
    {
        price = 0m;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var s = input.Trim();

        // Try current culture
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out price)) return true;
        // Try invariant
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out price)) return true;
        // Swap separators and try again
        var alt = s.Contains(',') ? s.Replace(',', '.') : s.Replace('.', ',');
        if (decimal.TryParse(alt, NumberStyles.Number, CultureInfo.CurrentCulture, out price)) return true;
        if (decimal.TryParse(alt, NumberStyles.Number, CultureInfo.InvariantCulture, out price)) return true;
        return false;
    }

    private void AddItem()
    {
        if (!TryParsePrice(NewItemPrice, out var price)) return;
        Items.Add(new ShoppingItem { Name = NewItemName.Trim(), Price = price });
        OnPropertyChanged(nameof(Items)); // extra notification
        OnPropertyChanged(nameof(Total));
        NewItemName = string.Empty;
        NewItemPrice = string.Empty;
    }

    private void DeleteItem(ShoppingItem? item)
    {
        if (item == null) return;
        if (Items.Remove(item))
        {
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(Total));
        }
    }

    private void AddCommandCanExecuteChanged()
    {
        (AddItemCommand as Command)?.ChangeCanExecute();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
