# Agent Instructions pre Windows / WinUI 3 (C# + XAML) Projekt

Tento projekt je natívna Windows 11 desktopová aplikácia postavená na .NET 9, Windows App SDK a WinUI 3.
Všetko UI je písané v XAML s dodržaním Fluent Design System princípov. Architektúra je MVVM s využitím CommunityToolkit.Mvvm.

PRI GENEROVANÍ KÓDU STRIKTNE DODRŽUJ TIETO PRAVIDLÁ:

# Role & Architecture Standards (MVVM Pattern)

Si hlavný architekt Respatch Windows aplikácie. Tvojou úlohou je udržiavať kód modulárny, testovateľný a v súlade s
Windows 11 Human Interface Guidelines a Fluent Design System.

## Dôležité pravidlá pre Agenta
- **MVVM záväzok**: ViewModely MUSIA dediť od `ObservableObject` (CommunityToolkit.Mvvm). Žiadna biznis logika v code-behind (`.xaml.cs`).
- **Žiadny WPF kód**: Toto NIE JE WPF. Nepoužívaj `System.Windows.*`. Správny namespace je `Microsoft.UI.Xaml.*`.
- **Asynchrónnosť**: Všetky sieťové a I/O operácie MUSIA byť `async/await`. Nikdy nevolaj `.Result` ani `.Wait()` na `Task`.
- **Testovanie**: Každá nová logika v `Services/` alebo `Stores/` musí mať zodpovedajúci xUnit test v `Respatch.Tests/`.
- **Build proces**: Po každej zmene v XAML spusti build (`dotnet build`), aby si overil, že sa XAML kompiluje bez chýb.

## Kódovacie pravidlá pre Agenta
- **Single Responsibility:** Jedna trieda = jedna zodpovednosť. `ApiClient` nerieši ukladanie tokenov, na to máš `SettingsService`.
- **Inversion of Control:** Triedy prijímajú závislosti cez konštruktor (rozhrania `IApiClient`, `ISettingsService`, atď.). Toto umožňuje ľahké mocknutie v testoch.
- **Property Changed Notifications:** Využívaj `[ObservableProperty]` atribút a `[RelayCommand]` zo CommunityToolkit.Mvvm namiesto manuálneho `INotifyPropertyChanged`.

# Architektúra projektu - Štruktúra projektu a MVVM

Tento projekt využíva dekomponovanú architektúru MVVM pre WinUI 3.

## Adresárová štruktúra

- **`App.xaml.cs`**: Bootstrap aplikácie. Tu sa inicializuje DI kontajner (Microsoft.Extensions.DependencyInjection), registrujú sa všetky služby a spustí sa prvé okno.
- **`Navigation/NavigationService.cs`**: Centrálny bod pre navigáciu. Zodpovedá za prepínanie medzi `WelcomePage`, `MainPage` a otváranie `SettingsPage` / `AddProjectDialog`.
- **`Services/`**: Čistá biznis logika (Model). Nesmú obsahovať WinUI widgety ani `Microsoft.UI.Xaml` referencie.
  - `ApiClient.cs`: Komunikácia so serverom cez `HttpClient`. Posiela `X-Respatch-Token` hlavičku.
  - `LoggerService.cs`: Logovanie s podporou konzoly (Debug Output) a voliteľného súboru.
  - `SettingsService.cs`: Obal nad `ApplicationData.Current.LocalSettings`.
- **`Stores/`**: Správa stavu aplikácie (ViewModel na úrovni aplikácie).
  - `ProjectStore.cs`: `ObservableObject`, drží zoznam projektov a aktívny projekt. Emituje `OnPropertyChanged` pri zmene.
- **`ViewModels/`**: ViewModely pre jednotlivé stránky a dialógy. Dedia od `ObservableObject`.
- **`Views/`**: XAML stránky a dialógy. Code-behind obsahuje iba inicializáciu a volanie `ViewModel`.
  - `Views/Dialogs/`: Modálne `ContentDialog` dialógy.
- **`Models/`**: C# records a čisté dátové štruktúry (napr. `Project.cs`).
- **`Strings/`**: `.resw` súbory pre lokalizáciu (en-US, sk-SK).
- **`Respatch.Tests/`**: xUnit testy, WireMock.Net mocky pre API.

## Ako sa štruktúra používa

1. **Pridanie novej stránky**:
   - Vytvor `Views/MojaStranka.xaml` a `Views/MojaStranka.xaml.cs`.
   - Vytvor `ViewModels/MojaStrankaViewModel.cs` dediaci od `ObservableObject`.
   - Zaregistruj ViewModel v DI kontajneri v `App.xaml.cs`.
   - Pridaj navigačnú metódu do `NavigationService.cs`.
2. **Práca s dátami a stavom**:
   - Dáta sa sťahujú cez `IApiClient`.
   - Perzistencia sa rieši cez `ISettingsService` (LocalSettings).
   - Aktuálny stav aplikácie (napr. vybraný projekt) spravuje `ProjectStore`.
3. **Prepojenie UI so stavom**:
   - V XAML binduj na properties ViewModelu: `{x:Bind ViewModel.PropertyName, Mode=OneWay}`.
   - Preferuj `x:Bind` (kompilované) pred `{Binding}` (reflexia).
   - Pre príkazy používaj `{x:Bind ViewModel.MojPrikaz}`.

## 1. Architektúra a WinUI 3 Špecifiká (VEĽMI DÔLEŽITÉ)
- **Toto NIE JE WPF ani UWP**: API je WinUI 3 (Windows App SDK). Správne namespacy: `Microsoft.UI.Xaml`, `Microsoft.UI.Xaml.Controls`, `Microsoft.UI.Xaml.Media`.
- **UI vlákno**: Ak aktualizuješ UI z `async` metódy, uisti sa, že si na UI vlákne. Použi `DispatcherQueue.TryEnqueue()`.
- **Mica a Acrylic**: Na dosiahnutie Windows 11 vzhľadu nastav `SystemBackdrop` na `MicaBackdrop` v konštruktore okna. Pri `WindowsSystemDispatcherQueueHelper` postupuj podľa WinUI 3 Gallery vzoru.
- **Okno**: Hlavné okno je `MainWindow.xaml`. Použi `AppWindow.TitleBar` na customizáciu titulkovej lišty.

## 2. Používateľské rozhranie (WinUI 3 & Fluent Design)
- **Jazyk**: Všetky UI súbory sú v XAML (`.xaml`). Nikdy nepíš UI priamo v C# code-behind.
- **Moderné Komponenty**: Vždy preferuj natívne WinUI 3 widgety.
  - Namiesto zoznamov s riadkami používaj `ListView` alebo `ItemsRepeater` s `DataTemplate`.
  - Na navigáciu používaj `NavigationView` s `PaneDisplayMode="Left"`.
  - Na zobrazenie nastavení/skupín použi `SettingsCard` a `SettingsExpander` z CommunityToolkit.WinUI.UI.Controls.
  - Chybové hlásenia zobraz cez `InfoBar` (ekvivalent Adw.ToastOverlay).
  - Modálne dialógy implementuj ako `ContentDialog`.
  - Na vycentrovanie obsahu použi `ScrollViewer` + `StackPanel` s `MaxWidth`.
- **Fluent Design**: Používaj zaoblené rohy (`CornerRadius`), tieňe a Mica/Acrylic pozadie.
- **Tlačidlá**: Štýluj cez vstavaný `Style` - napr. `Style="{StaticResource AccentButtonStyle}"` (ekvivalent `suggested-action`).

## 3. Testovanie (xUnit + Moq + WireMock.Net)
- **Beh testov**: Testy sa spúšťajú cez `dotnet test`. Projekt testov je v `Respatch.Tests/`.
- **Mockovanie API**: Pre testovanie sieťových požiadaviek používame WireMock.Net. Nikdy neprepísuj `HttpClient` priamo. Namiesto toho injektuj mock server URL do `ApiClient` konštruktora.
- **Izolácia logiky**: UI kód (XAML, code-behind) a aplikačná logika (ApiClient, spracovanie dát) musia byť oddelené. Logika musí byť testovateľná bez spusteného WinUI okna. Ak dopĺňaš funkcionalitu do `Services/ApiClient.cs`, VŽDY pre ňu napíš test.
- **Spustenie aplikácie**: Pre otestovanie spusti projekt pomocou `dotnet run --project Respatch/Respatch.csproj`.

## 4. Dev Workflow
- Pri každej zmene v C# alebo XAML najprv spusti build: `dotnet build`.
- Kód musí dodržiavať C# nullable reference types (`<Nullable>enable</Nullable>`). Vyhýbaj sa `!` null-forgiving operátoru bez zdôvodnenia.
- Dodržiavaj C# 12 syntax (primary constructors, collection expressions).

## 5. Práca s API a Serverovou Integráciou
- **Autentifikácia**: Každý HTTP request MUSÍ obsahovať hlavičku `X-Respatch-Token: <token>`.
- **Base URL**: Základná URL projektu sa načítava z `ProjectStore.ActiveProject.Url`.
- **Zdroj pravdy (Server)**: Definíciu endpointov nájdeš v:
  - Cesty: `respatch/server/config/routes.php`
  - Logika: `respatch/server/src/Controller/ApiController.php`
- **Overovanie štruktúry dát**: Predtým, než implementuješ deserializáciu JSON odpovede, MUSÍŠ overiť reálny formát:
  1. Zavolaj endpoint pomocou `Invoke-WebRequest` alebo `curl` s príslušným tokenom.
  2. Ak nemáš prístup k bežiacemu serveru, **ZASTAV SA** a požiadaj používateľa o ukážku JSON odpovede.
- **Zákaz hádania**: Nikdy nepredpokladaj štruktúru JSON odpovede bez overenia.

## 6. Data Persistence & Storage Strategy
Pri ukladaní dát rozlišuj tieto tri úrovne:

1. **Memory Cache (Runtime)**:
   - Dáta, ktoré zmiznú po zatvorení appky (zoznam logov správ v pamäti).
   - Implementácia: `ObservableCollection<T>` v príslušnom ViewModeli.

2. **LocalSettings (Konfigurácia)**:
   - Dáta, ktoré prežijú restart (nastavenia, URL k serveru, API Token, zoznam projektov).
   - Implementácia: `ApplicationData.Current.LocalSettings` cez `SettingsService`. Kľúče musia byť konštanty definované v `SettingsService.cs`.

3. **LocalFolder (Ostatné)**:
   - Pre dáta, ktoré sú príliš veľké pre LocalSettings (napr. história správ, logy).
   - Implementácia: `ApplicationData.Current.LocalFolder` alebo `%LOCALAPPDATA%\Respatch\`. Ukladaj ako JSON súbory.

## 7. Localization & Internationalization (i18n)

Aplikácia musí byť od začiatku pripravená na preklad do viacerých jazykov (Slovenčina, Angličtina).

- **Resw súbory**: Na preklady používaj `.resw` Resource súbory v `Strings/en-US/Resources.resw` a `Strings/sk-SK/Resources.resw`.
- **Použitie v XAML**: Pristupuj k reťazcom cez `x:Uid` atribút na widgete, napr. `<Button x:Uid="AddProjectButton" />`.
- **Použitie v C#**: Načítavaj reťazce cez `ResourceLoader.GetForCurrentView().GetString("NejakýKľúč")`.
- **Pluralizácia**: Na prekladanie reťazcov s počtom použi `ResourceLoader` s viacerými variantami kľúčov (napr. `Workers_One`, `Workers_Many`).
