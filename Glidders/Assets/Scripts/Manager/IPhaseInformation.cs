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
            /// フェーズごとの処理が完了したときに、Directorにそれを知らせるためのデリゲートを設定します。
            /// </summary>
            /// <param name="phaseCompleteAction">処理完了をDirectorに知らせる際に実行するデリゲート。</param>
            //void SetPhaseCompleteAction(System.Action phaseCompleteAction);
        }
    }
}