using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestJietu : MonoBehaviour
{
    Texture2D screenShot; //保存截取的纹理
    public Image image; //显示截屏的Image
    public Image im;
    Texture2D texture2ds; //存储的截图

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Jietu();
        }
    }

    public void Jietu()
    {
        StartCoroutine(getScreenTexture(im.rectTransform));
    }

    public IEnumerator getScreenTexture(RectTransform rectT)
    {
        yield return new WaitForEndOfFrame();

        texture2ds = new Texture2D((int)rectT.rect.width, (int)rectT.rect.height, TextureFormat.RGB24, true);
        float x = rectT.localPosition.x + (Screen.width - rectT.rect.width) / 2;
        float y = rectT.localPosition.y + (Screen.height - rectT.rect.height) / 2;
        Rect position = new Rect(x, y, rectT.rect.width, rectT.rect.height);
        texture2ds.ReadPixels(position, 0, 0, true); //按照设定区域读取像素；注意是以左下角为原点读取
        texture2ds.Apply();
        Sprite sp = Sprite.Create(texture2ds, new Rect(0, 0, texture2ds.width, texture2ds.height), Vector2.zero);
        image.sprite = sp;
        //保存到streamingAssets
        byte[] bytes = texture2ds.EncodeToJPG();
        if (!Directory.Exists(Application.streamingAssetsPath + "/Images")) Directory.CreateDirectory(Application.streamingAssetsPath + "/Images");
        string filename = Application.streamingAssetsPath + "/Images/Screenshot" + DateTime.UtcNow.Ticks + ".png";
        File.WriteAllBytes(filename, bytes);
    }
}