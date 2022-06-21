using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Es_ModelWarningInfo
{
    public GameObject Model { get; set; }
    public int AllModelTrisCount { get; set; }
}
public class Es_ParticleWarningInfo
{
    public GameObject RootResourceGameObject = null;
    public GameObject ResourceGameObject = null;

    public ParticleSystem ParticleSystem = null;
    public Texture2D ParticleTexture = null;

    public bool MaxParticlesWarning = false;
    public bool TextureSizeWarning = false;
}
public class Es_RseourcesCheckUtility : Editor
{
    public static void checkModelAboveThreshold(string path, int maxMeshTris, out List<Es_ModelWarningInfo> modelInfoList)
    {
        if(path == null || path.Length == 0)
        {
            EditorUtility.DisplayDialog("Error!", "未选择正确的目录", "确定");
            modelInfoList = null;
            return;
        }
        string[] models = null;
        models = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        modelInfoList = new List<Es_ModelWarningInfo>();
        int len = models.Length;
        Debug.Log("总共搜索" + len + "个Object");

        foreach (string model in models)
        {
            if (Path.GetExtension(model) == ".FBX" || Path.GetExtension(model) == ".fbx")
            {
                GameObject objAsset = (GameObject)AssetDatabase.LoadAssetAtPath(model, typeof(GameObject));
                MeshFilter[] meshfilters = objAsset?.GetComponentsInChildren<MeshFilter>();
                SkinnedMeshRenderer[] skinnedmeshrenderers = null;

                int trisCount = 0;
                if (meshfilters == null || meshfilters.Length == 0)
                {
                    meshfilters = null;
                    skinnedmeshrenderers = objAsset?.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer skinnedmeshrenderer in skinnedmeshrenderers)
                        trisCount += skinnedmeshrenderer.sharedMesh.triangles.Length / 3;
                }
                else
                {
                    foreach (MeshFilter meshfilter in meshfilters)
                        trisCount += meshfilter.sharedMesh.triangles.Length / 3;
                }

                if (trisCount > maxMeshTris)
                {
                    Es_ModelWarningInfo info = new Es_ModelWarningInfo();
                    info.Model = objAsset;
                    info.AllModelTrisCount = trisCount;
                    modelInfoList.Add(info);
                }
            }
        }
        return;
    }
    public static void checkParticleAboveThreshold(bool warningPaiticleCount, bool warningTextureSize,
        bool warningSingleTextureSize, string path, int maxParticles, int maxTexResolution, out List<Es_ParticleWarningInfo> particleInfoList)
    {
        if (path == null || path.Length == 0)
        {
            EditorUtility.DisplayDialog("Error!", "未选择正确的目录", "确定");
            particleInfoList = null;
            return;
        }
        //获取所有预制体
        string[] particlePrefabs = null;
        particlePrefabs = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        int len = particlePrefabs.Length;
        Debug.Log("总共搜索" + len + "个Object");

        particleInfoList = new List<Es_ParticleWarningInfo>();
        for (int i = 0; i < len; ++i)
        {
            //加载预制体资源
            GameObject RootResourceGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(particlePrefabs[i]);
            List<ParticleSystem> particleSystemList = new List<ParticleSystem>();
            //判断是否是特效
            particleSystemList.AddRange(RootResourceGameObject.GetComponentsInChildren<ParticleSystem>());
            if (particleSystemList.Count == 0) continue;

            //逐个检查特效里的粒子系统
            foreach (var particleSystem in particleSystemList)
            {
                if (particleSystem)
                {
                    Es_ParticleWarningInfo particleWarningInfo = new Es_ParticleWarningInfo();
                    //记录特效信息
                    particleWarningInfo.RootResourceGameObject = RootResourceGameObject;
                    particleWarningInfo.ResourceGameObject = particleSystem.gameObject;
                    particleWarningInfo.ParticleSystem = particleSystem;

                    if (warningPaiticleCount)
                        if (particleSystem.main.maxParticles > maxParticles)
                            particleWarningInfo.MaxParticlesWarning = true;

                    if (warningTextureSize)
                    {
                        var psRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                        if (psRenderer && psRenderer.sharedMaterial)
                        {
                            List<Texture2D> results = new List<Texture2D>();
                            Object[] roots = new Object[] { psRenderer.sharedMaterial };
                            Object[] dependObjs = EditorUtility.CollectDependencies(roots);
                            int sumTexSize = 0;
                            foreach (Object dependObj in dependObjs)
                            {
                                if (dependObj.GetType() == typeof(Texture2D))
                                {
                                    Texture2D tex = (Texture2D)dependObj;
                                    if (warningSingleTextureSize)
                                        sumTexSize = tex.width * tex.height;
                                    else
                                        sumTexSize += tex.width * tex.height;
                                    if (warningSingleTextureSize && sumTexSize > maxTexResolution * maxTexResolution)
                                    {
                                        particleWarningInfo.TextureSizeWarning = true;
                                        particleWarningInfo.ParticleTexture = tex;
                                    }
                                }
                            }
                            if (!warningSingleTextureSize && sumTexSize > maxTexResolution * maxTexResolution)
                            {
                                particleWarningInfo.TextureSizeWarning = true;
                                particleWarningInfo.ParticleTexture = null;
                            }

                        }
                    }
                    if (particleWarningInfo.MaxParticlesWarning || particleWarningInfo.TextureSizeWarning)
                        particleInfoList.Add(particleWarningInfo);
                }
            }
        }
    }

}
