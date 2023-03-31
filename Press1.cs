using UnityEngine;

/*
�푺�S���ł����A�����蔻��Ɋւ��镔���̓`�[�������o�[�̏������X�N���v�g�ɂȂ��Ă��܂��B
*/

/*
�M�~�b�N�̂P�A�v���X�@�̃N���X�B
�㉺�^�������Ă��āA�v���C���[��������ƃv���C���[���_���[�W���󂯂�B
*/
public class Press1 : MonoBehaviour
{
    [SerializeField,Header("�v���C���[���󂯂�_���[�W")]
    private int _damage = 0;

    // �v���C���[�̃Q�[���I�u�W�F�N�g
    private GameObject _playerObject = default;

    // �v���C���[��HP�Ǘ��̃X�N���v�g
    private PlayerOil _playerOil = default;

    [SerializeField,Header("�������ɍs���v���X�@�t���O")]
    private bool _isUp = false;

    [SerializeField, Header("�v���X�̔����Ԋu")]
    private float _pressInterval = 1;

    [SerializeField, Header("�v���X�̑��x(�}�C�i�X)")]
    private float _pressSpeed = 0.1f;

    [SerializeField, Header("�v���X���߂鑬�x(�v���X)")]
    private float _upSpeed = 1f;

    [SerializeField,Header("�����̍ő�l")]
    private float _upRimit = default;

    [SerializeField,Header("�����̍ŏ��l")]
    private float _downRimit = default;

    //�@�v���X�̕����Ƒ��x���i�[����ϐ�
   private float _moveDirection = default;


    private Vector2 _playerLocalTopPosMax = default;
    private Vector2 _playerLocalTopPosMin = default;
    private Vector2 _myLocalTopPosMax = default;
    private Vector2 _myLocalTopPosMin = default;

    // Collider�̃T�C�Y�̔���
    protected Vector2 _myHalfColliderSize = default;
    // �Ƃ���������蔻��̒��S
    protected Vector2 _myColliderOffset = default;


    private void Start()
    {
        //�v���C���[�̃Q�[���I�u�W�F�N�g���^�O�Ŏ擾�B
        _playerObject = GameObject.FindWithTag("Player");

        //�����蔻������郁�\�b�h���N���i�`�[�������o�[�S�����\�b�h�j
        MyCollisionStart();
    }

    /// <summary>
    /// �`�[�������o�[�S�����\�b�h
    /// </summary>
    protected void CollisionStart()
    {
        // �q�I�u�W�F�N�g�ɂ��Ă���q�I�u�W�F�N�g���擾
        BoxCollider2D boxCollider =GetComponentInChildren<BoxCollider2D>();

        // ���I�u�W�F�N�gCollider�̒��S(���[�J�����W)�Ƒ傫�����Ƃ�
        _myColliderOffset = this.transform.localScale * boxCollider.offset;
        _myHalfColliderSize = this.transform.localScale * boxCollider.size / 2;
    }

    /// <summary>
    /// �����蔻��Ɋւ��郁�\�b�h�i�`�[�������o�[�S�����\�b�h�j
    /// </summary>
    protected void MyCollisionStart()
    {
        BoxCollider2D playerCollider = _playerObject.GetComponent<BoxCollider2D>();

        // �v���C���[�̃R���C�_�[�̃T�C�Y�ƒ��S����̂�����擾����
        Vector2 playerHalfSize = _playerObject.transform.localScale * playerCollider.size / 2;
        Vector2 playerOffset = _playerObject.transform.localScale * playerCollider.offset;

        // �v���C���[�ƓG(���L����)�̃R���C�_�[�̒��S�ɑ΂��钸�_�̍��W�����߂�
        _playerLocalTopPosMax = new Vector2(playerOffset.x + playerHalfSize.x, playerOffset.y + playerHalfSize.y);
        _playerLocalTopPosMin = new Vector2(playerOffset.x - playerHalfSize.x, playerOffset.y - playerHalfSize.y);
        _myLocalTopPosMax = new Vector2(_myColliderOffset.x + _myHalfColliderSize.x, _myColliderOffset.y + _myHalfColliderSize.y);
        _myLocalTopPosMin = new Vector2(_myColliderOffset.x - _myHalfColliderSize.x, _myColliderOffset.y - _myHalfColliderSize.y);

        // ���L�����̃R���C�_�[�̑傫�����擾����
        CollisionStart();
    }

    private void FixedUpdate()
    {
        // �����蔻�胁�\�b�h�N���i�`�[�������o�[�S�����\�b�h�j
        OnCol();

        // �v���X�̋������\�b�h�N��
        Move();
    }


    /// <summary>
    /// �v���X�@���㉺�ɓ��������\�b�h�i�푺�S���j
    /// </summary>
    private void Move()
    {
        // �v���X�A���Ƃ��̏���
        transform.Translate(Vector3.up * Time.fixedDeltaTime  * _moveDirection);

        // Y���W���ő�l�ɂȂ����Ƃ��̏���
        if (transform.position.y > _upRimit)
        {
            // Y���W���ő�l�ɐݒ�
            transform.position = new Vector3(transform.position.x, _upRimit, transform.position.z);
            
            // �v���X����X�s�[�h��ݒ�
            _moveDirection = _pressSpeed;
        }

        // Y���W���ŏ��l�ɂȂ������̏���
        else if (transform.position.y < _downRimit)
        {
            // Y���W���ŏ��l�ɐݒ�
            transform.position = new Vector3(transform.position.x, _downRimit, transform.position.z);

            // ��ɏオ��X�s�[�h��ݒ�
            _moveDirection = _upSpeed;
        }
    }


    /// <summary>
    /// �v���C���[�������������ɂ��鏈���i�푺�S���j
    /// </summary>
    private void Damage()
    {
            // �v���C���[�̃I�C���X�N���v�g�iHP���Ǘ����Ă���X�N���v�g�j�擾
            _playerOil = _playerObject.GetComponent<PlayerOil>();

            // �v���C���[��HP���Z(_damage)�Ƀ_���[�W�l������B
            _playerOil.UseOil(_damage, true);
    }

    /// <summary>
    /// �����蔻��̏��������Ă��郁�\�b�h�i�`�[�������o�[�S�����\�b�h�j
    /// </summary>
    private void OnCol()
    {
        // �v���C���[��Collider�̒��_�̍��W�����߂�
        Vector2 playerColliderPosMax = new Vector2(_playerObject.transform.position.x + _playerLocalTopPosMax.x, _playerObject.transform.position.y + _playerLocalTopPosMax.y);
        Vector2 playerColliderPosMin = new Vector2(_playerObject.transform.position.x + _playerLocalTopPosMin.x, _playerObject.transform.position.y + _playerLocalTopPosMin.y);

        // ���I�u�W�F�N�g��Collider�̒��_�̍��W�����߂�
        Vector2 myColliderPosMax = new Vector2(this.transform.position.x + _myLocalTopPosMax.x, this.transform.position.y + _myLocalTopPosMax.y);
        Vector2 myColliderPosMin = new Vector2(this.transform.position.x + _myLocalTopPosMin.x, this.transform.position.y + _myLocalTopPosMin.y);

        // X���̔�����s��
        if (playerColliderPosMax.x <= myColliderPosMax.x && playerColliderPosMax.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMin.x <= myColliderPosMax.x && playerColliderPosMin.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMax.x >= myColliderPosMax.x && playerColliderPosMin.x <= myColliderPosMin.x) { }

        // X���ɓ������Ă��Ȃ��Ƃ���������߂�
        else { return; }

        // Y���̔�����s��
        if (playerColliderPosMax.y <= myColliderPosMax.y && playerColliderPosMax.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMin.y <= myColliderPosMax.y && playerColliderPosMin.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMax.y >= myColliderPosMax.y && playerColliderPosMin.y <= myColliderPosMin.y) { }

        // X���ɓ������Ă��Ȃ��Ƃ���������߂�
        else { return; }

        // �����蔻�肪�d�Ȃ��Ă���Ƃ��_���[�W��^����
        Damage();
    }
}
