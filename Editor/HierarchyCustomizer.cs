using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyCustomizer
{
    static HierarchyCustomizer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private struct ItemStyle
    {
        public Color32 backgroundColor;
        public Color textColor;
        public string prefix;

        public ItemStyle(Color32 bg, Color txtColor, string prefix)
        {
            backgroundColor = bg;
            textColor = txtColor;
            this.prefix = prefix;
        }
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        // 스타일 정의 (우선순위 높은 순서로)
        ItemStyle? style = GetStyle(obj.name);
        if (style == null) return;

        // 배경 칠하기
        EditorGUI.DrawRect(selectionRect, style.Value.backgroundColor);

        // 텍스트 위치 지정
        Rect labelRect = new Rect(selectionRect.x + 20f, selectionRect.y, selectionRect.width - 20f, selectionRect.height);

        // 스타일 생성
        GUIStyle guiStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = style.Value.textColor }
        };

        // 텍스트 조합
        string displayName = obj.name;
        if (obj.name.Contains("-"))
        {
            // "-"가 있을 땐 하이픈 제거하고 대괄호로 감싸기 (기존 스타일 유지)
            displayName = "[ " + obj.name.Replace("-", "") + " ]";
            // prefix는 이미 포함하므로 비움
            GUI.Label(labelRect, "     " + displayName, guiStyle);
        }
        else
        {
            GUI.Label(labelRect, style.Value.prefix + " " + displayName, guiStyle);
        }
    }

    static ItemStyle? GetStyle(string name)
    {
        // 조건별 스타일 및 아이콘, 색상 지정
        // 우선순위에 맞게 작성
        if (name.Contains("-"))
        {
            return new ItemStyle(new Color32(0, 0, 0, 255), Color.white, "");
        }
        if (name.Contains("Test"))
        {
            return new ItemStyle(new Color32(255, 255, 255, 255), Color.black, "├─ 🏷️");
        }
        if (name.Contains("Player"))
        {
            return new ItemStyle(new Color32(144, 238, 144, 255), Color.black, "├─ 🕹️");
        }
        if (name.Contains("Manager") || name.Contains("System"))
        {
            return new ItemStyle(new Color32(135, 206, 250, 255), Color.black, "├─ ⚙️");
        }
        if (name.Contains("Enemy"))
        {
            return new ItemStyle(new Color32(255, 160, 122, 255), Color.black, "├─ ⚔️");
        }
        if (name.Contains("Canvas"))
        {
            return new ItemStyle(new Color32(152, 255, 204, 255), Color.black, "├─ 🖥️");
        }
        if (name.Contains("Image"))
        {
            return new ItemStyle(new Color32(190, 255, 230, 255), Color.black, "├─ 🖼️");
        }
        if (name.Contains("Button"))
        {
            return new ItemStyle(new Color32(190, 255, 230, 255), Color.black, "├─ 🔲");
        }
        if (name.Contains("Slider") || name.Contains("Scroll") || name.Contains("Dropdown") || name.Contains("Field"))
        {
            return new ItemStyle(new Color32(190, 255, 230, 255), Color.black, "├─ 🎛️");
        }
        if (name.Contains("Text") || name.Contains("Toggle"))
        {
            return new ItemStyle(new Color32(190, 255, 230, 255), Color.black, "├─ 🔤");
        }
        if (name.Contains("Effect") || name.Contains("Particle"))
        {
            return new ItemStyle(new Color32(255, 239, 150, 255), Color.black, "├─ ✨");
        }
        if (name.Contains("Camera"))
        {
            return new ItemStyle(new Color32(175, 238, 238, 255), Color.black, "├─ 🎥");
        }
        if (name.Contains("Light"))
        {
            return new ItemStyle(new Color32(255, 244, 179, 255), Color.black, "├─ 💡");
        }

        // 기본 스타일
        return new ItemStyle(new Color32(56, 56, 56, 255), Color.white, "├─");
    }
}
