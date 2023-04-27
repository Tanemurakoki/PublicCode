using System.Collections.Generic;
using UnityEngine;


public class OilShot : MonoBehaviour
{
    #region インスペクターに表示している変数

    [SerializeField, Header("ロックオン可能な範囲指定")]
    private float _lockOnDistance = default;

    [SerializeField, Header("飛ばす球のオブジェクト")]
    private GameObject _bulletObject = default;

    [SerializeField, Header("減速値")]
    private float _decreaseSpeed = default;

    [Header("球の初速")]
    [SerializeField, Range(1, 200)]
    private float _bulletSpeed = default;

    [Header("球が消えるタイミングの速度")]
    [SerializeField, Range(0, 100)]
    private float _deleateSpeed = default;

    [Header("オイルオブジェクトのあたり判定のサイズ調整(円形)")]
    [SerializeField, Range(1, 100)]
    private float _shotColliderSize = default;

    [Header("オイルショットで与えるダメージ")]
    [SerializeField, Range(1, 10)]
    private int _shotTakeDamage = default;

    #endregion


    #region privateな変数、定数

    // オイルショットをしている状態かどうか
    private bool _isShot = default;

    // Enemyタグ
    private const string ENEMY = "Enemy";

    // 敵リスト
    private List<GameObject> _activeEnemys = default;

    // 敵リストを取得出来なかった時に使用する変数
    private GameObject[] _spareEnemys = default;

    // 現在速度格納変数
    private float _shotVelocity = default;

    // 入力値を保存する変数
    // ゲームスタート時は右を向いているため１を格納
    private float _saveHorizontalInput = 1;

    // 球を打つ方向を格納する変数
    private Vector3 _shotDirection = default;

    // ロックオン状態かどうか
    private bool _isLockOn = default;

    // 敵リストを持っているクラス
    private ObjectInCameraSingle _objectInCameraSingle = default;


    #endregion


    private void Start()
    {
        // ObjectInCameraSingleのインスタンスを取得
        _objectInCameraSingle = GameObject.FindObjectOfType<ObjectInCameraSingle>().GetComponent<ObjectInCameraSingle>();
    }


    private void Update()
    {

        float inputHorizontal = Input.GetAxisRaw("Horizontal");

        // ショット状態なら当たり判定メソッド起動
        if (_isShot)
        {
            CollisionJudge();
            return;
        }

        // 入力があり、ショット状態じゃない時
        if (inputHorizontal != 0)
        {
            // 入力値を保存しておく
            _saveHorizontalInput = inputHorizontal;
        }

        // ボタンが押され、ショット状態じゃない時
        if (Input.GetKeyDown(KeyCode.O))
        {
            // ショット状態にする
            _isShot = true;

            // 飛ばす球のオブジェクトをオンにする
            _bulletObject.SetActive(true);

            // 位置をプレイヤーの場所まで持ってくる。
            _bulletObject.transform.position = this.gameObject.transform.position;

            // 現在フィールドにいる敵を取得
            _activeEnemys = _objectInCameraSingle._spawnGrandChildren;

            // リストに敵がいたら行う処理
            if (_activeEnemys != null)
            {
                // 各敵との距離を格納していくリスト
                List<float> enemyDistances = new List<float>();

                // 取得した敵のオブジェクトのポジションを比較
                foreach (GameObject enemyObject in _activeEnemys)
                {
                    // プレイヤーオブジェクトと敵オブジェクトの距離を求める
                    float distance = (this.gameObject.transform.position.x - enemyObject.transform.position.x);

                    // 距離リストに格納
                    enemyDistances.Add(distance);
                }

                // ロックオン可能であればTrue、不可であればFalse
                _isLockOn = IsLockOn(enemyDistances);
            }
            // リストが取得出来なかった場合タグで敵の距離リストを作る。
            else if (GetEnemys() != null)
            {
                // 各敵との距離を格納していくリスト
                List<float> enemyDistances = new List<float>();

                // 取得した敵のオブジェクトのポジションを比較
                foreach (GameObject enemyObject in _spareEnemys)
                {
                    // プレイヤーオブジェクトと敵オブジェクトの距離を求める
                    float distance = (this.gameObject.transform.position.x - enemyObject.transform.position.x);

                    // 距離リストに格納
                    enemyDistances.Add(distance);
                }
                _isLockOn = IsLockOn(enemyDistances);
            }
            // 敵が居ない場合はロックオン不可
            else
            {
                _isLockOn = false;
            }

            // 球の現在速度に初速を入れる
            _shotVelocity = _bulletSpeed;
        }
    }


    /// <summary>
    /// 敵と球の衝突判定をしているメソッド
    /// </summary>
    private void CollisionJudge()
    {
        // 敵がいるときの処理
        if (_objectInCameraSingle._spawnGrandChildren != null)
        {
            // 敵リストを全探索
            for (int i = 0; i < _objectInCameraSingle._spawnGrandChildren.Count; i++)
            {
                //敵と球の距離が規定値内の時
                if (Mathf.Abs(_objectInCameraSingle._spawnGrandChildren[i].transform.position.x - _bulletObject.transform.position.x) +
                    Mathf.Abs(_objectInCameraSingle._spawnGrandChildren[i].transform.position.y - _bulletObject.transform.position.y) <= _shotColliderSize)
                {
                    // 敵のキャラクターマネージャーを取得し、ダメージを与える。
                    _objectInCameraSingle._spawnGrandChildren[i].GetComponent<CharacterManager>().TakesDamage(_shotTakeDamage);

                    // 球オブジェクトをオフにする
                    _bulletObject.SetActive(false);

                    // ショット状態解除
                    _isShot = false;
                }
            }
        }
        // 敵がいない時、リストが取れなかった時の処理
        // 敵を配列に格納して同じ処理をする
        else if (GetEnemys() != null)
        {
            for (int i = 0; i < _spareEnemys.Length; i++)
            {
                if (Mathf.Abs(_spareEnemys[i].transform.position.x - _bulletObject.transform.position.x) +
                    Mathf.Abs(_spareEnemys[i].transform.position.y - _bulletObject.transform.position.y) <= _shotColliderSize)
                {
                    // 敵のキャラクターマネージャーを取得し、ダメージを与える。
                    _spareEnemys[i].GetComponent<CharacterManager>().TakesDamage(_shotTakeDamage);

                    // 球オブジェクトをオフにする
                    _bulletObject.SetActive(false);

                    // ショット状態解除
                    _isShot = false;
                }
            }
        }
    }


    private void FixedUpdate()
    {
        // ショット状態ならショットメソッドを起動
        if (_isShot)
        {
            OilShot();
        }
    }


    /// <summary>
    /// オイルショットの球を飛ばす処理をしているメソッド
    /// </summary>
    private void OilShot()
    {
        // ロックオン状態の時
        if (_isLockOn)
        {
            // 敵に向かって球を飛ばす
            _bulletObject.transform.position += _shotDirection * _shotVelocity * Time.fixedDeltaTime;
        }
        // 範囲外、敵が居ない時は真っ直ぐ飛ばす
        else
        {
            // 球を向いている方向に真っ直ぐ飛ばす
            _bulletObject.transform.position += Vector3.right * _shotVelocity * Time.fixedDeltaTime * _saveHorizontalInput;
        }

        // 初速から減速させていく
        _shotVelocity -= _decreaseSpeed * Time.fixedDeltaTime;

        // 球の現在速度が規定値を下回った時、ショット状態を解除する
        if (_shotVelocity <= _deleateSpeed)
        {
            // オブジェクトをオフにする
            _bulletObject.SetActive(false);

            // ショット状態を解除
            _isShot = false;

            // ロックオン状態を解除
            _isLockOn = false;
        }
    }


    /// <summary>
    /// 敵と自分との最短距離を求めて、ロックオン範囲内にいるかどうかを返すメソッド
    /// </summary>
    /// <param name="DistanceList">敵とプレイヤーの距離リスト</param>
    /// <returns>ロックオン状態かどうか</returns>
    private bool IsLockOn(List<float> DistanceList)
    {
        // リスト上の最短距離の敵を示すインデックス
        int minDistanceIndex = default;

        // プレイヤーが右を向いている場合
        if (_saveHorizontalInput > 0)
        {
            // 右を向いている場合、負の数の最大値を最短距離とするためMinValueを初期値にして比較していく
            float minData = float.MinValue;

            // リストの中身を全て確認する
            for (int i = 0; i < DistanceList.Count; i++)
            {
                // 距離が正の数の時はスキップ
                if (DistanceList[i] > 0)
                {
                    continue;
                }

                // リストの要素と現在の最短距離の比較
                if (DistanceList[i] > minData)
                {
                    // 最短距離、最短距離のインデックスを更新
                    minData = DistanceList[i];
                    minDistanceIndex = i;
                }
            }

            // ロックオン可能距離にいない時Falseを返す
            if (Mathf.Abs(minData) > _lockOnDistance)
            {
                return false;
            }
            // そうでない時は発射するベクトルを計算した後、trueを返す
            else
            {
                // 敵の方向を向くベクトル計算
                _shotDirection = (_activeEnemys[minDistanceIndex].transform.position - _bulletObject.transform.position).normalized;

                return true;
            }
        }
                // プレイヤーが左を向いている場合
        else
        {
            // 左を向いている場合正の数の絶対値が小さいものを最短距離にするためMaxValueを初期値にして比較していく
            float minData = float.MaxValue;

            // リストの中身を全て確認する
            for (int i = 0; i < DistanceList.Count; i++)
            {
                // 距離が負の数のときはスキップ
                if (DistanceList[i] < 0)
                {
                    continue;
                }

                // リストの要素と現在の最短距離の比較
                if (DistanceList[i] < minData)
                {
                    // 最短距離、最短距離のインデックスを更新
                    minData = DistanceList[i];
                    minDistanceIndex = i;
                }
            }

            // ロックオン可能距離にいない時Falseを返す
            if (Mathf.Abs(minData) > _lockOnDistance)
            {
                return false;
            }
            // そうでない時は発射するベクトルを計算した後、trueを返す
            else
            {
                // 敵の方向を向くベクトル計算
                _shotDirection = (_activeEnemys[minDistanceIndex].transform.position - _bulletObject.transform.position).normalized;

                return true;
            }
        }
    }


    /// <summary>
    /// もし敵リストの取得に失敗した場合、GameObject.FindGameObjectsWithTagで敵がいないか調べるメソッド
    /// </summary>
    /// <returns>GameObject.FindGameObjectsWithTagした結果の配列を返す</returns>
    private GameObject[] GetEnemys()
    {
        // Enemyタグのついているオブジェクトを格納して返す
        _spareEnemys = GameObject.FindGameObjectsWithTag(ENEMY);
        return _spareEnemys;
    }
}
