using UnityEngine;
using Unity.FPS.Game;

/// <summary>
/// ������ �߻�ü�� �߻��Ҷ� �������� �߻�ü�� ���ӿ�����Ʈ ũ�� ����
/// </summary>
public class ChargedProjectileEffectHandler : MonoBehaviour
{
    #region Variables
    private ProjectileBase projectileBase;

    public GameObject charageObejct;
    public MinMaxVector3 scale;
    #endregion

    private void OnEnable()
    {
        //����
        projectileBase = GetComponent<ProjectileBase>();
        projectileBase.OnShoot += OnShoot;
    }

    void OnShoot()
    {
        charageObejct.transform.localScale = scale.GetValueFromRatio(projectileBase.InitialCharge);
    }
}
