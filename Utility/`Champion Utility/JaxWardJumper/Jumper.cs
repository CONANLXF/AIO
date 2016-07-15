using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;

 namespace WardJumper
{
    internal class Jumper
    {
        public static AIHeroClient Player = ObjectManager.Player;
        public static LeagueSharp.Common.Spell Q;

        public static float lastward;
        public static float last;

        public static int getJumpWardId()
        {
            int[] wardIds = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043, (int)ItemId.Sightstone, (int)ItemId.Warding_Totem_Trinket, (int)ItemId.Vision_Ward, (int)ItemId.Sightstone, (int)ItemId.Trackers_Knife, (int)ItemId.Trackers_Knife_Enchantment_Cinderhulk, (int)ItemId.Trackers_Knife_Enchantment_Devourer, (int)ItemId.Trackers_Knife_Enchantment_Runic_Echoes, (int)ItemId.Trackers_Knife_Enchantment_Sated_Devourer, (int)ItemId.Trackers_Knife_Enchantment_Warrior };
            foreach (var id in wardIds)
            {
                if (Item.HasItem(id) && Item.CanUseItem(id))
                    return id;
            }
            return -1;
        }

        public static void moveTo(Vector2 Pos)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Pos.To3D());
        }

        public static void wardJump(Vector2 pos)
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 700);

            if (!Q.IsReady())
                return;

            var wardIs = false;

            if (!InDistance(pos, Player.ServerPosition.To2D(), Q.Range))
            {
                pos = Player.ServerPosition.To2D() + Vector2.Normalize(pos - Player.ServerPosition.To2D()) * 600;
            }

            if (!Q.IsReady())
                return;

            foreach (var ally in ObjectManager.Get<Obj_AI_Base>().Where(ally => ally.IsAlly && !(ally is Obj_AI_Turret) && InDistance(pos, ally.ServerPosition.To2D(), 200)))
            {
                wardIs = true;
                moveTo(pos);
                if (InDistance(Player.ServerPosition.To2D(), ally.ServerPosition.To2D(), Q.Range + ally.BoundingRadius))
                {
                    if (last < Environment.TickCount)
                    {
                        Q.Cast(ally);
                        last = Environment.TickCount + 2000;
                    }
                    else return;
                }
                return;
            }
            Polygon pol;
            if ((pol = getInWhichPolygon(pos)) != null)
            {
                if (InDistance(pol.getProjOnPolygon(pos), Player.ServerPosition.To2D(), Q.Range) && !wardIs &&
                    InDistance(pol.getProjOnPolygon(pos), pos, 250))
                {
                    putWard(pos);
                }
            }
            else if (!wardIs)
            {
                putWard(pos);
            }
        }

        public static List<Polygon> poligs = new List<Polygon>();

        public static Polygon getInWhichPolygon(Vector2 vec)
        {
            return poligs.FirstOrDefault(pol => IsPointInPolygon(pol, vec));
        }

        private static bool IsPointInPolygon(Polygon polygon, Vector2 testPoint)
        {
            var result = false;
            var j = polygon.Count() - 1;
            for (var i = 0; i < polygon.Count(); i++)
            {
                if (polygon.Points[i].Y < testPoint.Y && polygon.Points[j].Y >= testPoint.Y
                    || polygon.Points[j].Y < testPoint.Y && polygon.Points[i].Y >= testPoint.Y)
                {
                    if (polygon.Points[i].X
                        + (testPoint.Y - polygon.Points[i].Y) / (polygon.Points[j].Y - polygon.Points[i].Y)
                        * (polygon.Points[j].X - polygon.Points[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static bool putWard(Vector2 pos)
        {
            int wardItem;
            if ((wardItem = getJumpWardId()) != -1)
            {
                foreach (var slot in Player.InventoryItems.Where(slot => slot.Id == (ItemId)wardItem))
                {
                    if (lastward < Environment.TickCount)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, pos.To3D());
                        lastward = Environment.TickCount + 2000;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }


        public static bool InDistance(Vector2 pos1, Vector2 pos2, float distance)
        {
            var dist2 = Vector2.DistanceSquared(pos1, pos2);
            return dist2 <= distance * distance;
        }
    }

    internal class Polygon
    {
        public List<Vector2> Points = new List<Vector2>();

        public Polygon()
        {
        }

        public Polygon(List<Vector2> P)
        {
            Points = P;
        }

        public void add(Vector2 vec)
        {
            Points.Add(vec);
        }

        public int Count()
        {
            return Points.Count;
        }

        public Vector2 getProjOnPolygon(Vector2 vec)
        {
            var closest = new Vector2(-1000, -1000);
            var start = Points[Count() - 1];
            foreach (var vecPol in Points)
            {
                var proj = projOnLine(start, vecPol, vec);
                closest = ClosestVec(proj, closest, vec);
                start = vecPol;
            }
            return closest;
        }

        public Vector2 ClosestVec(Vector2 vec1, Vector2 vec2, Vector2 to)
        {
            var dist1 = Vector2.DistanceSquared(vec1, to);
            var dist2 = Vector2.DistanceSquared(vec2, to);
            return dist1 > dist2 ? vec2 : vec1;
        }

        public void Draw(Color color, int width = 1)
        {
            for (var i = 0; i <= Points.Count - 1; i++)
            {
                if (Points[i].LSDistance(Jumper.Player.Position) < 1500)
                {
                    var nextIndex = Points.Count - 1 == i ? 0 : i + 1;
                    var from = Drawing.WorldToScreen(Points[i].To3D());
                    var to = Drawing.WorldToScreen(Points[nextIndex].To3D());
                    Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
                }
            }
        }

        private Vector2 projOnLine(Vector2 v, Vector2 w, Vector2 p)
        {
            var nullVec = new Vector2(-1, -1);
            var l2 = Vector2.DistanceSquared(v, w);

            if (l2 == 0.0)
            {
                return nullVec;
            }

            var t = Vector2.Dot(p - v, w - v) / l2;

            if (t < 0.0)
            {
                return nullVec;
            }

            if (t > 1.0)
            {
                return nullVec;
            }

            var projection = v + t * (w - v);

            return projection;
        }
    }

}