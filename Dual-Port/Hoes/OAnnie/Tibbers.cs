using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;

 namespace OAnnie
{
    internal class Tibbers
    {
        public static List<Obj_AI_Turret> Turrets = new List<Obj_AI_Turret>();
        public static AIHeroClient Player = ObjectManager.Player;
        public static GameObject Tibbersobject { get; set; }

        /// <summary>
        /// When loaded
        /// </summary>
        /// <param name="args"></param>
        internal static void OnLoad(EventArgs args)
        {
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
        }

        /// <summary>
        /// Setting tibbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            //  Game.PrintChat(sender.Name.ToLower());

            if (sender.Name == "tibbers")
            {
                Tibbersobject = sender;
            }
        }

        public static Obj_AI_Turret GetTurrets()
        {
                var turri =
                    Turrets.OrderBy(x => x.LSDistance(Tibbersobject.Position) <= 500 && !x.IsAlly && !x.IsDead)
                        .FirstOrDefault();
                return turri;
        }


        /// <summary>
        /// Tibbers movement logic
        /// </summary>
        public static void Tibbersmove()
        {
            var target = TargetSelector.GetTarget(2000, DamageType.Magical);

            if (Player.HasBuff("infernalguardiantime"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet,
                    target.LSIsValidTarget(1500) ? target.Position : GetTurrets().Position);
            }
        }
    }
}

