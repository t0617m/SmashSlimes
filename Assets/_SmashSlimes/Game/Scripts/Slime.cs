using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] private SlimeType _slimeType = SlimeType.Normal;
    [SerializeField] private float _coefficient = 2.8f;

    [SerializeField] private GameObject _smokeParticle;
    [SerializeField] private GameObject _chestAppearParticle;
    [SerializeField] private GameObject _jumpParticle;
    [SerializeField] private GameObject _destructionParticle;
    [SerializeField] private GameObject _destructionParticle2;
    [SerializeField] private GameObject _deadParticle;
    [SerializeField] private GameObject _chargeParticle;

    [SerializeField] [Range(0.0f, 1.0f)] private float _timingAdjustment1;
    [SerializeField] [Range(0.0f, 1.0f)] private float _timingAdjustment2;
    [SerializeField] [Range(0.0f, 1.0f)] private float _timingAdjustment3;

    [SerializeField] private AudioClip _chargeSound;
    [SerializeField] private AudioClip _landingSound;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _kickSound;
    [SerializeField] private AudioClip _explosionSound; //爆発
    [SerializeField] private AudioClip _extendSound; //伸びる
    [SerializeField] private AudioClip _middleHitSound;
    private Animator _animator;
    private AudioSource _audioSource;
    private CancellationTokenSource _cancellationTokenSource;
    private ParticleSystem _chargeParticleSystem;
    private ParticleSystem _chestAppearParticleSystem;
    private ParticleSystem _deadParticleSystem;
    private ParticleSystem _destructionParticleSystem;
    private ParticleSystem _destructionParticleSystem2;
    private readonly Vector3[] _endJumpPath = {new Vector3(-6.0f, 1.5f, 8.8f), new Vector3(-7.5f, 0.4f, 8.8f)};


    private readonly Vector3 _hitTopPosition = new Vector3(-3.0f, 4.5f, -10.0f);
    private readonly Vector3 _hitUnderPosition = new Vector3(-3.0f, 1.5f, -13.0f);
    private bool _isBad;
    private bool _isErase;
    private bool _isHit;
    private bool _isMidleHit;

    private bool _isMiss;

    private bool _isMove;
    private bool _isSyoukyo;
    private ParticleSystem _jumpParticleSystem;
    private readonly Vector3 _landingScale = new Vector3(25f, 15.0f, 25.0f);

    private readonly Vector3[] _mainJumpPath =
        {new Vector3(-2.7f, 1.4f, 8.8f), new Vector3(-3.1f, 1.4f, 8.8f), new Vector3(-3.7f, 0.4f, 8.8f)};

    private readonly Vector3 _maxScale = new Vector3(35.0f, 5.0f, 35.0f);

    private readonly Vector3 _middleHitTopPosition = new Vector3(-2.0f, 3.5f, -3.0f);
    private readonly Vector3 _middleHitUnderPosition = new Vector3(-2.0f, 1.0f, -7.5f);
    private readonly Vector3 _middleScale = new Vector3(30.0f, 7.5f, 30.0f);
    private float _missTime;

    private readonly Vector3 _missTopPosition = new Vector3(1f, 2.0f, 3.0f);
    private readonly Vector3 _missUnderPosition = new Vector3(2.0f, 0.4f, -1.0f);
    private Vector3 _myPosition = new Vector3(0, 0, 0);
    private readonly Vector3 _normalScale = new Vector3(20.0f, 20.0f, 20.0f);
    private Sequence _sequence;

    private readonly Subject<int> _slimeCountSubject = new Subject<int>();

    private readonly Subject<float> _slimeValueSubject = new Subject<float>();
    private readonly Vector3 _slimScale = new Vector3(15f, 35.0f, 15.0f);
    private ParticleSystem _smokeParticleSystem;

    private readonly Vector3[] _startJumpPath = {new Vector3(-1.2f, 1.5f, 8.2f), new Vector3(-1.8f, 0.4f, 8.8f)};
    private CancellationToken _token;

    public IObservable<float> SlimeValueIObservable => _slimeValueSubject;

    public IObservable<int> SlimeCountIObservable => _slimeCountSubject;

    private void Awake()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        _myPosition = transform.position;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0.25f;

        _sequence = DOTween.Sequence();

        var _slimeCreateObject = GameObject.FindWithTag("SlimeCreate");
        var _slimeCreateScript = _slimeCreateObject.GetComponent<SlimeCreate>();

        var _playerLeftToeBaseObject = GameObject.FindWithTag("LeftToeBase");
        var _playerLeftToeBaseScript = _playerLeftToeBaseObject.GetComponent<PlayerLeftToeBase>();

        var hitColliderRootObject = GameObject.FindWithTag("HitCollider");
        var hitColliderRootScript = hitColliderRootObject.GetComponent<HitColliderRoot>();

        hitColliderRootScript.DestructionSlimeIObservable
            .Subscribe(hitObject => FuruiSlimeSyoukyo(hitObject))
            .AddTo(this);

        hitColliderRootScript.HitPointIObservable
            .Subscribe(hitColliderType => HitPointReceive(hitColliderType))
            .AddTo(this);

        _animator = GetComponent<Animator>();

        GetParticleComponent();

        _cancellationTokenSource = new CancellationTokenSource();
        _token = _cancellationTokenSource.Token;

        if (_slimeType == SlimeType.Normal) JumpProcess();
        if (_slimeType == SlimeType.Fast) JumpProcessFast();
        if (_slimeType == SlimeType.Metal) JumpProcessMetal();
    }

    private void Update()
    {
        //ミスした時逃げるのに使用
        if (_isMove) transform.Translate(Vector3.forward * Time.deltaTime * _coefficient);

        ThroughProcess();

        EraseCounter();
    }

    //パーティクルコンポーネントの取得
    private void GetParticleComponent()
    {
        _deadParticleSystem = _deadParticle.GetComponent<ParticleSystem>();
        _deadParticleSystem.Stop();

        _smokeParticleSystem = _smokeParticle.GetComponent<ParticleSystem>();
        _smokeParticleSystem.Stop();
        _smokeParticle.SetActive(false);

        _jumpParticleSystem = _jumpParticle.GetComponent<ParticleSystem>();
        _jumpParticleSystem.Stop();

        _destructionParticleSystem = _destructionParticle.GetComponent<ParticleSystem>();
        _destructionParticleSystem.Stop();

        _destructionParticleSystem2 = _destructionParticle2.GetComponent<ParticleSystem>();
        _destructionParticleSystem2.Stop();

        _chestAppearParticleSystem = _chestAppearParticle.GetComponent<ParticleSystem>();
        _chestAppearParticleSystem.Stop();

        if (_slimeType == SlimeType.Fast)
        {
            _chargeParticleSystem = _chargeParticle.GetComponent<ParticleSystem>();
            _chargeParticleSystem.Stop();
        }
    }

    //ノーマルスライムの移動アニメーション
    private void JumpProcess()
    {
        _sequence.Append(transform.DOScale(_normalScale, _timingAdjustment1)
            .OnComplete(() => _audioSource.PlayOneShot(_jumpSound)).SetLink(gameObject)); //タイミング調整1

        _sequence.Append(transform.DOLocalPath(_startJumpPath, 0.4f, PathType.CatmullRom)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(true);
                _chestAppearParticleSystem.Play();
                _audioSource.PlayOneShot(_landingSound);
            }).SetLink(gameObject));

        _sequence.Join(gameObject.transform.DORotate(new Vector3(0, -90, 0), 0.4f).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_landingScale, 0.2f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_normalScale, 0.2f).SetEase(Ease.OutSine).SetLink(gameObject));

        _sequence.Append(transform.DOScale(_normalScale, _timingAdjustment2) //タイミング調整用2
            .OnComplete(() =>
            {
                _audioSource.PlayOneShot(_extendSound);
                _slimeCountSubject.OnNext(2);
            })).SetLink(gameObject);

        _sequence.Append(transform.DOScale(_slimScale, 0.6f)
            .OnComplete(() => _slimeCountSubject.OnNext(1))).SetLink(gameObject);

        _sequence.Append(transform.DOScale(_middleScale, 0.3f)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(false);
                _jumpParticleSystem.Play();
                _audioSource.Stop();
                _audioSource.PlayOneShot(_jumpSound);
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_maxScale, 0.1f).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_maxScale, _timingAdjustment3)).SetLink(gameObject);

        _sequence.Append(transform.DOScale(_normalScale, 0.1f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Join(transform.DOLocalPath(_mainJumpPath, 0.5f, PathType.CatmullRom).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(true);
                _chestAppearParticleSystem.Play();
                _audioSource.PlayOneShot(_landingSound);
            }).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_middleScale, 0.3f).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _audioSource.Stop();
                _chestAppearParticle.SetActive(false);
                _jumpParticleSystem.Play();
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_maxScale, 0.2f)
            .OnComplete(() =>
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(_jumpSound);
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_normalScale, 0.1f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Join(transform.DOLocalPath(_endJumpPath, 0.4f, PathType.CatmullRom).SetLink(gameObject));
    }

    //ファストスライムの移動アニメーション
    private void JumpProcessFast()
    {
        _sequence.Append(transform.DOScale(_normalScale, _timingAdjustment1)
            .OnComplete(() => _audioSource.PlayOneShot(_jumpSound)).SetLink(gameObject)); //タイミング調整1

        _sequence.Append(transform.DOLocalPath(_startJumpPath, 0.4f, PathType.CatmullRom)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(true);
                _chestAppearParticleSystem.Play();
                _audioSource.PlayOneShot(_landingSound);
            }).SetLink(gameObject));

        _sequence.Join(gameObject.transform.DORotate(new Vector3(0, -90, 0), 0.4f).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_landingScale, 0.2f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_normalScale, 0.2f).SetEase(Ease.OutSine).SetLink(gameObject));
        //タイミング調整用2
        _sequence.Append(transform.DOScale(_normalScale, _timingAdjustment2)
            .OnComplete(() => { _audioSource.PlayOneShot(_extendSound); })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_landingScale, 0.4f).SetLink(gameObject));

        _sequence.Append(transform.DOScale(_slimScale, 0.4f)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(false);
                _chargeParticleSystem.Play();
                _audioSource.Stop();
                _slimeCountSubject.OnNext(2);
                _audioSource.PlayOneShot(_chargeSound);
            })).SetLink(gameObject);

        _sequence.Append(transform.DOScale(_middleScale, 0.9f).SetEase(Ease.InOutQuint)
            .OnComplete(() =>
            {
                _slimeCountSubject.OnNext(1);
                _chargeParticleSystem.Stop();
                _audioSource.Stop();
            })).SetLink(gameObject);

        _sequence.Append(transform.DOScale(_middleScale, 0.2f)
            .OnComplete(() =>
            {
                _jumpParticleSystem.Play();
                _audioSource.Stop();
                _audioSource.PlayOneShot(_jumpSound);
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_maxScale, 0.3f).SetLink(gameObject));

        _sequence.Append(transform.DOScale(_maxScale, _timingAdjustment3)).SetLink(gameObject); //タイミング調整用3

        _sequence.Append(transform.DOScale(_normalScale, 0.1f).SetEase(Ease.OutSine).SetLink(gameObject));

        _sequence.Join(transform.DOLocalPath(_mainJumpPath, 0.3f, PathType.CatmullRom).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _chargeParticle.SetActive(false);
                _chestAppearParticle.SetActive(true);
                _chestAppearParticleSystem.Play();
                _audioSource.PlayOneShot(_landingSound);
            }).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_middleScale, 0.3f).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(false);
                _jumpParticleSystem.Play();
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_maxScale, 0.2f)
            .OnComplete(() =>
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(_jumpSound);
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_normalScale, 0.1f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Join(transform.DOLocalPath(_endJumpPath, 0.4f, PathType.CatmullRom).SetLink(gameObject));
    }

    //メタルスライム移動アニメーション
    private void JumpProcessMetal()
    {
        _sequence.Append(transform.DOScale(_normalScale, _timingAdjustment1)
            .OnComplete(() => _audioSource.PlayOneShot(_jumpSound)).SetLink(gameObject)); //タイミング調整1

        _sequence.Append(transform.DOLocalPath(_startJumpPath, 0.4f, PathType.CatmullRom)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(true);
                _chestAppearParticleSystem.Play();
                _audioSource.PlayOneShot(_landingSound);
                _slimeCountSubject.OnNext(1);
            }).SetLink(gameObject));

        _sequence.Join(gameObject.transform.DORotate(new Vector3(0, -90, 0), 0.4f).SetLink(gameObject));

        _sequence.Append(transform.DOScale(_normalScale, _timingAdjustment2)).SetLink(gameObject); //タイミング調整用2

        _sequence.Append(transform.DOScale(_middleScale, 0.1f)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(false);
                _jumpParticleSystem.Play();
                _audioSource.Stop();
                _audioSource.PlayOneShot(_jumpSound);
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_maxScale, 0.1f).SetLink(gameObject));

        _sequence.Append(transform.DOScale(_maxScale, _timingAdjustment3)).SetLink(gameObject); //タイミング調整用3

        _sequence.Append(transform.DOScale(_normalScale, 0.1f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Join(transform.DOLocalPath(_mainJumpPath, 0.6f, PathType.CatmullRom).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(true);
                _chestAppearParticleSystem.Play();
                _audioSource.PlayOneShot(_landingSound);
            }).SetLink(gameObject));
        _sequence.Append(transform.DOScale(_middleScale, 0.3f).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _chestAppearParticle.SetActive(false);
                _jumpParticleSystem.Play();
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_maxScale, 0.2f)
            .OnComplete(() =>
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(_jumpSound);
            })).SetLink(gameObject);
        _sequence.Append(transform.DOScale(_normalScale, 0.1f).SetEase(Ease.OutSine).SetLink(gameObject));
        _sequence.Join(transform.DOLocalPath(_endJumpPath, 0.4f, PathType.CatmullRom).SetLink(gameObject));
    }

    //キックした時に前回登場していたスライムを消去
    private async void FuruiSlimeSyoukyo(GameObject hitObject)
    {
        var rootObject = hitObject.transform.root.gameObject;
        if (gameObject != rootObject && rootObject.transform.position != _myPosition)
            try
            {
                await UniTask.WaitUntil(() => _isSyoukyo, cancellationToken: _token);
                if (gameObject != null) Destruction();
            }
            catch
            {
            }
    }

    //プレイヤーがキックしたときにスライムに当たった場合の処理
    private async void HitPointReceive(HitColliderType hitColliderType)
    {
        try
        {
            var isWait = false;
            var timer = Observable.Timer(TimeSpan.FromSeconds(0.06f));
            timer.Subscribe(_ => isWait = true)
                .AddTo(this);
            await UniTask.WaitUntil(() => isWait, cancellationToken: _token);
            _sequence.Kill();
            _smokeParticle.SetActive(true);
            _smokeParticleSystem.Play();
            var topPosition = _hitTopPosition;
            var underPosition = _hitUnderPosition;

            switch (hitColliderType)
            {
                case HitColliderType.Great:
                    topPosition = _hitTopPosition;
                    underPosition = _hitUnderPosition;
                    _isHit = true;
                    break;
                case HitColliderType.Good:
                    topPosition = _middleHitTopPosition;
                    underPosition = _middleHitUnderPosition;
                    _isMidleHit = true;
                    break;
                case HitColliderType.Bad:
                    topPosition = _missTopPosition;
                    underPosition = _missUnderPosition;
                    _isMiss = true;
                    break;
            }

            _jumpParticleSystem.Stop();
            _chestAppearParticleSystem.Stop();
            _audioSource.PlayOneShot(_kickSound);
            BlowAwayAnimation(topPosition, underPosition);
        }
        catch
        {
        }
    }

    //スライムが飛んで行くときのアニメーション
    private void BlowAwayAnimation(Vector3 topPosition, Vector3 underPosition)
    {
        var sequence = DOTween.Sequence();
        Tween tween = null;
        if (_isMiss)
            tween = sequence.Join(gameObject.transform.DORotate(new Vector3(0, 140, 0), 0f).SetLink(gameObject));
        else tween = sequence.Join(gameObject.transform.DORotate(new Vector3(0, 180, 0), 0f).SetLink(gameObject));

        tween = sequence.Append(transform.DOLocalPath(new[] {topPosition, underPosition}, 0.7f, PathType.CatmullRom)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => SlimeLanding())).SetLink(gameObject);
    }

    //スライムが飛ばされた後着地した時の動作
    private void SlimeLanding()
    {
        _smokeParticle.SetActive(false);
        _isErase = true;
        if (_isMiss)
        {
            _animator.SetBool("Run", true);
            _isMove = true;
            _slimeValueSubject.OnNext(0.01f);
            _isSyoukyo = true;
        }

        if (_isMidleHit)
        {
            _audioSource.PlayOneShot(_middleHitSound);
            _destructionParticleSystem.Play();
            _destructionParticleSystem2.Play();
            transform.DOMove(new Vector3(-1.0f, 0f, -2f), 0.2f).SetRelative(true).SetLink(gameObject);
            _slimeValueSubject.OnNext(0.5f);
            _isSyoukyo = true;
        }

        if (_isHit)
        {
            _audioSource.PlayOneShot(_explosionSound);
            _deadParticleSystem.Play();
            transform.DOMove(new Vector3(0f, -0.5f, -1f), 0.2f).SetRelative(true).SetLink(gameObject);
            _slimeValueSubject.OnNext(1.0f);
            _isSyoukyo = true;
        }
    }

    private void ThroughProcess()
    {
        var myposition = transform.position;
        if (myposition.x <= -4.0f && !_isBad)
        {
            _isBad = true;
            _slimeValueSubject.OnNext(0.0f);
        }

        if (myposition.x <= -6.0f)
        {
            _sequence.Kill();
            _isSyoukyo = true;
            Destruction();
        }
    }

    private void EraseCounter()
    {
        if (_isErase)
        {
            _missTime += Time.deltaTime;
            if (_missTime >= 1.0f)
            {
                _missTime = 0;
                _isErase = false;
                gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f).SetLink(gameObject)
                    .OnComplete(() => Destruction());
            }
        }
        else
        {
            _missTime = 0;
        }
    }

    private void Destruction()
    {
        _cancellationTokenSource?.Cancel();
        Destroy(gameObject);
    }
}