using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

 namespace HastaKalistaBaby
{
    internal class Helper
    {
        
        private static Menu r = Program.r;
        private static readonly Dictionary<float, float> IncDamage = new Dictionary<float, float>();
        private static readonly Dictionary<float, float> InstDamage = new Dictionary<float, float>();
        public static void OnProcessSpellCast(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (hero.IsMe)
            {
                var spellName = args.SData.Name;

                if (spellName == "KalistaExpungeWrapper")
                {
                    Program.lastecast = Game.Time;
                    Orbwalker.ResetAutoAttack();
                }
            }

            if (Program.R.IsReady() && hero.IsAlly && args.SData.Name == "RocketGrab" && Program.Player.LSDistance(hero.Position) < Program.R.Range && Program.Player.LSDistance(hero.Position) > 350 && r["KBS"].Cast<CheckBox>().CurrentValue)
            {
                Program.grabT = Game.Time;
            }
        }

        public static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && Program.Player.LSIsDashing())
            {
                args.Process = false;
            }
        }



        public static bool hasE(Obj_AI_Base target)
        {
            return target.GetBuffCount("kalistaexpungemarker") > 0;
        }

        public static float GetHealth(Obj_AI_Base target)
        {
            var debuffer = 0f;

            /// <summary>
            ///     Gets the predicted reduction from Blitzcrank Shield.
            /// </summary>
            if (target is AIHeroClient)
            {
                if ((target as AIHeroClient).ChampionName.Equals("Blitzcrank") &&
                    !(target as AIHeroClient).HasBuff("BlitzcrankManaBarrierCD"))
                {
                    debuffer += target.Mana / 2;
                }
            }

            return target.Health +
                target.HPRegenRate +
                debuffer;
        }

        public static bool Unkillable(AIHeroClient target) //asuna op
        {
            foreach (var b in target.Buffs.Where(b => b.IsValid))
            {
                switch (b.DisplayName)
                {
                    case "JudicatorIntervention":
                        return true;

                    case "Undying Rage":
                        if (target.ChampionName == "Tryndamere")
                            return true;
                        continue;

                    //Spell Shields
                    case "bansheesveil":
                        return true;

                    case "SivirE":
                        return true;

                    case "NocturneW":
                        return true;
                }
            }

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return true;
            }

            if (target.ChampionName == "Poppy" && HeroManager.Allies.Any(
                o =>
                {
                    return !o.IsMe
                           && o.Buffs.Any(
                               b =>
                                   b.Caster.NetworkId == target.NetworkId && b.IsValid
                                   && b.DisplayName == "PoppyDITarget");
                }))
            {
                return true;
            }

            return false;
        }

        public static float CountEnemy(Vector3 pos, float range)
        {
            return HeroManager.Enemies.Count(i => i.LSIsValidTarget(range, true, pos));
        }

        public static Vector3[] CalculateVertices(int sides, float radius, float startingAngle, Vector3 center)
        {

            List<Vector3> points = new List<Vector3>();
            float step = 360.0f / sides;

            float angle = startingAngle; //starting angle
            for (double i = startingAngle; i < startingAngle + 360.0; i += step) //go in a circle
            {
                points.Add(DegreesToXY(angle, radius, center));
                angle += step;
            }

            return points.ToArray();
        }

        private static Vector3 DegreesToXY(float degrees, float radius, Vector3 origin)
        {
            Vector3 xy = new Vector3();
            double radians = degrees * Math.PI / 180.0;

            xy.X = (int)(Math.Cos(radians) * radius + origin.X);
            xy.Y = (int)(Math.Sin(-radians) * radius + origin.Y);
            xy.Z = (int)Program.Player.Position.Z;

            return xy;
        }

        public static float GetAttackRange(Obj_AI_Base target)
        {
            var result = target.AttackRange + target.BoundingRadius;
            return result;
        }

        public static Vector3 CenterOfVectors(Vector3[] vectors)
        {
            var sum = Vector3.Zero;
            if (vectors == null || vectors.Length == 0)
                return sum;

            sum = vectors.Aggregate(sum, (current, vec) => current + vec);
            return sum / vectors.Length;
        }

        public static float GetMana(LeagueSharp.Common.Spell s)
        {

            var CurrentMana = Program.Player.Mana;
            var Spellcost = s.Instance.SData.Mana;

            return CurrentMana - Spellcost;
        }

        public static float AttackSpeed()
        {
            return 1 / Program.Player.AttackDelay;
        }

        public static float IncomingDamage
        {
            get { return IncDamage.Sum(e => e.Value) + InstDamage.Sum(e => e.Value); }
        }

        public static void AADamageRemove()
        {
            foreach (var entry in IncDamage.Where(entry => entry.Key < Game.Time).ToArray())
            {
                IncDamage.Remove(entry.Key);
            }
        }

        public static void SpellDamageRemove()
        {
            foreach (var entry in InstDamage.Where(entry => entry.Key < Game.Time).ToArray())
            {
                InstDamage.Remove(entry.Key);
            }
        }
    }
}
