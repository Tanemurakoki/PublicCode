using UnityEngine;

/*
種村担当ですが、当たり判定に関する部分はチームメンバーの書いたスクリプトになっています。
*/

/*
ギミックの１つ、プレス機のクラス。
上下運動をしていて、プレイヤーが当たるとプレイヤーがダメージを受ける。
*/
public class Press1 : MonoBehaviour
{
    [SerializeField,Header("プレイヤーが受けるダメージ")]
    private int _damage = 0;

    // プレイヤーのゲームオブジェクト
    private GameObject _playerObject = default;

    // プレイヤーのHP管理のスクリプト
    private PlayerOil _playerOil = default;

    [SerializeField,Header("下から上に行くプレス機フラグ")]
    private bool _isUp = false;

    [SerializeField, Header("プレスの発動間隔")]
    private float _pressInterval = 1;

    [SerializeField, Header("プレスの速度(マイナス)")]
    private float _pressSpeed = 0.1f;

    [SerializeField, Header("プレスが戻る速度(プラス)")]
    private float _upSpeed = 1f;

    [SerializeField,Header("高さの最大値")]
    private float _upRimit = default;

    [SerializeField,Header("高さの最小値")]
    private float _downRimit = default;

    //　プレスの方向と速度を格納する変数
   private float _moveDirection = default;


    private Vector2 _playerLocalTopPosMax = default;
    private Vector2 _playerLocalTopPosMin = default;
    private Vector2 _myLocalTopPosMax = default;
    private Vector2 _myLocalTopPosMin = default;

    // Colliderのサイズの半分
    protected Vector2 _myHalfColliderSize = default;
    // とりつけた当たり判定の中心
    protected Vector2 _myColliderOffset = default;


    private void Start()
    {
        //プレイヤーのゲームオブジェクトをタグで取得。
        _playerObject = GameObject.FindWithTag("Player");

        //当たり判定をするメソッドを起動（チームメンバー担当メソッド）
        MyCollisionStart();
    }

    /// <summary>
    /// チームメンバー担当メソッド
    /// </summary>
    protected void CollisionStart()
    {
        // 子オブジェクトにしている子オブジェクトを取得
        BoxCollider2D boxCollider =GetComponentInChildren<BoxCollider2D>();

        // 自オブジェクトColliderの中心(ローカル座標)と大きさをとる
        _myColliderOffset = this.transform.localScale * boxCollider.offset;
        _myHalfColliderSize = this.transform.localScale * boxCollider.size / 2;
    }

    /// <summary>
    /// 当たり判定に関するメソッド（チームメンバー担当メソッド）
    /// </summary>
    protected void MyCollisionStart()
    {
        BoxCollider2D playerCollider = _playerObject.GetComponent<BoxCollider2D>();

        // プレイヤーのコライダーのサイズと中心からのずれを取得する
        Vector2 playerHalfSize = _playerObject.transform.localScale * playerCollider.size / 2;
        Vector2 playerOffset = _playerObject.transform.localScale * playerCollider.offset;

        // プレイヤーと敵(自キャラ)のコライダーの中心に対する頂点の座標を求める
        _playerLocalTopPosMax = new Vector2(playerOffset.x + playerHalfSize.x, playerOffset.y + playerHalfSize.y);
        _playerLocalTopPosMin = new Vector2(playerOffset.x - playerHalfSize.x, playerOffset.y - playerHalfSize.y);
        _myLocalTopPosMax = new Vector2(_myColliderOffset.x + _myHalfColliderSize.x, _myColliderOffset.y + _myHalfColliderSize.y);
        _myLocalTopPosMin = new Vector2(_myColliderOffset.x - _myHalfColliderSize.x, _myColliderOffset.y - _myHalfColliderSize.y);

        // 自キャラのコライダーの大きさを取得する
        CollisionStart();
    }

    private void FixedUpdate()
    {
        // 当たり判定メソッド起動（チームメンバー担当メソッド）
        OnCol();

        // プレスの挙動メソッド起動
        Move();
    }


    /// <summary>
    /// プレス機を上下に動かすメソッド（種村担当）
    /// </summary>
    private void Move()
    {
        // プレス、上るときの処理
        transform.Translate(Vector3.up * Time.fixedDeltaTime  * _moveDirection);

        // Y座標が最大値になったときの処理
        if (transform.position.y > _upRimit)
        {
            // Y座標を最大値に設定
            transform.position = new Vector3(transform.position.x, _upRimit, transform.position.z);
            
            // プレスするスピードを設定
            _moveDirection = _pressSpeed;
        }

        // Y座標が最小値になった時の処理
        else if (transform.position.y < _downRimit)
        {
            // Y座標を最小値に設定
            transform.position = new Vector3(transform.position.x, _downRimit, transform.position.z);

            // 上に上がるスピードを設定
            _moveDirection = _upSpeed;
        }
    }


    /// <summary>
    /// プレイヤーが当たった時にする処理（種村担当）
    /// </summary>
    private void Damage()
    {
            // プレイヤーのオイルスクリプト（HPを管理しているスクリプト）取得
            _playerOil = _playerObject.GetComponent<PlayerOil>();

            // プレイヤーのHP減算(_damage)にダメージ値を入れる。
            _playerOil.UseOil(_damage, true);
    }

    /// <summary>
    /// 当たり判定の処理をしているメソッド（チームメンバー担当メソッド）
    /// </summary>
    private void OnCol()
    {
        // プレイヤーのColliderの頂点の座標を求める
        Vector2 playerColliderPosMax = new Vector2(_playerObject.transform.position.x + _playerLocalTopPosMax.x, _playerObject.transform.position.y + _playerLocalTopPosMax.y);
        Vector2 playerColliderPosMin = new Vector2(_playerObject.transform.position.x + _playerLocalTopPosMin.x, _playerObject.transform.position.y + _playerLocalTopPosMin.y);

        // 自オブジェクトのColliderの頂点の座標を求める
        Vector2 myColliderPosMax = new Vector2(this.transform.position.x + _myLocalTopPosMax.x, this.transform.position.y + _myLocalTopPosMax.y);
        Vector2 myColliderPosMin = new Vector2(this.transform.position.x + _myLocalTopPosMin.x, this.transform.position.y + _myLocalTopPosMin.y);

        // X軸の判定を行う
        if (playerColliderPosMax.x <= myColliderPosMax.x && playerColliderPosMax.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMin.x <= myColliderPosMax.x && playerColliderPosMin.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMax.x >= myColliderPosMax.x && playerColliderPosMin.x <= myColliderPosMin.x) { }

        // X軸に当たっていないとき処理をやめる
        else { return; }

        // Y軸の判定を行う
        if (playerColliderPosMax.y <= myColliderPosMax.y && playerColliderPosMax.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMin.y <= myColliderPosMax.y && playerColliderPosMin.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMax.y >= myColliderPosMax.y && playerColliderPosMin.y <= myColliderPosMin.y) { }

        // X軸に当たっていないとき処理をやめる
        else { return; }

        // 当たり判定が重なっているときダメージを与える
        Damage();
    }
}
