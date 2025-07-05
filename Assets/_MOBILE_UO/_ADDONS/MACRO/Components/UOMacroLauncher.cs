using System;
using System.Collections.Generic;
using UnityEngine;
using ClassicUO.Game;
using ClassicUO.Game.Managers;

internal class UOMacroLauncher : MonoBehaviour
{
	[Header("COMPONENT CONFIG")]
	[Tooltip("Check this box to automatically populate the macro handler list in the editor.")]
	[SerializeField] bool _fillMacroHandlerList = false;
	
	[Header("MACRO HANDLER LIST")]
	[SerializeField] List<UOMacroHandler> _macros = new List<UOMacroHandler>();
	public List<UOMacroHandler> macros => _macros;
	
	protected void OnValidate()
	{
		if (_fillMacroHandlerList)
		{
			_fillMacroHandlerList = false;
			Initialize();
		}
	}
	
	//INITIALIZE
	public void Initialize()
	{
		macros.Clear();
		FillMacroList();
	}
	
	//RUN MACRO
	public bool Run(string macroName)
	{
		HotkeyAction macroToRun;
		if (Enum.TryParse<HotkeyAction>(macroName, true, out macroToRun))
		{
			return Run(macroToRun);
		}
		
		return false;
	}
	public bool Run(HotkeyAction macroToHandle)
	{
		UOMacroHandler handler;
		bool handled = false;
		foreach (UOMacroHandler macro in macros)
		{
			if (macro.macro == macroToHandle)
			{
				ExecuteAction(macro.action);
				handled = true;
			}
		}
		return handled;
	}
	
	//EXECUTE ACTION
	private void ExecuteAction(Action actionToExecute)
	{
		if (actionToExecute != null) actionToExecute.Invoke();
	}
	
	//ADD MACRO HANDLER
	private void Add(HotkeyAction macroToAdd, Action actionToAdd)
	{
		macros.Add(new UOMacroHandler(macroToAdd, actionToAdd));
	}
	
	//FILL MACRO LIST
	private void FillMacroList()
	{
		FillMacroListWithSpells();
		FillMacroListWithAbilities();
		FillMacroListWithSkills();
	}
	
	//FILL SKILLS
	private void FillMacroListWithSkills()
	{
		//SKILLS
		//Add(HotkeyAction.UseSkillAlchemy, () => GameActions.UseSkill((int)UOSkill.alchemy));
		Add(HotkeyAction.UseSkillAnatomy, () => GameActions.UseSkill((int)UOSkill.anatomy));
		Add(HotkeyAction.UseSkillAnimalLore, () => GameActions.UseSkill((int)UOSkill.animallore));
		Add(HotkeyAction.UseSkillAnimalTaming, () => GameActions.UseSkill((int)UOSkill.animaltaming));
		//Add(HotkeyAction.UseSkillArchery, () => GameActions.UseSkill((int)UOSkill.archery));
		Add(HotkeyAction.UseSkillArmsLore, () => GameActions.UseSkill((int)UOSkill.armslore));
		Add(HotkeyAction.UseSkillBegging, () => GameActions.UseSkill((int)UOSkill.begging));
		//Add(HotkeyAction.UseSkillBlacksmithing, () => GameActions.UseSkill((int)UOSkill.blacksmithing));
		//Add(HotkeyAction.UseSkillBushido, () => GameActions.UseSkill((int)UOSkill.bushido));
		//Add(HotkeyAction.UseSkillCamping, () => GameActions.UseSkill((int)UOSkill.camping));
		//Add(HotkeyAction.UseSkillCarpentry, () => GameActions.UseSkill((int)UOSkill.carpentry));
		Add(HotkeyAction.UseSkillCartography, () => GameActions.UseSkill((int)UOSkill.cartography));
		//Add(HotkeyAction.UseSkillChivalry, () => GameActions.UseSkill((int)UOSkill.chivalry));
		//Add(HotkeyAction.UseSkillCooking, () => GameActions.UseSkill((int)UOSkill.cooking));
		Add(HotkeyAction.UseSkillDetectingHidden, () => GameActions.UseSkill((int)UOSkill.detecthidden));
		Add(HotkeyAction.UseSkillEnticement, () => GameActions.UseSkill((int)UOSkill.discordance));
		Add(HotkeyAction.UseSkillEvaluatingIntelligence, () => GameActions.UseSkill((int)UOSkill.evalint));
		//Add(HotkeyAction.UseSkillFencing, () => GameActions.UseSkill((int)UOSkill.fencing));
		//Add(HotkeyAction.UseSkillFishing, () => GameActions.UseSkill((int)UOSkill.fishing));
		//Add(HotkeyAction.UseSkillFletching, () => GameActions.UseSkill((int)UOSkill.fletching));
		//Add(HotkeyAction.UseSkillFocus, () => GameActions.UseSkill((int)UOSkill.focus));
		Add(HotkeyAction.UseSkillForensicEvaluation, () => GameActions.UseSkill((int)UOSkill.forensicevaluation));
		//Add(HotkeyAction.UseSkillHealing, () => GameActions.UseSkill((int)UOSkill.healing));
		//Add(HotkeyAction.UseSkillHerding, () => GameActions.UseSkill((int)UOSkill.herding));
		Add(HotkeyAction.UseSkillHiding, () => GameActions.UseSkill((int)UOSkill.hiding));
		Add(HotkeyAction.UseSkillImbuing, () => GameActions.UseSkill((int)UOSkill.imbuing));
		Add(HotkeyAction.UseSkillInscription, () => GameActions.UseSkill((int)UOSkill.inscription));
		Add(HotkeyAction.UseSkillItemIdentificator, () => GameActions.UseSkill((int)UOSkill.itemid));
		//Add(HotkeyAction.UseSkillLockpicking, () => GameActions.UseSkill((int)UOSkill.lockpicking));
		//Add(HotkeyAction.UseSkillLumberjacking, () => GameActions.UseSkill((int)UOSkill.lumberjacking));
		//Add(HotkeyAction.UseSkillMaceFighting, () => GameActions.UseSkill((int)UOSkill.macefighting));
		//Add(HotkeyAction.UseSkillMagery, () => GameActions.UseSkill((int)UOSkill.magery));
		Add(HotkeyAction.UseSkillMeditation, () => GameActions.UseSkill((int)UOSkill.meditation));
		//Add(HotkeyAction.UseSkillMining, () => GameActions.UseSkill((int)UOSkill.mining));
		//Add(HotkeyAction.UseSkillMusicianship, () => GameActions.UseSkill((int)UOSkill.musicianship));
		//Add(HotkeyAction.UseSkillNecromancy, () => GameActions.UseSkill((int)UOSkill.necromancy));
		//Add(HotkeyAction.UseSkillNinjitsu, () => GameActions.UseSkill((int)UOSkill.ninjitsu));
		//Add(HotkeyAction.UseSkillParry, () => GameActions.UseSkill((int)UOSkill.parry));
		Add(HotkeyAction.UseSkillPeacemaking, () => GameActions.UseSkill((int)UOSkill.peacemaking));
		Add(HotkeyAction.UseSkillPoisoning, () => GameActions.UseSkill((int)UOSkill.poisoning));
		Add(HotkeyAction.UseSkillProvocation, () => GameActions.UseSkill((int)UOSkill.provocation));
		Add(HotkeyAction.UseSkillRemoveTrap, () => GameActions.UseSkill((int)UOSkill.removetrap));
		//Add(HotkeyAction.UseSkillResistingSpells, () => GameActions.UseSkill((int)UOSkill.resistingspells));
		//Add(HotkeyAction.UseSkillSnooping, () => GameActions.UseSkill((int)UOSkill.snooping));
		Add(HotkeyAction.UseSkillSpiritSpeak, () => GameActions.UseSkill((int)UOSkill.spiritspeak));
		Add(HotkeyAction.UseSkillStealing, () => GameActions.UseSkill((int)UOSkill.stealing));
		Add(HotkeyAction.UseSkillStealth, () => GameActions.UseSkill((int)UOSkill.stealth));
		//Add(HotkeyAction.UseSkillSwordsmanship, () => GameActions.UseSkill((int)UOSkill.swordsmanship));
		//Add(HotkeyAction.UseSkillTactics, () => GameActions.UseSkill((int)UOSkill.tactics));
		//Add(HotkeyAction.UseSkillTailoring, () => GameActions.UseSkill((int)UOSkill.tailoring));
		Add(HotkeyAction.UseSkillTasteIdentification, () => GameActions.UseSkill((int)UOSkill.tasteid));
		//Add(HotkeyAction.UseSkillTinkering, () => GameActions.UseSkill((int)UOSkill.tinkering));
		Add(HotkeyAction.UseSkillTracking, () => GameActions.UseSkill((int)UOSkill.tracking));
		//Add(HotkeyAction.UseSkillVeterinary, () => GameActions.UseSkill((int)UOSkill.veterinary));
		//Add(HotkeyAction.UseSkillWrestling, () => GameActions.UseSkill((int)UOSkill.wrestling));
	}
	//FILL ABILITIES
	private void FillMacroListWithAbilities()
	{
		
		//WEAPON ABILITIES
		Add(HotkeyAction.UsePrimaryAbility, () => GameActions.UsePrimaryAbility());
		Add(HotkeyAction.UseSecondaryAbility, () => GameActions.UseSecondaryAbility());

		//VIRTUE ABILITIES
		//Add(HotkeyAction.UseVirtueHonor, () => GameActions.UseSkill());
		//Add(HotkeyAction.UseVirtueSacrifice, () => GameActions.UseSkill());
		//Add(HotkeyAction.UseVirtueValor, () => GameActions.UseSkill());
	}
	//FILL SPELLS
	private void FillMacroListWithSpells()
	{
		//MAGERY
		//1
		Add(HotkeyAction.CastClumsy, () => GameActions.CastSpell((int)UOSpell.Clumsy));
		Add(HotkeyAction.CastCreateFood, () => GameActions.CastSpell((int)UOSpell.CreateFood));
		Add(HotkeyAction.CastFeeblemind, () => GameActions.CastSpell((int)UOSpell.Feeblemind));
		Add(HotkeyAction.CastHeal, () => GameActions.CastSpell((int)UOSpell.Heal));
		Add(HotkeyAction.CastMagicArrow, () => GameActions.CastSpell((int)UOSpell.MagicArrow));
		Add(HotkeyAction.CastNightSight, () => GameActions.CastSpell((int)UOSpell.NightSight));
		Add(HotkeyAction.CastReactiveArmor, () => GameActions.CastSpell((int)UOSpell.ReactiveArmor));
		Add(HotkeyAction.CastWeaken, () => GameActions.CastSpell((int)UOSpell.Weaken));
		//2
		Add(HotkeyAction.CastAgility, () => GameActions.CastSpell((int)UOSpell.Agility));
		Add(HotkeyAction.CastCunning, () => GameActions.CastSpell((int)UOSpell.Cunning));
		Add(HotkeyAction.CastCure, () => GameActions.CastSpell((int)UOSpell.Cure));
		Add(HotkeyAction.CastHarm, () => GameActions.CastSpell((int)UOSpell.Harm));
		Add(HotkeyAction.CastMagicTrap, () => GameActions.CastSpell((int)UOSpell.MagicTrap));
		Add(HotkeyAction.CastMagicUntrap, () => GameActions.CastSpell((int)UOSpell.MagicUntrap));
		Add(HotkeyAction.CastProtection, () => GameActions.CastSpell((int)UOSpell.Protection));
		Add(HotkeyAction.CastStrength, () => GameActions.CastSpell((int)UOSpell.Strength));
		//3
		Add(HotkeyAction.CastBless, () => GameActions.CastSpell((int)UOSpell.Bless));
		Add(HotkeyAction.CastFireball, () => GameActions.CastSpell((int)UOSpell.Fireball));
		Add(HotkeyAction.CastMagicLock, () => GameActions.CastSpell((int)UOSpell.MagicLock));
		Add(HotkeyAction.CastPosion, () => GameActions.CastSpell((int)UOSpell.Poison));
		Add(HotkeyAction.CastTelekinesis, () => GameActions.CastSpell((int)UOSpell.Telekinesis));
		Add(HotkeyAction.CastTeleport, () => GameActions.CastSpell((int)UOSpell.Teleport));
		Add(HotkeyAction.CastUnlock, () => GameActions.CastSpell((int)UOSpell.Unlock));
		Add(HotkeyAction.CastWallOfStone, () => GameActions.CastSpell((int)UOSpell.WallOfStone));
		//4
		Add(HotkeyAction.CastArchCure, () => GameActions.CastSpell((int)UOSpell.ArchCure));
		Add(HotkeyAction.CastArchProtection, () => GameActions.CastSpell((int)UOSpell.ArchProtection));
		Add(HotkeyAction.CastCurse, () => GameActions.CastSpell((int)UOSpell.Curse));
		Add(HotkeyAction.CastFireField, () => GameActions.CastSpell((int)UOSpell.FireField));
		Add(HotkeyAction.CastGreaterHeal, () => GameActions.CastSpell((int)UOSpell.GreaterHeal));
		Add(HotkeyAction.CastLightning, () => GameActions.CastSpell((int)UOSpell.Lightning));
		Add(HotkeyAction.CastManaDrain, () => GameActions.CastSpell((int)UOSpell.ManaDrain));
		Add(HotkeyAction.CastRecall, () => GameActions.CastSpell((int)UOSpell.Recall));
		//5
		Add(HotkeyAction.CastBladeSpirits, () => GameActions.CastSpell((int)UOSpell.BladeSpirits));
		Add(HotkeyAction.CastDispelField, () => GameActions.CastSpell((int)UOSpell.DispelField));
		Add(HotkeyAction.CastIncognito, () => GameActions.CastSpell((int)UOSpell.Incognito));
		Add(HotkeyAction.CastMagicReflection, () => GameActions.CastSpell((int)UOSpell.MagicReflection));
		Add(HotkeyAction.CastMindBlast, () => GameActions.CastSpell((int)UOSpell.MindBlast));
		Add(HotkeyAction.CastParalyze, () => GameActions.CastSpell((int)UOSpell.Paralyze));
		Add(HotkeyAction.CastPoisonField, () => GameActions.CastSpell((int)UOSpell.PoisonField));
		Add(HotkeyAction.CastSummonCreature, () => GameActions.CastSpell((int)UOSpell.SummonCreature));
		//6
		Add(HotkeyAction.CastDispel, () => GameActions.CastSpell((int)UOSpell.Dispel));
		Add(HotkeyAction.CastEnergyBolt, () => GameActions.CastSpell((int)UOSpell.EnergyBolt));
		Add(HotkeyAction.CastExplosion, () => GameActions.CastSpell((int)UOSpell.Explosion));
		Add(HotkeyAction.CastInvisibility, () => GameActions.CastSpell((int)UOSpell.Invisibility));
		Add(HotkeyAction.CastMark, () => GameActions.CastSpell((int)UOSpell.Mark));
		Add(HotkeyAction.CastMassCurse, () => GameActions.CastSpell((int)UOSpell.MassCurse));
		Add(HotkeyAction.CastParalyzeField, () => GameActions.CastSpell((int)UOSpell.ParalyzeField));
		Add(HotkeyAction.CastReveal, () => GameActions.CastSpell((int)UOSpell.Reveal));
		//7
		Add(HotkeyAction.CastChainLightning, () => GameActions.CastSpell((int)UOSpell.ChainLightning));
		Add(HotkeyAction.CastEnergyField, () => GameActions.CastSpell((int)UOSpell.EnergyField));
		Add(HotkeyAction.CastFlamestrike, () => GameActions.CastSpell((int)UOSpell.Flamestrike));
		Add(HotkeyAction.CastGateTravel, () => GameActions.CastSpell((int)UOSpell.GateTravel));
		Add(HotkeyAction.CastManaVampire, () => GameActions.CastSpell((int)UOSpell.ManaVampire));
		Add(HotkeyAction.CastMassDispel, () => GameActions.CastSpell((int)UOSpell.MassDispel));
		Add(HotkeyAction.CastMeteorSwam, () => GameActions.CastSpell((int)UOSpell.MeteorSwarm));
		Add(HotkeyAction.CastPolymorph, () => GameActions.CastSpell((int)UOSpell.Polymorph));
		//8
		Add(HotkeyAction.CastEarthquake, () => GameActions.CastSpell((int)UOSpell.Earthquake));
		Add(HotkeyAction.CastEnergyVortex, () => GameActions.CastSpell((int)UOSpell.EnergyVortex));
		Add(HotkeyAction.CastResurrection, () => GameActions.CastSpell((int)UOSpell.Resurrection));
		Add(HotkeyAction.CastAirElemental, () => GameActions.CastSpell((int)UOSpell.AirElemental));
		Add(HotkeyAction.CastSummonDaemon, () => GameActions.CastSpell((int)UOSpell.SummonDaemon));
		Add(HotkeyAction.CastEarthElemental, () => GameActions.CastSpell((int)UOSpell.EarthElemental));
		Add(HotkeyAction.CastFireElemental, () => GameActions.CastSpell((int)UOSpell.FireElemental));
		Add(HotkeyAction.CastWaterElemental, () => GameActions.CastSpell((int)UOSpell.WaterElemental));

		//NECROMANCY
		Add(HotkeyAction.CastAnimatedDead, () => GameActions.CastSpell((int)UOSpell.AnimatedDead));
		Add(HotkeyAction.CastBloodOath, () => GameActions.CastSpell((int)UOSpell.BloodOath));
		Add(HotkeyAction.CastCorpseSkin, () => GameActions.CastSpell((int)UOSpell.CorpseSkin));
		Add(HotkeyAction.CastCurseWeapon, () => GameActions.CastSpell((int)UOSpell.CurseWeapon));
		Add(HotkeyAction.CastEvilOmen, () => GameActions.CastSpell((int)UOSpell.EvilOmen));
		Add(HotkeyAction.CastHorrificBeast, () => GameActions.CastSpell((int)UOSpell.HorrificBeast));
		Add(HotkeyAction.CastLichForm, () => GameActions.CastSpell((int)UOSpell.LichForm));
		Add(HotkeyAction.CastMindRot, () => GameActions.CastSpell((int)UOSpell.MindRot));
		Add(HotkeyAction.CastPainSpike, () => GameActions.CastSpell((int)UOSpell.PainSpike));
		Add(HotkeyAction.CastPoisonStrike, () => GameActions.CastSpell((int)UOSpell.PoisonStrike));
		Add(HotkeyAction.CastStrangle, () => GameActions.CastSpell((int)UOSpell.Strangle));
		Add(HotkeyAction.CastSummonFamiliar, () => GameActions.CastSpell((int)UOSpell.SummonFamiliar));
		Add(HotkeyAction.CastVampiricEmbrace, () => GameActions.CastSpell((int)UOSpell.VampiricEmbrace));
		Add(HotkeyAction.CastVangefulSpririt, () => GameActions.CastSpell((int)UOSpell.VengefulSpirit));
		Add(HotkeyAction.CastWither, () => GameActions.CastSpell((int)UOSpell.Wither));
		Add(HotkeyAction.CastWraithForm, () => GameActions.CastSpell((int)UOSpell.WraithForm));
		Add(HotkeyAction.CastExorcism, () => GameActions.CastSpell((int)UOSpell.Exorcism));

		//CHIVALRY
		Add(HotkeyAction.CastCleanseByFire, () => GameActions.CastSpell((int)UOSpell.CleanseByFire));
		Add(HotkeyAction.CastCloseWounds, () => GameActions.CastSpell((int)UOSpell.CloseWounds));
		Add(HotkeyAction.CastConsecrateWeapon, () => GameActions.CastSpell((int)UOSpell.ConsecrateWeapon));
		Add(HotkeyAction.CastDispelEvil, () => GameActions.CastSpell((int)UOSpell.DispelEvil));
		Add(HotkeyAction.CastDivineFury, () => GameActions.CastSpell((int)UOSpell.DivineFury));
		Add(HotkeyAction.CastEnemyOfOne, () => GameActions.CastSpell((int)UOSpell.EnemyOfOne));
		Add(HotkeyAction.CastHolyLight, () => GameActions.CastSpell((int)UOSpell.HolyLight));
		Add(HotkeyAction.CastNobleSacrifice, () => GameActions.CastSpell((int)UOSpell.NobleSacrifice));
		Add(HotkeyAction.CastRemoveCurse, () => GameActions.CastSpell((int)UOSpell.RemoveCurse));
		Add(HotkeyAction.CastSacredJourney, () => GameActions.CastSpell((int)UOSpell.SacredJourney));

		//BUSHIDO
		Add(HotkeyAction.CastHonorableExecution, () => GameActions.CastSpell((int)UOSpell.HonorableExecution));
		Add(HotkeyAction.CastConfidence, () => GameActions.CastSpell((int)UOSpell.Confidence));
		Add(HotkeyAction.CastEvasion, () => GameActions.CastSpell((int)UOSpell.Evasion));
		Add(HotkeyAction.CastCounterAttack, () => GameActions.CastSpell((int)UOSpell.CounterAttack));
		Add(HotkeyAction.CastLightningStrike, () => GameActions.CastSpell((int)UOSpell.LightningStrike));
		Add(HotkeyAction.CastMomentumStrike, () => GameActions.CastSpell((int)UOSpell.MomentumStrike));

		//NINJITSU
		Add(HotkeyAction.CastFocusAttack, () => GameActions.CastSpell((int)UOSpell.FocusAttack));
		Add(HotkeyAction.CastDeathStrike, () => GameActions.CastSpell((int)UOSpell.DeathStrike));
		Add(HotkeyAction.CastAnimalForm, () => GameActions.CastSpell((int)UOSpell.AnimalForm));
		Add(HotkeyAction.CastKiAttack, () => GameActions.CastSpell((int)UOSpell.KiAttack));
		Add(HotkeyAction.CastSurpriseAttack, () => GameActions.CastSpell((int)UOSpell.SurpriseAttack));
		Add(HotkeyAction.CastBackstab, () => GameActions.CastSpell((int)UOSpell.Backstab));
		Add(HotkeyAction.CastShadowjump, () => GameActions.CastSpell((int)UOSpell.Shadowjump));
		Add(HotkeyAction.CastMirrorImage, () => GameActions.CastSpell((int)UOSpell.MirrorImage));

		//SPELLWEAVING
		Add(HotkeyAction.CastArcaneCircle, () => GameActions.CastSpell((int)UOSpell.ArcaneCircle));
		Add(HotkeyAction.CastGiftOfRenewal, () => GameActions.CastSpell((int)UOSpell.GiftOfRenewal));
		Add(HotkeyAction.CastImmolatingWeapon, () => GameActions.CastSpell((int)UOSpell.ImmolatingWeapon));
		Add(HotkeyAction.CastAttuneWeapon, () => GameActions.CastSpell((int)UOSpell.AttuneWeapon));
		Add(HotkeyAction.CastThinderstorm, () => GameActions.CastSpell((int)UOSpell.Thunderstorm));
		Add(HotkeyAction.CastNaturesFury, () => GameActions.CastSpell((int)UOSpell.NaturesFury));
		Add(HotkeyAction.CastSummonFey, () => GameActions.CastSpell((int)UOSpell.SummonFey));
		Add(HotkeyAction.CastSummonFiend, () => GameActions.CastSpell((int)UOSpell.SummonFiend));
		Add(HotkeyAction.CastReaperForm, () => GameActions.CastSpell((int)UOSpell.ReaperForm));
		Add(HotkeyAction.CastWildFire, () => GameActions.CastSpell((int)UOSpell.WildFire));
		Add(HotkeyAction.CastEssenceOfWind, () => GameActions.CastSpell((int)UOSpell.EssenceOfWind));
		Add(HotkeyAction.CastDryadAllure, () => GameActions.CastSpell((int)UOSpell.DryadAllure));
		Add(HotkeyAction.CastEtherealVoyage, () => GameActions.CastSpell((int)UOSpell.EtherealVoyage));
		Add(HotkeyAction.CastWordOfDeath, () => GameActions.CastSpell((int)UOSpell.WordOfDeath));
		Add(HotkeyAction.CastGiftOfLife, () => GameActions.CastSpell((int)UOSpell.GiftOfLife));

		//MYSTICISM
		Add(HotkeyAction.CastNetherBolt, () => GameActions.CastSpell((int)UOSpell.NetherBolt));
		Add(HotkeyAction.CastHealingStone, () => GameActions.CastSpell((int)UOSpell.HealingStone));
		Add(HotkeyAction.CastPurgeMagic, () => GameActions.CastSpell((int)UOSpell.PurgeMagic));
		Add(HotkeyAction.CastEnchant, () => GameActions.CastSpell((int)UOSpell.Enchant));
		Add(HotkeyAction.CastSleep, () => GameActions.CastSpell((int)UOSpell.Sleep));
		Add(HotkeyAction.CastEagleStrike, () => GameActions.CastSpell((int)UOSpell.EagleStrike));
		Add(HotkeyAction.CastAnimatedWeapon, () => GameActions.CastSpell((int)UOSpell.AnimatedWeapon));
		Add(HotkeyAction.CastStoneForm, () => GameActions.CastSpell((int)UOSpell.StoneForm));
		Add(HotkeyAction.CastSpellTrigger, () => GameActions.CastSpell((int)UOSpell.SpellTrigger));
		Add(HotkeyAction.CastMassSleep, () => GameActions.CastSpell((int)UOSpell.MassSleep));
		Add(HotkeyAction.CastCleansingWinds, () => GameActions.CastSpell((int)UOSpell.CleansingWinds));
		Add(HotkeyAction.CastBombard, () => GameActions.CastSpell((int)UOSpell.Bombard));
		Add(HotkeyAction.CastSpellPlague, () => GameActions.CastSpell((int)UOSpell.SpellPlague));
		Add(HotkeyAction.CastHailStorm, () => GameActions.CastSpell((int)UOSpell.HailStorm));
		Add(HotkeyAction.CastNetherCyclone, () => GameActions.CastSpell((int)UOSpell.NetherCyclone));
		Add(HotkeyAction.CastRisingColossus, () => GameActions.CastSpell((int)UOSpell.RisingColossus));

		//BARD
		Add(HotkeyAction.CastInspire, () => GameActions.CastSpell((int)UOSpell.Inspire));
		Add(HotkeyAction.CastInvigorate, () => GameActions.CastSpell((int)UOSpell.Invigorate));
		Add(HotkeyAction.CastResilience, () => GameActions.CastSpell((int)UOSpell.Resilience));
		Add(HotkeyAction.CastPerseverance, () => GameActions.CastSpell((int)UOSpell.Perseverance));
		Add(HotkeyAction.CastTribulation, () => GameActions.CastSpell((int)UOSpell.Tribulation));
		Add(HotkeyAction.CastDespair, () => GameActions.CastSpell((int)UOSpell.Despair));

		//MASTERY
		Add(HotkeyAction.CastDeathRay, () => GameActions.CastSpell((int)UOSpell.DeathRay));
		Add(HotkeyAction.CastEtherealBurst, () => GameActions.CastSpell((int)UOSpell.EtherealBurst));
		Add(HotkeyAction.CastNetherBlast, () => GameActions.CastSpell((int)UOSpell.NetherBlast));
		Add(HotkeyAction.CastMysticWeapon, () => GameActions.CastSpell((int)UOSpell.MysticWeapon));
		Add(HotkeyAction.CastCommandUndead, () => GameActions.CastSpell((int)UOSpell.CommandUndead));
		Add(HotkeyAction.CastConduit, () => GameActions.CastSpell((int)UOSpell.Conduit));
		Add(HotkeyAction.CastManaShield, () => GameActions.CastSpell((int)UOSpell.ManaShield));
		Add(HotkeyAction.CastSummonReaper, () => GameActions.CastSpell((int)UOSpell.SummonReaper));
		Add(HotkeyAction.CastEnchantedSummoning, () => GameActions.CastSpell((int)UOSpell.EnchantedSummoning));
		Add(HotkeyAction.CastAnticipateHit, () => GameActions.CastSpell((int)UOSpell.AnticipateHit));
		Add(HotkeyAction.CastWarcry, () => GameActions.CastSpell((int)UOSpell.Warcry));
		Add(HotkeyAction.CastIntuition, () => GameActions.CastSpell((int)UOSpell.Intuition));
		Add(HotkeyAction.CastRejuvenate, () => GameActions.CastSpell((int)UOSpell.Rejuvenate));
		Add(HotkeyAction.CastHolyFist, () => GameActions.CastSpell((int)UOSpell.HolyFist));
		Add(HotkeyAction.CastShadow, () => GameActions.CastSpell((int)UOSpell.Shadow));
		Add(HotkeyAction.CastWhiteTigerForm, () => GameActions.CastSpell((int)UOSpell.WhiteTigerForm));
		Add(HotkeyAction.CastFlamingShot, () => GameActions.CastSpell((int)UOSpell.FlamingShot));
		Add(HotkeyAction.CastPlayingTheOdds, () => GameActions.CastSpell((int)UOSpell.PlayingTheOdds));
		Add(HotkeyAction.CastThrust, () => GameActions.CastSpell((int)UOSpell.Thrust));
		Add(HotkeyAction.CastPierce, () => GameActions.CastSpell((int)UOSpell.Pierce));
		Add(HotkeyAction.CastStagger, () => GameActions.CastSpell((int)UOSpell.Stagger));
		Add(HotkeyAction.CastToughness, () => GameActions.CastSpell((int)UOSpell.Toughness));
		Add(HotkeyAction.CastOnslaught, () => GameActions.CastSpell((int)UOSpell.Onslaught));
		Add(HotkeyAction.CastFocusedEye, () => GameActions.CastSpell((int)UOSpell.FocusedEye));
		Add(HotkeyAction.CastElementalFury, () => GameActions.CastSpell((int)UOSpell.ElementalFury));
		Add(HotkeyAction.CastCalledShot, () => GameActions.CastSpell((int)UOSpell.CalledShot));
		Add(HotkeyAction.CastWarriorsGifts, () => GameActions.CastSpell((int)UOSpell.WarriorsGift));
		Add(HotkeyAction.CastShieldBash, () => GameActions.CastSpell((int)UOSpell.ShieldBash));
		Add(HotkeyAction.CastBodyguard, () => GameActions.CastSpell((int)UOSpell.Bodyguard));
		Add(HotkeyAction.CastHeightenSenses, () => GameActions.CastSpell((int)UOSpell.HeightenSenses));
		Add(HotkeyAction.CastTolerance, () => GameActions.CastSpell((int)UOSpell.Tolerance));
		Add(HotkeyAction.CastInjectedStrike, () => GameActions.CastSpell((int)UOSpell.InjectedStrike));
		Add(HotkeyAction.CastPotency, () => GameActions.CastSpell((int)UOSpell.Potency));
		Add(HotkeyAction.CastRampage, () => GameActions.CastSpell((int)UOSpell.Rampage));
		Add(HotkeyAction.CastFistsofFury, () => GameActions.CastSpell((int)UOSpell.FistsOfFury));
		Add(HotkeyAction.CastKnockout, () => GameActions.CastSpell((int)UOSpell.Knockout));
		Add(HotkeyAction.CastWhispering, () => GameActions.CastSpell((int)UOSpell.Whispering));
		Add(HotkeyAction.CastCombatTraining, () => GameActions.CastSpell((int)UOSpell.CombatTraining));
		Add(HotkeyAction.CastBoarding, () => GameActions.CastSpell((int)UOSpell.Boarding));
	}
	/*private void OldFillMacroList()
	{
		//MAGERY
		Add(HotkeyAction.CastClumsy, () => GameActions.CastSpell(1));
		Add(HotkeyAction.CastCreateFood, () => GameActions.CastSpell(2));
		Add(HotkeyAction.CastFeeblemind, () => GameActions.CastSpell(3));
		Add(HotkeyAction.CastHeal, () => GameActions.CastSpell(4));
		Add(HotkeyAction.CastMagicArrow, () => GameActions.CastSpell(5));
		Add(HotkeyAction.CastNightSight, () => GameActions.CastSpell(6));
		Add(HotkeyAction.CastReactiveArmor, () => GameActions.CastSpell(7));
		Add(HotkeyAction.CastWeaken, () => GameActions.CastSpell(8));
		Add(HotkeyAction.CastAgility, () => GameActions.CastSpell(9));
		Add(HotkeyAction.CastCunning, () => GameActions.CastSpell(10));
		Add(HotkeyAction.CastCure, () => GameActions.CastSpell(11));
		Add(HotkeyAction.CastHarm, () => GameActions.CastSpell(12));
		Add(HotkeyAction.CastMagicTrap, () => GameActions.CastSpell(13));
		Add(HotkeyAction.CastMagicUntrap, () => GameActions.CastSpell(14));
		Add(HotkeyAction.CastProtection, () => GameActions.CastSpell(15));
		Add(HotkeyAction.CastStrength, () => GameActions.CastSpell(16));
		Add(HotkeyAction.CastBless, () => GameActions.CastSpell(17));
		Add(HotkeyAction.CastFireball, () => GameActions.CastSpell(18));
		Add(HotkeyAction.CastMagicLock, () => GameActions.CastSpell(19));
		Add(HotkeyAction.CastPosion, () => GameActions.CastSpell(20));
		Add(HotkeyAction.CastTelekinesis, () => GameActions.CastSpell(21));
		Add(HotkeyAction.CastTeleport, () => GameActions.CastSpell(22));
		Add(HotkeyAction.CastUnlock, () => GameActions.CastSpell(23));
		Add(HotkeyAction.CastWallOfStone, () => GameActions.CastSpell(24));
		Add(HotkeyAction.CastArchCure, () => GameActions.CastSpell(25));
		Add(HotkeyAction.CastArchProtection, () => GameActions.CastSpell(26));
		Add(HotkeyAction.CastCurse, () => GameActions.CastSpell(27));
		Add(HotkeyAction.CastFireField, () => GameActions.CastSpell(28));
		Add(HotkeyAction.CastGreaterHeal, () => GameActions.CastSpell(29));
		Add(HotkeyAction.CastLightning, () => GameActions.CastSpell(30));
		Add(HotkeyAction.CastManaDrain, () => GameActions.CastSpell(31));
		Add(HotkeyAction.CastRecall, () => GameActions.CastSpell(32));
		Add(HotkeyAction.CastBladeSpirits, () => GameActions.CastSpell(33));
		Add(HotkeyAction.CastDispelField, () => GameActions.CastSpell(34));
		Add(HotkeyAction.CastIncognito, () => GameActions.CastSpell(35));
		Add(HotkeyAction.CastMagicReflection, () => GameActions.CastSpell(36));
		Add(HotkeyAction.CastMindBlast, () => GameActions.CastSpell(37));
		Add(HotkeyAction.CastParalyze, () => GameActions.CastSpell(38));
		Add(HotkeyAction.CastPoisonField, () => GameActions.CastSpell(39));
		Add(HotkeyAction.CastSummonCreature, () => GameActions.CastSpell(40));
		Add(HotkeyAction.CastDispel, () => GameActions.CastSpell(41));
		Add(HotkeyAction.CastEnergyBolt, () => GameActions.CastSpell(42));
		Add(HotkeyAction.CastExplosion, () => GameActions.CastSpell(43));
		Add(HotkeyAction.CastInvisibility, () => GameActions.CastSpell(44));
		Add(HotkeyAction.CastMark, () => GameActions.CastSpell(45));
		Add(HotkeyAction.CastMassCurse, () => GameActions.CastSpell(46));
		Add(HotkeyAction.CastParalyzeField, () => GameActions.CastSpell(47));
		Add(HotkeyAction.CastReveal, () => GameActions.CastSpell(48));
		Add(HotkeyAction.CastChainLightning, () => GameActions.CastSpell(49));
		Add(HotkeyAction.CastEnergyField, () => GameActions.CastSpell(50));
		Add(HotkeyAction.CastFlamestrike, () => GameActions.CastSpell(51));
		Add(HotkeyAction.CastGateTravel, () => GameActions.CastSpell(52));
		Add(HotkeyAction.CastManaVampire, () => GameActions.CastSpell(53));
		Add(HotkeyAction.CastMassDispel, () => GameActions.CastSpell(54));
		Add(HotkeyAction.CastMeteorSwam, () => GameActions.CastSpell(55));
		Add(HotkeyAction.CastPolymorph, () => GameActions.CastSpell(56));
		Add(HotkeyAction.CastEarthquake, () => GameActions.CastSpell(57));
		Add(HotkeyAction.CastEnergyVortex, () => GameActions.CastSpell(58));
		Add(HotkeyAction.CastResurrection, () => GameActions.CastSpell(59));
		Add(HotkeyAction.CastAirElemental, () => GameActions.CastSpell(60));
		Add(HotkeyAction.CastSummonDaemon, () => GameActions.CastSpell(61));
		Add(HotkeyAction.CastEarthElemental, () => GameActions.CastSpell(62));
		Add(HotkeyAction.CastFireElemental, () => GameActions.CastSpell(63));
		Add(HotkeyAction.CastWaterElemental, () => GameActions.CastSpell(64));

		//NECROMANCY
		Add(HotkeyAction.CastAnimatedDead, () => GameActions.CastSpell(101));
		Add(HotkeyAction.CastBloodOath, () => GameActions.CastSpell(102));
		Add(HotkeyAction.CastCorpseSkin, () => GameActions.CastSpell(103));
		Add(HotkeyAction.CastCurseWeapon, () => GameActions.CastSpell(104));
		Add(HotkeyAction.CastEvilOmen, () => GameActions.CastSpell(105));
		Add(HotkeyAction.CastHorrificBeast, () => GameActions.CastSpell(106));
		Add(HotkeyAction.CastLichForm, () => GameActions.CastSpell(107));
		Add(HotkeyAction.CastMindRot, () => GameActions.CastSpell(108));
		Add(HotkeyAction.CastPainSpike, () => GameActions.CastSpell(109));
		Add(HotkeyAction.CastPoisonStrike, () => GameActions.CastSpell(110));
		Add(HotkeyAction.CastStrangle, () => GameActions.CastSpell(111));
		Add(HotkeyAction.CastSummonFamiliar, () => GameActions.CastSpell(112));
		Add(HotkeyAction.CastVampiricEmbrace, () => GameActions.CastSpell(113));
		Add(HotkeyAction.CastVangefulSpririt, () => GameActions.CastSpell(114));
		Add(HotkeyAction.CastWither, () => GameActions.CastSpell(115));
		Add(HotkeyAction.CastWraithForm, () => GameActions.CastSpell(116));
		Add(HotkeyAction.CastExorcism, () => GameActions.CastSpell(117));

		//CHIVALRY
		Add(HotkeyAction.CastCleanseByFire, () => GameActions.CastSpell(201));
		Add(HotkeyAction.CastCloseWounds, () => GameActions.CastSpell(202));
		Add(HotkeyAction.CastConsecrateWeapon, () => GameActions.CastSpell(203));
		Add(HotkeyAction.CastDispelEvil, () => GameActions.CastSpell(204));
		Add(HotkeyAction.CastDivineFury, () => GameActions.CastSpell(205));
		Add(HotkeyAction.CastEnemyOfOne, () => GameActions.CastSpell(206));
		Add(HotkeyAction.CastHolyLight, () => GameActions.CastSpell(207));
		Add(HotkeyAction.CastNobleSacrifice, () => GameActions.CastSpell(208));
		Add(HotkeyAction.CastRemoveCurse, () => GameActions.CastSpell(209));
		Add(HotkeyAction.CastSacredJourney, () => GameActions.CastSpell(210));

		//BUSHIDO
		Add(HotkeyAction.CastHonorableExecution, () => GameActions.CastSpell(401));
		Add(HotkeyAction.CastConfidence, () => GameActions.CastSpell(402));
		Add(HotkeyAction.CastEvasion, () => GameActions.CastSpell(403));
		Add(HotkeyAction.CastCounterAttack, () => GameActions.CastSpell(404));
		Add(HotkeyAction.CastLightningStrike, () => GameActions.CastSpell(405));
		Add(HotkeyAction.CastMomentumStrike, () => GameActions.CastSpell(406));

		//NINJITSU
		Add(HotkeyAction.CastFocusAttack, () => GameActions.CastSpell(501));
		Add(HotkeyAction.CastDeathStrike, () => GameActions.CastSpell(502));
		Add(HotkeyAction.CastAnimalForm, () => GameActions.CastSpell(503));
		Add(HotkeyAction.CastKiAttack, () => GameActions.CastSpell(504));
		Add(HotkeyAction.CastSurpriseAttack, () => GameActions.CastSpell(505));
		Add(HotkeyAction.CastBackstab, () => GameActions.CastSpell(506));
		Add(HotkeyAction.CastShadowjump, () => GameActions.CastSpell(507));
		Add(HotkeyAction.CastMirrorImage, () => GameActions.CastSpell(508));

		//SPELLWEAVING
		Add(HotkeyAction.CastArcaneCircle, () => GameActions.CastSpell(601));
		Add(HotkeyAction.CastGiftOfRenewal, () => GameActions.CastSpell(602));
		Add(HotkeyAction.CastImmolatingWeapon, () => GameActions.CastSpell(603));
		Add(HotkeyAction.CastAttuneWeapon, () => GameActions.CastSpell(604));
		Add(HotkeyAction.CastThinderstorm, () => GameActions.CastSpell(605));
		Add(HotkeyAction.CastNaturesFury, () => GameActions.CastSpell(606));
		Add(HotkeyAction.CastSummonFey, () => GameActions.CastSpell(607));
		Add(HotkeyAction.CastSummonFiend, () => GameActions.CastSpell(608));
		Add(HotkeyAction.CastReaperForm, () => GameActions.CastSpell(609));
		Add(HotkeyAction.CastWildFire, () => GameActions.CastSpell(610));
		Add(HotkeyAction.CastEssenceOfWind, () => GameActions.CastSpell(611));
		Add(HotkeyAction.CastDryadAllure, () => GameActions.CastSpell(612));
		Add(HotkeyAction.CastEtherealVoyage, () => GameActions.CastSpell(613));
		Add(HotkeyAction.CastWordOfDeath, () => GameActions.CastSpell(614));
		Add(HotkeyAction.CastGiftOfLife, () => GameActions.CastSpell(615));

		//MYSTICISM
		Add(HotkeyAction.CastNetherBolt, () => GameActions.CastSpell(678));
		Add(HotkeyAction.CastHealingStone, () => GameActions.CastSpell(679));
		Add(HotkeyAction.CastPurgeMagic, () => GameActions.CastSpell(680));
		Add(HotkeyAction.CastEnchant, () => GameActions.CastSpell(681));
		Add(HotkeyAction.CastSleep, () => GameActions.CastSpell(682));
		Add(HotkeyAction.CastEagleStrike, () => GameActions.CastSpell(683));
		Add(HotkeyAction.CastAnimatedWeapon, () => GameActions.CastSpell(684));
		Add(HotkeyAction.CastStoneForm, () => GameActions.CastSpell(685));
		Add(HotkeyAction.CastSpellTrigger, () => GameActions.CastSpell(686));
		Add(HotkeyAction.CastMassSleep, () => GameActions.CastSpell(687));
		Add(HotkeyAction.CastCleansingWinds, () => GameActions.CastSpell(688));
		Add(HotkeyAction.CastBombard, () => GameActions.CastSpell(689));
		Add(HotkeyAction.CastSpellPlague, () => GameActions.CastSpell(690));
		Add(HotkeyAction.CastHailStorm, () => GameActions.CastSpell(691));
		Add(HotkeyAction.CastNetherCyclone, () => GameActions.CastSpell(692));
		Add(HotkeyAction.CastRisingColossus, () => GameActions.CastSpell(693));

		//BARD
		Add(HotkeyAction.CastInspire, () => GameActions.CastSpell(701));
		Add(HotkeyAction.CastInvigorate, () => GameActions.CastSpell(702));
		Add(HotkeyAction.CastResilience, () => GameActions.CastSpell(703));
		Add(HotkeyAction.CastPerseverance, () => GameActions.CastSpell(704));
		Add(HotkeyAction.CastTribulation, () => GameActions.CastSpell(705));
		Add(HotkeyAction.CastDespair, () => GameActions.CastSpell(706));

		//MASTERY
		Add(HotkeyAction.CastDeathRay, () => GameActions.CastSpell(707));
		Add(HotkeyAction.CastEtherealBurst, () => GameActions.CastSpell(708));
		Add(HotkeyAction.CastNetherBlast, () => GameActions.CastSpell(709));
		Add(HotkeyAction.CastMysticWeapon, () => GameActions.CastSpell(710));
		Add(HotkeyAction.CastCommandUndead, () => GameActions.CastSpell(711));
		Add(HotkeyAction.CastConduit, () => GameActions.CastSpell(712));
		Add(HotkeyAction.CastManaShield, () => GameActions.CastSpell(713));
		Add(HotkeyAction.CastSummonReaper, () => GameActions.CastSpell(714));
		Add(HotkeyAction.CastEnchantedSummoning, () => GameActions.CastSpell(715));
		Add(HotkeyAction.CastAnticipateHit, () => GameActions.CastSpell(716));
		Add(HotkeyAction.CastWarcry, () => GameActions.CastSpell(717));
		Add(HotkeyAction.CastIntuition, () => GameActions.CastSpell(718));
		Add(HotkeyAction.CastRejuvenate, () => GameActions.CastSpell(719));
		Add(HotkeyAction.CastHolyFist, () => GameActions.CastSpell(720));
		Add(HotkeyAction.CastShadow, () => GameActions.CastSpell(721));
		Add(HotkeyAction.CastWhiteTigerForm, () => GameActions.CastSpell(722));
		Add(HotkeyAction.CastFlamingShot, () => GameActions.CastSpell(723));
		Add(HotkeyAction.CastPlayingTheOdds, () => GameActions.CastSpell(724));
		Add(HotkeyAction.CastThrust, () => GameActions.CastSpell(725));
		Add(HotkeyAction.CastPierce, () => GameActions.CastSpell(726));
		Add(HotkeyAction.CastStagger, () => GameActions.CastSpell(727));
		Add(HotkeyAction.CastToughness, () => GameActions.CastSpell(728));
		Add(HotkeyAction.CastOnslaught, () => GameActions.CastSpell(729));
		Add(HotkeyAction.CastFocusedEye, () => GameActions.CastSpell(730));
		Add(HotkeyAction.CastElementalFury, () => GameActions.CastSpell(731));
		Add(HotkeyAction.CastCalledShot, () => GameActions.CastSpell(732));
		Add(HotkeyAction.CastWarriorsGifts, () => GameActions.CastSpell(733));
		Add(HotkeyAction.CastShieldBash, () => GameActions.CastSpell(734));
		Add(HotkeyAction.CastBodyguard, () => GameActions.CastSpell(735));
		Add(HotkeyAction.CastHeightenSenses, () => GameActions.CastSpell(736));
		Add(HotkeyAction.CastTolerance, () => GameActions.CastSpell(737));
		Add(HotkeyAction.CastInjectedStrike, () => GameActions.CastSpell(738));
		Add(HotkeyAction.CastPotency, () => GameActions.CastSpell(739));
		Add(HotkeyAction.CastRampage, () => GameActions.CastSpell(740));
		Add(HotkeyAction.CastFistsofFury, () => GameActions.CastSpell(741));
		Add(HotkeyAction.CastKnockout, () => GameActions.CastSpell(742));
		Add(HotkeyAction.CastWhispering, () => GameActions.CastSpell(743));
		Add(HotkeyAction.CastCombatTraining, () => GameActions.CastSpell(744));
		Add(HotkeyAction.CastBoarding, () => GameActions.CastSpell(745));
		
		//WEAPON ABILITIES
		Add(HotkeyAction.UsePrimaryAbility, () => GameActions.UsePrimaryAbility());
		Add(HotkeyAction.UseSecondaryAbility, () => GameActions.UseSecondaryAbility());
		
		//SKILLS
		Add(HotkeyAction.UseSkillAnatomy, () => GameActions.UseSkill((int)UOSkill.anatomy));
		
		
		
		//VIRTUE ABILITIES
		//Add(HotkeyAction.UseVirtueHonor, () => GameActions.UseSkill());
		//Add(HotkeyAction.UseVirtueSacrifice, () => GameActions.UseSkill());
		//Add(HotkeyAction.UseVirtueValor, () => GameActions.UseSkill());
	}*/
}
