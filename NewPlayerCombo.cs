using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �v���C���[�̃R���{����������N���X
/// </summary>
public class PlayerCombo : NomaBehaviour
{
    // �U�������������̃t���O
    [SerializeField]
    private bool _isHit = false;

    // �G����U�����ꂽ���̃t���O
    [SerializeField]
    private bool _isDamageHit = false;

    // �v���C���[�̌��݂̍U����
    private int _currentPlayerAttackPower = default;

    // �v���C���[�̌��݂̃R���{
    private int _currentPlayerCombo = 0;

    // ���݂̃R���{�P�\����
    private float _currentComboGraceTime = default;

    // �R���{�e�L�X�g
    [SerializeField,Header("�R���{�e�L�X�g")]
    const Text _COMBOTEXT = default;

    [Header("�R���{���ʔ����^�C�~���O")]

    [SerializeField, Tooltip("1�i�K��")]
    const int _firstStepComboCount = 3;

    [SerializeField, Tooltip("2�i�K��")]
    const int _secondStepComboCount = 7;

    [SerializeField, Tooltip("3�i�K��")]
    const int _thirdStepComboCount = 12;


    [Header("�v���C���[�U����")]

    [SerializeField, Tooltip("�����l")]
    public int _defaultAttackPower = 1;

    [SerializeField, Tooltip("1�i�K��")]
    public int _firstAttackPower = 2;

    [SerializeField, Tooltip("2�i�K��")]
    public int _secondAttackPower = 3;

    [SerializeField, Tooltip("3�i�K��")]
    public int _thirdAttackPower = 4;


    [Header("�R���{�P�\����")]

    [SerializeField, Tooltip("�����l")]
    private float _defaultComboGraceTime = 10;

    [SerializeField, Tooltip("1�i�K��")]
    private float _firstComboGraceTime = 6;

    [SerializeField, Tooltip("2�i�K��")]
    private float _secondComboGraceTime = 4;

    [SerializeField, Tooltip("3�i�K��")]
    private float _thirdComboGraceTime = 1;

    private void Start()
    {
        // �U���͂̏�����
        _currentPlayerAttackPower = _defaultAttackPower;

        // �R���{�̏�����
        _currentPlayerCombo = 0;
    }


    private void FixedUpdate()
    {
        //�R���{���\�b�h�N��
        Combo();
    }

    /// <summary>
    /// �R���{�̂̏����B�R���{�̉��Z�A�P�\���Ԃ̌��Z���s���B
    /// FixedUpdate�ŌĂ�ł��������B
    /// </summary>
    private void Combo()
    {
        // �U��������������
        if (_isHit)
        {
            // �R���{���Z
            _currentPlayerCombo++;

            // �R���{�J�E���g�̃`�F�b�N
            ComboCount();

            // �t���O��off�ɂ���
            _isHit = false;
        }

        //�P�R���{�ȏ�̎��̏���
        if (_currentPlayerCombo > 0)
        {
            // �R���{�e�L�X�g�\��
            _comboText.text = _currentPlayerCombo + "Combo";

            // �R���{�P�\���Ԃ����鎞�̏���
            if (_currentComboGraceTime >0)
            {
                // �R���{�P�\���Ԍ��Z
                _currentComboGraceTime -= Time.deltaTime;
            }

            // �R���{�P�\���Ԃ��Ȃ���
            else
            {
                ComboReset();
            }
        }

        // �G����_���[�W��H�������
        if (_isDamageHit)
        {
            ComboReset();
            _isDamageHit = false;
        }
    }

    /// <summary>
    /// �R���{�ɂ��U���͂̏㏸�A�R���{�P�\���Ԃ̃��Z�b�g�����郁�\�b�h�B
    /// </summary>
    private void ComboCount()
    {
        // �R���{���ʖ������
        if (_currentPlayerCombo < _firstStepComboCount)
        {
            SetAttackPower(_defaultAttackPower, _defaultComboGraceTime);
        }

        // �R���{���ʂP�i�K�ڔ���
        else if (_currentPlayerCombo < _secondStepComboCount)
        {
            SetAttackPower(_firstAttackPower, _firstComboGraceTime);
        }

        // �R���{���ʂQ�i�K�ڔ���
        else if (_currentPlayerCombo < _thirdStepComboCount)
        {
            // �Q�i�K�ڂ̍U���́A�R���{�P�\���Ԃ�ݒ�B
            SetAttackPower(_secondAttackPower, _secondComboGraceTime);
        }

        // �R���{���ʂR�i�K�ڔ���
        else
        {
            // �R�i�K�ڂ̍U���́A�R���{�P�\���Ԃ�ݒ�B
            SetAttackPower(_thirdAttackPower, _thirdComboGraceTime);
        }
    }
    /// <summary>
    /// �U���͂ƃR���{�P�\���Ԃ̃Z�b�g
    /// </summary>
    /// <param name="attackPower">�U����</param>
    /// <param name="comboGraceTime">�R���{�P�\����</param>
    private void SetAttackPower(int attackPower, float comboGraceTime)
    {
        _currentPlayerAttackPower = attackPower;
        _currentComboGraceTime = comboGraceTime;
    }

    /// <summary>
    /// �R���{�̃��Z�b�g�����郁�\�b�h
    /// </summary>
    private void ComboReset()
    {
        // �R���{�P�\���Ԃ��}�C�i�X�ɂȂ�Ȃ��悤�ɂ���
        _currentComboGraceTime = _defaultComboGraceTime;

        // �R���{�̃��Z�b�g
        _currentPlayerCombo = 0;

        _currentPlayerAttackPower = _defaultAttackPower;

        // �R���{�e�L�X�g�\��������
        _comboText.text = null;

        
    }
}
