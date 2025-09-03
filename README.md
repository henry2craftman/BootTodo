# BootTodo

**A simple yet powerful C# console diary that automatically syncs your daily to-do list to a Google Doc upon startup.**

BootTodo is designed for those who want to start their day with focus. As soon as you turn on your computer, this app can launch, prompt you for your daily plan, and save it both locally and to a centralized Google Doc. It's the perfect, frictionless way to build a daily logging habit.

![A GIF showing the BootTodo main menu, writing a diary entry with hotkeys, and the result appearing in a Google Doc.](https://your-image-host.com/BootTodo-demo.gif)
*(Recommendation: Record a short GIF of the application in action and replace the link above. This dramatically increases user engagement.)*

## ✨ Features

-   **Local First:** All entries are saved locally to a `Diary.txt` file, ensuring you always have a reliable backup.
-   **Seamless Google Docs Sync:** Automatically appends your daily entries to a single, specified Google Doc.
-   **Lightweight & Fast:** Runs in any terminal with no heavy dependencies.
-   **Hotkey Support:** Use `F1` for `[o]` (Done) and `F2` for `[x]` (Not Done) to quickly format your to-do lists.
-   **Simple Setup:** No complex API libraries or OAuth consent screens in the C# code. Just a simple web request.

## 📋 Prerequisites

-   [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or later.
-   A [Google Account](https://www.google.com/).

## 🚀 Setup & Configuration

This setup involves creating a simple "middleman" using Google Apps Script. This script will receive data from the C# app and add it to your Google Doc.

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/BootTodo.git
cd BootTodo
```

### 2. Configure Google Docs & Apps Script

#### A. Create a Google Doc and get the Document ID

1.  Go to [Google Docs](https://docs.google.com/) and create a new, blank document. Name it something like "My Daily Log".
2.  Look at the URL in your browser. It will look like this:
    `https://docs.google.com/document/d/`**`1aBcDeFgHiJkLmNoPqRsTuVwXyZ_12345AbCdEfGhIjK`**`/edit`
3.  The **Document ID** is the long string between `/d/` and `/edit`. Copy this ID. You'll need it soon.

#### B. Create the Google Apps Script

1.  Go to [Google Apps Script](https://script.google.com/) and click **"+ New project"**.
2.  Give the project a name, like "BootTodo Syncer".
3.  Delete all the default code in the editor and **paste the entire script below**:

    ```javascript
    // ★★★ PASTE YOUR DOCUMENT ID HERE ★★★
    const DOC_ID = "PASTE_THE_DOCUMENT_ID_YOU_COPIED_IN_STEP_A";

    // This function will be called when our C# app sends a request.
    function doPost(e) {
      try {
        const textToAppend = e.parameter.text;
        
        if (!textToAppend) {
          return ContentService.createTextOutput("Error: 'text' parameter is missing.");
        }
        
        const doc = DocumentApp.openById(DOC_ID);
        const body = doc.getBody();
        
        // Append the text from the C# app as a new paragraph.
        body.appendParagraph(textToAppend);
        
        // Return a success message back to the C# app.
        return ContentService.createTextOutput("Success");
        
      } catch (error) {
        return ContentService.createTextOutput("Error: " + error.toString());
      }
    }
    ```
4.  Replace `"PASTE_THE_DOCUMENT_ID_YOU_COPIED_IN_STEP_A"` with the actual Document ID from the previous step.
5.  Click the **Save project** (💾) icon.

#### C. Deploy the Script as a Web App to get your URL

1.  In the Apps Script editor, click the blue **"Deploy"** button in the top-right corner and select **"New deployment"**.
2.  Click the **gear icon** (⚙️) next to "Select type" and choose **"Web app"**.
3.  Configure the settings as follows:
    -   **Description:** `BootTodo Syncer` (optional)
    -   **Execute as:** **Me** (your Google account)
    -   **Who has access:** **Anyone**
4.  Click **"Deploy"**.
5.  A window will pop up asking for authorization. Click **"Authorize access"**.
6.  Choose your Google account. You will see a "Google hasn’t verified this app" warning. This is normal for personal scripts.
7.  Click **"Advanced"**, then click **"Go to [Your Project Name] (unsafe)"**.
8.  Click **"Allow"** on the final screen to grant the script permission to edit your Google Docs.
9.  Finally, a **Web app URL** will be displayed. **Copy this URL.** This is the secret link your C# application will use.

### 3. Configure the C# Application

1.  Open the cloned project in your code editor.
2.  Navigate to the `GoogleDocsManager.cs` file.
3.  Find the following line and paste the **Web app URL** you just copied:

    ```csharp
    private static readonly string AppScriptUrl = "PASTE_YOUR_WEB_APP_URL_HERE";
    ```

---

## 🚀 Getting Started: Building and Running the App

### Step 1: Publish the Application

First, create a standalone, optimized version of the app. This packages everything into a single folder that can be run anywhere.

Open your terminal in the project's root directory and run:

```bash
# This creates a 'publish' folder with the executable and all dependencies.
dotnet publish -c Release --self-contained
```

After the command finishes, you will find a `publish` folder inside `bin/Release/netX.X/`.

### Step 2: Run the Published Application

**Important:** After publishing, you must run the executable file directly. Do not use `dotnet run`.

1.  Navigate into the `publish` folder using your terminal:
    ```bash
    # Example path - adjust to your actual .NET version
    cd bin/Release/net8.0/win-x64/publish
    ```
2.  Run the executable:
    -   **On Windows:** `BootTodo.exe`
    -   **On macOS/Linux:** `./BootTodo` (you may need to run `chmod +x BootTodo` first to make it executable)

---

## ⌨️ How to Use

### Application Features

-   **Main Menu:**
    -   **1. Write Diary:** Start writing a new entry for the current day.
    -   **2. Search Diary:** Search for a past entry in your local `Diary.txt` file.
    -   **3. Exit:** Close the application.
-   **Writing an Entry:**
    -   Type your text and press `Enter` for a new line.
    -   **Hotkeys:** Press `F1` for `[o]` and `F2` for `[x]`.
    -   Type `quit` and press `Enter` to save and sync.

### Making the App Start Automatically (Recommended)

To get the full experience, set the app to launch automatically when your computer starts.

#### For Windows 

1.  Press `Win + R` to open the **Run** dialog.
2.  Type `shell:startup` and press **Enter** to open the Startup folder.
3.  In another window, open the `publish` folder you created earlier.
4.  Right-click on **`BootTodo.exe`** and select **"Create shortcut"**.
5.  Drag this new shortcut into the **Startup folder**.

#### For macOS

1.  Open **System Settings** > **General** > **Login Items**.
2.  Click the plus (`+`) button.
3.  Navigate to the `publish` folder and select the **`BootTodo`** executable file.
4.  Click **"Add"**.

---

## 👨‍💻 For Developers (Optional)

If you have cloned the source code and want to run it directly for development or testing purposes, you can use `dotnet run` from the project's root directory (the one containing the `.csproj` file).

```bash
# Only for development, from the project root folder
dotnet run
```

## 🤝 Contributing

Contributions are welcome! If you have ideas for new features or improvements, feel free to fork the repository, make your changes, and submit a pull request.

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## 📜 License

This project is licensed under the MIT License. See the `LICENSE` file for details.
