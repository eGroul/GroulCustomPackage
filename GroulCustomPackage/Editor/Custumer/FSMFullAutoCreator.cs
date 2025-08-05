using UnityEditor;
using UnityEngine;
using System.IO;

public class FSMFullAutoCreator
{
    private const string scriptFileName = "FSMExample.cs";
    private const string textFileName = "FSM_Description.txt";
    private const string menuName = "Assets/Create/FSM Complete Example";

    private static readonly string fsmCode = @"using UnityEngine;

// 1. IState 인터페이스
public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

// 2. 상태 클래스 예시
public class IdleState : IState
{
    private MonoBehaviour owner;

    public IdleState(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        Debug.Log(""Idle 상태 진입"");
    }

    public void Execute()
    {
        // 예: Idle 상태에서 특별한 동작 없음
    }

    public void Exit()
    {
        Debug.Log(""Idle 상태 종료"");
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
        Debug.Log(""Move 상태 진입"");
    }

    public void Execute()
    {
        // 예: Move 상태에서 간단히 좌우 이동 구현
        float speed = 3f;
        float move = speed * Time.deltaTime;
        owner.transform.Translate(move, 0, 0);
    }

    public void Exit()
    {
        Debug.Log(""Move 상태 종료"");
    }
}

// 3. FSM 클래스
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

// 4. MonoBehaviour 예제
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
        // 스페이스바 누르면 Move 상태로 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fsm.ChangeState(moveState);
        }
        // I 키 누르면 Idle 상태로 전환
        else if (Input.GetKeyDown(KeyCode.I))
        {
            fsm.ChangeState(idleState);
        }

        fsm.Update();
    }
}
";

    private static readonly string fsmDescription =
@"===== FSM 유니티 C# 스크립트 설명 =====

1. IState 인터페이스
- 상태별로 Enter(), Execute(), Exit() 메서드 구현 필요

2. IdleState, MoveState
- 상태 클래스는 MonoBehaviour 인스턴스(오브젝트) 참조를 받아 동작 제어 가능
- MoveState는 매 프레임 오브젝트를 오른쪽으로 이동시킴

3. FSM 클래스
- 현재 상태 관리 및 상태 전환 기능 제공

4. FSMExample MonoBehaviour
- 게임 오브젝트에 붙이면 자동으로 FSM 초기화 및 실행
- 스페이스바 누르면 Move 상태, I 누르면 Idle 상태로 전환됨

사용법:
- 생성된 FSMExample.cs를 프로젝트 내 원하는 폴더에 둡니다.
- 씬 내 빈 게임 오브젝트에 FSMExample 컴포넌트 추가 후 실행하세요.
- 상태 전환과 동작을 바로 확인할 수 있습니다.
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
            if (!EditorUtility.DisplayDialog("덮어쓰기 확인", $"{scriptFileName} 파일이 이미 존재합니다. 덮어쓰시겠습니까?", "예", "아니오"))
            {
                return;
            }
        }

        if (File.Exists(textFullPath))
        {
            if (!EditorUtility.DisplayDialog("덮어쓰기 확인", $"{textFileName} 파일이 이미 존재합니다. 덮어쓰시겠습니까?", "예", "아니오"))
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

        Debug.Log($"'{scriptFileName}' 와 '{textFileName}' 파일이 생성되었습니다.");
    }
}
