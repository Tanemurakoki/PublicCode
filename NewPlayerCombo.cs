using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのコンボ処理をするクラス
/// </summary>
public class PlayerCombo : NomaBehaviour
{
    // 攻撃当たった時のフラグ
    [SerializeField]
    private bool _isHit = false;

    // 敵から攻撃された時のフラグ
    [SerializeField]
    private bool _isDamageHit = false;

    // プレイヤーの現在の攻撃力
    private int _currentPlayerAttackPower = default;

    // プレイヤーの現在のコンボ
    private int _currentPlayerCombo = 0;

    // 現在のコンボ猶予時間
    private float _currentComboGraceTime = default;

    // コンボテキスト
    [SerializeField,Header("コンボテキスト")]
    const Text _COMBOTEXT = default;

    [Header("コンボ効果発動タイミング")]

    [SerializeField, Tooltip("1段階目")]
    const int _firstStepComboCount = 3;

    [SerializeField, Tooltip("2段階目")]
    const int _secondStepComboCount = 7;

    [SerializeField, Tooltip("3段階目")]
    const int _thirdStepComboCount = 12;


    [Header("プレイヤー攻撃力")]

    [SerializeField, Tooltip("初期値")]
    public int _defaultAttackPower = 1;

    [SerializeField, Tooltip("1段階目")]
    public int _firstAttackPower = 2;

    [SerializeField, Tooltip("2段階目")]
    public int _secondAttackPower = 3;

    [SerializeField, Tooltip("3段階目")]
    public int _thirdAttackPower = 4;


    [Header("コンボ猶予時間")]

    [SerializeField, Tooltip("初期値")]
    private float _defaultComboGraceTime = 10;

    [SerializeField, Tooltip("1段階目")]
    private float _firstComboGraceTime = 6;

    [SerializeField, Tooltip("2段階目")]
    private float _secondComboGraceTime = 4;

    [SerializeField, Tooltip("3段階目")]
    private float _thirdComboGraceTime = 1;

    private void Start()
    {
        // 攻撃力の初期化
        _currentPlayerAttackPower = _defaultAttackPower;

        // コンボの初期化
        _currentPlayerCombo = 0;
    }


    private void FixedUpdate()
    {
        //コンボメソッド起動
        Combo();
    }

    /// <summary>
    /// コンボのの処理。コンボの加算、猶予時間の減算を行う。
    /// FixedUpdateで呼んでください。
    /// </summary>
    private void Combo()
    {
        // 攻撃が当たったら
        if (_isHit)
        {
            // コンボ加算
            _currentPlayerCombo++;

            // コンボカウントのチェック
            ComboCount();

            // フラグをoffにする
            _isHit = false;
        }

        //１コンボ以上の時の処理
        if (_currentPlayerCombo > 0)
        {
            // コンボテキスト表示
            _comboText.text = _currentPlayerCombo + "Combo";

            // コンボ猶予時間がある時の処理
            if (_currentComboGraceTime >0)
            {
                // コンボ猶予時間減算
                _currentComboGraceTime -= Time.deltaTime;
            }

            // コンボ猶予時間がない時
            else
            {
                ComboReset();
            }
        }

        // 敵からダメージを食らったら
        if (_isDamageHit)
        {
            ComboReset();
            _isDamageHit = false;
        }
    }

    /// <summary>
    /// コンボによる攻撃力の上昇、コンボ猶予時間のリセットをするメソッド。
    /// </summary>
    private void ComboCount()
    {
        // コンボ効果無し状態
        if (_currentPlayerCombo < _firstStepComboCount)
        {
            SetAttackPower(_defaultAttackPower, _defaultComboGraceTime);
        }

        // コンボ効果１段階目発動
        else if (_currentPlayerCombo < _secondStepComboCount)
        {
            SetAttackPower(_firstAttackPower, _firstComboGraceTime);
        }

        // コンボ効果２段階目発動
        else if (_currentPlayerCombo < _thirdStepComboCount)
        {
            // ２段階目の攻撃力、コンボ猶予時間を設定。
            SetAttackPower(_secondAttackPower, _secondComboGraceTime);
        }

        // コンボ効果３段階目発動
        else
        {
            // ３段階目の攻撃力、コンボ猶予時間を設定。
            SetAttackPower(_thirdAttackPower, _thirdComboGraceTime);
        }
    }
    /// <summary>
    /// 攻撃力とコンボ猶予時間のセット
    /// </summary>
    /// <param name="attackPower">攻撃力</param>
    /// <param name="comboGraceTime">コンボ猶予時間</param>
    private void SetAttackPower(int attackPower, float comboGraceTime)
    {
        _currentPlayerAttackPower = attackPower;
        _currentComboGraceTime = comboGraceTime;
    }

    /// <summary>
    /// コンボのリセットをするメソッド
    /// </summary>
    private void ComboReset()
    {
        // コンボ猶予時間がマイナスにならないようにする
        _currentComboGraceTime = _defaultComboGraceTime;

        // コンボのリセット
        _currentPlayerCombo = 0;

        _currentPlayerAttackPower = _defaultAttackPower;

        // コンボテキスト表示を消す
        _comboText.text = null;

        
    }
}
