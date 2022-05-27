using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MaskTexGeneratorWindow : EditorWindow
{
   public WindowMode Mode;
   public Object ImageFolder;
   public string MetallicTexLabel;
   public string OcclusionTexLabel;
   public string DetailTexLabel;
   public string SmoothnessTexLabel;
   public Texture2D MetallicTex;
   public Texture2D OcclusionTex;
   public Texture2D DetailTex;
   public bool isRoughnessMap;
   public Texture2D SmoothnessTex;

   [MenuItem("MaskTexGenerator/Show")]
   static void Init()
   {
      var window = (MaskTexGeneratorWindow) EditorWindow.GetWindow(typeof(MaskTexGeneratorWindow));;
      window.Show();
   }

   private void OnGUI()
   {
      Mode =  (WindowMode) EditorGUILayout.EnumPopup("Window Mode", Mode);
      switch (Mode)
      {
         case WindowMode.Label:
            DrawLabel();
            break;
         case WindowMode.Object:
            DrawObject();
            break;
      }

      if (GUILayout.Button("Generate Mask Tex"))
      {
         GenerateMaskTex();
      }
   }

   private void DrawLabel()
   {
      ImageFolder = EditorGUILayout.ObjectField("Images Folder", ImageFolder, typeof(Object), false);
      MetallicTexLabel = EditorGUILayout.TextField("Metallic Texture Label", MetallicTexLabel);
      OcclusionTexLabel = EditorGUILayout.TextField("Occlusion Texture Label", OcclusionTexLabel);
      DetailTexLabel = EditorGUILayout.TextField("Detail Texture Label", DetailTexLabel);
      isRoughnessMap = EditorGUILayout.Toggle("Is Roughness Tex?", isRoughnessMap);
      SmoothnessTexLabel = EditorGUILayout.TextField("Smoothness Texture Label", SmoothnessTexLabel);
   }
   private void DrawObject()
   {
      MetallicTex = EditorGUILayout.ObjectField("Metallic Tex", MetallicTex, typeof(Texture2D), false) as Texture2D;
      OcclusionTex = EditorGUILayout.ObjectField("Occlusion Tex", OcclusionTex, typeof(Texture2D), false) as Texture2D;
      DetailTex = EditorGUILayout.ObjectField("Detail Tex", DetailTex, typeof(Texture2D), false) as Texture2D;
      isRoughnessMap = EditorGUILayout.Toggle("Is Roughness Tex?", isRoughnessMap);
      SmoothnessTex = EditorGUILayout.ObjectField(isRoughnessMap ? "Roughness Tex" : "Smoothness Tex", SmoothnessTex, typeof(Texture2D), false) as Texture2D;
   }
   private Texture2D GetNotNullTex()
   {
      if (MetallicTex)
      {
         return MetallicTex;
      }
      if (OcclusionTex)
      {
         return OcclusionTex;
      }
      if (DetailTex)
      {
         return DetailTex;
      }
      if (SmoothnessTex)
      {
         return SmoothnessTex;
      }

      return null;
   }
   private void GenerateMaskTex()
   {
      if (Mode == WindowMode.Label)
      {
         LoadTexturesByLabels();
      }
      var notNullTex = GetNotNullTex();
      if (notNullTex == null)
      {
         Debug.LogError("Set at least one texture");
         return;
      }
      var tex = new Texture2D(notNullTex.width, notNullTex.height, TextureFormat.ARGB32, false);
      var pixels = tex.GetPixels();
      if (MetallicTex != null)
      {
         var secondPix = CustomTextureLoad(MetallicTex).GetPixels();
         for (int i = 0; i < secondPix.Length; i++)
         {
            pixels[i].r = secondPix[i].r;
         }
      }
      if (OcclusionTex != null)
      {
         var secondPix = CustomTextureLoad(OcclusionTex).GetPixels();
         for (int i = 0; i < secondPix.Length; i++)
         {
            pixels[i].g = secondPix[i].g;
         }
      }
      if (DetailTex != null)
      {
         var secondPix = CustomTextureLoad(DetailTex).GetPixels();
         for (int i = 0; i < secondPix.Length; i++)
         {
            pixels[i].b = secondPix[i].b;
         }
      }
      if (SmoothnessTex != null)
      {
         var secondPix = CustomTextureLoad(SmoothnessTex).GetPixels();
         for (int i = 0; i < secondPix.Length; i++)
         {
            pixels[i].a = isRoughnessMap ? 1 - secondPix[i].r : secondPix[i].r;
         }
       
      }
      tex.SetPixels(pixels);
      tex.Apply();
      var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(notNullTex));

      File.WriteAllBytes($"./{assetPath}/Generated_MaskTex.png", tex.EncodeToPNG());
      AssetDatabase.Refresh();
      Debug.Log($"Texture generated at {assetPath}");
   }

   private void LoadTexturesByLabels()
   {
      var path = AssetDatabase.GetAssetPath(ImageFolder);
      var files = Directory.GetFiles(path);
      MetallicTex = null;
      OcclusionTex = null;
      DetailTex = null;
      SmoothnessTex = null;
      foreach (var file in files.Where(str=> !str.Contains(".meta")))
      {
         if (!string.IsNullOrEmpty(MetallicTexLabel) && file.Contains(MetallicTexLabel))
         {
            MetallicTex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
         }
         if (!string.IsNullOrEmpty(OcclusionTexLabel) && file.Contains(OcclusionTexLabel))
         {
            OcclusionTex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
         }
         if (!string.IsNullOrEmpty(DetailTexLabel) && file.Contains(DetailTexLabel))
         {
            DetailTex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
         }
         if (!string.IsNullOrEmpty(SmoothnessTexLabel) && file.Contains(SmoothnessTexLabel))
         {
            SmoothnessTex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
         }
      }
   }
   //Need to not set read and write enabled
   private Texture2D CustomTextureLoad(Texture2D texture2D)
   {
      byte[] tmp = File.ReadAllBytes(AssetDatabase.GetAssetPath(texture2D));
      Texture2D tmpTexture = new Texture2D(1, 1);
      tmpTexture.LoadImage(tmp);
      return tmpTexture;
   }
   
   public enum WindowMode
   {
      Label,
      Object
   }
}
