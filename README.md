# AvaterEncryption

# English
This is a fork of [GeoTetr​​a AvaCrypt](https://github.com/rygo6/GTAvaCrypt) with some automation.

## Warning
It could break the blendshape.  
Meshes containing materials with `_IgnoreEncryption` turned on will not be encrypted, so it is recommended to turn it on for materials with blendshapes.  
Also, `Shaders` and `Custom Animations` must be turned on in the safety setting to display the avatar correctly.

## Requirements
- VRCSDK3
- Shader that support the AvaterEncryption system
- 32 or more free memory of ExpressionParameters

## Usage
1. Add the `AvaCryptRoot` component onto the root GameObject of your avatar, next to the `VRCAvatarDescriptor` component. Take note of the four "Key" values which are shown in this component, these are the values you must enter into your Avatar 3.0 puppeting menu.
2. On the `AvaCryptRoot` component click the `Encrypt Avatar` button.
3. `Build and Publish` your avatar which has `_Encrypted` appended to the name.

# 日本語
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