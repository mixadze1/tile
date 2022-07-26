using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// author  :   jave.lin
/// date    :   2020.03.01
/// ������� ���������� ��������� ��������, ������� ������ ������ �������������� ��� ������������
/// ���� � ������������� �������� ������������ ����� ������, ������ ���������� ������ ����� ����� ������
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
        // ������������ �������� ����������, �������� ������ ������������� ����������, ������� ������� �������� ��� ���������� ��������
        moveDir_hash = Shader.PropertyToID("_MoveDir");
        motionColor_hash = Shader.PropertyToID("_MotionTintColor");
        invMaxMotion_hash = Shader.PropertyToID("_InvMaxMotion");
        alpha_hash = Shader.PropertyToID("_alpha");
        matList = new List<Material>();
        // �������� ���������
        CollectMats(gameObject.GetComponentsInChildren<MeshRenderer>(true));
        CollectMats(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true));
        // ���������� ��������� �������
        lastPos = transform.position;
    }

    private void CollectMats<T>(T[] renderers) where T : Renderer
    {
        if (renderers == null || renderers.Length == 0)
            return;

        foreach (var item in renderers)
        {
            // ������� �����������, ����� ������ ���� ������� ����� ��������, ������ ��� ���������� ������ �������� ���������
            if (item.sharedMaterial.shader == targetShader)
            { // �� ����������� ����� sharedMaterial, ����� ��� �������� �� ������ ��������� �������, ������� ���������� ��� �� ��������
              // ����, �� ���������� ���������� ���������
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
            item.SetVector(moveDir_hash, moveDir);              // ������ ��������, xyz: ��������� ������, �����������, w: ������
            item.SetColor(motionColor_hash, motionColor);       // ���� ������ ���������� ��������
            item.SetFloat(invMaxMotion_hash, 1f / maxMotion);   // �������� ������������ ���������� ��������, ������������ ��� ���������� ������ ��������� ����������� �����
            item.SetFloat(alpha_hash, alpha);                   // ����� �����
        }
    }
}
