# Step 1: Localization

### Current Implementation (GNOME)
The GNOME version uses `gettext` for internationalization.
-   **Service**: `app/src/gettext.ts` initializes the text domain and provides the `_` (gettext) and `ngettext` functions.
-   **Usage**: Strings are wrapped in `_('string')` in TypeScript and handled via translation markers in Blueprint/UI files.
-   **Storage**: Translation files are stored in `.po` files and compiled to `.mo` files.

### Windows Port Strategy
The Windows version will use the native **Windows Resources (.resw)** system.

-   **Resource Files**: Create `Strings/en-US/Resources.resw` (and other languages as needed).
-   **XAML Localization**: Use the `x:Uid` attribute in XAML to automatically map UI properties to resource strings.
    -   Example: `<TextBlock x:Uid="WelcomeMessage" />` will look for `WelcomeMessage.Text` in the resource file.
-   **Code-behind Localization**: Use the `ResourceLoader` class to retrieve strings programmatically.
    -   Example: `var loader = new ResourceLoader(); string msg = loader.GetString("Error_FailedFetch");`

### Migration Tasks
1.  **Extract Strings**: Scan the GNOME source code for `_()` and `ngettext()` calls.
2.  **Create .resw**: Map the extracted strings to key-value pairs in the `Resources.resw` file.
    -   Key: Use a descriptive name (e.g., `MainWindow_Title`).
    -   Value: The original English string.
3.  **Implement Helper**: (Optional) Create a simple helper class or extension method in C# to mimic the `_()` function for easier transition of code-based strings.
