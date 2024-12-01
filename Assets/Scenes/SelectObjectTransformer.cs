using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;

public class SelectObjectTransformer : MonoBehaviour
{
    [SerializeField] private string _selectableTag = "Selectable";
    [SerializeField] private string _runtimeTransformHandleTag = "RuntimeTransformHandle";

    private Camera _mainCamera;
    private EventSystem _eventSystem;

    private Transform _selection;
    private RaycastHit _raycastHit;
    private GameObject _runtimeTransformGo;
    private RuntimeTransformHandle _runtimeTransformHandle;

    private void Start()
    {
        _mainCamera = Camera.main;
        _eventSystem = EventSystem.current;

        _runtimeTransformGo = new GameObject
        {
            transform =
            {
                parent = transform
            },
            tag = _runtimeTransformHandleTag
        };
        _runtimeTransformHandle = _runtimeTransformGo.AddComponent<RuntimeTransformHandle>();
        _runtimeTransformHandle.SetHandleMode(HandleType.POSITION);
        _runtimeTransformHandle.space = HandleSpace.WORLD;
        _runtimeTransformHandle.autoScale = true;
        _runtimeTransformHandle.autoScaleFactor = 1.0f;
        _runtimeTransformGo.SetActive(false);
    }

    private void Update()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (RuntimeTransformHandle.GetPointerDown() && !_eventSystem.IsPointerOverGameObject())
        {
            ApplyParentTagToChildren(_runtimeTransformGo);

            if (Physics.Raycast(ray, out _raycastHit))
            {
                if (_raycastHit.transform.CompareTag(_runtimeTransformHandleTag))
                {
                }
                else if (_raycastHit.transform.CompareTag(_selectableTag))
                {
                    _selection = _raycastHit.transform;
                    _runtimeTransformHandle.target = _selection;
                    _runtimeTransformGo.SetActive(true);
                }
                else
                {
                    if (_selection)
                    {
                        _selection = null;
                        _runtimeTransformGo.SetActive(false);
                    }
                }
            }
            else
            {
                if (_selection)
                {
                    _selection = null;
                    _runtimeTransformGo.SetActive(false);
                }
            }
        }

        if (!_runtimeTransformGo.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            _runtimeTransformHandle.SetHandleMode(HandleType.POSITION);
            _runtimeTransformHandle.space = HandleSpace.WORLD;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _runtimeTransformHandle.SetHandleMode(HandleType.ROTATION);
            _runtimeTransformHandle.space = HandleSpace.LOCAL;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _runtimeTransformHandle.SetHandleMode(HandleType.SCALE);
            _runtimeTransformHandle.space = HandleSpace.LOCAL;
        }
    }

    private void ApplyParentTagToChildren(GameObject parentGo)
    {
        foreach (Transform child in parentGo.transform)
        {
            var childGo = child.gameObject;
            childGo.tag = parentGo.tag;
            ApplyParentTagToChildren(childGo);
        }
    }
}