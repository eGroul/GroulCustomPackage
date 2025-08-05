using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class FSM_PlayerController : MonoBehaviour
{
    // �÷��̾��� ���¸� ��Ÿ���� �������Դϴ�.
    private enum PlayerState
    {
        Idle,     // ��� ����
        Walking,  // �ȴ� ����
        StartJump, // ���� �� ����
        Jumping   // ���� �� ����
    }
    [Header("�÷��̾� ����")]
    [Tooltip("���� �÷��̾��� �����Դϴ�.")]
    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    [Header("�÷��̾� �̵� ���� ����")]
    [Tooltip("�÷��̾��� �̵� �ӵ��Դϴ�.")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("�÷��̾ ������ �� ����Ǵ� ���Դϴ�.")]
    [SerializeField] private float jumpForce = 7f;

    [Header("����(Raycast) ���� ����")]
    [Tooltip("�ٴ����� �ν��� ���̾ �����մϴ�.")]
    [SerializeField] private LayerMask groundLayerMask;
    [Tooltip("�ٴ��� �����ϴ� Ray�� �����Դϴ�.")]
    [SerializeField] private float groundLayLength = 0.7f;
    [Tooltip("�÷��̾ ���� ��, ���� ���θ� �Ǵ��ϴ� �ڵ带 �����ϴ� �������Դϴ�.")]
    [SerializeField] private float goundDelay = 0.1f;

    private Rigidbody rb;

    private void Awake()
    {
        // Rigidbody ������Ʈ ���� ����
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        // ���� ���´� Idle
        ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        // ���� ���¿� ���� �ش� ���� ����
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

    // ���¸� �����ϴ� �Լ�
    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
    }

    // ��� ���� ����
    private void HandleIdle()
    {
        // �̵� �Է��� ���� ��� �ȱ� ���·� ��ȯ
        if (ReturnGetAxis() != Vector3.zero)
        {
            ChangeState(PlayerState.Walking);
        }
        // ���� Ű �Է� �� ���� ���·� ��ȯ
        else if (Input.GetButtonDown("Jump"))
        {
            ChangeState(PlayerState.StartJump);
        }
    }

    // �ȱ� ���� ����
    private void HandleWalking()
    {
        Vector3 moveDir = ReturnGetAxis();

        // �밢�� �̵� �� �ӵ� ������ ���� ����ȭ
        if (moveDir.magnitude > 1)
            moveDir.Normalize();

        // �Է��� ���� ��� �̵�
        if (moveDir.magnitude > 0f)
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // �̵��� ������ ��� ��� ���·� ��ȯ
            ChangeState(PlayerState.Idle);
        }

        // ���� Ű �Է� �� ���� ���·� ��ȯ
        if (Input.GetButtonDown("Jump"))
        {
            ChangeState(PlayerState.StartJump);
        }
    }

    private IEnumerator HandleStartJump()
    {
        if (groundLayerMask != 0)
        {
            // �����ϰ� �ִ� ���·� ����
            ChangeState(PlayerState.Jumping);

            // ���� ������ ��쿡�� ������ ����
            if (Physics.Raycast(transform.position, Vector3.down, groundLayLength, groundLayerMask))
                rb.AddForce(Vector3.up * jumpForce + ReturnGetAxis().normalized * moveSpeed, ForceMode.Impulse);

            yield return new WaitForSeconds(goundDelay);

            while (math.abs(rb.linearVelocity.y) > 0.1f || Physics.Raycast(transform.position, Vector3.down, groundLayLength, groundLayerMask) == false)
            {
                // Ray �ð�ȭ (�� �� ����)
                Debug.DrawRay(transform.position, Vector3.down * groundLayLength, Color.green, 0.1f);
                yield return null;
            }
            // �����ϸ� �ٷ� ��� ���·� ����
            ChangeState(PlayerState.Idle);
        }
        else
        {
            Debug.LogWarning("groundLayerMask �� nothing �̾ ������ ���մϴ�.");
            ChangeState(PlayerState.Idle);
        }
    }

    // Ű���� �Է����κ��� �̵� ������ ��ȯ�ϴ� �Լ�
    private Vector3 ReturnGetAxis()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }
}
