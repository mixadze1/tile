using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// author  :   jave.lin
/// date    :   2020.03.01
/// Простая реализация трейлинга движения, вершины модели должны использоваться как компланарные
/// Если в некопланарных индексах используется много вершин, эффект растяжения модели будет очень плохим
/// </summary>
public class MotionEffectScript : MonoBehaviour
{
    public Shader targetShader;

    public Color motionColor = Color.white;

    [Range(1, 100f)]
    public float tweenSpeed = 10;
    [Range(1, 100f)]
    public float maxMotion = 10;

    private List<Material> matList;
    private int moveDir_hash = 0;
    private int motionColor_hash = 0;
    private int invMaxMotion_hash = 0;
    private int alpha_hash = 0;

    private Vector3 lastPos;

    void Start()
    {
        if (targetShader == null) targetShader = Shader.Find("Custom/MotionEffect");
        // Обеспечиваем скорость обновления, материал задает универсальные переменные, поэтому сначала получаем хэш однородных символов
        moveDir_hash = Shader.PropertyToID("_MoveDir");
        motionColor_hash = Shader.PropertyToID("_MotionTintColor");
        invMaxMotion_hash = Shader.PropertyToID("_InvMaxMotion");
        alpha_hash = Shader.PropertyToID("_alpha");
        matList = new List<Material>();
        // Собираем материалы
        CollectMats(gameObject.GetComponentsInChildren<MeshRenderer>(true));
        CollectMats(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true));
        // записываем последнюю позицию
        lastPos = transform.position;
    }

    private void CollectMats<T>(T[] renderers) where T : Renderer
    {
        if (renderers == null || renderers.Length == 0)
            return;

        foreach (var item in renderers)
        {
            // Решение разделяется, иначе нижний слой создаст новый Материал, потому что вызывается геттер свойства материала
            if (item.sharedMaterial.shader == targetShader)
            { // Не используйте здесь sharedMaterial, иначе это повлияет на другие ссылочные объекты, которые используют тот же материал
              // Итак, мы используем переменные материала
                matList.Add(item.material);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 moveDir = Vector4.zero;
        var curPos = transform.position;
        float alpha = 0;
        if ((curPos - lastPos).magnitude > 0.05f)
        {
            lastPos = Vector3.Lerp(lastPos, curPos, Time.deltaTime * tweenSpeed);
            var desPos = lastPos - curPos;
            moveDir = desPos.normalized;
            moveDir.w = desPos.magnitude;
            alpha = Mathf.Clamp01(moveDir.w / maxMotion);
        }

        foreach (var item in matList)
        {
            item.SetVector(moveDir_hash, moveDir);              // Вектор движения, xyz: единичный вектор, направление, w: модуль
            item.SetColor(motionColor_hash, motionColor);       // Цвет хвоста индикатора движения
            item.SetFloat(invMaxMotion_hash, 1f / maxMotion);   // Обратное максимальное расстояние смещения, используемое для управления второй половиной замыкающего альфа
            item.SetFloat(alpha_hash, alpha);                   // общая альфа
        }
    }
}
