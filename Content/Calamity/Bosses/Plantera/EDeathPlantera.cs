﻿using System.IO;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Plantera
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathPlantera : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.Plantera;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(dashTimer);
            binaryWriter.Write(dashing);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.Read7BitEncodedInt();
            dashTimer = binaryReader.Read7BitEncodedInt();
            dashing = binaryReader.ReadBoolean();
        }

        public int timer = 0;
        public int dashTimer = 0;
        public bool dashing = false;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            Player target = Main.player[NPC.target];
            FargowiltasSouls.Content.Bosses.VanillaEternity.Plantera plant = NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Plantera>();

            if (NPC.localAI[0] == 1)
            {

                timer++;
                if (timer >= 800)
                {
                    timer = 0;
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<HomingGasBulb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
            }
            else
            {
                if (dashing)
                {
                    if (dashTimer == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                        NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                        if (DLCUtils.HostCheck)
                        {
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.Next(5, 10)).RotatedBy(MathHelper.ToRadians(360 / 15f * i)), ModContent.ProjectileType<SporeGasPlantera>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage / 2), 0);
                            }
                        }
                    }
                    dashTimer++;
                    if (dashTimer >= 60)
                    {
                        dashing = false;
                        dashTimer = 0;
                    }
                    return false;
                }
                if (plant.TentacleTimer == -390)
                {
                    dashing = true;
                }
                //Main.NewText(plant.TentacleTimer);
            }
            return true;
        }
    }
}
