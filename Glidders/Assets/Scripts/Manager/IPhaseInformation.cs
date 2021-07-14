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
        }
    }
}