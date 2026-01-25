# 📱 Deployment Guide - Coffee Shop App

## Overview

This guide provides comprehensive instructions for deploying the Coffee Shop .NET MAUI application to end users across different platforms. Users can install and run the app on their devices without needing Visual Studio.

## Table of Contents

- [Quick Start](#quick-start)
- [Android Deployment](#android-deployment)
- [iOS Deployment](#ios-deployment)
- [Windows Deployment](#windows-deployment)
- [Web Distribution](#web-distribution)
- [Pre-Release Checklist](#pre-release-checklist)
- [Troubleshooting](#troubleshooting)

---

## Quick Start

### Fastest Method for Students/Personal Use

**GitHub Releases (Recommended)**

1. Build release APK:
```bash
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk
```

2. Create GitHub release:
```bash
git tag v1.0.0
git push origin v1.0.0
```

3. Upload APK to GitHub Releases
4. Share download link with users

**Output Location:**
```
bin/Release/net10.0-android/publish/com.companyname.coffeeshop-Signed.apk
```

---

## Android Deployment

### Prerequisites

- **.NET 10 SDK** installed
- **Android SDK** configured
- **Java Development Kit (JDK)** 11 or later

### Option 1: Google Play Store (Professional Distribution)

#### Step 1: Create Google Play Console Account

**Cost:** $25 USD (one-time fee)

**Process:**
1. Go to https://play.google.com/console
2. Create developer account
3. Pay registration fee
4. Accept developer agreement

#### Step 2: Prepare AndroidManifest.xml

**Location:** `Platforms/Android/AndroidManifest.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application 
        android:label="Coffee Shop"
        android:icon="@mipmap/appicon"
        android:supportsRtl="true"
        android:debuggable="false">  <!-- MUST be false for release -->
    </application>
    
    <uses-sdk 
        android:minSdkVersion="21" 
        android:targetSdkVersion="34" />
    
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
</manifest>
```

#### Step 3: Create Signing Keystore

**⚠️ Critical:** Store this file securely! You'll need it for all future updates.

```bash
# Open Command Prompt or PowerShell
keytool -genkey -v -keystore coffeeshop.keystore -alias coffeeshop -keyalg RSA -keysize 2048 -validity 10000
```

**You'll be prompted for:**
- Keystore password (remember this!)
- Your name
- Organization unit
- Organization name
- City/Locality
- State/Province
- Country code
- Key password (can match keystore password)

**Example:**
```
Enter keystore password: MySecurePassword123!
Re-enter new password: MySecurePassword123!
What is your first and last name?
  [Unknown]:  Michael McKibbin
What is the name of your organizational unit?
  [Unknown]:  ATU
What is the name of your organization?
  [Unknown]:  Atlantic Technological University
What is the name of your City or Locality?
  [Unknown]:  Galway
What is the name of your State or Province?
  [Unknown]:  Galway
What is the two-letter country code for this unit?
  [Unknown]:  IE
Is CN=Michael McKibbin, OU=ATU, O=Atlantic Technological University, L=Galway, ST=Galway, C=IE correct?
  [no]:  yes
```

**Backup:** Store `coffeeshop.keystore` in a secure location (e.g., password manager, encrypted drive)

#### Step 4: Configure Project for Signing

**Edit `CoffeeShop.csproj`** and add:

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
  <!-- Enable signing -->
  <AndroidKeyStore>true</AndroidKeyStore>
  <AndroidSigningKeyStore>coffeeshop.keystore</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>coffeeshop</AndroidSigningKeyAlias>
  
  <!-- Package format: AAB for Play Store, APK for direct install -->
  <AndroidPackageFormat>aab</AndroidPackageFormat>
  
  <!-- ⚠️ Security Warning: Don't commit passwords to Git! -->
  <!-- Use environment variables in production -->
  <AndroidSigningKeyPass>$(AndroidSigningPassword)</AndroidSigningKeyPass>
  <AndroidSigningStorePass>$(AndroidSigningPassword)</AndroidSigningStorePass>
</PropertyGroup>
```

**Setting Password via Environment Variable:**

**PowerShell:**
```powershell
$env:AndroidSigningPassword = "MySecurePassword123!"
```

**Command Prompt:**
```cmd
set AndroidSigningPassword=MySecurePassword123!
```

**Alternative (Less Secure):** Hard-code temporarily for build:
```xml
<AndroidSigningKeyPass>YourPasswordHere</AndroidSigningKeyPass>
<AndroidSigningStorePass>YourPasswordHere</AndroidSigningStorePass>
```

#### Step 5: Build Android App Bundle (AAB)

**For Google Play Store:**
```bash
dotnet publish -f net10.0-android -c Release
```

**Output:**
```
bin/Release/net10.0-android/publish/com.companyname.coffeeshop-Signed.aab
```

**For Direct Distribution (APK):**
```bash
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk
```

**Output:**
```
bin/Release/net10.0-android/publish/com.companyname.coffeeshop-Signed.apk
```

#### Step 6: Prepare Store Assets

**Required Assets:**

1. **App Icon** - 512x512 PNG (no transparency)
2. **Feature Graphic** - 1024x500 JPG or PNG
3. **Screenshots** - At least 2, up to 8
   - Phone: 1080x1920 to 1920x1080 (portrait or landscape)
   - 7-inch Tablet (optional): 1920x1200
   - 10-inch Tablet (optional): 2560x1600

**Screenshot Tips:**
- Use Android emulator or physical device
- Capture key screens: Login, Categories, Menu, Checkout, History
- Show app functionality clearly
- No text overlays or borders

#### Step 7: Create Privacy Policy

**Required by Google Play.** Create a simple privacy policy:

**Create `PRIVACY-POLICY.md`:**

```markdown
# Privacy Policy for Coffee Shop

**Effective Date:** January 2025

## Introduction
Coffee Shop is a mobile application for ordering coffee and food items.

## Data Collection
We collect and store the following data locally on your device:
- User account information (username, name, phone number)
- Order history and transaction records
- Menu preferences

## Data Storage
All data is stored locally in an SQLite database on your device.
No data is transmitted to external servers or third parties.

## Data Security
- Passwords are hashed using SHA-256 before storage
- All data remains on your device
- No cloud synchronization
- No data sharing with third parties

## User Rights
You can:
- Delete your account data by uninstalling the app
- Access all your stored data within the app
- No data is retained after app uninstall

## Children's Privacy
This app is suitable for all ages and does not collect personal data from children.

## Changes to Privacy Policy
Updates will be posted within the app and on our GitHub repository.

## Contact
For privacy concerns, contact: michael.mckibbin@example.com

GitHub: https://github.com/MichaelMcKibbin/CoffeeShop
```

**Host Privacy Policy:**
- GitHub Pages: https://michaelmckibbin.github.io/CoffeeShop/privacy
- Or paste directly into Play Console

#### Step 8: Submit to Google Play Console

1. **Go to Play Console** → https://play.google.com/console

2. **Create New App**
   - Click "Create app"
   - App name: "Coffee Shop"
   - Default language: English (United States)
   - App or game: App
   - Free or paid: Free

3. **Complete Store Listing**
   ```
   Short description (80 chars):
   Order coffee and food with this modern .NET MAUI mobile app.
   
   Full description (4000 chars):
   ☕ Coffee Shop - Order with Ease
   
   Coffee Shop is a modern mobile application built with .NET MAUI 
   that makes ordering your favorite coffee and food items simple 
   and convenient.
   
   FEATURES:
   • User Authentication - Secure login and registration
   • Menu Browsing - Browse Hot Drinks, Cold Drinks, and Food
   • Shopping Basket - Easy item management
   • Quick Checkout - Auto-filled customer details
   • Order History - View all past orders with detailed information
   • Beautiful UI - Coffee-themed design with custom colors
   
   TECHNICAL DETAILS:
   Built with .NET 10 and .NET MAUI, featuring:
   - Local SQLite database
   - MVVM architecture
   - Responsive design
   - Dark/Light mode support
   
   EDUCATIONAL PROJECT:
   This app was created as an educational project to demonstrate 
   .NET MAUI development, mobile app architecture, and modern 
   C# programming practices.
   
   "It's not Java anymore!" ☕
   
   GitHub: https://github.com/MichaelMcKibbin/CoffeeShop
   Developer: Michael McKibbin (ATU Student)
   ```

4. **Upload Graphics**
   - App icon (512x512)
   - Feature graphic (1024x500)
   - Screenshots (minimum 2)

5. **Set Content Rating**
   - Start questionnaire
   - Select "Utility, Productivity, Communication, or Other"
   - Answer questions (all likely "No" for Coffee Shop)
   - Generate rating

6. **Set Up App Pricing**
   - Select "Free"
   - Select countries (all or specific)

7. **Upload AAB**
   - Go to "Release" → "Production"
   - Click "Create new release"
   - Upload `com.companyname.coffeeshop-Signed.aab`
   - Add release notes:
     ```
     Initial release of Coffee Shop!
     
     Features:
     - User authentication
     - Menu browsing by category
     - Shopping basket
     - Order checkout
     - Order history
     ```

8. **Review and Publish**
   - Review all sections
   - Click "Submit for review"
   - Wait for approval (typically 1-7 days)

#### Step 9: Monitor Review Status

**Check status:** Play Console → Publishing Overview

**Common rejection reasons:**
- Missing privacy policy
- Incorrect content rating
- Broken app functionality
- Missing required permissions declaration

---

### Option 2: Direct APK Distribution (Sideloading)

**Best for:** Testing, internal use, small user base

#### Step 1: Build Signed APK

```bash
# Set password (if using environment variable)
$env:AndroidSigningPassword = "YourPassword"

# Build APK
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk
```

#### Step 2: Distribute APK

**Upload to:**

1. **GitHub Releases**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   # Go to GitHub → Releases → Create Release
   # Upload APK as asset
   ```

2. **Google Drive**
   - Upload APK
   - Right-click → Get link
   - Set to "Anyone with the link"
   - Share link

3. **Dropbox**
   - Upload APK
   - Create shareable link
   - Change `?dl=0` to `?dl=1` in URL for direct download

4. **Your Website/Server**
   - Upload APK via FTP/SFTP
   - Share direct download URL

#### Step 3: Create Installation Instructions

**Create `INSTALL-ANDROID.md`:**

```markdown
# Installing Coffee Shop on Android

## Requirements
- Android 5.0 (Lollipop) or higher
- ~20 MB free storage space

## Installation Steps

### Step 1: Enable Unknown Sources
1. Open **Settings** on your Android device
2. Go to **Security** (or **Apps & Notifications** → **Special App Access**)
3. Find **"Install unknown apps"** or **"Unknown sources"**
4. Enable for your **Browser** or **File Manager**

**Note:** Settings location varies by Android version and manufacturer.

### Step 2: Download APK
1. Download `CoffeeShop.apk` from:
   - [GitHub Releases](https://github.com/MichaelMcKibbin/CoffeeShop/releases/latest)
   - Or the provided download link

2. You may see a warning - this is normal for apps outside Play Store
3. Tap **Download anyway** or **OK**

### Step 3: Install
1. Open your **Downloads** folder or notification
2. Tap the `CoffeeShop.apk` file
3. Tap **Install**
4. Wait for installation to complete
5. Tap **Open** or find "Coffee Shop" in your app drawer

### Step 4: First Launch
1. Open **Coffee Shop** ☕
2. Click **Register** to create an account
3. Enter your details
4. Start ordering!

## Troubleshooting

### "Installation Blocked"
**Solution:** Enable unknown sources (see Step 1)

### "App Not Installed"
**Possible causes:**
- Insufficient storage space (free up ~50 MB)
- Corrupted download (re-download APK)
- Android version too old (requires 5.0+)

### "App Keeps Stopping"
**Solution:** 
- Clear app data: Settings → Apps → Coffee Shop → Storage → Clear Data
- Reinstall the app
- Ensure Android 5.0 or higher

### Can't Find Downloaded File
**Solution:** 
- Check **Downloads** folder in file manager
- Check **Downloads** section in Chrome/Browser
- Re-download if needed

## Uninstallation
1. Long-press Coffee Shop icon
2. Tap **Uninstall** or drag to **Uninstall**
3. Confirm

## Support
For issues, contact: michael.mckibbin@example.com
GitHub: https://github.com/MichaelMcKibbin/CoffeeShop/issues
```

---

## iOS Deployment

### Prerequisites

- **Mac computer** (required for iOS builds)
- **Xcode** 15 or later
- **Apple Developer Account** ($99/year)
- **Visual Studio for Mac** or **Visual Studio 2022** with Mac connection

### Apple Developer Account Setup

1. **Enroll in Apple Developer Program**
   - Go to https://developer.apple.com/programs/
   - Click "Enroll"
   - Cost: $99 USD per year
   - Approval: 1-2 business days

2. **Create App ID**
   - Log in to https://developer.apple.com/account
   - Certificates, IDs & Profiles → Identifiers
   - Click "+" → App IDs → Continue
   - Description: "Coffee Shop"
   - Bundle ID: `com.companyname.coffeeshop`
   - Capabilities: None needed for basic app
   - Register

### Option 1: App Store Distribution

#### Step 1: Create App in App Store Connect

1. **Go to App Store Connect**
   - https://appstoreconnect.apple.com
   - Sign in with Apple Developer account

2. **Create New App**
   - My Apps → "+" → New App
   - Platforms: iOS
   - Name: "Coffee Shop"
   - Primary Language: English (U.S.)
   - Bundle ID: Select `com.companyname.coffeeshop`
   - SKU: `coffeeshop-001`
   - User Access: Full Access

#### Step 2: Prepare App Information

**Required Information:**

```
Name: Coffee Shop
Subtitle: Order Coffee & Food
Category: Food & Drink (Primary), Lifestyle (Secondary)
Content Rights: No, it does not contain third-party content

Privacy Policy URL: https://michaelmckibbin.github.io/CoffeeShop/privacy
Support URL: https://github.com/MichaelMcKibbin/CoffeeShop

Description:
☕ Coffee Shop - Modern Mobile Ordering

Order your favorite coffee and food items with this beautiful .NET MAUI app.

FEATURES:
• Secure user authentication
• Browse menu by category
• Easy shopping basket
• Quick checkout with saved details
• View complete order history
• Beautiful coffee-themed design

Built with .NET 10 and .NET MAUI as an educational project 
demonstrating modern mobile development.

"It's not Java anymore!" ☕

Keywords: coffee, shop, order, food, drinks, .NET, MAUI

Copyright: © 2025 Michael McKibbin
```

**Screenshots Required:**
- 6.7" Display: 1290 x 2796 (iPhone 15 Pro Max)
- 5.5" Display: 1242 x 2208 (iPhone 8 Plus) - Optional

#### Step 3: Build and Archive

**In Visual Studio for Mac:**

1. **Set Build Configuration**
   - Configuration: Release
   - Platform: iPhone (not Simulator)

2. **Update Info.plist**
   ```xml
   <key>CFBundleDisplayName</key>
   <string>Coffee Shop</string>
   <key>CFBundleShortVersionString</key>
   <string>1.0</string>
   <key>CFBundleVersion</key>
   <string>1</string>
   ```

3. **Archive**
   - Build → Archive for Publishing
   - Select signing identity (Distribution)
   - Select provisioning profile
   - Click "Sign and Distribute"

4. **Upload to App Store**
   - Choose "App Store"
   - Select team
   - Upload

#### Step 4: Submit for Review

1. **Complete App Store Information**
   - Pricing: Free
   - Availability: All countries
   - Age Rating: 4+

2. **Add Build**
   - Version Information → Build
   - Select uploaded build

3. **Submit**
   - Save
   - Submit for Review
   - Approval: 24-48 hours (typically)

### Option 2: TestFlight (Beta Testing)

**Best for:** Testing with small group before public release

#### Process

1. **Upload Build** (same as App Store)
2. **Go to TestFlight** tab in App Store Connect
3. **Add Internal Testers**
   - Members of your App Store Connect team
   - Up to 100 testers
   - Instant access

4. **Add External Testers**
   - Anyone with email address
   - Up to 10,000 testers
   - Requires Apple approval (1-2 days)

5. **Invite Testers**
   - Send email invites
   - Testers install TestFlight app
   - Accept invitation
   - Install Coffee Shop beta

---

## Windows Deployment

### Prerequisites

- **Windows 10/11** development machine
- **.NET 10 SDK** with MAUI workload
- **Visual Studio 2022** (17.8+)

### Option 1: Microsoft Store

#### Step 1: Register Developer Account

- **Cost:** $19 USD (one-time for individual)
- **URL:** https://partner.microsoft.com/dashboard
- **Process:** 
  - Create Microsoft account
  - Register as Windows app developer
  - Pay registration fee

#### Step 2: Reserve App Name

1. Go to Partner Center
2. Create new app
3. Reserve "Coffee Shop"

#### Step 3: Create MSIX Package

**Update `CoffeeShop.csproj`:**

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-windows'))">
  <!-- Package settings -->
  <GenerateAppxPackageOnBuild>true</GenerateAppxPackageOnBuild>
  <AppxPackageSigningEnabled>true</AppxPackageSigningEnabled>
  <AppxBundle>Always</AppxBundle>
  <AppxBundlePlatforms>x64|x86|ARM64</AppxBundlePlatforms>
  
  <!-- Store identity -->
  <PackageCertificateKeyFile>CoffeeShop_TemporaryKey.pfx</PackageCertificateKeyFile>
  <PackageCertificateThumbprint></PackageCertificateThumbprint>
</PropertyGroup>
```

#### Step 4: Build Package

```bash
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

Output: `bin/Release/net10.0-windows10.0.19041.0/publish/`

#### Step 5: Submit to Microsoft Store

1. **Upload MSIX** bundle to Partner Center
2. **Complete Store Listing**
   - Description
   - Screenshots (1366x768 minimum)
   - Store logos (various sizes)
3. **Set Pricing** (Free)
4. **Submit** for certification
5. **Certification:** 1-3 business days

### Option 2: Sideload MSIX

**For internal distribution without Microsoft Store**

#### Enable Sideloading

**Users must:**
1. Settings → Update & Security → For developers
2. Select "Sideload apps"

#### Install Certificate

```powershell
# Install app certificate (admin PowerShell)
Add-AppxPackage -Path "CoffeeShop.msix"
```

---

## Web Distribution

### GitHub Pages Hosting

Create a simple download page for your app.

#### Step 1: Create Landing Page

**Create `docs/index.html`:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Coffee Shop - Download</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #4E342E 0%, #6D4C41 100%);
            color: #333;
            line-height: 1.6;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }
        
        header {
            text-align: center;
            padding: 60px 20px;
            color: white;
        }
        
        h1 {
            font-size: 3em;
            margin-bottom: 10px;
        }
        
        .tagline {
            font-size: 1.2em;
            opacity: 0.9;
            font-style: italic;
        }
        
        .content {
            background: white;
            border-radius: 20px;
            padding: 40px;
            margin: 40px 0;
            box-shadow: 0 10px 50px rgba(0,0,0,0.2);
        }
        
        .download-section {
            display: flex;
            justify-content: center;
            gap: 30px;
            flex-wrap: wrap;
            margin: 40px 0;
        }
        
        .download-card {
            background: #F3E9DC;
            border-radius: 15px;
            padding: 30px;
            text-align: center;
            min-width: 250px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            transition: transform 0.3s;
        }
        
        .download-card:hover {
            transform: translateY(-5px);
        }
        
        .platform-icon {
            font-size: 4em;
            margin-bottom: 15px;
        }
        
        .download-btn {
            display: inline-block;
            background: #4E342E;
            color: white;
            padding: 15px 30px;
            text-decoration: none;
            border-radius: 8px;
            font-size: 1.1em;
            font-weight: bold;
            transition: background 0.3s;
        }
        
        .download-btn:hover {
            background: #3E2723;
        }
        
        .features {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin: 40px 0;
        }
        
        .feature {
            background: #EFE6DC;
            padding: 20px;
            border-radius: 10px;
        }
        
        .feature h3 {
            color: #4E342E;
            margin-bottom: 10px;
        }
        
        .instructions {
            background: #FFF3E0;
            border-left: 4px solid #C69C6D;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }
        
        .instructions ol {
            margin-left: 20px;
        }
        
        .instructions li {
            margin: 10px 0;
        }
        
        footer {
            text-align: center;
            padding: 40px 20px;
            color: white;
        }
        
        .github-link {
            color: white;
            text-decoration: none;
            border-bottom: 2px solid white;
        }
        
        .github-link:hover {
            opacity: 0.8;
        }
        
        @media (max-width: 768px) {
            h1 { font-size: 2em; }
            .content { padding: 20px; }
        }
    </style>
</head>
<body>
    <header>
        <div class="container">
            <h1>☕ Coffee Shop ☕</h1>
            <p class="tagline">"It's not Java anymore!"</p>
            <p>Modern .NET MAUI Mobile Ordering App</p>
        </div>
    </header>
    
    <div class="container">
        <div class="content">
            <h2 style="text-align: center; color: #4E342E; margin-bottom: 30px;">
                Download Coffee Shop
            </h2>
            
            <div class="download-section">
                <div class="download-card">
                    <div class="platform-icon">📱</div>
                    <h3>Android</h3>
                    <p>Version 1.0</p>
                    <p style="margin: 15px 0;">Android 5.0+</p>
                    <a href="CoffeeShop.apk" class="download-btn" download>
                        Download APK
                    </a>
                    <p style="margin-top: 15px; font-size: 0.9em; color: #666;">
                        ~15 MB
                    </p>
                </div>
                
                <div class="download-card">
                    <div class="platform-icon">🍎</div>
                    <h3>iOS</h3>
                    <p>Coming Soon</p>
                    <p style="margin: 15px 0;">iOS 13.0+</p>
                    <a href="#" class="download-btn" style="background: #999; cursor: not-allowed;">
                        Not Available
                    </a>
                </div>
                
                <div class="download-card">
                    <div class="platform-icon">🪟</div>
                    <h3>Windows</h3>
                    <p>Coming Soon</p>
                    <p style="margin: 15px 0;">Windows 10+</p>
                    <a href="#" class="download-btn" style="background: #999; cursor: not-allowed;">
                        Not Available
                    </a>
                </div>
            </div>
            
            <h2 style="color: #4E342E; margin-top: 50px;">Features</h2>
            <div class="features">
                <div class="feature">
                    <h3>🔐 Secure Authentication</h3>
                    <p>Create your account with password-protected access</p>
                </div>
                <div class="feature">
                    <h3>📋 Menu Browsing</h3>
                    <p>Browse Hot Drinks, Cold Drinks, and Food categories</p>
                </div>
                <div class="feature">
                    <h3>🛒 Shopping Basket</h3>
                    <p>Easy item selection with quantity management</p>
                </div>
                <div class="feature">
                    <h3>📱 Quick Checkout</h3>
                    <p>Auto-filled details for fast ordering</p>
                </div>
                <div class="feature">
                    <h3>📜 Order History</h3>
                    <p>View all your past orders with details</p>
                </div>
                <div class="feature">
                    <h3>🎨 Beautiful Design</h3>
                    <p>Coffee-themed colors and responsive layout</p>
                </div>
            </div>
            
            <h2 style="color: #4E342E; margin-top: 50px;">Installation Instructions (Android)</h2>
            <div class="instructions">
                <ol>
                    <li><strong>Download</strong> the APK file using the button above</li>
                    <li><strong>Enable</strong> "Install unknown apps" in Settings → Security</li>
                    <li><strong>Open</strong> the downloaded APK file</li>
                    <li><strong>Tap Install</strong> and wait for completion</li>
                    <li><strong>Launch</strong> Coffee Shop from your app drawer</li>
                    <li><strong>Register</strong> your account to start ordering!</li>
                </ol>
            </div>
            
            <h3 style="color: #4E342E; margin-top: 30px;">Requirements</h3>
            <ul style="margin-left: 20px;">
                <li>Android 5.0 (Lollipop) or higher</li>
                <li>~20 MB free storage space</li>
                <li>Internet permission (for future updates)</li>
            </ul>
            
            <h3 style="color: #4E342E; margin-top: 30px;">Troubleshooting</h3>
            <p><strong>Installation blocked?</strong> Enable "Unknown sources" in Settings</p>
            <p><strong>App not installed?</strong> Ensure you have Android 5.0+ and sufficient storage</p>
            <p><strong>Need help?</strong> Check the <a href="https://github.com/MichaelMcKibbin/CoffeeShop/issues">GitHub Issues</a></p>
        </div>
    </div>
    
    <footer>
        <div class="container">
            <p style="font-size: 1.2em; margin-bottom: 10px;">
                <strong>Coffee Shop</strong> - Educational .NET MAUI Project
            </p>
            <p>
                Developed by Michael McKibbin (ATU Student# L00197067)
            </p>
            <p style="margin-top: 15px;">
                <a href="https://github.com/MichaelMcKibbin/CoffeeShop" class="github-link">
                    View Source Code on GitHub
                </a>
            </p>
            <p style="margin-top: 20px; opacity: 0.8;">
                Built with .NET 10 and .NET MAUI
            </p>
        </div>
    </footer>
</body>
</html>
```

#### Step 2: Add Privacy Policy Page

**Create `docs/privacy.html`:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Privacy Policy - Coffee Shop</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
            line-height: 1.6;
        }
        h1 { color: #4E342E; }
        h2 { color: #6D4C41; margin-top: 30px; }
        .last-updated { color: #666; font-style: italic; }
    </style>
</head>
<body>
    <h1>Privacy Policy for Coffee Shop</h1>
    <p class="last-updated">Last Updated: January 2025</p>
    
    <h2>Introduction</h2>
    <p>Coffee Shop is a mobile application for ordering coffee and food items. This privacy policy explains how we handle your data.</p>
    
    <h2>Data Collection</h2>
    <p>We collect and store the following data locally on your device:</p>
    <ul>
        <li>User account information (username, name, phone number)</li>
        <li>Order history and transaction records</li>
        <li>Menu preferences and shopping basket contents</li>
    </ul>
    
    <h2>Data Storage</h2>
    <p>All data is stored locally in an SQLite database on your device. No data is transmitted to external servers or third-party services.</p>
    
    <h2>Data Security</h2>
    <ul>
        <li>Passwords are hashed using SHA-256 before storage</li>
        <li>All data remains on your device</li>
        <li>No cloud synchronization</li>
        <li>No data sharing with third parties</li>
    </ul>
    
    <h2>User Rights</h2>
    <p>You have the right to:</p>
    <ul>
        <li>Delete your account data by uninstalling the app</li>
        <li>Access all your stored data within the app</li>
        <li>Know that no data is retained after app uninstall</li>
    </ul>
    
    <h2>Children's Privacy</h2>
    <p>This app is suitable for all ages and does not collect personal data from children.</p>
    
    <h2>Changes to Privacy Policy</h2>
    <p>Updates will be posted within the app and on our GitHub repository.</p>
    
    <h2>Contact</h2>
    <p>For privacy concerns, contact: <a href="mailto:michael.mckibbin@example.com">michael.mckibbin@example.com</a></p>
    <p>GitHub: <a href="https://github.com/MichaelMcKibbin/CoffeeShop">https://github.com/MichaelMcKibbin/CoffeeShop</a></p>
    
    <p style="margin-top: 40px; text-align: center;">
        <a href="index.html">← Back to Download Page</a>
    </p>
</body>
</html>
```

#### Step 3: Enable GitHub Pages

1. **Push to GitHub:**
   ```bash
   git add docs/
   git commit -m "Add download page and privacy policy"
   git push origin master
   ```

2. **Enable GitHub Pages:**
   - Go to repository Settings
   - Pages section
   - Source: Deploy from branch
   - Branch: `master`
   - Folder: `/docs`
   - Save

3. **Access your page:**
   ```
   https://michaelmckibbin.github.io/CoffeeShop/
   ```

---

## Pre-Release Checklist

### Code Preparation

- [ ] Update version numbers
  ```xml
  <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
  <ApplicationVersion>1</ApplicationVersion>
  ```

- [ ] Set package/bundle identifiers
  ```xml
  <ApplicationId>com.companyname.coffeeshop</ApplicationId>
  ```

- [ ] Remove debug code and logging
- [ ] Set `android:debuggable="false"`
- [ ] Test on physical devices (not just emulator)
- [ ] Verify all features work offline
- [ ] Test on minimum OS version

### Security

- [ ] Create and secure keystore file
- [ ] Set up environment variables for passwords
- [ ] Remove hardcoded passwords from `.csproj`
- [ ] Add `.keystore` to `.gitignore`
- [ ] Create backup of signing keys

### Assets

- [ ] App icon (multiple sizes)
  - Android: `mipmap-*dpi` folders
  - iOS: `Assets.xcassets`
  - Windows: Various sizes
  
- [ ] Splash screen
- [ ] Store screenshots (4-8 images)
- [ ] Feature graphic (Play Store)
- [ ] Store icon (512x512 for Play Store)

### Documentation

- [ ] Create privacy policy (required by stores)
- [ ] Write terms of service (if applicable)
- [ ] Prepare release notes
- [ ] Update README.md with download instructions
- [ ] Create installation guide
- [ ] Document troubleshooting steps

### Legal

- [ ] Verify app name availability
- [ ] Check for trademark conflicts
- [ ] Ensure compliance with store policies
- [ ] Prepare copyright notice
- [ ] Set up support email

### Testing

- [ ] Test registration flow
- [ ] Test login/logout
- [ ] Test all menu categories
- [ ] Test basket functionality
- [ ] Test checkout process
- [ ] Test order history
- [ ] Test on different screen sizes
- [ ] Test on different Android versions
- [ ] Verify database operations
- [ ] Test app permissions

---

## Troubleshooting

### Build Issues

#### "Could not find Android SDK"
**Solution:**
```bash
# Set ANDROID_HOME environment variable
set ANDROID_HOME=C:\Program Files\Android\android-sdk
```

#### "Keystore not found"
**Solution:**
- Ensure `coffeeshop.keystore` is in project root
- Or update path in `.csproj`:
  ```xml
  <AndroidSigningKeyStore>path\to\coffeeshop.keystore</AndroidSigningKeyStore>
  ```

#### "Failed to sign APK"
**Solution:**
- Verify password is correct
- Check keystore alias matches
- Ensure keystore file isn't corrupted

### Installation Issues

#### "App not installed" on Android
**Causes:**
- Insufficient storage
- Corrupted APK
- Conflicting package name
- Unsupported Android version

**Solutions:**
```bash
# Check device storage
adb shell df /data

# Uninstall previous version
adb uninstall com.companyname.coffeeshop

# Reinstall
adb install -r CoffeeShop.apk
```

#### "Installation blocked" on Android
**Solution:**
- Enable Unknown Sources for installer app
- Settings → Security → Install unknown apps → [Your Browser/File Manager] → Allow

### Runtime Issues

#### App crashes on launch
**Solutions:**
- Clear app data
- Check minimum SDK version
- Verify all dependencies are included
- Check logcat for exceptions:
  ```bash
  adb logcat | findstr "CoffeeShop"
  ```

#### Database errors
**Solutions:**
- Clear app data to reset database
- Verify SQLite permissions
- Check database path is valid

---

## Distribution Strategies Comparison

| Method | Cost | Time to Deploy | User Reach | Best For |
|--------|------|----------------|------------|----------|
| **Google Play Store** | $25 one-time | 1-7 days | Worldwide | Public release |
| **Direct APK** | Free | Immediate | Manual sharing | Testing, internal |
| **GitHub Releases** | Free | Immediate | Anyone with link | Open source |
| **TestFlight** | $99/year | 1-2 days | 10,000 testers | iOS beta |
| **App Store** | $99/year | 1-2 days | Worldwide | iOS public |
| **Microsoft Store** | $19 one-time | 1-3 days | Worldwide | Windows public |

---

## Recommended Path for Students

### Phase 1: Development & Testing
```
✅ Build and test locally
✅ Share APK with friends/classmates via Drive/Dropbox
✅ Gather feedback
```

### Phase 2: GitHub Distribution
```
✅ Create GitHub Release with APK
✅ Add installation instructions to README
✅ Share release link
✅ Track downloads and issues
```

### Phase 3: Portfolio/Professional (Optional)
```
⭐ Create GitHub Pages download site
⭐ Submit to Google Play Store ($25)
⭐ Add to resume/portfolio
⭐ Demonstrate to employers
```

---

## Quick Command Reference

### Build Commands

```bash
# Android APK (direct distribution)
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk

# Android AAB (Play Store)
dotnet publish -f net10.0-android -c Release

# iOS Archive (requires Mac)
dotnet build -f net10.0-ios -c Release /p:ArchiveOnBuild=true

# Windows MSIX
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

### Keystore Commands

```bash
# Create keystore
keytool -genkey -v -keystore coffeeshop.keystore -alias coffeeshop -keyalg RSA -keysize 2048 -validity 10000

# View keystore info
keytool -list -v -keystore coffeeshop.keystore

# Verify APK signature
jarsigner -verify -verbose -certs CoffeeShop.apk
```

### Git Commands

```bash
# Create release
git tag v1.0.0
git push origin v1.0.0

# Update release
git tag -d v1.0.0
git push origin :refs/tags/v1.0.0
git tag v1.0.0
git push origin v1.0.0
```

---

## Additional Resources

### Official Documentation

- **.NET MAUI Deployment**  
  https://docs.microsoft.com/en-us/dotnet/maui/deployment/

- **Google Play Console**  
  https://support.google.com/googleplay/android-developer

- **Apple App Store Connect**  
  https://developer.apple.com/app-store-connect/

- **Microsoft Partner Center**  
  https://partner.microsoft.com/dashboard

### Tools

- **Android Studio** (for SDK management)  
  https://developer.android.com/studio

- **Xcode** (for iOS builds)  
  https://developer.apple.com/xcode/

### Community

- **.NET MAUI GitHub**  
  https://github.com/dotnet/maui

- **.NET MAUI Community Toolkit**  
  https://github.com/CommunityToolkit/Maui

---

## Document Information

**Created:** January 2025  
**Project:** Coffee Shop - .NET MAUI Application  
**Developer:** Michael McKibbin (ATU Student# L00197067)  
**Version:** 1.0  
**Related Files:** `README.md`, `SECURITY-HASHING.md`

---

*This document is part of the Coffee Shop application's technical documentation. For project overview, see [README.md](README.md). For security details, see [SECURITY-HASHING.md](SECURITY-HASHING.md).*

---

## Summary

**Easiest Path for Your Project:**

1. **Build signed APK** with the keystore
2. **Create GitHub Release** and upload APK
3. **Update README** with download link and instructions
4. **Share with users/classmates**

This avoids store fees and approval delays while still providing a professional distribution method! 🎉📱☕
