using SmashSlimes;
using UniRx;
using UnityEngine;

public class MeshChange : MonoBehaviour
{
    [SerializeField] private GroundType _groundType = GroundType.Center;
    [SerializeField] private Mesh[] _mesh = new Mesh[21];
    private int _greatCounter;
    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshFilter = gameObject.GetComponent<MeshFilter>();

        var gameControllerObject = GameObject.FindWithTag("GameController");
        var gameControllerScript = gameControllerObject.GetComponent<GameController>();

        gameControllerScript.SlimeValueIObservable
            .Subscribe(x =>
            {
                if (x == 1)
                {
                    _greatCounter++;
                    if (_groundType == GroundType.Center)
                    {
                        if (_greatCounter <= 16) _meshFilter.mesh = _mesh[_greatCounter];
                    }
                    else if (_groundType == GroundType.Left)
                    {
                        if (_greatCounter == 17 && _greatCounter == 18) _meshFilter.mesh = _mesh[_greatCounter];
                    }
                    else if (_groundType == GroundType.Right)
                    {
                        if (_greatCounter == 19 && _greatCounter == 20) _meshFilter.mesh = _mesh[_greatCounter];
                    }
                }
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.Continue)
            .Subscribe(_ =>
            {
                _greatCounter = 0;
                _meshFilter.mesh = _mesh[_greatCounter];
            })
            .AddTo(this);
    }
}