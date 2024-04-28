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
                //透明通道
                textureImporter.alphaIsTransparency = true;
                //不需要图片深度
                textureImporter.mipmapEnabled = false;

                //没有Sprite精灵图集，代码修改没有Apply,如何加入图集？？？
                textureImporter.spritePackingTag = "test";
                //todo SpriteAtlas
            }
        }
    }
}
