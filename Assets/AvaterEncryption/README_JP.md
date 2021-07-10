# AvaterEncryption
これは[GeoTetr​​a AvaCrypt](https://github.com/rygo6/GTAvaCrypt)を自動化したフォークです。

## 注意
ブレンドシェイプが壊れる可能性があります。  
`_IgnoreEncryption`がオンになったメッシュを含むマテリアルでは暗号化が実行されないため、ブレンドシェイプを含むマテリアルではオンにすることをオススメします。
また、アバターを正しく表示するためにセーフティーで`Shaders`と`Custom Animations`がオンになっている必要があります。

## 必要条件
- VRCSDK3
- AvaterEncryptionシステムに対応したシェーダー
- ExpressionParametersの空きメモリが32以上

## 使い方
1. `AvaCryptRoot`コンポーネントを`VRCAvatarDescriptor`と同一階層（アバターのルート）に追加してください。コンポーネント内の4つのキーをAvatar 3.0のメニューで入力することで復号化されます。
2. `AvaCryptRoot`コンポーネントの`Encrypt Avatar`をクリックしてください。
3. 名前に`_Encrypted`が追加されたアバターをアップロードしてください。