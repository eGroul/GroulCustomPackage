using System.Collections;
using UnityEngine;

public class FSM_PlayerController : MonoBehaviour
{
    // 플레이어의 상태를 나타내는 열거형입니다.
    private enum PlayerState
    {
        Idle,     // 대기 상태
        Walking,  // 걷는 상태
        StartJump, // 점프 한 상태
        Jumping   // 점프 중 상태
    }
    [Header("플레이어 상태")]
    [Tooltip("현재 플레이어의 상태입니다.")]
    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    [Header("플레이어 이동 관련 설정")]
    [Tooltip("플레이어의 이동 속도입니다.")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("플레이어가 점프할 때 적용되는 힘입니다.")]
    [SerializeField] private float jumpForce = 7f;

    [Header("접지(Raycast) 관련 설정")]
    [Tooltip("바닥으로 인식할 레이어를 설정합니다.")]
    [SerializeField] private LayerMask groundLayerMask;
    [Tooltip("바닥을 감지하는 Ray의 길이입니다.")]
    [SerializeField] private float groundLayLength = 0.7f;
    [Tooltip("플레이어가 점프 후, 착지 여부를 판단하는 코드를 실행하는 딜레이입니다.")]
    [SerializeField] private float goundDelay = 0.1f;

    private Rigidbody rb;

    private void Awake()
    {
        // Rigidbody 컴포넌트 참조 저장
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        // 시작 상태는 Idle
        ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        // 현재 상태에 따라 해당 동작 수행
        switch (currentState)
        {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Walking:
                HandleWalking();
                break;
            case PlayerState.StartJump:
                StartCoroutine(HandleStartJump());
                break;
        }
    }

    // 상태를 변경하는 함수
    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
    }

    // 대기 상태 로직
    private void HandleIdle()
    {
        // 이동 입력이 있을 경우 걷기 상태로 전환
        if (ReturnGetAxis() != Vector3.zero)
        {
            ChangeState(PlayerState.Walking);
        }
        // 점프 키 입력 시 점프 상태로 전환
        else if (Input.GetButtonDown("Jump"))
        {
            ChangeState(PlayerState.StartJump);
        }
    }

    // 걷기 상태 로직
    private void HandleWalking()
    {
        Vector3 moveDir = ReturnGetAxis();

        // 대각선 이동 시 속도 보정을 위해 정규화
        if (moveDir.magnitude > 1)
            moveDir.Normalize();

        // 입력이 있을 경우 이동
        if (moveDir.magnitude > 0f)
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // 이동이 멈췄을 경우 대기 상태로 전환
            ChangeState(PlayerState.Idle);
        }

        // 점프 키 입력 시 점프 상태로 전환
        if (Input.GetButtonDown("Jump"))
        {
            ChangeState(PlayerState.StartJump);
        }
    }

    private IEnumerator HandleStartJump()
    {
        if (groundLayerMask != 0)
        {
            // 점프하고 있는 상태로 변경
            ChangeState(PlayerState.Jumping);

            // 접지 상태일 경우에만 점프력 적용
            if (Physics.Raycast(transform.position, Vector3.down, groundLayLength, groundLayerMask))
                rb.AddForce(Vector3.up * jumpForce + ReturnGetAxis().normalized * moveSpeed, ForceMode.Impulse);

            yield return new WaitForSeconds(goundDelay);

            while (Abs(rb.linearVelocity.y) > 0.1f || Physics.Raycast(transform.position, Vector3.down, groundLayLength, groundLayerMask) == false)
            {
                // Ray 시각화 (씬 뷰 전용)
                Debug.DrawRay(transform.position, Vector3.down * groundLayLength, Color.green, 0.1f);
                yield return null;
            }
            // 접지하면 바로 대기 상태로 복귀
            ChangeState(PlayerState.Idle);
        }
        else
        {
            Debug.LogWarning("groundLayerMask 가 nothing 이어서 점프를 못합니다.");
            ChangeState(PlayerState.Idle);
        }
    }
    private float Abs(float value)
    {
        return value < 0 ? -value : value;
    }
    // 키보드 입력으로부터 이동 방향을 반환하는 함수
    private Vector3 ReturnGetAxis()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }
}
