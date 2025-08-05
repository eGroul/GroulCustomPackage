using UnityEditor;
using UnityEngine;

public static class CustomHierarchyContextMenu
{
    [MenuItem("GameObject/GroulCustomPackage/3D/FSM/Player", false, 0)] // ��Ŭ�� �� ���� �޴� �׸�
    static void Create_CharacterController_Player(MenuCommand menuCommand)
    {
        // ���ο� GameObject ����
        GameObject go = new GameObject("Player");
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = "TestModel";
        model.transform.SetParent(go.transform);

        // ���� ������ ������Ʈ�� �θ�� ���� (������)
        GameObject context = menuCommand.context as GameObject;
        if (context != null)
        {
            go.transform.SetParent(context.transform);
        }

        // ������Ʈ �߰�
        go.AddComponent<Rigidbody>();
        go.AddComponent<FSM_PlayerController>();
        go.GetComponent<Rigidbody>().freezeRotation = true;

        // ����Ƽ Undo �ý��� ���
        Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);

        // ������ ������Ʈ ���� ���·� �����
        Selection.activeObject = go;

        Debug.Log("[ GroulCustomPackage : FSM Player ]\n- Rigidbody�� Freeze Rotation�� ���� �ֽ��ϴ�, �����ϼ���.\n- �⺻ ��������, �ٴ����� �ν��� Layer(GroundLayerMask)�� ������ �ּ���.");
    }
}
