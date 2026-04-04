# Battle.Net Installer

[English version](README.md)

Простая утилита для установки, обновления или восстановления игр Blizzard через локально установленный Battle.net Agent.

Этот репозиторий является неофициальным поддерживаемым форком [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer) и содержит поддерживаемый фикс для `Error 2310`.

## Скачать

Скачайте последний `BNetInstaller.exe` на странице [Releases](https://github.com/DokPlay/Battle.Net-Installer/releases/latest).

## Быстрый старт

1. Установите Battle.net.
2. Один раз войдите в свой аккаунт Battle.net.
3. Скачайте `BNetInstaller.exe`.
4. Дважды нажмите на `BNetInstaller.exe`.
5. Введите значения, которые попросит программа.
6. Дождитесь появления прогресса загрузки или установки.

## Что будет, если просто дважды нажать EXE

Программа откроет окно консоли и спросит:

1. `TACT Product`
2. `Agent UID`
3. `Installation Directory`
4. `Game/Asset Language`
5. `Repair Install? (Y/N)`

Пример:

```text
Please complete the following information:
TACT Product (example: s2): fenris
Agent UID (example: s2_enUS, blank = same as product): fenris
Installation Directory (example: D:\Battle.net\StarCraft II): D:\Diablo IV
Game/Asset Language (example: enUS): ruRU
Repair Install? (Y/N, default N): n
```

Пояснение:

- В этом примере использовалась папка `D:\Diablo IV`.
- Вы можете указать другой диск, например `C:\Diablo IV`, `E:\Games\Diablo IV` или любой другой путь.
- Название папки тоже можно поменять на любое своё.
- Если `Agent UID` совпадает с product, это поле можно оставить пустым и нажать `Enter`.
- Папка для игры выбирается здесь, в консоли, а не в лаунчере Battle.net.
- В поле `Game/Asset Language` нужно вводить код языка, например `ruRU`, `enUS`, `deDE`, `frFR`, `esES`, `ptBR`, `itIT`, `koKR`, `plPL`, `zhCN` или `zhTW`.

## Пример запуска через командную строку

Если хотите запускать вручную через терминал:

```bat
BNetInstaller.exe --prod fenris --uid fenris --lang ruRU --dir "D:\Diablo IV"
```

Вы можете изменить:

- `fenris` на другой product
- `ruRU` на другой язык
- `D:\Diablo IV` на любой диск и любую папку

## Error 2310

Если появляется `Error 2310`:

1. Убедитесь, что вы вошли в Battle.net.
2. Снова запустите утилиту с той же папкой.
3. Если ошибка остаётся, откройте Battle.net и попробуйте `Locate the game` для этой же папки.
4. После этого снова запустите `BNetInstaller.exe`.

## Требования

- Windows
- установленный Battle.net
- выполненный вход в Battle.net хотя бы один раз

Если вы используете готовый релизный `EXE`, отдельно ставить .NET runtime не нужно, потому что релизная сборка self-contained.

## Сборка из исходников

Если хотите собрать программу самостоятельно:

```powershell
git clone https://github.com/DokPlay/Battle.Net-Installer.git
cd Battle.Net-Installer
dotnet publish BNetInstaller\BNetInstaller.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Итоговый файл:

```text
BNetInstaller\bin\Release\net8.0\win-x64\publish\BNetInstaller.exe
```

## Примечание

Используйте утилиту только для продуктов, которые уже доступны вашему аккаунту Blizzard.

## Благодарность

Исходный проект: [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer)
