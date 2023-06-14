using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Newtonsoft.Json.Linq;
using Unity.Collections.LowLevel.Unsafe;
using System.Security.Cryptography;

namespace SahilariousGames.IconCreator
{
    public class IconCreatorWindow : EditorWindow
    {
        [MenuItem("Window/Art Tools/IconCreatorWindow")]
        public static void ShowExample()
        {
            IconCreatorWindow window = GetWindow<IconCreatorWindow>();
            window.titleContent = new GUIContent("IconCreatorWindow");

            window.minSize = new Vector2(1000, 800);
            window.maxSize = new Vector2(1000, 800);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            m_root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/IconCreator/IconCreatorWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            m_root.Add(labelFromUXML);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/IconCreator/IconCreatorWindow.uss");
            m_root.styleSheets.Add(styleSheet);
            //VisualElement labelWithStyle = new Label("Hello World! With Style");
            //labelWithStyle.styleSheets.Add(styleSheet);
            //root.Add(labelWithStyle);


            // ------------------------------ Get UI Elements ------------------------------


            m_viewport = m_root.Query<UIImage>("viewport");

            m_createIconButton = m_root.Query<Button>("createIconButton");
            m_createIconButton.clicked += OnCreateIconButtonSelected;

            m_loadPrefabButton = m_root.Query<Button>("loadPrefabButton");
            m_loadPrefabButton.clicked += OnLoadPrefabButtonSelected;


            m_cameraXRotationSlider = m_root.Query<Slider>("cameraXRotation");

            m_cameraYRotationSlider = m_root.Query<Slider>("cameraYRotation");

            m_cameraXRotationSlider.RegisterValueChangedCallback<float>(OnCameraXRotationChanged);
            m_cameraYRotationSlider.RegisterValueChangedCallback<float>(OnCameraYRotationChanged);

            m_cameraZoomSlider = m_root.Query<Slider>("cameraZoom");

            m_cameraZoomSlider.RegisterValueChangedCallback<float>(OnCameraZoomChanged);

            m_cameraXPositionSlider = m_root.Query<Slider>("cameraXPosition");
            m_cameraYPositionSlider = m_root.Query<Slider>("cameraYPosition");

            m_cameraXPositionSlider.RegisterValueChangedCallback<float>(OnCameraXPositionChanged);
            m_cameraYPositionSlider.RegisterValueChangedCallback<float>(OnCameraYPositionChanged);


            m_objectXRotationSlider = m_root.Query<Slider>("objectXRotation");
            m_objectYRotationSlider = m_root.Query<Slider>("objectYRotation");
            m_objectZRotationSlider = m_root.Query<Slider>("objectZRotation");

            m_objectXRotationSlider.RegisterValueChangedCallback(OnObjectXRotationChanged);
            m_objectYRotationSlider.RegisterValueChangedCallback(OnObjectYRotationChanged);
            m_objectZRotationSlider.RegisterValueChangedCallback(OnObjectZRotationChanged);


            m_lightOnecolorField = m_root.Query<UIColorField>("firstLightColorField");
            m_lightTwocolorField = m_root.Query<UIColorField>("secondLightColorField");

            m_lightOnecolorField.RegisterValueChangedCallback(OnFirstLightColorChanged);
            m_lightTwocolorField.RegisterValueChangedCallback(OnSecondLightColorChanged);

            m_backgroundColorField = m_root.Query<UIColorField>("backgroundColorField");

            m_backgroundColorField.RegisterValueChangedCallback(OnBackgroundColorChanged);

            m_backgroundToggle = m_root.Query<Toggle>("backgroundToggle");

            m_backgroundToggle.RegisterValueChangedCallback(OnBackgroundToggleChanged);

            // ------------------------------ Set Stage ------------------------------


            string stagepath = "Assets/IconCreator/Prefabs/IconCreatorStage.prefab";

            m_stage = GameObject.Instantiate(PrefabUtility.LoadPrefabContents(stagepath), Vector3.zero, Quaternion.identity).GetComponent<IconCreatorStage>();


            // ------------------------------ Set Initial Settings ------------------------------

            m_stage.Pivot.rotation = Quaternion.identity;

            m_lightOnecolorField.value = m_stage.LightOne.color;
            m_lightTwocolorField.value = m_stage.LightTwo.color;

            m_backgroundColorField.value = m_stage.Camera.backgroundColor;

            m_backgroundToggle.value = true;


            UpdateViewport();
        }

        private VisualElement m_root;

        private UIImage m_viewport;

        private Button m_createIconButton;

        private Button m_loadPrefabButton;

        private GameObject m_currentLoadedObject;

        private IconCreatorStage m_stage;

        // ------------------------------ Camera Options ------------------------------

        private Slider m_cameraXRotationSlider;

        private Slider m_cameraYRotationSlider;

        private Slider m_cameraZoomSlider;

        private Slider m_cameraXPositionSlider;

        private Slider m_cameraYPositionSlider;

        // ------------------------------ Object Options ------------------------------

        private Slider m_objectXRotationSlider;

        private Slider m_objectYRotationSlider;

        private Slider m_objectZRotationSlider;

        // ------------------------------ Lighting Options ------------------------------


        private UIColorField m_lightOnecolorField;

        private UIColorField m_lightTwocolorField;

        // ------------------------------ Background Options ------------------------------

        private UIColorField m_backgroundColorField;

        private Toggle m_backgroundToggle;


        private void OnCreateIconButtonSelected()
        {
            if (m_currentLoadedObject)
            {
                UpdateViewport();

                string path = $"Assets/IconCreator/Images/{m_currentLoadedObject.name}-{GUID.Generate()}.png";
                //m_viewport.sprite.
                // Encode texture into PNG
                byte[] bytes = ImageConversion.EncodeToPNG(m_viewport.sprite.texture);

                // write to a file in the project folder
                //File.WriteAllBytes(path, bytes);


                WriteImageFile(path, bytes);
            }
        }

        private async void WriteImageFile(string path, byte[] bytes)
        {
            await File.WriteAllBytesAsync(path, bytes);

            //Texture2D newObject = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IconCreator/Prefabs/IconCreatorStage.prefab", typeof(Object));
            GameObject go = PrefabUtility.LoadPrefabContents("Assets/IconCreator/Prefabs/IconCreatorStage.prefab");
            Selection.activeObject = go;

            //EditorGUIUtility.PingObject(go);

            AssetDatabase.Refresh();
        }

        private void OnLoadPrefabButtonSelected()
        {
            DestroyCurrentObject();

            string path = EditorUtility.OpenFilePanel("Load Prefab To Take Image", "Assets/Prefabs", "prefab");

            if (string.IsNullOrEmpty(path) == false)
            {
                m_currentLoadedObject = GameObject.Instantiate(PrefabUtility.LoadPrefabContents(path), Vector3.zero, Quaternion.identity);
            }

            UpdateViewport();
        }

        private Color m_backgroundColor = new Color(0,0,0,0);

        private int m_textureSize = 512;

        private void UpdateViewport()
        {
            Debug.Log("Update Viewport");
            RenderTexture renderTexture = new RenderTexture(m_textureSize, m_textureSize, 0);
            m_stage.Camera.targetTexture = renderTexture;

            m_stage.Camera.Render();

            RenderTexture currentRenderTexture = RenderTexture.active;

            RenderTexture.active = renderTexture;

            Texture2D texture = new Texture2D(m_textureSize, m_textureSize, TextureFormat.RGBA32, false);
            Rect rect = new Rect(0, 0, m_textureSize, m_textureSize);

            texture.ReadPixels(rect, 0, 0);

            ////////////////////////////////////////////

            if (m_backgroundToggle.value == false)
            {
                for (int i = 0; i < texture.width; i++)
                {
                    for (int j = 0; j < texture.width; j++)
                    {
                        Color pixelColor = texture.GetPixel(i, j);
                        if (CheckColorValueAccurate(pixelColor.r, m_stage.Camera.backgroundColor.r, 1.0f) &&
                             CheckColorValueAccurate(pixelColor.g, m_stage.Camera.backgroundColor.g, 1.0f) &&
                             CheckColorValueAccurate(pixelColor.b, m_stage.Camera.backgroundColor.b, 1.0f))
                        {
                            texture.SetPixel(i, j, m_backgroundColor);
                        }
                        else
                        {

                        }
                    }
                }
            }

           ///////////////////////////////////////////

            texture.Apply();

            //reset camera’s target texture
            m_stage.Camera.targetTexture = null;
            // reset active renderTexture
            RenderTexture.active = currentRenderTexture;
            // free some memory?
            DestroyImmediate(renderTexture);

            m_viewport.sprite = Sprite.Create(texture, rect, Vector2.zero);
        }

        private bool CheckColorValueAccurate(float value1, float value2, float precision)
        {
            return (value1 * 255 - value2 * 255) < precision;
        }

        private void OnDestroy()
        {
            DestroyCurrentObject();
            DestroyStage();
        }

        private void DestroyCurrentObject()
        {
            if (m_currentLoadedObject)
            {
                DestroyImmediate(m_currentLoadedObject);
            }
        }
        private void DestroyStage()
        {
            if (m_stage)
            {
                DestroyImmediate(m_stage.gameObject);
            }
        }

        // Slider Actual Range: 0 to 100

        private float m_cameraXRotationMultFactor = 90.0f / 50.0f;

        // Range Target: -90 to 90

        private float m_cameraXRot;
        private float m_cameraYRot;

        private void OnCameraXRotationChanged(ChangeEvent<float> e)
        {
            m_cameraXRot = (e.newValue - 50) * m_cameraXRotationMultFactor;

            m_stage.Pivot.localRotation = Quaternion.Euler(m_cameraXRot,
                                                           m_cameraYRot,
                                                           m_stage.Pivot.localRotation.z);

            Debug.Log($"Camera X Rotation: {m_cameraXRot}, Local Rotation: {m_stage.Pivot.localRotation.eulerAngles}");


            UpdateViewport();
        }

        private float m_cameraYRotationMultFactor = 180.0f / 50.0f;

        // Range Target: -180 to 180
        private void OnCameraYRotationChanged(ChangeEvent<float> e)
        {
            m_cameraYRot = (e.newValue - 50) * m_cameraYRotationMultFactor;

            m_stage.Pivot.localRotation = Quaternion.Euler(m_cameraXRot,
                                                       m_cameraYRot,
                                                       m_stage.Pivot.localRotation.z);

            Debug.Log($"Camera Y Rotation: {m_cameraYRot}, Local Rotation: {m_stage.Pivot.localRotation.eulerAngles}");

            UpdateViewport();

        }

        // target -4 0 

        private void OnCameraZoomChanged(ChangeEvent<float> e)
        {
            m_stage.Camera.transform.localPosition = new Vector3(m_stage.Camera.transform.localPosition.x,
                                                                    m_stage.Camera.transform.localPosition.y, 
                                                                    e.newValue * -4.0f / 100.0f);
            Debug.Log($"Zoom New Value: {e.newValue * -4.0f / 100.0f}");
            UpdateViewport();

        }

        private void OnCameraXPositionChanged(ChangeEvent<float> e)
        {
            m_stage.Camera.transform.localPosition = new Vector3((e.newValue - 50.0f) * m_cameraPositionMultFactor,
                                                        m_stage.Camera.transform.localPosition.y,
                                                        m_stage.Camera.transform.localPosition.z);

            UpdateViewport();
        }

        private float m_cameraPositionMultFactor = 1.0f/50.0f;

        private void OnCameraYPositionChanged(ChangeEvent<float> e)
        {
            m_stage.Camera.transform.localPosition = new Vector3(m_stage.Camera.transform.localPosition.x,
                                               (e.newValue - 50.0f) * m_cameraPositionMultFactor,
                                               m_stage.Camera.transform.localPosition.z);

            UpdateViewport();
        }

        private float m_objectRotationMultFactor = 180/50.0f;

        private float m_objectXRot;
        private float m_objectYRot;
        private float m_objectZRot;

        private void OnObjectXRotationChanged(ChangeEvent<float> e)
        {
            float newValue = (e.newValue - 50) * m_objectRotationMultFactor;

            m_objectXRot = newValue;

            m_currentLoadedObject.transform.localRotation = Quaternion.Euler(m_objectXRot,
                                                            m_objectYRot,
                                                            m_objectZRot);
                
            UpdateViewport();
        }

        private void OnObjectYRotationChanged(ChangeEvent<float> e)
        {
            float newValue = (e.newValue - 50) * m_objectRotationMultFactor;

            m_objectYRot = newValue;

            m_currentLoadedObject.transform.localRotation = Quaternion.Euler(m_objectXRot,
                                                            m_objectYRot,
                                                            m_objectZRot);
            
            UpdateViewport();
        }

        private void OnObjectZRotationChanged(ChangeEvent<float> e)
        {
            float newValue = (e.newValue - 50) * m_objectRotationMultFactor;

            m_objectZRot = newValue;

            m_currentLoadedObject.transform.localRotation = Quaternion.Euler(m_objectXRot,
                                                            m_objectYRot,
                                                            m_objectZRot);

            UpdateViewport();
        }

        private void OnFirstLightColorChanged(ChangeEvent<Color> e)
        {
            m_stage.LightOne.color = e.newValue;

            UpdateViewport();
        }

        private void OnSecondLightColorChanged(ChangeEvent<Color> e)
        {
            m_stage.LightTwo.color = e.newValue;

            UpdateViewport();
        }

        private void OnBackgroundColorChanged(ChangeEvent<Color> e)
        {
            m_stage.Camera.backgroundColor = e.newValue;

            UpdateViewport();
        }

        private void OnBackgroundToggleChanged(ChangeEvent<bool> e)
        {
            UpdateViewport();
        }
    }
}
