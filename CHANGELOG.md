## 1.15.2
Fixed 'Show_Only_On_Tab' setting not working.  
'Close windows on input' no longer closes on: Enter, F2, BackQuote  
Added config to make internal buffs visible.  

## 1.15.1
Readded 'orig(self)' to item counters.  
Fixed issue where having some modded item tiers breaks the item counter.  
Fixed Crafting sorting being off breaking crafting.

Added stats for scrap as Drifter.  
Added option to have StatsDisplay only when scoreboard is closed.  

## 1.15.0
Condensed Stat Display & sorting options, to be more managable to use in RiskOfOptions. *(14 configs turned into 4)*


Added option to disable all other info boxes when scoreboard is open, and only show stats.   
	- *For modpacks that have too many mods using that space.*   
	- Preset to go alongside it.  


Command & Pickup menus now show tooltips with Controllers.  
Added Cooldown & ProcCoeff to skills loadout in lobby.  
Added proper, local, Wandering Chef sorting.  
Added [damagePercent], [damagePercentWithWatch] stat. *(damage increases as %)*  
Added [maxHpPercent], [maxHealthStat], [maxShieldStat] stat.  *(hp/health/shield increases as %)*  
Added [shieldFraction] stat.	*(% of CombinedHP that is shield)*  
Added config to disable item procs in skill descriptions.  
Access Node and Artifact Portal now tracked by [portals] stat


Expanded Item icons no longer all update every item pickup.\
Expanded Equip icons no longer update every frame.\
Expanded Skill icons no longer update every frame.\
Expanded Item Counters no longer update every frame when scoreboard is open.\
	- Instead, just update when Scoreboard is opened, or item counts change.\
	- Should be more optimized, report any cases where something isn't updating.
 

Total Temp Item counter config should now work even if Tiered Counters are disabled.  
DPS/Kills/Combo no longer counts minion damage.   
Permanent Skill/Equip cooldown should work a bit better in general.  


Fixed PermamentEquipmentCooldown getting stuck showing 0.  
Fixed Command menu controller navigation getting broken by mod.  
Fixed DPS meter counting kills multiple times on a lot of attacks.  
Fixed maxRunKillCombo not getting reset per run.  
Fixed stat display being empty when opening a menu while paused.  
Fixed spacing on the last line of the stat display.  
Fixed closing command/scrapper menu on key press not working consistently.  
Fixed closing menu & translucent menu config not working on Drone Scrapper.     
Fixed Stew Damage% stat being a bit off or weird at times.  

 
## 1.14.4
Regen Stat on items now multiplied by difficulties regen modifier.   
	- *(Like how luck affected stats are treated)*  

Regen Stat in stat display now halved on E5+ & multiplied by RejuRacks to better show the actual amount you're healing.  
	- [regenRaw] added to show the unmodified stat.  

Seperate config for Drone info when pinging.   
Drones now show full info with full description config.    
 
Fixed Encrypted Portal never disappearing in stats.  
Fixed a rare nullref for tooltips.   
Fixed BarrierDecay stat not working.  
Removed crafting sorting for networking related reasons.  


## 1.14.3
Fixed incompat with RiskUI  
Fix for tooltips not working in artifact of command  

## 1.14.2
Added config option for sorting items in crafting stations specifically.  
Added meal tier to default sorting options  

## 1.14.1
Added config options for temporary items.  
Added option to show item descriptions when another player pings an item.  
Added option to show item descriptions for hidden items.  


## 1.14.0
Added support for DLC3. (Fixes, Items, Skills, Portal, Buffs)  
Pinging Drones now displays info, like items.  
Hovering over allies now shows info, like items.  

Tier Item counts now hidden until you have 1 of that tier. (Other than main 3 tiers)  
-*(To avoid showing counts for tiers that may not be accessible. (Void, Meal)*
 
Sorts chef by available and unavailable.    
Hid Item Counts on scrappers, as vanilla has that feature.  
Hid Item Counts for unavilable options. (ie locked items in command)  
Hid Item Counts on equipment options.  
Command Resize is more compact like vanilla.  
Command Resize increased minimum size (to fit text).  
Command Resize no longer multiplies ui color.  
 

## 1.13.1
Fixed Command sorting being locked behind "Potential & Fragment" sorting.\
Fixed Command sorting not using the Command settings.\
Fixed [baseDamage] stat being gone.\
Fixed old default settings not actually updating automatically.

Added option to only have Stats Display when scoreboard is open.\
Added stats: portals, ping, lvl1_damage, lvl1_maxHealth.


## 1.13.0
Updated Elusive Antler, Growth Nectar, Bolstering Lantern stats to match updates they recieved.\
Added Stats for Longstanding Solitude, Unstable Transmitter, some Untiered Items, Spinel Tonic.\
Added some extra stats to some items. (More unlisted stats, Max Boost, more item specific damage/cooldowns)


Added Proc Coeff info to Items.\
Added Proc Coeff info to Equipment.\
Updated/Added Proc Coeffs for Phase 3 Skills & Heretic\
Hid item procs on 0 proc & movement skills.\
Added Electric Boomerang, Runic Lens, Leech Seed, Scythe to items skills proc.

Equipment now show base cooldown like skills.\
Skills now show cooldown reduction like Equipment.\
Hid baseCooldown & cooldown reduction if 0%.
 
\
Added Stat Display Presets to reset it more easily in game and add alternative variants to suit different needs.\
Config to move purchase text so it doesnt clip into Stat Display. (Default: True)

Default Stat Display Config changes:\
\- Shrunk down stat display.\
\- Enabled Secondary stat display by default.\
\- Removed some stats or moved them to Tab only.\
\- Added marging to stat display

Should automatically update for people using the default one.

\
Buff Descriptions tooltip now buff colored instead just gray.\
Added buff descriptions for DLC2 Buffs.\
Added option to not have Decimal Buff Timers.

Items are no longer sorted on Death Screen.\
Items are no longer sorted in Fragments & Potentials.

Added stats: killCountRun, greenPortal, curseHealthReduction, effectiveHealth, effectiveMaxHealth, speedPercent, attackSpeedPercent stats.\
GoldPortal stat now accounts for Halcyonite Shrines

\
Fixed Buff names & descriptions not functioning\
Fixed some vanilla and most modded skills not showing cooldown.\
Fixed PermamentCooldown showing 0 if you had more than maxStock.\
\- (Lost a Fuel Cell, would permamently show 0 until used, various modded skills had this issue)\
Fixed 0 cooldown skills showing the 0 on the icon.\
Multiplied percentDps value by 100 so it's actually percent\
Fixed unstackable timed buffs showing stacks.\
Fixed listed CDR never going below 0.5 for AttackSpeedCDR Skills (Arti Primary), despite it not being capped at 0.5\
Fixed some item stats showing up wrong. (Mostly minor issues)\
\- Stone & Flux Pauldron listing 0% downsides\
\- Eulogy Zero stat showing it as LuckAffected when it isn't.\
\- War Bonds stats being on Long Standing Solitude.\
\- Meathook saying it caps despite being hyperbolic (on skills).\
\- Wording on Perferator/Slug not matching their effect.\
\- Lost Seers now properly says 200 to cap instead of 201.\
Fixed an issue with disabling Huds and Buff Icons.\
Fixed maxKillCombo not using proper stat.\
Fixed killCount not working on Clients.\
Fixed DPS & DPSPercent being inaccurate due to using long instead of float.\
Fixed hasOneShotProtection stat saying True at 100% Hp even with Shaped Glass.

-Wolfo

## 1.12.0
Functional with seekers patch 3.\
Thanks to Bumperdo09 for restructuing PickupPickerPanel sorting to work better with multiplayer.

## 1.11.0
Update to work with new patch

## 1.10.2
Potential fix for the statsDisplay randomly breaking.

## 1.10.1
Thanks to isolno for this update. \
Updated stats display to use regex calls to increase performance.\
Fixed velocity stat not working.\
Added percentDps stat.

## 1.10.0
Update to work with new patch.\
Fixed descriptions of changed items. Big thankies to isolNo for adding options for moving the statsdisplay around the screen.\
Adjusted safer spaces cooldown display, thanks itsschwer

## 1.9.10
Fixed niche error where a null inventory would cause sorting errors

## 1.9.9
Fixed incompat with RiskUI (RiskUI isn't technically updated but it might be soon)

## 1.9.8
Fixed issue with scoreboard not showing money.\
Maybe fixed issue with StatsDisplay stopping it's auto updates, but I could never reproduce the bug so idk.

## 1.9.7
Added some checks on SkillUpdate to be more lenient for skills without targetSkills (probably?).\
Added additional options for item sorting to use acquired order

## 1.9.6
Reworked the function to calculate skill cooldowns to be slightly (maybe more idk) more performant and easier to work with in the future.

## 1.9.5
Fixed proc coefficient for MUL-T.\
Finished adding them for the new survivors.

## 1.9.4
Fixed issue with "Input Disables Command Prompt" not working

## 1.9.3
Fixed typo with razorwire

## 1.9.2
Added config option to disable ability proc coefficients.\
Fixed currentCombatKills showing currentCombatDamage.

## 1.9.1
Huge thanks to timoreo22 for doing the research for the new item descriptions. Yes a lot of them don't match the in-game description, blame Gearbox for not writing good descriptions I guess.

## 1.9.0
Quick fix for Seekers of the Storm, minor visual issues but it functions.

## 1.8.5
Fixed issue with CommandQueue (probably)

## 1.8.4
Added setting to uncap crit chance.\
Fixed Purity cooldown calculations being broken.\
Void item pickers have clearer text. 

## 1.8.3
Minor fix for pickuip descriptions on items without descriptions

## 1.8.2
Fixed default config for statsdisplay using baseDamage not damage

## 1.8.1
Fixed item sorting bugs with Regenerating Scrap.\
Re-added a few "for fun" item stats that got removed.\
Added option to show corrupted item info in the command menu.

## 1.8.0
Hude thanks to Warmek and SSM240 for 99% of this update.\
Added a new internal calculateValuesNew alternative to calculateValues for items.\
Fixed descriptions of items that have no descriptions.\
Added cooldown/proc information for skills.\
Added more options for item sorting, you can now sort them in very intricate ways if you so desire.\
Added stage to statsdisplay.

## 1.7.4
Fixed the wording slightly in the "one more" text.\
Added basic proc info to abilities.

## 1.7.3
Fixed math issue with items that have exponentially scaling cooldown reduction.

## 1.7.2
Quick fix for some calculations going in reverse if you exceed 100% chance

## 1.7.1
Big thanks to shirograhm for this update.\ 
Refactor of the backend for itemstats definitions/stats definitions to be much more readable.\
Adjusted definitions of some item stats to be more accurate.

## 1.7.0
Fixed equipment not always showing their full descriptions.\
Added option to adjust how long pickup display notifications last.\
Added difficultyCoefficient to StatsDisplay as an option.\
Made the item calculations for item stats optional.

## 1.6.3
Fixed healthPercentage not using the float precision option

## 1.6.2
Big thanks to shirograhm for most of these changes.\
uck is gathered in a more accurate way now, should work better with modded items that add luck.\
Added a check for buff definitions to just use their name if no definition (name/description) was provided.\
Fixed equipment not utilizing ally's stats when looking in the scoreboard.

## 1.6.1
Forgot to push option for Buff Timer size

## 1.6.0
Added Buff Definitions, hover over your buffs to see them. Buff definitions have to be pre-defined, so modded buffs need to utilize BuffDefinitions.RegisterBuff in order for their buffs to show up (this can technically be done one either side if you know the names of buffs, but it's extremely tedius with how many mods are out there).\
Added a few new items to StatsDisplay (maxComboThisRun, currentCombatKills, maxKillCombo, maxKillComboThisRun).\
Added font scale options for Item Stats.\
Currently thinking about a setup where you can create as many StatsDisplay tabs as you want and move them around (such as having one as a dedicated DPS meter if you wanted to), but nothing to show for it yet, look forward to it though!

## 1.5.2
Fixed a few bugs around command. Fixed a description conflict with Manuscript from mystic's items

## 1.5.1
Fixed sorting options for the equipment menus.\
Changed calculate function for ItemStats to use a CharacterMaster for better calculations in the future

## 1.5.0
Added sorting options to the command menu and scrapper menu.\
Added equip slot functionality for itemstats.

## 1.4.2
Fixed DPS meter not resetting if you leave while it still has values stored.\
Fixed transforming into specific mobs like a LesserWisp causing StatsDisplay to break until restart

## 1.4.1
Fixed StatsDisplay not working with RiskUI.\
Added an option for how many decimal points to be used for floating point values.

## 1.4.0
Fixed a rare bug with CleanHUD.\
Fixed issue with scrappers and other menu's scaling (as in we don't touch them anymore since it's not needed and makes stuff look wonky and small).\
Fixed buff timers not showing up all the time. We actually use the setting for item counters now.\
Probably fixed an issue with Minimap and Refightilization (but it was inconsistent so I'll be on the lookout).\
Updated StatsDisplay to scale better automatically, also supports manual scaling now (thanks yuukotf2 on github!).\
Added an option for full item descriptions on pickup (thanks SSM240 on github!).\
New option to show permanent cooldown numbers for skills too.\
Added a secondary StatsDisplay string which, when enabled, will be used when the scoreboard is held open.

## 1.3.2
Last update was fake, this one is real a cool, not cringe

## 1.3.1
Fixed issue with buff timers applying to all buffs. Added a few new items to the StatsDisplay syntax. Added an option for StatsDisplay to remove the default color formatting.

## 1.3.0
Added buff timers. Added option to show hidden items. Added permanent equip item timers. Added option for global resizing of StatsDisplay text size

## 1.2.6
I suck at math

## 1.2.5
I forgot but it probably didn't work

## 1.2.4
Fixed luck based calculations. Fixed command windows with controllers (again)

## 1.2.3
Fixed UI padding for the RiskUI section

## 1.2.2
Fixed resized command window behaving poorly with controllers

## 1.2.1
Fixed item sorting when only sorting scrap and nothing else.\
Added critWithLuck and bleedChanceWithLuck to the stats display stuff, which as stated, factors in your luck

## 1.2.0
Added item stats on ping.\
Added item counters.\
Fixed the armor coloring being cIsUtility instead of cIsHealing

## 1.1.0
Number go up

## 1.0.0
Initial release
