using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ExpandedMaterials_Plastics
{

    public class VMEu_Placeworker_OnOilPond : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Thing thing2 = map.thingGrid.ThingAt(loc, ThingDef.Named("VMEu_OilPond"));
            if (thing2 == null || thing2.Position != loc)
            {
                return "VMEu_MustPlaceOnOilPond".Translate();
            }
            return true;
        }

        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == ThingDef.Named("VMEu_OilPond");
        }
    }


	public class CompOilExtractor : ThingComp
	{
		public VMEu_Building_OilPond oilPond;

		public float ticksInADay = 60000f;

		public int ticksCounter;

		public CompProperties_OilExtractor Props => (CompProperties_OilExtractor)props;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0, false);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			oilPond = (VMEu_Building_OilPond)parent.Map.thingGrid.ThingAt(parent.Position, ThingDef.Named("VMEu_OilPond"));
		}

		public override void CompTick()
		{
			base.CompTick();
			ticksCounter++;
			if (ticksCounter > ticksInADay * Props.oilInterval)
			{
				oilPond = (VMEu_Building_OilPond)parent.Map.thingGrid.ThingAt(parent.Position, ThingDef.Named("VMEu_OilPond"));
				if (oilPond.oilLeft > 0)
				{
					oilPond.oilLeft -= Props.oilProduced;
					Thing obj = ThingMaker.MakeThing(ThingDef.Named("VMEu_DarkOil"), null);
					obj.stackCount = Props.oilProduced;
					GenPlace.TryPlaceThing(obj, parent.Position, parent.Map, (ThingPlaceMode)1, null, null, default);
					ticksCounter = 0;
				}
			}
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			oilPond = (VMEu_Building_OilPond)parent.Map.thingGrid.ThingAt(parent.Position, ThingDef.Named("VMEu_OilPond"));
			if (oilPond != null && oilPond.oilLeft > 0)
			{
				stringBuilder.Append(TranslatorFormattedStringExtensions.Translate("VMEu_PondHasOil",oilPond.oilLeft));
				stringBuilder.AppendLine();
				int num = (int)(ticksInADay * Props.oilInterval) - ticksCounter;
				stringBuilder.Append(TranslatorFormattedStringExtensions.Translate("VMEu_ExtractorProducing", Props.oilProduced, GenDate.ToStringTicksToPeriod(num, true, false, true, true)));
				return stringBuilder.ToString();
			}
			stringBuilder.Append(Translator.Translate("VMEu_PondNoOil"));
			return stringBuilder.ToString();
		}
	}

	public class CompProperties_OilExtractor : CompProperties
	{
		public int oilProduced = 1;

		public float oilInterval = 1f;

		public CompProperties_OilExtractor()
		{
			compClass = typeof(CompOilExtractor);
		}
	}

	public class VMEu_Building_OilPond : Building
	{
		public int oilLeft = 9000;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref oilLeft, "oilLeft", 0, false);
		}

		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (HasFuel())
			{
				return text + TranslatorFormattedStringExtensions.Translate("VMEu_PondHasOil", oilLeft);
			}
			return text + Translator.Translate("VMEu_PondNoOil");
		}

		public bool HasFuel()
		{
			if (oilLeft > 0)
			{
				return true;
			}
			return false;
		}
	}
}
