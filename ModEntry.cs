using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SelfHostedServer
{
    public class ModEntry : Mod
    {

        private int clients;
        private int bedX;
        private int bedY;

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