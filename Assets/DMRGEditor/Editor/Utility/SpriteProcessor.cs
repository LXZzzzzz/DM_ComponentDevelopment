using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace DM.RGEditor
{
    public class SpriteProcessor : AssetPostprocessor
    {
        private void OnPostprocessTexture(Texture2D texture)
        {
            if (SettingsAsset.PreViewPicture==assetPath)
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                //͸��ͨ��
                textureImporter.alphaIsTransparency = true;
                //����ҪͼƬ���
                textureImporter.mipmapEnabled = false;

                //û��Sprite����ͼ���������޸�û��Apply,��μ���ͼ��������
                textureImporter.spritePackingTag = "test";
                //todo SpriteAtlas
            }
        }
    }
}
