using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngineInternal;

public class CrossHairLookAt : NetworkBehaviour
{
    [SerializeField] private GameObject crosshair;  // 크로스헤어
    [SerializeField] private Transform objectToRotate;  // 공격 지점 오브젝트
    [SerializeField] private float distance = 10f;  // 오브젝트가 크로스헤어를 기준으로 볼 거리
    [SerializeField] private Vector3 endPoint; // 끝 지점
    [SerializeField] private float rayDistance = 1000f;  // Ray가 발사될 거리
    [SerializeField] private Vector3 dir;
    [SerializeField] private Vector3 groundHitPosition;

    private Vector3 direction;  // 오브젝트가 바라보는 방향    
    private bool isGrounding = false;
    private float endPointDistance;

    private void Awake()
    {
        crosshair = FindObjectOfType<AimController>().gameObject;
    }


    void Update()
    {
        if (crosshair != null)
        {
            // 크로스헤어의 RectTransform을 참조
            RectTransform crosshairRect = crosshair.GetComponent<RectTransform>();

            // Canvas 상에서 크로스헤어의 위치를 참조
            Vector3 crosshairScreenPos = crosshairRect.position;

            // 화면 좌표를 월드 좌표로 변환 
            Vector3 crosshairWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(crosshairScreenPos.x, crosshairScreenPos.y, distance));

            // 오브젝트가 크로스헤어를 바라보게 방향을 계산
            direction = crosshairWorldPos - objectToRotate.position;

            // 오브젝트를 크로스헤어 방향으로 회전
            objectToRotate.rotation = Quaternion.LookRotation(direction);

            // 끝 지점 참조
            endPoint = objectToRotate.position + direction.normalized * distance;

            EndPointUntilRay();

            if (!isGrounding)
            {
                // Ray 발사 로직 추가
                ShootRayFromEndPoint();
            }
        }
    }

    public void CameraReceive(GameObject cameraObject)
    {
        crosshair = cameraObject;
    }

    private void EndPointUntilRay()
    {
        Vector3 rayDirection = endPoint - objectToRotate.position;
        Ray ray = new Ray(objectToRotate.position, rayDirection.normalized);

        // RaycastHit 구조체로 정보를 받음
        RaycastHit hit;

        // Ray 발사, 거리와 태그 체크
        if (Physics.Raycast(ray, out hit, rayDirection.magnitude))
        {
            // Ground 태그를 가진 오브젝트에 맞았을 경우
            if (hit.collider.CompareTag("Ground"))
            {
                // 충돌 위치를 가져오도록
                groundHitPosition = hit.point;
                isGrounding = true;
            }

            // Todo: 플레이어로 수정 필요
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("enemy success");
                groundHitPosition = hit.collider.transform.position;
                isGrounding = true;
            }

            //if (hit.collider.gameObject.GetComponentInParent<NetworkObject>().CompareTag("Enemy"))
            //{
                
            //}
        }
        else
        {
            isGrounding = false;
        }
    }

    private void ShootRayFromEndPoint()
    {
        // Ray를 endPoint에서 -y축 방향으로 발사
        Ray ray = new Ray(endPoint, Vector3.down);

        // RaycastHit 구조체로 정보를 받음
        RaycastHit hit;

        // Ray 발사, 거리와 태그 체크
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Ground 태그를 가진 오브젝트에 맞았을 경우
            if (hit.collider.CompareTag("Ground"))
            {
                // 충돌 위치를 가져와서 원하는 작업을 수행
                groundHitPosition = hit.point;
            }
        }
    }

    public Vector3 GroundHitPositionTransmission()
    {
        return groundHitPosition;
    }

    public void EndPointDistanceChanged(float newDistance)
    {
        endPointDistance = newDistance;
    }

    private void OnDrawGizmos()
    {
        if (objectToRotate != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(objectToRotate.position, objectToRotate.position + direction.normalized * 5f);

            Gizmos.DrawRay(objectToRotate.position, direction.normalized * 5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(endPoint, Vector3.down * rayDistance);
        }
    }
}
