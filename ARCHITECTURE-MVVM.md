# 🏗️ MVVM Architecture - Coffee Shop App

## Overview

The Coffee Shop application implements the **MVVM (Model-View-ViewModel)** architectural pattern, which is the recommended design pattern for .NET MAUI applications. This document explains how MVVM is implemented throughout the app and why this architecture improves code quality, maintainability, and testability.

## Table of Contents

- [What is MVVM?](#what-is-mvvm)
- [The Three Layers](#the-three-layers)
- [Project Structure](#project-structure)
- [CommunityToolkit.Mvvm](#communitytoolkitmvvm)
- [Data Binding](#data-binding)
- [Commands](#commands)
- [Complete Example Flow](#complete-example-flow)
- [Real Code Examples](#real-code-examples)
- [Services Layer](#services-layer)
- [Benefits of MVVM](#benefits-of-mvvm)
- [Best Practices](#best-practices)

---

## What is MVVM?

**MVVM (Model-View-ViewModel)** is a software architectural pattern that separates an application into three interconnected components:

```
┌─────────┐      ┌──────────────┐      ┌───────┐
│  VIEW   │◄────►│  VIEWMODEL   │◄────►│ MODEL │
│ (XAML)  │      │ (PageModel)  │      │ (Data)│
└─────────┘      └──────────────┘      └───────┘
     ↑                   ↑                   ↑
   User              Business             Data
Interface             Logic              Layer
```

### Why MVVM?

1. **Separation of Concerns** - UI, logic, and data are independent
2. **Testability** - ViewModels can be tested without UI
3. **Maintainability** - Changes to one layer don't affect others
4. **Data Binding** - Automatic UI updates when data changes
5. **Reusability** - ViewModels can work with different Views

---

## The Three Layers

### 1. MODEL - Data & Business Logic

**Location:** `Models/` folder

Models represent the data structures and business entities in your application.

```csharp
// Models/User.cs
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Indexed, Unique]
    public string Username { get; set; }
    
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Models in Coffee Shop:**
- `User` - User account data
- `Order` - Order information
- `OrderLine` - Individual order items
- `CoffeeShopMenuItem` - Menu item data

**Characteristics:**
- ✅ Pure data structures (POCOs - Plain Old CLR Objects)
- ✅ No UI logic
- ✅ No business logic (that belongs in ViewModels or Services)
- ✅ Can be persisted to database

---

### 2. VIEW - User Interface

**Location:** `Pages/` folder (XAML files)

Views define the user interface using XAML markup. They display data and capture user input.

```xaml
<!-- Pages/LoginPage.xaml -->
<ContentPage x:Class="CoffeeShop.Pages.LoginPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="Coffee Shop">

    <VerticalStackLayout Padding="40" Spacing="16">
        <!-- Data binding to ViewModel properties -->
        <Entry Placeholder="Username" 
               Text="{Binding Username}"
               ReturnType="Next" />

        <Entry Placeholder="Password" 
               Text="{Binding Password}"
               IsPassword="True"
               ReturnType="Next" />

        <!-- Command binding to ViewModel method -->
        <Button Text="{Binding IsRegistering, Converter={StaticResource BoolToButtonTextConverter}}"
                Command="{Binding SubmitCommand}" />
    </VerticalStackLayout>
</ContentPage>
```

**Views in Coffee Shop:**
- `LoginPage.xaml` - Authentication UI
- `CategoriesPage.xaml` - Menu categories navigation
- `MenuPage.xaml` - Menu items with add/remove functionality
- `CheckoutPage.xaml` - Order review and submission
- `ReceiptPage.xaml` - Order confirmation
- `HistoryPage.xaml` - Past orders display

**Characteristics:**
- ✅ XAML markup for UI structure
- ✅ Data bindings to ViewModel properties
- ✅ Command bindings to ViewModel methods
- ✅ Minimal or no code-behind (C#)
- ✅ Platform-specific rendering handled by .NET MAUI

---

### 3. VIEWMODEL (PageModel) - Presentation Logic

**Location:** `PageModels/` folder

ViewModels (called PageModels in this app) contain the presentation logic and expose data and commands to the View.

```csharp
// PageModels/ReceiptPageModel.cs
[QueryProperty(nameof(Order), "Order")]
public partial class ReceiptPageModel : BaseViewModel
{
    // Observable property - automatically notifies UI when changed
    [ObservableProperty]
    private Order order = new();

    // Property changed handler - updates Title when Order changes
    partial void OnOrderChanged(Order value)
    {
        Title = $"Order #{value.OrderNumber}";
    }

    // Command - UI can invoke this asynchronous method
    [RelayCommand]
    private async Task BackToCategoriesAsync()
    {
        await Shell.Current.GoToAsync("//categories");
    }
}
```

**PageModels in Coffee Shop:**
- `LoginPageModel` - Authentication logic
- `CategoriesPageModel` - Navigation and welcome message
- `MenuPageModel` - Menu display and basket operations
- `CheckoutPageModel` - Order review and submission
- `ReceiptPageModel` - Receipt display
- `HistoryPageModel` - Order history retrieval

**Characteristics:**
- ✅ Contains presentation logic
- ✅ Exposes properties for data binding
- ✅ Exposes commands for user actions
- ✅ Implements `INotifyPropertyChanged` (via base class)
- ✅ No direct reference to UI controls
- ✅ Can be unit tested

---

## Project Structure

```
CoffeeShop/
│
├── Models/                          ◄─── MODEL LAYER
│   ├── CoffeeShopMenuItem.cs        # Menu item data structure
│   ├── Order.cs                     # Order data with history
│   ├── OrderLine.cs                 # Individual order line items
│   └── User.cs                      # User account model
│
├── PageModels/                      ◄─── VIEWMODEL LAYER
│   ├── BaseViewModel.cs             # Base class with Title, IsBusy
│   ├── CategoriesPageModel.cs       # Categories navigation logic
│   ├── CheckoutPageModel.cs         # Checkout processing logic
│   ├── HistoryPageModel.cs          # Order history display logic
│   ├── LoginPageModel.cs            # Authentication logic
│   ├── MenuPageModel.cs             # Menu browsing and basket logic
│   └── ReceiptPageModel.cs          # Receipt display logic
│
├── Pages/                           ◄─── VIEW LAYER
│   ├── CategoriesPage.xaml          # Categories navigation UI
│   ├── CheckoutPage.xaml            # Checkout UI
│   ├── HistoryPage.xaml             # Order history UI
│   ├── LoginPage.xaml               # Login/registration UI
│   ├── MenuPage.xaml                # Menu items UI
│   └── ReceiptPage.xaml             # Receipt UI
│
├── Services/                        ◄─── INFRASTRUCTURE
│   ├── BasketService.cs             # Shopping basket management
│   ├── DatabaseService.cs           # SQLite data access
│   ├── MenuService.cs               # Menu data provider
│   ├── OrderStore.cs                # Order persistence
│   └── UserSession.cs               # User authentication state
│
├── Converters/                      ◄─── VALUE CONVERTERS
│   └── LoginConverters.cs           # Binding value transformations
│
└── Resources/
    ├── Images/                      # App images and backgrounds
    └── Styles/                      # XAML styles and colors
```

---

## CommunityToolkit.Mvvm

The Coffee Shop app uses **CommunityToolkit.Mvvm** (formerly MVVM Toolkit), which provides source generators to reduce boilerplate code in ViewModels.

### Key Features Used

#### 1. `[ObservableProperty]` - Automatic Property Notification

**Before (Manual Implementation):**
```csharp
private string username;
public string Username
{
    get => username;
    set
    {
        if (username != value)
        {
            username = value;
            OnPropertyChanged(nameof(Username));
        }
    }
}
```

**After (With Source Generator):**
```csharp
[ObservableProperty]
private string username = "";
```

The source generator automatically creates the public property with change notification!

#### 2. `[RelayCommand]` - Automatic Command Generation

**Before (Manual Implementation):**
```csharp
private ICommand submitCommand;
public ICommand SubmitCommand => submitCommand ??= new Command(async () => await SubmitAsync());

private async Task SubmitAsync()
{
    // Logic here
}
```

**After (With Source Generator):**
```csharp
[RelayCommand]
private async Task SubmitAsync()
{
    // Logic here
}
```

The generator creates a public `SubmitCommand` property automatically!

#### 3. `partial` Classes - Enable Source Generation

```csharp
public partial class ReceiptPageModel : BaseViewModel
//     ↑ partial keyword allows source generators to extend the class
```

The `partial` modifier allows the source generator to add code to your class at compile time.

---

## Data Binding

Data binding creates a connection between the View (XAML) and ViewModel (PageModel) properties.

### One-Way Binding (ViewModel → View)

```xaml
<!-- View automatically updates when WelcomeMessage changes -->
<Label Text="{Binding WelcomeMessage}" />
```

```csharp
// PageModels/CategoriesPageModel.cs
[ObservableProperty]
private string welcomeMessage = "";

// When this changes, the Label automatically updates
WelcomeMessage = $"Welcome, {user.FullName}!";
```

### Two-Way Binding (View ↔ ViewModel)

```xaml
<!-- User input updates Username property, and property changes update Entry -->
<Entry Text="{Binding Username}" />
```

```csharp
// PageModels/LoginPageModel.cs
[ObservableProperty]
private string username = "";

// Typing in Entry updates this property
// Setting this property updates Entry text
```

### How It Works

```
┌──────────────┐                    ┌────────────────────┐
│     VIEW     │                    │    VIEWMODEL       │
│              │                    │                    │
│  <Entry>     │                    │  [ObservableProperty]
│  Text="{...}"├───── Binding ─────►│  string username   │
│              │                    │                    │
│  Display: "mike"                  │  Value: "mike"     │
│              │◄── OnPropertyChanged─┤                 │
│  Updates     │      Notification   │  Changes to "john"│
│  to "john"   │                    │                    │
└──────────────┘                    └────────────────────┘
```

---

## Commands

Commands provide a way for the View to invoke methods in the ViewModel without tight coupling.

### Command Binding

```xaml
<!-- Button invokes AddCommand when clicked -->
<Button Text="+"
        Command="{Binding AddCommand}"
        CommandParameter="{Binding .}" />
```

```csharp
// PageModels/MenuPageModel.cs
[RelayCommand]
private void Add(CoffeeShopMenuItem item)
{
    _basketService.Add(item);
    OnPropertyChanged(nameof(BasketTotal));
}
```

### Command Flow

```
1. User taps button
        ↓
2. Command binding invokes AddCommand
        ↓
3. AddCommand executes Add method
        ↓
4. Add method updates basket
        ↓
5. OnPropertyChanged notifies UI
        ↓
6. UI automatically updates
```

### Async Commands

```csharp
// Async command example
[RelayCommand]
private async Task BackToCategoriesAsync()
{
    await Shell.Current.GoToAsync("//categories");
}
```

```xaml
<!-- Async command binding -->
<Button Text="Back to Categories" 
        Command="{Binding BackToCategoriesCommand}" />
```

---

## Complete Example Flow

Let's trace how adding an item to the basket works through all MVVM layers:

### Step 1: User Interaction (VIEW)

User taps the '+' button on a menu item:

```xaml
<!-- Pages/MenuPage.xaml -->
<Button Text="+"
        WidthRequest="50"
        MinimumWidthRequest="50"
        Command="{Binding Source={RelativeSource AncestorType={x:Type pages:MenuPage}}, 
                          Path=BindingContext.AddCommand}"
        CommandParameter="{Binding .}" />
```

### Step 2: Command Invocation (VIEWMODEL)

The `AddCommand` executes the `Add` method:

```csharp
// PageModels/MenuPageModel.cs
public partial class MenuPageModel : BaseViewModel
{
    private readonly BasketService _basketService;
    private readonly MenuService _menuService;

    [RelayCommand]
    private void Add(CoffeeShopMenuItem item)
    {
        _basketService.Add(item);
        OnPropertyChanged(nameof(BasketTotal));
    }

    public decimal BasketTotal => _basketService.Total;
}
```

### Step 3: Service Updates Model (SERVICE + MODEL)

The `BasketService` updates the basket:

```csharp
// Services/BasketService.cs
public class BasketService
{
    public ObservableCollection<OrderLine> Lines { get; } = new();

    public void Add(CoffeeShopMenuItem item)
    {
        var line = Lines.FirstOrDefault(l => l.Item.Id == item.Id);
        if (line is null)
        {
            Lines.Add(new OrderLine { Item = item, Quantity = 1 });
        }
        else
        {
            line.Quantity++;
            // Force UI refresh
            var idx = Lines.IndexOf(line);
            Lines.RemoveAt(idx);
            Lines.Insert(idx, line);
        }
    }

    public decimal Total => Lines.Sum(l => l.LineTotal);
}
```

### Step 4: UI Updates Automatically (VIEW)

The checkout button automatically updates:

```xaml
<!-- Pages/MenuPage.xaml -->
<Button Text="{Binding BasketTotal, StringFormat='Checkout (Total: €{0:0.00})'}"
        Command="{Binding CheckoutCommand}"
        BackgroundColor="{StaticResource Tertiary}" />
```

### Complete Flow Diagram

```
┌─────────────────────────────────────────────────────────┐
│                   1. USER ACTION                        │
│  User taps '+' button on "Cappuccino" item             │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│              2. VIEW (MenuPage.xaml)                    │
│  <Button Command="{Binding AddCommand}"                │
│          CommandParameter="Cappuccino" />               │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│          3. VIEWMODEL (MenuPageModel.cs)                │
│  [RelayCommand]                                         │
│  void Add(CoffeeShopMenuItem item)                      │
│  {                                                       │
│      _basketService.Add(item);                          │
│      OnPropertyChanged(nameof(BasketTotal));            │
│  }                                                       │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│          4. SERVICE (BasketService.cs)                  │
│  void Add(CoffeeShopMenuItem item)                      │
│  {                                                       │
│      Lines.Add(new OrderLine {                          │
│          Item = item, Quantity = 1                      │
│      });                                                 │
│  }                                                       │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│               5. MODEL (OrderLine)                      │
│  New OrderLine created:                                 │
│  - Item: Cappuccino (€3.50)                            │
│  - Quantity: 1                                          │
│  - LineTotal: €3.50                                     │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│         6. PROPERTY CHANGED NOTIFICATION                │
│  OnPropertyChanged(nameof(BasketTotal))                 │
│  Notifies View that BasketTotal changed                 │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│              7. VIEW AUTO-UPDATE                        │
│  Checkout button text updates:                          │
│  "Checkout (Total: €0.00)" → "Checkout (Total: €3.50)" │
└─────────────────────────────────────────────────────────┘
```

---

## Real Code Examples

### Example 1: LoginPageModel - Complete MVVM ViewModel

```csharp
// PageModels/LoginPageModel.cs
public partial class LoginPageModel : BaseViewModel
{
    private readonly UserSession _userSession;

    public LoginPageModel(UserSession userSession)
    {
        _userSession = userSession;
    }

    // Observable properties - bound to View inputs
    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string phoneNumber = "";
    [ObservableProperty] private bool isRegistering;
    [ObservableProperty] private string errorMessage = "";

    // Computed property for error visibility
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Commands - bound to View buttons
    [RelayCommand]
    private async Task SubmitAsync()
    {
        ErrorMessage = "";
        
        if (IsRegistering)
            await RegisterAsync();
        else
            await LoginAsync();
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsRegistering = !IsRegistering;
        ErrorMessage = "";
    }

    private async Task LoginAsync()
    {
        var (success, message) = await _userSession.LoginAsync(Username, Password);
        
        if (success)
        {
            await Shell.Current.GoToAsync("//categories");
        }
        else
        {
            ErrorMessage = message;
            OnPropertyChanged(nameof(HasError));
        }
    }

    private async Task RegisterAsync()
    {
        var (success, message) = await _userSession.RegisterAsync(
            Username, Password, FullName, PhoneNumber);
        
        if (success)
        {
            await Shell.Current.GoToAsync("//categories");
        }
        else
        {
            ErrorMessage = message;
            OnPropertyChanged(nameof(HasError));
        }
    }
}
```

### Example 2: CategoriesPageModel - Navigation Logic

```csharp
// PageModels/CategoriesPageModel.cs
public partial class CategoriesPageModel : BaseViewModel
{
    private readonly UserSession _userSession;

    public CategoriesPageModel(UserSession userSession)
    {
        _userSession = userSession;
    }

    [ObservableProperty]
    private string welcomeMessage = "";

    // Called when page appears
    public override void OnAppearing()
    {
        base.OnAppearing();
        
        if (_userSession.CurrentUser != null)
        {
            WelcomeMessage = $"Welcome, {_userSession.CurrentUser.FullName}!";
        }
    }

    // Navigation command with parameter
    [RelayCommand]
    private async Task OpenCategoryAsync(string category)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Category", category }
        };
        
        await Shell.Current.GoToAsync("menu", parameters);
    }

    [RelayCommand]
    private async Task OpenHistoryAsync()
    {
        await Shell.Current.GoToAsync("history");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _userSession.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}
```

### Example 3: MenuPageModel - Complex Business Logic

```csharp
// PageModels/MenuPageModel.cs
[QueryProperty(nameof(Category), "Category")]
public partial class MenuPageModel : BaseViewModel
{
    private readonly BasketService _basketService;
    private readonly MenuService _menuService;

    public MenuPageModel(BasketService basketService, MenuService menuService)
    {
        _basketService = basketService;
        _menuService = menuService;
    }

    [ObservableProperty]
    private string category = "";

    public ObservableCollection<CoffeeShopMenuItem> Items { get; } = new();

    // Computed property
    public decimal BasketTotal => _basketService.Total;

    partial void OnCategoryChanged(string value)
    {
        Title = value switch
        {
            "HotDrinks" => "Hot Drinks",
            "ColdDrinks" => "Cold Drinks",
            "Food" => "Food",
            _ => "Menu"
        };
        LoadItems();
    }

    private void LoadItems()
    {
        Items.Clear();
        var categoryEnum = Enum.Parse<MenuCategory>(Category);
        var items = _menuService.GetItemsByCategory(categoryEnum);
        
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    [RelayCommand]
    private void Add(CoffeeShopMenuItem item)
    {
        _basketService.Add(item);
        OnPropertyChanged(nameof(BasketTotal));
    }

    [RelayCommand]
    private void Decrease(CoffeeShopMenuItem item)
    {
        _basketService.Decrease(item);
        OnPropertyChanged(nameof(BasketTotal));
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        if (_basketService.Lines.Count == 0)
            return;
            
        await Shell.Current.GoToAsync("checkout");
    }
}
```

### Example 4: View Binding (XAML)

```xaml
<!-- Pages/CategoriesPage.xaml -->
<ContentPage x:Class="CoffeeShop.Pages.CategoriesPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="Categories">

    <VerticalStackLayout Padding="60,60,60,10" Spacing="12">
        
        <!-- One-Way Binding: ViewModel → View -->
        <Label Text="{Binding WelcomeMessage}" 
               FontSize="20" 
               FontAttributes="Bold"
               HorizontalOptions="Center" />

        <!-- Command with Parameter -->
        <Button Text="Hot drinks"
                Command="{Binding OpenCategoryCommand}"
                CommandParameter="HotDrinks" />

        <Button Text="Cold drinks"
                Command="{Binding OpenCategoryCommand}"
                CommandParameter="ColdDrinks" />

        <Button Text="Food"
                Command="{Binding OpenCategoryCommand}"
                CommandParameter="Food" />

        <BoxView HeightRequest="1" Opacity="0.2" />

        <!-- Simple Command -->
        <Button Text="Order History"
                Command="{Binding OpenHistoryCommand}" />
        
        <Button Text="Logout"
                Command="{Binding LogoutCommand}"
                BackgroundColor="{StaticResource Gray100}" />
    </VerticalStackLayout>
</ContentPage>
```

---

## Services Layer

While not part of traditional MVVM, the Services layer is crucial infrastructure:

```csharp
// Services provide reusable business logic
public class BasketService
{
    public ObservableCollection<OrderLine> Lines { get; } = new();
    public decimal Total => Lines.Sum(l => l.LineTotal);
    
    public void Add(CoffeeShopMenuItem item) { /* ... */ }
    public void Decrease(CoffeeShopMenuItem item) { /* ... */ }
    public void Clear() { /* ... */ }
}
```

### Services in Coffee Shop

| Service | Responsibility |
|---------|---------------|
| `BasketService` | Shopping basket state management |
| `DatabaseService` | SQLite database operations |
| `MenuService` | Menu data provider |
| `OrderStore` | Order persistence |
| `UserSession` | Authentication and user state |

### Dependency Injection

Services are injected into ViewModels via constructor:

```csharp
// MauiProgram.cs
builder.Services.AddSingleton<BasketService>();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<UserSession>();

builder.Services.AddTransient<MenuPageModel>();
builder.Services.AddTransient<CheckoutPageModel>();
```

```csharp
// PageModels/MenuPageModel.cs
public MenuPageModel(BasketService basketService, MenuService menuService)
{
    _basketService = basketService;
    _menuService = menuService;
}
```

---

## Benefits of MVVM

### 1. Separation of Concerns

**Without MVVM (Code-Behind):**
```csharp
// LoginPage.xaml.cs (BAD - mixing UI and logic)
private async void LoginButton_Clicked(object sender, EventArgs e)
{
    var username = UsernameEntry.Text;
    var password = PasswordEntry.Text;
    
    if (string.IsNullOrEmpty(username))
    {
        ErrorLabel.Text = "Username required";
        return;
    }
    
    // Business logic mixed with UI logic
    var user = await _database.GetUserAsync(username);
    // ...
}
```

**With MVVM:**
```csharp
// LoginPageModel.cs (GOOD - pure logic)
[RelayCommand]
private async Task LoginAsync()
{
    if (string.IsNullOrEmpty(Username))
    {
        ErrorMessage = "Username required";
        return;
    }
    
    var (success, message) = await _userSession.LoginAsync(Username, Password);
    // ...
}
```

### 2. Testability

ViewModels can be unit tested without UI:

```csharp
[Test]
public void Add_ItemToBasket_IncreasesTotal()
{
    // Arrange
    var basketService = new BasketService();
    var menuService = new MenuService();
    var viewModel = new MenuPageModel(basketService, menuService);
    var item = new CoffeeShopMenuItem 
    { 
        Id = Guid.NewGuid(), 
        Name = "Cappuccino", 
        Price = 3.50m 
    };

    // Act
    viewModel.AddCommand.Execute(item);

    // Assert
    Assert.AreEqual(3.50m, viewModel.BasketTotal);
    Assert.AreEqual(1, basketService.Lines.Count);
}
```

### 3. Data Binding Reduces Boilerplate

**Without Binding:**
```csharp
// Manual UI updates (BAD)
private void UpdateUI()
{
    UsernameLabel.Text = _currentUser.Username;
    WelcomeLabel.Text = $"Welcome, {_currentUser.FullName}!";
    BasketTotalLabel.Text = $"Total: €{_basket.Total:0.00}";
}
```

**With Binding:**
```xaml
<!-- Automatic UI updates (GOOD) -->
<Label Text="{Binding Username}" />
<Label Text="{Binding WelcomeMessage}" />
<Label Text="{Binding BasketTotal, StringFormat='Total: €{0:0.00}'}" />
```

### 4. Maintainability

Changes to one layer don't affect others:

```
Change UI Design:
✅ Modify XAML only
✅ ViewModels unchanged
✅ Models unchanged

Change Business Logic:
✅ Modify ViewModel only
✅ XAML unchanged (same bindings)
✅ Models unchanged

Change Data Structure:
✅ Modify Models only
✅ Update Services/ViewModels as needed
✅ XAML unchanged (same bindings)
```

### 5. Reusability

ViewModels can work with different Views:

```
MenuPageModel
    ↓
├─► MenuPage.xaml (Mobile)
├─► MenuPageDesktop.xaml (Desktop)
└─► MenuPageTablet.xaml (Tablet)
```

---

## Best Practices

### 1. Keep ViewModels Pure

✅ **DO:**
```csharp
[RelayCommand]
private async Task SaveAsync()
{
    var order = CreateOrder();
    await _orderStore.SaveAsync(order);
    await Shell.Current.GoToAsync("receipt", new Dictionary<string, object> 
    { 
        { "Order", order } 
    });
}
```

❌ **DON'T:**
```csharp
[RelayCommand]
private async Task SaveAsync()
{
    // Don't reference UI controls directly
    SaveButton.IsEnabled = false;
    LoadingIndicator.IsVisible = true;
}
```

### 2. Use ObservableProperty for Simple Properties

✅ **DO:**
```csharp
[ObservableProperty]
private string username = "";
```

❌ **DON'T:**
```csharp
private string username = "";
public string Username
{
    get => username;
    set => SetProperty(ref username, value);
}
```

### 3. Use RelayCommand for Commands

✅ **DO:**
```csharp
[RelayCommand]
private async Task SubmitAsync()
{
    // Logic here
}
```

❌ **DON'T:**
```csharp
private ICommand submitCommand;
public ICommand SubmitCommand => 
    submitCommand ??= new Command(async () => await SubmitAsync());
```

### 4. Notify UI of Computed Property Changes

✅ **DO:**
```csharp
[RelayCommand]
private void Add(CoffeeShopMenuItem item)
{
    _basketService.Add(item);
    OnPropertyChanged(nameof(BasketTotal));  // ← Notify computed property changed
}

public decimal BasketTotal => _basketService.Total;
```

### 5. Use Services for Shared Logic

✅ **DO:**
```csharp
public class MenuPageModel : BaseViewModel
{
    private readonly BasketService _basketService;  // ← Service
    
    [RelayCommand]
    private void Add(CoffeeShopMenuItem item)
    {
        _basketService.Add(item);  // ← Delegate to service
    }
}
```

❌ **DON'T:**
```csharp
public class MenuPageModel : BaseViewModel
{
    private List<OrderLine> basket = new();  // ← Duplicated in every ViewModel
    
    [RelayCommand]
    private void Add(CoffeeShopMenuItem item)
    {
        // Duplicate basket logic in every ViewModel
    }
}
```

### 6. Use BaseViewModel for Common Properties

```csharp
// PageModels/BaseViewModel.cs
public abstract class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "";

    [ObservableProperty]
    private bool isBusy;

    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}
```

All PageModels inherit common functionality:

```csharp
public partial class MenuPageModel : BaseViewModel
{
    // Inherits Title and IsBusy properties
}
```

---

## Data Flow Diagram

```
┌───────────────────────────────────────────────────────────┐
│                      VIEW (XAML)                          │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐ │
│  │  Entry   │  │  Button  │  │  Label   │  │ListView  │ │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘ │
│       │             │              ↑              ↑        │
└───────┼─────────────┼──────────────┼──────────────┼────────┘
        │             │              │              │
    {Binding}    {Command}      {Binding}    {ItemsSource}
        │             │              │              │
        ↓             ↓              │              │
┌───────┴─────────────┴──────────────┴──────────────┴────────┐
│              VIEWMODEL (PageModel)                         │
│  ┌─────────────────────────────────────────────────────┐  │
│  │ [ObservableProperty] string username                │  │
│  │ [RelayCommand] async Task LoginAsync()              │  │
│  │ public string WelcomeMessage { get; }               │  │
│  │ public ObservableCollection<Order> Orders { get; }  │  │
│  │ INotifyPropertyChanged implementation               │  │
│  └──────────────────────┬──────────────────────────────┘  │
│                         │                                  │
└─────────────────────────┼──────────────────────────────────┘
                          │
                          ↓
┌─────────────────────────┴──────────────────────────────────┐
│                  SERVICES (Business Logic)                 │
│  ┌───────────────┐  ┌───────────────┐  ┌──────────────┐  │
│  │BasketService  │  │ UserSession   │  │OrderStore    │  │
│  └───────┬───────┘  └───────┬───────┘  └──────┬───────┘  │
│          │                   │                  │          │
└──────────┼───────────────────┼──────────────────┼──────────┘
           │                   │                  │
           ↓                   ↓                  ↓
┌──────────┴───────────────────┴──────────────────┴──────────┐
│               DATABASE (SQLite via DatabaseService)        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│  │Users Table  │  │Orders Table │  │(In Memory)  │       │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘       │
│         │                │                 │               │
└─────────┼────────────────┼─────────────────┼───────────────┘
          │                │                 │
          ↓                ↓                 ↓
┌─────────┴────────────────┴─────────────────┴───────────────┐
│                   MODELS (Data Structures)                  │
│     User, Order, OrderLine, CoffeeShopMenuItem            │
└─────────────────────────────────────────────────────────────┘
```

---

## Summary

### MVVM in Coffee Shop App

| Component | Files | Responsibility |
|-----------|-------|---------------|
| **Model** | `Models/*.cs` | Data structures (User, Order, etc.) |
| **View** | `Pages/*.xaml` | UI markup with data bindings |
| **ViewModel** | `PageModels/*PageModel.cs` | Presentation logic, properties, commands |
| **Services** | `Services/*.cs` | Infrastructure and shared logic |

### Key Technologies

- ✅ **CommunityToolkit.Mvvm** - Source generators for reduced boilerplate
- ✅ **Data Binding** - Automatic UI updates via `{Binding}`
- ✅ **Commands** - UI actions via `{Command}`
- ✅ **ObservableCollection** - Automatic list updates
- ✅ **Dependency Injection** - Service registration and injection
- ✅ **Navigation** - Shell-based navigation with parameters

### Benefits Achieved

1. **Clean Architecture** - Separation of UI, logic, and data
2. **Maintainability** - Easy to modify and extend
3. **Testability** - ViewModels can be unit tested
4. **Reusability** - Services and ViewModels are reusable
5. **Productivity** - Less boilerplate code with source generators

---

## Additional Resources

### Official Documentation

- **.NET MAUI Documentation**  
  https://docs.microsoft.com/en-us/dotnet/maui/

- **CommunityToolkit.Mvvm Documentation**  
  https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/

- **Data Binding Guide**  
  https://docs.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/

### MVVM Pattern

- **MVVM Pattern Overview**  
  https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel

- **Microsoft MVVM Guidance**  
  https://docs.microsoft.com/en-us/windows/apps/design/basics/mvvm-overview

### Related Documentation

- **README.md** - Project overview and features
- **SECURITY-HASHING.md** - Password security implementation
- **DEPLOYMENT.md** - Application deployment guide

---

## Document Information

**Created:** January 2025  
**Project:** Coffee Shop - .NET MAUI Application  
**Developer:** Michael McKibbin (ATU Student# L00197067)  
**Version:** 1.0  
**Related Files:** `PageModels/*.cs`, `Pages/*.xaml`, `Models/*.cs`

---

*This document is part of the Coffee Shop application's technical documentation. For project overview, see [README.md](README.md).*
