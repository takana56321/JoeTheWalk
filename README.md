# Joe_The_Walk

## 概要
このプロジェクトは、Unity 2021.3.24f1を使用して開発された2Dプラットフォーマーゲームです。プレイヤーは羊のキャラクターを操作し、障害物や敵を避けながらゴールを目指します。

## 環境構築手順

### 前提条件
- Unity Hubのインストール
- Unity 2021.3.24f1のインストール
- Gitのインストール（オプション）

### 手順

1. **Unity Hubのインストール**
   - [Unity Hubのダウンロードページ](https://unity3d.com/get-unity/download)からUnity Hubをダウンロードしてインストールします。

2. **Unity 2021.3.24f1のインストール**
   - Unity Hubを開き、`インストール`タブをクリックします。
   - `新規インストール`をクリックし、バージョンリストから`2021.3.24f1`を選択してインストールします。
   - 必要なモジュール（例：Android Build SupportやiOS Build Supportなど）を選択し、`次へ`をクリックしてインストールを完了させます。

3. **プロジェクトのダウンロード**
   - プロジェクト全体のZIPファイルは以下のリンクからダウンロードできます：
     [https://d.kuku.lu/z3j3vuhda](https://d.kuku.lu/z3j3vuhda)
   - ダウンロードしたZIPファイルを解凍します。

4. **プロジェクトのクローン（オプション）**
   - コードのみを見たい場合はGitHubリポジトリからクローンします。ターミナルまたはコマンドプロンプトを開き、以下のコマンドを入力します：
     ```bash
     git clone <repository_url>
     ```
   - クローンしたリポジトリのディレクトリに移動します：
     ```bash
     cd <repository_directory>
     ```
   - プロジェクトのスクリプトは `Joe_The_Walk/Assets/Scripts` フォルダにあります。

5. **プロジェクトのインポート**
   - Unity Hubを開き、`プロジェクト`タブをクリックします。
   - `既存のプロジェクトを追加`をクリックし、解凍したプロジェクトフォルダを選択して`開く`をクリックします。
   - プロジェクトがリストに追加されたら、該当するプロジェクト名をクリックしてUnityエディタを開きます。

6. **必要なパッケージのインストール**
   - Unityエディタ内で`Window > Package Manager`を開きます。
   - `Add package from git URL...`をクリックし、DOTweenとEPOOutlineのパッケージURLを入力してインストールします。
     - DOTween: `https://github.com/Demigiant/dotween`（もしくはAsset Storeからインストール）
     - EPOOutline: `https://github.com/LiminalEngagement/EPOOutline`（もしくはAsset Storeからインストール）

7. **プロジェクトの設定**
   - プロジェクト設定を確認し、必要に応じて調整します（例：ビルド設定、入力設定、品質設定など）。
   - `File > Build Settings`を開き、ターゲットプラットフォームを選択して設定を保存します。

## ビルドしたゲームの実行

ビルドされたゲームを実行するには、以下の手順に従ってください：

1. [こちらのリンク](https://d.kuku.lu/kfbyh4t2f)からZIPファイルをダウンロードします。
2. ダウンロードしたZIPファイルを解凍します。
3. 解凍したフォルダ内にある `Joe_The_Walk.exe` をダブルクリックして実行します。

※このゲームはWindowsのみを対象としています。

以上で、Unity 2021.3.24f1環境でのプロジェクト構築およびビルドしたゲームの実行が完了です。
