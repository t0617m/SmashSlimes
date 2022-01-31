using System;
using UniRx;
using UnityEngine;

public class SlimeCreate : MonoBehaviour
{
    [SerializeField] private GameObject[] _slimeObject = new GameObject[10];
    private readonly Vector3 _createPosition = new Vector3(-0.5f, 0.4f, 7.5f);
    private readonly Quaternion _createRotation = Quaternion.Euler(new Vector3(0, -45, 0));

    private readonly Subject<GameObject> _slimeCreatedSubject = new Subject<GameObject>();

    public IObservable<GameObject> SlimeCreatedIObservable => _slimeCreatedSubject;

    private void Start()
    {
        var _gameControllerObject = GameObject.FindWithTag("GameController");
        var _gameControllerScript = _gameControllerObject.GetComponent<GameController>();

        _gameControllerScript.SlimeCreateIObservable
            .Subscribe(stimeType => Create(stimeType))
            .AddTo(this);
    }

    private void Create(int stimeType)
    {
        var instantiateObject = Instantiate(_slimeObject[stimeType], _createPosition, _createRotation);
        var generationCheck = false;
        if (instantiateObject.tag != "Slime")
            while (!generationCheck)
            {
                if (instantiateObject.tag == "Slime") generationCheck = true;
                instantiateObject = Instantiate(_slimeObject[stimeType], _createPosition, _createRotation);
            }

        instantiateObject.SetActive(true);
        _slimeCreatedSubject.OnNext(instantiateObject);
    }
}