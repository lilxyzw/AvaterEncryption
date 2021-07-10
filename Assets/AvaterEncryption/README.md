# AvaterEncryption
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