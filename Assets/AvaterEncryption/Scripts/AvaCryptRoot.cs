using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Editor;
using VRC.SDKBase.Editor;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
using ExpressionControl = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
#endif
#endif

namespace GeoTetra.GTAvaCrypt
{
    public class AvaCryptRoot : MonoBehaviour
    {
        [Header("Higher value causes more distortion. Default = 0.02")]
        [Range(0.005f, 0.2f)]
        [SerializeField]
        private float _distortRatio = 0.02f;

        [Header("The four values which must be entered in game to display the model.")]
        [Range(3, 100)]
        [SerializeField]
        private int _key0;

        [Range(3, 100)]
        [SerializeField]
        private int _key1;

        [Range(3, 100)]
        [SerializeField]
        private int _key2;

        [Range(3, 100)]
        [SerializeField]
        private int _key3;

#if UNITY_EDITOR
        private readonly AvaCryptController _avaCryptController = new AvaCryptController();
        private readonly AvaCryptMesh _avaCryptMesh = new AvaCryptMesh();
        const int maxExPropMemory = 128;
        const string pathParam = "Assets/expressionParam_Encrypted.asset";
        const string pathMenu = "Assets/expressionMenu_Encrypted.asset";
        const string pathController = "Assets/expressionAnimator_Encrypted.asset";
        const string pathDefaultExpressionParameters = "Assets/VRCSDK/Examples3/Expressions Menu/DefaultExpressionParameters.asset";
        const string pathDefaultExpressionsMenu = "Assets/VRCSDK/Examples3/Expressions Menu/DefaultExpressionsMenu.asset";
        const string pathDefaultAnimationController = "Assets/VRCSDK/Examples3/Animation/Controllers/vrc_AvatarV3FaceLayer.controller";
        const string pathAvaCryptMenu = "Assets/AvaterEncryption/VrcExpressions";
        const string additionalName = "_Encrypted";

        public void ValidateAnimatorController(GameObject encodedGameObject)
        {
            if (encodedGameObject.transform.parent != null)
            {
                EditorUtility.DisplayDialog("AvaCryptRoot component not on a Root GameObject.",
                    "The GameObject which the AvaCryptRoot component is placed on must not be the child of any other GameObject.",
                    "Ok");
                return;
            }
            VRCAvatarDescriptor avatarDescriptor = encodedGameObject.GetComponent<VRCAvatarDescriptor>();
            if(avatarDescriptor == null)
            {
                EditorUtility.DisplayDialog("No Avatar Descriptor","Add Avatar Descriptor to","OK");
                return;
            }
            AnimatorController controller = GetAnimatorController(avatarDescriptor);

            if(!_avaCryptController.AddExpressionParameters(avatarDescriptor)) return;
            if(!_avaCryptController.AddExpressionsMenu(avatarDescriptor)) return;
            _avaCryptController.ValidateAnimations(gameObject, controller);
            _avaCryptController.ValidateParameters(controller);
            _avaCryptController.ValidateLayers(controller);
        }

        private AnimatorController GetAnimatorController(VRCAvatarDescriptor avatarDescriptor)
        {
            AnimatorController copiedController = null;

            // Backup ExpressionParameters
            if(avatarDescriptor.expressionParameters != null)
            {
                string paramPath = AssetDatabase.GetAssetPath(avatarDescriptor.expressionParameters);
                ExpressionParameters copiedParam = (ExpressionParameters)Instantiate(avatarDescriptor.expressionParameters);
                string paramSavePath = Path.GetDirectoryName(paramPath) + "/" + Path.GetFileNameWithoutExtension(paramPath) + additionalName + ".asset";
                EditorUtility.SetDirty(copiedParam);
                AssetDatabase.CreateAsset(copiedParam, paramSavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(paramSavePath, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionParameters = (ExpressionParameters)AssetDatabase.LoadAssetAtPath(paramSavePath, typeof(ExpressionParameters));
            }
            else
            {
                ExpressionParameters defaultParam = (ExpressionParameters)AssetDatabase.LoadAssetAtPath(pathDefaultExpressionParameters, typeof(ExpressionParameters));
                ExpressionParameters newParam = (ExpressionParameters)GameObject.Instantiate(defaultParam);
                EditorUtility.SetDirty(newParam);
                AssetDatabase.CreateAsset(newParam, pathParam);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(pathParam, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionParameters = (ExpressionParameters)AssetDatabase.LoadAssetAtPath(pathParam, typeof(ExpressionParameters));
            }

            // Backup ExpressionsMenu
            if(avatarDescriptor.expressionsMenu != null)
            {
                string menuPath = AssetDatabase.GetAssetPath(avatarDescriptor.expressionsMenu);
                string menuSavePath = Path.GetDirectoryName(menuPath) + "/" + Path.GetFileNameWithoutExtension(menuPath) + additionalName + ".asset";
                ExpressionsMenu copiedMenu = (ExpressionsMenu)Instantiate(avatarDescriptor.expressionsMenu);
                EditorUtility.SetDirty(copiedMenu);
                AssetDatabase.CreateAsset(copiedMenu, menuSavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(menuSavePath, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionsMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(menuSavePath, typeof(ExpressionsMenu));
            }
            else
            {
                ExpressionsMenu defaultMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(pathDefaultExpressionsMenu, typeof(ExpressionsMenu));
                ExpressionsMenu newMenu = (ExpressionsMenu)GameObject.Instantiate(defaultMenu);
                EditorUtility.SetDirty(newMenu);
                AssetDatabase.CreateAsset(newMenu, pathMenu);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(pathMenu, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionsMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(pathMenu, typeof(ExpressionsMenu));
            }

            // Backup AnimatorController
            for(int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
            {
                if (avatarDescriptor.baseAnimationLayers[i].type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    if(avatarDescriptor.baseAnimationLayers[i].animatorController != null)
                    {
                        string controllerPath = AssetDatabase.GetAssetPath(avatarDescriptor.baseAnimationLayers[i].animatorController);
                        string controllerSavePath = Path.GetDirectoryName(controllerPath) + "/" + Path.GetFileNameWithoutExtension(controllerPath) + additionalName + ".controller";
                        copiedController = (AnimatorController)Instantiate(avatarDescriptor.baseAnimationLayers[i].animatorController);
                        EditorUtility.SetDirty(copiedController);
                        AssetDatabase.CreateAsset(copiedController, controllerSavePath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(controllerSavePath, ImportAssetOptions.ForceUpdate);
                        avatarDescriptor.baseAnimationLayers[i].animatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(controllerSavePath, typeof(AnimatorController));
                    }
                    else
                    {
                        AnimatorController defaultCont = (AnimatorController)AssetDatabase.LoadAssetAtPath(pathDefaultAnimationController, typeof(AnimatorController));
                        AnimatorController newCont = (AnimatorController)Instantiate(defaultCont);
                        EditorUtility.SetDirty(newCont);
                        AssetDatabase.CreateAsset(newCont, pathController);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(pathController, ImportAssetOptions.ForceUpdate);
                        avatarDescriptor.baseAnimationLayers[i].animatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(pathController, typeof(AnimatorController));
                    }
                }
            }

            return copiedController;
        }

        public void EncryptAvatar()
        {
            // Disable old for convienence.
            gameObject.SetActive(false);

            string newName = gameObject.name + additionalName;

            // delete old GO, do as such in case its disabled
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] sceneRoots = scene.GetRootGameObjects();

            foreach (GameObject oldGameObject in sceneRoots)
            {
                if (oldGameObject.name == newName)
                {
                    DestroyImmediate(oldGameObject);
                }
            }

            GameObject encodedGameObject = Instantiate(gameObject);
            encodedGameObject.name = newName;
            encodedGameObject.SetActive(true);

            ValidateAnimatorController(encodedGameObject);

            MeshFilter[] meshFilters = encodedGameObject.GetComponentsInChildren<MeshFilter>(true);
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.GetComponent<MeshRenderer>() != null)
                {
                    var Materials = meshFilter.GetComponent<MeshRenderer>().sharedMaterials;

                    foreach (var Material in Materials)
                    {
                        if (Material.HasProperty("_Keys") && Material.HasProperty("_IgnoreEncryption") && Material.GetFloat("_IgnoreEncryption") == 0.0f)
                        {
                            meshFilter.sharedMesh = _avaCryptMesh.EncryptMesh(meshFilter.sharedMesh, _key0, _key1, _key2, _key3, _distortRatio);
                            break;
                        }
                    }
                }
            }

            SkinnedMeshRenderer[] skinnedMeshRenderers = encodedGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                var Materials = skinnedMeshRenderer.sharedMaterials;

                foreach (var Material in Materials)
                {
                    if (Material.HasProperty("_Keys") && Material.HasProperty("_IgnoreEncryption") && Material.GetFloat("_IgnoreEncryption") == 0.0f)
                    {
                        skinnedMeshRenderer.sharedMesh = _avaCryptMesh.EncryptMesh(skinnedMeshRenderer.sharedMesh, _key0, _key1, _key2, _key3, _distortRatio);
                        break;
                    }
                }
            }

            AvaCryptRoot[] avaCryptRoots = encodedGameObject.GetComponentsInChildren<AvaCryptRoot>(true);
            foreach (AvaCryptRoot avaCryptRoot in avaCryptRoots)
            {
                DestroyImmediate(avaCryptRoot);
            }
        }

        private void Reset()
        {
            // Start at 3 because 0 is kept to show unencrypted avatars normally.
            if (_key0 == 0)
            {
                _key0 = Random.Range(3, 100);
            }

            if (_key1 == 0)
            {
                _key1 = Random.Range(3, 100);
            }

            if (_key2 == 0)
            {
                _key2 = Random.Range(3, 100);
            }

            if (_key3 == 0)
            {
                _key3 = Random.Range(3, 100);
            }
        }

        private void OnValidate()
        {
            _key0 = RoundToThree(_key0);
            _key1 = RoundToThree(_key1);
            _key2 = RoundToThree(_key2);
            _key3 = RoundToThree(_key3);

            _key0 = Skip76(_key0);
            _key1 = Skip76(_key1);
            _key2 = Skip76(_key2);
            _key3 = Skip76(_key3);
        }

        private int RoundToThree(int value)
        {
            return (value / 3) * 3 + 1;
        }

        /// <summary>
        /// This is super specific to current version of VRC.
        /// There is a big which doesn't let you select 76 in radial menu, so skip it.
        /// </summary>
        private int Skip76(int value)
        {
            if (value == 76)
            {
                return value -= 3;
            }

            return value;
        }
#endif
    }
}
