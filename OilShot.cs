using System.Collections.Generic;
using UnityEngine;


public class OilShot : MonoBehaviour
{
    #region �C���X�y�N�^�[�ɕ\�����Ă���ϐ�

    [SerializeField, Header("���b�N�I���\�Ȕ͈͎w��")]
    private float _lockOnDistance = default;

    [SerializeField, Header("��΂����̃I�u�W�F�N�g")]
    private GameObject _bulletObject = default;

    [SerializeField, Header("�����l")]
    private float _decreaseSpeed = default;

    [Header("���̏���")]
    [SerializeField, Range(1, 200)]
    private float _bulletSpeed = default;

    [Header("����������^�C�~���O�̑��x")]
    [SerializeField, Range(0, 100)]
    private float _deleateSpeed = default;

    [Header("�I�C���I�u�W�F�N�g�̂����蔻��̃T�C�Y����(�~�`)")]
    [SerializeField, Range(1, 100)]
    private float _shotColliderSize = default;

    [Header("�I�C���V���b�g�ŗ^����_���[�W")]
    [SerializeField, Range(1, 10)]
    private int _shotTakeDamage = default;

    #endregion


    #region private�ȕϐ��A�萔

    // �I�C���V���b�g�����Ă����Ԃ��ǂ���
    private bool _isShot = default;

    // Enemy�^�O
    private const string ENEMY = "Enemy";

    // �G���X�g
    private List<GameObject> _activeEnemys = default;

    // �G���X�g���擾�o���Ȃ��������Ɏg�p����ϐ�
    private GameObject[] _spareEnemys = default;

    // ���ݑ��x�i�[�ϐ�
    private float _shotVelocity = default;

    // ���͒l��ۑ�����ϐ�
    // �Q�[���X�^�[�g���͉E�������Ă��邽�߂P���i�[
    private float _saveHorizontalInput = 1;

    // ����ł������i�[����ϐ�
    private Vector3 _shotDirection = default;

    // ���b�N�I����Ԃ��ǂ���
    private bool _isLockOn = default;

    // �G���X�g�������Ă���N���X
    private ObjectInCameraSingle _objectInCameraSingle = default;


    #endregion


    private void Start()
    {
        // ObjectInCameraSingle�̃C���X�^���X���擾
        _objectInCameraSingle = GameObject.FindObjectOfType<ObjectInCameraSingle>().GetComponent<ObjectInCameraSingle>();
    }


    private void Update()
    {

        float inputHorizontal = Input.GetAxisRaw("Horizontal");

        // �V���b�g��ԂȂ瓖���蔻�胁�\�b�h�N��
        if (_isShot)
        {
            CollisionJudge();
            return;
        }

        // ���͂�����A�V���b�g��Ԃ���Ȃ���
        if (inputHorizontal != 0)
        {
            // ���͒l��ۑ����Ă���
            _saveHorizontalInput = inputHorizontal;
        }

        // �{�^����������A�V���b�g��Ԃ���Ȃ���
        if (Input.GetKeyDown(KeyCode.O))
        {
            // �V���b�g��Ԃɂ���
            _isShot = true;

            // ��΂����̃I�u�W�F�N�g���I���ɂ���
            _bulletObject.SetActive(true);

            // �ʒu���v���C���[�̏ꏊ�܂Ŏ����Ă���B
            _bulletObject.transform.position = this.gameObject.transform.position;

            // ���݃t�B�[���h�ɂ���G���擾
            _activeEnemys = _objectInCameraSingle._spawnGrandChildren;

            // ���X�g�ɓG��������s������
            if (_activeEnemys != null)
            {
                // �e�G�Ƃ̋������i�[���Ă������X�g
                List<float> enemyDistances = new List<float>();

                // �擾�����G�̃I�u�W�F�N�g�̃|�W�V�������r
                foreach (GameObject enemyObject in _activeEnemys)
                {
                    // �v���C���[�I�u�W�F�N�g�ƓG�I�u�W�F�N�g�̋��������߂�
                    float distance = (this.gameObject.transform.position.x - enemyObject.transform.position.x);

                    // �������X�g�Ɋi�[
                    enemyDistances.Add(distance);
                }

                // ���b�N�I���\�ł����True�A�s�ł����False
                _isLockOn = IsLockOn(enemyDistances);
            }
            // ���X�g���擾�o���Ȃ������ꍇ�^�O�œG�̋������X�g�����B
            else if (GetEnemys() != null)
            {
                // �e�G�Ƃ̋������i�[���Ă������X�g
                List<float> enemyDistances = new List<float>();

                // �擾�����G�̃I�u�W�F�N�g�̃|�W�V�������r
                foreach (GameObject enemyObject in _spareEnemys)
                {
                    // �v���C���[�I�u�W�F�N�g�ƓG�I�u�W�F�N�g�̋��������߂�
                    float distance = (this.gameObject.transform.position.x - enemyObject.transform.position.x);

                    // �������X�g�Ɋi�[
                    enemyDistances.Add(distance);
                }
                _isLockOn = IsLockOn(enemyDistances);
            }
            // �G�����Ȃ��ꍇ�̓��b�N�I���s��
            else
            {
                _isLockOn = false;
            }

            // ���̌��ݑ��x�ɏ���������
            _shotVelocity = _bulletSpeed;
        }
    }


    /// <summary>
    /// �G�Ƌ��̏Փ˔�������Ă��郁�\�b�h
    /// </summary>
    private void CollisionJudge()
    {
        // �G������Ƃ��̏���
        if (_objectInCameraSingle._spawnGrandChildren != null)
        {
            // �G���X�g��S�T��
            for (int i = 0; i < _objectInCameraSingle._spawnGrandChildren.Count; i++)
            {
                //�G�Ƌ��̋������K��l���̎�
                if (Mathf.Abs(_objectInCameraSingle._spawnGrandChildren[i].transform.position.x - _bulletObject.transform.position.x) +
                    Mathf.Abs(_objectInCameraSingle._spawnGrandChildren[i].transform.position.y - _bulletObject.transform.position.y) <= _shotColliderSize)
                {
                    // �G�̃L�����N�^�[�}�l�[�W���[���擾���A�_���[�W��^����B
                    _objectInCameraSingle._spawnGrandChildren[i].GetComponent<CharacterManager>().TakesDamage(_shotTakeDamage);

                    // ���I�u�W�F�N�g���I�t�ɂ���
                    _bulletObject.SetActive(false);

                    // �V���b�g��ԉ���
                    _isShot = false;
                }
            }
        }
        // �G�����Ȃ����A���X�g�����Ȃ��������̏���
        // �G��z��Ɋi�[���ē�������������
        else if (GetEnemys() != null)
        {
            for (int i = 0; i < _spareEnemys.Length; i++)
            {
                if (Mathf.Abs(_spareEnemys[i].transform.position.x - _bulletObject.transform.position.x) +
                    Mathf.Abs(_spareEnemys[i].transform.position.y - _bulletObject.transform.position.y) <= _shotColliderSize)
                {
                    // �G�̃L�����N�^�[�}�l�[�W���[���擾���A�_���[�W��^����B
                    _spareEnemys[i].GetComponent<CharacterManager>().TakesDamage(_shotTakeDamage);

                    // ���I�u�W�F�N�g���I�t�ɂ���
                    _bulletObject.SetActive(false);

                    // �V���b�g��ԉ���
                    _isShot = false;
                }
            }
        }
    }


    private void FixedUpdate()
    {
        // �V���b�g��ԂȂ�V���b�g���\�b�h���N��
        if (_isShot)
        {
            OilShot();
        }
    }


    /// <summary>
    /// �I�C���V���b�g�̋����΂����������Ă��郁�\�b�h
    /// </summary>
    private void OilShot()
    {
        // ���b�N�I����Ԃ̎�
        if (_isLockOn)
        {
            // �G�Ɍ������ċ����΂�
            _bulletObject.transform.position += _shotDirection * _shotVelocity * Time.fixedDeltaTime;
        }
        // �͈͊O�A�G�����Ȃ����͐^��������΂�
        else
        {
            // ���������Ă�������ɐ^��������΂�
            _bulletObject.transform.position += Vector3.right * _shotVelocity * Time.fixedDeltaTime * _saveHorizontalInput;
        }

        // �������猸�������Ă���
        _shotVelocity -= _decreaseSpeed * Time.fixedDeltaTime;

        // ���̌��ݑ��x���K��l������������A�V���b�g��Ԃ���������
        if (_shotVelocity <= _deleateSpeed)
        {
            // �I�u�W�F�N�g���I�t�ɂ���
            _bulletObject.SetActive(false);

            // �V���b�g��Ԃ�����
            _isShot = false;

            // ���b�N�I����Ԃ�����
            _isLockOn = false;
        }
    }


    /// <summary>
    /// �G�Ǝ����Ƃ̍ŒZ���������߂āA���b�N�I���͈͓��ɂ��邩�ǂ�����Ԃ����\�b�h
    /// </summary>
    /// <param name="DistanceList">�G�ƃv���C���[�̋������X�g</param>
    /// <returns>���b�N�I����Ԃ��ǂ���</returns>
    private bool IsLockOn(List<float> DistanceList)
    {
        // ���X�g��̍ŒZ�����̓G�������C���f�b�N�X
        int minDistanceIndex = default;

        // �v���C���[���E�������Ă���ꍇ
        if (_saveHorizontalInput > 0)
        {
            // �E�������Ă���ꍇ�A���̐��̍ő�l���ŒZ�����Ƃ��邽��MinValue�������l�ɂ��Ĕ�r���Ă���
            float minData = float.MinValue;

            // ���X�g�̒��g��S�Ċm�F����
            for (int i = 0; i < DistanceList.Count; i++)
            {
                // ���������̐��̎��̓X�L�b�v
                if (DistanceList[i] > 0)
                {
                    continue;
                }

                // ���X�g�̗v�f�ƌ��݂̍ŒZ�����̔�r
                if (DistanceList[i] > minData)
                {
                    // �ŒZ�����A�ŒZ�����̃C���f�b�N�X���X�V
                    minData = DistanceList[i];
                    minDistanceIndex = i;
                }
            }

            // ���b�N�I���\�����ɂ��Ȃ���False��Ԃ�
            if (Mathf.Abs(minData) > _lockOnDistance)
            {
                return false;
            }
            // �����łȂ����͔��˂���x�N�g�����v�Z������Atrue��Ԃ�
            else
            {
                // �G�̕����������x�N�g���v�Z
                _shotDirection = (_activeEnemys[minDistanceIndex].transform.position - _bulletObject.transform.position).normalized;

                return true;
            }
        }
                // �v���C���[�����������Ă���ꍇ
        else
        {
            // ���������Ă���ꍇ���̐��̐�Βl�����������̂��ŒZ�����ɂ��邽��MaxValue�������l�ɂ��Ĕ�r���Ă���
            float minData = float.MaxValue;

            // ���X�g�̒��g��S�Ċm�F����
            for (int i = 0; i < DistanceList.Count; i++)
            {
                // ���������̐��̂Ƃ��̓X�L�b�v
                if (DistanceList[i] < 0)
                {
                    continue;
                }

                // ���X�g�̗v�f�ƌ��݂̍ŒZ�����̔�r
                if (DistanceList[i] < minData)
                {
                    // �ŒZ�����A�ŒZ�����̃C���f�b�N�X���X�V
                    minData = DistanceList[i];
                    minDistanceIndex = i;
                }
            }

            // ���b�N�I���\�����ɂ��Ȃ���False��Ԃ�
            if (Mathf.Abs(minData) > _lockOnDistance)
            {
                return false;
            }
            // �����łȂ����͔��˂���x�N�g�����v�Z������Atrue��Ԃ�
            else
            {
                // �G�̕����������x�N�g���v�Z
                _shotDirection = (_activeEnemys[minDistanceIndex].transform.position - _bulletObject.transform.position).normalized;

                return true;
            }
        }
    }


    /// <summary>
    /// �����G���X�g�̎擾�Ɏ��s�����ꍇ�AGameObject.FindGameObjectsWithTag�œG�����Ȃ������ׂ郁�\�b�h
    /// </summary>
    /// <returns>GameObject.FindGameObjectsWithTag�������ʂ̔z���Ԃ�</returns>
    private GameObject[] GetEnemys()
    {
        // Enemy�^�O�̂��Ă���I�u�W�F�N�g���i�[���ĕԂ�
        _spareEnemys = GameObject.FindGameObjectsWithTag(ENEMY);
        return _spareEnemys;
    }
}
