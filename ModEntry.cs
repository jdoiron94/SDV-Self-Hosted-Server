using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

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
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!config.debug && Context.IsWorldReady)
            {
                int players = Game1.otherFarmers.Count;
                Monitor.Log($"One second tick method, players: {players}", LogLevel.Debug);
                if (clients != players)
                {
                    Monitor.Log($"OneSecondUpdateTicked event: [clients] => {clients}", LogLevel.Debug);
                    clients = players;
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
            if (Context.IsWorldReady && !Game1.paused && !asleep && e.NewTime >= 610)
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