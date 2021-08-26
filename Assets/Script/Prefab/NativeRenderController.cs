using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NativeRenderController : MonoBehaviour
{
    private bool isSetupTexture;
    private RenderTexture renderTexture;
    public Camera renderCamera;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {   
        setupRender();
        yield return StartCoroutine("CallPluginAtEndOfFrames");
    }

    /// Setup a RenderCamera, render target is a RenderTexture (720 x 1280).
    /// RenderTexture will send the texture pointer to Native side,
    /// And Native side will convert the texture pointer to OpenGL textureID / Metal texture.
    private void setupRender()
    {
        renderTexture = new RenderTexture(720, 1280, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        renderTexture.name = "RenderTexture";

        var data = renderCamera.GetUniversalAdditionalCameraData();
        data.renderShadows = false;

        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.CopyFrom(Camera.main);
        renderCamera.backgroundColor = Color.clear;
        renderCamera.targetTexture = renderTexture;
        Debug.Log("[Unity] setup Render");
        updateRenderCameraTransform();
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android) 
        {
            renderCamera.enabled = false;
        }
#endif  
    }

    private void Update()
    {
        updateRenderCameraTransform();
    }

    private void updateRenderCameraTransform()
    {
        if (renderCamera)
        {
            renderCamera.transform.position = Camera.main.transform.position;
            renderCamera.transform.rotation = Camera.main.transform.rotation;
            renderCamera.transform.localScale = Camera.main.transform.localScale;
        }
    }

    void updateRenderTexture()
    {
        renderCamera.Render();
    }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        // Wait until all frame rendering is done
        yield return new WaitForEndOfFrame();

        if (!isSetupTexture)
        {
            PassRenderTextureToPlugin();
            isSetupTexture = true;
        }
    }

    void PassRenderTextureToPlugin()
    {
#if UNITY_ANDROID
        // Excute Android Rendering Logic
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidNativeAPI.SetTexturePtrFromUnity(renderTexture.GetNativeTexturePtr());
        }
#elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iOSNativeAPI.SetRenderTextureFromUnity(renderTexture.GetNativeTexturePtr());
        }   
#endif
    }
}
