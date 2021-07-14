namespace Glidders
{
    namespace Manager
    {
        public interface IPhaseInformation
        {
            void TurnStart();
            void ActionSelect();
            void Move();
            void Attack();
            void TurnEnd();

            /// <summary>
            /// �t�F�[�Y���Ƃ̏��������������Ƃ��ɁADirector�ɂ����m�点�邽�߂̃f���Q�[�g��ݒ肵�܂��B
            /// </summary>
            /// <param name="phaseCompleteAction">����������Director�ɒm�点��ۂɎ��s����f���Q�[�g�B</param>
            //void SetPhaseCompleteAction(System.Action phaseCompleteAction);
        }
    }
}