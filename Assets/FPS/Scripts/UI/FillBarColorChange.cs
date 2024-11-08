using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    /// <summary>
    /// ���������� ��������, ��׶���� ���� ����
    /// </summary>
    public class FillBarColorChange : MonoBehaviour
    {
        #region Variables
        public Image foregroundImage;
        public Color defaultForegroundColor;        //�������� �⺻ �÷�
        public Color flashForeGroundColorFull;      //�������� Ǯ�� ���� ���� �� �÷��� ȿ��

        public Image backgroundImage;
        public Color defaultBackgroundColor;        //��׶��� �⺻ �÷�
        public Color flashBackgroundColorEmpty;     //��׶��� ���������� 0�϶� �÷���

        private float fullValue = 1f;               //�������� Ǯ�϶��� ��
        private float emptyValue = 0f;              //�����ڰ� ������ ��

        private float colorChangeSharpness = 5f;    //�÷� ���� �ӵ�
        private float prevousValue;                 //�������� Ǯ�� ���� ������ ã�� ����
        #endregion

        //�� ���� ���� �� �ʱ�ȭ
        public void Initialize(float fullVauleRatio, float emptyValueRatio)
        {
            fullValue = fullVauleRatio;
            emptyValue = emptyValueRatio;

            prevousValue = fullValue;
        }

        public void UpdateVisual(float currentRatio)
        {
            //�������� Ǯ�� ���� ����
            if(currentRatio == fullValue && currentRatio != prevousValue)
            {
                foregroundImage.color = flashForeGroundColorFull;
            }
            else if(currentRatio < emptyValue)
            {
                backgroundImage.color = flashBackgroundColorEmpty;
            }
            else
            {
                foregroundImage.color = Color.Lerp(foregroundImage.color, defaultForegroundColor,
                    colorChangeSharpness * Time.deltaTime);
                backgroundImage.color = Color.Lerp(backgroundImage.color, defaultBackgroundColor,
                    colorChangeSharpness * Time.deltaTime);
            }

            prevousValue = currentRatio;
        }
    }
}