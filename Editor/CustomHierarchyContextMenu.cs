using UnityEditor;
using UnityEngine;

public static class CustomHierarchyContextMenu
{
    [MenuItem("GameObject/GroulCustomPackage/3D/FSM/Player", false, 0)] // 우클릭 시 나올 메뉴 항목
    static void Create_CharacterController_Player(MenuCommand menuCommand)
    {
        // 새로운 GameObject 생성
        GameObject go = new GameObject("Player");
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = "TestModel";
        model.transform.SetParent(go.transform);

        // 현재 선택한 오브젝트를 부모로 설정 (있으면)
        GameObject context = menuCommand.context as GameObject;
        if (context != null)
        {
            go.transform.SetParent(context.transform);
        }

        // 컴포넌트 추가
        go.AddComponent<Rigidbody>();
        go.AddComponent<FSM_PlayerController>();
        go.GetComponent<Rigidbody>().freezeRotation = true;

        // 유니티 Undo 시스템 등록
        Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);

        // 생성된 오브젝트 선택 상태로 만들기
        Selection.activeObject = go;

        Debug.Log("[ GroulCustomPackage : FSM Player ]\n- Rigidbody의 Freeze Rotation이 켜져 있습니다, 참고하세요.\n- 기본 설정으로, 바닥으로 인식할 Layer(GroundLayerMask)을 선택해 주세요.");
    }
}
