using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.IO;

namespace SelfHostedServer
{
    public class ModEntry : Mod
    {

        private int clients;
        private int bedX;
        private int bedY;
        private int titleMenuTicks = 30;

        private bool asleep;

        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Game1.activeClickableMenu is ShippingMenu)
            {
                Monitor.Log($"SavingEvent event: [Game1.activeClickableMenu] => {Game1.activeClickableMenu}", LogLevel.Debug);
                Helper.Reflection.GetMethod(Game1.activeClickableMenu, "okClicked").Invoke();
            }
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                int players = Game1.otherFarmers.Count;
                if (clients != players)
                {
                    clients = players;
                    Monitor.Log($"OneSecondUpdateTicked event: [clients] => {clients}", LogLevel.Debug);
                    if (players >= 1)
                    {
                        Game1.paused = false;
                    }
                    else
                    {
                        Game1.paused = true;
                    }
                }
                else if (players == 0 && !Game1.paused)
                {
                    Monitor.Log($"OneSecondUpdateTicked event: [clients] => {clients}", LogLevel.Debug);
                    Game1.paused = true;
                }
            }
            else
            {
                if (Game1.activeClickableMenu is TitleMenu)
                {
                    if (titleMenuTicks <= 0)
                    {
                        Monitor.Log($"OneSecondUpdateTicked event: [Game1.activeClickableMenu] => {Game1.activeClickableMenu}", LogLevel.Debug);
                        if (!Directory.Exists(Path.Combine(Constants.SavesPath, config.SaveData.FileName)))
                        {
                            Monitor.Log($"OneSecondUpdateTicked event: Save file does not exist ({config.SaveData.FileName})", LogLevel.Warn);
                        }
                        else
                        {
                            if (config.SaveData.Multiplayer)
                            {
                                Game1.multiplayerMode = 2;
                            }
                            SaveGame.Load(config.SaveData.FileName);
                            titleMenuTicks = 30;
                            Monitor.Log($"OneSecondUpdateTicked event: Loaded save file => {config.SaveData.FileName}", LogLevel.Debug);
                            if (Game1.activeClickableMenu is TitleMenu)
                            {
                                Game1.activeClickableMenu.exitThisMenu(false);
                            }
                        }
                    }
                    else
                    {
                        titleMenuTicks -= 1;
                    }
                }
            }
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Context.IsWorldReady && !Game1.paused && !asleep && e.NewTime >= 700)
            {
                Monitor.Log($"TimeChanged event: [TimeChangedEventArgs.NewTime] => {e.NewTime}", LogLevel.Debug);
                Sleep();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            asleep = false;
            Monitor.Log($"DayStarted event: [asleep] => {asleep}", LogLevel.Debug);
        }

        private void Sleep()
        {
            asleep = true;
            int houseUpgradeLevel = Game1.player.HouseUpgradeLevel;
            if (houseUpgradeLevel == 0)
            {
                bedX = 9;
                bedY = 9;
            }
            else if (houseUpgradeLevel == 1)
            {
                bedX = 21;
                bedY = 4;
            }
            else
            {
                bedX = 27;
                bedY = 13;
            }
            Game1.warpFarmer("Farmhouse", bedX, bedY, false);
            Helper.Reflection.GetMethod(Game1.currentLocation, "startSleep").Invoke();
        }
    }
}