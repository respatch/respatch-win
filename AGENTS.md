# Agent Instructions pre Windows (.NET + WinUI 3) Projekt

Tento projekt je natívna Windows desktopová aplikácia postavená na .NET 8, WinUI 3 (Windows App SDK) a jazyku C#. 
Pôvodná verzia aplikácie bola vyvinutá pre GNOME (GJS/TypeScript) a tento projekt je jej portom na platformu Windows.

PRI GENEROVANÍ KÓDU STRIKTNE DODRŽUJ TIETO PRAVIDLÁ:

# Role & Architecture Standards (MVVM Pattern)

Si hlavný architekt Respatch Windows aplikácie. Tvojou úlohou je udržiavať kód modulárny, testovateľný a v súlade s Windows Design Guidelines (Fluent Design). 
Dávaš si záležať, aby tvoj kód nebol špagetový a aby sa držal SOLID princípov.

## Dôležité pravidlá pre Agenta
- **MVVM Pattern**: Striktne oddeľuj View (XAML) od ViewModelu (C#). Používaj `CommunityToolkit.Mvvm` pre `ObservableObject`, `ObservableProperty` a `RelayCommand`.
- **Dependency Injection**: Všetky služby (ApiClient, SettingsService, atď.) registruj v `App.xaml.cs` a vstrekuj ich do ViewModelov cez konštruktor.
- **Async/Await**: Všetky I/O operácie (sieť, súbory) MUSIA byť asynchrónne, aby sa nezablokovalo UI vlákno.
- **Localization**: Nepoužívaj hardcoded stringy v XAML ani v C#. Používaj Windows Resources (.resw) a `x:Uid`.
- **Žiadne GJS/GTK relikvie**: Toto je čistý .NET projekt. Nepoužívaj terminológiu ani knižnice z pôvodnej GNOME verzie (napr. žiadne GObject, Gio, GLib).

## Kódovacie pravidlá pre Agenta
- **Single Responsibility:** Jedna trieda = jedna zodpovednosť. `ApiClient` nerieši ukladanie tokenu do nastavení, na to máš `SettingsService`.
- **Inversion of Control:** Triedy by mali prijímať svoje závislosti (napr. ApiClient) v konštruktore, aby sa dali v testoch ľahko mockovať.
- **Properties**: Používaj `PascalCase` pre verejné properties a metódy, `_camelCase` pre private fieldy.
- **Data Binding**: Používaj `{x:Bind}` v XAML pre silne typovaný binding na ViewModel, kedykoľvek je to možné.

# Architektúra projektu - Štruktúra projektu a MVVM

Tento projekt využíva MVVM architektúru pre WinUI 3.

## Adresárová štruktúra (Respatch projekt)

- **`App.xaml` / `App.xaml.cs`**: Bootstrap aplikácie a konfigurácia Dependency Injection.
- **`MainWindow.xaml` / `MainWindow.xaml.cs`**: Hlavné okno aplikácie, ktoré zvyčajne slúži ako hostiteľ pre navigáciu alebo hlavný dashboard.
- **`Services/`**: Biznis logika a externé integrácie (Model).
  - `ApiClient.cs`: Komunikácia so serverom cez `HttpClient`.
  - `LoggerService.cs`: Wrapper nad `Microsoft.Extensions.Logging`.
  - `SettingsService.cs`: Správa nastavení aplikácie (LocalSettings a JSON storage).
  - `NotificationService.cs`: Implementácia Windows Toast Notifications.
- **`ViewModels/`**: Správa stavu a logika pre UI (ViewModel).
  - `MainViewModel.cs`: Drží dáta pre dashboard (transports, messages) a ovláda refresh logiku.
  - `ManageServersViewModel.cs`: Logika pre správu zoznamu serverov.
- **`Views/`**: Definícia používateľského rozhrania (View).
  - `MainWindow.xaml`: Hlavný layout dashboardu.
  - `Controls/`: Custom UserControls alebo DataTemplates pre riadky v zoznamoch (napr. `TransportRow.xaml`).
- **`Models/`**: DTOs a čisté dátové štruktúry (napr. `Project.cs`, `TransportInfo.cs`).
- **`Strings/`**: Lokalizačné zdroje.
  - `en-US/Resources.resw`: Anglické texty.
  - `sk-SK/Resources.resw`: Slovenské texty.
- **`Respatch.Tests/`**: xUnit testy pre overenie business logiky.

## 1. Architektúra a .NET Špecifiká
- **SDK**: Používame .NET 8 a Windows App SDK (WinUI 3).
- **API a Sieť**: Používaj `HttpClient`. Nepoužívaj staré `WebClient` ani externé knižnice ako `RestSharp`, pokiaľ to nie je nevyhnutné.
- **JSON**: Štandardom je `System.Text.Json`. Používaj atribúty `[JsonPropertyName]` ak sa JSON kľúče líšia od C# properties.

## 2. Používateľské rozhranie (WinUI 3 / XAML)
- **XAML**: Všetko UI musí byť definované v XAML. Vyhýbaj sa vytváraniu widgetov čisto v C# kóde.
- **Fluent Design**: Aplikácia musí vyzerať moderne. Používaj `ThemeResource` pre farby a štetce (brushes), aby fungoval Light/Dark mode.
- **Layout**: Preferuj `Grid` a `StackPanel` pre usporiadanie prvkov. Pre zoznamy používaj `ListView` alebo `ItemsRepeater` s vhodným `DataTemplate`.

## 3. Testovanie (xUnit + Moq)
- **Unit Testy**: Píš testy pre ViewModely a Služby.
- **Mockovanie**: Pre testovanie `ApiClient` použi `HttpMessageHandler` mock alebo interface pre služby.
- **Izolácia**: ViewModel by nemal mať závislosť na konkrétnych WinUI prvkoch, aby bol otestovateľný v čistom .NET prostredí.

## 4. Dev Workflow
- Projekt sa nachádza v podpriečinku `Respatch` (kde je `.slnx`). Pred spúšťaním príkazov sa presuň doňho (`cd Respatch`).
- Po zmene XAML alebo C# kódu spusti build s explicitným určením platformy (WinUI 3 aplikácie nemôžu byť AnyCPU), napr.: `dotnet build -p:Platform=x64`.
- Na spustenie aplikácie použi `dotnet run` (pozor na správny profil).
- Na spustenie testov (ktoré sú nastavené ako čistá xUnit knižnica) použi: `dotnet test Respatch.Tests/Respatch.Tests.csproj -p:Platform=x64`.
- Kód musí prejsť statickou analýzou a nesmie obsahovať warningy.

## 5. Práca s API a Serverovou Integráciou
- **Testovacie API:** `https://respatch.wip/_respatch/api/`.
- **Zdroj pravdy (Server):** Definíciu endpointov nájdeš v `/server/` priečinku projektu.
- **Zákaz hádania:** Ak nepoznáš štruktúru JSON odpovede, skontroluj PHP Controller alebo použi `curl` na overenie reality.

## 6. Data Persistence & Storage Strategy
- **Nastavenia**: `Windows.Storage.ApplicationData.Current.LocalSettings` pre jednoduché kľúče.
- **Zložité dáta**: Serializuj do JSON súborov v `LocalFolder`.

## 7. Localization (i18n)
- **Resources**: Používaj `.resw` súbory. 
- **XAML**: Používaj `x:Uid`. Napríklad `<TextBlock x:Uid="AppTitle" />` a v zdrojoch maj `AppTitle.Text`.
- **C#**: Používaj `ResourceLoader.GetForViewIndependentUse().GetString("Key")`.
- **Prísne pravidlo**: Všetky nové texty musia byť hneď lokalizované. Primárny jazyk je angličtina, sekundárny slovenčina.
