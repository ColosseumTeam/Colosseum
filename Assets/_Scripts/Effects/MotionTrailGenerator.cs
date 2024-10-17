using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MotionTrailGenerator : MonoBehaviour
{
    public bool isOn { get; private set; }

    /// <summary>
    /// 잔상 
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DummyMesh : MonoBehaviour
    {
        public MotionTrailGenerator generator { get; set; }
        public float lifeTime { get; set; }
        public MeshFilter meshFilter { get; private set; }
        public MeshRenderer meshRenderer { get; private set; }
        private float _timer;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (_timer < 0)
                Off();
            else
                _timer -= Time.deltaTime;
        }

        public void On()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _timer = lifeTime;
            transform.parent = null;
            meshRenderer.enabled = true;
        }

        public void Off()
        {
            generator.Return(this);
            transform.parent = generator.transform;
            meshRenderer.enabled = false;
        }
    }

    private Stack<DummyMesh> _stack;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private Material _material;
    [Range(1f, 30f)]
    [SerializeField] private float _rate; // 초당 잔상 수
    [Range(0.01f, 20f)]
    [SerializeField] private float _lifeTime; // 잔상 생명주기
    [Range(1, 100)]
    [SerializeField] private int _capacity; // 잔상 풀링 용량
    private float _timer;

    private void Awake()
    {
        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _stack = new Stack<DummyMesh>(_capacity);

        for (int i = 0; i < _capacity; i++)
        {
            DummyMesh dummyMesh = new GameObject("MotionTrailDummyMesh").AddComponent<DummyMesh>();
            dummyMesh.generator = this;
            dummyMesh.lifeTime = _lifeTime;
            dummyMesh.meshRenderer.material = _material;
            _stack.Push(dummyMesh);
        }
    }

    private void Start()
    {
        Off(); // Just for testing.
    }

    private void Update()
    {
        if (isOn)
            Generate();
    }

    /// <summary>
    /// 풀에서 현재 포즈의 잔상을 가져옴
    /// </summary>
    private DummyMesh Get()
    {
        DummyMesh dummyMesh = _stack.Pop();
        _skinnedMeshRenderer.BakeMesh(dummyMesh.meshFilter.mesh);
        dummyMesh.On();
        return dummyMesh;
    }

    /// <summary>
    /// 잔상을 풀로 반환
    /// </summary>
    private void Return(DummyMesh dummyMesh)
    {
        _stack.Push(dummyMesh);
    }

    public void On() => isOn = true;
    
    public void Off() => isOn = false;

    private void Generate()
    {
        if (_timer < 0)
        {
            if (_stack.Count > 0)
            {
                Get();
                _timer = 1.0f / _rate;
            }
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }
}
