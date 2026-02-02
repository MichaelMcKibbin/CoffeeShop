# ☕ Coffee Shop ☕

A modern .NET MAUI mobile application for ordering coffee, food, and beverages. Built with .NET 10 and featuring a beautiful coffee-themed UI with automatic dark mode support, user authentication, and order management.

## ✨ Key Highlights

- 🌓 **Smart Theme System** - Automatic dark mode detection with manual override option
- 👤 **Flexible Authentication** - Login, register, or continue as guest
- 🛒 **Intuitive Shopping** - Easy-to-use basket with real-time total updates
- 📱 **Cross-Platform** - Runs on Android, iOS, and Windows
- 💾 **Local Storage** - SQLite database for offline-first functionality
- 🎨 **Beautiful UI** - Coffee-themed design with responsive layouts

## Features

### User Authentication
- **User Registration** - Create new customer accounts with full name and phone number
- **Secure Login** - Password-protected access with SHA-256 hashing
- **Guest Mode** - Continue as guest without creating an account (orders not saved)
- **User Sessions** - Persistent login state with automatic profile loading

### Menu & Ordering
- **Category Browsing** - Navigate through Hot Drinks, Cold Drinks, and Food categories
- **Shopping Basket** - Add/remove items with quantity management
- **Order Checkout** - Auto-populated customer details from user profile
- **Receipt Generation** - Detailed order confirmation with unique order numbers

### Order Management
- **Order History** - View all past orders with formatted dates and details (registered users only)
- **Order Tracking** - Each order includes timestamp, items, and total amount
- **Guest Orders** - Guests can place orders and receive receipts without account creation

### UI/UX Features
- **Background Images** - Coffee-themed backgrounds with optimized opacity
- **Responsive Design** - 80% width layout that adapts to different screen sizes
- **Custom Color Theme** - Coffee shop color palette (Espresso, Latte, Caramel)
- **Touch-Optimized** - Minimum 44px touch targets for accessibility
- **🌓 Automatic Dark Mode** - Follows system theme settings (Android, iOS, Windows)
- **🎨 Manual Theme Toggle** - Override system theme in Settings page
- **Theme Persistence** - Theme preference saved across app restarts

## Technology Stack

- **.NET 10** - Latest .NET framework
- **.NET MAUI** - Cross-platform UI framework (Android/iOS/Windows)
- **SQLite** - Local database for users and orders
- **CommunityToolkit.Mvvm** - MVVM helpers and commands
- **Syncfusion.Maui.Toolkit** - UI components
- **C# 14** - Modern C# features

## Project Structure

```
CoffeeShop/
├── Models/
│   ├── CoffeeShopMenuItem.cs    # Menu item data model
│   ├── Order.cs                  # Order data model with history
│   ├── OrderLine.cs              # Individual order line items
│   └── User.cs                   # User account model
├── PageModels/
│   ├── BaseViewModel.cs          # Base MVVM ViewModel
│   ├── CategoriesPageModel.cs    # Main navigation logic
│   ├── CheckoutPageModel.cs      # Checkout and order placement
│   ├── HistoryPageModel.cs       # Order history display
│   ├── LoginPageModel.cs         # Authentication logic
│   ├── MenuPageModel.cs          # Menu browsing and basket
│   ├── ReceiptPageModel.cs       # Order receipt display
│   └── SettingsPageModel.cs      # App settings and theme control
├── Pages/
│   ├── CategoriesPage.xaml       # Main menu navigation
│   ├── CheckoutPage.xaml         # Order checkout
│   ├── HistoryPage.xaml          # Order history
│   ├── LoginPage.xaml            # Login/registration/guest
│   ├── MenuPage.xaml             # Menu item browsing
│   ├── ReceiptPage.xaml          # Order confirmation
│   └── SettingsPage.xaml         # Theme settings
├── Services/
│   ├── BasketService.cs          # Shopping basket management
│   ├── DatabaseService.cs        # SQLite data access
│   ├── MenuService.cs            # Menu data provider
│   ├── OrderStore.cs             # Order persistence
│   └── UserSession.cs            # User authentication & guest mode
└── Resources/
    ├── Images/                   # App images and icons
    └── Styles/                   # XAML styles and colors
```

## Getting Started

### Prerequisites
- Visual Studio 2022 (17.8 or later)
- .NET 10 SDK
- Android SDK (for Android deployment)
- iOS development tools (for iOS deployment, macOS only)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/MichaelMcKibbin/CoffeeShop.git
   cd CoffeeShop
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   - Open `CoffeeShop.sln` in Visual Studio
   - Select your target platform (Android/iOS)
   - Press F5 to run

## Usage

### First Time Setup

**Option 1: Create Account (Recommended)**
1. Launch the app and click "Register" on the login page
2. Enter your username, password, full name, and phone number
3. The app will automatically log you in after registration
4. Your orders will be saved to view later in Order History

**Option 2: Continue as Guest**
1. Launch the app and click "Continue as Guest"
2. Browse and order without creating an account
3. Provide name and phone number at checkout
4. Receive order receipt immediately
5. Note: Guest orders are not saved to history

### Placing an Order
1. Select a category (Hot drinks, Cold drinks, or Food)
2. Use +/- buttons to add items to your basket
3. Click "Checkout" to review your order
4. Customer details are auto-populated from your profile (or enter manually for guests)
5. Click "Place Order" to complete
6. View your order confirmation and receipt

### Viewing Order History
1. From the Categories page, click "Order History"
2. See all past orders with formatted dates
3. Each order shows items, quantities, and total amount
4. Note: Only available for registered users

### Theme Settings
The app automatically detects your device's light/dark mode setting and adapts the interface accordingly.

**To manually control the theme:**
1. From the Categories page, click "⚙️ Settings"
2. Toggle "Use System Theme" OFF to enable manual control
3. Toggle "Dark Mode" ON/OFF to set your preference
4. Your theme preference is saved and persists across app restarts

**Theme Modes:**
- **System Theme (Default)** - Follows your device's dark mode setting
- **Manual Light** - Always shows light theme regardless of device setting
- **Manual Dark** - Always shows dark theme regardless of device setting

## Design Features

### Theme System
The app features a sophisticated dual-theme system with automatic device detection and manual override capability.

**Automatic Theme Detection:**
- ✅ Follows system dark mode settings on Android, iOS, and Windows
- ✅ Updates in real-time when device theme changes
- ✅ Applies to all UI elements, backgrounds, and text
- ✅ Status bar color matches app theme (Android)

**Manual Theme Control:**
- 🎨 Access via Settings page (⚙️ button on Categories page)
- 🔄 Toggle "Use System Theme" to enable manual control
- 🌓 Switch between Light and Dark modes independently
- 💾 Theme preference saved locally and persists across sessions

**Theme-Aware Components:**
- Background images with adaptive opacity (lighter in dark mode)
- Text colors optimized for contrast in both themes
- Button styles with appropriate colors for light/dark backgrounds
- Border colors and UI elements that adapt to theme

**Color Palettes:**

**Light Mode:**
- Background: `#FAF7F2` (Warm off-white)
- Text: `#1C1B1A` (Almost black)
- Primary: `#4E342E` (Espresso brown)
- Secondary: `#F3E9DC` (Latte cream)

**Dark Mode:**
- Background: `#161312` (Warm dark)
- Text: `#E7E2DC` (Light cream)
- Primary: `#4E342E` (Espresso brown)
- Secondary: `#F3E9DC` (Latte cream)

### Color Palette
- **Primary (Espresso)**: `#4E342E` - Main buttons and branding
- **Secondary (Latte Cream)**: `#F3E9DC` - Accents and highlights
- **Tertiary (Caramel)**: `#C69C6D` - Purchase action buttons
- **Background**: Coffee-themed image with adaptive opacity (40% light, 20% dark)

### Typography
- **OpenSans** - Primary font family
- Responsive font sizes (18px base, 24px desktop)
- Bold headers for navigation and emphasis
- High contrast text colors for accessibility

### Layout
- **80% width** responsive design with horizontal padding
- **Centered content** for optimal readability
- **12-16px spacing** between elements for touch targets
- **Theme-aware borders** and separators

## Database Schema

### Users Table
- `Id` (PrimaryKey, AutoIncrement)
- `Username` (Unique)
- `PasswordHash` (SHA-256)
- `FullName`
- `PhoneNumber`
- `CreatedAt`

### Orders Table
- `Id` (PrimaryKey, AutoIncrement)
- `UserId` (Foreign Key to Users)
- `OrderNumber` (Format: yyyyMMdd-HHmmss)
- `LocalDateTime`
- `CustomerName`
- `Telephone`
- `LinesJson` (Serialized order items)

### MenuItems Table
- `Id` (GUID)
- `Name`
- `Price` (Decimal)
- `Category` (Enum: HotDrinks, ColdDrinks, Food)

## Security

- **Password Hashing**: SHA-256 hashing for password storage
- **Local Storage**: SQLite database stored in app's private directory
- **Session Management**: User sessions cleared on logout
- **Input Validation**: Username, password, and form validation

> **Note**: This is a demonstration app. For production use, implement:
> - BCrypt, PBKDF2, or Argon2 password hashing
> - Server-side API authentication
> - OAuth/JWT tokens
> - HTTPS for network communication

## Potential Future Enhancements

- [ ] Cloud sync for order history across devices
- [ ] Payment gateway integration
- [ ] Push notifications for order updates
- [ ] Loyalty points and rewards program
- [ ] Order customization (size, milk type, extras)
- [ ] Store locator with maps integration
- [ ] Social media sharing
- [ ] Dietary filters (vegan, gluten-free, etc.)
- [ ] iOS dynamic theme support improvements
- [ ] Custom theme color picker
- [ ] Accessibility: High contrast mode
- [ ] Account recovery (forgot password)
- [ ] Guest to registered user conversion flow

## License

This project is open source and available for educational purposes.

## Developer

**Michael McKibbin**
- GitHub: [@MichaelMcKibbin](https://github.com/MichaelMcKibbin)
- ATU Student# L00197067

## Acknowledgments

- Built with .NET MAUI
- Icons and UI components from Syncfusion
- Community Toolkit for MVVM patterns
- Coffee-themed design inspired by artisan coffee culture

---

☕ **"It's not Java anymore!"** ☕ - Enjoy your .NET MAUI Coffee Shop experience!