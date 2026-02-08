# SelectAid

SelectAid is a Windows 10/11 WPF (.NET 8) AAC + PC operation support application with shared input routing, scan support, overlay control, and supporter-managed settings.

## Build

```bash
dotnet build SelectAid.sln -c Release
```

## Run

```bash
dotnet run --project SelectAid/SelectAid.csproj
```

##操作ガイド（利用者）

- **Home** から各機能へ移動します。
- **AAC**
  - 文字盤をタップして入力します。
  - **Speak** でSAPI発話し、必要なら自動クリアします。
  - **Buzzer** は即時意思表示（音＋フラッシュ）です。
- **Phrases**
  - シーン/カテゴリから定型文を選択し **Speak** または **Insert** を使います。
- **PC(Overlay)**
  - Overlay ボタンで最前面の操作パネルを表示し、クリック/スクロール/Tab/Back を送出します。
  - MouseGrid から分割→拡大→クリックで外部アプリを操作します。

## 支援者設定

- **Supporter** で入力モード、テーマ、ハイコントラスト、自動起動を設定します。
- **Settings** で dwell / scan / confirm guard / undo window / mouse grid 分割数を調整します。
- 電源操作は **Allow Power Controls** を有効にした上で二重確認で実行されます。

## バックアップ/復元

- **Backup/Restore** でワンタップバックアップ（zip）を作成します。
- **Restore** は二重確認で最新バックアップを復元し、復元前に自動退避バックアップを作成します。

## 復旧手順

1. アプリ起動後、Home → Supporter で入力モードやテーマを確認します。
2. 設定ファイル破損時は `%AppData%\SelectAid\backups` の最新バックアップを **Restore** で戻します。
3. どうしても復元できない場合は `%AppData%\SelectAid` を退避し、再起動で初期設定を再生成します。

## データ保存先

`%AppData%\SelectAid`

- settings.json
- profiles.json
- keyboardLayouts.json
- phrases.json
- userDict.json
- history.json
- backups\
- log.txt
- metrics.csv
