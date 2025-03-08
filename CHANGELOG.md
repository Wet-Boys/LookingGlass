### 1.11.0
Update to work with new patch

### 1.10.2
Potential fix for the statsDisplay randomly breaking.

### 1.10.1
Thanks to isolno for this update. Updated stats display to use regex calls to increase performance. Fixed velocity stat not working. Added percentDps stat.

### 1.10.0
Update to work with new patch. Fixed descriptions of changed items. Big thankies to isolNo for adding options for moving the statsdisplay around the screen. Adjusted safer spaces cooldown display, thanks itsschwer

### 1.9.10
Fixed niche error where a null inventory would cause sorting errors

### 1.9.9
Fixed incompat with RiskUI (RiskUI isn't technically updated but it might be soon)

### 1.9.8
Fixed issue with scoreboard not showing money. Maybe fixed issue with StatsDisplay stopping it's auto updates, but I could never reproduce the bug so idk.

### 1.9.7
Added some checks on SkillUpdate to be more lenient for skills without targetSkills (probably?). Added additional options for item sorting to use acquired order

### 1.9.6
Reworked the function to calculate skill cooldowns to be slightly (maybe more idk) more performant and easier to work with in the future.

### 1.9.5
Fixed proc coefficient for MUL-T. Finished adding them for the new survivors.

### 1.9.4
Fixed issue with "Input Disables Command Prompt" not working

### 1.9.3
Fixed typo with razorwire

### 1.9.2
Added config option to disable ability proc coefficients. fixed currentCombatKills showing currentCombatDamage.

### 1.9.1
Huge thanks to timoreo22 for doing the research for the new item descriptions. Yes a lot of them don't match the in-game description, blame Gearbox for not writing good descriptions I guess.

### 1.9.0
Quick fix for Seekers of the Storm, minor visual issues but it functions.

### 1.8.5
Fixed issue with CommandQueue (probably)

### 1.8.4
Added setting to uncap crit chance. Fixed Purity cooldown calculations being broken. Void item pickers have clearer text. 

### 1.8.3
Minor fix for pickuip descriptions on items without descriptions

### 1.8.2
Fixed default config for statsdisplay using baseDamage not damage

### 1.8.1
Fixed item sorting bugs with Regenerating Scrap. Re-added a few "for fun" item stats that got removed. Added option to show corrupted item info in the command menu.

### 1.8.0
Hude thanks to Warmek and SSM240 for 99% of this update. Added a new internal calculateValuesNew alternative to calculateValues for items. Fixed descriptions of items that have no descriptions. Added cooldown/proc information for skills. Added more options for item sorting, you can now sort them in very intricate ways if you so desire. Added stage to statsdisplay.

### 1.7.4
Fixed the wording slightly in the "one more" text. Added basic proc info to abilities.

### 1.7.3
Fixed math issue with items that have exponentially scaling cooldown reduction.

### 1.7.2
Quick fix for some calculations going in reverse if you exceed 100% chance

### 1.7.1
Big thanks to shirograhm for this update. Refactor of the backend for itemstats definitions/stats definitions to be much more readable. Adjusted definitions of some item stats to be more accurate.

### 1.7.0
Fixed equipment not always showing their full descriptions. Added option to adjust how long pickup display notifications last. Added difficultyCoefficient to StatsDisplay as an option. Made the item calculations for item stats optional.

### 1.6.3
Fixed healthPercentage not using the float precision option

### 1.6.2
Big thanks to shirograhm for most of these changes. Luck is gathered in a more accurate way now, should work better with modded items that add luck. Added a check for buff definitions to just use their name if no definition (name/description) was provided. Fixed equipment not utilizing ally's stats when looking in the scoreboard.

### 1.6.1
Forgot to push option for Buff Timer size

### 1.6.0
Added Buff Definitions, hover over your buffs to see them. Buff definitions have to be pre-defined, so modded buffs need to utilize BuffDefinitions.RegisterBuff in order for their buffs to show up (this can technically be done one either side if you know the names of buffs, but it's extremely tedius with how many mods are out there). Added a few new items to StatsDisplay (maxComboThisRun, currentCombatKills, maxKillCombo, maxKillComboThisRun). Added font scale options for Item Stats. Currently thinking about a setup where you can create as many StatsDisplay tabs as you want and move them around (such as having one as a dedicated DPS meter if you wanted to), but nothing to show for it yet, look forward to it though!

### 1.5.2
Fixed a few bugs around command. Fixed a description conflict with Manuscript from mystic's items

### 1.5.1
Fixed sorting options for the equipment menus. Changed calculate function for ItemStats to use a CharacterMaster for better calculations in the future

### 1.5.0
Added sorting options to the command menu and scrapper menu. Added equip slot functionality for itemstats.

### 1.4.2
Fixed DPS meter not resetting if you leave while it still has values stored. Fixed transforming into specific mobs like a LesserWisp causing StatsDisplay to break until restart

### 1.4.1
Fixed StatsDisplay not working with RiskUI. Added an option for how many decimal points to be used for floating point values.

### 1.4.0
Fixed a rare bug with CleanHUD. Fixed issue with scrappers and other menu's scaling (as in we don't touch them anymore since it's not needed and makes stuff look wonky and small). Fixed buff timers not showing up all the time. We actually use the setting for item counters now. Probably fixed an issue with Minimap and Refightilization (but it was inconsistent so I'll be on the lookout). Updated StatsDisplay to scale better automatically, also supports manual scaling now (thanks yuukotf2 on github!). Added an option for full item descriptions on pickup (thanks SSM240 on github!). New option to show permanent cooldown numbers for skills too. Added a secondary StatsDisplay string which, when enabled, will be used when the scoreboard is held open.

### 1.3.2
Last update was fake, this one is real a cool, not cringe

### 1.3.1
Fixed issue with buff timers applying to all buffs. Added a few new items to the StatsDisplay syntax. Added an option for StatsDisplay to remove the default color formatting.

### 1.3.0
Added buff timers. Added option to show hidden items. Added permanent equip item timers. Added option for global resizing of StatsDisplay text size

### 1.2.6
I suck at math

### 1.2.5
I forgot but it probably didn't work

### 1.2.4
Fixed luck based calculations. Fixed command windows with controllers (again)

### 1.2.3
Fixed UI padding for the RiskUI section

### 1.2.2
Fixed resized command window behaving poorly with controllers

### 1.2.1
Fixed item sorting when only sorting scrap and nothing else. Added critWithLuck and bleedChanceWithLuck to the stats display stuff, which as stated, factors in your luck

### 1.2.0
Added item stats on ping. Added item counters. Fixed the armor coloring being cIsUtility instead of cIsHealing

### 1.1.0
Number go up

### 1.0.0
Initial release
