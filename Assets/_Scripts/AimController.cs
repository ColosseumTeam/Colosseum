using Fusion;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [SerializeField] private CrossHairLookAt crossHairEvent;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minYPosition = -450f;
    [SerializeField] private float maxYPosition = 450f;
    [SerializeField] private float oneGroundRangeSize = 2f; // 지면에 표시할 사각형의 크기
    [SerializeField] private LayerMask groundLayer; // 지면 레이어

    private Vector3 hitPoint;

    private float yPosition = 0f;
    private RectTransform rect;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private NetworkObject pg;
    private LineRenderer lineRenderer;

    private bool isSkillReady;
    private float isSkillState;    

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
             
        //mainCamera = Camera.main;

        // LineRenderer를 현재 오브젝트에 추가
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // LineRenderer의 기본 설정
        lineRenderer.positionCount = 5; // 사각형을 그리기 위해 5개의 점이 필요 (시작점과 끝점이 동일해야 함)
        lineRenderer.startWidth = 0.05f; // 선의 시작 두께
        lineRenderer.endWidth = 0.05f; // 선의 끝 두께
        lineRenderer.useWorldSpace = true; // 월드 좌표계 사용

        // 선의 색상 설정 (여기서는 빨간색)
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // 처음엔 LineRenderer를 비활성화하여 보이지 않게 함
        lineRenderer.enabled = false;

        // 마우스 커서를 숨기고 고정시킴
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnAimMove(InputValue value)
    {
        float inputY = value.Get<Vector2>().y;
        yPosition += inputY * moveSpeed;
        yPosition = Mathf.Clamp(yPosition, minYPosition, maxYPosition);
        rect.anchoredPosition = new Vector2(0, yPosition);
    }

    private void Update()
    {
        if (isSkillReady)
        {
            if (isSkillState == 0)
            {
                DrawGroundIndicator();
            }

            else if (isSkillState == 3)
            {
                hitPoint = crossHairEvent.GroundHitPositionTransmission();
            }

            HitPointPositionSave();
        }
    }

    public void SkillReadyAcitve(float newSkillState)
    {
        // 스킬 준비
        isSkillReady = true;

        // 스킬 타입 파악
        isSkillState = newSkillState;

        // LineRenderer를 활성화하여 범위를 보이도록 함
        // lineRenderer.enabled = true;
    }

    public void AimSkillReadyNonActive()
    {
        // 스킬 사용 후 스킬 준비 해제
        isSkillReady = false;

        // LineRenderer를 비활성화하여 사각형이 보이지 않게 함
        lineRenderer.enabled = false;
    }

    private void DrawGroundIndicator()
    {
        // 이전에 그려진 모양을 지우기 위해 점 개수를 0으로 설정
        lineRenderer.positionCount = 0;

        hitPoint = crossHairEvent.GroundHitPositionTransmission();

        // 사각형의 네 모서리 좌표 계산
        Vector3 topLeft = hitPoint + new Vector3(-oneGroundRangeSize / 2, 0, -oneGroundRangeSize / 2);
        Vector3 topRight = hitPoint + new Vector3(oneGroundRangeSize / 2, 0, -oneGroundRangeSize / 2);
        Vector3 bottomLeft = hitPoint + new Vector3(-oneGroundRangeSize / 2, 0, oneGroundRangeSize / 2);
        Vector3 bottomRight = hitPoint + new Vector3(oneGroundRangeSize / 2, 0, oneGroundRangeSize / 2);

        // 새로 그리기 위해 점 개수 설정
        lineRenderer.positionCount = 5; // 사각형을 그리기 위해 5개의 점이 필요

        // LineRenderer를 사용해 사각형 그리기
        lineRenderer.SetPosition(0, topLeft);
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, bottomRight);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, topLeft); // 마지막 점은 시작점과 동일해야 사각형이 완성됨
    }

    private void HitPointPositionSave()
    {
        Ray ray = mainCamera.ScreenPointToRay(rect.position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer) && (isSkillState != 0 || isSkillState != 3))
        {
            hitPoint = hit.point;
        }
    }

    //public Vector3 GetGroundIndicatorCenter()
    //{
    //    if(isSkillState == 0 || isSkillState == 3)
    //    {
    //        Debug.Log(isSkillState);
    //        return crossHairEvent.GroundHitPositionTransmission();
    //    }

    //    return hitPoint;
    //}

    // 플레이어가 보유한 카메라를 전달 받는 메서드
    public void PlayerObjectTransmission(Camera mainCam)
    {
        //pg = newPg;
        
        //if(pg == null)
        //{
        //    Debug.Log("pg not have");
        //    return;
        //}

        //foreach(Transform child in pg.transform)
        //{
        //    Camera cam = child.GetComponentInChildren<Camera>();

        //    if (cam != null)
        //    {
        //        mainCamera = cam;
        //    }
        //}
        mainCamera = mainCam;
    }
}
