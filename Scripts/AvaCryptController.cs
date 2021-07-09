#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using BlendTree = UnityEditor.Animations.BlendTree;
using Object = UnityEngine.Object;

#if VRC_SDK_VRCSDK3
    using VRC.SDK3.Avatars.Components;
    using VRC.SDK3.Editor;
    using VRC.SDKBase.Editor;
    using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
    using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
    using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
    using ExpressionControl = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
#endif

namespace GeoTetra.GTAvaCrypt
{
    public class AvaCryptController
    {
        const int maxExPropMemory = 128;
        const string pathParam = "Assets/expressionParam_Encrypted.asset";
        const string pathMenu = "Assets/expressionMenu_Encrypted.asset";
        const string pathController = "Assets/expressionAnimator_Encrypted.asset";
        const string pathDefaultExpressionParameters = "Assets/VRCSDK/Examples3/Expressions Menu/DefaultExpressionParameters.asset";
        const string pathDefaultExpressionsMenu = "Assets/VRCSDK/Examples3/Expressions Menu/DefaultExpressionsMenu.asset";
        const string pathDefaultAnimationController = "Assets/VRCSDK/Examples3/Animation/Controllers/vrc_AvatarV3FaceLayer.controller";
        const string pathAvaCryptMenu = "Assets/AvaterEncryption/VrcExpressions/AvaCryptKeyMenu.asset";
        const string additionalName = "_Encrypted";
        const string menuName = "AvaCryptKey";
        private readonly string[] AvaCryptKeyNames = { "AvaCryptKey0", "AvaCryptKey1", "AvaCryptKey2", "AvaCryptKey3" };
        private readonly AnimationClip[] _clips0 = new AnimationClip[4];
        private readonly AnimationClip[] _clips100 = new AnimationClip[4];

        private const string StateMachineName = "AvaCryptKey{0} State Machine";
        private const string BlendTreeName = "AvaCryptKey{0} Blend Tree";

        public void ValidateAnimations(GameObject gameObject, AnimatorController controller)
        {
            for (int i = 0; i < AvaCryptKeyNames.Length; ++i)
            {
                ValidateClip(gameObject, controller, i);
            }

            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                for (int i = 0; i < _clips0.Length; ++i)
                {
                    string animKeyName = "";
                    if(i == 0) animKeyName += $"material._Keys.x";
                    if(i == 1) animKeyName += $"material._Keys.y";
                    if(i == 2) animKeyName += $"material._Keys.z";
                    if(i == 3) animKeyName += $"material._Keys.w";
                    string transformPath = AnimationUtility.CalculateTransformPath(meshRenderer.transform, gameObject.transform);
                    _clips0[i].SetCurve(transformPath, typeof(MeshRenderer), animKeyName, new AnimationCurve(new Keyframe(0, 0)));
                    _clips100[i].SetCurve(transformPath, typeof(MeshRenderer), animKeyName, new AnimationCurve(new Keyframe(0, 100)));
                }
            }

            SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                for (int i = 0; i < _clips0.Length; ++i)
                {
                    string animKeyName = "";
                    if(i == 0) animKeyName += $"material._Keys.x";
                    if(i == 1) animKeyName += $"material._Keys.y";
                    if(i == 2) animKeyName += $"material._Keys.z";
                    if(i == 3) animKeyName += $"material._Keys.w";
                    string transformPath = AnimationUtility.CalculateTransformPath(skinnedMeshRenderer.transform, gameObject.transform);
                    _clips0[i].SetCurve(transformPath, typeof(SkinnedMeshRenderer), animKeyName, new AnimationCurve(new Keyframe(0, 0)));
                    _clips100[i].SetCurve(transformPath, typeof(SkinnedMeshRenderer), animKeyName, new AnimationCurve(new Keyframe(0, 100)));
                }
            }

            AssetDatabase.SaveAssets();
        }

        private void ValidateClip(GameObject gameObject, AnimatorController controller, int index)
        {
            string controllerPath = AssetDatabase.GetAssetPath(controller);
            string controllerFileName = System.IO.Path.GetFileName(controllerPath);

            string clipName = $"{gameObject.name}_{AvaCryptKeyNames[index]}";
            string clipName0 = $"{clipName}_0";
            string clipName0File = $"{clipName0}.anim";
            string clipName100 = $"{clipName}_100";
            string clipName100File = $"{clipName100}.anim";

            if (controller.animationClips.All(c => c.name != clipName0))
            {
                _clips0[index] = new AnimationClip()
                {
                    name = clipName0
                };
                string clip0Path = controllerPath.Replace(controllerFileName, clipName0File);
                AssetDatabase.CreateAsset(_clips0[index], clip0Path);
                AssetDatabase.SaveAssets();
                Debug.Log($"Adding and Saving Clip: {clip0Path}");
            }
            else
            {
                _clips0[index] = controller.animationClips.FirstOrDefault(c => c.name == clipName0);
                Debug.Log($"Found clip: {clipName0}");
            }

            if (controller.animationClips.All(c => c.name != clipName100))
            {
                _clips100[index] = new AnimationClip()
                {
                    name = clipName100
                };
                string clip100Path = controllerPath.Replace(controllerFileName, clipName100File);
                AssetDatabase.CreateAsset(_clips100[index], clip100Path);
                AssetDatabase.SaveAssets();
                Debug.Log($"Adding and Saving Clip: {clip100Path}");
            }
            else
            {
                _clips100[index] = controller.animationClips.FirstOrDefault(c => c.name == clipName100);
                Debug.Log($"Found clip: {clipName100}");
            }
        }

        public void ValidateParameters(AnimatorController controller)
        {
            foreach (string keyName in AvaCryptKeyNames)
            {
                if (controller.parameters.All(parameter => parameter.name != keyName))
                {
                    controller.AddParameter(keyName, AnimatorControllerParameterType.Float);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Adding parameter: {keyName}");
                }
                else
                {
                    Debug.Log($"Parameter already added: {keyName}");
                }
            }
        }

        public void ValidateLayers(AnimatorController controller)
        {
            for (int i = 0; i < AvaCryptKeyNames.Length; ++i)
            {
                if (controller.layers.All(l => l.name != AvaCryptKeyNames[i]))
                {
                    CreateLayer(i, controller);
                }
                else
                {
                    Debug.Log($"Layer already existing: {AvaCryptKeyNames[i]}");
                    AnimatorControllerLayer layer = controller.layers.FirstOrDefault(l => l.name == AvaCryptKeyNames[i]);

                    if (layer == null || layer.stateMachine == null)
                    {
                        Debug.Log("Layer missing state machine.");

                        // Try to delete blend tree and layers if by chance they still exist for some reason.
                        DeleteObjectFromController(controller, string.Format(StateMachineName, i));
                        DeleteObjectFromController(controller, string.Format(BlendTreeName, i));

                        controller.RemoveLayer(controller.layers.ToList().IndexOf(layer));

                        CreateLayer(i, controller);
                    }
                    else
                    {
                        ValidateLayerBlendTree(i, layer, controller);
                    }
                }
            }
        }

        private void ValidateLayerBlendTree(int index, AnimatorControllerLayer layer, AnimatorController controller)
        {
            string blendTreeName = string.Format(BlendTreeName, index);

            if (layer.stateMachine.states.All(s => s.state.name != blendTreeName))
            {
                Debug.Log($"Layer missing blend tree. {blendTreeName}");
                DeleteObjectFromController(controller, blendTreeName);
                AddBlendTree(index, layer, controller);
            }
            else
            {
                Debug.Log($"Layer Blend Tree Validated {blendTreeName}.");

                BlendTree blendTree = layer.stateMachine.states.FirstOrDefault(s => s.state.name == blendTreeName).state.motion as BlendTree;

                // Just re-assign since ChildMotions aren't their own ScriptableObjects.
                ChildMotion childMotion0 = new ChildMotion
                {
                    motion = _clips0[index],
                    timeScale = 1
                };
                ChildMotion childMotion1 = new ChildMotion
                {
                    motion = _clips100[index],
                    timeScale = 1
                };

                if (blendTree != null)
                {
                    blendTree.children = new ChildMotion[2] { childMotion0, childMotion1 };
                }

                AssetDatabase.SaveAssets(); ;
            }
        }

        private void CreateLayer(int index, AnimatorController controller)
        {
            Debug.Log($"Creating layer: {AvaCryptKeyNames[index]}");

            string controllerPath = AssetDatabase.GetAssetPath(controller);

            AnimatorControllerLayer layer = new AnimatorControllerLayer
            {
                name = AvaCryptKeyNames[index],
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine(),
            };
            layer.stateMachine.name = string.Format(StateMachineName, index);

            controller.AddLayer(layer);
            AssetDatabase.AddObjectToAsset(layer.stateMachine, controllerPath);
            AssetDatabase.SaveAssets();

            AddBlendTree(index, layer, controller);
        }

        private void AddBlendTree(int index, AnimatorControllerLayer layer, AnimatorController controller)
        {
            string controllerPath = AssetDatabase.GetAssetPath(controller);
            string blendTreeName = string.Format(BlendTreeName, index);

            AnimatorState state = layer.stateMachine.AddState(blendTreeName);
            state.speed = 1;

            BlendTree blendTree = new BlendTree
            {
                name = blendTreeName,
                blendType = BlendTreeType.Simple1D,
                blendParameter = AvaCryptKeyNames[index],
            };

            ChildMotion childMotion0 = new ChildMotion
            {
                motion = _clips0[index],
                timeScale = 1
            };
            ChildMotion childMotion1 = new ChildMotion
            {
                motion = _clips100[index],
                timeScale = 1
            };
            blendTree.children = new ChildMotion[2] { childMotion0, childMotion1 };

            state.motion = blendTree;
            AssetDatabase.AddObjectToAsset(blendTree, controllerPath);

            AssetDatabase.SaveAssets();
        }

        public bool AddExpressionParameters(VRCAvatarDescriptor avatarDescriptor)
        {
            avatarDescriptor.customExpressions = true;
            ExpressionParameters origParam = avatarDescriptor.expressionParameters;
            if(!AddParamsByName(origParam, AvaCryptKeyNames[0], true)) return false;
            if(!AddParamsByName(origParam, AvaCryptKeyNames[1])) return false;
            if(!AddParamsByName(origParam, AvaCryptKeyNames[2])) return false;
            if(!AddParamsByName(origParam, AvaCryptKeyNames[3])) return false;
            EditorUtility.SetDirty(origParam);
            return true;
        }

        private bool AddParamsByName(ExpressionParameters origParam, string paramName, bool shouldCheckSize = false)
        {
            int memoryCount = 0;
            for(int i = 0; i < origParam.parameters.Length; i++)
            {
                if(origParam.parameters[i].valueType == ExpressionParameters.ValueType.Bool)
                {
                    memoryCount += 1;
                }
                else
                {
                    memoryCount += 8;
                }

                if(origParam.parameters[i].name == paramName || origParam.parameters[i].name == string.Empty)
                {
                    origParam.parameters[i].name = paramName;
                    origParam.parameters[i].valueType = ExpressionParameters.ValueType.Float;
                    origParam.parameters[i].saved = true;
                    origParam.parameters[i].defaultValue = 0.0f;
                    i = 256;
                }
                else if(i == origParam.parameters.Length - 1)
                {
                    if(shouldCheckSize && (memoryCount + 32) > maxExPropMemory)
                    {
                        EditorUtility.DisplayDialog("Too many parameters","Please delete unused parameter from ExpressionParameters","OK");
                        return false;
                    }
                    else
                    {
                        Array.Resize(ref origParam.parameters, i+2);
                        ExpressionParameters.Parameter newParam = new ExpressionParameters.Parameter();
                        newParam.name = paramName;
                        newParam.valueType = ExpressionParameters.ValueType.Float;
                        newParam.saved = true;
                        newParam.defaultValue = 0.0f;
                        origParam.parameters[i+1] = newParam;
                        i = 256;
                    }
                }
            }
            return true;
        }

        public bool AddExpressionsMenu(VRCAvatarDescriptor avatarDescriptor)
        {
            ExpressionsMenu origMenu = avatarDescriptor.expressionsMenu;
            for(int i = 0; i < origMenu.controls.Count; i++)
            {
                if(origMenu.controls[i].name == menuName)
                {
                    origMenu.controls.RemoveAt(i);
                }
            }
            if(origMenu.controls.Count == 8)
            {
                EditorUtility.DisplayDialog("Too many menu items","Too many menu items","OK");
                return false;
            }
            ExpressionControl control = new ExpressionControl();
            control.name = menuName;
            control.type = ExpressionControl.ControlType.SubMenu;
            control.subMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(pathAvaCryptMenu, typeof(ExpressionsMenu));
            origMenu.controls.Add(control);
            EditorUtility.SetDirty(origMenu);
            return true;
        }

        private void DeleteObjectFromController(AnimatorController controller, string name)
        {
            string controllerPath = AssetDatabase.GetAssetPath(controller);
            foreach (Object subObject in AssetDatabase.LoadAllAssetsAtPath(controllerPath))
            {
                if (subObject.hideFlags == HideFlags.None && subObject.name == name)
                {
                    AssetDatabase.RemoveObjectFromAsset(subObject);
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
