# AssetSystem

## 概要

AssetBundleは実行時にBuildできないので作成したプログラム...の一部

オブジェクトのデータをJSON(とか)で保存、読み込みできる。

### 保存できるデータ
1. 位置,回転,スケール
2. オブジェクトの親子関係
3. 任意のコンポーネント
4. 任意のデータ
5. プログラム

C#プログラムを動的に読み込むのは危険なので、実行できる機能を絞ることで安全性を担保

(Assets/VisualScriptingは[nananapo/GraphConnectEngine](https://github.com/nananapo/GraphConnectEngine)に依存)

## 構成
```
Assets/
  AssetSystem		: AssetBundleの代わりの形式でオブジェクトを保存するスクリプト
  VisualScripting	: ノードベースプログラミングを実装するスクリプト
  VRUI				:  簡単なUI
```