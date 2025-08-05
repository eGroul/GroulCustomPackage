using UnityEditor;
using UnityEngine;
using System.IO;

public class FSMFullAutoCreator
{
    private const string scriptFileName = "FSMExample.cs";
    private const string textFileName = "FSM_Description.txt";
    private const string menuName = "Assets/Create/FSM Complete Example";

    private static readonly string fsmCode = @"using UnityEngine;

// 1. IState �������̽�
public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

// 2. ���� Ŭ���� ����
public class IdleState : IState
{
    private MonoBehaviour owner;

    public IdleState(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        Debug.Log(""Idle ���� ����"");
    }

    public void Execute()
    {
        // ��: Idle ���¿��� Ư���� ���� ����
    }

    public void Exit()
    {
        Debug.Log(""Idle ���� ����"");
    }
}

public class MoveState : IState
{
    private MonoBehaviour owner;

    public MoveState(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        Debug.Log(""Move ���� ����"");
    }

    public void Execute()
    {
        // ��: Move ���¿��� ������ �¿� �̵� ����
        float speed = 3f;
        float move = speed * Time.deltaTime;
        owner.transform.Translate(move, 0, 0);
    }

    public void Exit()
    {
        Debug.Log(""Move ���� ����"");
    }
}

// 3. FSM Ŭ����
public class FSM
{
    private IState currentState;

    public void ChangeState(IState newState)
    {
        if(currentState != null)
            currentState.Exit();

        currentState = newState;

        if(currentState != null)
            currentState.Enter();
    }

    public void Update()
    {
        if(currentState != null)
            currentState.Execute();
    }
}

// 4. MonoBehaviour ����
public class FSMExample : MonoBehaviour
{
    private FSM fsm;

    private IdleState idleState;
    private MoveState moveState;

    void Start()
    {
        fsm = new FSM();

        idleState = new IdleState(this);
        moveState = new MoveState(this);

        fsm.ChangeState(idleState);
    }

    void Update()
    {
        // �����̽��� ������ Move ���·� ��ȯ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fsm.ChangeState(moveState);
        }
        // I Ű ������ Idle ���·� ��ȯ
        else if (Input.GetKeyDown(KeyCode.I))
        {
            fsm.ChangeState(idleState);
        }

        fsm.Update();
    }
}
";

    private static readonly string fsmDescription =
@"===== FSM ����Ƽ C# ��ũ��Ʈ ���� =====

1. IState �������̽�
- ���º��� Enter(), Execute(), Exit() �޼��� ���� �ʿ�

2. IdleState, MoveState
- ���� Ŭ������ MonoBehaviour �ν��Ͻ�(������Ʈ) ������ �޾� ���� ���� ����
- MoveState�� �� ������ ������Ʈ�� ���������� �̵���Ŵ

3. FSM Ŭ����
- ���� ���� ���� �� ���� ��ȯ ��� ����

4. FSMExample MonoBehaviour
- ���� ������Ʈ�� ���̸� �ڵ����� FSM �ʱ�ȭ �� ����
- �����̽��� ������ Move ����, I ������ Idle ���·� ��ȯ��

����:
- ������ FSMExample.cs�� ������Ʈ �� ���ϴ� ������ �Ӵϴ�.
- �� �� �� ���� ������Ʈ�� FSMExample ������Ʈ �߰� �� �����ϼ���.
- ���� ��ȯ�� ������ �ٷ� Ȯ���� �� �ֽ��ϴ�.
";

    [MenuItem(menuName)]
    public static void CreateFSMExample()
    {
        string folderPath = "Assets";

        if (Selection.activeObject != null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (File.Exists(path))
                folderPath = Path.GetDirectoryName(path);
            else if (Directory.Exists(path))
                folderPath = path;
        }

        string scriptFullPath = Path.Combine(folderPath, scriptFileName);
        string textFullPath = Path.Combine(folderPath, textFileName);

        if (File.Exists(scriptFullPath))
        {
            if (!EditorUtility.DisplayDialog("����� Ȯ��", $"{scriptFileName} ������ �̹� �����մϴ�. ����ðڽ��ϱ�?", "��", "�ƴϿ�"))
            {
                return;
            }
        }

        if (File.Exists(textFullPath))
        {
            if (!EditorUtility.DisplayDialog("����� Ȯ��", $"{textFileName} ������ �̹� �����մϴ�. ����ðڽ��ϱ�?", "��", "�ƴϿ�"))
            {
                return;
            }
        }

        File.WriteAllText(scriptFullPath, fsmCode);
        File.WriteAllText(textFullPath, fsmDescription);
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();

        var scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(scriptFullPath);
        var textAsset = AssetDatabase.LoadAssetAtPath<Object>(textFullPath);

        Selection.objects = new Object[] { scriptAsset, textAsset };

        Debug.Log($"'{scriptFileName}' �� '{textFileName}' ������ �����Ǿ����ϴ�.");
    }
}
